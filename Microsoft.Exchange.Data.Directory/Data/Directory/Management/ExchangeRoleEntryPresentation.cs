using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class ExchangeRoleEntryPresentation : ADPresentationObject, IConfigurable
	{
		public ExchangeRoleEntryPresentation()
		{
		}

		public ExchangeRoleEntryPresentation(ExchangeRole role, RoleEntry roleEntry) : base(role)
		{
			if (roleEntry is CmdletRoleEntry)
			{
				this.Type = ManagementRoleEntryType.Cmdlet;
				this.PSSnapinName = ((CmdletRoleEntry)roleEntry).PSSnapinName;
			}
			else if (roleEntry is ScriptRoleEntry)
			{
				this.Type = ManagementRoleEntryType.Script;
			}
			else if (roleEntry is ApplicationPermissionRoleEntry)
			{
				this.Type = ManagementRoleEntryType.ApplicationPermission;
			}
			else if (roleEntry is WebServiceRoleEntry)
			{
				this.Type = ManagementRoleEntryType.WebService;
			}
			this.Name = roleEntry.Name;
			this.identity = string.Format("{0}\\{1}", role.Id.ToString(), this.Name);
			this.Parameters = roleEntry.Parameters;
		}

		public new string Name
		{
			get
			{
				return this.name;
			}
			internal set
			{
				this.name = value;
			}
		}

		public ADObjectId Role
		{
			get
			{
				return (ADObjectId)this[ExchangeRoleEntryPresentationSchema.Role];
			}
			internal set
			{
				this[ExchangeRoleEntryPresentationSchema.Role] = value;
			}
		}

		public ManagementRoleEntryType Type
		{
			get
			{
				return this.roleEntryType;
			}
			internal set
			{
				this.roleEntryType = value;
			}
		}

		public ICollection<string> Parameters
		{
			get
			{
				return this.parameters;
			}
			internal set
			{
				this.parameters = value;
			}
		}

		public string PSSnapinName
		{
			get
			{
				return this.snapinName;
			}
			internal set
			{
				this.snapinName = value;
			}
		}

		ObjectId IConfigurable.Identity
		{
			get
			{
				return new ConfigObjectId(this.identity);
			}
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return ExchangeRoleEntryPresentation.schema;
			}
		}

		private static ExchangeRoleEntryPresentationSchema schema = ObjectSchema.GetInstance<ExchangeRoleEntryPresentationSchema>();

		private string identity;

		private ManagementRoleEntryType roleEntryType;

		private string name;

		private ICollection<string> parameters;

		private string snapinName;
	}
}
