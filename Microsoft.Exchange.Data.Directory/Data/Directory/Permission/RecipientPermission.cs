using System;
using System.DirectoryServices;
using System.Security.AccessControl;

namespace Microsoft.Exchange.Data.Directory.Permission
{
	[Serializable]
	public class RecipientPermission : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return RecipientPermission.schema;
			}
		}

		public RecipientPermission() : base(new SimpleProviderPropertyBag())
		{
		}

		public RecipientPermission(ActiveDirectoryAccessRule ace, ADObjectId identity, string trustee) : this()
		{
			if (ace == null)
			{
				throw new ArgumentNullException("ace");
			}
			this.Identity = identity;
			this.Trustee = trustee;
			this.AccessControlType = ace.AccessControlType;
			this.IsInherited = ace.IsInherited;
			this.InheritanceType = ace.InheritanceType;
			this.AccessRights = new MultiValuedProperty<RecipientAccessRight>(RecipientPermissionHelper.GetRecipientAccessRight(ace).Value);
		}

		public RecipientPermission(ActiveDirectoryAccessRule ace, ADObjectId identity, string trustee, RecipientAccessRight accessRight) : this()
		{
			if (ace == null)
			{
				throw new ArgumentNullException("ace");
			}
			this.Identity = identity;
			this.Trustee = trustee;
			this.AccessControlType = ace.AccessControlType;
			this.AccessRights = new MultiValuedProperty<RecipientAccessRight>(accessRight);
			this.IsInherited = ace.IsInherited;
			this.InheritanceType = ace.InheritanceType;
		}

		public new ObjectId Identity
		{
			get
			{
				return base.Identity;
			}
			private set
			{
				this[SimpleProviderObjectSchema.Identity] = value;
			}
		}

		public string Trustee
		{
			get
			{
				return (string)this[RecipientPermissionSchema.Trustee];
			}
			private set
			{
				this[RecipientPermissionSchema.Trustee] = value;
			}
		}

		public AccessControlType AccessControlType
		{
			get
			{
				return (AccessControlType)this[RecipientPermissionSchema.AccessControlType];
			}
			private set
			{
				this[RecipientPermissionSchema.AccessControlType] = value;
			}
		}

		public MultiValuedProperty<RecipientAccessRight> AccessRights
		{
			get
			{
				return (MultiValuedProperty<RecipientAccessRight>)this[RecipientPermissionSchema.AccessRights];
			}
			private set
			{
				this[RecipientPermissionSchema.AccessRights] = value;
			}
		}

		public bool IsInherited
		{
			get
			{
				return (bool)this[RecipientPermissionSchema.IsInherited];
			}
			private set
			{
				this[RecipientPermissionSchema.IsInherited] = value;
			}
		}

		public ActiveDirectorySecurityInheritance InheritanceType
		{
			get
			{
				return (ActiveDirectorySecurityInheritance)this[RecipientPermissionSchema.InheritanceType];
			}
			private set
			{
				this[RecipientPermissionSchema.InheritanceType] = value;
			}
		}

		private static readonly ObjectSchema schema = ObjectSchema.GetInstance<RecipientPermissionSchema>();
	}
}
