using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Update", "Recipient", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class UpdateRecipient : RecipientObjectActionTask<RecipientIdParameter, ADRecipient>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageUpdateRecipient(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public PSCredential Credential
		{
			get
			{
				return (PSCredential)base.Fields["Credential"];
			}
			set
			{
				base.Fields["Credential"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(this.domainRecipientSession, base.CurrentOrganizationId))
			{
				this.domainRecipientSession = (IRecipientSession)TaskHelper.UnderscopeSessionToOrganization(this.domainRecipientSession, base.CurrentOrganizationId, true);
			}
			return this.domainRecipientSession;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			if (this.Credential != null)
			{
				base.NetCredential = this.Credential.GetNetworkCredential();
			}
			base.InternalBeginProcessing();
			base.WriteVerbose(Strings.VerboseIgnoreDefaultScope);
			this.domainRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, base.NetCredential, base.OrgWideSessionSettings, 106, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\recipient\\UpdateRecipient.cs");
			this.domainRecipientSession.EnforceDefaultScope = false;
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			if (!OrganizationId.ForestWideOrgId.Equals(base.OrgWideSessionSettings.CurrentOrganizationId))
			{
				sessionSettings = ADSessionSettings.FromAccountPartitionRootOrgScopeSet(base.OrgWideSessionSettings.CurrentOrganizationId.PartitionId);
			}
			this.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, string.IsNullOrEmpty(base.DomainController) ? null : base.NetCredential, sessionSettings, 120, "InternalBeginProcessing", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\recipient\\UpdateRecipient.cs");
			this.localForestLinkResolutionServer = ADSession.GetCurrentConfigDC(base.OrgWideSessionSettings.GetAccountOrResourceForestFqdn());
			string fqdn = NativeHelpers.CanonicalNameFromDistinguishedName(this.configurationSession.GetRootDomainNamingContextFromCurrentReadConnection());
			if (ADForest.IsLocalForestFqdn(fqdn) || !OrganizationId.ForestWideOrgId.Equals(base.OrgWideSessionSettings.CurrentOrganizationId))
			{
				this.domainRecipientSession.LinkResolutionServer = this.localForestLinkResolutionServer;
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ResolveDataObject()
		{
			IConfigurable configurable = null;
			Exception innerException = null;
			ADObjectId adobjectId = null;
			ADObjectId rootID = RecipientTaskHelper.IsValidDistinguishedName(this.Identity, out adobjectId) ? adobjectId.Parent : null;
			try
			{
				configurable = base.GetDataObject<ADRecipient>(this.Identity, base.DataSession, rootID, null, new LocalizedString?(Strings.ErrorRecipientNotUnique(this.Identity.ToString())));
			}
			catch (ADTransientException ex)
			{
				innerException = ex;
				base.WriteVerbose(Strings.VerboseCannotReadObject(this.Identity.ToString(), base.DataSession.Source, ex.Message));
			}
			catch (ADOperationException ex2)
			{
				innerException = ex2;
				base.WriteVerbose(Strings.VerboseCannotReadObject(this.Identity.ToString(), base.DataSession.Source, ex2.Message));
			}
			catch (ManagementObjectNotFoundException ex3)
			{
				innerException = ex3;
				base.WriteVerbose(Strings.VerboseCannotReadObject(this.Identity.ToString(), base.DataSession.Source, ex3.Message));
			}
			if (configurable == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorObjectNotFound(this.Identity.ToString()), innerException), ErrorCategory.ObjectNotFound, this.Identity);
			}
			if (this.globalCatalog == null || (base.DomainController == null && !StringComparer.InvariantCultureIgnoreCase.Equals(this.configurationSession.DomainController, ((ADObject)configurable).OriginatingServer)))
			{
				if (base.DomainController == null)
				{
					ADObject adobject = (ADObject)configurable;
					string originatingServer = adobject.OriginatingServer;
					ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
					if (!adobject.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
					{
						sessionSettings = ADSessionSettings.FromAccountPartitionRootOrgScopeSet(adobject.OrganizationId.PartitionId);
					}
					this.configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(originatingServer, true, ConsistencyMode.PartiallyConsistent, base.NetCredential, sessionSettings, 210, "ResolveDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\recipient\\UpdateRecipient.cs");
				}
				string text = NativeHelpers.CanonicalNameFromDistinguishedName(this.configurationSession.GetRootDomainNamingContextFromCurrentReadConnection());
				this.globalCatalog = null;
				if (this.IsServerSuitableAsGC(this.configurationSession.DomainController, this.configurationSession.NetworkCredential))
				{
					this.globalCatalog = this.configurationSession.DomainController;
				}
				else
				{
					NetworkCredential credentials = ADForest.IsLocalForestFqdn(text) ? null : this.configurationSession.NetworkCredential;
					ADForest forest = ADForest.GetForest(text, credentials);
					ReadOnlyCollection<ADServer> readOnlyCollection = forest.FindAllGlobalCatalogs(false);
					if (readOnlyCollection != null && readOnlyCollection.Count != 0)
					{
						foreach (ADServer adserver in readOnlyCollection)
						{
							if (this.IsServerSuitableAsGC(adserver.DnsHostName, this.configurationSession.NetworkCredential))
							{
								this.globalCatalog = adserver.DnsHostName;
								break;
							}
						}
					}
					if (string.IsNullOrEmpty(this.globalCatalog))
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorNoGlobalGatalogFound(text)), (ErrorCategory)1011, this.Identity);
					}
				}
				if (ADForest.IsLocalForestFqdn(text) || !OrganizationId.ForestWideOrgId.Equals(base.OrgWideSessionSettings.CurrentOrganizationId))
				{
					this.domainRecipientSession.LinkResolutionServer = this.localForestLinkResolutionServer;
				}
				else
				{
					this.domainRecipientSession.LinkResolutionServer = null;
				}
			}
			return configurable;
		}

		protected override void ProvisioningUpdateConfigurationObject()
		{
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADRecipient adrecipient = (ADRecipient)base.PrepareDataObject();
			if (base.IsProvisioningLayerAvailable)
			{
				Fqdn value = (Fqdn)base.UserSpecifiedParameters["DomainController"];
				try
				{
					base.UserSpecifiedParameters["DomainController"] = this.globalCatalog;
					ProvisioningLayer.UpdateAffectedIConfigurable(this, RecipientTaskHelper.ConvertRecipientToPresentationObject(adrecipient), false);
					goto IL_82;
				}
				finally
				{
					base.UserSpecifiedParameters["DomainController"] = value;
				}
			}
			base.WriteError(new InvalidOperationException(Strings.ErrorNoProvisioningHandlerAvailable), (ErrorCategory)1012, null);
			IL_82:
			if (RecipientType.UserMailbox == adrecipient.RecipientType)
			{
				ADUser aduser = (ADUser)adrecipient;
				if (string.IsNullOrEmpty(aduser.ServerLegacyDN))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorInvalidObjectMissingCriticalProperty(typeof(Mailbox).Name, adrecipient.Identity.ToString(), MailEnabledRecipientSchema.LegacyExchangeDN.Name)), (ErrorCategory)1009, this.Identity);
				}
				Server server = this.configurationSession.FindServerByLegacyDN(aduser.ServerLegacyDN);
				if (server != null)
				{
					if (!server.IsExchange2007OrLater)
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorCannotUpdateLegacyMailbox(this.Identity.ToString())), (ErrorCategory)1010, this.Identity);
					}
					else if (RecipientTaskHelper.IsE15OrLater(server.VersionNumber))
					{
						if (adrecipient.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2012))
						{
							adrecipient.SetExchangeVersion(ExchangeObjectVersion.Exchange2012);
						}
					}
					else if (server.IsE14OrLater)
					{
						if (adrecipient.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
						{
							adrecipient.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
						}
					}
					else if (adrecipient.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2007))
					{
						adrecipient.SetExchangeVersion(ExchangeObjectVersion.Exchange2007);
					}
				}
				SetMailboxBase<MailboxIdParameter, Mailbox>.StampMailboxTypeDetails(adrecipient, true);
				MailboxTaskHelper.StampMailboxRecipientDisplayType(adrecipient);
				if (server != null && server.IsE14OrLater)
				{
					NetID netID = aduser.NetID;
					if (netID != null)
					{
						aduser.NetID = netID;
					}
				}
				if (aduser.RoleAssignmentPolicy == null && RecipientTypeDetails.None == (aduser.RecipientTypeDetails & (RecipientTypeDetails.PublicFolder | RecipientTypeDetails.SystemMailbox | RecipientTypeDetails.ArbitrationMailbox | RecipientTypeDetails.DiscoveryMailbox | RecipientTypeDetails.AuditLogMailbox)))
				{
					RoleAssignmentPolicy roleAssignmentPolicy = RecipientTaskHelper.FindDefaultRoleAssignmentPolicy(RecipientTaskHelper.GetTenantLocalConfigSession(base.CurrentOrganizationId, base.ExecutingUserOrganizationId, base.RootOrgContainerId), new Task.ErrorLoggerDelegate(base.WriteError), Strings.ErrorDefaultRoleAssignmentPolicyNotUnique, Strings.ErrorDefaultRoleAssignmentPolicyNotFound);
					if (roleAssignmentPolicy != null)
					{
						aduser.RoleAssignmentPolicy = (ADObjectId)roleAssignmentPolicy.Identity;
					}
				}
			}
			TaskLogger.LogExit();
			return adrecipient;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalProcessRecord();
			}
			catch (DataValidationException ex)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCannotUpdateInvalidMailbox(this.Identity.ToString(), ex.Message), ex), ErrorCategory.InvalidOperation, this.Identity);
			}
			TaskLogger.LogExit();
		}

		private bool IsServerSuitableAsGC(string fqdn, NetworkCredential credential)
		{
			LocalizedString empty = LocalizedString.Empty;
			string text;
			bool flag = SuitabilityVerifier.IsServerSuitableIgnoreExceptions(fqdn, true, credential, out text, out empty);
			if (!flag)
			{
				TaskLogger.Trace("Server {0} is not suitable as a Global Catalog. Reason: {1}", new object[]
				{
					fqdn,
					empty
				});
			}
			return flag;
		}

		private const string ParamCredential = "Credential";

		private IRecipientSession domainRecipientSession;

		private ITopologyConfigurationSession configurationSession;

		private string globalCatalog;

		private string localForestLinkResolutionServer;
	}
}
