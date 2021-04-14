using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.SharePointSignalStore
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract(Namespace = "http://www.microsoft.com/sharepoint/search/KnownTypes/2011/01")]
	[Serializable]
	internal sealed class AnalyticsSignal
	{
		[DataMember]
		public string Source { get; set; }

		[DataMember]
		public AnalyticsSignal.AnalyticsActor Actor { get; set; }

		[DataMember]
		public AnalyticsSignal.AnalyticsAction Action { get; set; }

		[DataMember]
		public AnalyticsSignal.AnalyticsItem Item { get; set; }

		[DataContract(Namespace = "http://www.microsoft.com/sharepoint/search/KnownTypes/2011/01")]
		[Serializable]
		internal sealed class AnalyticsActor
		{
			[DataMember]
			public string Id { get; set; }

			[DataMember]
			public Dictionary<string, object> Properties { get; set; }

			[DataMember]
			public Guid TenantId { get; set; }
		}

		[DataContract(Namespace = "http://www.microsoft.com/sharepoint/search/KnownTypes/2011/01")]
		[Serializable]
		internal sealed class AnalyticsAction
		{
			[DataMember]
			public string ActionType { get; set; }

			[DataMember]
			public DateTime UserTime { get; set; }

			[DataMember]
			public DateTime ExpireTime { get; set; }

			[DataMember]
			public Dictionary<string, object> Properties { get; set; }
		}

		[DataContract(Namespace = "http://www.microsoft.com/sharepoint/search/KnownTypes/2011/01")]
		[Serializable]
		internal sealed class AnalyticsItem
		{
			[DataMember]
			public string Id { get; set; }

			[DataMember]
			public Dictionary<string, object> Properties { get; set; }
		}
	}
}
