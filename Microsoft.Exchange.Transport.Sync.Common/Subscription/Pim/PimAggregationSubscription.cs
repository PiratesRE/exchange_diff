using System;
using System.Security;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.SendAsVerification;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class PimAggregationSubscription : AggregationSubscription, ISendAsSource
	{
		public override SmtpAddress Email
		{
			get
			{
				return this.userEmailAddress;
			}
		}

		public override string Domain
		{
			get
			{
				string text = this.userEmailAddress.ToString();
				if (string.IsNullOrEmpty(text))
				{
					return string.Empty;
				}
				return text.Substring(text.IndexOf("@") + 1);
			}
		}

		public string UserDisplayName
		{
			get
			{
				return this.userDisplayName;
			}
			set
			{
				this.userDisplayName = value;
			}
		}

		public SmtpAddress UserEmailAddress
		{
			get
			{
				return this.userEmailAddress;
			}
			set
			{
				this.userEmailAddress = value;
			}
		}

		public Guid SourceGuid
		{
			get
			{
				return base.SubscriptionGuid;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return base.Status != AggregationStatus.Disabled && base.Status != AggregationStatus.Poisonous;
			}
		}

		public string LogonPassword
		{
			get
			{
				return SyncUtilities.SecureStringToString(this.LogonPasswordSecured);
			}
			set
			{
				this.LogonPasswordSecured = SyncUtilities.StringToSecureString(value);
			}
		}

		public virtual SecureString LogonPasswordSecured
		{
			get
			{
				return this.logonPasswordSecured;
			}
			set
			{
				this.logonPasswordSecured = value;
			}
		}

		public virtual SendAsState SendAsState
		{
			get
			{
				return this.sendAsState;
			}
			set
			{
				this.sendAsState = value;
			}
		}

		public virtual VerificationEmailState VerificationEmailState
		{
			get
			{
				return this.verificationEmailState;
			}
			set
			{
				this.verificationEmailState = value;
			}
		}

		public string VerificationEmailMessageId
		{
			get
			{
				return this.verificationEmailMessageId;
			}
			set
			{
				this.verificationEmailMessageId = value;
			}
		}

		public DateTime? VerificationEmailTimeStamp
		{
			get
			{
				return this.verificationEmailTimestamp;
			}
			set
			{
				this.verificationEmailTimestamp = value;
			}
		}

		public virtual string SendAsValidatedEmail
		{
			get
			{
				return this.sendAsValidatedEmail;
			}
			set
			{
				this.sendAsValidatedEmail = value;
			}
		}

		public virtual string VerifiedUserName
		{
			get
			{
				throw new NotSupportedException("Verified user name not supported for this subscription.  Type: " + base.SubscriptionType.ToString());
			}
		}

		public virtual string VerifiedIncomingServer
		{
			get
			{
				throw new NotSupportedException("Verified incoming server not supported for this subscription.  Type: " + base.SubscriptionType.ToString());
			}
		}

		public virtual bool SendAsNeedsVerification
		{
			get
			{
				return false;
			}
		}

		public sealed override bool IsMirrored
		{
			get
			{
				return base.AggregationType == AggregationType.Mirrored;
			}
		}

		public override FolderSupport FolderSupport
		{
			get
			{
				throw new InvalidOperationException("PimAggregation is a base aggregation class, no FolderSupport information is available.");
			}
		}

		public override ItemSupport ItemSupport
		{
			get
			{
				throw new InvalidOperationException("PimAggregation is a base aggregation class, no ItemSupport information is available.");
			}
		}

		public override SyncQuirks SyncQuirks
		{
			get
			{
				throw new InvalidOperationException("PimAggregation is a base aggregation class, no SyncQuirks information is available.");
			}
		}

		public virtual bool PasswordRequired
		{
			get
			{
				return true;
			}
		}

		internal string EncryptedLogonPassword
		{
			get
			{
				string result;
				if (!this.TryEncryptPassword(out result))
				{
					return null;
				}
				return result;
			}
			set
			{
				this.LogonPasswordSecured = this.DecryptPassword(value);
			}
		}

		public virtual PimSubscriptionProxy CreateSubscriptionProxy()
		{
			return new PimSubscriptionProxy(this);
		}

		public IEmailSender CreateEmailSenderFor(ADUser subscriptionAdUser, ExchangePrincipal subscriptionExchangePrincipal)
		{
			if (this.SendAsNeedsVerification)
			{
				return new EmailSender(this, subscriptionAdUser, subscriptionExchangePrincipal, CommonLoggingHelper.SyncLogSession.OpenWithContext(Guid.Empty, this));
			}
			return EmailSender.NullEmailSender;
		}

		public string GetUserAddressInRfc822SmtpFormat()
		{
			Participant participant = new Participant(this.UserDisplayName, this.UserEmailAddress.ToString(), "SMTP");
			return participant.ToString(AddressFormat.Rfc822Smtp);
		}

		protected override void SetPropertiesToMessageObject(MessageItem message)
		{
			base.SetPropertiesToMessageObject(message);
			message[AggregationSubscriptionMessageSchema.SharingInitiatorName] = this.UserDisplayName;
			message[AggregationSubscriptionMessageSchema.SharingInitiatorSmtp] = this.UserEmailAddress.ToString();
			string value;
			if (this.LogonPasswordSecured != null && this.LogonPasswordSecured.Length > 0 && this.TryEncryptPassword(out value))
			{
				message[AggregationSubscriptionMessageSchema.SharingRemotePass] = value;
			}
			message[MessageItemSchema.SharingSendAsState] = this.sendAsState;
			message[MessageItemSchema.SharingSendAsValidatedEmail] = this.sendAsValidatedEmail;
			message[AggregationSubscriptionMessageSchema.SharingSendAsVerificationEmailState] = this.verificationEmailState;
			message[AggregationSubscriptionMessageSchema.SharingSendAsVerificationMessageId] = this.verificationEmailMessageId;
			if (this.verificationEmailTimestamp == null)
			{
				message[AggregationSubscriptionMessageSchema.SharingSendAsVerificationTimestamp] = SyncUtilities.ExZeroTime;
				return;
			}
			message[AggregationSubscriptionMessageSchema.SharingSendAsVerificationTimestamp] = new ExDateTime(ExTimeZone.UtcTimeZone, this.VerificationEmailTimeStamp.Value.ToUniversalTime());
		}

		protected override bool LoadMinimumInfo(MessageItem message)
		{
			bool result = base.LoadMinimumInfo(message);
			try
			{
				base.GetSmtpAddressProperty(message, AggregationSubscriptionMessageSchema.SharingInitiatorSmtp, out this.userEmailAddress);
			}
			catch (SyncPropertyValidationException ex)
			{
				this.userEmailAddress = SmtpAddress.Parse(PimAggregationSubscription.DefaultEmailAddress);
				base.SetPropertyLoadError(ex.Property, ex.Value);
				result = false;
			}
			try
			{
				base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingInitiatorName, false, new uint?(0U), new uint?(256U), out this.userDisplayName);
			}
			catch (SyncPropertyValidationException ex2)
			{
				this.userDisplayName = this.UserEmailAddress.ToString();
				base.SetPropertyLoadError(ex2.Property, ex2.Value);
				result = false;
			}
			return result;
		}

		protected override void LoadProperties(MessageItem message)
		{
			base.LoadProperties(message);
			if (!PropertyError.IsPropertyError(message.TryGetProperty(MessageItemSchema.SharingSendAsState)))
			{
				base.GetEnumProperty<SendAsState>(message, MessageItemSchema.SharingSendAsState, null, out this.sendAsState);
				base.GetStringProperty(message, MessageItemSchema.SharingSendAsValidatedEmail, true, null, null, out this.sendAsValidatedEmail);
			}
			bool flag = base.SubscriptionType == AggregationSubscriptionType.DeltaSyncMail || base.SubscriptionType == AggregationSubscriptionType.Facebook || base.SubscriptionType == AggregationSubscriptionType.LinkedIn;
			string text;
			base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingRemotePass, flag, flag, null, null, out text);
			if (string.IsNullOrEmpty(text))
			{
				this.LogonPasswordSecured = new SecureString();
				if (!flag)
				{
					CommonLoggingHelper.SyncLogSession.LogError((TSLID)158UL, ExTraceGlobals.SubscriptionManagerTracer, "Unexpected Password value for Subscription: ID {0}, Name {1}, Protocol {2}, Version {3}. The logon password is empty.", new object[]
					{
						base.SubscriptionGuid,
						base.Name,
						base.SubscriptionProtocolName,
						base.Version
					});
				}
			}
			else
			{
				this.EncryptedLogonPassword = text;
			}
			CommonLoggingHelper.SyncLogSession.LogDebugging((TSLID)79UL, ExTraceGlobals.SubscriptionManagerTracer, "Subscription credentials Loaded: ID {0}, Name {1}, Protocol {2}.", new object[]
			{
				base.SubscriptionGuid,
				base.Name,
				base.SubscriptionProtocolName
			});
			if (base.Version >= 2L)
			{
				base.GetEnumProperty<VerificationEmailState>(message, AggregationSubscriptionMessageSchema.SharingSendAsVerificationEmailState, null, out this.verificationEmailState);
			}
			if (base.Version >= 3L)
			{
				base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingSendAsVerificationMessageId, true, null, null, out this.verificationEmailMessageId);
				ExDateTime? exDateTime;
				base.GetExDateTimeProperty(message, AggregationSubscriptionMessageSchema.SharingSendAsVerificationTimestamp, out exDateTime);
				this.verificationEmailTimestamp = (DateTime?)exDateTime;
			}
		}

		protected override void SetMinimumInfoToMessageObject(MessageItem message)
		{
			base.SetMinimumInfoToMessageObject(message);
			message[AggregationSubscriptionMessageSchema.SharingInitiatorSmtp] = this.userEmailAddress.ToString();
			message[AggregationSubscriptionMessageSchema.SharingInitiatorName] = this.userDisplayName;
		}

		protected override void Serialize(AggregationSubscription.SubscriptionSerializer subscriptionSerializer)
		{
			throw new NotImplementedException();
		}

		protected override void Deserialize(AggregationSubscription.SubscriptionDeserializer deserializer)
		{
			throw new NotImplementedException();
		}

		private SecureString DecryptPassword(string encryptedString)
		{
			SecureString result;
			Exception ex;
			if (this.TryEncryptedStringToSecureString(encryptedString, out result, out ex))
			{
				return result;
			}
			CommonLoggingHelper.SyncLogSession.LogError((TSLID)78UL, ExTraceGlobals.SubscriptionManagerTracer, "Failed to Decrypt Password due to error: {0}, for Subscription: ID {1}, Name {2}, Protocol {3}, Version {4}.", new object[]
			{
				ex,
				base.SubscriptionGuid,
				base.Name,
				base.SubscriptionProtocolName,
				base.Version
			});
			return new SecureString();
		}

		private bool TryEncryptPassword(out string encryptedPassword)
		{
			Exception ex;
			if (this.TrySecureStringToEncryptedString(this.LogonPasswordSecured, out encryptedPassword, out ex))
			{
				return true;
			}
			CommonLoggingHelper.SyncLogSession.LogError((TSLID)77UL, ExTraceGlobals.SubscriptionCacheMessageTracer, "Failed to encrypt password. Subscription: '{0}', Name {1}, Protocol {2}. Error: {3}", new object[]
			{
				base.SubscriptionGuid,
				base.Name,
				base.SubscriptionProtocolName,
				ex
			});
			return false;
		}

		private const int MaxStringLength = 256;

		internal static readonly string DefaultEmailAddress = "missing@no-email.microsoft.com";

		private string userDisplayName;

		private SendAsState sendAsState;

		private VerificationEmailState verificationEmailState = VerificationEmailState.EmailNotSent;

		private string verificationEmailMessageId = string.Empty;

		private DateTime? verificationEmailTimestamp;

		private string sendAsValidatedEmail = string.Empty;

		private SmtpAddress userEmailAddress;

		[NonSerialized]
		private SecureString logonPasswordSecured;
	}
}
