using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.ActivityLog;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	internal class ActivityLogEntryPresentationObjectSchema : ObjectSchema
	{
		public static readonly SimplePropertyDefinition ClientId = new SimplePropertyDefinition("ClientId", typeof(string), Microsoft.Exchange.Data.Storage.ActivityLog.ClientId.Min.ToString());

		public static readonly SimplePropertyDefinition ActivityId = new SimplePropertyDefinition("ActivityId", typeof(string), Enum.GetName(typeof(ActivityId), Microsoft.Exchange.Data.Storage.ActivityLog.ActivityId.Min));

		public static readonly SimplePropertyDefinition TimeStamp = new SimplePropertyDefinition("TimeStamp", typeof(ExDateTime), ExDateTime.MinValue);

		public static readonly SimplePropertyDefinition EntryId = new SimplePropertyDefinition("EntryId", typeof(string), string.Empty);

		public static readonly SimplePropertyDefinition CustomProperties = new SimplePropertyDefinition("CustomProperties", typeof(string), string.Empty);
	}
}
