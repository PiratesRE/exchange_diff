using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	public abstract class UMPromptTaskBase<TIdentity> : SystemConfigurationObjectActionTask<TIdentity, UMDialPlan> where TIdentity : IIdentityParameter, new()
	{
		public override TIdentity Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		public abstract UMAutoAttendantIdParameter UMAutoAttendant { get; set; }

		public abstract UMDialPlanIdParameter UMDialPlan { get; set; }

		protected UMAutoAttendant AutoAttendant { get; set; }

		protected override IConfigDataProvider CreateSession()
		{
			if (this.UMDialPlan != null)
			{
				return base.CreateSession();
			}
			if (this.UMAutoAttendant != null)
			{
				return this.CreateSessionForAA();
			}
			ExAssert.RetailAssert(false, "Invalid option. Either UMAutoAttendant or UMDialplan optons are valid");
			return null;
		}

		private IConfigDataProvider CreateSessionForAA()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.CreateSession();
			this.AutoAttendant = (UMAutoAttendant)base.GetDataObject<UMAutoAttendant>(this.UMAutoAttendant, configurationSession, null, new LocalizedString?(Strings.NonExistantAutoAttendant(this.UMAutoAttendant.ToString())), new LocalizedString?(Strings.MultipleAutoAttendantsWithSameId(this.UMAutoAttendant.ToString())));
			configurationSession = (IConfigurationSession)TaskHelper.UnderscopeSessionToOrganization(configurationSession, this.AutoAttendant.OrganizationId, true);
			base.VerifyIsWithinScopes(configurationSession, this.AutoAttendant, true, new DataAccessTask<UMDialPlan>.ADObjectOutOfScopeString(Strings.ScopeErrorOnAutoAttendant));
			this.Identity = (TIdentity)((object)new UMDialPlanIdParameter(this.AutoAttendant.UMDialPlan));
			return configurationSession;
		}
	}
}
