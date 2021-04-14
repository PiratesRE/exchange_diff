using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.XropService
{
	[CLSCompliant(false)]
	[DataContract(Name = "ConnectRequest", Namespace = "http://schemas.microsoft.com/exchange/2010/xrop")]
	public sealed class ConnectRequest
	{
		[DataMember]
		public bool Interactive { get; set; }

		[DataMember]
		public string UserDN { get; set; }

		[DataMember]
		public uint Flags { get; set; }

		[DataMember]
		public uint ConMod { get; set; }

		[DataMember]
		public uint Limit { get; set; }

		[DataMember]
		public uint Cpid { get; set; }

		[DataMember]
		public uint LcidString { get; set; }

		[DataMember]
		public uint LcidSort { get; set; }

		[DataMember]
		public uint IcxrLink { get; set; }

		[DataMember]
		public ushort FCanConvertCodePages { get; set; }

		[DataMember]
		public byte[] ClientVersion { get; set; }

		[DataMember]
		public uint TimeStamp { get; set; }

		[DataMember]
		public byte[] AuxIn { get; set; }

		[DataMember]
		public uint AuxOutMaxSize { get; set; }
	}
}
