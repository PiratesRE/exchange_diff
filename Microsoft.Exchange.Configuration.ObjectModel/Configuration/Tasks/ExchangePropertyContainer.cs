using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.Provisioning;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal sealed class ExchangePropertyContainer : IRunspaceObserver
	{
		private ExchangePropertyContainer()
		{
		}

		public void Activate()
		{
			if (this.budget != null)
			{
				OverBudgetException ex;
				if (this.budget.TryCheckOverBudget(CostType.ActiveRunspace, out ex) && (ex.PolicyPart == "MaxConcurrency" || ex.PolicyPart == "LocalTime"))
				{
					throw ex;
				}
				if (this.activeRunSpaceCostHandle != null)
				{
					this.activeRunSpaceCostHandle.Dispose();
				}
				this.activeRunSpaceCostHandle = this.budget.StartActiveRunspace();
			}
		}

		public void Deactivate()
		{
			if (this.activeRunSpaceCostHandle != null)
			{
				this.activeRunSpaceCostHandle.Dispose();
				this.activeRunSpaceCostHandle = null;
			}
			if (this.exchangeRunspaceConfiguration.EnablePiiMap && !string.IsNullOrEmpty(this.exchangeRunspaceConfiguration.PiiMapId))
			{
				PiiMapManager.Instance.Remove(this.exchangeRunspaceConfiguration.PiiMapId);
			}
		}

		internal static void InitExchangePropertyContainer(ISessionState sessionState, ExchangeRunspaceConfiguration configuration)
		{
			ExchangePropertyContainer exchangePropertyContainer = new ExchangePropertyContainer();
			exchangePropertyContainer.exchangeRunspaceConfiguration = configuration;
			exchangePropertyContainer.budget = ExchangePropertyContainer.AcquirePowerShellBudget(configuration);
			if (sessionState.Variables.ContainsName(ExchangePropertyContainer.ADServerSettingsVarName))
			{
				exchangePropertyContainer.serverSettings = (sessionState.Variables[ExchangePropertyContainer.ADServerSettingsVarName] as ADServerSettings);
			}
			ExchangePropertyContainer.SetExchangePropertyContainer(sessionState, exchangePropertyContainer);
		}

		internal static void InitExchangePropertyContainer(InitialSessionState initialSessionState, ExchangeRunspaceConfiguration configuration)
		{
			ExchangePropertyContainer exchangePropertyContainer = new ExchangePropertyContainer();
			exchangePropertyContainer.logEntries = new CmdletLogEntries();
			exchangePropertyContainer.exchangeRunspaceConfiguration = configuration;
			exchangePropertyContainer.budget = ExchangePropertyContainer.AcquirePowerShellBudget(configuration);
			SessionStateVariableEntry item = new SessionStateVariableEntry(ExchangePropertyContainer.ExchangePropertyContainerName, exchangePropertyContainer, ExchangePropertyContainer.ExchangePropertyContainerName, ScopedItemOptions.ReadOnly | ScopedItemOptions.Constant | ScopedItemOptions.Private | ScopedItemOptions.AllScope);
			initialSessionState.Variables.Add(item);
		}

		internal static void InitExchangePropertyContainer(ISessionState sessionState, OrganizationId currentExecutingOrgId, ADObjectId currentExecutingUserId)
		{
			ExchangePropertyContainer exchangePropertyContainer = new ExchangePropertyContainer();
			exchangePropertyContainer.executingUserOrganizationId = currentExecutingOrgId;
			exchangePropertyContainer.executingUserId = currentExecutingUserId;
			if (sessionState.Variables.ContainsName(ExchangePropertyContainer.ADServerSettingsVarName))
			{
				exchangePropertyContainer.serverSettings = (sessionState.Variables[ExchangePropertyContainer.ADServerSettingsVarName] as ADServerSettings);
			}
			ExchangePropertyContainer.SetExchangePropertyContainer(sessionState, exchangePropertyContainer);
		}

		internal static void PropagateExchangePropertyContainer(ISessionState sessionState, RunspaceProxy runspace, bool propagateRBAC, bool propagateBudget, ADServerSettings adServerSettingsOverride, ExchangeRunspaceConfigurationSettings.ExchangeApplication application)
		{
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			ExchangePropertyContainer exchangePropertyContainer = new ExchangePropertyContainer();
			if (propertyContainer.exchangeRunspaceConfiguration != null)
			{
				propertyContainer.exchangeRunspaceConfiguration.TryGetExecutingUserId(out exchangePropertyContainer.executingUserId);
				exchangePropertyContainer.executingUserOrganizationId = propertyContainer.exchangeRunspaceConfiguration.OrganizationId;
				if (propagateRBAC)
				{
					exchangePropertyContainer.exchangeRunspaceConfiguration = propertyContainer.exchangeRunspaceConfiguration;
				}
				exchangePropertyContainer.propagatedClientAppId = application;
				if (propertyContainer.budget != null && propagateBudget)
				{
					exchangePropertyContainer.budget = ExchangePropertyContainer.AcquirePowerShellBudget(propertyContainer.exchangeRunspaceConfiguration);
				}
			}
			else
			{
				exchangePropertyContainer.executingUserId = propertyContainer.executingUserId;
				exchangePropertyContainer.executingUserOrganizationId = propertyContainer.executingUserOrganizationId;
			}
			exchangePropertyContainer.logEntries = propertyContainer.logEntries;
			exchangePropertyContainer.logEnabled = propertyContainer.logEnabled;
			if (adServerSettingsOverride == null)
			{
				exchangePropertyContainer.serverSettings = propertyContainer.serverSettings;
			}
			else
			{
				exchangePropertyContainer.serverSettings = adServerSettingsOverride;
			}
			runspace.SetVariable(ExchangePropertyContainer.ExchangePropertyContainerName, exchangePropertyContainer);
		}

		internal static bool InitializeExchangePropertyContainerIfNeeded(ISessionState sessionState, out ADObjectId executingUserId, out OrganizationId executingUserOrganizationId)
		{
			executingUserId = null;
			executingUserOrganizationId = null;
			if (sessionState != null && !ExchangePropertyContainer.IsContainerInitialized(sessionState))
			{
				ExTraceGlobals.LogTracer.Information(0L, "ExchangePropertyContainer is not initialized. Case of Service, Setup or Powershell with manually added snapin");
				executingUserOrganizationId = TaskHelper.ResolveCurrentUserOrganization(out executingUserId);
				if (executingUserOrganizationId == null)
				{
					executingUserOrganizationId = OrganizationId.ForestWideOrgId;
				}
				ExchangePropertyContainer.InitExchangePropertyContainer(sessionState, executingUserOrganizationId, executingUserId);
				return true;
			}
			return false;
		}

		internal static ADObjectId GetExecutingUserId(ISessionState sessionState)
		{
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			if (propertyContainer == null)
			{
				return null;
			}
			return propertyContainer.executingUserId;
		}

		internal static string GetSiteRedirectionTemplate(ISessionState sessionState)
		{
			ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = ExchangePropertyContainer.GetExchangeRunspaceConfiguration(sessionState);
			if (exchangeRunspaceConfiguration == null || exchangeRunspaceConfiguration.ConfigurationSettings == null)
			{
				return null;
			}
			return exchangeRunspaceConfiguration.ConfigurationSettings.SiteRedirectionTemplate;
		}

		internal static string GetPodRedirectionTemplate(ISessionState sessionState)
		{
			ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = ExchangePropertyContainer.GetExchangeRunspaceConfiguration(sessionState);
			if (exchangeRunspaceConfiguration == null || exchangeRunspaceConfiguration.ConfigurationSettings == null)
			{
				return null;
			}
			return exchangeRunspaceConfiguration.ConfigurationSettings.PodRedirectionTemplate;
		}

		internal static Uri GetOriginalConnectionUri(ISessionState sessionState)
		{
			ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = ExchangePropertyContainer.GetExchangeRunspaceConfiguration(sessionState);
			if (exchangeRunspaceConfiguration == null || exchangeRunspaceConfiguration.ConfigurationSettings == null)
			{
				return null;
			}
			return exchangeRunspaceConfiguration.ConfigurationSettings.OriginalConnectionUri;
		}

		internal static ExchangeRunspaceConfiguration GetExchangeRunspaceConfiguration(ISessionState sessionState)
		{
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			if (propertyContainer == null)
			{
				return null;
			}
			return propertyContainer.exchangeRunspaceConfiguration;
		}

		internal static ExchangeRunspaceConfiguration GetExchangeRunspaceConfiguration(InitialSessionState initialSessionState)
		{
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(initialSessionState);
			return propertyContainer.exchangeRunspaceConfiguration;
		}

		internal static IPowerShellBudget GetPowerShellBudget(ISessionState sessionState)
		{
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			if (propertyContainer == null)
			{
				return null;
			}
			return propertyContainer.budget;
		}

		internal static ExchangeRunspaceConfigurationSettings.ExchangeApplication GetPropagatedClientAppId(ISessionState sessionState)
		{
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			if (propertyContainer == null)
			{
				return ExchangeRunspaceConfigurationSettings.ExchangeApplication.Unknown;
			}
			return propertyContainer.propagatedClientAppId;
		}

		internal static OrganizationId GetExecutingUserOrganizationId(ISessionState sessionState)
		{
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			if (propertyContainer == null)
			{
				return null;
			}
			return propertyContainer.executingUserOrganizationId;
		}

		internal static bool IsContainerInitialized(ISessionState sessionState)
		{
			object obj;
			return sessionState.Variables.TryGetValue(ExchangePropertyContainer.ExchangePropertyContainerName, out obj) && obj is ExchangePropertyContainer;
		}

		internal static ExchangeRunspaceConfiguration UpdateExchangeRunspaceConfiguration(ISessionState sessionState)
		{
			WindowsIdentity current = WindowsIdentity.GetCurrent();
			ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = new ExchangeRunspaceConfiguration(current);
			if (!ExchangePropertyContainer.IsContainerInitialized(sessionState))
			{
				ExchangePropertyContainer.InitExchangePropertyContainer(sessionState, exchangeRunspaceConfiguration);
			}
			else
			{
				ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
				propertyContainer.exchangeRunspaceConfiguration = exchangeRunspaceConfiguration;
				if (propertyContainer.budget != null)
				{
					propertyContainer.budget.Dispose();
				}
				propertyContainer.budget = ExchangePropertyContainer.AcquirePowerShellBudget(exchangeRunspaceConfiguration);
			}
			return exchangeRunspaceConfiguration;
		}

		internal static void SetServerSettings(ISessionState sessionState, ADServerSettings serverSettings)
		{
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			if (propertyContainer == null)
			{
				throw new ArgumentException("sessionState");
			}
			propertyContainer.serverSettings = serverSettings;
		}

		internal static ADServerSettings GetServerSettings(ISessionState sessionState)
		{
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			if (propertyContainer == null)
			{
				throw new ArgumentException("sessionState");
			}
			return propertyContainer.serverSettings;
		}

		internal static CmdletLogEntries GetCmdletLogEntries(ISessionState sessionState)
		{
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			if (propertyContainer == null)
			{
				return null;
			}
			return propertyContainer.logEntries;
		}

		internal static bool IsCmdletLogEnabled(ISessionState sessionState)
		{
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			return propertyContainer != null && propertyContainer.logEnabled;
		}

		internal static void EnableCmdletLog(ISessionState sessionState)
		{
			ADObjectId adobjectId = null;
			OrganizationId organizationId = null;
			ExchangePropertyContainer.InitializeExchangePropertyContainerIfNeeded(sessionState, out adobjectId, out organizationId);
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			propertyContainer.logEnabled = true;
			propertyContainer.logEntries.IncreaseIndentation();
		}

		internal static void DisableCmdletLog(ISessionState sessionState)
		{
			ADObjectId adobjectId = null;
			OrganizationId organizationId = null;
			ExchangePropertyContainer.InitializeExchangePropertyContainerIfNeeded(sessionState, out adobjectId, out organizationId);
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			propertyContainer.logEnabled = false;
			propertyContainer.logEntries.DecreaseIndentation();
		}

		internal static ProvisioningBroker GetProvisioningBroker(ISessionState sessionState)
		{
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			if (propertyContainer == null)
			{
				throw new ArgumentException("sessionState");
			}
			ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = propertyContainer.exchangeRunspaceConfiguration;
			if (exchangeRunspaceConfiguration != null)
			{
				return exchangeRunspaceConfiguration.GetProvisioningBroker();
			}
			if (propertyContainer.provisioningBroker == null)
			{
				propertyContainer.provisioningBroker = new ProvisioningBroker();
			}
			return propertyContainer.provisioningBroker;
		}

		internal static void RefreshProvisioningBroker(ISessionState sessionState)
		{
			ExchangePropertyContainer propertyContainer = ExchangePropertyContainer.GetPropertyContainer(sessionState);
			if (propertyContainer == null)
			{
				throw new ArgumentException("sessionState");
			}
			ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = propertyContainer.exchangeRunspaceConfiguration;
			if (exchangeRunspaceConfiguration != null)
			{
				exchangeRunspaceConfiguration.RefreshProvisioningBroker();
			}
			if (propertyContainer.provisioningBroker != null)
			{
				propertyContainer.provisioningBroker = null;
			}
		}

		internal static void ResetPerOrganizationData(ISessionState sessionState)
		{
			ExchangePropertyContainer.SetServerSettings(sessionState, null);
			ExchangePropertyContainer.RefreshProvisioningBroker(sessionState);
		}

		private static IPowerShellBudget AcquirePowerShellBudget(ExchangeRunspaceConfiguration configuration)
		{
			IPowerShellBudget powerShellBudget;
			SecurityIdentifier callerSid;
			if (configuration.DelegatedPrincipal != null)
			{
				powerShellBudget = PowerShellBudget.Acquire(new DelegatedPrincipalBudgetKey(configuration.DelegatedPrincipal, BudgetType.PowerShell));
			}
			else if (!configuration.TryGetExecutingUserSid(out callerSid))
			{
				ADObjectId adobjectId;
				if (!configuration.TryGetExecutingUserId(out adobjectId))
				{
					throw new ExecutingUserPropertyNotFoundException("ExecutingUserSid");
				}
				powerShellBudget = PowerShellBudget.AcquireFallback(adobjectId.ObjectGuid.ToString(), BudgetType.PowerShell);
			}
			else
			{
				ADObjectId rootOrgId;
				if (configuration.ExecutingUserOrganizationId == null || configuration.ExecutingUserOrganizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					rootOrgId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
				}
				else
				{
					rootOrgId = ADSystemConfigurationSession.GetRootOrgContainerId(configuration.ExecutingUserOrganizationId.PartitionId.ForestFQDN, null, null);
				}
				powerShellBudget = PowerShellBudget.Acquire(callerSid, BudgetType.PowerShell, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgId, configuration.ExecutingUserOrganizationId, configuration.ExecutingUserOrganizationId, true));
			}
			PowerShellThrottlingPolicyUpdater.RevertExpiredThrottlingPolicyIfNeeded(powerShellBudget);
			if (configuration.IsPowerShellWebService)
			{
				IPowerShellBudget result = new PswsBudgetWrapper(((BudgetWrapper<PowerShellBudget>)powerShellBudget).GetInnerBudget());
				if (powerShellBudget != null)
				{
					powerShellBudget.Dispose();
				}
				return result;
			}
			return powerShellBudget;
		}

		internal static void SetExchangePropertyContainer(ISessionState sessionState, ExchangePropertyContainer container)
		{
			container.logEntries = new CmdletLogEntries();
			object obj;
			if (!sessionState.Variables.TryGetValue(ExchangePropertyContainer.ExchangePropertyContainerName, out obj) || !(obj is ExchangePropertyContainer))
			{
				VariableScopedOptions scope = VariableScopedOptions.AllScope | VariableScopedOptions.Constant | VariableScopedOptions.Private | VariableScopedOptions.ReadOnly;
				sessionState.Variables.Set(ExchangePropertyContainer.ExchangePropertyContainerName, container, scope);
			}
		}

		private static ExchangePropertyContainer GetPropertyContainer(ISessionState sessionState)
		{
			object obj;
			sessionState.Variables.TryGetValue(ExchangePropertyContainer.ExchangePropertyContainerName, out obj);
			return obj as ExchangePropertyContainer;
		}

		private static ExchangePropertyContainer GetPropertyContainer(InitialSessionState initialSessionState)
		{
			IList<SessionStateVariableEntry> list = initialSessionState.Variables[ExchangePropertyContainer.ExchangePropertyContainerName];
			ExchangePropertyContainer exchangePropertyContainer = null;
			if (list.Count > 0)
			{
				exchangePropertyContainer = (ExchangePropertyContainer)list[0].Value;
			}
			if (exchangePropertyContainer == null)
			{
				throw new ArgumentException("initialSessionState");
			}
			return exchangePropertyContainer;
		}

		internal static readonly string ADServerSettingsVarName = "ADServerSettings";

		private static string ExchangePropertyContainerName = "ExchangePropertyContainer";

		private CmdletLogEntries logEntries;

		private bool logEnabled;

		private ExchangeRunspaceConfiguration exchangeRunspaceConfiguration;

		private ADServerSettings serverSettings;

		private ProvisioningBroker provisioningBroker;

		private OrganizationId executingUserOrganizationId;

		private ADObjectId executingUserId;

		private IPowerShellBudget budget;

		private CostHandle activeRunSpaceCostHandle;

		private ExchangeRunspaceConfigurationSettings.ExchangeApplication propagatedClientAppId;
	}
}
