using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class PublicFolder : MailboxFolder
	{
		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return PublicFolder.schema;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return (PublicFolderId)this[PublicFolderSchema.Identity];
			}
		}

		private new string FolderStoreObjectId
		{
			get
			{
				return (string)this[MailboxFolderSchema.FolderStoreObjectId];
			}
		}

		[Parameter]
		[ValidateNotNullOrEmpty]
		public new string Name
		{
			get
			{
				return (string)this[PublicFolderSchema.Name];
			}
			set
			{
				this[PublicFolderSchema.Name] = value;
			}
		}

		public bool MailEnabled
		{
			get
			{
				return (bool)this[PublicFolderSchema.MailEnabled];
			}
			set
			{
				this[PublicFolderSchema.MailEnabled] = value;
			}
		}

		internal byte[] ProxyGuid
		{
			get
			{
				return (byte[])this[PublicFolderSchema.ProxyGuid];
			}
			set
			{
				this[PublicFolderSchema.ProxyGuid] = value;
			}
		}

		public Guid? MailRecipientGuid
		{
			get
			{
				return (Guid?)this[PublicFolderSchema.MailRecipientGuid];
			}
			set
			{
				this[PublicFolderSchema.MailRecipientGuid] = value;
			}
		}

		public string ParentPath
		{
			get
			{
				MapiFolderPath folderPath = base.FolderPath;
				if (folderPath == null || folderPath.IsSubtreeRoot)
				{
					return null;
				}
				return folderPath.Parent.ToString();
			}
		}

		public string ContentMailboxName { get; internal set; }

		public Guid ContentMailboxGuid
		{
			get
			{
				return ((PublicFolderContentMailboxInfo)this[PublicFolderSchema.ContentMailboxInfo]).MailboxGuid;
			}
			set
			{
				this[PublicFolderSchema.ContentMailboxInfo] = new PublicFolderContentMailboxInfo(value.ToString());
			}
		}

		[Parameter]
		public CultureInfo EformsLocaleId
		{
			get
			{
				return (CultureInfo)this[PublicFolderSchema.EformsLocaleId];
			}
			set
			{
				this[PublicFolderSchema.EformsLocaleId] = value;
			}
		}

		[Parameter]
		public bool PerUserReadStateEnabled
		{
			get
			{
				return (bool)this[PublicFolderSchema.PerUserReadStateEnabled];
			}
			set
			{
				this[PublicFolderSchema.PerUserReadStateEnabled] = value;
			}
		}

		public string EntryId
		{
			get
			{
				return (string)this[PublicFolderSchema.EntryId];
			}
		}

		public string DumpsterEntryId { get; internal set; }

		public new string ParentFolder
		{
			get
			{
				if (this[PublicFolderSchema.ParentFolder] == null)
				{
					return string.Empty;
				}
				return ((PublicFolderId)this[PublicFolderSchema.ParentFolder]).StoreObjectId.ToHexEntryId();
			}
		}

		public OrganizationId OrganizationId { get; set; }

		[Parameter]
		public EnhancedTimeSpan? AgeLimit
		{
			get
			{
				return (EnhancedTimeSpan?)this[PublicFolderSchema.AgeLimit];
			}
			set
			{
				this[PublicFolderSchema.AgeLimit] = value;
			}
		}

		[Parameter]
		public EnhancedTimeSpan? RetainDeletedItemsFor
		{
			get
			{
				return (EnhancedTimeSpan?)this[PublicFolderSchema.RetainDeletedItemsFor];
			}
			set
			{
				this[PublicFolderSchema.RetainDeletedItemsFor] = value;
			}
		}

		[Parameter]
		public Unlimited<ByteQuantifiedSize>? ProhibitPostQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>?)this[PublicFolderSchema.ProhibitPostQuota];
			}
			set
			{
				this[PublicFolderSchema.ProhibitPostQuota] = value;
			}
		}

		[Parameter]
		public Unlimited<ByteQuantifiedSize>? IssueWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>?)this[PublicFolderSchema.IssueWarningQuota];
			}
			set
			{
				this[PublicFolderSchema.IssueWarningQuota] = value;
			}
		}

		[Parameter]
		public Unlimited<ByteQuantifiedSize>? MaxItemSize
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>?)this[PublicFolderSchema.MaxItemSize];
			}
			set
			{
				this[PublicFolderSchema.MaxItemSize] = value;
			}
		}

		[Parameter]
		public ExDateTime? LastMovedTime
		{
			get
			{
				return (ExDateTime?)this[PublicFolderSchema.LastMovedTime];
			}
			internal set
			{
				this[PublicFolderSchema.LastMovedTime] = value;
			}
		}

		internal int QuotaStyle
		{
			get
			{
				return (int)this[PublicFolderSchema.PfQuotaStyle];
			}
			set
			{
				this[PublicFolderSchema.PfQuotaStyle] = value;
			}
		}

		internal new static object IdentityGetter(IPropertyBag propertyBag)
		{
			VersionedId versionedId = (VersionedId)propertyBag[MailboxFolderSchema.InternalFolderIdentity];
			MapiFolderPath mapiFolderPath = (MapiFolderPath)propertyBag[MailboxFolderSchema.FolderPath];
			if (null != mapiFolderPath || versionedId != null)
			{
				return new PublicFolderId((versionedId == null) ? null : versionedId.ObjectId, mapiFolderPath);
			}
			return null;
		}

		internal static void MailRecipientGuidSetter(object value, IPropertyBag propertyBag)
		{
			if (value is Guid?)
			{
				byte[] value2 = ((Guid?)value).Value.ToByteArray();
				propertyBag[PublicFolderSchema.ProxyGuid] = value2;
				return;
			}
			propertyBag[PublicFolderSchema.ProxyGuid] = null;
		}

		internal static object MailRecipientGuidGetter(IPropertyBag propertyBag)
		{
			byte[] array = (byte[])propertyBag[PublicFolderSchema.ProxyGuid];
			if (array != null && array.Length == 16)
			{
				return new Guid(array);
			}
			return null;
		}

		internal new static object ParentFolderGetter(IPropertyBag propertyBag)
		{
			VersionedId versionedId = (VersionedId)propertyBag[MailboxFolderSchema.InternalFolderIdentity];
			StoreObjectId storeObjectId = (StoreObjectId)propertyBag[MailboxFolderSchema.InternalParentFolderIdentity];
			MapiFolderPath mapiFolderPath = (MapiFolderPath)propertyBag[MailboxFolderSchema.FolderPath];
			if (versionedId != null && versionedId.ObjectId != null && object.Equals(versionedId.ObjectId, storeObjectId))
			{
				return null;
			}
			if ((null != mapiFolderPath && null != mapiFolderPath.Parent) || storeObjectId != null)
			{
				return new PublicFolderId(storeObjectId, (null == mapiFolderPath) ? null : mapiFolderPath.Parent);
			}
			return null;
		}

		internal static object EformsLocaleIdGetter(IPropertyBag propertyBag)
		{
			int? num = (int?)propertyBag[PublicFolderSchema.EformsLocaleIdValue];
			if (num == null)
			{
				return null;
			}
			return new CultureInfo(num.Value);
		}

		internal static void EformsLocaleIdSetter(object value, IPropertyBag propertyBag)
		{
			if (value != null)
			{
				propertyBag[PublicFolderSchema.EformsLocaleIdValue] = ((CultureInfo)value).LCID;
				return;
			}
			propertyBag[PublicFolderSchema.EformsLocaleIdValue] = null;
		}

		internal static void PerUserReadStateEnabledSetter(object value, IPropertyBag propertyBag)
		{
			if (value != null)
			{
				propertyBag[PublicFolderSchema.DisablePerUserReadValue] = !(bool)value;
				return;
			}
			propertyBag[PublicFolderSchema.DisablePerUserReadValue] = null;
		}

		internal static object PerUserReadStateEnabledGetter(IPropertyBag propertyBag)
		{
			bool? flag = (bool?)propertyBag[PublicFolderSchema.DisablePerUserReadValue];
			return flag == null || !flag.Value;
		}

		internal static object MailEnabledGetter(IPropertyBag propertyBag)
		{
			bool? flag = (bool?)propertyBag[PublicFolderSchema.MailEnabledValue];
			return flag != null && flag.Value;
		}

		internal static void MailEnabledSetter(object value, IPropertyBag propertyBag)
		{
			if (value != null)
			{
				propertyBag[PublicFolderSchema.MailEnabledValue] = (bool)value;
				return;
			}
			propertyBag[PublicFolderSchema.MailEnabledValue] = null;
		}

		internal static string EntryIdGetter(IPropertyBag propertyBag)
		{
			VersionedId versionedId = propertyBag[MailboxFolderSchema.InternalFolderIdentity] as VersionedId;
			if (versionedId == null || versionedId.ObjectId == null)
			{
				return string.Empty;
			}
			return versionedId.ObjectId.ToHexEntryId();
		}

		internal static PublicFolderContentMailboxInfo ContentMailboxInfoGetter(IPropertyBag propertyBag)
		{
			string contentMailboxInfo = null;
			byte[] array = propertyBag[PublicFolderSchema.ReplicaListBinary] as byte[];
			if (array != null)
			{
				string[] stringArrayFromBytes = ReplicaListProperty.GetStringArrayFromBytes(array);
				if (stringArrayFromBytes.Length > 0)
				{
					contentMailboxInfo = stringArrayFromBytes[0];
				}
			}
			return new PublicFolderContentMailboxInfo(contentMailboxInfo);
		}

		internal static void ContentMailboxInfoSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[PublicFolderSchema.ReplicaListBinary] = ((value != null) ? ReplicaListProperty.GetBytesFromStringArray(new string[]
			{
				((PublicFolderContentMailboxInfo)value).ToString()
			}) : null);
		}

		internal static object AgeLimitGetter(IPropertyBag propertyBag)
		{
			int? num = propertyBag[PublicFolderSchema.OverallAgeLimit] as int?;
			if (num != null)
			{
				return EnhancedTimeSpan.FromSeconds((double)num.Value);
			}
			return null;
		}

		internal static void AgeLimitSetter(object value, IPropertyBag propertyBag)
		{
			EnhancedTimeSpan? enhancedTimeSpan = value as EnhancedTimeSpan?;
			if (enhancedTimeSpan != null)
			{
				propertyBag[PublicFolderSchema.OverallAgeLimit] = (int)enhancedTimeSpan.Value.TotalSeconds;
				return;
			}
			propertyBag[PublicFolderSchema.OverallAgeLimit] = null;
		}

		internal static object RetainDeletedItemsForGetter(IPropertyBag propertyBag)
		{
			int? num = propertyBag[PublicFolderSchema.RetentionAgeLimit] as int?;
			if (num != null)
			{
				return EnhancedTimeSpan.FromSeconds((double)num.Value);
			}
			return null;
		}

		internal static void RetainDeletedItemsForSetter(object value, IPropertyBag propertyBag)
		{
			EnhancedTimeSpan? enhancedTimeSpan = value as EnhancedTimeSpan?;
			if (enhancedTimeSpan != null)
			{
				propertyBag[PublicFolderSchema.RetentionAgeLimit] = (int)enhancedTimeSpan.Value.TotalSeconds;
				return;
			}
			propertyBag[PublicFolderSchema.RetentionAgeLimit] = null;
		}

		internal static object ProhibitPostQuotaGetter(IPropertyBag propertyBag)
		{
			int? num = propertyBag[PublicFolderSchema.PfOverHardQuotaLimit] as int?;
			if (num != null)
			{
				int value = num.Value;
				return (value < 0) ? Unlimited<ByteQuantifiedSize>.UnlimitedValue : new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(checked((ulong)value)));
			}
			return Unlimited<ByteQuantifiedSize>.UnlimitedValue;
		}

		internal static void ProhibitPostQuotaSetter(object value, IPropertyBag propertyBag)
		{
			Unlimited<ByteQuantifiedSize>? unlimited = value as Unlimited<ByteQuantifiedSize>?;
			if (unlimited != null)
			{
				Unlimited<ByteQuantifiedSize> value2 = unlimited.Value;
				propertyBag[PublicFolderSchema.PfOverHardQuotaLimit] = (value2.IsUnlimited ? -1 : (propertyBag[PublicFolderSchema.PfOverHardQuotaLimit] = (int)value2.Value.ToKB()));
				return;
			}
			propertyBag[PublicFolderSchema.PfOverHardQuotaLimit] = null;
		}

		internal static object LastMovedTimeGetter(IPropertyBag propertyBag)
		{
			ExDateTime? exDateTime = propertyBag[PublicFolderSchema.LastMovedTimeStamp] as ExDateTime?;
			if (exDateTime != null)
			{
				return exDateTime;
			}
			return null;
		}

		internal static object IssueWarningQuotaGetter(IPropertyBag propertyBag)
		{
			int? num = propertyBag[PublicFolderSchema.PfStorageQuota] as int?;
			if (num != null)
			{
				int value = num.Value;
				return (value < 0) ? Unlimited<ByteQuantifiedSize>.UnlimitedValue : new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(checked((ulong)value)));
			}
			return Unlimited<ByteQuantifiedSize>.UnlimitedValue;
		}

		internal static void IssueWarningQuotaSetter(object value, IPropertyBag propertyBag)
		{
			Unlimited<ByteQuantifiedSize>? unlimited = value as Unlimited<ByteQuantifiedSize>?;
			if (unlimited != null)
			{
				Unlimited<ByteQuantifiedSize> value2 = unlimited.Value;
				propertyBag[PublicFolderSchema.PfStorageQuota] = (value2.IsUnlimited ? -1 : ((int)value2.Value.ToKB()));
				return;
			}
			propertyBag[PublicFolderSchema.PfStorageQuota] = null;
		}

		internal static object MaxItemSizeGetter(IPropertyBag propertyBag)
		{
			int? num = propertyBag[PublicFolderSchema.PfMsgSizeLimit] as int?;
			if (num != null)
			{
				int value = num.Value;
				return (value < 0) ? Unlimited<ByteQuantifiedSize>.UnlimitedValue : new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(checked((ulong)value)));
			}
			return Unlimited<ByteQuantifiedSize>.UnlimitedValue;
		}

		internal static void MaxItemSizeSetter(object value, IPropertyBag propertyBag)
		{
			Unlimited<ByteQuantifiedSize>? unlimited = value as Unlimited<ByteQuantifiedSize>?;
			if (unlimited != null)
			{
				Unlimited<ByteQuantifiedSize> value2 = unlimited.Value;
				propertyBag[PublicFolderSchema.PfMsgSizeLimit] = (value2.IsUnlimited ? -1 : ((int)value2.Value.ToKB()));
				return;
			}
			propertyBag[PublicFolderSchema.PfMsgSizeLimit] = null;
		}

		private static PublicFolderSchema schema = ObjectSchema.GetInstance<PublicFolderSchema>();
	}
}
