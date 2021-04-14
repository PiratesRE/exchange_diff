using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class GetVoiceMailConfiguration : GetVoiceMailBase
	{
		public GetVoiceMailConfiguration(UMMailbox mailbox) : base(mailbox)
		{
			this.UMMailboxPolicy = mailbox.GetPolicy();
			this.carrierData = this.GetCarrierData();
		}

		public UMMailboxPolicy UMMailboxPolicy { get; private set; }

		[DataMember]
		public string PhoneNumber
		{
			get
			{
				return base.UMMailbox.PhoneNumber;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PIN
		{
			get
			{
				return this.GetDisplayPIN();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CountryOrRegionCode
		{
			get
			{
				return base.UMDialPlan.CountryOrRegionCode;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CountryOrRegionId
		{
			get
			{
				return "US";
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CallForwardingPilotNumber
		{
			get
			{
				return this.GetCallForwardingPilotNumber();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string VoiceMailAccessNumbers
		{
			get
			{
				return this.GetVoiceMailAccessNumbers();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PhoneProviderId
		{
			get
			{
				return base.UMMailbox.PhoneProviderId;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PhoneProviderName
		{
			get
			{
				if (this.carrierData != null)
				{
					return this.carrierData.Name;
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int RequiredPINLength
		{
			get
			{
				return this.UMMailboxPolicy.MinPINLength;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CallForwardingDisableDigits
		{
			get
			{
				if (this.carrierData != null)
				{
					return this.carrierData.UnifiedMessagingInfo.RenderDisableSequence(this.PhoneNumber);
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SMSNotificationPhoneNumber
		{
			get
			{
				if (base.SmsOptions == null || !base.SmsOptions.NotificationPhoneNumberVerified)
				{
					return string.Empty;
				}
				return base.SmsOptions.NotificationPhoneNumber;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SMSNotificationPhoneProviderId
		{
			get
			{
				if (base.SmsOptions == null || !base.SmsOptions.NotificationPhoneNumberVerified)
				{
					return string.Empty;
				}
				return base.SmsOptions.MobileOperatorId;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool VerificationCodeRequired { get; internal set; }

		[DataMember]
		public string VerificationCode
		{
			get
			{
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private string GetDisplayPIN()
		{
			return new string('*', this.UMMailboxPolicy.MinPINLength);
		}

		private CarrierData GetCarrierData()
		{
			if (!string.IsNullOrEmpty(this.PhoneProviderId))
			{
				CarrierData result = null;
				if (SmsServiceProviders.Instance.VoiceMailCarrierDictionary.TryGetValue(this.PhoneProviderId, out result))
				{
					return result;
				}
			}
			return null;
		}

		private string GetCallForwardingPilotNumber()
		{
			string result = string.Empty;
			TelephonyInfo telephonyInfo;
			if (AirSyncUtils.GetTelephonyInfo(base.UMDialPlan, base.UMMailbox.SIPResourceIdentifier, out telephonyInfo))
			{
				result = telephonyInfo.VoicemailNumber.Number;
			}
			return result;
		}

		private string GetVoiceMailAccessNumbers()
		{
			string text = string.Empty;
			if (base.UMDialPlan.AccessTelephoneNumbers != null)
			{
				foreach (string text2 in base.UMDialPlan.AccessTelephoneNumbers)
				{
					if (!string.IsNullOrEmpty(text2))
					{
						if (text.Length == 0)
						{
							text = text2;
						}
						else
						{
							text = text + " " + OwaOptionStrings.VoicemailAccessNumbersTemplate(text2).ToString();
						}
					}
				}
			}
			return text;
		}

		private CarrierData carrierData;
	}
}
