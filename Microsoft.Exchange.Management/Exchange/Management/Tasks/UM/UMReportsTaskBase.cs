using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	public abstract class UMReportsTaskBase<TIdentity> : ObjectActionTenantADTask<TIdentity, ADUser> where TIdentity : IIdentityParameter, new()
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

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.Organization != null)
			{
				IConfigurationSession configurationSession = this.CreateSessionToResolveConfigObjects(true);
				configurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, configurationSession, null, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				base.CurrentOrganizationId = adorganizationalUnit.OrganizationId;
			}
		}

		protected abstract void ProcessMailbox();

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, 107, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\UMReportsTaskBase.cs");
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADUser result = null;
			try
			{
				result = CommonUtil.ValidateAndReturnUMDataStorageOrgMbx(OrganizationMailbox.GetOrganizationMailboxesByCapability((IRecipientSession)base.DataSession, OrganizationCapability.UMDataStorage));
			}
			catch (ObjectNotFoundException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, null);
			}
			catch (NonUniqueRecipientException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ReadError, null);
			}
			return result;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.ProcessMailbox();
			TaskLogger.LogExit();
		}

		protected void ValidateCommonParamsAndSetOrg(UMDialPlanIdParameter dpParam, UMIPGatewayIdParameter gwParam, out Guid dpGuid, out Guid gwGuid, out string dpName, out string gwName)
		{
			dpGuid = Guid.Empty;
			gwGuid = Guid.Empty;
			dpName = string.Empty;
			gwName = string.Empty;
			if (dpParam == null && gwParam == null)
			{
				return;
			}
			IConfigurationSession session = this.CreateSessionToResolveConfigObjects(false);
			OrganizationId organizationId = null;
			if (dpParam != null)
			{
				UMDialPlan umdialPlan = (UMDialPlan)base.GetDataObject<UMDialPlan>(dpParam, session, null, new LocalizedString?(Strings.NonExistantDialPlan(dpParam.ToString())), new LocalizedString?(Strings.MultipleDialplansWithSameId(dpParam.ToString())));
				dpGuid = umdialPlan.Guid;
				dpName = umdialPlan.Name;
				organizationId = umdialPlan.OrganizationId;
			}
			if (gwParam != null)
			{
				UMIPGateway umipgateway = (UMIPGateway)base.GetDataObject<UMIPGateway>(gwParam, session, null, new LocalizedString?(Strings.NonExistantIPGateway(gwParam.ToString())), new LocalizedString?(Strings.MultipleIPGatewaysWithSameId(gwParam.ToString())));
				gwGuid = umipgateway.Guid;
				gwName = umipgateway.Name;
				if (organizationId != null && !organizationId.Equals(umipgateway.OrganizationId))
				{
					base.WriteError(new InvalidParameterException(Strings.MismatchedOrgInDPAndGW(dpParam.ToString(), gwParam.ToString())), ErrorCategory.InvalidArgument, null);
				}
				else
				{
					organizationId = umipgateway.OrganizationId;
				}
			}
			if (this.Organization != null)
			{
				organizationId != null;
			}
			if (organizationId != null)
			{
				base.CurrentOrganizationId = organizationId;
			}
		}

		internal MailboxSession ConnectToEDiscoveryMailbox(string clientString)
		{
			ADUser dataObject = this.DataObject;
			ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(dataObject, RemotingOptions.AllowCrossSite);
			return MailboxSession.OpenAsAdmin(mailboxOwner, CultureInfo.InvariantCulture, clientString);
		}

		private IConfigurationSession CreateSessionToResolveConfigObjects(bool scopeToExcecutingUser)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, scopeToExcecutingUser);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 278, "CreateSessionToResolveConfigObjects", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\um\\UMReportsTaskBase.cs");
		}
	}
}
