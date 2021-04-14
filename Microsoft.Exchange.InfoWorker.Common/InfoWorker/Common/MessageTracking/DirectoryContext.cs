using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class DirectoryContext
	{
		public ITopologyConfigurationSession GlobalConfigSession
		{
			get
			{
				return this.globalConfigSession;
			}
		}

		public IConfigurationSession TenantConfigSession
		{
			get
			{
				return this.tenantConfigSession;
			}
		}

		public IRecipientSession TenantGalSession
		{
			get
			{
				return this.tenantGalSession;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		public ClientContext ClientContext
		{
			get
			{
				return this.clientContext;
			}
		}

		public DiagnosticsContext DiagnosticsContext
		{
			get
			{
				return this.diagnosticsContext;
			}
		}

		public TrackingEventBudget TrackingBudget { get; internal set; }

		public TrackingErrorCollection Errors { get; private set; }

		public DirectoryContext(ClientContext clientContext, OrganizationId organizationId, ITopologyConfigurationSession globalConfigSession, IConfigurationSession tenantConfigSession, IRecipientSession tenantGalSession, TrackingEventBudget trackingBudget, DiagnosticsLevel diagnosticsLevel, TrackingErrorCollection errors, bool suppressIdAllocation)
		{
			this.clientContext = clientContext;
			this.organizationId = organizationId;
			this.globalConfigSession = globalConfigSession;
			this.tenantConfigSession = tenantConfigSession;
			this.tenantGalSession = tenantGalSession;
			this.diagnosticsContext = new DiagnosticsContext(suppressIdAllocation, diagnosticsLevel);
			this.TrackingBudget = trackingBudget;
			this.Errors = errors;
			if (!this.TrySetExternalOrgId(organizationId))
			{
				TraceWrapper.SearchLibraryTracer.TraceError(0, "Failed to set ExternalOrgId. Assuming forest wide organization", new object[0]);
			}
		}

		public bool IsTenantInScope(string tenantId)
		{
			return string.IsNullOrEmpty(this.externalOrganizationIdString) || string.Equals(this.externalOrganizationIdString, tenantId, StringComparison.OrdinalIgnoreCase);
		}

		public void Acquire()
		{
			Monitor.Enter(this);
		}

		public void Yield()
		{
			Monitor.Exit(this);
		}

		private bool TrySetExternalOrgId(OrganizationId orgId)
		{
			if (orgId.Equals(OrganizationId.ForestWideOrgId))
			{
				this.externalOrganizationIdString = string.Empty;
				return true;
			}
			ExchangeConfigurationUnit configUnitPassedToDelegate = null;
			Guid empty = Guid.Empty;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId), 241, "TrySetExternalOrgId", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\MessageTracking\\DirectoryContext.cs");
				configUnitPassedToDelegate = configurationSession.Read<ExchangeConfigurationUnit>(orgId.ConfigurationUnit);
			});
			if (!adoperationResult.Succeeded)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string, Exception>(0, "Failed to get ExternalOrgId from AD. {0} Error: {1}", (adoperationResult.ErrorCode == ADOperationErrorCode.PermanentError) ? "Permanent" : "Retriable", adoperationResult.Exception);
				return false;
			}
			if (configUnitPassedToDelegate == null || !Guid.TryParse(configUnitPassedToDelegate.ExternalDirectoryOrganizationId, out empty))
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug(0, "Failed read ExternalOrgId from AD Session", new object[0]);
				return false;
			}
			this.externalOrganizationIdString = empty.ToString();
			return true;
		}

		private ITopologyConfigurationSession globalConfigSession;

		private IConfigurationSession tenantConfigSession;

		private IRecipientSession tenantGalSession;

		private OrganizationId organizationId;

		private string externalOrganizationIdString;

		private ClientContext clientContext;

		private DiagnosticsContext diagnosticsContext;
	}
}
