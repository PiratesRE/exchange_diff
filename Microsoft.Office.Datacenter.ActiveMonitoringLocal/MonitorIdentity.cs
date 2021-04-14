using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class MonitorIdentity : WorkItemIdentity.Typed<MonitorDefinition>
	{
		public MonitorIdentity(Component component, string baseName, string targetResource) : base(component, WorkItemIdentity.ToLocalName(baseName, "Monitor"), targetResource)
		{
			this.baseName = baseName;
		}

		public static implicit operator MonitorIdentity(MonitorDefinition definition)
		{
			if (definition == null)
			{
				return null;
			}
			Component component = ExchangeComponent.WellKnownComponents[definition.ServiceName];
			return new MonitorIdentity(component, WorkItemIdentity.GetLocalName(component, definition.Name, "Monitor"), definition.TargetResource);
		}

		public ResponderIdentity CreateResponderIdentity(string responderDisplayType, string targetResource = null)
		{
			return new ResponderIdentity(base.Component, this.baseName, responderDisplayType, targetResource ?? base.TargetResource);
		}

		public override void ApplyTo(MonitorDefinition definition)
		{
			base.ApplyTo(definition);
			definition.Component = base.Component;
		}

		private const string StandardSuffix = "Monitor";

		private readonly string baseName;
	}
}
