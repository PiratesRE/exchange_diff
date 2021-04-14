using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class GetVoiceMailBase : BaseRow
	{
		public GetVoiceMailBase(UMMailbox mailbox) : base(mailbox)
		{
			this.UMMailbox = mailbox;
			this.UMDialPlan = mailbox.GetDialPlan();
		}

		public UMMailbox UMMailbox { get; private set; }

		public UMDialPlan UMDialPlan { get; private set; }

		public SmsOptions SmsOptions { get; internal set; }

		[DataMember]
		public bool IsConfigured
		{
			get
			{
				return !string.IsNullOrEmpty(this.UMMailbox.PhoneNumber);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool PinlessAccessToVoiceMailEnabled
		{
			get
			{
				return this.UMMailbox.PinlessAccessToVoiceMailEnabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SubscriberType
		{
			get
			{
				return this.UMDialPlan.SubscriberType.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SMSNotificationOption
		{
			get
			{
				return this.UMMailbox.UMSMSNotificationOption.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool SMSNotificationConfigured
		{
			get
			{
				return this.SmsOptions != null && this.SmsOptions.NotificationPhoneNumberVerified && !string.IsNullOrEmpty(this.SmsOptions.NotificationPhoneNumber);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
