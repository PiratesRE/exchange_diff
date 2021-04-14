using System;
using System.Linq;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class OverrideEndpoint : IEndpoint
	{
		public bool RestartOnChange
		{
			get
			{
				return true;
			}
		}

		public Exception Exception { get; set; }

		public void Initialize()
		{
		}

		public bool DetectChange()
		{
			if (this.changed)
			{
				return this.changed;
			}
			if (LocalOverrideManager.IsLocalOverridesChanged())
			{
				this.changed = true;
			}
			if (DirectoryAccessor.Instance.IsGlobalOverridesChanged())
			{
				this.changed = true;
			}
			DateTime now = DateTime.UtcNow;
			if (ProbeDefinition.LocalOverrides != null && ProbeDefinition.LocalOverrides.Any((WorkDefinitionOverride o) => o.ExpirationDate < now))
			{
				this.changed = true;
			}
			if (ProbeDefinition.GlobalOverrides != null && ProbeDefinition.GlobalOverrides.Any((WorkDefinitionOverride o) => o.ExpirationDate < now))
			{
				this.changed = true;
			}
			if (MonitorDefinition.LocalOverrides != null && MonitorDefinition.LocalOverrides.Any((WorkDefinitionOverride o) => o.ExpirationDate < now))
			{
				this.changed = true;
			}
			if (MonitorDefinition.GlobalOverrides != null && MonitorDefinition.GlobalOverrides.Any((WorkDefinitionOverride o) => o.ExpirationDate < now))
			{
				this.changed = true;
			}
			if (ResponderDefinition.LocalOverrides != null && ResponderDefinition.LocalOverrides.Any((WorkDefinitionOverride o) => o.ExpirationDate < now))
			{
				this.changed = true;
			}
			if (ResponderDefinition.GlobalOverrides != null && ResponderDefinition.GlobalOverrides.Any((WorkDefinitionOverride o) => o.ExpirationDate < now))
			{
				this.changed = true;
			}
			return this.changed;
		}

		private bool changed;
	}
}
