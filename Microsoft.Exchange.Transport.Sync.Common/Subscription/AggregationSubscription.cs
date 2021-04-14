using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Dkm;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class AggregationSubscription : ISyncWorkerData
	{
		protected AggregationSubscription() : this(CommonLoggingHelper.SyncLogSession)
		{
		}

		protected AggregationSubscription(SyncLogSession syncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			this.syncLogSession = syncLogSession;
			this.LastSuccessfulSyncTime = null;
			this.LastSyncTime = null;
			this.lastSyncNowRequestTime = null;
			this.Status = AggregationStatus.InProgress;
			this.subscriptionIdentity = new AggregationSubscriptionIdentity();
			this.subscriptionIdentity.SubscriptionId = Guid.NewGuid();
			this.foldersToExclude = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
			this.subscriptionEvents = SubscriptionEvents.None;
			this.adjustedLastSuccessfulSyncTime = DateTime.UtcNow;
			this.version = AggregationSubscription.CurrentSupportedVersion;
		}

		public static long CurrentSupportedVersion
		{
			get
			{
				return AggregationSubscription.currentSupportedVersion;
			}
			set
			{
				AggregationSubscription.currentSupportedVersion = value;
			}
		}

		public bool IsNew
		{
			get
			{
				return this.SubscriptionMessageId == null;
			}
		}

		public string Diagnostics
		{
			get
			{
				return this.diagnostics;
			}
			set
			{
				this.diagnostics = value;
			}
		}

		public AggregationSubscriptionIdentity SubscriptionIdentity
		{
			get
			{
				return this.subscriptionIdentity;
			}
		}

		public Guid SubscriptionGuid
		{
			get
			{
				return this.subscriptionIdentity.SubscriptionId;
			}
		}

		public string UserLegacyDN
		{
			get
			{
				return this.subscriptionIdentity.LegacyDN;
			}
		}

		public string PrimaryMailboxUserLegacyDN
		{
			get
			{
				return this.subscriptionIdentity.PrimaryMailboxLegacyDN;
			}
		}

		public ADObjectId AdUserId
		{
			get
			{
				return this.subscriptionIdentity.AdUserId;
			}
		}

		public AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return this.subscriptionType;
			}
			set
			{
				this.subscriptionType = value;
			}
		}

		public long Version
		{
			get
			{
				return this.version;
			}
			protected set
			{
				this.version = value;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.isValid;
			}
		}

		public AggregationType AggregationType
		{
			get
			{
				return this.aggregationType;
			}
			set
			{
				if (AggregationType.Mirrored == value)
				{
					throw new SyncPropertyValidationException(AggregationSubscriptionMessageSchema.SharingAggregationType.ToString(), value.ToString(), new InvalidOperationException("Mirrored subscriptions are not supported"));
				}
				this.aggregationType = value;
			}
		}

		public SyncPhase SyncPhase
		{
			get
			{
				return this.syncPhase;
			}
			set
			{
				if (value < this.syncPhase)
				{
					throw new InvalidOperationException("Sync phase can only move forward, not backwards");
				}
				this.syncPhase = value;
			}
		}

		public SubscriptionCreationType CreationType
		{
			get
			{
				return this.creationType;
			}
			set
			{
				this.creationType = value;
			}
		}

		public string UserExchangeMailboxDisplayName
		{
			get
			{
				return this.userExchangeMailboxDisplayName;
			}
			set
			{
				this.userExchangeMailboxDisplayName = value;
			}
		}

		public string UserExchangeMailboxSmtpAddress
		{
			get
			{
				return this.userExchangeMailboxSmtpAddress;
			}
			set
			{
				this.userExchangeMailboxSmtpAddress = value;
			}
		}

		public int SubscriptionGeneralConfig
		{
			get
			{
				return this.subscriptionGeneralConfig;
			}
			set
			{
				this.subscriptionGeneralConfig = value;
			}
		}

		public int SubscriptionProtocolVersion
		{
			get
			{
				return this.subscriptionProtocolVersion;
			}
			set
			{
				this.subscriptionProtocolVersion = value;
			}
		}

		public string SubscriptionProtocolName
		{
			get
			{
				return this.subscriptionProtocolName;
			}
			set
			{
				this.subscriptionProtocolName = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public AggregationStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		public DetailedAggregationStatus DetailedAggregationStatus
		{
			get
			{
				return this.detailedAggregationStatus;
			}
			set
			{
				if (!EnumValidator.IsValidValue<DetailedAggregationStatus>(value))
				{
					CommonLoggingHelper.SyncLogSession.ReportWatson(string.Format("Invalid DetailedAggregationStatus {0}", value));
					this.detailedAggregationStatus = DetailedAggregationStatus.None;
					return;
				}
				this.detailedAggregationStatus = value;
			}
		}

		public string PoisonCallstack
		{
			get
			{
				return this.poisonCallstack;
			}
			set
			{
				this.poisonCallstack = value;
			}
		}

		public DateTime LastModifiedTime
		{
			get
			{
				return this.lastModifiedTime;
			}
		}

		public DateTime? LastSyncTime
		{
			get
			{
				return this.lastSyncTime;
			}
			set
			{
				this.lastSyncTime = value;
			}
		}

		public DateTime? LastSyncNowRequestTime
		{
			get
			{
				return this.lastSyncNowRequestTime;
			}
			set
			{
				this.lastSyncNowRequestTime = value;
			}
		}

		public DateTime? LastSuccessfulSyncTime
		{
			get
			{
				return this.lastSuccessfulSyncTime;
			}
			set
			{
				this.lastSuccessfulSyncTime = value;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}

		public DateTime AdjustedLastSuccessfulSyncTime
		{
			get
			{
				return this.adjustedLastSuccessfulSyncTime;
			}
			set
			{
				this.adjustedLastSuccessfulSyncTime = value;
			}
		}

		public string OutageDetectionDiagnostics
		{
			get
			{
				return this.outageDetectionDiagnostics;
			}
			set
			{
				this.outageDetectionDiagnostics = value;
			}
		}

		public virtual int? EnumeratedItemsLimitPerConnection
		{
			get
			{
				return null;
			}
		}

		public virtual bool SendAsCapable
		{
			get
			{
				return true;
			}
		}

		public string TypeFullName
		{
			get
			{
				if (this.instanceTypeFullName == null)
				{
					this.instanceTypeFullName = base.GetType().FullName;
				}
				return this.instanceTypeFullName;
			}
		}

		public StoreObjectId SubscriptionMessageId
		{
			get
			{
				return this.subscriptionMessageId;
			}
			set
			{
				this.subscriptionMessageId = value;
			}
		}

		public SubscriptionEvents SubscriptionEvents
		{
			get
			{
				return this.subscriptionEvents;
			}
			set
			{
				this.subscriptionEvents = value;
			}
		}

		public long ItemsSynced
		{
			get
			{
				return this.itemsSynced;
			}
		}

		public long ItemsSkipped
		{
			get
			{
				return this.itemsSkipped;
			}
		}

		public long? TotalItemsInSourceMailbox
		{
			get
			{
				if (this.totalItemsInSourceMailbox == SyncUtilities.DataNotAvailable)
				{
					return null;
				}
				return new long?(this.totalItemsInSourceMailbox);
			}
			set
			{
				if (value != null)
				{
					this.totalItemsInSourceMailbox = value.Value;
					return;
				}
				this.totalItemsInSourceMailbox = SyncUtilities.DataNotAvailable;
			}
		}

		public long? TotalSizeOfSourceMailbox
		{
			get
			{
				if (this.totalSizeOfSourceMailbox == SyncUtilities.DataNotAvailable)
				{
					return null;
				}
				return new long?(this.totalSizeOfSourceMailbox);
			}
			set
			{
				if (value != null)
				{
					this.totalSizeOfSourceMailbox = value.Value;
					return;
				}
				this.totalSizeOfSourceMailbox = SyncUtilities.DataNotAvailable;
			}
		}

		public byte[] InstanceKey
		{
			get
			{
				return this.instanceKey;
			}
			set
			{
				this.instanceKey = value;
			}
		}

		public abstract bool IsMirrored { get; }

		public bool IsAggregation
		{
			get
			{
				return this.AggregationType == AggregationType.Aggregation;
			}
		}

		public bool IsMigration
		{
			get
			{
				return this.AggregationType == AggregationType.Migration;
			}
		}

		public bool IsPartnerProtocol
		{
			get
			{
				return this.SubscriptionType == AggregationSubscriptionType.DeltaSyncMail;
			}
		}

		public bool IsInitialSyncDone
		{
			get
			{
				return this.syncPhase != SyncPhase.Initial;
			}
		}

		public bool WasInitialSyncDone
		{
			get
			{
				return this.wasInitialSyncDone;
			}
		}

		public bool Inactive
		{
			get
			{
				return this.status == AggregationStatus.Disabled || this.status == AggregationStatus.Poisonous;
			}
		}

		public bool InitialSyncInRecoveryMode
		{
			get
			{
				return this.initialSyncInRecoveryMode;
			}
			internal set
			{
				this.initialSyncInRecoveryMode = value;
			}
		}

		public string FoldersToExclude
		{
			get
			{
				return AggregationSubscription.SerializeFoldersToExclude(this.foldersToExclude);
			}
		}

		public virtual string IncomingServerName
		{
			get
			{
				return this.SubscriptionType.ToString();
			}
		}

		public virtual int IncomingServerPort
		{
			get
			{
				return -1;
			}
		}

		public virtual string AuthenticationType
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual string EncryptionType
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual SmtpAddress Email
		{
			get
			{
				return SmtpAddress.Empty;
			}
		}

		public virtual string Domain
		{
			get
			{
				return string.Empty;
			}
		}

		public abstract FolderSupport FolderSupport { get; }

		public abstract ItemSupport ItemSupport { get; }

		public abstract SyncQuirks SyncQuirks { get; }

		public static AggregationSubscriptionType GetSubscriptionKind(MessageItem message)
		{
			string className = message.ClassName;
			return AggregationSubscription.GetSubscriptionKind(className);
		}

		public static AggregationSubscriptionType GetSubscriptionKind(string messageClass)
		{
			if (messageClass.Equals("IPM.Aggregation.Pop", StringComparison.OrdinalIgnoreCase))
			{
				return AggregationSubscriptionType.Pop;
			}
			if (messageClass.Equals("IPM.Aggregation.DeltaSync", StringComparison.OrdinalIgnoreCase))
			{
				return AggregationSubscriptionType.DeltaSyncMail;
			}
			if (messageClass.Equals("IPM.Aggregation.IMAP", StringComparison.OrdinalIgnoreCase))
			{
				return AggregationSubscriptionType.IMAP;
			}
			if (messageClass.Equals("IPM.Aggregation.Facebook", StringComparison.OrdinalIgnoreCase))
			{
				return AggregationSubscriptionType.Facebook;
			}
			if (messageClass.Equals("IPM.Aggregation.LinkedIn", StringComparison.OrdinalIgnoreCase))
			{
				return AggregationSubscriptionType.LinkedIn;
			}
			return AggregationSubscriptionType.Unknown;
		}

		public override bool Equals(object obj)
		{
			AggregationSubscription otherSubscription = obj as AggregationSubscription;
			return this.Equals(otherSubscription);
		}

		public bool Equals(AggregationSubscription otherSubscription)
		{
			return otherSubscription != null && otherSubscription.SubscriptionGuid == this.SubscriptionGuid;
		}

		public override int GetHashCode()
		{
			return this.SubscriptionGuid.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} : {1}", new object[]
			{
				this.SubscriptionType.ToString(),
				this.UserExchangeMailboxDisplayName
			});
		}

		public void LoadSubscription(MessageItem message, ADObjectId adUserId, string legacyDN)
		{
			SyncUtilities.ThrowIfArgumentNull("message", message);
			this.isValid = false;
			bool flag = false;
			bool flag2 = false;
			if (!this.TryLoadSubscriptionVersion(message, out flag2))
			{
				flag = true;
			}
			this.baseLoadMinimumInfoCalled = false;
			this.subscriptionIdentity.AdUserId = adUserId;
			this.subscriptionIdentity.LegacyDN = legacyDN;
			this.subscriptionIdentity.PrimaryMailboxLegacyDN = legacyDN;
			bool flag3 = this.LoadMinimumInfo(message);
			if (!flag && flag3)
			{
				this.baseLoadPropertiesCalled = false;
				try
				{
					this.LoadProperties(message);
					this.isValid = true;
					return;
				}
				catch (SyncPropertyValidationException ex)
				{
					this.SetPropertyLoadError(ex.Property, ex.Value);
				}
			}
			if (!flag && flag2)
			{
				this.MarkSubscriptionInvalidVersion(message);
				return;
			}
			this.MarkSubscriptionCorrupted(message);
		}

		public void SetToMessageObject(MessageItem message)
		{
			SyncUtilities.ThrowIfArgumentNull("message", message);
			this.baseSetPropertiesToMessageObject = false;
			this.SetPropertiesToMessageObject(message);
		}

		public void SetFoldersToExclude(IEnumerable<string> foldersToExclude)
		{
			this.foldersToExclude.Clear();
			if (foldersToExclude != null)
			{
				foreach (string text in foldersToExclude)
				{
					if (!string.IsNullOrEmpty(text))
					{
						this.foldersToExclude.Add(text);
					}
				}
			}
		}

		public bool ShouldFolderBeExcluded(string folderName, char folderSeparator)
		{
			if (this.foldersToExclude.Contains(folderName))
			{
				return true;
			}
			foreach (string arg in this.foldersToExclude)
			{
				if (folderName.StartsWith(arg + folderSeparator, StringComparison.CurrentCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public void UpdateItemStatistics(long itemsSynced, long itemsSkipped)
		{
			SyncUtilities.ThrowIfArgumentLessThanZero("itemsSynced", itemsSynced);
			SyncUtilities.ThrowIfArgumentLessThanZero("itemsSkipped", itemsSkipped);
			this.itemsSynced += itemsSynced;
			this.itemsSkipped += itemsSkipped;
		}

		public void AppendOutageDetectionDiagnostics(string machineName, Guid databaseGuid, TimeSpan configuredOutageDetectionThreshold, TimeSpan observedOutageDuration)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("machineName", machineName);
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			this.outageDetectionDiagnostics += string.Format(CultureInfo.InvariantCulture, "Machine: {0} ,Date: {1}, Database: {2}, Threshold: {3}, Observed Duration: {4}{5}", new object[]
			{
				machineName,
				ExDateTime.UtcNow,
				databaseGuid,
				configuredOutageDetectionThreshold,
				observedOutageDuration,
				Environment.NewLine
			});
		}

		internal static string SerializeFoldersToExclude(IEnumerable<string> foldersToExclude)
		{
			if (foldersToExclude == null)
			{
				throw new ArgumentException("FoldersToExclude cannot be null");
			}
			StringBuilder stringBuilder = new StringBuilder(265);
			foreach (string value in foldersToExclude)
			{
				stringBuilder.Append(value);
				stringBuilder.Append(AggregationSubscription.FolderExclusionDelimiter);
			}
			return stringBuilder.ToString();
		}

		internal static HashSet<string> DeserializeFoldersToExclude(string s)
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (!string.IsNullOrEmpty(s))
			{
				string[] array = s.Split(new string[]
				{
					AggregationSubscription.FolderExclusionDelimiter
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string item in array)
				{
					hashSet.Add(item);
				}
			}
			return hashSet;
		}

		internal static void DeserializeVersionAndSubscriptionType(BinaryReader reader, out long version, out AggregationSubscriptionType subscriptionType)
		{
			AggregationSubscription.SubscriptionDeserializer subscriptionDeserializer = new AggregationSubscription.SubscriptionDeserializer(reader);
			subscriptionDeserializer.DeserializeVersionAndSubscriptionType(out version, out subscriptionType);
		}

		internal void Serialize(BinaryWriter writer)
		{
			AggregationSubscription.SubscriptionSerializer serializer = new AggregationSubscription.SubscriptionSerializer(writer);
			this.Serialize(serializer);
		}

		internal void Deserialize(BinaryReader reader)
		{
			AggregationSubscription.SubscriptionDeserializer deserializer = new AggregationSubscription.SubscriptionDeserializer(reader);
			this.Deserialize(deserializer);
		}

		internal bool IsUpgradeRequired()
		{
			return this.isValid && AggregationSubscription.CurrentSupportedVersion > this.version;
		}

		internal void UpgradeSubscription()
		{
			if (!this.isValid)
			{
				throw new InvalidOperationException("Can't upgrade invalid subscription");
			}
			this.version = AggregationSubscription.CurrentSupportedVersion;
		}

		internal void MarkSubscriptionInvalidVersion(MessageItem message)
		{
			this.diagnostics += string.Format(CultureInfo.InvariantCulture, "\r\nExpected version {0}, but Subscription has version {1}.", new object[]
			{
				AggregationSubscription.CurrentSupportedVersion,
				this.version
			});
			this.status = AggregationStatus.InvalidVersion;
			this.detailedAggregationStatus = DetailedAggregationStatus.None;
			message.OpenAsReadWrite();
			message[AggregationSubscriptionMessageSchema.SharingAggregationStatus] = AggregationStatus.InvalidVersion;
			this.isValid = false;
		}

		internal void MarkSubscriptionCorrupted(MessageItem message)
		{
			this.status = AggregationStatus.Poisonous;
			this.detailedAggregationStatus = DetailedAggregationStatus.Corrupted;
			try
			{
				message.OpenAsReadWrite();
				this.baseSetMinimumInfoToMessageObject = false;
				this.SetMinimumInfoToMessageObject(message);
				this.SaveMessage(message);
			}
			catch (LocalizedException ex)
			{
				this.syncLogSession.LogError((TSLID)41UL, "AggregationSubscription.MarkSubscriptionCorrupted: Failed to mark subscription as corrupted. Subscription ID {0}, Exception: {1}.", new object[]
				{
					message.Id.ObjectId,
					ex
				});
			}
			this.isValid = false;
		}

		protected abstract void Serialize(AggregationSubscription.SubscriptionSerializer serializer);

		protected abstract void Deserialize(AggregationSubscription.SubscriptionDeserializer deserializer);

		protected virtual void SetPropertiesToMessageObject(MessageItem message)
		{
			this.baseSetPropertiesToMessageObject = true;
			message[MessageItemSchema.SharingSubscriptionVersion] = AggregationSubscription.CurrentSupportedVersion;
			message[AggregationSubscriptionMessageSchema.SharingSubscriptionCreationType] = (int)this.CreationType;
			message[AggregationSubscriptionMessageSchema.SharingSubscriptionSyncPhase] = (int)this.SyncPhase;
			message[AggregationSubscriptionMessageSchema.SharingAggregationProtocolName] = this.SubscriptionProtocolName;
			message[AggregationSubscriptionMessageSchema.SharingAggregationProtocolVersion] = this.SubscriptionProtocolVersion;
			message[MessageItemSchema.SharingInstanceGuid] = this.SubscriptionGuid;
			message[AggregationSubscriptionMessageSchema.SharingSubscriptionConfiguration] = this.SubscriptionGeneralConfig;
			message[AggregationSubscriptionMessageSchema.SharingSubscriptionName] = this.Name;
			message[MessageItemSchema.SharingDetailedStatus] = this.DetailedAggregationStatus;
			message[AggregationSubscriptionMessageSchema.SharingAggregationStatus] = this.Status;
			this.SetDiagnosticInfoToMessageObject(message);
			if (this.LastSuccessfulSyncTime == null)
			{
				message[AggregationSubscriptionMessageSchema.SharingLastSuccessSyncTime] = SyncUtilities.ExZeroTime;
			}
			else
			{
				message[AggregationSubscriptionMessageSchema.SharingLastSuccessSyncTime] = new ExDateTime(ExTimeZone.UtcTimeZone, this.LastSuccessfulSyncTime.Value.ToUniversalTime());
			}
			if (this.LastSyncTime == null)
			{
				message[MessageItemSchema.SharingLastSync] = SyncUtilities.ExZeroTime;
			}
			else
			{
				message[MessageItemSchema.SharingLastSync] = new ExDateTime(ExTimeZone.UtcTimeZone, this.LastSyncTime.Value.ToUniversalTime());
			}
			if (this.LastSyncNowRequestTime == null)
			{
				message[AggregationSubscriptionMessageSchema.SharingLastSyncNowRequest] = SyncUtilities.ExZeroTime;
			}
			else
			{
				message[AggregationSubscriptionMessageSchema.SharingLastSyncNowRequest] = new ExDateTime(ExTimeZone.UtcTimeZone, this.LastSyncNowRequestTime.Value.ToUniversalTime());
			}
			message[AggregationSubscriptionMessageSchema.SharingMigrationState] = ((this.IsInitialSyncDone ? 16 : 0) | (this.IsMigration ? 1 : 0));
			message[AggregationSubscriptionMessageSchema.SharingAggregationType] = (int)this.AggregationType;
			if (this.poisonCallstack != null && this.poisonCallstack.Length > 0)
			{
				Exception ex = null;
				string value = null;
				using (SecureString secureString = SyncUtilities.StringToSecureString(this.poisonCallstack))
				{
					if (this.TrySecureStringToEncryptedString(secureString, out value, out ex))
					{
						message[AggregationSubscriptionMessageSchema.SharingPoisonCallstack] = value;
					}
					else
					{
						this.syncLogSession.LogError((TSLID)39UL, ExTraceGlobals.SubscriptionManagerTracer, "Failed to Encrypt Callstack due to error: {0}, for Subscription: ID {1}, Name {2}, Protocol {3}.", new object[]
						{
							ex,
							this.SubscriptionGuid,
							this.Name,
							this.SubscriptionProtocolName
						});
					}
					goto IL_2DF;
				}
			}
			message[AggregationSubscriptionMessageSchema.SharingPoisonCallstack] = string.Empty;
			IL_2DF:
			message[AggregationSubscriptionMessageSchema.SharingSubscriptionEvents] = this.subscriptionEvents;
			string value2 = AggregationSubscription.SerializeFoldersToExclude(this.foldersToExclude);
			if (!string.IsNullOrEmpty(value2))
			{
				message[AggregationSubscriptionMessageSchema.SharingSubscriptionExclusionFolders] = value2;
			}
			message[AggregationSubscriptionMessageSchema.SharingSubscriptionItemsSynced] = this.itemsSynced;
			message[AggregationSubscriptionMessageSchema.SharingSubscriptionItemsSkipped] = this.itemsSkipped;
			message[AggregationSubscriptionMessageSchema.SharingSubscriptionTotalItemsInSourceMailbox] = this.totalItemsInSourceMailbox;
			message[AggregationSubscriptionMessageSchema.SharingSubscriptionTotalSizeOfSourceMailbox] = this.totalSizeOfSourceMailbox;
			message[AggregationSubscriptionMessageSchema.SharingAdjustedLastSuccessfulSyncTime] = new ExDateTime(ExTimeZone.UtcTimeZone, this.adjustedLastSuccessfulSyncTime.ToUniversalTime());
			message[AggregationSubscriptionMessageSchema.SharingOutageDetectionDiagnostics] = this.TruncateDiagnosticInformation(this.outageDetectionDiagnostics);
			message[AggregationSubscriptionMessageSchema.SharingInitialSyncInRecoveryMode] = this.initialSyncInRecoveryMode;
		}

		protected virtual bool TrySecureStringToEncryptedString(SecureString secureString, out string encryptedString, out Exception exception)
		{
			return AggregationSubscription.ExchangeGroupKeyObject.TrySecureStringToEncryptedString(secureString, out encryptedString, out exception);
		}

		protected virtual bool TryEncryptedStringToSecureString(string encryptedString, out SecureString secureString, out Exception exception)
		{
			return AggregationSubscription.ExchangeGroupKeyObject.TryEncryptedStringToSecureString(encryptedString, out secureString, out exception);
		}

		protected virtual bool LoadMinimumInfo(MessageItem message)
		{
			this.baseLoadMinimumInfoCalled = true;
			bool result = true;
			this.subscriptionIdentity.SubscriptionId = SyncUtilities.SafeGetProperty<Guid>(message, MessageItemSchema.SharingInstanceGuid);
			this.diagnostics = SyncUtilities.SafeGetProperty<string>(message, MessageItemSchema.SharingDiagnostics, string.Empty);
			if (this.subscriptionIdentity.SubscriptionId == Guid.Empty)
			{
				this.SetPropertyLoadError("SharingInstanceGuid", this.subscriptionIdentity.SubscriptionId.ToString());
				this.subscriptionIdentity.SubscriptionId = Guid.NewGuid();
				result = false;
			}
			try
			{
				this.GetEnumProperty<AggregationStatus>(message, AggregationSubscriptionMessageSchema.SharingAggregationStatus, null, out this.status);
			}
			catch (SyncPropertyValidationException ex)
			{
				this.SetPropertyLoadError(ex.Property, ex.Value);
				result = false;
			}
			try
			{
				this.GetEnumProperty<DetailedAggregationStatus>(message, MessageItemSchema.SharingDetailedStatus, null, true, out this.detailedAggregationStatus);
			}
			catch (SyncPropertyValidationException ex2)
			{
				this.SetPropertyLoadError(ex2.Property, ex2.Value);
				result = false;
			}
			return result;
		}

		protected virtual void LoadProperties(MessageItem message)
		{
			this.baseLoadPropertiesCalled = true;
			this.SubscriptionType = AggregationSubscription.GetSubscriptionKind(message);
			this.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingAggregationProtocolName, true, null, null, out this.subscriptionProtocolName);
			this.GetIntProperty(message, AggregationSubscriptionMessageSchema.SharingAggregationProtocolVersion, new int?(0), null, out this.subscriptionProtocolVersion);
			this.GetIntProperty(message, AggregationSubscriptionMessageSchema.SharingSubscriptionConfiguration, null, null, out this.subscriptionGeneralConfig);
			this.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingSubscriptionName, true, null, null, out this.name);
			this.creationTime = (DateTime)message.CreationTime;
			this.lastModifiedTime = (DateTime)message.LastModifiedTime;
			ExDateTime? exDateTime;
			this.GetExDateTimeProperty(message, MessageItemSchema.SharingLastSync, out exDateTime);
			this.lastSyncTime = (DateTime?)exDateTime;
			ExDateTime? exDateTime2;
			this.GetExDateTimeProperty(message, AggregationSubscriptionMessageSchema.SharingLastSuccessSyncTime, out exDateTime2);
			this.lastSuccessfulSyncTime = (DateTime?)exDateTime2;
			bool flag = false;
			if (this.version >= 2L)
			{
				this.GetEnumProperty<SubscriptionCreationType>(message, AggregationSubscriptionMessageSchema.SharingSubscriptionCreationType, null, out this.creationType);
				int num = SyncUtilities.SafeGetProperty<int>(message, AggregationSubscriptionMessageSchema.SharingMigrationState);
				flag = (this.wasInitialSyncDone = ((num & 16) != 0));
				if ((num & 1) != 0)
				{
					this.AggregationType = AggregationType.Migration;
				}
				string text;
				this.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingPoisonCallstack, true, true, null, null, out text);
				if (!string.IsNullOrEmpty(text))
				{
					Exception ex = null;
					SecureString secureString = null;
					if (this.TryEncryptedStringToSecureString(text, out secureString, out ex))
					{
						this.poisonCallstack = SyncUtilities.SecureStringToString(secureString);
					}
					else
					{
						this.syncLogSession.LogError((TSLID)40UL, ExTraceGlobals.SubscriptionManagerTracer, "Failed to Decrypt poison callstack due to error: {0}, for Subscription: ID {1}, Name {2}, Protocol {3}.", new object[]
						{
							ex,
							this.SubscriptionGuid,
							this.Name,
							this.SubscriptionProtocolName
						});
					}
				}
			}
			if (this.version >= 4L)
			{
				this.GetEnumProperty<SubscriptionEvents>(message, AggregationSubscriptionMessageSchema.SharingSubscriptionEvents, null, out this.subscriptionEvents);
			}
			else
			{
				this.subscriptionEvents = SubscriptionEvents.None;
			}
			if (this.version >= 5L)
			{
				string empty = string.Empty;
				this.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingSubscriptionExclusionFolders, true, true, null, null, out empty);
				this.SetFoldersToExclude(AggregationSubscription.DeserializeFoldersToExclude(empty));
			}
			else
			{
				this.SetFoldersToExclude(null);
			}
			if (this.version >= 7L)
			{
				this.GetLongProperty(message, AggregationSubscriptionMessageSchema.SharingSubscriptionItemsSynced, new long?(0L), null, out this.itemsSynced);
				this.GetLongProperty(message, AggregationSubscriptionMessageSchema.SharingSubscriptionItemsSkipped, new long?(0L), null, out this.itemsSkipped);
			}
			else
			{
				this.itemsSynced = 0L;
				this.itemsSkipped = 0L;
			}
			if (this.version >= 8L)
			{
				ExDateTime exDateTime3;
				this.GetExDateTimeProperty(message, AggregationSubscriptionMessageSchema.SharingAdjustedLastSuccessfulSyncTime, out exDateTime3);
				this.adjustedLastSuccessfulSyncTime = (DateTime)exDateTime3;
				this.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingOutageDetectionDiagnostics, true, false, null, null, out this.outageDetectionDiagnostics);
			}
			else if (this.lastSuccessfulSyncTime != null)
			{
				this.adjustedLastSuccessfulSyncTime = this.lastSuccessfulSyncTime.Value;
			}
			else
			{
				this.adjustedLastSuccessfulSyncTime = this.creationTime;
			}
			if (this.version >= 9L)
			{
				this.GetEnumProperty<SyncPhase>(message, AggregationSubscriptionMessageSchema.SharingSubscriptionSyncPhase, null, out this.syncPhase);
			}
			else
			{
				this.syncPhase = (flag ? SyncPhase.Incremental : SyncPhase.Initial);
			}
			if (this.version >= 10L)
			{
				ExDateTime? exDateTime4;
				this.GetExDateTimeProperty(message, AggregationSubscriptionMessageSchema.SharingLastSyncNowRequest, out exDateTime4);
				this.lastSyncNowRequestTime = (DateTime?)exDateTime4;
			}
			if (this.Version >= 11L)
			{
				try
				{
					this.GetLongProperty(message, AggregationSubscriptionMessageSchema.SharingSubscriptionTotalItemsInSourceMailbox, null, null, out this.totalItemsInSourceMailbox);
				}
				catch (SyncPropertyValidationException ex2)
				{
					this.syncLogSession.LogError((TSLID)322UL, ExTraceGlobals.SubscriptionManagerTracer, "Failed to load SharingSubscriptionTotalItemsInSourceMailbox property due to error: {0}, for Subscription: ID {1}, Name {2}, Protocol {3}.", new object[]
					{
						ex2,
						this.SubscriptionGuid,
						this.Name,
						this.SubscriptionProtocolName
					});
				}
				try
				{
					this.GetLongProperty(message, AggregationSubscriptionMessageSchema.SharingSubscriptionTotalSizeOfSourceMailbox, null, null, out this.totalSizeOfSourceMailbox);
				}
				catch (SyncPropertyValidationException ex3)
				{
					this.syncLogSession.LogError((TSLID)323UL, ExTraceGlobals.SubscriptionManagerTracer, "Failed to load SharingSubscriptionTotalSizeOfSourceMailbox property due to error: {0}, for Subscription: ID {1}, Name {2}, Protocol {3}.", new object[]
					{
						ex3,
						this.SubscriptionGuid,
						this.Name,
						this.SubscriptionProtocolName
					});
				}
			}
			if (this.Version >= 13L)
			{
				this.GetEnumProperty<AggregationType>(message, AggregationSubscriptionMessageSchema.SharingAggregationType, null, out this.aggregationType);
				object value;
				this.GetProperty(message, AggregationSubscriptionMessageSchema.SharingInitialSyncInRecoveryMode, out value);
				this.initialSyncInRecoveryMode = Convert.ToBoolean(value);
			}
		}

		protected virtual void SetMinimumInfoToMessageObject(MessageItem message)
		{
			this.baseSetMinimumInfoToMessageObject = true;
			message[MessageItemSchema.SharingInstanceGuid] = this.subscriptionIdentity.SubscriptionId;
			message[AggregationSubscriptionMessageSchema.SharingAggregationStatus] = (int)this.status;
			message[MessageItemSchema.SharingDetailedStatus] = (int)this.detailedAggregationStatus;
			this.SetDiagnosticInfoToMessageObject(message);
		}

		protected void SetPropertyLoadError(string propertyName, string value)
		{
			this.diagnostics += string.Format(CultureInfo.InvariantCulture, "\r\nNot able to load {0} property or value loaded doesn't match expected type. Value loaded is: {1}", new object[]
			{
				propertyName,
				value
			});
		}

		protected void GetEnumProperty<T>(MessageItem message, StorePropertyDefinition propertyDefinition, int? enumMask, out T propertyValue) where T : struct
		{
			this.GetEnumProperty<T>(message, propertyDefinition, enumMask, false, out propertyValue);
		}

		protected void GetEnumProperty<T>(MessageItem message, StorePropertyDefinition propertyDefinition, int? enumMask, bool useDefaultIfPropertyInvalid, out T propertyValue) where T : struct
		{
			object obj = null;
			Exception innerException = null;
			try
			{
				this.GetProperty(message, propertyDefinition, out obj);
				propertyValue = (T)((object)obj);
				if (enumMask != null)
				{
					int num = Convert.ToInt32(propertyValue) & enumMask.Value;
					propertyValue = (T)((object)num);
				}
				if (EnumValidator.IsValidValue<T>(propertyValue))
				{
					return;
				}
				if (useDefaultIfPropertyInvalid)
				{
					propertyValue = default(T);
					return;
				}
			}
			catch (FormatException ex)
			{
				innerException = ex;
			}
			throw new SyncPropertyValidationException(propertyDefinition.ToString(), (obj == null) ? "Null" : obj.ToString(), innerException);
		}

		protected void GetIntProperty(MessageItem message, StorePropertyDefinition propertyDefinition, int? minValue, int? maxValue, out int propertyValue)
		{
			propertyValue = 0;
			object obj = null;
			Exception innerException = null;
			try
			{
				this.GetProperty(message, propertyDefinition, out obj);
				propertyValue = (int)obj;
				if ((minValue == null || !(propertyValue < minValue)) && (maxValue == null || !(propertyValue > maxValue)))
				{
					return;
				}
			}
			catch (FormatException ex)
			{
				innerException = ex;
			}
			throw new SyncPropertyValidationException(propertyDefinition.ToString(), (obj == null) ? "Null" : obj.ToString(), innerException);
		}

		protected void GetLongProperty(MessageItem message, StorePropertyDefinition propertyDefinition, long? minValue, long? maxValue, out long propertyValue)
		{
			propertyValue = 0L;
			object obj = null;
			Exception innerException = null;
			try
			{
				this.GetProperty(message, propertyDefinition, out obj);
				propertyValue = (long)obj;
				if ((minValue == null || !(propertyValue < minValue)) && (maxValue == null || !(propertyValue > maxValue)))
				{
					return;
				}
			}
			catch (FormatException ex)
			{
				innerException = ex;
			}
			throw new SyncPropertyValidationException(propertyDefinition.ToString(), (obj == null) ? "Null" : obj.ToString(), innerException);
		}

		protected void GetDoubleProperty(MessageItem message, StorePropertyDefinition propertyDefinition, double? minValue, double? maxValue, out double propertyValue)
		{
			propertyValue = 0.0;
			object obj = null;
			Exception innerException = null;
			try
			{
				this.GetProperty(message, propertyDefinition, out obj);
				propertyValue = (double)obj;
				if (minValue != null)
				{
					double num = propertyValue;
					double? num2 = minValue;
					if (num < num2.GetValueOrDefault() && num2 != null)
					{
						goto IL_76;
					}
				}
				if (maxValue != null)
				{
					double num3 = propertyValue;
					double? num4 = maxValue;
					if (num3 > num4.GetValueOrDefault() && num4 != null)
					{
						goto IL_76;
					}
				}
				return;
				IL_76:;
			}
			catch (FormatException ex)
			{
				innerException = ex;
			}
			throw new SyncPropertyValidationException(propertyDefinition.ToString(), (obj == null) ? "Null" : obj.ToString(), innerException);
		}

		protected void GetSmtpAddressProperty(MessageItem message, StorePropertyDefinition propertyDefinition, out SmtpAddress propertyValue)
		{
			object obj = null;
			Exception innerException = null;
			try
			{
				this.GetProperty(message, propertyDefinition, out obj);
				SmtpAddress smtpAddress = new SmtpAddress(obj.ToString());
				if (smtpAddress.IsValidAddress)
				{
					propertyValue = smtpAddress;
					return;
				}
			}
			catch (FormatException ex)
			{
				innerException = ex;
			}
			throw new SyncPropertyValidationException(propertyDefinition.ToString(), (obj == null) ? "Null" : obj.ToString(), innerException);
		}

		protected void GetFqdnProperty(MessageItem message, StorePropertyDefinition propertyDefinition, out Fqdn server, out int port)
		{
			object obj = null;
			Exception innerException = null;
			try
			{
				this.GetProperty(message, propertyDefinition, out obj);
				string text = (string)obj;
				if (!string.IsNullOrEmpty(text))
				{
					int num = text.IndexOf(':');
					if (num > 0 && Fqdn.TryParse(text.Remove(num), out server))
					{
						port = int.Parse(text.Remove(0, num + 1));
						if (port >= 0 && port <= 65535)
						{
							return;
						}
					}
				}
			}
			catch (FormatException ex)
			{
				innerException = ex;
			}
			throw new SyncPropertyValidationException(propertyDefinition.ToString(), (obj == null) ? "Null" : obj.ToString(), innerException);
		}

		protected void GetExDateTimeProperty(MessageItem message, StorePropertyDefinition propertyDefinition, out ExDateTime propertyValue)
		{
			object obj = null;
			this.GetProperty(message, propertyDefinition, out obj);
			ExDateTime? exDateTime = obj as ExDateTime?;
			if (exDateTime != null)
			{
				propertyValue = exDateTime.Value;
				return;
			}
			throw new SyncPropertyValidationException(propertyDefinition.ToString(), obj.ToString());
		}

		protected void GetExDateTimeProperty(MessageItem message, StorePropertyDefinition propertyDefinition, out ExDateTime? propertyValue)
		{
			ExDateTime exDateTime;
			this.GetExDateTimeProperty(message, propertyDefinition, out exDateTime);
			if (exDateTime == SyncUtilities.ExZeroTime)
			{
				propertyValue = null;
				return;
			}
			propertyValue = new ExDateTime?(exDateTime);
		}

		protected void GetStringProperty(MessageItem message, StorePropertyDefinition propertyDefinition, bool canBeEmptyNull, uint? minLength, uint? maxLength, out string propertyValue)
		{
			propertyValue = null;
			object obj = null;
			Exception innerException = null;
			try
			{
				this.GetProperty(message, propertyDefinition, out obj);
				propertyValue = (string)obj;
				if (canBeEmptyNull)
				{
					return;
				}
				if (!string.IsNullOrEmpty(propertyValue))
				{
					if (minLength != null)
					{
						long num = (long)propertyValue.Length;
						uint? num2 = minLength;
						if (num < (long)((ulong)num2.GetValueOrDefault()) && num2 != null)
						{
							goto IL_8A;
						}
					}
					if (maxLength != null)
					{
						long num3 = (long)propertyValue.Length;
						uint? num4 = maxLength;
						if (num3 > (long)((ulong)num4.GetValueOrDefault()) && num4 != null)
						{
							goto IL_8A;
						}
					}
					return;
				}
				IL_8A:;
			}
			catch (FormatException ex)
			{
				innerException = ex;
			}
			throw new SyncPropertyValidationException(propertyDefinition.ToString(), (obj == null) ? "Null" : obj.ToString(), innerException);
		}

		protected void GetStringProperty(MessageItem message, StorePropertyDefinition propertyDefinition, bool canBeEmptyNull, bool canBeMissing, uint? minLength, uint? maxLength, out string propertyValue)
		{
			propertyValue = null;
			if (canBeMissing)
			{
				propertyValue = SyncUtilities.SafeGetProperty<string>(message, propertyDefinition);
				return;
			}
			this.GetStringProperty(message, propertyDefinition, canBeEmptyNull, minLength, maxLength, out propertyValue);
		}

		protected void GetProperty(MessageItem message, StorePropertyDefinition propertyDefinition, out object propertyValue)
		{
			propertyValue = null;
			Exception innerException = null;
			try
			{
				propertyValue = message.TryGetProperty(propertyDefinition);
				if (!(propertyValue is PropertyError))
				{
					return;
				}
			}
			catch (PropertyErrorException ex)
			{
				innerException = ex;
			}
			throw new SyncPropertyValidationException(propertyDefinition.ToString(), (propertyValue == null) ? "Null" : propertyValue.ToString(), innerException);
		}

		protected virtual StoreObjectId GetMessageId(MessageItem message)
		{
			return message.Id.ObjectId;
		}

		protected virtual void SaveMessage(MessageItem message)
		{
			message.Save(SaveMode.NoConflictResolution);
		}

		protected virtual void LoadSubscriptionIdentity(MailboxSession mailboxSession)
		{
			if (!mailboxSession.MailboxOwner.ObjectId.IsNullOrEmpty())
			{
				this.subscriptionIdentity.AdUserId = mailboxSession.MailboxOwner.ObjectId;
			}
			this.subscriptionIdentity.LegacyDN = mailboxSession.MailboxOwnerLegacyDN;
			this.subscriptionIdentity.PrimaryMailboxLegacyDN = mailboxSession.MailboxOwner.LegacyDn;
		}

		protected virtual void OpenMessageForReadWrite(MessageItem message)
		{
			message.OpenAsReadWrite();
		}

		private void SetDiagnosticInfoToMessageObject(MessageItem message)
		{
			if (this.diagnostics == null)
			{
				message[MessageItemSchema.SharingDiagnostics] = string.Empty;
				return;
			}
			message[MessageItemSchema.SharingDiagnostics] = this.TruncateDiagnosticInformation(this.diagnostics);
		}

		private string TruncateDiagnosticInformation(string diagnosticInformation)
		{
			if (diagnosticInformation.Length <= 8192)
			{
				return diagnosticInformation;
			}
			return diagnosticInformation.Substring(diagnosticInformation.Length - 8192);
		}

		private bool TryLoadSubscriptionVersion(MessageItem message, out bool isFutureVersion)
		{
			isFutureVersion = false;
			long num = SyncUtilities.SafeGetProperty<long>(message, MessageItemSchema.SharingSubscriptionVersion);
			if (num < 0L)
			{
				this.SetPropertyLoadError("SharingSubscriptionVersion", num.ToString());
				return false;
			}
			if (num > AggregationSubscription.CurrentSupportedVersion)
			{
				this.syncLogSession.LogVerbose((TSLID)507UL, "Setting version of the in-memory subscription object from {0} to {1}", new object[]
				{
					num,
					AggregationSubscription.CurrentSupportedVersion
				});
				this.version = AggregationSubscription.CurrentSupportedVersion;
				isFutureVersion = true;
			}
			else
			{
				this.version = num;
			}
			return true;
		}

		internal const int MaxDiagnosticsLength = 8192;

		private const string InvalidVersionMessage = "\r\nExpected version {0}, but Subscription has version {1}.";

		private const string PropertyLoadError = "\r\nNot able to load {0} property or value loaded doesn't match expected type. Value loaded is: {1}";

		private const int DefaultFoldersToExcludeLength = 265;

		public static readonly string FolderExclusionDelimiter = "\r\n";

		protected static readonly IExchangeGroupKey ExchangeGroupKeyObject = SyncUtilities.CreateExchangeGroupKey();

		[NonSerialized]
		private readonly SyncLogSession syncLogSession;

		private static long currentSupportedVersion = 13L;

		private HashSet<string> foldersToExclude;

		private AggregationSubscriptionIdentity subscriptionIdentity;

		private string instanceTypeFullName;

		private AggregationSubscriptionType subscriptionType;

		private long version;

		private SubscriptionCreationType creationType = SubscriptionCreationType.Manual;

		private AggregationType aggregationType;

		private SyncPhase syncPhase;

		private string userExchangeMailboxDisplayName;

		private string userExchangeMailboxSmtpAddress;

		private int subscriptionGeneralConfig;

		private int subscriptionProtocolVersion;

		private string subscriptionProtocolName;

		private string name;

		private AggregationStatus status;

		private DetailedAggregationStatus detailedAggregationStatus;

		private string diagnostics = string.Empty;

		private string poisonCallstack;

		private DateTime lastModifiedTime;

		private DateTime? lastSyncTime;

		private DateTime? lastSuccessfulSyncTime;

		private DateTime? lastSyncNowRequestTime;

		private DateTime creationTime = DateTime.MinValue.ToUniversalTime();

		private DateTime adjustedLastSuccessfulSyncTime;

		private StoreObjectId subscriptionMessageId;

		private byte[] instanceKey;

		private bool isValid = true;

		private bool baseLoadPropertiesCalled;

		private bool baseLoadMinimumInfoCalled;

		private bool baseSetPropertiesToMessageObject;

		private bool baseSetMinimumInfoToMessageObject;

		private bool wasInitialSyncDone;

		private SubscriptionEvents subscriptionEvents;

		private long itemsSynced;

		private long itemsSkipped;

		private string outageDetectionDiagnostics = string.Empty;

		private long totalItemsInSourceMailbox;

		private long totalSizeOfSourceMailbox;

		private bool initialSyncInRecoveryMode;

		protected class SubscriptionDeserializer
		{
			internal SubscriptionDeserializer(BinaryReader reader)
			{
				this.reader = reader;
			}

			internal void DeserializeVersionAndSubscriptionType(out long version, out AggregationSubscriptionType subscriptionType)
			{
				version = this.reader.ReadInt64();
				subscriptionType = this.GetEnumValue<AggregationSubscriptionType>();
			}

			internal void DeserializePopSubscription(PopAggregationSubscription popAggregationSubscription)
			{
				this.DeserializeWithExceptionHandling(delegate
				{
					this.DeserializePimSubscription(popAggregationSubscription);
					popAggregationSubscription.PopServer = new Fqdn(this.reader.ReadString());
					popAggregationSubscription.PopPort = this.reader.ReadInt32();
					popAggregationSubscription.PopLogonName = this.reader.ReadString();
					popAggregationSubscription.Flags = this.GetEnumValue<PopAggregationFlags>();
				});
			}

			internal void DeserializeImapSubscription(IMAPAggregationSubscription imapAggregationSubscription)
			{
				this.DeserializeWithExceptionHandling(delegate
				{
					this.DeserializePimSubscription(imapAggregationSubscription);
					imapAggregationSubscription.IMAPServer = new Fqdn(this.reader.ReadString());
					imapAggregationSubscription.IMAPPort = this.reader.ReadInt32();
					imapAggregationSubscription.IMAPLogOnName = this.reader.ReadString();
					imapAggregationSubscription.Flags = this.GetEnumValue<IMAPAggregationFlags>();
					imapAggregationSubscription.ImapPathPrefix = this.reader.ReadString();
				});
			}

			internal void DeserializeDeltaSyncSubscription(DeltaSyncAggregationSubscription deltaSyncAggregationSubscription)
			{
				this.DeserializeWithExceptionHandling(delegate
				{
					this.DeserializeWindowsLiveSubscription(deltaSyncAggregationSubscription);
					deltaSyncAggregationSubscription.MinSyncPollInterval = this.reader.ReadInt32();
					deltaSyncAggregationSubscription.MinSettingPollInterval = this.reader.ReadInt32();
					deltaSyncAggregationSubscription.SyncMultiplier = this.reader.ReadDouble();
					deltaSyncAggregationSubscription.MaxObjectInSync = this.reader.ReadInt32();
					deltaSyncAggregationSubscription.MaxNumberOfEmailAdds = this.reader.ReadInt32();
					deltaSyncAggregationSubscription.MaxNumberOfFolderAdds = this.reader.ReadInt32();
					deltaSyncAggregationSubscription.MaxAttachments = this.reader.ReadInt32();
					deltaSyncAggregationSubscription.MaxMessageSize = this.reader.ReadInt64();
					deltaSyncAggregationSubscription.MaxRecipients = this.reader.ReadInt32();
				});
			}

			internal void DeserializeConnectSubscription(ConnectSubscription subscription)
			{
				this.DeserializeWithExceptionHandling(delegate
				{
					this.DeserializePimSubscription(subscription);
					subscription.EncryptedAccessToken = this.reader.ReadString();
					subscription.EncryptedAccessTokenSecret = this.reader.ReadString();
				});
			}

			private void DeserializeWindowsLiveSubscription(WindowsLiveServiceAggregationSubscription subscription)
			{
				this.DeserializePimSubscription(subscription);
				subscription.IncommingServerUrl = this.reader.ReadString();
				subscription.AuthPolicy = this.reader.ReadString();
				subscription.Puid = this.reader.ReadString();
				subscription.LogonName = this.reader.ReadString();
				subscription.AuthToken = this.reader.ReadString();
				subscription.AuthTokenExpirationTime = (this.GetDateTimeValue() ?? subscription.AuthTokenExpirationTime);
			}

			private void DeserializePimSubscription(PimAggregationSubscription subscription)
			{
				this.Deserialize(subscription);
				subscription.UserDisplayName = this.reader.ReadString();
				subscription.UserEmailAddress = new SmtpAddress(this.reader.ReadString());
				subscription.EncryptedLogonPassword = this.reader.ReadString();
			}

			private void Deserialize(AggregationSubscription subscription)
			{
				this.DeserializeVersionAndSubscriptionType(out subscription.version, out subscription.subscriptionType);
				subscription.aggregationType = this.GetEnumValue<AggregationType>();
				string text = this.reader.ReadString();
				if (string.IsNullOrEmpty(text))
				{
					throw new SerializationException("Subscription Guid is not valid.");
				}
				Guid guid = new Guid(text);
				if (object.Equals(guid, Guid.Empty))
				{
					throw new SerializationException("Subscription Guid is Guid.Empty.");
				}
				string legacyDN = this.reader.ReadString();
				string primaryMailboxLegacyDN = this.reader.ReadString();
				subscription.subscriptionIdentity = new AggregationSubscriptionIdentity(guid, legacyDN, primaryMailboxLegacyDN);
				subscription.creationType = this.GetEnumValue<SubscriptionCreationType>();
				subscription.subscriptionMessageId = StoreObjectId.Deserialize(this.reader.ReadString());
				subscription.name = this.reader.ReadString();
				subscription.subscriptionEvents = this.GetEnumValue<SubscriptionEvents>();
				subscription.foldersToExclude = AggregationSubscription.DeserializeFoldersToExclude(this.reader.ReadString());
				subscription.lastSuccessfulSyncTime = this.GetDateTimeValue();
				subscription.lastSyncTime = this.GetDateTimeValue();
				subscription.adjustedLastSuccessfulSyncTime = this.GetDateTimeValue().Value;
				subscription.poisonCallstack = this.reader.ReadString();
				subscription.syncPhase = this.GetEnumValue<SyncPhase>();
				subscription.status = this.GetEnumValue<AggregationStatus>();
				subscription.detailedAggregationStatus = this.GetEnumValue<DetailedAggregationStatus>();
				subscription.itemsSynced = this.reader.ReadInt64();
				subscription.itemsSkipped = this.reader.ReadInt64();
				subscription.totalItemsInSourceMailbox = this.reader.ReadInt64();
				subscription.totalSizeOfSourceMailbox = this.reader.ReadInt64();
				subscription.outageDetectionDiagnostics = this.reader.ReadString();
				subscription.creationTime = this.GetDateTimeValue().Value;
				subscription.isValid = this.reader.ReadBoolean();
				subscription.diagnostics = this.reader.ReadString();
				subscription.userExchangeMailboxSmtpAddress = this.reader.ReadString();
			}

			private void DeserializeWithExceptionHandling(Action action)
			{
				try
				{
					action();
				}
				catch (CorruptDataException innerException)
				{
					throw new SerializationException("Subscription is not valid.", innerException);
				}
				catch (IOException innerException2)
				{
					throw new SerializationException("Subscription is not valid.", innerException2);
				}
				catch (FormatException innerException3)
				{
					throw new SerializationException("Subscription is not valid.", innerException3);
				}
				catch (CryptographicException innerException4)
				{
					throw new SerializationException("Subscription is not valid.", innerException4);
				}
				catch (InvalidDataException innerException5)
				{
					throw new SerializationException("Subscription is not valid.", innerException5);
				}
				catch (Exception ex)
				{
					if (AggregationSubscription.ExchangeGroupKeyObject.IsDkmException(ex))
					{
						throw new SerializationException("Subscription is not valid.", ex);
					}
					throw;
				}
			}

			private DateTime? GetDateTimeValue()
			{
				long num = this.reader.ReadInt64();
				if (num == 0L)
				{
					return null;
				}
				if (num < DateTime.MinValue.Ticks || num > DateTime.MaxValue.Ticks)
				{
					throw new SerializationException("Invalid dateTime in ticks.");
				}
				return new DateTime?(new DateTime(num, DateTimeKind.Utc));
			}

			private T GetEnumValue<T>() where T : struct
			{
				int value = this.reader.ReadInt32();
				T t = (T)((object)Enum.ToObject(typeof(T), value));
				if (!EnumValidator.IsValidValue<T>(t))
				{
					throw new SerializationException(string.Format("{0} is invalid: {1}", typeof(T), t));
				}
				return t;
			}

			private readonly BinaryReader reader;
		}

		protected class SubscriptionSerializer
		{
			internal SubscriptionSerializer(BinaryWriter writer)
			{
				this.writer = writer;
			}

			internal void SerializePopSubscription(PopAggregationSubscription popSubscription)
			{
				this.SerializePimSubscription(popSubscription);
				this.writer.Write(popSubscription.PopServer.ToString());
				this.writer.Write(popSubscription.PopPort);
				this.writer.Write(popSubscription.PopLogonName);
				this.writer.Write((int)popSubscription.Flags);
			}

			internal void SerializeImapSubscription(IMAPAggregationSubscription imapSubscription)
			{
				this.SerializePimSubscription(imapSubscription);
				this.writer.Write(imapSubscription.IMAPServer.ToString());
				this.writer.Write(imapSubscription.IMAPPort);
				this.writer.Write(imapSubscription.IMAPLogOnName);
				this.writer.Write((int)imapSubscription.Flags);
				this.writer.Write(string.IsNullOrEmpty(imapSubscription.ImapPathPrefix) ? string.Empty : imapSubscription.ImapPathPrefix);
			}

			internal void SerializeDeltaSyncSubscription(DeltaSyncAggregationSubscription deltaSyncAggregationSubscription)
			{
				this.SerializeWindowsLiveSubscription(deltaSyncAggregationSubscription);
				this.writer.Write(deltaSyncAggregationSubscription.MinSyncPollInterval);
				this.writer.Write(deltaSyncAggregationSubscription.MinSettingPollInterval);
				this.writer.Write(deltaSyncAggregationSubscription.SyncMultiplier);
				this.writer.Write(deltaSyncAggregationSubscription.MaxObjectInSync);
				this.writer.Write(deltaSyncAggregationSubscription.MaxNumberOfEmailAdds);
				this.writer.Write(deltaSyncAggregationSubscription.MaxNumberOfFolderAdds);
				this.writer.Write(deltaSyncAggregationSubscription.MaxAttachments);
				this.writer.Write(deltaSyncAggregationSubscription.MaxMessageSize);
				this.writer.Write(deltaSyncAggregationSubscription.MaxRecipients);
			}

			internal void SerializeConnectSubscription(ConnectSubscription subscription)
			{
				this.SerializePimSubscription(subscription);
				this.writer.Write(subscription.EncryptedAccessToken);
				this.writer.Write(subscription.EncryptedAccessTokenSecret ?? string.Empty);
			}

			private void SerializeWindowsLiveSubscription(WindowsLiveServiceAggregationSubscription subscription)
			{
				this.SerializePimSubscription(subscription);
				this.writer.Write(subscription.IncommingServerUrl);
				this.writer.Write(subscription.AuthPolicy);
				this.writer.Write(subscription.Puid ?? string.Empty);
				this.writer.Write(subscription.LogonName);
				this.writer.Write(subscription.AuthToken ?? string.Empty);
				this.WriteDateTime(new DateTime?(subscription.AuthTokenExpirationTime));
			}

			private void SerializePimSubscription(PimAggregationSubscription subscription)
			{
				this.Serialize(subscription);
				this.writer.Write(subscription.UserDisplayName);
				this.writer.Write(subscription.UserEmailAddress.ToString());
				this.writer.Write(subscription.EncryptedLogonPassword ?? string.Empty);
			}

			private void Serialize(AggregationSubscription subscription)
			{
				this.writer.Write(subscription.version);
				this.writer.Write((int)subscription.SubscriptionType);
				this.writer.Write((int)subscription.AggregationType);
				this.writer.Write(subscription.SubscriptionGuid.ToString("N"));
				this.writer.Write(subscription.UserLegacyDN);
				this.writer.Write(subscription.PrimaryMailboxUserLegacyDN);
				this.writer.Write((int)subscription.CreationType);
				this.writer.Write(subscription.SubscriptionMessageId.ToBase64String());
				this.writer.Write(subscription.Name);
				this.writer.Write((int)subscription.SubscriptionEvents);
				this.writer.Write(AggregationSubscription.SerializeFoldersToExclude(subscription.foldersToExclude));
				this.WriteDateTime(subscription.LastSuccessfulSyncTime);
				this.WriteDateTime(subscription.LastSyncTime);
				this.WriteDateTime(new DateTime?(subscription.AdjustedLastSuccessfulSyncTime));
				this.writer.Write(subscription.PoisonCallstack ?? string.Empty);
				this.writer.Write((int)subscription.SyncPhase);
				this.writer.Write((int)subscription.Status);
				this.writer.Write((int)subscription.DetailedAggregationStatus);
				this.writer.Write(subscription.ItemsSynced);
				this.writer.Write(subscription.ItemsSkipped);
				this.WriteNullableLong(subscription.TotalItemsInSourceMailbox);
				this.WriteNullableLong(subscription.TotalSizeOfSourceMailbox);
				this.writer.Write(subscription.TruncateDiagnosticInformation(subscription.OutageDetectionDiagnostics));
				this.WriteDateTime(new DateTime?(subscription.CreationTime));
				this.writer.Write(subscription.IsValid);
				this.writer.Write(subscription.TruncateDiagnosticInformation(subscription.Diagnostics));
				this.writer.Write(subscription.UserExchangeMailboxSmtpAddress);
			}

			private void WriteDateTime(DateTime? dateTime)
			{
				if (dateTime != null)
				{
					this.writer.Write(dateTime.Value.ToUniversalTime().Ticks);
					return;
				}
				this.writer.Write(0L);
			}

			private void WriteNullableLong(long? longValue)
			{
				if (longValue != null)
				{
					this.writer.Write(longValue.Value);
					return;
				}
				this.writer.Write(SyncUtilities.DataNotAvailable);
			}

			private readonly BinaryWriter writer;
		}
	}
}
