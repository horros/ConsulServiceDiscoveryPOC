using System.Reflection;
using Consul;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace HelloService;

public sealed class ConsulBackgroundService : IHostedService, IDisposable
{
    private readonly ConsulClient _consulClient = new();
    private readonly IServer _server;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private AgentServiceRegistration? _registration;

    public ConsulBackgroundService(IServer server, IHostApplicationLifetime hostApplicationLifetime)
    {
        _server = server;
        _hostApplicationLifetime = hostApplicationLifetime;
    }
    

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _hostApplicationLifetime.ApplicationStarted.Register( () =>
        {
            // Get server IP address
            var address = _server.Features.Get<IServerAddressesFeature>()?.Addresses.First();
        
            // Register service with consul
            if (address != null)
            {
                var uri = new Uri(address);

                var name = Assembly.GetAssembly(typeof(Program)).GetName().Name;
                _registration = new AgentServiceRegistration()
                {
                    
                    ID = $"{name}-{uri.Port}",
                    Name = name,
                    Address = $"{uri.Scheme}://{uri.Host}",
                    Port = uri.Port,
                    Tags = new[] { "HelloService", "Service", "Test", "Infrastructure" }
                };
            }
            _consulClient.Agent.ServiceRegister(_registration, cancellationToken).Wait(cancellationToken);
        });

        return Task.CompletedTask;

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_registration != null)
            _consulClient.Agent.ServiceDeregister(_registration.ID, cancellationToken).Wait(cancellationToken);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _consulClient.Dispose();
        _server.Dispose();
    }
}
