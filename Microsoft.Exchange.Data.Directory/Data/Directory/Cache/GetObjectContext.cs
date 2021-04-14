using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[DataContract]
	internal class GetObjectContext : CacheResponseContext
	{
		[DataMember]
		internal SimpleADObject Object { get; set; }
	}
}
