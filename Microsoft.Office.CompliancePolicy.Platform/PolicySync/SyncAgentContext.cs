using System;
using Microsoft.Office.CompliancePolicy.Monitor;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public sealed class SyncAgentContext
	{
		public SyncAgentContext(SyncAgentConfiguration config, ICredentialsFactory credentialFactory, ITenantInfoProviderFactory tenantInfoProviderFactory, IPolicyConfigProviderManager policyConfigProviderFactory, HostStateProvider hostStateProvider, ExecutionLog logProvider, IMonitoringNotification monitorProvider, PerfCounterProvider perfCounterProvider) : this(config, new PolicySyncWebserviceClientFactory(logProvider), credentialFactory, tenantInfoProviderFactory, policyConfigProviderFactory, hostStateProvider, logProvider, new JobFactory(), monitorProvider, perfCounterProvider)
		{
		}

		internal SyncAgentContext(SyncAgentConfiguration config, IPolicySyncWebserviceClientFactory syncSvcClientFactory, ICredentialsFactory credentialFactory, ITenantInfoProviderFactory tenantInfoProviderFactory, IPolicyConfigProviderManager policyConfigProviderFactory, HostStateProvider hostStateProvider, ExecutionLog logProvider, IJobFactory jobFactory, IMonitoringNotification monitorProvider, PerfCounterProvider perfCounterProvider)
		{
			ArgumentValidator.ThrowIfNull("config", config);
			ArgumentValidator.ThrowIfNull("hostStateProvider", hostStateProvider);
			ArgumentValidator.ThrowIfNull("logProvider", logProvider);
			ArgumentValidator.ThrowIfNull("jobFactory", jobFactory);
			ArgumentValidator.ThrowIfNull("syncSvcClientFactory", syncSvcClientFactory);
			ArgumentValidator.ThrowIfNull("credentialFactory", credentialFactory);
			ArgumentValidator.ThrowIfNull("tenantInfoProviderFactory", tenantInfoProviderFactory);
			ArgumentValidator.ThrowIfNull("policyConfigProviderFactory", policyConfigProviderFactory);
			ArgumentValidator.ThrowIfNull("monitorProvider", monitorProvider);
			ArgumentValidator.ThrowIfNull("perfCounterProvider", perfCounterProvider);
			this.SyncAgentConfig = config;
			this.SyncSvcClientFactory = syncSvcClientFactory;
			this.CredentialFactory = credentialFactory;
			this.TenantInfoProviderFactory = tenantInfoProviderFactory;
			this.PolicyConfigProviderFactory = policyConfigProviderFactory;
			this.HostStateProvider = hostStateProvider;
			this.LogProvider = logProvider;
			this.JobFactory = jobFactory;
			this.MonitorProvider = monitorProvider;
			this.PerfCounterProvider = perfCounterProvider;
		}

		public SyncAgentConfiguration SyncAgentConfig { get; private set; }

		public ICredentialsFactory CredentialFactory { get; private set; }

		public ITenantInfoProviderFactory TenantInfoProviderFactory { get; private set; }

		public IPolicyConfigProviderManager PolicyConfigProviderFactory { get; private set; }

		public HostStateProvider HostStateProvider { get; private set; }

		public ExecutionLog LogProvider { get; private set; }

		public IMonitoringNotification MonitorProvider { get; private set; }

		internal IPolicySyncWebserviceClientFactory SyncSvcClientFactory { get; private set; }

		internal IJobFactory JobFactory { get; private set; }

		internal PerfCounterProvider PerfCounterProvider { get; private set; }
	}
}
