# MultithreadedEncryptionApp

Aplicación de escritorio WPF en C# para experimentar y comparar el rendimiento de diferentes técnicas de paralelismo y asincronía en la encriptación y desencriptación de texto.

## Características

- **Encriptación y desencriptación de texto** usando un algoritmo personalizado.
- **Comparación de rendimiento** entre varios métodos de procesamiento:
  - TPL (Task Parallel Library)
  - Parallel.ForEach
  - Async/Await
- **Soporte para cancelación** de operaciones largas.
- **División automática del trabajo** según el número de núcleos del procesador.
- **Interfaz gráfica intuitiva** para introducir texto, seleccionar el método y visualizar resultados.
- **Medición y visualización del tiempo de ejecución** de cada técnica.
- **Gestión de errores y notificaciones** al usuario.

## Uso

1. Introduce el texto que deseas encriptar o desencriptar.
2. Selecciona el método de procesamiento deseado.
3. Haz clic en "Encrypt" para encriptar o "Decrypt" para desencriptar el texto.
4. Puedes cancelar la operación en cualquier momento usando el botón "Cancel".
5. El resultado y el tiempo de ejecución aparecerán en pantalla.

## Requisitos

- .NET 6.0 o superior
- Windows

## Instalación

1. Clona este repositorio.
2. Abre la solución `MultithreadedEncryptionApp.sln` en Visual Studio.
3. Restaura los paquetes NuGet si es necesario.
4. Compila y ejecuta la aplicación.

## Licencia

MIT
# MultithreadedEncryptionApp

Aplicación de escritorio WPF en C# para experimentar y comparar el rendimiento de diferentes técnicas de paralelismo y asincronía en la encriptación y desencriptación de texto.

## Características

- **Encriptación y desencriptación de texto** usando un algoritmo personalizado.
- **Comparación de rendimiento** entre varios métodos de procesamiento:
  - TPL (Task Parallel Library)
  - Parallel.ForEach
  - Async/Await
- **Soporte para cancelación** de operaciones largas.
- **División automática del trabajo** según el número de núcleos del procesador.
- **Interfaz gráfica intuitiva** para introducir texto, seleccionar el método y visualizar resultados.
- **Medición y visualización del tiempo de ejecución** de cada técnica.
- **Gestión de errores y notificaciones** al usuario.

## Uso

1. Introduce el texto que deseas encriptar o desencriptar.
2. Selecciona el método de procesamiento deseado.
3. Haz clic en "Encrypt" para encriptar o "Decrypt" para desencriptar el texto.
4. Puedes cancelar la operación en cualquier momento usando el botón "Cancel".
5. El resultado y el tiempo de ejecución aparecerán en pantalla.

## Requisitos

- .NET 6.0 o superior
- Windows

## Instalación

1. Clona este repositorio.
2. Abre la solución `proyectPP_app.sln` en Visual Studio.
3. Restaura los paquetes NuGet si es necesario.
4. Compila y ejecuta la aplicación.

## Licencia

MIT
