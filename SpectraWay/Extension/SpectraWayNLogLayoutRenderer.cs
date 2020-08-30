using System;
using System.Text;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Layouts;
using NLog.Targets;

namespace SpectraWay.Extension
{
    [LayoutRenderer("spectra-way-layout")]
    public class SpectraWayNLogLayoutRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append($"[{DateTime.Now.ToString("HH:mm:ss")}] ");
            builder.Append(logEvent.Level);
            builder.Append(" | ");
            if (logEvent.Parameters != null && logEvent.Parameters.Length > 0)
            {
                builder.Append(string.Format(logEvent.Message, logEvent.Parameters));
            }
            else
            {
                builder.Append(logEvent.Message);
            }
            builder.AppendLine();
        }
    }

    [Target("spectra-way-target")]
    public sealed class SpectraWayNLogTarget : TargetWithLayout
    {
        public SpectraWayNLogTarget()
        {
            this.Host = "localhost";
            //this.Layout = new SimpleLayout(new SpectraWayNLogLayoutRenderer());
        }

        [RequiredParameter]
        public string Host { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = this.Layout.Render(logEvent);

            SendTheMessageToRemoteHost(this.Host, logMessage);
        }

        private void SendTheMessageToRemoteHost(string host, string message)
        {
            var e = new NewMessageRecievedEventHandlerArgs(message);
            NewMessageRecieved?.Invoke(typeof(SpectraWayNLogTarget), e);
        }

        public static  event NewMessageRecievedEventHandler NewMessageRecieved;
    }

    //public interface INotifyLogger
    //{
    //    event NewMessageRecievedEventHandler NewMessageRecieved;
    //}

    public delegate void NewMessageRecievedEventHandler(object sender, NewMessageRecievedEventHandlerArgs args);

    public class NewMessageRecievedEventHandlerArgs
    {
        public NewMessageRecievedEventHandlerArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}