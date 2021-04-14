using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Delivery
{
	internal class DeliveryAgentConnectionHandler : IStartableTransportComponent, ITransportComponent, IDiagnosable
	{
		public DeliveryAgentConnectionHandler()
		{
			this.connections = new List<DeliveryAgentConnection>();
		}

		public string CurrentState
		{
			get
			{
				return "Connection count=" + this.connections.Count;
			}
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
		}

		public void Stop()
		{
			lock (this.syncObject)
			{
				this.retire = true;
				this.allConnectionsRetired = new AutoResetEvent(this.connections.Count == 0);
				foreach (DeliveryAgentConnection deliveryAgentConnection in this.connections)
				{
					deliveryAgentConnection.Retire();
				}
			}
			this.allConnectionsRetired.WaitOne();
		}

		public void Pause()
		{
		}

		public void Continue()
		{
		}

		public void Load()
		{
			this.mexEvents = new DeliveryAgentMExEvents();
			this.mexEvents.Initialize(Path.Combine(ConfigurationContext.Setup.InstallPath, "TransportRoles\\Shared\\agents.config"));
		}

		public void Unload()
		{
			this.mexEvents.Shutdown();
			this.mexEvents = null;
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void HandleConnection(NextHopConnection nextHopConnection)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.HandleConnectionCallback), nextHopConnection);
		}

		private void HandleConnectionCallback(object connection)
		{
			DeliveryAgentConnection deliveryAgentConnection = null;
			lock (this.syncObject)
			{
				if (this.retire)
				{
					return;
				}
				deliveryAgentConnection = new DeliveryAgentConnection((NextHopConnection)connection, this.mexEvents);
				this.connections.Add(deliveryAgentConnection);
			}
			deliveryAgentConnection.BeginConnection(deliveryAgentConnection, new AsyncCallback(this.ConnectionCompleted));
		}

		private void ConnectionCompleted(IAsyncResult ar)
		{
			DeliveryAgentConnection deliveryAgentConnection = (DeliveryAgentConnection)ar.AsyncState;
			deliveryAgentConnection.EndConnection(ar);
			lock (this.syncObject)
			{
				this.connections.Remove(deliveryAgentConnection);
				if (this.retire && this.connections.Count == 0)
				{
					this.allConnectionsRetired.Set();
				}
			}
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "DeliveryAgents";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
			if (this.mexEvents != null)
			{
				xelement.Add(this.mexEvents.GetDiagnosticInfo(parameters));
			}
			return xelement;
		}

		private DeliveryAgentMExEvents mexEvents;

		private List<DeliveryAgentConnection> connections;

		private bool retire;

		private AutoResetEvent allConnectionsRetired;

		private object syncObject = new object();
	}
}
