using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.ServiceModel;
using System.ServiceModel.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Ehf;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.EdgeSync
{
	[Cmdlet("Test", "EdgeSyncEhf", DefaultParameterSetName = "Health", SupportsShouldProcess = true)]
	public sealed class TestEdgeSyncEhf : TestEdgeSyncBase
	{
		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "CompareSyncedEntries")]
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

		[Parameter(Mandatory = false, ParameterSetName = "CompareSyncedEntries")]
		public EdgeSyncEhfConnectorIdParameter ConnectorId
		{
			get
			{
				return (EdgeSyncEhfConnectorIdParameter)base.Fields["ConnectorId"];
			}
			set
			{
				base.Fields["ConnectorId"] = value;
			}
		}

		protected override string CmdletMonitoringEventSource
		{
			get
			{
				return "MSExchange Monitoring EdgeSyncEhf";
			}
		}

		protected override string Service
		{
			get
			{
				return "EHF";
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestEdgeSyncEhf;
			}
		}

		internal override bool ReadConnectorLeasePath(IConfigurationSession session, ADObjectId rootId, out string primaryLeasePath, out string backupLeasePath, out bool hasOneConnectorEnabledInCurrentForest)
		{
			string text;
			backupLeasePath = (text = null);
			primaryLeasePath = text;
			hasOneConnectorEnabledInCurrentForest = false;
			EdgeSyncEhfConnector edgeSyncEhfConnector = base.FindSiteEdgeSyncConnector<EdgeSyncEhfConnector>(session, rootId, out hasOneConnectorEnabledInCurrentForest);
			if (edgeSyncEhfConnector == null)
			{
				return false;
			}
			primaryLeasePath = Path.Combine(edgeSyncEhfConnector.PrimaryLeaseLocation, "ehf.lease");
			backupLeasePath = Path.Combine(edgeSyncEhfConnector.BackupLeaseLocation, "ehf.lease");
			return true;
		}

		internal override ADObjectId GetCookieContainerId(IConfigurationSession session)
		{
			return EhfTargetConnection.GetCookieContainerId(session);
		}

		protected override EnhancedTimeSpan GetSyncInterval(EdgeSyncServiceConfig config)
		{
			return config.ConfigurationSyncInterval;
		}

		protected override void InternalEndProcessing()
		{
			if (this.ehfService != null)
			{
				this.ehfService.Dispose();
				this.ehfService = null;
			}
			base.InternalEndProcessing();
		}

		protected override void InternalProcessRecord()
		{
			if (this.Organization != null)
			{
				if (this.ehfService == null)
				{
					this.globalSession = this.CreateGlobalSession();
					EhfTargetServerConfig config = Utils.CreateEhfTargetConfig(this.globalSession, this.ConnectorId, this);
					this.edgeSyncEhfConnector = Utils.GetConnector(this.globalSession, this.ConnectorId, this);
					this.ehfService = new EhfProvisioningService(config);
				}
				ExchangeConfigurationUnit currentOrganization = this.GetCurrentOrganization(this.globalSession);
				ExchangeTenantRecord exchangeTenantRecord = this.CreateExchangeTenantRecord(currentOrganization);
				EhfCompanyRecord ehfCompanyRecord = this.CreateEhfCompanyRecord(exchangeTenantRecord.PerimeterConfig);
				IList<object> list = EhfSyncDiffRecord<object>.Compare(exchangeTenantRecord, ehfCompanyRecord);
				using (IEnumerator<object> enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object sendToPipeline = enumerator.Current;
						base.WriteObject(sendToPipeline);
					}
					return;
				}
			}
			base.TestGeneralSyncHealth();
		}

		private ExchangeTenantRecord CreateExchangeTenantRecord(ExchangeConfigurationUnit cu)
		{
			IConfigurationSession session = this.CreateScopedSession(cu);
			SyncedPerimeterConfig perimeterConfig = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ADPagedReader<SyncedPerimeterConfig> adpagedReader = session.FindAllPaged<SyncedPerimeterConfig>();
				if (adpagedReader != null)
				{
					foreach (SyncedPerimeterConfig perimeterConfig in adpagedReader)
					{
						if (perimeterConfig != null)
						{
							this.WriteError(new InvalidDataException(string.Format("expected 1 perimeterconfig object for {0} organization but found more than 1", cu.Name)), ErrorCategory.InvalidData, null);
						}
						perimeterConfig = perimeterConfig;
					}
				}
			});
			if (!adoperationResult.Succeeded)
			{
				base.WriteError(adoperationResult.Exception, ErrorCategory.NotSpecified, null);
			}
			if (perimeterConfig == null)
			{
				base.WriteError(new InvalidDataException(string.Format("expected 1 perimeterconfig object for {0} organization but found 0", cu.Name)), ErrorCategory.InvalidData, null);
			}
			List<SyncedAcceptedDomain> syncedAcceptedDomains = new List<SyncedAcceptedDomain>();
			adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ADPagedReader<SyncedAcceptedDomain> adpagedReader = session.FindAllPaged<SyncedAcceptedDomain>();
				if (adpagedReader != null)
				{
					foreach (SyncedAcceptedDomain syncedAcceptedDomain in adpagedReader)
					{
						if (syncedAcceptedDomain.DomainType == AcceptedDomainType.Authoritative || this.edgeSyncEhfConnector.Version > 1)
						{
							syncedAcceptedDomains.Add(syncedAcceptedDomain);
						}
					}
				}
			});
			if (!adoperationResult.Succeeded)
			{
				base.WriteError(adoperationResult.Exception, ErrorCategory.NotSpecified, null);
			}
			return new ExchangeTenantRecord(this.Organization, perimeterConfig, syncedAcceptedDomains);
		}

		private EhfCompanyRecord CreateEhfCompanyRecord(SyncedPerimeterConfig perimeterConfig)
		{
			EhfCompanyRecord result = null;
			Exception ex = null;
			try
			{
				Company company = null;
				int companyId;
				if (!this.ehfService.TryGetCompanyByGuid(perimeterConfig.Guid, out company) && int.TryParse(perimeterConfig.PerimeterOrgId, out companyId))
				{
					company = this.ehfService.GetCompany(companyId);
				}
				if (company != null)
				{
					Domain[] allDomains = this.ehfService.GetAllDomains(company.CompanyId);
					result = new EhfCompanyRecord(company, allDomains);
				}
			}
			catch (FaultException<ServiceFault> faultException)
			{
				ServiceFault detail = faultException.Detail;
				if (detail.Id == FaultId.UnableToConnectToDatabase)
				{
					ex = new InvalidOperationException("ServiceFault: EHF is unable to connect to its database", faultException);
				}
				else
				{
					if (detail.Id == FaultId.CompanyDoesNotExist)
					{
						return null;
					}
					ex = faultException;
				}
			}
			catch (MessageSecurityException ex2)
			{
				switch (EhfProvisioningService.DecodeMessageSecurityException(ex2))
				{
				case EhfProvisioningService.MessageSecurityExceptionReason.DatabaseFailure:
					ex = new InvalidOperationException("MessageSecurityException: EHF is unable to connect to its database", ex2.InnerException);
					goto IL_E6;
				case EhfProvisioningService.MessageSecurityExceptionReason.InvalidCredentials:
					ex = new InvalidOperationException("MessageSecurityException: EHF connector contains invalid credentials", ex2.InnerException);
					goto IL_E6;
				}
				ex = ex2;
				IL_E6:;
			}
			catch (CommunicationException ex3)
			{
				ex = ex3;
			}
			catch (TimeoutException ex4)
			{
				ex = ex4;
			}
			catch (EhfProvisioningService.ContractViolationException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidOperation, null);
			}
			return result;
		}

		private ExchangeConfigurationUnit GetCurrentOrganization(IConfigurationSession session)
		{
			ExchangeConfigurationUnit exchangeConfigurationUnit = null;
			IEnumerable<ExchangeConfigurationUnit> objects = this.Organization.GetObjects<ExchangeConfigurationUnit>(null, session);
			foreach (ExchangeConfigurationUnit exchangeConfigurationUnit2 in objects)
			{
				if (exchangeConfigurationUnit != null)
				{
					base.WriteError(new InvalidOperationException(string.Format("multiple organizations match {0} ", this.Organization.ToString())), ErrorCategory.InvalidArgument, null);
				}
				exchangeConfigurationUnit = exchangeConfigurationUnit2;
			}
			if (exchangeConfigurationUnit == null)
			{
				base.WriteError(new InvalidOperationException(string.Format("{0} is not found ", this.Organization.ToString())), ErrorCategory.InvalidArgument, null);
			}
			return exchangeConfigurationUnit;
		}

		private ITopologyConfigurationSession CreateGlobalSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 451, "CreateGlobalSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\EdgeSync\\TestEdgeSyncEhf.cs");
		}

		private IConfigurationSession CreateScopedSession(ExchangeConfigurationUnit cu)
		{
			ADObjectId rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(base.DomainController, null);
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerId, cu.OrganizationId, base.ExecutingUserOrganizationId, false);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 473, "CreateScopedSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\EdgeSync\\TestEdgeSyncEhf.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = true;
			return tenantOrTopologyConfigurationSession;
		}

		private const string CmdletNoun = "EdgeSyncEhf";

		private const string ServiceName = "EHF";

		private const string ParameterSetValidateEhfSync = "CompareSyncedEntries";

		private EhfProvisioningService ehfService;

		private ITopologyConfigurationSession globalSession;

		private EdgeSyncEhfConnector edgeSyncEhfConnector;
	}
}
