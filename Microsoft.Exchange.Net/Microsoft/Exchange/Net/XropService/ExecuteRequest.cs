using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.XropService
{
	[DataContract(Name = "ExecuteRequest", Namespace = "http://schemas.microsoft.com/exchange/2010/xrop")]
	[CLSCompliant(false)]
	public sealed class ExecuteRequest
	{
		[DataMember]
		public uint Context { get; set; }

		[DataMember]
		public uint Flags { get; set; }

		[DataMember]
		public byte[] In { get; set; }

		[DataMember]
		public uint MaxSizeOut { get; set; }

		[DataMember]
		public byte[] AuxIn { get; set; }

		[DataMember]
		public uint MaxSizeAuxOut { get; set; }
	}
}
