using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Serializable]
	public class PublicFolderClientPermissionEntry : IConfigurable
	{
		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
			throw new NotSupportedException();
		}

		public ObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return true;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		void IConfigurable.ResetChangeTracking()
		{
			throw new NotSupportedException();
		}

		ValidationError[] IConfigurable.Validate()
		{
			return ValidationError.None;
		}

		public PublicFolderUserId User
		{
			get
			{
				return this.user;
			}
		}

		public PublicFolderClientPermissionEntry()
		{
		}

		public PublicFolderClientPermissionEntry(PublicFolderId identity, PublicFolderUserId user)
		{
			if (null == identity)
			{
				throw new ArgumentNullException("Identity");
			}
			if (null == user)
			{
				throw new ArgumentNullException("user");
			}
			this.identity = identity;
			this.user = user;
		}

		public MultiValuedProperty<PublicFolderAccessRight> AccessRights
		{
			get
			{
				if (this.accessRights != null)
				{
					return new MultiValuedProperty<PublicFolderAccessRight>(this.accessRights);
				}
				return new MultiValuedProperty<PublicFolderAccessRight>();
			}
			set
			{
				PublicFolderClientPermissionEntry.ValidateAccessRights(value);
				this.accessRights = value;
			}
		}

		internal static void ValidateAccessRights(MultiValuedProperty<PublicFolderAccessRight> accessRights)
		{
			if (accessRights != null)
			{
				bool flag = false;
				bool flag2 = false;
				foreach (PublicFolderAccessRight publicFolderAccessRight in accessRights)
				{
					if (publicFolderAccessRight.IsRole)
					{
						if (flag2)
						{
							throw new ArgumentException(Strings.ErrorPrecannedRoleAndSpecificPermission);
						}
						flag = true;
					}
					else
					{
						if (flag)
						{
							throw new ArgumentException(Strings.ErrorPrecannedRoleAndSpecificPermission);
						}
						flag2 = true;
					}
				}
			}
		}

		private PublicFolderId identity;

		private PublicFolderUserId user;

		private MultiValuedProperty<PublicFolderAccessRight> accessRights;
	}
}
