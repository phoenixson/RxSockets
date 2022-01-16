# RxSockets&nbsp;&nbsp; [![Build status](https://ci.appveyor.com/api/projects/status/rfxxbpx2agq8r93n?svg=true)](https://ci.appveyor.com/project/dshe/RxSockets) [![NuGet](https://img.shields.io/nuget/vpre/RxSockets.svg)](https://www.nuget.org/packages/RxSockets/) [![NuGet](https://img.shields.io/nuget/dt/RxSockets?color=orange)](https://www.nuget.org/packages/RxSockets/) [![License](https://img.shields.io/badge/license-Apache%202.0-7755BB.svg)](https://opensource.org/licenses/Apache-2.0)
***Minimal Reactive Socket Implementation***
- **.NET 6.0** library
- connect: *asynchronous*
- send: *synchronous*
- receive: *observable* or *async enumerable*
- accept:  *observable* or *async enumerable*
- simple and intuitive API
- fast
- dependencies: System.Reactive, System.Linq.Async, Microsoft.Extensions.Logging

### installation
```csharp
PM> Install-Package RxSockets
```
### example
```csharp
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;
using RxSockets;
```
#### server
```csharp
interface IRxSocketServer : IAsyncDisposable
{
    IPEndPoint IPEndPoint { get; }
    IAsyncEnumerable<IRxSocketClient> AcceptAllAsync();
}
```
```csharp
// Create a server using an available port on the local machine.
IRxSocketServer server = RxSocketServer.Create();

// Prepare to start accepting connections from clients.
server
    .AcceptAllAsync()
    .ToObservableFromAsyncEnumerable()
    .Subscribe(onNext: acceptClient =>
    {
        // After the server accepts a client connection,
        // start receiving messages from the client and ...
        acceptClient
            .ReceiveAllAsync()
            .ToObservableFromAsyncEnumerable()
            .ToStrings()
            .Subscribe(onNext: message =>
            {
                // echo each message received back to the client.
                acceptClient.Send(message.ToByteArray());
            });
    });
```
#### client
```csharp
interface IRxSocketClient : IAsyncDisposable
{
    bool Connected { get; }
    int Send(ReadOnlySpan<byte> buffer);
    IAsyncEnumerable<byte> ReceiveAllAsync();
}
```
```csharp
// Create a client by connecting to the IPEndPoint of the server.
IRxSocketClient client = await server.IPEndPoint.CreateRxSocketClientAsync();

// Send the message "Hello!" to the server,
// which the server will then echo back to the client.
client.Send("Hello!".ToByteArray());

// Receive the message from the server.
string message = await client.ReceiveAllAsync().ToStrings().FirstAsync();
Assert.Equal("Hello!", message);

await client.DisposeAsync();
await server.DisposeAsync();
```
### notes
The extension method ```ToObservableFromAsyncEnumerable()``` may be used to create observables from the async enumerables ```IRxSocketClient.ReceiveAllAsync()``` and ```IRxSocketServer.AcceptAllAsync()```.

```Observable.Publish()[.RefCount() | .AutoConnect()]``` may be used to support multiple simultaneous observers.

To communicate using strings (see example above), the following extension methods are provided:
```csharp
byte[] ToByteArray(this string source);
byte[] ToByteArray(this IEnumerable<string> source)

IEnumerable<string>      ToStrings(this IEnumerable<byte> source)
IAsyncEnumerable<string> ToStrings(this IAsyncEnumerable<byte> source)
IObservable<string>      ToStrings(this IObservable<byte> source)
```
