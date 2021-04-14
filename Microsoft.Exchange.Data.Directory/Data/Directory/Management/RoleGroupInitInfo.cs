using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	public struct RoleGroupInitInfo
	{
		public string Name
		{
			get
			{
				return this.roleGroupName;
			}
		}

		public int Id
		{
			get
			{
				return this.roleGroupId;
			}
		}

		public Guid WellKnownGuid
		{
			get
			{
				return this.wellKnownGuid;
			}
		}

		public RoleGroupInitInfo(string name, int id, Guid wkGuid)
		{
			this.roleGroupName = name;
			this.roleGroupId = id;
			this.wellKnownGuid = wkGuid;
		}

		private string roleGroupName;

		private int roleGroupId;

		private Guid wellKnownGuid;
	}
}
