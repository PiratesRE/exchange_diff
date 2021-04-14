using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.XropService
{
	[DataContract(Name = "DisconnectResponse", Namespace = "http://schemas.microsoft.com/exchange/2010/xrop")]
	[CLSCompliant(false)]
	public sealed class DisconnectResponse
	{
		[DataMember]
		public uint ServiceCode { get; set; }

		[DataMember]
		public uint ErrorCode { get; set; }

		[DataMember]
		public uint Context { get; set; }
	}
}
