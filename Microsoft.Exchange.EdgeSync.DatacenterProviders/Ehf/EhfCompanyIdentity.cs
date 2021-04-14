using System;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal sealed class EhfCompanyIdentity
	{
		public int EhfCompanyId
		{
			get
			{
				return this.ehfCompanyId;
			}
		}

		public Guid EhfCompanyGuid
		{
			get
			{
				return this.ehfCompanyGuid;
			}
		}

		public EhfCompanyIdentity(int companyId, Guid companyGuid)
		{
			if (companyGuid == Guid.Empty)
			{
				throw new ArgumentException("CompanyGuid cannot be empty", "companyGuid");
			}
			this.ehfCompanyId = companyId;
			this.ehfCompanyGuid = companyGuid;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			EhfCompanyIdentity ehfCompanyIdentity = obj as EhfCompanyIdentity;
			return ehfCompanyIdentity != null && this.ehfCompanyGuid.Equals(ehfCompanyIdentity.ehfCompanyGuid);
		}

		public override int GetHashCode()
		{
			return this.ehfCompanyGuid.GetHashCode();
		}

		private int ehfCompanyId;

		private Guid ehfCompanyGuid;
	}
}
