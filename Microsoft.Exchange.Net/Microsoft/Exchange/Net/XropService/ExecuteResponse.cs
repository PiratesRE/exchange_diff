using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.XropService
{
	[DataContract(Name = "ExecuteResponse", Namespace = "http://schemas.microsoft.com/exchange/2010/xrop")]
	[CLSCompliant(false)]
	public sealed class ExecuteResponse
	{
		[DataMember]
		public uint ServiceCode { get; set; }

		[DataMember]
		public uint ErrorCode { get; set; }

		[DataMember]
		public uint Context { get; set; }

		[DataMember]
		public uint Flags { get; set; }

		[DataMember]
		public byte[] Out { get; set; }

		[DataMember]
		public byte[] AuxOut { get; set; }

		[DataMember]
		public uint TransTime { get; set; }
	}
}
