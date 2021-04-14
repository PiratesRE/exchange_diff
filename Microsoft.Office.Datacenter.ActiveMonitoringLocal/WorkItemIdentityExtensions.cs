using System;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public static class WorkItemIdentityExtensions
	{
		public static TDefinition Apply<TDefinition>(this TDefinition definition, WorkItemIdentity.Typed<TDefinition> identity) where TDefinition : WorkDefinition
		{
			identity.ApplyTo(definition);
			return definition;
		}
	}
}
