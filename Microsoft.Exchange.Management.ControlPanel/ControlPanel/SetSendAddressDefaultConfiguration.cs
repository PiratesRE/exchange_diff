using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetSendAddressDefaultConfiguration : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-MailboxMessageConfiguration";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		[DataMember]
		public string SendAddressDefault
		{
			get
			{
				return (string)base[MailboxMessageConfigurationSchema.SendAddressDefault];
			}
			set
			{
				base[MailboxMessageConfigurationSchema.SendAddressDefault] = value.Trim();
			}
		}
	}
}
