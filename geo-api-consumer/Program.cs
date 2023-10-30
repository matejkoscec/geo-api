using Microsoft.AspNetCore.SignalR.Client;


var connection = new HubConnectionBuilder()
    .WithUrl("wss://localhost:7113/hub/v1/locations/nearby-search")
    .WithAutomaticReconnect()
    .Build();

connection.On<string>("ReceiveMessage", Console.WriteLine);

connection.Reconnecting += _ =>
{
    Console.WriteLine("Connection lost. Reconnecting...");
    return Task.CompletedTask;
};

try
{
    Console.WriteLine("Connecting to NearbySearchHub...");
    await connection.StartAsync();
    Console.WriteLine("Press 'q' to disconnect.");

    while (Console.ReadKey().Key != ConsoleKey.Q)
    {
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex}");
}
finally
{
    Console.WriteLine("\nClosing connection...");
    await connection.StopAsync();
}