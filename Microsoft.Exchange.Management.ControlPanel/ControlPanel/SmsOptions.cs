using System;
using System.Runtime.Serialization;
using AjaxControlToolkit;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[ClientScriptResource("OwaOptions", "Microsoft.Exchange.Management.ControlPanel.Client.OwaOptions.js")]
	public class SmsOptions : BaseRow
	{
		public SmsOptions(TextMessagingAccount account) : base(account)
		{
			this.account = account;
		}

		[DataMember]
		public bool EasEnabled
		{
			get
			{
				return this.account.EasEnabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool NotificationEnabled
		{
			get
			{
				return this.account.NotificationPhoneNumber != null && this.account.NotificationPhoneNumberVerified;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string NotificationPhoneCountryCode
		{
			get
			{
				if (!(this.account.NotificationPhoneNumber != null))
				{
					return string.Empty;
				}
				return this.account.NotificationPhoneNumber.CountryCode;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string NotificationPhoneNumber
		{
			get
			{
				if (!(this.account.NotificationPhoneNumber != null))
				{
					return string.Empty;
				}
				return this.account.NotificationPhoneNumber.SignificantNumber;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool NotificationPhoneNumberVerified
		{
			get
			{
				return this.account.NotificationPhoneNumberVerified;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Description
		{
			get
			{
				if (this.EasEnabled)
				{
					return OwaOptionStrings.TextMessagingSlabMessage;
				}
				return OwaOptionStrings.TextMessagingSlabMessageNotificationOnly;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string StatusPrefix
		{
			get
			{
				if (this.NotificationEnabled)
				{
					return OwaOptionStrings.TextMessagingStatusPrefixNotifications;
				}
				return OwaOptionStrings.TextMessagingStatusPrefixStatus;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string StatusDetails
		{
			get
			{
				if (this.EasEnabled)
				{
					return OwaOptionStrings.TextMessagingTurnedOnViaEas;
				}
				if (this.NotificationEnabled)
				{
					return OwaOptionStrings.ReceiveNotificationsUsingFormat(this.NotificationPhoneNumber);
				}
				return OwaOptionStrings.TextMessagingOff;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CountryRegionId
		{
			get
			{
				if (this.account.CountryRegionId != null)
				{
					return this.account.CountryRegionId.Name;
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MobileOperatorId
		{
			get
			{
				if (this.account.MobileOperatorId > 0)
				{
					return this.account.MobileOperatorId.ToString();
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private readonly TextMessagingAccount account;
	}
}
