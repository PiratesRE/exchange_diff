using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.MessageDepot;

namespace Microsoft.Exchange.Transport
{
	internal sealed class MessageDepotComponent : IMessageDepotComponent, ITransportComponent, IDiagnosable
	{
		public void SetLoadTimeDependencies(MessageDepotConfig msgDepotConfig)
		{
			ArgumentValidator.ThrowIfNull("msgDepotConfig", msgDepotConfig);
			this.msgDepotConfig = msgDepotConfig;
		}

		public void Load()
		{
			if (!this.msgDepotConfig.IsMessageDepotEnabled)
			{
				return;
			}
			this.messageDepot = new MessageDepot(null, new TimeSpan?(this.msgDepotConfig.DelayNotificationTimeout));
			this.refreshTimer = new GuardedTimer(new TimerCallback(this.TimedUpdate), null, MessageDepotComponent.RefreshTimeInterval);
		}

		public void Unload()
		{
			if (!this.msgDepotConfig.IsMessageDepotEnabled)
			{
				return;
			}
			this.refreshTimer.Dispose(false);
			this.messageDepot = null;
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public string GetDiagnosticComponentName()
		{
			return "MessageDepot";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(this.GetDiagnosticComponentName());
			bool flag = parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("config", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag3 = parameters.Argument.IndexOf("showCounts", StringComparison.OrdinalIgnoreCase) != -1;
			if (flag2)
			{
				xelement.Add(TransportAppConfig.GetDiagnosticInfoForType(this.msgDepotConfig));
			}
			if (flag3)
			{
				foreach (object obj in Enum.GetValues(typeof(MessageDepotItemStage)))
				{
					int num = (int)obj;
					XElement xelement2 = new XElement(Enum.GetName(typeof(MessageDepotItemStage), num));
					foreach (object obj2 in Enum.GetValues(typeof(MessageDepotItemState)))
					{
						int num2 = (int)obj2;
						long count = this.messageDepot.GetCount((MessageDepotItemStage)num, (MessageDepotItemState)num2);
						if (count > 0L)
						{
							XElement content = new XElement(Enum.GetName(typeof(MessageDepotItemState), num2), count);
							xelement2.Add(content);
						}
					}
					xelement.Add(xelement2);
				}
			}
			if (flag)
			{
				xelement.Add(new XElement("help", "Supported arguments: config, showCounts, help"));
			}
			return xelement;
		}

		public IMessageDepot MessageDepot
		{
			get
			{
				return this.messageDepot;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.msgDepotConfig.IsMessageDepotEnabled;
			}
		}

		private void TimedUpdate(object state)
		{
			if (this.messageDepot != null)
			{
				this.messageDepot.TimedUpdate();
			}
		}

		private static readonly TimeSpan RefreshTimeInterval = TimeSpan.FromMinutes(1.0);

		private MessageDepot messageDepot;

		private GuardedTimer refreshTimer;

		private MessageDepotConfig msgDepotConfig;
	}
}
