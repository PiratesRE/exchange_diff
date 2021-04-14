using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal sealed class SystemCheckComponent : ISystemCheckComponent, ITransportComponent
	{
		public SystemCheckComponent()
		{
		}

		internal SystemCheckComponent(ISystemCheck diskSystemCheck)
		{
			ArgumentValidator.ThrowIfNull("diskSystemCheck", diskSystemCheck);
			this.diskSystemCheck = diskSystemCheck;
		}

		public void SetLoadTimeDependencies(SystemCheckConfig systemCheckConfig, TransportAppConfig transportAppConfig, ITransportConfiguration transportConfiguration)
		{
			ArgumentValidator.ThrowIfNull("systemCheckConfig", systemCheckConfig);
			this.systemCheckConfig = systemCheckConfig;
			ArgumentValidator.ThrowIfNull("transportAppConfig", transportAppConfig);
			this.transportAppConfig = transportAppConfig;
			ArgumentValidator.ThrowIfNull("transportConfiguration", transportConfiguration);
			this.transportConfiguration = transportConfiguration;
		}

		public void Load()
		{
			if (!this.systemCheckConfig.IsSystemCheckEnabled)
			{
				return;
			}
			if (this.systemCheckConfig.IsDiskSystemCheckEnabled)
			{
				if (this.diskSystemCheck == null)
				{
					this.diskSystemCheck = new DiskSystemCheck(this.systemCheckConfig, this.transportAppConfig, this.transportConfiguration);
				}
				this.diskSystemCheck.Check();
			}
		}

		public void Unload()
		{
			if (!this.systemCheckConfig.IsSystemCheckEnabled)
			{
				return;
			}
			this.diskSystemCheck = null;
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public ISystemCheck DiskSystemCheck
		{
			get
			{
				return this.diskSystemCheck;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.systemCheckConfig.IsSystemCheckEnabled;
			}
		}

		private SystemCheckConfig systemCheckConfig;

		private TransportAppConfig transportAppConfig;

		private ITransportConfiguration transportConfiguration;

		private ISystemCheck diskSystemCheck;
	}
}
