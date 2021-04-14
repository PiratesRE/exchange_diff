using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class GetVoiceMailSettings : GetVoiceMailBase
	{
		static GetVoiceMailSettings()
		{
			GetVoiceMailSettings.smsOptionNames[UMSMSNotificationOptions.None] = OwaOptionStrings.VoicemailSMSOptionNone;
			GetVoiceMailSettings.smsOptionNames[UMSMSNotificationOptions.VoiceMail] = OwaOptionStrings.VoicemailSMSOptionVoiceMailOnly;
			GetVoiceMailSettings.smsOptionNames[UMSMSNotificationOptions.VoiceMailAndMissedCalls] = OwaOptionStrings.VoicemailSMSOptionVoiceMailAndMissedCalls;
			GetVoiceMailSettings.smsAvailableOptions = new Dictionary<UMSubscriberType, UMSMSNotificationOptions[]>(2);
			GetVoiceMailSettings.smsAvailableOptions[UMSubscriberType.Consumer] = new UMSMSNotificationOptions[]
			{
				UMSMSNotificationOptions.None,
				UMSMSNotificationOptions.VoiceMail
			};
			GetVoiceMailSettings.smsAvailableOptions[UMSubscriberType.Enterprise] = new UMSMSNotificationOptions[]
			{
				UMSMSNotificationOptions.None,
				UMSMSNotificationOptions.VoiceMail,
				UMSMSNotificationOptions.VoiceMailAndMissedCalls
			};
		}

		public GetVoiceMailSettings(UMMailbox mailbox) : base(mailbox)
		{
		}

		[DataMember]
		public string[] AvailableSMSOptionValues
		{
			get
			{
				UMSMSNotificationOptions[] array = GetVoiceMailSettings.smsAvailableOptions[base.UMDialPlan.SubscriberType];
				string[] array2 = new string[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = array[i].ToString();
				}
				return array2;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string[] AvailableSMSOptionNames
		{
			get
			{
				UMSMSNotificationOptions[] array = GetVoiceMailSettings.smsAvailableOptions[base.UMDialPlan.SubscriberType];
				string[] array2 = new string[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = GetVoiceMailSettings.smsOptionNames[array[i]].ToString();
				}
				return array2;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private static Dictionary<UMSMSNotificationOptions, LocalizedString> smsOptionNames = new Dictionary<UMSMSNotificationOptions, LocalizedString>(3);

		private static Dictionary<UMSubscriberType, UMSMSNotificationOptions[]> smsAvailableOptions;
	}
}
