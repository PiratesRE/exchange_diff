using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "MediaRelay")]
	internal class MediaRelay : Resource
	{
		public MediaRelay(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "Location", EmitDefaultValue = false)]
		public RelayLocation Location
		{
			get
			{
				return base.GetValue<RelayLocation>("Location");
			}
			set
			{
				base.SetValue<RelayLocation>("Location", value);
			}
		}

		[DataMember(Name = "Host", EmitDefaultValue = false)]
		public string Host
		{
			get
			{
				return base.GetValue<string>("Host");
			}
			set
			{
				base.SetValue<string>("Host", value);
			}
		}

		[DataMember(Name = "TcpPort", EmitDefaultValue = false)]
		public int TcpPort
		{
			get
			{
				return base.GetValue<int>("TcpPort");
			}
			set
			{
				base.SetValue<int>("TcpPort", value);
			}
		}

		[DataMember(Name = "UdpPort", EmitDefaultValue = false)]
		public int UdpPort
		{
			get
			{
				return base.GetValue<int>("UdpPort");
			}
			set
			{
				base.SetValue<int>("UdpPort", value);
			}
		}
	}
}
