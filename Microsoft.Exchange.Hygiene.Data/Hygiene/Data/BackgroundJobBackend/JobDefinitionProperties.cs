using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal static class JobDefinitionProperties
	{
		internal static readonly BackgroundJobBackendPropertyDefinition BackgroundJobIdProperty = new BackgroundJobBackendPropertyDefinition("BackgroundJobId", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition NameProperty = new BackgroundJobBackendPropertyDefinition("Name", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition PathProperty = new BackgroundJobBackendPropertyDefinition("Path", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition CommandLineProperty = new BackgroundJobBackendPropertyDefinition("CommandLine", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition DescriptionProperty = new BackgroundJobBackendPropertyDefinition("Description", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition RoleIdProperty = new BackgroundJobBackendPropertyDefinition("RoleId", typeof(Guid), PropertyDefinitionFlags.Mandatory, Guid.Empty);

		internal static readonly BackgroundJobBackendPropertyDefinition SingleInstancePerMachineProperty = new BackgroundJobBackendPropertyDefinition("SingleInstancePerMachine", typeof(bool), PropertyDefinitionFlags.Mandatory, false);

		internal static readonly BackgroundJobBackendPropertyDefinition SchedulingStrategyProperty = new BackgroundJobBackendPropertyDefinition("SchedulingStrategy", typeof(byte), PropertyDefinitionFlags.Mandatory, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition TimeoutProperty = new BackgroundJobBackendPropertyDefinition("Timeout", typeof(int), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition MaxLocalRetryCountProperty = new BackgroundJobBackendPropertyDefinition("MaxLocalRetryCount", typeof(byte), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0);

		internal static readonly BackgroundJobBackendPropertyDefinition MaxFailoverCountProperty = new BackgroundJobBackendPropertyDefinition("MaxFailoverCount", typeof(short), PropertyDefinitionFlags.Mandatory, 0);
	}
}
