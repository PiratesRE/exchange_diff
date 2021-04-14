using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SubscriptionCacheEntry
	{
		internal SubscriptionCacheEntry(GlobalSyncLogSession syncLogSession, Guid subscriptionGuid, StoreObjectId subscriptionMessageId, string userLegacyDn, Guid mailboxGuid, Guid tenantGuid, Guid externalDirectoryOrgId, AggregationSubscriptionType type, AggregationType aggregationType, DateTime? lastSyncCompletedTime, bool disabled, string incomingServerName, SyncPhase syncPhase, SerializedSubscription serializedSubscription)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			SyncUtilities.ThrowIfArgumentNull("serializedSubscription", serializedSubscription);
			SyncUtilities.ThrowIfGuidEmpty("mailboxGuid", mailboxGuid);
			SyncUtilities.ThrowIfArgumentNull("subscriptionMessageId", subscriptionMessageId);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("userLegacyDn", userLegacyDn);
			this.syncLogSession = syncLogSession;
			this.subscriptionGuid = subscriptionGuid;
			this.subscriptionMessageId = subscriptionMessageId;
			this.userLegacyDn = userLegacyDn;
			this.mailboxGuid = mailboxGuid;
			this.tenantGuid = tenantGuid;
			this.externalDirectoryOrgId = externalDirectoryOrgId;
			this.subscriptionType = type;
			this.aggregationType = aggregationType;
			this.disabled = disabled;
			this.incomingServerName = incomingServerName;
			this.syncPhase = syncPhase;
			this.serializedSubscription = serializedSubscription;
			if (lastSyncCompletedTime != null)
			{
				this.lastSyncCompletedTime = new ExDateTime?(new ExDateTime(ExTimeZone.UtcTimeZone, lastSyncCompletedTime.Value));
				return;
			}
			lastSyncCompletedTime = null;
		}

		private SubscriptionCacheEntry(GlobalSyncLogSession syncLogSession)
		{
			this.syncLogSession = syncLogSession;
		}

		internal Guid SubscriptionGuid
		{
			get
			{
				return this.subscriptionGuid;
			}
		}

		internal StoreObjectId SubscriptionMessageId
		{
			get
			{
				return this.subscriptionMessageId;
			}
		}

		internal string UserLegacyDn
		{
			get
			{
				return this.userLegacyDn;
			}
		}

		internal AggregationType AggregationType
		{
			get
			{
				return this.aggregationType;
			}
		}

		internal SyncPhase SyncPhase
		{
			get
			{
				return this.syncPhase;
			}
			set
			{
				this.syncPhase = value;
			}
		}

		internal string IncomingServerName
		{
			get
			{
				return this.incomingServerName;
			}
		}

		internal ExDateTime? LastSyncCompletedTime
		{
			get
			{
				return this.lastSyncCompletedTime;
			}
			set
			{
				this.lastSyncCompletedTime = value;
			}
		}

		internal Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		internal Guid TenantGuid
		{
			get
			{
				return this.tenantGuid;
			}
		}

		internal Guid ExternalDirectoryOrgId
		{
			get
			{
				return this.externalDirectoryOrgId;
			}
		}

		internal AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return this.subscriptionType;
			}
		}

		internal string HubServerDispatched
		{
			get
			{
				return this.hubServerDispatched;
			}
			set
			{
				this.hubServerDispatched = value;
			}
		}

		internal string LastHubServerDispatched
		{
			get
			{
				return this.lastHubServerDispatched;
			}
			set
			{
				this.lastHubServerDispatched = value;
			}
		}

		internal ExDateTime? FirstOutstandingDispatchTime
		{
			get
			{
				return this.firstOutstandingDispatchTime;
			}
			set
			{
				this.firstOutstandingDispatchTime = value;
			}
		}

		internal ExDateTime? LastSuccessfulDispatchTime
		{
			get
			{
				return this.lastSuccessfulDispatchTime;
			}
			set
			{
				this.lastSuccessfulDispatchTime = value;
			}
		}

		internal bool RecoverySyncEnabled
		{
			get
			{
				return this.recoverySyncEnabled;
			}
			set
			{
				this.recoverySyncEnabled = value;
			}
		}

		internal bool IsMigration
		{
			get
			{
				return this.AggregationType == AggregationType.Migration;
			}
		}

		internal SerializedSubscription SerializedSubscription
		{
			get
			{
				return this.serializedSubscription;
			}
			set
			{
				this.serializedSubscription = value;
			}
		}

		internal bool Disabled
		{
			get
			{
				return this.disabled;
			}
			set
			{
				this.disabled = value;
			}
		}

		internal string Diagnostics
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

		internal string SyncWatermark
		{
			get
			{
				if (!this.syncWatermarkIsInitialized)
				{
					return null;
				}
				return this.syncWatermark;
			}
			set
			{
				SyncUtilities.ThrowIfArgumentNull("SyncWatermark", value);
				this.syncWatermark = value;
				this.syncWatermarkIsInitialized = true;
			}
		}

		public override bool Equals(object obj)
		{
			SubscriptionCacheEntry otherCacheEntry = obj as SubscriptionCacheEntry;
			return this.Equals(otherCacheEntry);
		}

		public bool Equals(SubscriptionCacheEntry otherCacheEntry)
		{
			return otherCacheEntry != null && this.SubscriptionGuid == otherCacheEntry.SubscriptionGuid;
		}

		public override int GetHashCode()
		{
			return this.SubscriptionGuid.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "LegDn: {0}; HubDispatched: {1}; LastHubDispatched: {2}; LastSuccessfulDispatchTime: {3}; FirstOutstandingDispatchTime: {4}; RecoverySync: {5}; LastSyncCompletedTime: {6}; Disabled: {7}", new object[]
			{
				this.userLegacyDn,
				this.hubServerDispatched,
				this.lastHubServerDispatched,
				this.lastSuccessfulDispatchTime,
				this.firstOutstandingDispatchTime,
				this.recoverySyncEnabled,
				this.lastSyncCompletedTime,
				this.disabled
			});
		}

		internal static SubscriptionCacheEntry FromSerialization(GlobalSyncLogSession syncLogSession, BinaryReader reader, byte version)
		{
			SubscriptionCacheEntry subscriptionCacheEntry = new SubscriptionCacheEntry(syncLogSession);
			subscriptionCacheEntry.Deserialize(reader, version);
			return subscriptionCacheEntry;
		}

		internal void Serialize(BinaryWriter writer)
		{
			writer.Write(this.subscriptionGuid.ToString("N"));
			writer.Write((short)this.subscriptionType);
			writer.Write(this.subscriptionMessageId.ToBase64String());
			writer.Write(this.userLegacyDn);
			writer.Write(this.mailboxGuid.ToString("N"));
			writer.Write((this.lastSyncCompletedTime != null) ? this.lastSyncCompletedTime.Value.UtcTicks : 0L);
			writer.Write(this.hubServerDispatched ?? string.Empty);
			writer.Write((this.firstOutstandingDispatchTime != null) ? this.firstOutstandingDispatchTime.Value.UtcTicks : 0L);
			writer.Write((this.lastSuccessfulDispatchTime != null) ? this.lastSuccessfulDispatchTime.Value.UtcTicks : 0L);
			writer.Write(this.recoverySyncEnabled);
			writer.Write(this.disabled);
			writer.Write(this.diagnostics ?? string.Empty);
			writer.Write(this.tenantGuid.ToString("N"));
			writer.Write(this.externalDirectoryOrgId.ToString("N"));
			writer.Write((int)this.aggregationType);
			writer.Write(this.incomingServerName ?? string.Empty);
			writer.Write((short)this.syncPhase);
			writer.Write((this.lastHubServerDispatched == null) ? string.Empty : this.lastHubServerDispatched);
			this.serializedSubscription.Serialize(writer);
			writer.Write(this.syncWatermarkIsInitialized);
			writer.Write(this.syncWatermark);
		}

		internal bool Validate(AggregationSubscription actualSubscription, Guid actualUserMailboxGuid, bool fix, out string inconsistencyInfo)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			if (this.subscriptionGuid != actualSubscription.SubscriptionGuid)
			{
				stringBuilder.Append(Strings.InvalidSubscriptionGuid);
			}
			if (!object.Equals(this.subscriptionMessageId, actualSubscription.SubscriptionMessageId))
			{
				stringBuilder.Append(Strings.InvalidSubscriptionMessageId);
			}
			if (!string.Equals(this.userLegacyDn, actualSubscription.UserLegacyDN, StringComparison.OrdinalIgnoreCase))
			{
				stringBuilder.Append(Strings.InvalidUserLegacyDn);
			}
			if (this.mailboxGuid != actualUserMailboxGuid)
			{
				stringBuilder.Append(Strings.InvalidUserMailboxGuid);
			}
			if (this.subscriptionType != actualSubscription.SubscriptionType)
			{
				stringBuilder.Append(Strings.InvalidSubscriptionType);
			}
			if (this.syncPhase != actualSubscription.SyncPhase)
			{
				stringBuilder.Append(Strings.InvalidSyncPhase);
			}
			bool inactive = actualSubscription.Inactive;
			if (this.disabled != inactive)
			{
				stringBuilder.Append(Strings.InvalidDisabledStatus);
			}
			if (this.serializedSubscription == null)
			{
				stringBuilder.Append(Strings.InvalidSubscription);
			}
			if (fix)
			{
				this.subscriptionGuid = actualSubscription.SubscriptionGuid;
				this.userLegacyDn = actualSubscription.UserLegacyDN;
				this.mailboxGuid = actualUserMailboxGuid;
				this.syncPhase = actualSubscription.SyncPhase;
				this.disabled = inactive;
				this.subscriptionType = actualSubscription.SubscriptionType;
				this.subscriptionMessageId = actualSubscription.SubscriptionMessageId;
				this.incomingServerName = actualSubscription.IncomingServerName;
				this.serializedSubscription = SerializedSubscription.FromSubscription(actualSubscription);
			}
			bool flag;
			if (stringBuilder.Length > 0)
			{
				inconsistencyInfo = stringBuilder.ToString();
				flag = false;
			}
			else
			{
				inconsistencyInfo = null;
				flag = true;
			}
			this.syncLogSession.LogDebugging((TSLID)42UL, actualSubscription.SubscriptionGuid, actualUserMailboxGuid, "Cache Entry Validation Result: {0}:'{1}' and Was Fixed: {2}", new object[]
			{
				flag,
				inconsistencyInfo,
				fix
			});
			return !flag;
		}

		internal void UpdateSyncPhase(SyncPhase syncPhase)
		{
			this.syncPhase = syncPhase;
		}

		private void Deserialize(BinaryReader reader, byte version)
		{
			this.subscriptionGuid = this.ReadGuid(reader, false);
			this.subscriptionType = this.ReadEnumValue<AggregationSubscriptionType>(reader);
			if (!EnumValidator.IsValidValue<AggregationSubscriptionType>(this.subscriptionType) || this.subscriptionType == AggregationSubscriptionType.Unknown)
			{
				throw new SerializationException("AggregationSubscriptionType is invalid: " + this.subscriptionType);
			}
			string text = reader.ReadString();
			if (string.IsNullOrEmpty(text))
			{
				throw new SerializationException("Message ID is not valid.");
			}
			this.subscriptionMessageId = StoreObjectId.Deserialize(text);
			string value = reader.ReadString();
			if (string.IsNullOrEmpty(value))
			{
				throw new SerializationException("User Legacy DN is not valid.");
			}
			this.userLegacyDn = value;
			this.mailboxGuid = this.ReadGuid(reader, false);
			this.lastSyncCompletedTime = this.ReadDateTimeValue(reader);
			this.hubServerDispatched = reader.ReadString();
			this.firstOutstandingDispatchTime = this.ReadDateTimeValue(reader);
			this.lastSuccessfulDispatchTime = this.ReadDateTimeValue(reader);
			this.recoverySyncEnabled = reader.ReadBoolean();
			this.disabled = reader.ReadBoolean();
			if (!string.IsNullOrEmpty(this.hubServerDispatched) && (this.lastSuccessfulDispatchTime == null || this.firstOutstandingDispatchTime == null))
			{
				this.lastSuccessfulDispatchTime = null;
				this.firstOutstandingDispatchTime = new ExDateTime?(ExDateTime.UtcNow);
			}
			this.diagnostics = reader.ReadString();
			this.tenantGuid = this.ReadGuid(reader, true);
			this.externalDirectoryOrgId = this.ReadGuid(reader, true);
			this.aggregationType = (AggregationType)reader.ReadInt32();
			this.incomingServerName = reader.ReadString();
			this.syncPhase = this.ReadEnumValue<SyncPhase>(reader);
			this.lastHubServerDispatched = reader.ReadString();
			this.serializedSubscription = SerializedSubscription.FromReader(reader);
			this.syncWatermarkIsInitialized = reader.ReadBoolean();
			this.syncWatermark = reader.ReadString();
		}

		private Guid ReadGuid(BinaryReader reader, bool emptyIsValid)
		{
			string text = reader.ReadString();
			if (string.IsNullOrEmpty(text))
			{
				throw new SerializationException("Guid is not valid.");
			}
			Guid guid = new Guid(text);
			if (!emptyIsValid && object.Equals(guid, Guid.Empty))
			{
				throw new SerializationException("Guid is Guid.Empty.");
			}
			return guid;
		}

		private ExDateTime? ReadDateTimeValue(BinaryReader reader)
		{
			long num = reader.ReadInt64();
			if (num == 0L)
			{
				return null;
			}
			if (num < DateTime.MinValue.Ticks || num > DateTime.MaxValue.Ticks)
			{
				throw new SerializationException("Invalid dateTime in ticks.");
			}
			return new ExDateTime?(new ExDateTime(ExTimeZone.UtcTimeZone, num));
		}

		private T ReadEnumValue<T>(BinaryReader reader) where T : struct
		{
			int value = (int)reader.ReadInt16();
			T t = (T)((object)Enum.ToObject(typeof(T), value));
			if (!EnumValidator.IsValidValue<T>(t))
			{
				throw new SerializationException(string.Format("{0} is invalid: {1}", typeof(T), t));
			}
			return t;
		}

		private readonly GlobalSyncLogSession syncLogSession;

		private Guid subscriptionGuid;

		private StoreObjectId subscriptionMessageId;

		private string userLegacyDn;

		private AggregationSubscriptionType subscriptionType;

		private AggregationType aggregationType;

		private SyncPhase syncPhase;

		private ExDateTime? lastSyncCompletedTime;

		private string incomingServerName;

		private Guid mailboxGuid;

		private SerializedSubscription serializedSubscription;

		private Guid tenantGuid;

		private Guid externalDirectoryOrgId;

		private string hubServerDispatched;

		private string lastHubServerDispatched;

		private ExDateTime? firstOutstandingDispatchTime;

		private ExDateTime? lastSuccessfulDispatchTime;

		private bool recoverySyncEnabled;

		private bool disabled;

		private string diagnostics;

		private string syncWatermark = string.Empty;

		private bool syncWatermarkIsInitialized;
	}
}
