using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ExPolicyConfigProviderManager : PolicyConfigProviderManager<ExPolicyConfigProviderManager>
	{
		public override PolicyConfigProvider CreateForSyncEngine(Guid externalDirectoryOrganizationId, string auxiliaryStore, bool enablePolicyApplication = true, ExecutionLog logProvider = null)
		{
			return this.WrapKnownException(delegate
			{
				ExPolicyConfigProvider exPolicyConfigProvider = new ExPolicyConfigProvider(externalDirectoryOrganizationId, false, string.Empty, logProvider);
				if (enablePolicyApplication)
				{
					exPolicyConfigProvider.PolicyConfigChanged += ExPolicyConfigChangeHandler.Current.EventHandler;
				}
				return exPolicyConfigProvider;
			});
		}

		public IConfigurationSession CreateForCmdlet(IConfigurationSession configurationSession, ExecutionLog logProvider)
		{
			return (IConfigurationSession)this.WrapKnownException(() => new ExPolicyConfigProvider(configurationSession, logProvider));
		}

		public PolicyConfigProvider CreateForProcessingEngine(OrganizationId organizationId, ExecutionLog logProvider, string preferredDomainController)
		{
			return this.WrapKnownException(delegate
			{
				OrganizationId organizationId2 = organizationId;
				ExecutionLog logProvider2 = logProvider;
				return new ExPolicyConfigProvider(organizationId2, true, preferredDomainController, logProvider2);
			});
		}

		public PolicyConfigProvider CreateForProcessingEngine(OrganizationId organizationId)
		{
			return this.WrapKnownException(() => new ExPolicyConfigProvider(organizationId, true, "", null));
		}

		public PolicyConfigProvider CreateForTest(Guid externalDirectoryOrganizationId, string auxiliaryStore)
		{
			return this.WrapKnownException(() => new ExPolicyConfigProvider(externalDirectoryOrganizationId, false, "", null));
		}

		public PolicyConfigProvider CreateForTest(OrganizationId organizationId, ExecutionLog logger)
		{
			return this.WrapKnownException(() => new ExPolicyConfigProvider(organizationId, false, "", logger));
		}

		private PolicyConfigProvider WrapKnownException(Func<PolicyConfigProvider> createProviderDelegate)
		{
			PolicyConfigProvider result;
			try
			{
				result = createProviderDelegate();
			}
			catch (DataSourceOperationException ex)
			{
				throw new PolicyConfigProviderPermanentException(ex.Message, ex);
			}
			catch (DataValidationException ex2)
			{
				throw new PolicyConfigProviderPermanentException(ex2.Message, ex2);
			}
			catch (DataSourceTransientException ex3)
			{
				throw new PolicyConfigProviderTransientException(ex3.Message, ex3);
			}
			return result;
		}
	}
}
