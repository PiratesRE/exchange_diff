using System;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class PartnerGroupAdminSyncUser : AdminSyncUser
	{
		public PartnerGroupAdminSyncUser(string dn, Guid objectGuid, Guid partnerGroupGuid) : base(dn, objectGuid)
		{
			if (partnerGroupGuid == Guid.Empty)
			{
				throw new ArgumentException("PartnerGroupGuid cannot be emtpy");
			}
			this.partnerGroupGuid = partnerGroupGuid;
		}

		public Guid PartnerGroupGuid
		{
			get
			{
				return this.partnerGroupGuid;
			}
		}

		public override string ToString()
		{
			return this.partnerGroupGuid.ToString();
		}

		private Guid partnerGroupGuid;
	}
}
