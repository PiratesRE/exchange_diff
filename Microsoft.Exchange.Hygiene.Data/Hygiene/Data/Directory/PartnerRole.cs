using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class PartnerRole : Role
	{
		public PartnerRole()
		{
		}

		public PartnerRole(string callerId, string roleName)
		{
			if (string.IsNullOrEmpty(callerId))
			{
				throw new ArgumentNullException("callerId");
			}
			if (string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException("roleName");
			}
			this.PartnerCallerId = callerId;
			base.RoleName = roleName;
		}

		public string PartnerCallerId
		{
			get
			{
				return this[PartnerRole.PartnerCallerIdDef] as string;
			}
			internal set
			{
				this[PartnerRole.PartnerCallerIdDef] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				if (string.IsNullOrWhiteSpace(base.RoleGroupName))
				{
					return new ConfigObjectId(string.Format("{0}_{1}_{2}", this.PartnerCallerId, base.RoleName, base.RoleEntryName));
				}
				return new ConfigObjectId(string.Format("{0}_{1}_{2}_{3}", new object[]
				{
					this.PartnerCallerId,
					base.RoleGroupName,
					base.RoleName,
					base.RoleEntryName
				}));
			}
		}

		internal static readonly HygienePropertyDefinition PartnerCallerIdDef = new HygienePropertyDefinition("callerId", typeof(string));
	}
}
