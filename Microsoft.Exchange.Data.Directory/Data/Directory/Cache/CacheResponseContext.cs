using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[DataContract]
	internal class CacheResponseContext
	{
		[DataMember]
		internal long BeginOperationLatency { get; set; }

		[DataMember]
		internal ADCacheResultState ResultState { get; set; }

		[DataMember]
		internal long EndOperationLatency { get; set; }
	}
}
