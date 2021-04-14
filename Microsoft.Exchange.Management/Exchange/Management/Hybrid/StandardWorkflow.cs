using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class StandardWorkflow : WorkflowBase, IDisposable
	{
		public StandardWorkflow(ILogger logger, IUserInterface ui, HybridConfiguration hybridConfigurationObject, Func<IOnPremisesSession> createOnPremisesSession, Func<ITenantSession> createTenantSession)
		{
			base.AddOverhead(1);
			base.AddOverhead(1);
			base.AddTask(new TenantDetectionTask());
			base.AddTask(new UpgradeConfigurationFrom14Task(ExchangeObjectVersion.Exchange2012));
			base.AddTask(new GlobalPrereqTask());
			base.AddTask(new RecipientTask());
			base.AddTask(new OrganizationRelationshipTask());
			base.AddTask(new OnOffSettingsTask());
			base.AddTask(new MailFlowTask());
			base.AddTask(new MRSProxyTask());
			base.AddTask(new IOCConfigurationTask());
			LocalizedString hybridActivityEstablish = HybridStrings.HybridActivityEstablish;
			LocalizedString localizedString = HybridStrings.HybridConnectingToOnPrem;
			ui.WriteVerbose(localizedString);
			ui.WriteProgessIndicator(hybridActivityEstablish, localizedString, base.PercentCompleted);
			try
			{
				this.onPremisesSession = createOnPremisesSession();
			}
			catch (Exception ex)
			{
				if (ex is CouldNotResolveServerException || ex is CouldNotOpenRunspaceException)
				{
					throw new CouldNotCreateOnPremisesSessionException(ex, ex);
				}
				throw ex;
			}
			base.UpdateProgress(1);
			logger.LogInformation(HybridStrings.HybridInfoConnectedToOnPrem);
			localizedString = HybridStrings.HybridConnectingToTenant;
			ui.WriteVerbose(localizedString);
			ui.WriteProgessIndicator(hybridActivityEstablish, localizedString, base.PercentCompleted);
			try
			{
				this.tenantSession = createTenantSession();
			}
			catch (Exception ex2)
			{
				if (ex2 is CouldNotResolveServerException || ex2 is CouldNotOpenRunspaceException)
				{
					throw new CouldNotCreateTenantSessionException(ex2, ex2);
				}
				throw ex2;
			}
			base.UpdateProgress(1);
			logger.LogInformation(HybridStrings.HybridInfoConnectedToTenant);
			this.TaskContext = new TaskContext(ui, logger, hybridConfigurationObject, this.onPremisesSession, this.tenantSession);
			this.TaskContext.Parameters.Set<List<Uri>>("_onPremAcceptedTokenIssuerUris", Configuration.OnPremiseAcceptedTokenIssuerUriList);
			this.TaskContext.Parameters.Set<List<Uri>>("_tenantAcceptedTokenIssuerUris", Configuration.TenantAcceptedTokenIssuerUriList);
		}

		public ITaskContext TaskContext { get; private set; }

		public void Dispose()
		{
			if (this.onPremisesSession != null)
			{
				this.onPremisesSession.Dispose();
				this.onPremisesSession = null;
			}
			if (this.tenantSession != null)
			{
				this.tenantSession.Dispose();
				this.tenantSession = null;
			}
		}

		private const int OnPremisesSessionWeight = 1;

		private const int TenantSessionWeight = 1;

		private IOnPremisesSession onPremisesSession;

		private ITenantSession tenantSession;
	}
}
