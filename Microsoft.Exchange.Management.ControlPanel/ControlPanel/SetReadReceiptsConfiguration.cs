using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetReadReceiptsConfiguration : SetMessagingConfigurationBase
	{
		[DataMember]
		public string ReadReceiptResponse
		{
			get
			{
				return (string)base["ReadReceiptResponse"];
			}
			set
			{
				base["ReadReceiptResponse"] = value;
			}
		}
	}
}
