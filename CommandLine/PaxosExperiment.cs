using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Util.Internal;
using CommandLine.Paxos;
using Core;
using Microsoft.Extensions.Hosting;

namespace CommandLine
{
    internal class PaxosExperiment : Experiment
    {
        public PaxosExperiment(IServiceProvider sp, IHostApplicationLifetime applicationLifetime) :
            base(sp, applicationLifetime)
        {
        }

        public override Task StartExperimentAsync(ActorSystem actorSystem)
        {
            string[] nodes = new string[] { "server1", "server2", "server3" };

            var servers = nodes.Select(address => (address, peers: nodes.Where(val => !val.Equals(address)).ToArray()))
                .Select(pair =>
                {
                    var provider = DependencyResolver.For(actorSystem);
                    var props = provider.Props<Server>(pair.address, pair.peers);
                    return actorSystem.ActorOf(props, pair.address);
                }).ToArray();
            return Task.CompletedTask;
        }
    }
}
