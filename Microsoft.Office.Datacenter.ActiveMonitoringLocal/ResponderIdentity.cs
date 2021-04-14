using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class ResponderIdentity : WorkItemIdentity.Typed<ResponderDefinition>
	{
		public ResponderIdentity(Component component, string baseName, string verb, string targetResource) : base(component, ResponderIdentity.ExpandBaseName(baseName, verb), targetResource)
		{
		}

		private static string ExpandBaseName(string baseName, string displayType)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("baseName", baseName);
			ArgumentValidator.ThrowIfNullOrEmpty("displayType", displayType);
			return WorkItemIdentity.ToLocalName(baseName, displayType);
		}

		public static class Verb
		{
			public const string Restart = "Restart";

			public const string Failover = "Failover";

			public const string KillServer = "KillServer";

			public const string Escalate = "Escalate";
		}
	}
}
