using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MailboxFolder : XsoMailboxConfigurationObject
	{
		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return MailboxFolder.schema;
			}
		}

		internal static object IdentityGetter(IPropertyBag propertyBag)
		{
			ADObjectId mailboxOwnerId = (ADObjectId)propertyBag[XsoMailboxConfigurationObjectSchema.MailboxOwnerId];
			VersionedId versionedId = (VersionedId)propertyBag[MailboxFolderSchema.InternalFolderIdentity];
			MapiFolderPath mapiFolderPath = (MapiFolderPath)propertyBag[MailboxFolderSchema.FolderPath];
			if (null != mapiFolderPath || versionedId != null)
			{
				return new MailboxFolderId(mailboxOwnerId, (versionedId == null) ? null : versionedId.ObjectId, mapiFolderPath);
			}
			return null;
		}

		internal static object ParentFolderGetter(IPropertyBag propertyBag)
		{
			ADObjectId mailboxOwnerId = (ADObjectId)propertyBag[XsoMailboxConfigurationObjectSchema.MailboxOwnerId];
			VersionedId versionedId = (VersionedId)propertyBag[MailboxFolderSchema.InternalFolderIdentity];
			StoreObjectId storeObjectId = (StoreObjectId)propertyBag[MailboxFolderSchema.InternalParentFolderIdentity];
			MapiFolderPath mapiFolderPath = (MapiFolderPath)propertyBag[MailboxFolderSchema.FolderPath];
			if (versionedId != null && versionedId != null && object.Equals(versionedId.ObjectId, storeObjectId))
			{
				return null;
			}
			if ((null != mapiFolderPath && null != mapiFolderPath.Parent) || storeObjectId != null)
			{
				return new MailboxFolderId(mailboxOwnerId, storeObjectId, (null == mapiFolderPath) ? null : mapiFolderPath.Parent);
			}
			return null;
		}

		internal static object FolderStoreObjectIdGetter(IPropertyBag propertyBag)
		{
			VersionedId versionedId = (VersionedId)propertyBag[MailboxFolderSchema.InternalFolderIdentity];
			if (versionedId != null && versionedId.ObjectId != null)
			{
				return versionedId.ObjectId.ToString();
			}
			return string.Empty;
		}

		internal void SetDefaultFolderType(DefaultFolderType defaultFolderType)
		{
			this[MailboxFolderSchema.DefaultFolderType] = new DefaultFolderType?(defaultFolderType);
		}

		[ValidateNotNullOrEmpty]
		public string Name
		{
			get
			{
				return (string)this[MailboxFolderSchema.Name];
			}
			set
			{
				this[MailboxFolderSchema.Name] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return (MailboxFolderId)this[MailboxFolderSchema.Identity];
			}
		}

		public MailboxFolderId ParentFolder
		{
			get
			{
				return (MailboxFolderId)this[MailboxFolderSchema.ParentFolder];
			}
		}

		internal VersionedId InternalFolderIdentity
		{
			get
			{
				return (VersionedId)this[MailboxFolderSchema.InternalFolderIdentity];
			}
		}

		public string FolderStoreObjectId
		{
			get
			{
				return (string)this[MailboxFolderSchema.FolderStoreObjectId];
			}
		}

		internal StoreObjectId InternalParentFolderIdentity
		{
			get
			{
				return (StoreObjectId)this[MailboxFolderSchema.InternalParentFolderIdentity];
			}
		}

		public long? FolderSize
		{
			get
			{
				return (long?)this[MailboxFolderSchema.FolderSize];
			}
		}

		public bool? HasSubfolders
		{
			get
			{
				return (bool?)this[MailboxFolderSchema.HasSubfolders];
			}
		}

		public string FolderClass
		{
			get
			{
				return (string)this[MailboxFolderSchema.FolderClass];
			}
			internal set
			{
				this[MailboxFolderSchema.FolderClass] = value;
			}
		}

		public MapiFolderPath FolderPath
		{
			get
			{
				return (MapiFolderPath)this[MailboxFolderSchema.FolderPath];
			}
			internal set
			{
				this[MailboxFolderSchema.FolderPath] = value;
			}
		}

		public DefaultFolderType? DefaultFolderType
		{
			get
			{
				return (DefaultFolderType?)this[MailboxFolderSchema.DefaultFolderType];
			}
		}

		public ExtendedFolderFlags? ExtendedFolderFlags
		{
			get
			{
				return (ExtendedFolderFlags?)this[MailboxFolderSchema.ExtendedFolderFlags];
			}
		}

		public override string ToString()
		{
			if (this.Name != null)
			{
				return this.Name;
			}
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return base.ToString();
		}

		private static MailboxFolderSchema schema = ObjectSchema.GetInstance<MailboxFolderSchema>();
	}
}
