using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class RoleGroupDefinition
	{
		public Guid RoleGroupGuid { get; private set; }

		public string Name { get; private set; }

		public int Id { get; private set; }

		public string Description { get; private set; }

		public List<Guid> E12USG { get; private set; }

		public List<Datacenter.ExchangeSku> AlwaysCreateOnSku { get; private set; }

		internal ADGroup ADGroup { get; set; }

		public RoleGroupDefinition(RoleGroupDefinition roleGroup) : this(roleGroup.Name, roleGroup.Id, roleGroup.RoleGroupGuid, roleGroup.Description, roleGroup.AlwaysCreateOnSku, roleGroup.E12USG.ToArray())
		{
		}

		public RoleGroupDefinition(RoleGroupInitInfo roleGroupInitInfo, string description, params Guid[] e12USG) : this(roleGroupInitInfo.Name, roleGroupInitInfo.Id, roleGroupInitInfo.WellKnownGuid, description, null, e12USG)
		{
		}

		public RoleGroupDefinition(string name, int id, Guid wellKnownGuid, string description, params Guid[] e12USG) : this(name, id, wellKnownGuid, description, null, e12USG)
		{
		}

		public RoleGroupDefinition(RoleGroupInitInfo roleGroupInitInfo, string description, List<Datacenter.ExchangeSku> alwaysCreateOn, params Guid[] e12USG) : this(roleGroupInitInfo.Name, roleGroupInitInfo.Id, roleGroupInitInfo.WellKnownGuid, description, alwaysCreateOn, e12USG)
		{
		}

		public RoleGroupDefinition(string name, int id, Guid wellKnownGuid, string description, List<Datacenter.ExchangeSku> alwaysCreateOn, params Guid[] e12USG)
		{
			this.Name = name;
			this.RoleGroupGuid = wellKnownGuid;
			this.Description = description;
			this.Id = id;
			this.E12USG = ((e12USG == null) ? new List<Guid>(0) : e12USG.ToList<Guid>());
			this.ADGroup = null;
			this.AlwaysCreateOnSku = alwaysCreateOn;
		}

		public LocalizedException GuidNotFoundException
		{
			get
			{
				string name;
				if ((name = this.Name) != null)
				{
					if (name == "Organization Management")
					{
						return new ExOrgAdminSGroupNotFoundException(this.RoleGroupGuid);
					}
					if (name == "Public Folder Management")
					{
						return new ExPublicFolderAdminSGroupNotFoundException(this.RoleGroupGuid);
					}
					if (name == "Recipient Management")
					{
						return new ExMailboxAdminSGroupNotFoundException(this.RoleGroupGuid);
					}
					if (name == "View-Only Organization Management")
					{
						return new ExOrgReadAdminSGroupNotFoundException(this.RoleGroupGuid);
					}
				}
				return new ExRbacRoleGroupNotFoundException(this.RoleGroupGuid, this.Name);
			}
		}
	}
}
