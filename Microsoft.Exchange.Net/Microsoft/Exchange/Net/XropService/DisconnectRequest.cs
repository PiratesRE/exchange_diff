using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.XropService
{
	[DataContract(Name = "DisconnectRequest", Namespace = "http://schemas.microsoft.com/exchange/2010/xrop")]
	[CLSCompliant(false)]
	public sealed class DisconnectRequest
	{
		[DataMember]
		public uint Context { get; set; }
	}
}
