using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MultithreadedEncryptionApp
    {
        public partial class MainWindow : Window
        {
        private static CancellationTokenSource cancellationTokenSource;
        private static List<Task<string>> encryptionTasks = new List<Task<string>>();
            private string originalText;
            private int i;
         List<string> encryptedTexts= new List<string>();

        public MainWindow()
            {
                InitializeComponent();

                EncryptButton.Click += EncryptButton_Click;
                DecryptButton.Click += DecryptButton_Click;
                CancelButton.Click += CancelButton_Click; 

            }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
                ExecuteEncryptionTask(EncryptionMethod.TPLParallel);
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            ExecuteDecryptionTask(encryptedTexts);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
            {
                CancelEncryptionTask();
            }

        private void ExecuteEncryptionTask(EncryptionMethod encryptionMethod)
        {
            EncryptedTextBox.Text = string.Empty;  // Limpiar el contenido actual de EncryptedTextBox

            // Obtener el texto de OriginalTextBox
            originalText = OriginalTextBox.Text;
            cancellationTokenSource = new CancellationTokenSource();

            int numberOfTasks = Environment.ProcessorCount;
            List<string> substrings = DivideTextIntoSubstrings(originalText, numberOfTasks);


            switch (encryptionMethod)
            {
                case EncryptionMethod.TPLParallel:
                    ExecuteTPLParallelEncryption(substrings);
                    break;
                case EncryptionMethod.Parallel:
                    ExecuteParallelEncryption(substrings);
                    break;
                case EncryptionMethod.Async:
                    ExecuteAsyncEncryption(substrings);
                    break;
            }


        }


        private List<string> ExecuteTPLParallelEncryption(List<string> substrings)
        {
            cancellationTokenSource = new CancellationTokenSource();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<Task<List<string>>> encryptionTasks = new List<Task<List<string>>>();

            try
            {
                // Utilizar TaskFactory para crear tareas de encriptación
                TaskFactory<List<string>> taskFactory = new TaskFactory<List<string>>(TaskCreationOptions.AttachedToParent, TaskContinuationOptions.None);

                foreach (var substring in substrings)
                {
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                        break;

                    // Utilizar TaskFactory.StartNew para encriptar cada subcadena de forma paralela
                    Task<List<string>> encryptionTask = taskFactory.StartNew(() => Encrypt(new List<string> { substring }));
                    encryptionTasks.Add(encryptionTask);

                    // Actualizar la interfaz de usuario con el texto encriptado
                    EncryptedTextBox.Text += string.Join("", encryptionTask.Result);
                }
            }
            finally
            {
                stopwatch.Stop();

                // Mostrar el tiempo de ejecución en la interfaz de usuario
                ExecutionTimeTextBox.Text = $"{stopwatch.Elapsed.TotalMilliseconds} ms";
            }

            // Obtener los resultados de las tareas de encriptación cuando todas estén completas
            encryptedTexts = (Task.WhenAll(encryptionTasks).Result).SelectMany(x => x).ToList();
            return encryptedTexts;
        }



        void ExecuteDecryptionTask(List<string> encryptedMessages)
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var decryptedMessages = Decrypt(encryptedMessages);

            stopwatch.Stop();
            double elapsedTime = stopwatch.Elapsed.TotalMilliseconds;

            DecryptedTextBox.Text = string.Join("", decryptedMessages);
            ExecutionTimeTextBox.Text = $"{elapsedTime} ms";
        }

        private List<string> ExecuteParallelEncryption(List<string> substrings)
        {
            cancellationTokenSource = new CancellationTokenSource();
           

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            Parallel.ForEach(substrings, (substring, state) =>
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    state.Stop();
                    return;
                }
                string encryptedSubstring = Encrypt(new List<string> { substring })[0];

                    encryptedTexts.Add(encryptedSubstring);

            });

            stopwatch.Stop();

            // Actualizar la interfaz de usuario después de completar la encriptación
            Dispatcher.Invoke(() =>
            {
                EncryptedTextBox.Text = string.Join("", encryptedTexts);
                ExecutionTimeTextBox.Text = $"{stopwatch.Elapsed.TotalMilliseconds} ms";
            });

            return encryptedTexts;
        }







        private async Task<List<string>> ExecuteAsyncEncryption(List<string> substrings)
        {
            // Limpiar resultados anteriores
            EncryptedTextBox.Text = string.Empty;

            // Restablecer la fuente de cancelación
            cancellationTokenSource = new CancellationTokenSource();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<Task<List<string>>> encryptionTasks = new List<Task<List<string>>>();

            try
            {
                foreach (var substring in substrings)
                {
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                        break;

                    // Utilizar Task.Run para encriptar cada subcadena de forma paralela
                    Task<List<string>> encryptionTask = Task.Run(() => Encrypt(new List<string> { substring }));
                    encryptionTasks.Add(encryptionTask);

                    // Actualizar la interfaz de usuario con el texto encriptado
                    EncryptedTextBox.Text += string.Join("", await encryptionTask);
                }
            }
            finally
            {
                stopwatch.Stop();

                // Mostrar el tiempo de ejecución en la interfaz de usuario
                ExecutionTimeTextBox.Text = $"{stopwatch.Elapsed.TotalMilliseconds} ms";
            }

            // Obtener los resultados de las tareas de encriptación cuando todas estén completas
            encryptedTexts = (await Task.WhenAll(encryptionTasks)).SelectMany(x => x).ToList();
            return encryptedTexts;
        }





        private static List<string> Encrypt(List<string> textsToEncrypt)
        {
            var encryptedTexts = new List<string>();

            for (int i = 0; i < textsToEncrypt.Count; i++)
            {
                string textToEncrypt = textsToEncrypt[i];
                var encryptedTextBuilder = new StringBuilder();

                for (int j = 0; j < textToEncrypt.Length; j++)
                {
                    char originalChar = textToEncrypt[j];
                    char randomChar = (char)(128 - j);

                    char encryptedChar = (char)(originalChar ^ randomChar);

                    encryptedTextBuilder.Append(encryptedChar);
                    encryptedTextBuilder.Append(randomChar);
                }

                encryptedTexts.Add(encryptedTextBuilder.ToString());
            }

            return encryptedTexts;
        }


        static List<string> DivideTextIntoSubstrings(string text, int numberOfSubstrings)
        {
            List<string> substrings = new List<string>();
            int substringLength = text.Length / numberOfSubstrings;

            for (int i = 0; i < numberOfSubstrings; i++)
            {
                int startIndex = i * substringLength;
                int length = i == numberOfSubstrings - 1 ? text.Length - startIndex : substringLength;
                string substring = text.Substring(startIndex, length);
                substrings.Add(substring);
            }

            return substrings;
        }
    

    private void CancelEncryptionTask()
        {
            // Cancela la fuente de cancelación
            cancellationTokenSource?.Cancel();
        }


        private static List<string> Decrypt(List<string> textsToDecrypt)
        {
            var decryptedTexts = new List<string>();

            foreach (var textToDecrypt in textsToDecrypt)
            {
                StringBuilder decryptedTextBuilder = new StringBuilder();

                for (int i = 0; i < textToDecrypt.Length; i += 2)
                {
                    char encryptedChar = textToDecrypt[i];
                    char randomChar = textToDecrypt[i + 1];

                    char decryptedChar = (char)(encryptedChar ^ randomChar);

                    decryptedTextBuilder.Append(decryptedChar);
                }

                decryptedTexts.Add(decryptedTextBuilder.ToString());
            }

            return decryptedTexts;
        }


        private enum EncryptionMethod
            {
                TPLParallel,
                Parallel,
                Async
            }
        }
    }

