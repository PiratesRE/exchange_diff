using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class PublicFolderId : ObjectId, IEquatable<PublicFolderId>
	{
		internal StoreObjectId StoreObjectId { get; set; }

		public MapiFolderPath MapiFolderPath { get; private set; }

		public OrganizationId OrganizationId { get; set; }

		internal PublicFolderId(MapiFolderPath folderPath)
		{
			this.MapiFolderPath = folderPath;
		}

		internal PublicFolderId(StoreObjectId storeObjectId)
		{
			this.StoreObjectId = storeObjectId;
		}

		internal PublicFolderId(PublicFolderId publicFolderId) : this(publicFolderId.OrganizationId, publicFolderId.StoreObjectId, publicFolderId.MapiFolderPath)
		{
		}

		internal PublicFolderId(OrganizationId organizationId, StoreObjectId storeObjectId, MapiFolderPath folderPath) : this(storeObjectId, folderPath)
		{
			this.OrganizationId = organizationId;
		}

		internal PublicFolderId(StoreObjectId storeObjectId, MapiFolderPath folderPath)
		{
			this.StoreObjectId = storeObjectId;
			this.MapiFolderPath = folderPath;
		}

		public bool Equals(PublicFolderId other)
		{
			if (other == null)
			{
				return false;
			}
			bool flag = object.Equals(this.StoreObjectId, other.StoreObjectId);
			if (flag && this.StoreObjectId != null)
			{
				return true;
			}
			bool flag2 = object.Equals(this.MapiFolderPath, other.MapiFolderPath);
			return (flag2 && this.MapiFolderPath != null) || (flag2 && flag);
		}

		public override byte[] GetBytes()
		{
			return (this.StoreObjectId == null) ? Array<byte>.Empty : this.StoreObjectId.GetBytes();
		}

		public override int GetHashCode()
		{
			if (this.StoreObjectId != null)
			{
				return this.StoreObjectId.GetHashCode();
			}
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as PublicFolderId);
		}

		public override string ToString()
		{
			string str = (this.OrganizationId == null) ? string.Empty : (this.OrganizationId.OrganizationalUnit.Name + '\\'.ToString());
			if (this.MapiFolderPath != null)
			{
				return str + this.MapiFolderPath.ToString();
			}
			if (this.StoreObjectId != null)
			{
				return str + this.StoreObjectId.ToString();
			}
			return null;
		}
	}
}
