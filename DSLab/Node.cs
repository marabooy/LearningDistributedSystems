using Akka.Actor;

namespace Core
{
    public abstract partial class Node : UntypedActor, IWithTimers
    {
        public ITimerScheduler Timers { get; set; }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case ITimer timer:
                    ReceiveTimer(timer);
                    return;
                case IResult response:
                    ReceiveResponse(response, GetSender());
                    return;
                case IRequest request:
                    HandleRequest(request, GetSender());
                    return;
            }
        }

        protected abstract void HandleRequest(IRequest request, string v);

        private string GetSender()
        {
            return Sender.Path.Elements[Sender.Path.Elements.Count - 1];
        }


        private string GetSelf()
        {
            return Sender.Path.Elements[Self.Path.Elements.Count - 1];
        }

        protected void SendToPeer(string peerId, object message)
        {
            var selection = Context.ActorSelection($"../{peerId}");
            selection.Tell(message);
        }

        private void BroadCastToPeers(object message, params string[] peers)
        {
            foreach (var peer in peers)
            {
                this.SendToPeer(peer, message);
            }
        }

        public void AddTimer(ITimer timer, int millis)
        {
            this.Timers.StartSingleTimer(timer, timer, TimeSpan.FromMilliseconds(millis));
        }

        /// <summary>
        /// Processes messages from other senders
        /// </summary>
        /// <param name="message"></param>
        protected abstract void ReceiveResponse(IResult message, string sender);

        public abstract void ReceiveTimer(ITimer timer);
    }
}