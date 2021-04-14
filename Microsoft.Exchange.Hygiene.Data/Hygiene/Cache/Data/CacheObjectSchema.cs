using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data;

namespace Microsoft.Exchange.Hygiene.Cache.Data
{
	internal static class CacheObjectSchema
	{
		public static readonly HygienePropertyDefinition EntityNameProp = new HygienePropertyDefinition("EntityName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition LastSyncTimeProp = new HygienePropertyDefinition("LastSyncTime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition LastFullSyncTimeProp = new HygienePropertyDefinition("LastFullSyncTime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition LastTracerSyncTimeProp = new HygienePropertyDefinition("LastTracerSyncTime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
