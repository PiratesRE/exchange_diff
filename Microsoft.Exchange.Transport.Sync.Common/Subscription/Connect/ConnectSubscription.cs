using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ConnectSubscription : PimAggregationSubscription, IConnectSubscription, ISyncWorkerData
	{
		public ConnectSubscription()
		{
			base.UserDisplayName = string.Empty;
			base.UserEmailAddress = SmtpAddress.Empty;
			base.AggregationType = AggregationType.PeopleConnection;
		}

		public override bool SendAsNeedsVerification
		{
			get
			{
				return false;
			}
		}

		public override bool SendAsCapable
		{
			get
			{
				return false;
			}
		}

		public override FolderSupport FolderSupport
		{
			get
			{
				return FolderSupport.ContactsOnly;
			}
		}

		public override ItemSupport ItemSupport
		{
			get
			{
				return ItemSupport.Contacts;
			}
		}

		public override SyncQuirks SyncQuirks
		{
			get
			{
				return SyncQuirks.None;
			}
		}

		public override bool PasswordRequired
		{
			get
			{
				return false;
			}
		}

		public string MessageClass { get; internal set; }

		public Guid ProviderGuid { get; internal set; }

		public ConnectState ConnectState
		{
			get
			{
				if (base.Status == AggregationStatus.Disabled)
				{
					return ConnectState.Disabled;
				}
				if (base.Status == AggregationStatus.Delayed)
				{
					return ConnectState.Delayed;
				}
				if (base.DetailedAggregationStatus == DetailedAggregationStatus.None)
				{
					return ConnectState.Connected;
				}
				return ConnectState.ConnectedNeedsToken;
			}
		}

		public string AccessTokenInClearText
		{
			get
			{
				return this.accessTokenClearText;
			}
			internal set
			{
				this.accessTokenClearText = value;
			}
		}

		public bool HasAccessToken { get; private set; }

		public int EncryptedAccessTokenHash { get; private set; }

		public string AppId { get; internal set; }

		public string UserId { get; internal set; }

		public string AccessTokenSecretInClearText
		{
			get
			{
				return this.accessTokenSecretClearText;
			}
			internal set
			{
				this.accessTokenSecretClearText = value;
			}
		}

		internal string EncryptedAccessToken
		{
			get
			{
				return this.EncryptString(this.accessTokenClearText);
			}
			set
			{
				this.accessTokenClearText = this.DecryptToUnsecureString(value);
			}
		}

		internal string EncryptedAccessTokenSecret
		{
			get
			{
				return this.EncryptString(this.accessTokenSecretClearText);
			}
			set
			{
				this.accessTokenSecretClearText = this.DecryptToUnsecureString(value);
			}
		}

		public static bool IsDkmException(Exception e)
		{
			return AggregationSubscription.ExchangeGroupKeyObject.IsDkmException(e);
		}

		public override PimSubscriptionProxy CreateSubscriptionProxy()
		{
			return new ConnectSubscriptionProxy(this);
		}

		protected override void SetPropertiesToMessageObject(MessageItem message)
		{
			base.AggregationType = AggregationType.PeopleConnection;
			base.SubscriptionEvents = SubscriptionEvents.WorkItemCompleted;
			message[StoreObjectSchema.ItemClass] = this.MessageClass;
			message[MessageItemSchema.SharingProviderGuid] = this.ProviderGuid;
			message[AggregationSubscriptionMessageSchema.SharingEncryptedAccessToken] = (this.EncryptedAccessToken ?? string.Empty);
			message[AggregationSubscriptionMessageSchema.SharingEncryptedAccessTokenSecret] = (this.EncryptedAccessTokenSecret ?? string.Empty);
			message[AggregationSubscriptionMessageSchema.SharingAppId] = this.AppId;
			message[AggregationSubscriptionMessageSchema.SharingUserId] = this.UserId;
			base.SetPropertiesToMessageObject(message);
		}

		protected override void LoadProperties(MessageItem message)
		{
			base.LoadProperties(message);
			if (base.AggregationType != AggregationType.PeopleConnection)
			{
				throw new SyncPropertyValidationException("AggregationType", base.AggregationType.ToString());
			}
			string messageClass;
			base.GetStringProperty(message, StoreObjectSchema.ItemClass, false, false, null, null, out messageClass);
			this.MessageClass = messageClass;
			this.ProviderGuid = SyncUtilities.SafeGetProperty<Guid>(message, MessageItemSchema.SharingProviderGuid);
			string appId;
			base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingAppId, false, false, null, null, out appId);
			this.AppId = appId;
			string userId;
			base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingUserId, false, false, null, null, out userId);
			this.UserId = userId;
			string text;
			base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingEncryptedAccessToken, true, true, null, null, out text);
			if (!string.IsNullOrEmpty(text))
			{
				this.accessTokenClearText = this.DecryptToUnsecureStringAndThrowPropertyValidationExceptionIfDecryptionFails(text, AggregationSubscriptionMessageSchema.SharingEncryptedAccessToken.Name);
				this.HasAccessToken = true;
				this.EncryptedAccessTokenHash = text.GetHashCode();
			}
			else
			{
				this.HasAccessToken = false;
			}
			string encrypted;
			base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingEncryptedAccessTokenSecret, true, true, null, null, out encrypted);
			this.accessTokenSecretClearText = this.DecryptToUnsecureStringAndThrowPropertyValidationExceptionIfDecryptionFails(encrypted, AggregationSubscriptionMessageSchema.SharingEncryptedAccessTokenSecret.Name);
		}

		protected override void Serialize(AggregationSubscription.SubscriptionSerializer subscriptionSerializer)
		{
			subscriptionSerializer.SerializeConnectSubscription(this);
		}

		protected override void Deserialize(AggregationSubscription.SubscriptionDeserializer deserializer)
		{
			deserializer.DeserializeConnectSubscription(this);
		}

		private string EncryptString(string s)
		{
			return AggregationSubscription.ExchangeGroupKeyObject.ClearStringToEncryptedString(s);
		}

		private string DecryptToUnsecureString(string s)
		{
			string result;
			using (SecureString secureString = AggregationSubscription.ExchangeGroupKeyObject.EncryptedStringToSecureString(s))
			{
				result = secureString.AsUnsecureString();
			}
			return result;
		}

		private string DecryptToUnsecureStringAndThrowPropertyValidationExceptionIfDecryptionFails(string encrypted, string property)
		{
			string result;
			try
			{
				result = this.DecryptToUnsecureString(encrypted);
			}
			catch (FormatException innerException)
			{
				throw new SyncPropertyValidationException(property, "<undisclosed>", innerException);
			}
			catch (CryptographicException innerException2)
			{
				throw new SyncPropertyValidationException(property, "<undisclosed>", innerException2);
			}
			catch (InvalidDataException innerException3)
			{
				throw new SyncPropertyValidationException(property, "<undisclosed>", innerException3);
			}
			catch (Exception ex)
			{
				if (ConnectSubscription.IsDkmException(ex))
				{
					throw new SyncPropertyValidationException(property, "<undisclosed>", ex);
				}
				throw;
			}
			return result;
		}

		private const string UndisclosedSentitiveData = "<undisclosed>";

		public static readonly string FacebookProtocolName = "Graph";

		public static readonly int FacebookProtocolVersion = 1;

		public static readonly Guid FacebookProviderGuid = new Guid("b7cfcba5-ec45-4712-bd37-dc0c26eb03c2");

		public static readonly string LinkedInProtocolName = "LinkedIn";

		public static readonly int LinkedInProtocolVersion = 1;

		public static readonly Guid LinkedInProviderGuid = new Guid("1c006afd-7c4e-4ce5-bbd3-b67352fdc685");

		[NonSerialized]
		private string accessTokenClearText;

		[NonSerialized]
		private string accessTokenSecretClearText;
	}
}
