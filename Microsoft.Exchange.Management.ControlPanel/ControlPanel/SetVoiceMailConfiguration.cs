using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetVoiceMailConfiguration : SetVoiceMailBase
	{
		public SetVoiceMailConfiguration()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-UMMailbox";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		public SetVoiceMailPIN SetVoiceMailPIN { get; private set; }

		public SetSmsOptions SetSmsOptions { get; private set; }

		[DataMember]
		public string PhoneNumber
		{
			get
			{
				return (string)base[UMMailboxSchema.PhoneNumber];
			}
			set
			{
				base[UMMailboxSchema.PhoneNumber] = value;
			}
		}

		[DataMember]
		public string PIN
		{
			get
			{
				return this.SetVoiceMailPIN.PIN;
			}
			set
			{
				this.SetVoiceMailPIN.PIN = value;
			}
		}

		[DataMember]
		public string PhoneProviderId
		{
			get
			{
				return (string)base[UMMailboxSchema.PhoneProviderId];
			}
			set
			{
				base[UMMailboxSchema.PhoneProviderId] = value;
			}
		}

		[DataMember]
		public bool VerifyGlobalRoutingEntry
		{
			get
			{
				return (bool)(base["VerifyGlobalRoutingEntry"] ?? false);
			}
			set
			{
				base["VerifyGlobalRoutingEntry"] = value;
			}
		}

		[DataMember]
		public string VerificationCode
		{
			get
			{
				return this.SetSmsOptions.VerificationCode;
			}
			set
			{
				this.SetSmsOptions.VerificationCode = value;
			}
		}

		[DataMember]
		public string SMSNotificationPhoneNumber
		{
			get
			{
				return this.SetSmsOptions.NotificationPhoneNumber;
			}
			set
			{
				this.SetSmsOptions.NotificationPhoneNumber = value;
			}
		}

		[DataMember]
		public string SMSNotificationPhoneProviderId
		{
			get
			{
				return this.SetSmsOptions.MobileOperatorId;
			}
			set
			{
				this.SetSmsOptions.MobileOperatorId = value;
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.SetVoiceMailPIN = new SetVoiceMailPIN();
			this.SetSmsOptions = new SetSmsOptions();
		}

		private const string VerifyGlobalRoutingEntryParameter = "VerifyGlobalRoutingEntry";
	}
}
