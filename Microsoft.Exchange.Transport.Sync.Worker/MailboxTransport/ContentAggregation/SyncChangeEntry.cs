using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SyncChangeEntry : ISyncClientOperation
	{
		internal SyncChangeEntry(ChangeType changeType, SchemaType schemaType, string cloudId) : this(changeType, schemaType, cloudId, null, null)
		{
		}

		internal SyncChangeEntry(ChangeType changeType, SchemaType schemaType, string cloudId, object cloudObject) : this(changeType, schemaType, cloudId, cloudObject, null)
		{
		}

		internal SyncChangeEntry(ChangeType changeType, SchemaType schemaType, string cloudId, ISyncObject syncObject) : this(changeType, schemaType, cloudId, null, syncObject)
		{
		}

		internal SyncChangeEntry(SyncOperation syncOperation, SchemaType schemaType) : this(syncOperation.ChangeType, schemaType, (StoreObjectId)syncOperation.Id.NativeId)
		{
			this.syncOperation = syncOperation;
		}

		internal SyncChangeEntry(HierarchySyncOperation hierarchySyncOperation) : this(hierarchySyncOperation.ChangeType, SchemaType.Folder, hierarchySyncOperation.ItemId)
		{
		}

		internal SyncChangeEntry(ChangeType changeType, SchemaType schemaType, StoreObjectId nativeId)
		{
			this.changeType = changeType;
			this.nativeId = nativeId;
			this.schemaType = schemaType;
			this.persist = true;
			this.suspectedSyncPoisonItem = new SyncPoisonItem(this.nativeId.ToBase64String(), SyncPoisonEntitySource.Labs, SyncChangeEntry.GetSyncPoisonEntityType(schemaType));
		}

		internal SyncChangeEntry(SyncChangeEntry change)
		{
			this.changeKey = change.changeKey;
			this.changeType = change.changeType;
			this.cloudFolderId = change.cloudFolderId;
			this.cloudId = change.cloudId;
			this.cloudObject = change.cloudObject;
			this.cloudVersion = change.cloudVersion;
			this.exception = change.exception;
			this.nativeFolderId = change.nativeFolderId;
			this.nativeId = change.nativeId;
			this.newCloudFolderId = change.newCloudFolderId;
			this.newCloudId = change.newCloudId;
			this.newNativeFolderId = change.newNativeFolderId;
			this.newNativeId = change.newNativeId;
			this.persist = change.persist;
			this.recovered = change.recovered;
			this.schemaType = change.schemaType;
			this.suspectedSyncPoisonItem = change.suspectedSyncPoisonItem;
			this.syncObject = change.syncObject;
			this.syncReportObject = new SyncReportObject(this.syncObject, this.schemaType);
		}

		private SyncChangeEntry(ChangeType changeType, SchemaType schemaType, string cloudId, object cloudObject, ISyncObject syncObject)
		{
			this.changeType = changeType;
			this.schemaType = schemaType;
			this.cloudId = cloudId;
			this.cloudObject = cloudObject;
			this.syncObject = syncObject;
			this.persist = true;
			this.suspectedSyncPoisonItem = new SyncPoisonItem(cloudId, SyncPoisonEntitySource.Remote, SyncChangeEntry.GetSyncPoisonEntityType(schemaType));
			this.syncReportObject = new SyncReportObject(this.syncObject, this.schemaType);
		}

		public int?[] ChangeTrackingInformation
		{
			get
			{
				if (this.syncOperation != null)
				{
					return this.syncOperation.ChangeTrackingInformation;
				}
				return this.changeTrackingInformation;
			}
			set
			{
				if (this.syncOperation != null)
				{
					this.syncOperation.ChangeTrackingInformation = value;
					return;
				}
				this.changeTrackingInformation = value;
			}
		}

		public ChangeType ChangeType
		{
			get
			{
				return this.changeType;
			}
			set
			{
				this.changeType = value;
			}
		}

		public SchemaType SchemaType
		{
			get
			{
				return this.schemaType;
			}
			set
			{
				this.schemaType = value;
			}
		}

		public string CloudId
		{
			get
			{
				return this.cloudId;
			}
			set
			{
				this.cloudId = value;
			}
		}

		public StoreObjectId NativeId
		{
			get
			{
				return this.nativeId;
			}
			set
			{
				this.nativeId = value;
			}
		}

		public StoreObjectId NativeFolderId
		{
			get
			{
				return this.nativeFolderId;
			}
			set
			{
				this.nativeFolderId = value;
			}
		}

		public bool Recovered
		{
			get
			{
				return this.recovered;
			}
			set
			{
				this.recovered = value;
			}
		}

		public StoreObjectId NewNativeFolderId
		{
			get
			{
				return this.newNativeFolderId;
			}
			set
			{
				this.newNativeFolderId = value;
			}
		}

		public StoreObjectId NewNativeId
		{
			get
			{
				return this.newNativeId;
			}
			set
			{
				this.newNativeId = value;
			}
		}

		public string NewCloudFolderId
		{
			get
			{
				return this.newCloudFolderId;
			}
			set
			{
				this.newCloudFolderId = value;
			}
		}

		public string NewCloudId
		{
			get
			{
				return this.newCloudId;
			}
			set
			{
				this.newCloudId = value;
			}
		}

		public string CloudFolderId
		{
			get
			{
				return this.cloudFolderId;
			}
			set
			{
				this.cloudFolderId = value;
			}
		}

		public string CloudVersion
		{
			get
			{
				return this.cloudVersion;
			}
			set
			{
				this.cloudVersion = value;
			}
		}

		public ISyncItem SyncItem
		{
			get
			{
				if (this.syncOperation != null)
				{
					return this.syncOperation.GetItem(new PropertyDefinition[0]);
				}
				return null;
			}
		}

		public SyncOperation SyncOperation
		{
			get
			{
				return this.syncOperation;
			}
		}

		public object CloudObject
		{
			get
			{
				return this.cloudObject;
			}
			set
			{
				this.cloudObject = value;
			}
		}

		public ISyncReportObject SyncReportObject
		{
			get
			{
				return this.syncReportObject;
			}
			set
			{
				this.syncReportObject = value;
			}
		}

		public ISyncObject SyncObject
		{
			get
			{
				return this.syncObject;
			}
			set
			{
				if (value != null && value.Type != this.schemaType)
				{
					throw new InvalidOperationException("Either SyncObject is null or it must match the SchemaType of this change.");
				}
				this.syncObject = value;
				if (this.syncObject != null)
				{
					this.syncReportObject = new SyncReportObject(this.syncObject, this.schemaType);
				}
			}
		}

		public ISyncItem Item
		{
			get
			{
				return this.SyncItem;
			}
		}

		public ISyncItemId Id
		{
			get
			{
				if (this.syncOperation != null)
				{
					return this.syncOperation.Id;
				}
				return null;
			}
		}

		public bool SendEnabled
		{
			get
			{
				return false;
			}
		}

		public string ClientAddId
		{
			get
			{
				return this.CloudId;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
			set
			{
				this.exception = value;
			}
		}

		public bool Persist
		{
			get
			{
				return this.persist;
			}
			set
			{
				this.persist = value;
			}
		}

		public bool HasException
		{
			get
			{
				return this.exception != null;
			}
		}

		public bool ApplyAttempted { get; set; }

		public bool ResolvedSuccessfully
		{
			get
			{
				return this.resolvedSuccessfully;
			}
		}

		public byte[] ChangeKey
		{
			get
			{
				return this.changeKey;
			}
			set
			{
				this.changeKey = value;
			}
		}

		internal SyncPoisonItem SuspectedSyncPoisonItem
		{
			get
			{
				return this.suspectedSyncPoisonItem;
			}
		}

		internal string MessageClass
		{
			get
			{
				return this.messageClass;
			}
			set
			{
				this.messageClass = value;
			}
		}

		internal Dictionary<string, string> Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.Properties = new Dictionary<string, string>(3);
				}
				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}

		internal bool Submitted { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} - {1} - CloudFolderId/CloudId: {2}/{3} - NativeFolderId/NativeID: {4}/{5} - Persist: {6} - Exception: {7}", new object[]
			{
				this.changeType,
				this.schemaType,
				this.cloudFolderId ?? string.Empty,
				this.cloudId ?? string.Empty,
				(this.nativeFolderId == null) ? string.Empty : this.nativeFolderId.ToString(),
				(this.nativeId == null) ? string.Empty : this.nativeId.ToString(),
				this.persist,
				(this.exception == null) ? string.Empty : this.exception.ToString()
			});
		}

		internal void SetResolvedSuccessfully()
		{
			this.resolvedSuccessfully = true;
		}

		private static SyncPoisonEntityType GetSyncPoisonEntityType(SchemaType schemaType)
		{
			if (schemaType != SchemaType.Folder)
			{
				return SyncPoisonEntityType.Item;
			}
			return SyncPoisonEntityType.Folder;
		}

		private const int DefaultEstimatePropertyCapacity = 3;

		private readonly SyncPoisonItem suspectedSyncPoisonItem;

		private int?[] changeTrackingInformation;

		private ChangeType changeType;

		private string cloudId;

		private object cloudObject;

		private ISyncObject syncObject;

		private ISyncReportObject syncReportObject;

		private SyncOperation syncOperation;

		private Exception exception;

		private bool persist;

		private StoreObjectId nativeId;

		private StoreObjectId nativeFolderId;

		private string cloudFolderId;

		private string cloudVersion;

		private SchemaType schemaType;

		private StoreObjectId newNativeFolderId;

		private StoreObjectId newNativeId;

		private string newCloudFolderId;

		private string newCloudId;

		private bool recovered;

		private byte[] changeKey;

		private string messageClass;

		private bool resolvedSuccessfully;

		private Dictionary<string, string> properties;
	}
}
