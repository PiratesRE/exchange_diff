using System;

namespace Microsoft.Exchange.Assistants
{
	internal static class Globals
	{
		public const string EventSource = "MSExchange Assistants";

		public const string ComponentGuidString = "EDC33045-05FB-4abb-A608-AEE572BC3C5F";

		public const string ParameterRegistryPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange Assistants\\Parameters";

		public static readonly Guid ComponentGuid = new Guid("EDC33045-05FB-4abb-A608-AEE572BC3C5F");
	}
}
