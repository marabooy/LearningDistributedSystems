namespace Core;

using Akka.Actor;
using Akka.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// This sets up the nodes.
/// </summary>
public abstract partial class Experiment : IHostedService
{
    private ActorSystem _actorSystem;
    private readonly IServiceProvider _serviceProvider;

    private readonly IHostApplicationLifetime _applicationLifetime;

    public Experiment(IServiceProvider sp, IHostApplicationLifetime applicationLifetime)
    {
        _serviceProvider = sp;
        _applicationLifetime = applicationLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var bootstrap = BootstrapSetup.Create();


        // enable DI support inside this ActorSystem, if needed
        var diSetup = DependencyResolverSetup.Create(_serviceProvider);

        // merge this setup (and any others) together into ActorSystemSetup
        var actorSystemSetup = bootstrap.And(diSetup);

        // start ActorSystem
        _actorSystem = ActorSystem.Create("headless-service", actorSystemSetup);


        // add a continuation task that will guarantee shutdown of application if ActorSystem terminated.
        await StartExperimentAsync(_actorSystem);

        _actorSystem.WhenTerminated.ContinueWith(tr =>
       {
           _applicationLifetime.StopApplication();
       });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // strictly speaking this may not be necessary - terminating the ActorSystem would also work
        // but this call guarantees that the shutdown of the cluster is graceful regardless
        await CoordinatedShutdown.Get(_actorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
    }

    public abstract Task StartExperimentAsync(ActorSystem actorSystem);
}
