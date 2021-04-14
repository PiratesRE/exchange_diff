using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal abstract class TraceConfigurationBase
	{
		public bool IsUpdateNeeded
		{
			get
			{
				return this.exTraceConfigVersion != ExTraceConfiguration.Instance.Version;
			}
		}

		public bool FilteredTracingEnabled
		{
			get
			{
				return this.exTraceConfiguration.PerThreadTracingConfigured;
			}
		}

		public void Load(ExTraceConfiguration exTraceConfiguration)
		{
			this.exTraceConfiguration = exTraceConfiguration;
			this.exTraceConfigVersion = this.exTraceConfiguration.Version;
			this.OnLoad();
		}

		public abstract void OnLoad();

		protected ExTraceConfiguration exTraceConfiguration;

		protected int exTraceConfigVersion;
	}
}
