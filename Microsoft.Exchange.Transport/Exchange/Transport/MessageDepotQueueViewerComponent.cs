using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.MessageDepot;

namespace Microsoft.Exchange.Transport
{
	internal sealed class MessageDepotQueueViewerComponent : IMessageDepotQueueViewerComponent, ITransportComponent, IDiagnosable
	{
		public IMessageDepotQueueViewer MessageDepotQueueViewer
		{
			get
			{
				return this.messageDepotQueueViewer;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
		}

		public void SetLoadTimeDependencies(IMessageDepotComponent messageDepotComponent, TransportAppConfig.ILegacyQueueConfig queueConfig)
		{
			ArgumentValidator.ThrowIfNull("messageDepotComponent", messageDepotComponent);
			this.messageDepotComponent = messageDepotComponent;
			this.queueConfig = queueConfig;
		}

		public void Load()
		{
			this.enabled = this.messageDepotComponent.Enabled;
			if (this.enabled)
			{
				this.messageDepotQueueViewer = (IMessageDepotQueueViewer)this.messageDepotComponent.MessageDepot;
				this.msgDepotLegacyPerfCounterWrapper = new MsgDepotLegacyPerfCounterWrapper(this.messageDepotComponent.MessageDepot, this.messageDepotQueueViewer, this.queueConfig);
				this.refreshTimer = new GuardedTimer(new TimerCallback(this.TimedUpdate), null, MessageDepotQueueViewerComponent.RefreshTimeInterval);
			}
		}

		public void Unload()
		{
			if (!this.enabled)
			{
				return;
			}
			this.refreshTimer.Dispose(false);
			this.msgDepotLegacyPerfCounterWrapper = null;
			this.messageDepotQueueViewer = null;
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public string GetDiagnosticComponentName()
		{
			return "MessageDepotQueueViewer";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(this.GetDiagnosticComponentName());
			bool flag = parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("config", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag3 = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			if (flag3 && this.Enabled)
			{
				foreach (object obj in Enum.GetValues(typeof(MessageDepotItemStage)))
				{
					MessageDepotItemStage messageDepotItemStage = (MessageDepotItemStage)obj;
					foreach (object obj2 in Enum.GetValues(typeof(MessageDepotItemState)))
					{
						MessageDepotItemState messageDepotItemState = (MessageDepotItemState)obj2;
						long count = this.messageDepotQueueViewer.GetCount(messageDepotItemStage, messageDepotItemState);
						if (count > 0L)
						{
							XElement content = new XElement("Messages", new object[]
							{
								new XAttribute("Stage", messageDepotItemStage.ToString()),
								new XAttribute("State", messageDepotItemState.ToString()),
								new XAttribute("Count", count)
							});
							xelement.Add(content);
						}
					}
				}
			}
			if (flag2)
			{
				xelement.Add(new XElement("Enabled", this.Enabled));
			}
			if (flag)
			{
				xelement.Add(new XElement("help", "Supported arguments: verbose, config, help"));
			}
			return xelement;
		}

		private void TimedUpdate(object state)
		{
			if (this.msgDepotLegacyPerfCounterWrapper != null)
			{
				this.msgDepotLegacyPerfCounterWrapper.TimedUpdate();
			}
		}

		private static readonly TimeSpan RefreshTimeInterval = TimeSpan.FromMinutes(1.0);

		private IMessageDepotQueueViewer messageDepotQueueViewer;

		private MsgDepotLegacyPerfCounterWrapper msgDepotLegacyPerfCounterWrapper;

		private GuardedTimer refreshTimer;

		private bool enabled;

		private IMessageDepotComponent messageDepotComponent;

		private TransportAppConfig.ILegacyQueueConfig queueConfig;
	}
}
