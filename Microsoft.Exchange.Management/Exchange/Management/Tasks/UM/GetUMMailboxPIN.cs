using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.Rpc;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Get", "UMMailboxPin", DefaultParameterSetName = "Identity")]
	public class GetUMMailboxPIN : GetRecipientObjectTask<MailboxIdParameter, ADUser>
	{
		[Parameter]
		public SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return base.InternalIgnoreDefaultScope;
			}
			set
			{
				base.InternalIgnoreDefaultScope = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreErrors
		{
			get
			{
				return (SwitchParameter)(base.Fields["IgnoreErrors"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IgnoreErrors"] = value;
			}
		}

		[Parameter]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter internalFilter = base.InternalFilter;
				QueryFilter queryFilter = new BitMaskAndFilter(ADUserSchema.UMEnabledFlags, 1UL);
				if (internalFilter != null)
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						internalFilter,
						queryFilter
					});
				}
				return queryFilter;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				if (!(base.CurrentOrganizationId == null))
				{
					return base.CurrentOrganizationId.OrganizationalUnit;
				}
				return base.RootId;
			}
		}

		protected override void InternalProcessRecord()
		{
			this.matchFound = false;
			try
			{
				base.InternalProcessRecord();
			}
			catch (InvalidOperationForGetUMMailboxPinException ex)
			{
				this.WriteWarningOrError(ex.LocalizedString);
				return;
			}
			if (this.Identity != null && !this.matchFound)
			{
				LocalizedString errorMessageObjectNotFound = base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), null);
				base.WriteError(new ManagementObjectNotFoundException(errorMessageObjectNotFound), ErrorCategory.InvalidData, this.Identity);
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			PINInfo pininfo = null;
			ADUser aduser = dataObject as ADUser;
			this.userObject = aduser;
			if (UMSubscriber.IsValidSubscriber(aduser))
			{
				this.matchFound = true;
				try
				{
					using (IUMUserMailboxStorage umuserMailboxAccessor = InterServerMailboxAccessor.GetUMUserMailboxAccessor(aduser, false))
					{
						pininfo = umuserMailboxAccessor.GetUMPin();
					}
					goto IL_84;
				}
				catch (LocalizedException ex)
				{
					throw new InvalidOperationForGetUMMailboxPinException(Strings.GetPINInfoError(aduser.PrimarySmtpAddress.ToString(), ex.LocalizedString), ex);
				}
				goto IL_64;
				IL_84:
				return new UMMailboxPin(aduser, pininfo.PinExpired, pininfo.LockedOut, pininfo.FirstTimeUser, base.NeedSuppressingPiiData);
			}
			IL_64:
			throw new InvalidOperationForGetUMMailboxPinException(Strings.InvalidUMUserName(aduser.PrimarySmtpAddress.ToString()));
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.NetCredential, sessionSettings, ConfigScopes.TenantSubTree, 236, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\GetUMMailboxpin.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				return adorganizationalUnit.OrganizationId;
			}
			return base.ResolveCurrentOrganization();
		}

		private void WriteWarningOrError(LocalizedString message)
		{
			if (this.IgnoreErrors.IsPresent)
			{
				this.WriteWarning(message);
				return;
			}
			base.WriteError(new LocalizedException(message), ErrorCategory.InvalidArgument, this.userObject);
		}

		private bool matchFound;

		private ADUser userObject;
	}
}
