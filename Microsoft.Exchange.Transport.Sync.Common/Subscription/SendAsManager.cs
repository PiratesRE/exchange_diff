using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Security.Dkm;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.SendAsVerification;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SendAsManager
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.SendAsTracer;
			}
		}

		public void ResetVerificationEmailData(PimAggregationSubscription subscription)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			subscription.VerificationEmailState = VerificationEmailState.EmailNotSent;
			subscription.VerificationEmailMessageId = string.Empty;
			subscription.VerificationEmailTimeStamp = null;
		}

		public bool ValidateSharedSecret(PimAggregationSubscription subscription, string sharedSecret)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("sharedSecret", sharedSecret);
			if (!subscription.SendAsNeedsVerification)
			{
				throw new ArgumentException("The subscription type does not support SendAs validation: " + subscription.SubscriptionType.ToString(), "subscription");
			}
			Guid a;
			if (!this.TryParseGuid(sharedSecret, out a))
			{
				this.currentSyncLogSession.LogVerbose((TSLID)43UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Invalid format for shared secret.  Actual data: {0}", new object[]
				{
					sharedSecret
				});
				return false;
			}
			SendAsState sendAsState;
			Guid guid;
			Guid b;
			SmtpAddress smtpAddress;
			string text;
			string text2;
			if (!this.TryParseSignedHash(subscription.SendAsValidatedEmail, subscription.SendAsNeedsVerification, out sendAsState, out guid, out b, out smtpAddress, out text, out text2))
			{
				this.currentSyncLogSession.LogVerbose((TSLID)44UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Failed to parse SendAsValidatedEmail", new object[0]);
				return false;
			}
			string text3;
			string text4;
			this.GetVerifiedSubscriptionData(subscription, out text3, out text4);
			if (sendAsState != subscription.SendAsState || guid != subscription.SubscriptionGuid || smtpAddress != subscription.UserEmailAddress || text != text3 || text2 != text4)
			{
				string format = "The parsed values in SendAsValidatedEmail do not match the current subscription values. Parsed: SendAsState:'{0}' SubscriptionGuid:'{1}' EmailAddress:'{2}' UserName:'{3}' Server:'{4}' Actual: SendAsState:'{5}' SubscriptionGuid:'{6}' EmailAddress:'{7}' UserName:'{8}' Server:'{9}'";
				this.currentSyncLogSession.LogVerbose((TSLID)45UL, SendAsManager.Tracer, (long)this.GetHashCode(), format, new object[]
				{
					sendAsState,
					guid,
					smtpAddress,
					text,
					text2,
					subscription.SendAsState,
					subscription.SubscriptionGuid,
					subscription.UserEmailAddress,
					text3,
					text4
				});
				return false;
			}
			if (subscription.SendAsState == SendAsState.Enabled)
			{
				return true;
			}
			if (a != b)
			{
				this.currentSyncLogSession.LogVerbose((TSLID)46UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Shared secret mismatch.  Expected: {0} Actual: {1}", new object[]
				{
					b.ToString("N"),
					a.ToString("N")
				});
				return false;
			}
			subscription.SendAsState = SendAsState.Enabled;
			Guid guid2;
			subscription.SendAsValidatedEmail = this.CreateSignedHash(subscription, out guid2);
			return true;
		}

		public void ResendVerificationEmail(PimAggregationSubscription subscription, IEmailSender emailSender)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("emailSender", emailSender);
			if (!subscription.SendAsNeedsVerification)
			{
				throw new ArgumentException("subscription is not SendAs verified.  Type: " + subscription.SubscriptionType.ToString(), "subscription");
			}
			Guid sharedSecret = Guid.Empty;
			bool flag = false;
			SendAsState sendAsState;
			Guid guid;
			Guid guid2;
			SmtpAddress smtpAddress;
			string text;
			string text2;
			if (this.TryParseSignedHash(subscription.SendAsValidatedEmail, subscription.SendAsNeedsVerification, out sendAsState, out guid, out guid2, out smtpAddress, out text, out text2))
			{
				string text3;
				string text4;
				this.GetVerifiedSubscriptionData(subscription, out text3, out text4);
				if (sendAsState == subscription.SendAsState && guid == subscription.SubscriptionGuid && smtpAddress == subscription.UserEmailAddress && text == text3 && text2 == text4)
				{
					if (subscription.SendAsState == SendAsState.Enabled)
					{
						this.currentSyncLogSession.LogVerbose((TSLID)47UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Subscription already enabled.  Not sending email.", new object[0]);
						return;
					}
					sharedSecret = guid2;
				}
				else
				{
					string format = "The parsed values in SendAsValidatedEmail do not match the current subscription values. Creating a new signed hash. Parsed: SendAsState:'{0}' SubscriptionGuid:'{1}' EmailAddress:'{2}' UserName:'{3}' Server:'{4}' Actual: SendAsState:'{5}' SubscriptionGuid:'{6}' EmailAddress:'{7}' UserName:'{8}' Server:'{9}'";
					this.currentSyncLogSession.LogVerbose((TSLID)48UL, SendAsManager.Tracer, (long)this.GetHashCode(), format, new object[]
					{
						sendAsState,
						guid,
						smtpAddress,
						text,
						text2,
						subscription.SendAsState,
						subscription.SubscriptionGuid,
						subscription.UserEmailAddress,
						text3,
						text4
					});
					flag = true;
				}
			}
			else
			{
				this.currentSyncLogSession.LogVerbose((TSLID)49UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Failed to parse SendAsValidatedEmail.  Creating a new signed hash.", new object[0]);
				flag = true;
			}
			if (flag)
			{
				subscription.SendAsState = SendAsState.Disabled;
				subscription.SendAsValidatedEmail = this.CreateSignedHash(subscription, out sharedSecret);
			}
			this.SendVerificationEmail(subscription, sharedSecret, emailSender);
		}

		public void EnableSendAs(PimAggregationSubscription subscription, bool meetsEnableCriteria, IEmailSender emailSender)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("emailSender", emailSender);
			if (meetsEnableCriteria)
			{
				this.currentSyncLogSession.LogVerbose((TSLID)50UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Send as has been enabled on this subscription.  Subscription id: {0}, type: {1}, meetsEnableCriteria: {2}", new object[]
				{
					subscription.SubscriptionGuid,
					subscription.SubscriptionType,
					meetsEnableCriteria
				});
				subscription.SendAsState = SendAsState.Enabled;
				Guid guid;
				subscription.SendAsValidatedEmail = this.CreateSignedHash(subscription, out guid);
				return;
			}
			this.DisableSendAs(subscription, emailSender);
		}

		public void DisableSendAs(PimAggregationSubscription subscription, IEmailSender emailSender)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("emailSender", emailSender);
			this.currentSyncLogSession.LogVerbose((TSLID)51UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Send as has been disabled on this subscription.  Subscription id: {0}, type: {1}", new object[]
			{
				subscription.SubscriptionGuid,
				subscription.SubscriptionType
			});
			subscription.SendAsState = SendAsState.Disabled;
			Guid sharedSecret;
			subscription.SendAsValidatedEmail = this.CreateSignedHash(subscription, out sharedSecret);
			this.SendVerificationEmail(subscription, sharedSecret, emailSender);
		}

		public bool IsSubscriptionEnabled(ISendAsSource subscription)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			return subscription.IsEnabled;
		}

		public bool IsSendAsEnabled(ISendAsSource source)
		{
			if (source is TransactionalRequestJob)
			{
				return true;
			}
			PimAggregationSubscription pimAggregationSubscription = source as PimAggregationSubscription;
			SyncUtilities.ThrowIfArgumentNull("subscription", pimAggregationSubscription);
			string verifiedSubscriptionUserName;
			string verifiedSubscriptionIncomingServer;
			this.GetVerifiedSubscriptionData(pimAggregationSubscription, out verifiedSubscriptionUserName, out verifiedSubscriptionIncomingServer);
			return this.IsSendAsEnabled(pimAggregationSubscription.SubscriptionGuid, pimAggregationSubscription.UserEmailAddress, pimAggregationSubscription.SendAsState, pimAggregationSubscription.SendAsNeedsVerification, verifiedSubscriptionUserName, verifiedSubscriptionIncomingServer, pimAggregationSubscription.SendAsValidatedEmail);
		}

		public bool TryGetSendAsSubscription(MessageItem messageItem, MailboxSession mailboxSession, out ISendAsSource source)
		{
			source = null;
			SyncUtilities.ThrowIfArgumentNull("messageItem", messageItem);
			PimAggregationSubscription pimAggregationSubscription = null;
			TransactionalRequestJob transactionalRequestJob = null;
			object property = SendAsManager.GetProperty(messageItem, MessageItemSchema.SharingInstanceGuid);
			if (property != null && !PropertyError.IsPropertyNotFound(property) && (Guid)property != Guid.Empty)
			{
				this.TryLoadSubscription(mailboxSession, (Guid)property, out pimAggregationSubscription);
				if (pimAggregationSubscription != null)
				{
					source = pimAggregationSubscription;
					return true;
				}
				this.TryLoadSyncJob(mailboxSession, (Guid)property, out transactionalRequestJob);
				source = transactionalRequestJob;
			}
			return transactionalRequestJob != null;
		}

		public bool TryGetSendAsSubscription(MailboxSession mailboxSession, out ISendAsSource subscription, out bool foundMultipleSubscriptions)
		{
			subscription = null;
			foundMultipleSubscriptions = false;
			List<AggregationSubscription> allSubscriptions = this.GetAllSubscriptions(mailboxSession, AggregationSubscriptionType.All);
			foreach (AggregationSubscription aggregationSubscription in allSubscriptions)
			{
				PimAggregationSubscription pimAggregationSubscription = aggregationSubscription as PimAggregationSubscription;
				if (pimAggregationSubscription != null && this.IsSubscriptionEnabled(pimAggregationSubscription) && this.IsSendAsEnabled(pimAggregationSubscription))
				{
					if (subscription != null)
					{
						this.currentSyncLogSession.LogError((TSLID)52UL, SendAsManager.Tracer, "Found more than one enabled subscription", new object[0]);
						foundMultipleSubscriptions = true;
						subscription = null;
						break;
					}
					subscription = pimAggregationSubscription;
				}
			}
			if (subscription == null)
			{
				this.currentSyncLogSession.LogError((TSLID)53UL, SendAsManager.Tracer, "Did not find a unique send as subscription", new object[0]);
				return false;
			}
			this.currentSyncLogSession.LogVerbose((TSLID)54UL, SendAsManager.Tracer, "Found unique send as subscription: {0}", new object[]
			{
				subscription.SourceGuid
			});
			return true;
		}

		public bool IsValidSendAsMessage(ISendAsSource source, MessageItem messageItem)
		{
			if (source is TransactionalRequestJob)
			{
				return true;
			}
			PimAggregationSubscription pimAggregationSubscription = source as PimAggregationSubscription;
			SyncUtilities.ThrowIfArgumentNull("subscription", pimAggregationSubscription);
			SyncUtilities.ThrowIfArgumentNull("messageItem", messageItem);
			bool result = false;
			string text = SendAsManager.GetProperty(messageItem, ItemSchema.SentRepresentingType) as string;
			string text2 = SendAsManager.GetProperty(messageItem, ItemSchema.SentRepresentingEmailAddress) as string;
			string text3 = SendAsManager.GetProperty(messageItem, ItemSchema.SentRepresentingDisplayName) as string;
			if (string.Equals(text, "SMTP", StringComparison.OrdinalIgnoreCase) && string.Equals(text2, pimAggregationSubscription.UserEmailAddress.ToString(), StringComparison.OrdinalIgnoreCase) && string.Equals(text3, pimAggregationSubscription.UserDisplayName, StringComparison.OrdinalIgnoreCase))
			{
				result = true;
			}
			else
			{
				this.currentSyncLogSession.LogVerbose((TSLID)55UL, SendAsManager.Tracer, (long)this.GetHashCode(), "The sent representing properties on the message don't match the subscription.  Subscription id: {0}, type: {1}, email: {2}, display name: {3}", new object[]
				{
					pimAggregationSubscription.SubscriptionGuid,
					text,
					text2,
					text3
				});
			}
			return result;
		}

		public SendAsError MarkMessageForSendAs(MessageItem messageItem, Guid subscriptionGuid, MailboxSession mailboxSession)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNull("messageItem", messageItem);
			List<AggregationSubscription> allSubscriptions = this.GetAllSubscriptions(mailboxSession, AggregationSubscriptionType.AllEMail);
			PimAggregationSubscription subscription = null;
			int num = allSubscriptions.FindIndex(delegate(AggregationSubscription foundSubscription)
			{
				subscription = (foundSubscription as PimAggregationSubscription);
				return subscription != null && subscription.SubscriptionGuid == subscriptionGuid;
			});
			if (num == -1)
			{
				messageItem[MessageItemSchema.SharingInstanceGuid] = subscriptionGuid;
				return SendAsError.InvalidSubscriptionGuid;
			}
			bool flag = subscription.SendAsCapable && SubscriptionManager.IsValidForSendAs(subscription.SendAsState, subscription.Status);
			messageItem[MessageItemSchema.SharingInstanceGuid] = subscription.SubscriptionGuid;
			messageItem[ItemSchema.SentRepresentingType] = "smtp";
			messageItem[ItemSchema.SentRepresentingEmailAddress] = subscription.UserEmailAddress.ToString();
			messageItem[ItemSchema.SentRepresentingDisplayName] = subscription.UserDisplayName;
			if (!flag)
			{
				return SendAsError.SubscriptionDisabledForSendAs;
			}
			return SendAsError.Success;
		}

		public void SaveSubscriptionProperties(ISendAsSource source, IExtendedPropertyCollection properties)
		{
			SyncUtilities.ThrowIfArgumentNull("source", source);
			SyncUtilities.ThrowIfArgumentNull("properties", properties);
			PimAggregationSubscription pimAggregationSubscription = source as PimAggregationSubscription;
			TransactionalRequestJob transactionalRequestJob = source as TransactionalRequestJob;
			properties.SetValue<Guid>("Microsoft.Exchange.Transport.Sync.SendAs.SubscriptionGuid", source.SourceGuid);
			properties.SetValue<int>("Microsoft.Exchange.Transport.Sync.SendAs.SubscriptionType", (int)((pimAggregationSubscription != null) ? pimAggregationSubscription.SubscriptionType : ((AggregationSubscriptionType)1)));
			properties.SetValue<int>("Microsoft.Exchange.Transport.Sync.SendAs.SendAsState", (int)((pimAggregationSubscription != null) ? pimAggregationSubscription.SendAsState : SendAsState.Enabled));
			properties.SetValue<string>("Microsoft.Exchange.Transport.Sync.SendAs.SendAsValidatedEmail", (pimAggregationSubscription != null) ? pimAggregationSubscription.SendAsValidatedEmail : null);
			properties.SetValue<string>("Microsoft.Exchange.Transport.Sync.SendAs.UserDisplayName", (pimAggregationSubscription != null) ? pimAggregationSubscription.UserDisplayName : transactionalRequestJob.RemoteCredentialUsername);
			properties.SetValue<string>("Microsoft.Exchange.Transport.Sync.SendAs.UserEmailAddress", (pimAggregationSubscription != null) ? ((string)pimAggregationSubscription.UserEmailAddress) : ((string)transactionalRequestJob.EmailAddress));
			string value;
			string value2;
			this.GetVerifiedSubscriptionData(source, out value, out value2);
			properties.SetValue<bool>("Microsoft.Exchange.Transport.Sync.SendAs.Verified.IsVerifiedSubscription", pimAggregationSubscription != null && pimAggregationSubscription.SendAsNeedsVerification);
			properties.SetValue<string>("Microsoft.Exchange.Transport.Sync.SendAs.Verified.UserName", value);
			properties.SetValue<string>("Microsoft.Exchange.Transport.Sync.SendAs.Verified.IncomingServer", value2);
		}

		public bool TryLoadSubscriptionProperties(IDictionary<string, object> properties, out SendAsManager.SendAsProperties sendAsProperties)
		{
			SyncUtilities.ThrowIfArgumentNull("properties", properties);
			sendAsProperties = SendAsManager.SendAsProperties.Invalid;
			object obj;
			object obj2;
			object obj3;
			object obj4;
			object obj5;
			object obj6;
			object obj7;
			object obj8;
			object obj9;
			if (properties.TryGetValue("Microsoft.Exchange.Transport.Sync.SendAs.SubscriptionGuid", out obj) && obj is Guid && properties.TryGetValue("Microsoft.Exchange.Transport.Sync.SendAs.SubscriptionType", out obj2) && obj2 is int && properties.TryGetValue("Microsoft.Exchange.Transport.Sync.SendAs.SendAsState", out obj3) && obj3 is int && properties.TryGetValue("Microsoft.Exchange.Transport.Sync.SendAs.SendAsValidatedEmail", out obj4) && obj4 is string && properties.TryGetValue("Microsoft.Exchange.Transport.Sync.SendAs.UserDisplayName", out obj5) && obj5 is string && properties.TryGetValue("Microsoft.Exchange.Transport.Sync.SendAs.UserEmailAddress", out obj6) && obj6 is string && properties.TryGetValue("Microsoft.Exchange.Transport.Sync.SendAs.Verified.UserName", out obj7) && obj7 is string && properties.TryGetValue("Microsoft.Exchange.Transport.Sync.SendAs.Verified.IncomingServer", out obj8) && obj8 is string && properties.TryGetValue("Microsoft.Exchange.Transport.Sync.SendAs.Verified.IsVerifiedSubscription", out obj9) && obj9 is bool)
			{
				this.SetUpSyncLogSessionWithContext((Guid)obj);
				this.TryLoadSubscriptionProperties((Guid)obj, (int)obj2, (int)obj3, (bool)obj9, (string)obj4, (string)obj5, (string)obj6, (string)obj7, (string)obj8, out sendAsProperties);
				return true;
			}
			return false;
		}

		public void SetSendAsHeaders(HeaderList headers, string originalFromEmailAddress, string originalFromDisplayName, string subscriptionEmailAddress, string subscriptionDisplayName)
		{
			SyncUtilities.ThrowIfArgumentNull("headers", headers);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("originalFromEmailAddress", originalFromEmailAddress);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("subscriptionEmailAddress", subscriptionEmailAddress);
			MimeRecipient recipient = new MimeRecipient(originalFromDisplayName, originalFromEmailAddress);
			MimeRecipient recipient2 = new MimeRecipient(subscriptionDisplayName, subscriptionEmailAddress);
			this.SetAddressHeader(headers, HeaderId.From, recipient2);
			this.SetAddressHeader(headers, HeaderId.Sender, recipient);
		}

		public void UpdateSubscriptionWithDiagnostics(PimAggregationSubscription subscription, IEmailSender emailSender)
		{
			SyncUtilities.ThrowIfArgumentNull("subscription", subscription);
			SyncUtilities.ThrowIfArgumentNull("emailSender", emailSender);
			subscription.VerificationEmailState = this.GetVerificationEmailState(emailSender);
			if (subscription.VerificationEmailState == VerificationEmailState.EmailSent)
			{
				subscription.VerificationEmailMessageId = emailSender.MessageId;
			}
			if (subscription.VerificationEmailState != VerificationEmailState.EmailNotSent)
			{
				subscription.VerificationEmailTimeStamp = new DateTime?(DateTime.UtcNow);
			}
		}

		internal bool TryParseSignedHash(string hash, bool doesSubscriptionNeedVerification, out SendAsState sendAsState, out Guid subscriptionId, out Guid sharedSecret, out SmtpAddress email, out string verifiedSubscriptionUserName, out string verifiedSubscriptionIncomingServer)
		{
			sendAsState = SendAsState.None;
			subscriptionId = Guid.Empty;
			sharedSecret = Guid.Empty;
			email = SmtpAddress.Empty;
			verifiedSubscriptionUserName = string.Empty;
			verifiedSubscriptionIncomingServer = string.Empty;
			string decryptedHash;
			if (!this.TryDecryptSignedHash(hash, out decryptedHash))
			{
				return false;
			}
			if (doesSubscriptionNeedVerification)
			{
				string text;
				string text2;
				string text3;
				string text4;
				string text5;
				string text6;
				if (!SendAsManager.VerifiedSubscriptionHashUtility.TryParseHashContents(decryptedHash, out text, out text2, out text3, out text4, out text5, out text6, this.currentSyncLogSession))
				{
					return false;
				}
				if (this.TryParseSendAsState(text, out sendAsState) && this.TryParseGuid(text2, out subscriptionId) && this.TryParseGuid(text3, out sharedSecret) && this.TryParseSmtpAddress(text5, out email))
				{
					verifiedSubscriptionUserName = text4;
					verifiedSubscriptionIncomingServer = text6;
					return true;
				}
				this.currentSyncLogSession.LogVerbose((TSLID)56UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Failed to parse individual components. RawSendAsState: {0} RawSubscriptionId: {1} RawSharedSecret: {2} RawEmailAddress: {3} RawUserName: {4} RawIncomingServer: {5}", new object[]
				{
					text,
					text2,
					text3,
					text5,
					text4,
					text6
				});
			}
			else
			{
				string text7;
				string text8;
				string text9;
				if (!SendAsManager.SubscriptionHashUtility.TryParseHashComponents(decryptedHash, out text7, out text8, out text9, this.currentSyncLogSession))
				{
					return false;
				}
				if (this.TryParseSendAsState(text7, out sendAsState) && this.TryParseGuid(text8, out subscriptionId) && this.TryParseSmtpAddress(text9, out email))
				{
					return true;
				}
				this.currentSyncLogSession.LogVerbose((TSLID)57UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Failed to parse individual components. RawSendAsState: {0} RawSubscriptionId: {1} RawEmailAddress: {2}", new object[]
				{
					text7,
					text8,
					text9
				});
			}
			return false;
		}

		protected virtual string CreateSignedHashFrom(string toEncode)
		{
			return this.exchangeGroupKeyObject.ClearStringToEncryptedString(toEncode);
		}

		protected virtual Guid CreateSharedSecret()
		{
			return Guid.NewGuid();
		}

		protected virtual bool TryDecryptSignedHash(string signedHash, out string decryptedHash)
		{
			decryptedHash = string.Empty;
			Exception ex = null;
			SecureString secureString = null;
			if (!this.exchangeGroupKeyObject.TryEncryptedStringToSecureString(signedHash, out secureString, out ex))
			{
				this.currentSyncLogSession.LogError((TSLID)58UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Failed to Decrypt hash:{0}, error:{1}", new object[]
				{
					signedHash,
					ex
				});
				return false;
			}
			decryptedHash = SyncUtilities.SecureStringToString(secureString);
			return decryptedHash != null;
		}

		protected virtual bool TryLoadSubscription(MailboxSession session, Guid subscriptionId, out PimAggregationSubscription subscription)
		{
			try
			{
				subscription = (SubscriptionManager.GetSubscription(session, subscriptionId) as PimAggregationSubscription);
				if (subscription != null)
				{
					this.SetUpSyncLogSessionWithContext(session.MailboxGuid, subscription);
				}
			}
			catch (ObjectNotFoundException)
			{
				this.currentSyncLogSession.LogVerbose((TSLID)59UL, SendAsManager.Tracer, (long)this.GetHashCode(), "The subscription was not found.  Subscription id: {0}", new object[]
				{
					subscriptionId
				});
				subscription = null;
			}
			return subscription != null;
		}

		protected virtual bool TryLoadSyncJob(MailboxSession session, Guid subscriptionId, out TransactionalRequestJob requestJob)
		{
			Guid databaseGuid = session.MailboxOwner.MailboxInfo.GetDatabaseGuid();
			try
			{
				using (RequestJobProvider requestJobProvider = new RequestJobProvider(databaseGuid))
				{
					RequestJobObjectId identity = new RequestJobObjectId(subscriptionId, databaseGuid, null);
					requestJobProvider.AttachToMDB(databaseGuid);
					requestJob = (requestJobProvider.Read<TransactionalRequestJob>(identity) as TransactionalRequestJob);
				}
			}
			catch (NotEnoughInformationToFindMoveRequestPermanentException)
			{
				requestJob = null;
			}
			return requestJob != null;
		}

		protected virtual List<AggregationSubscription> GetAllSubscriptions(MailboxSession session, AggregationSubscriptionType aggregationSubscriptionType)
		{
			SyncUtilities.ThrowIfArgumentNull("mailboxSession", session);
			return SubscriptionManager.GetAllSubscriptions(session, aggregationSubscriptionType);
		}

		private static object GetProperty(MessageItem messageItem, PropertyDefinition propertyDefinition)
		{
			try
			{
				return messageItem.TryGetProperty(propertyDefinition);
			}
			catch (StoragePermanentException)
			{
			}
			catch (StorageTransientException)
			{
			}
			return null;
		}

		private string CreateSignedHash(PimAggregationSubscription subscription, out Guid generatedSharedSecret)
		{
			generatedSharedSecret = Guid.Empty;
			string toEncode;
			if (subscription.SendAsNeedsVerification)
			{
				string rawUserName;
				string rawIncomingServer;
				this.GetVerifiedSubscriptionData(subscription, out rawUserName, out rawIncomingServer);
				generatedSharedSecret = this.CreateSharedSecret();
				toEncode = SendAsManager.VerifiedSubscriptionHashUtility.MakeHashContents((int)subscription.SendAsState, subscription.SubscriptionGuid.ToString("N"), generatedSharedSecret.ToString("N"), rawUserName, subscription.UserEmailAddress.ToString(), rawIncomingServer, this.currentSyncLogSession);
			}
			else
			{
				toEncode = SendAsManager.SubscriptionHashUtility.MakeHashContents((int)subscription.SendAsState, subscription.SubscriptionGuid.ToString("N"), subscription.UserEmailAddress.ToString());
			}
			return this.CreateSignedHashFrom(toEncode);
		}

		private VerificationEmailState GetVerificationEmailState(IEmailSender emailSender)
		{
			if (!emailSender.SendAttempted)
			{
				return VerificationEmailState.EmailNotSent;
			}
			if (emailSender.SendSuccessful)
			{
				return VerificationEmailState.EmailSent;
			}
			return VerificationEmailState.EmailFailedToSend;
		}

		private bool TryLoadSubscriptionProperties(Guid subscriptionGuid, int subscriptionType, int sendAsState, bool isVerifiedSubscription, string sendAsValidatedEmail, string userDisplayName, string userEmailAddress, string verifiedSubscriptionUserName, string verifiedSubscriptionIncomingServer, out SendAsManager.SendAsProperties sendAsProperties)
		{
			SmtpAddress userEmailAddress2 = new SmtpAddress(userEmailAddress);
			if (!userEmailAddress2.IsValidAddress)
			{
				sendAsProperties = SendAsManager.SendAsProperties.Invalid;
				this.currentSyncLogSession.LogError((TSLID)60UL, SendAsManager.Tracer, (long)this.GetHashCode(), "This message has send as properties, but the email address is invalid.  Subscription guid: {0}", new object[]
				{
					subscriptionGuid
				});
				return false;
			}
			if (!this.IsSendAsEnabled(subscriptionGuid, userEmailAddress2, (SendAsState)sendAsState, isVerifiedSubscription, verifiedSubscriptionUserName, verifiedSubscriptionIncomingServer, sendAsValidatedEmail))
			{
				sendAsProperties = SendAsManager.SendAsProperties.Invalid;
				this.currentSyncLogSession.LogError((TSLID)61UL, SendAsManager.Tracer, (long)this.GetHashCode(), "This message has send as properties, but they are invalid. Subscription guid: {0}", new object[]
				{
					subscriptionGuid
				});
				return false;
			}
			sendAsProperties = new SendAsManager.SendAsProperties(subscriptionGuid, (AggregationSubscriptionType)subscriptionType, userDisplayName, userEmailAddress2);
			return true;
		}

		private bool TryParseGuid(string value, out Guid guid)
		{
			try
			{
				guid = new Guid(value);
				return true;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}
			guid = Guid.Empty;
			return false;
		}

		private bool TryParseSmtpAddress(string value, out SmtpAddress smtpAddress)
		{
			smtpAddress = new SmtpAddress(value);
			return smtpAddress.IsValidAddress;
		}

		private bool TryParseSendAsState(string value, out SendAsState sendAsState)
		{
			sendAsState = SendAsState.None;
			int num;
			if (!int.TryParse(value, out num))
			{
				return false;
			}
			sendAsState = (SendAsState)num;
			return EnumValidator.IsValidValue<SendAsState>(sendAsState);
		}

		private bool IsSendAsEnabled(Guid subscriptionGuid, SmtpAddress userEmailAddress, SendAsState sendAsState, bool isVerifiedSubscription, string verifiedSubscriptionUserName, string verifiedSubscriptionIncomingServer, string sendAsValidatedEmail)
		{
			if (sendAsState != SendAsState.Enabled)
			{
				this.currentSyncLogSession.LogVerbose((TSLID)62UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Send as is not enabled on the subscription.  Subscription id: {0}", new object[]
				{
					subscriptionGuid
				});
				return false;
			}
			if (!this.IsSendAsValidatedEmailValid(sendAsValidatedEmail, sendAsState, isVerifiedSubscription, subscriptionGuid, userEmailAddress, verifiedSubscriptionUserName, verifiedSubscriptionIncomingServer))
			{
				this.currentSyncLogSession.LogVerbose((TSLID)63UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Send as is marked as enabled, but the SendAsValidatedEmail property is invalid.  Subscription id: {0}, email address: {1}, actual SendAsValidatedEmail value: {2}", new object[]
				{
					subscriptionGuid,
					userEmailAddress,
					sendAsValidatedEmail
				});
				return false;
			}
			return true;
		}

		private bool IsSendAsValidatedEmailValid(string sendAsValidatedEmail, SendAsState sendAsState, bool isVerifiedSubscription, Guid subscriptionGuid, SmtpAddress userEmailAddress, string verifiedSubscriptionUserName, string verifiedSubscriptionIncomingServer)
		{
			SendAsState sendAsState2;
			Guid a;
			Guid guid;
			SmtpAddress value;
			string a2;
			string a3;
			return this.TryParseSignedHash(sendAsValidatedEmail, isVerifiedSubscription, out sendAsState2, out a, out guid, out value, out a2, out a3) && (sendAsState2 == sendAsState && a == subscriptionGuid && value == userEmailAddress && a2 == verifiedSubscriptionUserName) && a3 == verifiedSubscriptionIncomingServer;
		}

		private void GetVerifiedSubscriptionData(ISendAsSource source, out string verifiedSubscriptionUserName, out string verifiedSubscriptionIncomingServer)
		{
			PimAggregationSubscription pimAggregationSubscription = source as PimAggregationSubscription;
			TransactionalRequestJob transactionalRequestJob = source as TransactionalRequestJob;
			verifiedSubscriptionUserName = string.Empty;
			verifiedSubscriptionIncomingServer = string.Empty;
			if (pimAggregationSubscription != null && !pimAggregationSubscription.SendAsNeedsVerification)
			{
				this.currentSyncLogSession.LogVerbose((TSLID)64UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Subscription does not need SendAs Verification.  No user name or password to retrieve.", new object[0]);
				return;
			}
			verifiedSubscriptionUserName = ((pimAggregationSubscription != null) ? pimAggregationSubscription.VerifiedUserName : transactionalRequestJob.RemoteCredentialUsername);
			verifiedSubscriptionIncomingServer = ((pimAggregationSubscription != null) ? pimAggregationSubscription.VerifiedIncomingServer : transactionalRequestJob.RemoteHostName);
			this.currentSyncLogSession.LogVerbose((TSLID)65UL, SendAsManager.Tracer, (long)this.GetHashCode(), "Retrieved verified data.  User name: {0} Incoming server: {1}", new object[]
			{
				verifiedSubscriptionUserName,
				verifiedSubscriptionIncomingServer
			});
		}

		private void SetAddressHeader(HeaderList headerList, HeaderId headerId, MimeRecipient recipient)
		{
			AddressHeader addressHeader = headerList.FindFirst(headerId) as AddressHeader;
			if (addressHeader == null)
			{
				addressHeader = (AddressHeader)Header.Create(headerId);
				headerList.AppendChild(addressHeader);
			}
			addressHeader.RemoveAll();
			addressHeader.AppendChild(recipient);
		}

		private void SendVerificationEmail(PimAggregationSubscription subscription, Guid sharedSecret, IEmailSender emailSender)
		{
			emailSender.SendWith(sharedSecret);
			this.UpdateSubscriptionWithDiagnostics(subscription, emailSender);
		}

		private void SetUpSyncLogSessionWithContext(Guid mailboxGuid, PimAggregationSubscription subscription)
		{
			this.currentSyncLogSession = this.currentSyncLogSession.OpenWithContext(mailboxGuid, subscription);
		}

		private void SetUpSyncLogSessionWithContext(Guid subscriptionId)
		{
			this.currentSyncLogSession = this.currentSyncLogSession.OpenWithContext(subscriptionId);
		}

		private ExchangeGroupKey exchangeGroupKeyObject = new ExchangeGroupKey(null, "Microsoft Exchange DKM");

		private SyncLogSession currentSyncLogSession = CommonLoggingHelper.SyncLogSession;

		public struct SendAsProperties
		{
			public SendAsProperties(Guid subscriptionGuid, AggregationSubscriptionType subscriptionType, string userDisplayName, SmtpAddress userEmailAddress)
			{
				this.SubscriptionGuid = subscriptionGuid;
				this.SubscriptionType = subscriptionType;
				this.UserDisplayName = userDisplayName;
				this.UserEmailAddress = userEmailAddress;
			}

			public bool IsValid
			{
				get
				{
					return this.SubscriptionGuid != Guid.Empty;
				}
			}

			public static readonly SendAsManager.SendAsProperties Invalid = default(SendAsManager.SendAsProperties);

			public readonly Guid SubscriptionGuid;

			public readonly AggregationSubscriptionType SubscriptionType;

			public readonly string UserDisplayName;

			public readonly SmtpAddress UserEmailAddress;
		}

		private static class VerifiedSubscriptionHashUtility
		{
			public static string MakeHashContents(int rawSendAsState, string rawSubscriptionId, string rawSharedSecret, string rawUserName, string rawEmailAddress, string rawIncomingServer, SyncLogSession syncLogSession)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3} {4} {5}", new object[]
				{
					rawSendAsState,
					rawSubscriptionId,
					rawSharedSecret,
					HttpUtility.UrlEncode(rawUserName),
					HttpUtility.UrlEncode(rawEmailAddress),
					HttpUtility.UrlEncode(rawIncomingServer)
				});
			}

			public static bool TryParseHashContents(string decryptedHash, out string rawSendAsState, out string rawSubscriptionId, out string rawSharedSecret, out string rawUserName, out string rawEmailAddress, out string rawIncomingServer, SyncLogSession syncLogSession)
			{
				string[] array = decryptedHash.Split(new char[]
				{
					' '
				});
				if (array.Length != 6)
				{
					syncLogSession.LogVerbose((TSLID)66UL, SendAsManager.Tracer, "Incorrect number of values in signed hash.  Expected: {0} Actual: {1}", new object[]
					{
						6,
						array.Length
					});
					rawSendAsState = string.Empty;
					rawSubscriptionId = string.Empty;
					rawSharedSecret = string.Empty;
					rawUserName = string.Empty;
					rawEmailAddress = string.Empty;
					rawIncomingServer = string.Empty;
					return false;
				}
				rawSendAsState = array[0];
				rawSubscriptionId = array[1];
				rawSharedSecret = array[2];
				rawUserName = HttpUtility.UrlDecode(array[3]);
				rawEmailAddress = HttpUtility.UrlDecode(array[4]);
				rawIncomingServer = HttpUtility.UrlDecode(array[5]);
				return true;
			}

			private const int VerifiedSubscriptionHashComponentCount = 6;
		}

		private static class SubscriptionHashUtility
		{
			public static string MakeHashContents(int rawSendAsState, string rawSubscriptionId, string rawEmailAddress)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", new object[]
				{
					rawSendAsState,
					rawSubscriptionId,
					HttpUtility.UrlEncode(rawEmailAddress)
				});
			}

			public static bool TryParseHashComponents(string decryptedHash, out string rawSendAsState, out string rawSubscriptionId, out string rawEmailAddress, SyncLogSession syncLogSession)
			{
				string[] array = decryptedHash.Split(new char[]
				{
					' '
				});
				if (array.Length != 3)
				{
					syncLogSession.LogVerbose((TSLID)67UL, SendAsManager.Tracer, "Incorrect number of values in signed hash.  Expected: {0} Actual: {1}", new object[]
					{
						3,
						array.Length
					});
					rawSendAsState = string.Empty;
					rawSubscriptionId = string.Empty;
					rawEmailAddress = string.Empty;
					return false;
				}
				rawSendAsState = array[0];
				rawSubscriptionId = array[1];
				rawEmailAddress = HttpUtility.UrlDecode(array[2]);
				return true;
			}

			private const int RegularSubscriptionHashComponentCount = 3;
		}

		private static class Constants
		{
			public const string SmtpAddressType = "SMTP";

			public static class SubscriptionProperty
			{
				public const string Prefix = "Microsoft.Exchange.Transport.Sync.SendAs.";

				public const string VerifiedPrefix = "Microsoft.Exchange.Transport.Sync.SendAs.Verified.";

				public const string SubscriptionGuid = "Microsoft.Exchange.Transport.Sync.SendAs.SubscriptionGuid";

				public const string SubscriptionType = "Microsoft.Exchange.Transport.Sync.SendAs.SubscriptionType";

				public const string SendAsState = "Microsoft.Exchange.Transport.Sync.SendAs.SendAsState";

				public const string SendAsValidatedEmail = "Microsoft.Exchange.Transport.Sync.SendAs.SendAsValidatedEmail";

				public const string UserDisplayName = "Microsoft.Exchange.Transport.Sync.SendAs.UserDisplayName";

				public const string UserEmailAddress = "Microsoft.Exchange.Transport.Sync.SendAs.UserEmailAddress";

				public const string IsVerifiedSubscription = "Microsoft.Exchange.Transport.Sync.SendAs.Verified.IsVerifiedSubscription";

				public const string VerifiedSubscriptionUserName = "Microsoft.Exchange.Transport.Sync.SendAs.Verified.UserName";

				public const string VerifiedSubscriptionIncomingServer = "Microsoft.Exchange.Transport.Sync.SendAs.Verified.IncomingServer";
			}
		}
	}
}
