using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class WindowsLiveServiceAggregationSubscription : PimAggregationSubscription
	{
		public WindowsLiveServiceAggregationSubscription()
		{
			this.AuthPolicy = string.Empty;
			this.IncommingServerUrl = string.Empty;
			this.AuthTokenExpirationTime = new DateTime(0L, DateTimeKind.Utc);
		}

		public string LogonName
		{
			get
			{
				return this.logonName;
			}
			set
			{
				this.logonName = value;
			}
		}

		public string IncommingServerUrl
		{
			get
			{
				return this.incommingServerUrl;
			}
			set
			{
				this.incommingServerUrl = value;
			}
		}

		public string AuthPolicy
		{
			get
			{
				return this.authPolicy;
			}
			set
			{
				this.authPolicy = value;
			}
		}

		public string Puid
		{
			get
			{
				return this.puid;
			}
			set
			{
				this.puid = value;
			}
		}

		public string AuthToken
		{
			get
			{
				return this.authToken;
			}
			set
			{
				this.authToken = value;
			}
		}

		public DateTime AuthTokenExpirationTime
		{
			get
			{
				return this.authTokenExpirationTime;
			}
			set
			{
				this.authTokenExpirationTime = value;
			}
		}

		public override bool PasswordRequired
		{
			get
			{
				return false;
			}
		}

		protected override void SetPropertiesToMessageObject(MessageItem message)
		{
			base.SetPropertiesToMessageObject(message);
			message[AggregationSubscriptionMessageSchema.SharingRemoteUser] = base.UserEmailAddress.ToString();
			message[MessageItemSchema.SharingRemotePath] = this.IncommingServerUrl;
			message[AggregationSubscriptionMessageSchema.SharingWlidAuthPolicy] = this.AuthPolicy;
			if (this.Puid != null)
			{
				message[AggregationSubscriptionMessageSchema.SharingWlidUserPuid] = this.Puid;
			}
			if (this.AuthToken != null)
			{
				message[AggregationSubscriptionMessageSchema.SharingWlidAuthToken] = this.AuthToken;
			}
			message[AggregationSubscriptionMessageSchema.SharingWlidAuthTokenExpireTime] = new ExDateTime(ExTimeZone.UtcTimeZone, this.AuthTokenExpirationTime.ToUniversalTime());
			message[MessageItemSchema.SharingDetail] = (int)((AggregationType)(-49) | base.AggregationType);
		}

		protected override void LoadProperties(MessageItem message)
		{
			base.LoadProperties(message);
			base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingRemoteUser, false, new uint?(0U), new uint?(256U), out this.logonName);
			base.GetStringProperty(message, MessageItemSchema.SharingRemotePath, true, null, null, out this.incommingServerUrl);
			if (!string.IsNullOrEmpty(this.incommingServerUrl) && !this.ValidateIncomingServerUrl(this.incommingServerUrl))
			{
				throw new SyncPropertyValidationException(MessageItemSchema.SharingRemotePath.ToString(), this.incommingServerUrl.ToString(), null);
			}
			base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingWlidAuthPolicy, true, null, null, out this.authPolicy);
			ExDateTime exDateTime;
			base.GetExDateTimeProperty(message, AggregationSubscriptionMessageSchema.SharingWlidAuthTokenExpireTime, out exDateTime);
			this.authTokenExpirationTime = (DateTime)exDateTime;
			bool flag = this.LogonPasswordSecured != null && this.LogonPasswordSecured.Length > 0;
			base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingWlidUserPuid, flag, flag, new uint?(0U), new uint?(16U), out this.puid);
			base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingWlidAuthToken, true, true, null, null, out this.authToken);
		}

		protected abstract bool ValidateIncomingServerUrl(string incomingServerUrl);

		private const int MaxStringLength = 256;

		private string logonName;

		private string incommingServerUrl;

		private string authPolicy;

		private string puid;

		private string authToken;

		private DateTime authTokenExpirationTime;
	}
}
