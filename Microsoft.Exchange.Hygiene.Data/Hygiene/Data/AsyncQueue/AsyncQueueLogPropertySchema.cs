using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueLogPropertySchema
	{
		internal static readonly HygienePropertyDefinition LogTimeProperty = new HygienePropertyDefinition("LogTime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition LogTypeProperty = new HygienePropertyDefinition("LogType", typeof(string));

		internal static readonly HygienePropertyDefinition LogIndexProperty = new HygienePropertyDefinition("LogIndex", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition LogDataProperty = new HygienePropertyDefinition("LogData", typeof(string));
	}
}
