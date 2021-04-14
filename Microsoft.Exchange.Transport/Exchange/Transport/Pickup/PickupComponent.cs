using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Pickup
{
	internal sealed class PickupComponent : IStartableTransportComponent, ITransportComponent
	{
		public PickupComponent(IPickupSubmitHandler submitHandler)
		{
			if (submitHandler == null)
			{
				throw new ArgumentException("Submission handler is not provided");
			}
			ExTraceGlobals.PickupTracer.TraceDebug((long)this.GetHashCode(), "Creating new Pickup Component");
			this.simpleDirectory = new PickupDirectory(PickupType.Pickup, submitHandler);
			this.replayDirectory = new PickupDirectory(PickupType.Replay, submitHandler);
		}

		public void Load()
		{
			this.RegisterConfigurationChangeHandlers();
		}

		public void Unload()
		{
			this.UnregisterConfigurationChangeHandlers();
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
			ExTraceGlobals.PickupTracer.TraceDebug((long)this.GetHashCode(), "Pickup starting.");
			this.targetRunningState = targetRunningState;
			this.paused = (initiallyPaused || !this.ShouldExecute());
			if (!this.paused)
			{
				this.simpleDirectory.Start();
				this.replayDirectory.Start();
			}
			ExTraceGlobals.PickupTracer.TraceDebug((long)this.GetHashCode(), "Pickup started.");
		}

		public void Stop()
		{
			ExTraceGlobals.PickupTracer.TraceDebug((long)this.GetHashCode(), "Pickup stopping.");
			this.simpleDirectory.Stop();
			this.replayDirectory.Stop();
			ExTraceGlobals.PickupTracer.TraceDebug((long)this.GetHashCode(), "Pickup stopped.");
		}

		public void Pause()
		{
			ExTraceGlobals.PickupTracer.TraceDebug((long)this.GetHashCode(), "Pickup pausing.");
			if (!this.paused)
			{
				this.paused = true;
				this.simpleDirectory.Stop();
				this.replayDirectory.Stop();
			}
			ExTraceGlobals.PickupTracer.TraceDebug((long)this.GetHashCode(), "Pickup paused.");
		}

		public void Continue()
		{
			ExTraceGlobals.PickupTracer.TraceDebug((long)this.GetHashCode(), "Pickup resuming.");
			if (this.paused && this.ShouldExecute())
			{
				this.paused = false;
				this.simpleDirectory.Start();
				this.replayDirectory.Start();
			}
			ExTraceGlobals.PickupTracer.TraceDebug((long)this.GetHashCode(), "Pickup resumed.");
		}

		public string CurrentState
		{
			get
			{
				return null;
			}
		}

		public void ConfigUpdate(object source, EventArgs args)
		{
			this.TransportServerConfigUpdate(null);
		}

		private void RegisterConfigurationChangeHandlers()
		{
			Components.ConfigChanged += this.ConfigUpdate;
			Components.Configuration.LocalServerChanged += this.TransportServerConfigUpdate;
		}

		private void UnregisterConfigurationChangeHandlers()
		{
			Components.ConfigChanged -= this.ConfigUpdate;
			Components.Configuration.LocalServerChanged -= this.TransportServerConfigUpdate;
		}

		private void Configure()
		{
			this.simpleDirectory.Reconfigure();
			this.replayDirectory.Reconfigure();
			ExTraceGlobals.PickupTracer.TraceDebug((long)this.GetHashCode(), "Pickup Reconfigured.");
		}

		private void TransportServerConfigUpdate(TransportServerConfiguration args)
		{
			if (!this.paused)
			{
				this.Configure();
			}
		}

		private bool ShouldExecute()
		{
			return this.targetRunningState == ServiceState.Active;
		}

		private PickupDirectory simpleDirectory;

		private PickupDirectory replayDirectory;

		private bool paused;

		private ServiceState targetRunningState;
	}
}
