using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.CmdletInfra;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal static class CmdletLogHelper
	{
		internal static bool NeedConvertLogMessageToUS
		{
			get
			{
				return Datacenter.IsMicrosoftHostedOnly(true) && (!CmdletLogHelper.DefaultLoggingCulture.Equals(CultureInfo.CurrentUICulture) || !CmdletLogHelper.DefaultLoggingCulture.Equals(CultureInfo.CurrentCulture));
			}
		}

		internal static void LogADServerSettingsAfterCmdExecuted(Guid cmdletUniqueId, ADServerSettings serverSettings)
		{
			string key = "ADServerSettingsInEnd";
			StringBuilder stringBuilder = new StringBuilder();
			string format = "{0}:{1} ";
			if (serverSettings == null)
			{
				CmdletLogger.SafeAppendGenericInfo(cmdletUniqueId, key, "null");
				return;
			}
			stringBuilder.AppendFormat(format, RpsCmdletMetadata.ADViewEntireForest.ToString(), serverSettings.ViewEntireForest);
			if (serverSettings.RecipientViewRoot != null)
			{
				stringBuilder.AppendFormat(format, RpsCmdletMetadata.ADRecipientViewRoot.ToString(), serverSettings.RecipientViewRoot.ToCanonicalName());
			}
			if (serverSettings.ConfigurationDomainControllers != null)
			{
				stringBuilder.AppendFormat(format, RpsCmdletMetadata.ADConfigurationDomainControllers, string.Join<Fqdn>(",", serverSettings.ConfigurationDomainControllers.Values));
			}
			if (serverSettings.PreferredGlobalCatalogs != null)
			{
				stringBuilder.AppendFormat(format, RpsCmdletMetadata.ADPreferredGlobalCatalogs.ToString(), string.Join<Fqdn>(",", serverSettings.PreferredGlobalCatalogs.Values));
			}
			if (serverSettings.PreferredDomainControllers != null)
			{
				stringBuilder.AppendFormat(format, RpsCmdletMetadata.ADPreferredDomainControllers.ToString(), string.Join<Fqdn>(",", serverSettings.PreferredDomainControllers.ToArray()));
			}
			RunspaceServerSettings runspaceServerSettings = serverSettings as RunspaceServerSettings;
			if (runspaceServerSettings == null)
			{
				CmdletLogger.SafeAppendGenericInfo(cmdletUniqueId, key, stringBuilder.ToString());
				CmdletLogger.SafeAppendGenericInfo(cmdletUniqueId, "RunspaceServerSettings", "null");
				return;
			}
			if (runspaceServerSettings.UserConfigurationDomainController != null)
			{
				stringBuilder.AppendFormat(format, RpsCmdletMetadata.ADUserConfigurationDomainController.ToString(), runspaceServerSettings.UserConfigurationDomainController);
			}
			if (runspaceServerSettings.UserPreferredGlobalCatalog != null)
			{
				stringBuilder.AppendFormat(format, RpsCmdletMetadata.ADUserPreferredGlobalCatalog.ToString(), runspaceServerSettings.UserConfigurationDomainController);
			}
			if (runspaceServerSettings.UserPreferredDomainControllers != null)
			{
				stringBuilder.AppendFormat(format, RpsCmdletMetadata.ADUserPreferredDomainControllers.ToString(), string.Join<Fqdn>(",", runspaceServerSettings.UserPreferredDomainControllers.ToArray()));
			}
			CmdletLogger.SafeAppendGenericInfo(cmdletUniqueId, key, stringBuilder.ToString());
		}

		internal static void LogADServerSettings(Guid cmdletUniqueId, ADServerSettings serverSettings)
		{
			if (serverSettings == null)
			{
				CmdletLogger.SafeAppendGenericInfo(cmdletUniqueId, "ADServerSettings", "null");
				return;
			}
			CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.ADViewEntireForest, serverSettings.ViewEntireForest);
			if (serverSettings.RecipientViewRoot != null)
			{
				CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.ADRecipientViewRoot, serverSettings.RecipientViewRoot.ToCanonicalName());
			}
			if (serverSettings.ConfigurationDomainControllers != null)
			{
				CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.ADConfigurationDomainControllers, string.Join<Fqdn>(",", serverSettings.ConfigurationDomainControllers.Values));
			}
			if (serverSettings.PreferredGlobalCatalogs != null)
			{
				CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.ADPreferredGlobalCatalogs, string.Join<Fqdn>(",", serverSettings.PreferredGlobalCatalogs.Values));
			}
			if (serverSettings.PreferredDomainControllers != null)
			{
				CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.ADPreferredDomainControllers, string.Join<Fqdn>(",", serverSettings.PreferredDomainControllers.ToArray()));
			}
			RunspaceServerSettings runspaceServerSettings = serverSettings as RunspaceServerSettings;
			if (runspaceServerSettings == null)
			{
				CmdletLogger.SafeAppendGenericInfo(cmdletUniqueId, "RunspaceServerSettings", "null");
				return;
			}
			if (runspaceServerSettings.UserConfigurationDomainController != null)
			{
				CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.ADUserConfigurationDomainController, runspaceServerSettings.UserConfigurationDomainController);
			}
			if (runspaceServerSettings.UserPreferredGlobalCatalog != null)
			{
				CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.ADUserPreferredGlobalCatalog, runspaceServerSettings.UserConfigurationDomainController);
			}
			if (runspaceServerSettings.UserPreferredDomainControllers != null)
			{
				CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.ADUserPreferredDomainControllers, string.Join<Fqdn>(",", runspaceServerSettings.UserPreferredDomainControllers.ToArray()));
			}
		}

		internal static void SetCmdletErrorType(Guid cmdletUniqueId, string errorType)
		{
			string a = CmdletStaticDataWithUniqueId<string>.Get(cmdletUniqueId);
			if (a == "UnHandled")
			{
				return;
			}
			CmdletStaticDataWithUniqueId<string>.Set(cmdletUniqueId, errorType);
			CmdletLogger.SafeSetLogger(cmdletUniqueId, RpsCmdletMetadata.ErrorType, errorType);
		}

		internal static void ClearCmdletErrorType(Guid cmdletUniuqueId)
		{
			CmdletStaticDataWithUniqueId<string>.Remove(cmdletUniuqueId);
			CmdletLogger.SafeSetLogger(cmdletUniuqueId, RpsCmdletMetadata.ErrorType, string.Empty);
		}

		internal const string BizLogicLatencyFuncName = "BizLogic";

		internal const string CmdletProxyLatencyFuncName = "CmdletProxyLatency";

		internal const string PowerShellLatencyFuncName = "PowerShellLatency";

		internal const string UserInteractionLatencyFuncName = "UserInteractionLatency";

		internal const string ProvisioningLayerLatencyFuncName = "ProvisioningLayerLatency";

		internal const string TaskModuleFuncName = "TaskModuleLatency";

		internal static readonly CultureInfo DefaultLoggingCulture = CultureInfo.GetCultureInfo(1033);

		internal static readonly Dictionary<string, Enum> FuncNameToLogMetaDic = new Dictionary<string, Enum>
		{
			{
				"ParameterBinding",
				RpsCmdletMetadata.ParameterBinding
			},
			{
				"BeginProcessing",
				RpsCmdletMetadata.BeginProcessing
			},
			{
				"ProcessRecord",
				RpsCmdletMetadata.ProcessRecord
			},
			{
				"EndProcessing",
				RpsCmdletMetadata.EndProcessing
			},
			{
				"StopProcessing",
				RpsCmdletMetadata.StopProcessing
			},
			{
				"BizLogic",
				RpsCmdletMetadata.BizLogic
			},
			{
				"CmdletProxyLatency",
				RpsCmdletMetadata.CmdletProxyLatency
			},
			{
				"PowerShellLatency",
				RpsCmdletMetadata.PowerShellLatency
			},
			{
				"UserInteractionLatency",
				RpsCmdletMetadata.UserInteractionLatency
			},
			{
				"ProvisioningLayerLatency",
				RpsCmdletMetadata.ProvisioningLayerLatency
			}
		};
	}
}
