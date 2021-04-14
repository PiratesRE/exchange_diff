using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetEMailSignatureConfiguration : SetMessagingConfigurationBase
	{
		[DataMember]
		public string SignatureHtml
		{
			get
			{
				return (string)base["SignatureHtml"];
			}
			set
			{
				base["SignatureHtml"] = value;
			}
		}

		[DataMember]
		public bool AutoAddSignature
		{
			get
			{
				return (bool)(base["AutoAddSignature"] ?? false);
			}
			set
			{
				base["AutoAddSignature"] = value;
			}
		}
	}
}
