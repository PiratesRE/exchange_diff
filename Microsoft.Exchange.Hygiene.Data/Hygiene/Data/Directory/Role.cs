using System;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class Role : ConfigurablePropertyBag
	{
		public Role()
		{
		}

		internal Role(string roleGroupName, RoleSet roleGroupSet, string roleName, RoleSet roleSet, string roleEntryName, string roleEntryParameter, RoleEntryType roleEntryType) : this(roleName, roleSet, roleEntryName, roleEntryParameter, roleEntryType)
		{
			if (string.IsNullOrEmpty(roleGroupName))
			{
				throw new ArgumentException("roleGroupName");
			}
			this.RoleGroupName = roleGroupName;
			this.RoleGroupSet = roleGroupSet;
		}

		internal Role(string roleName, RoleSet roleSet, string roleEntryName, string roleEntryParameter, RoleEntryType roleEntryType)
		{
			if (string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName");
			}
			if (string.IsNullOrEmpty(roleEntryName))
			{
				throw new ArgumentException("roleEntryName");
			}
			this.RoleName = roleName;
			this.RoleSet = roleSet;
			this.RoleEntryName = roleEntryName;
			this.RoleEntryType = roleEntryType;
			this.RoleEntryParameter = roleEntryParameter;
		}

		public Guid RoleGroupId
		{
			get
			{
				return (Guid)this[Role.RoleGroupIdDef];
			}
		}

		public string RoleGroupName
		{
			get
			{
				return this[Role.RoleGroupNameDef] as string;
			}
			internal set
			{
				this[Role.RoleGroupNameDef] = value;
			}
		}

		public RoleSet RoleGroupSet
		{
			get
			{
				return (RoleSet)this[Role.RoleGroupSetDef];
			}
			internal set
			{
				this[Role.RoleGroupSetDef] = value;
			}
		}

		public Guid RoleId
		{
			get
			{
				return (Guid)this[Role.RoleIdDef];
			}
		}

		public string RoleName
		{
			get
			{
				return this[Role.RoleNameDef] as string;
			}
			internal set
			{
				this[Role.RoleNameDef] = value;
			}
		}

		public RoleSet RoleSet
		{
			get
			{
				return (RoleSet)this[Role.RoleSetDef];
			}
			internal set
			{
				this[Role.RoleSetDef] = value;
			}
		}

		public string RoleEntryName
		{
			get
			{
				return this[Role.RoleEntryNameDef] as string;
			}
			internal set
			{
				this[Role.RoleEntryNameDef] = value;
			}
		}

		public string RoleEntryParameter
		{
			get
			{
				return this[Role.RoleEntryParametersDef] as string;
			}
			internal set
			{
				this[Role.RoleEntryParametersDef] = value;
			}
		}

		public RoleEntryType RoleEntryType
		{
			get
			{
				return (RoleEntryType)this[Role.RoleEntryTypeDef];
			}
			internal set
			{
				this[Role.RoleEntryTypeDef] = value;
			}
		}

		public string[] RoleEntryFeatures
		{
			get
			{
				string text = this[Role.RoleEntryFeaturesDef] as string;
				if (text != null)
				{
					return (from x in text.Split(new char[]
					{
						','
					}, StringSplitOptions.RemoveEmptyEntries)
					select x.Trim()).ToArray<string>();
				}
				return null;
			}
			internal set
			{
				if (value != null)
				{
					this[Role.RoleEntryFeaturesDef] = string.Join(", ", value);
					return;
				}
				this[Role.RoleEntryFeaturesDef] = null;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.RoleGroupName))
				{
					return new ConfigObjectId(this.RoleName + "_" + this.RoleEntryName);
				}
				return new ConfigObjectId(string.Concat(new string[]
				{
					this.RoleGroupName,
					"_",
					this.RoleName,
					"_",
					this.RoleEntryName
				}));
			}
		}

		internal static readonly HygienePropertyDefinition RoleGroupIdDef = new HygienePropertyDefinition("roleGroupId", typeof(Guid));

		internal static readonly HygienePropertyDefinition RoleGroupNameDef = new HygienePropertyDefinition("roleGroupName", typeof(string));

		internal static readonly HygienePropertyDefinition RoleGroupSetDef = new HygienePropertyDefinition("roleGroupSet", typeof(RoleSet));

		internal static readonly HygienePropertyDefinition RoleIdDef = new HygienePropertyDefinition("roleId", typeof(Guid));

		internal static readonly HygienePropertyDefinition RoleNameDef = new HygienePropertyDefinition("roleName", typeof(string));

		internal static readonly HygienePropertyDefinition RoleSetDef = new HygienePropertyDefinition("roleSet", typeof(RoleSet));

		internal static readonly HygienePropertyDefinition RoleEntryTypeDef = new HygienePropertyDefinition("roleEntryType", typeof(RoleEntryType));

		internal static readonly HygienePropertyDefinition RoleEntryNameDef = new HygienePropertyDefinition("roleEntryName", typeof(string));

		internal static readonly HygienePropertyDefinition RoleEntryParametersDef = new HygienePropertyDefinition("roleEntryParameters", typeof(string));

		internal static readonly HygienePropertyDefinition RoleEntryFeaturesDef = new HygienePropertyDefinition("roleEntryFeatures", typeof(string));
	}
}
