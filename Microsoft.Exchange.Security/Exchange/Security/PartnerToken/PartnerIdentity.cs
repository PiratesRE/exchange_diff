using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Security.PartnerToken
{
	internal class PartnerIdentity : WindowsIdentity, IOrganizationScopedIdentity, IIdentity
	{
		private PartnerIdentity(DelegatedPrincipal delegatedPrincipal, OrganizationId delegatedOrganizationId, IntPtr token) : base(token)
		{
			this.delegatedPrincipal = delegatedPrincipal;
			this.delegatedOrganizationId = delegatedOrganizationId;
		}

		public DelegatedPrincipal DelegatedPrincipal
		{
			get
			{
				return this.delegatedPrincipal;
			}
		}

		public OrganizationId DelegatedOrganizationId
		{
			get
			{
				return this.delegatedOrganizationId;
			}
		}

		string IIdentity.AuthenticationType
		{
			get
			{
				return "Partner";
			}
		}

		bool IIdentity.IsAuthenticated
		{
			get
			{
				return true;
			}
		}

		string IIdentity.Name
		{
			get
			{
				return this.delegatedPrincipal.ToString();
			}
		}

		OrganizationId IOrganizationScopedIdentity.OrganizationId
		{
			get
			{
				return this.DelegatedOrganizationId;
			}
		}

		public static PartnerIdentity Create(DelegatedPrincipal delegatedPrincipal, OrganizationId delegatedOrganizationId)
		{
			PartnerIdentity result;
			using (WindowsIdentity current = WindowsIdentity.GetCurrent())
			{
				result = new PartnerIdentity(delegatedPrincipal, delegatedOrganizationId, current.Token);
			}
			return result;
		}

		IStandardBudget IOrganizationScopedIdentity.AcquireBudget()
		{
			return StandardBudget.Acquire(new DelegatedPrincipalBudgetKey(this.DelegatedPrincipal, BudgetType.Ews));
		}

		private const string PartnerAuthenticationType = "Partner";

		private readonly DelegatedPrincipal delegatedPrincipal;

		private readonly OrganizationId delegatedOrganizationId;
	}
}
