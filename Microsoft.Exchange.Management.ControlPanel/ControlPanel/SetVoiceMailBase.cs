using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class SetVoiceMailBase : SetObjectProperties
	{
		[DataMember]
		public bool PinlessAccessToVoiceMailEnabled
		{
			get
			{
				return (bool)(base[UMMailboxSchema.PinlessAccessToVoiceMailEnabled] ?? false);
			}
			set
			{
				base[UMMailboxSchema.PinlessAccessToVoiceMailEnabled] = value;
			}
		}

		[DataMember]
		public string SMSNotificationOption
		{
			get
			{
				return ((UMSMSNotificationOptions)(base[UMMailboxSchema.UMSMSNotificationOption] ?? UMSMSNotificationOptions.None)).ToString();
			}
			set
			{
				base[UMMailboxSchema.UMSMSNotificationOption] = value;
			}
		}
	}
}
