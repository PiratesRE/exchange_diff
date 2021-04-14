using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class ProbeIdentity : WorkItemIdentity.Typed<ProbeDefinition>
	{
		private ProbeIdentity(Component component, string baseName, string targetResource) : base(component, WorkItemIdentity.ToLocalName(baseName, "Probe"), targetResource)
		{
			this.baseName = baseName;
		}

		public static implicit operator ProbeIdentity(ProbeDefinition definition)
		{
			if (definition == null)
			{
				return null;
			}
			Component component = ExchangeComponent.WellKnownComponents[definition.ServiceName];
			return new ProbeIdentity(component, WorkItemIdentity.GetLocalName(component, definition.Name, "Probe"), definition.TargetResource);
		}

		public static ProbeIdentity Create(Component healthSet, ProbeType probeType, string scenario = null, string targetResource = null)
		{
			string subsetName = healthSet.SubsetName;
			if (((probeType == ProbeType.DeepTest || probeType == ProbeType.SelfTest) && subsetName != "Protocol") || (probeType == ProbeType.ProxyTest && subsetName != "Proxy") || (probeType == ProbeType.Ctp && healthSet.HealthGroup != HealthGroup.CustomerTouchPoints))
			{
				throw new ArgumentException(string.Format("Probes of type {0} cannot be declared on \"{1}\" health subsets {2}", probeType, healthSet.HealthGroup, subsetName), "probeType");
			}
			return new ProbeIdentity(healthSet, string.Format("{0}{1}", scenario, probeType), targetResource);
		}

		public MonitorIdentity CreateMonitorIdentity()
		{
			return new MonitorIdentity(base.Component, this.baseName, base.TargetResource);
		}

		public ProbeIdentity ForTargetResource(string targetResource)
		{
			return new ProbeIdentity(base.Component, this.baseName, targetResource);
		}

		private const string StandardSuffix = "Probe";

		private readonly string baseName;
	}
}
