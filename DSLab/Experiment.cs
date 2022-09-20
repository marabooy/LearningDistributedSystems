namespace Core;

using Akka.Actor;
using Akka.DependencyInjection;

/// <summary>
/// This sets up the nodes.
/// </summary>
public partial class Experiment
{
    private readonly float networkReliability;
    private readonly DependencyResolverSetup dependencyResolver;
    private ActorSystem actorSystem;

    public Experiment(IServiceProvider serviceProvider, float networkReliability)
    {
        this.networkReliability = networkReliability;
        this.dependencyResolver = DependencyResolverSetup.Create(serviceProvider);
    }

    public async Task StartAsync()
    {
        var actorSystemSetup = BootstrapSetup.Create().And(dependencyResolver);
        this.actorSystem = ActorSystem.Create("dependencyResolver-experiments", actorSystemSetup);
    }
}
