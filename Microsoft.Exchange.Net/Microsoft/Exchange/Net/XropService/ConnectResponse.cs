using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.XropService
{
	[CLSCompliant(false)]
	[DataContract(Name = "ConnectResponse", Namespace = "http://schemas.microsoft.com/exchange/2010/xrop")]
	public sealed class ConnectResponse
	{
		[DataMember]
		public uint ServiceCode { get; set; }

		[DataMember]
		public uint ErrorCode { get; set; }

		[DataMember]
		public uint Context { get; set; }

		[DataMember]
		public uint PollsMax { get; set; }

		[DataMember]
		public uint Retry { get; set; }

		[DataMember]
		public uint RetryDelay { get; set; }

		[DataMember]
		public ushort Icxr { get; set; }

		[DataMember]
		public string DNPrefix { get; set; }

		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public byte[] ServerVersion { get; set; }

		[DataMember]
		public byte[] BestVersion { get; set; }

		[DataMember]
		public uint TimeStamp { get; set; }

		[DataMember]
		public byte[] AuxOut { get; set; }
	}
}
