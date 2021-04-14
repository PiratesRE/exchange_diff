using System;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Tasks;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal static class CmdletProxy
	{
		public static void ThrowExceptionIfProxyIsNeeded(TaskContext context, ADUser user, bool shouldAsyncProxy, LocalizedString confirmationMessage, CmdletProxyInfo.ChangeCmdletProxyParametersDelegate changeCmdletProxyParameters)
		{
			if (user == null)
			{
				return;
			}
			int remoteServerVersion;
			string remoteServerForADUser = TaskHelper.GetRemoteServerForADUser(user, new Task.TaskVerboseLoggingDelegate(context.CommandShell.WriteVerbose), out remoteServerVersion);
			CmdletProxy.ThrowExceptionIfProxyIsNeeded(context, remoteServerForADUser, remoteServerVersion, shouldAsyncProxy, confirmationMessage, changeCmdletProxyParameters);
		}

		public static void ThrowExceptionIfProxyIsNeeded(TaskContext context, string remoteServerFqdn, int remoteServerVersion, bool shouldAsyncProxy, LocalizedString confirmationMessage, CmdletProxyInfo.ChangeCmdletProxyParametersDelegate changeCmdletProxyParameters)
		{
			if (CmdletProxy.ShouldProxyCmdlet(context, remoteServerFqdn, remoteServerVersion))
			{
				throw new CmdletNeedsProxyException(new CmdletProxyInfo(remoteServerFqdn, remoteServerVersion, shouldAsyncProxy, confirmationMessage, changeCmdletProxyParameters));
			}
		}

		public static bool TryToProxyOutputObject(ICmdletProxyable cmdletProxyableObject, TaskContext context, ADUser user, bool shouldAsyncProxy, LocalizedString confirmationMessage, CmdletProxyInfo.ChangeCmdletProxyParametersDelegate changeCmdletProxyParameters)
		{
			if (user == null)
			{
				return false;
			}
			int remoteServerVersion;
			string remoteServerForADUser = TaskHelper.GetRemoteServerForADUser(user, new Task.TaskVerboseLoggingDelegate(context.CommandShell.WriteVerbose), out remoteServerVersion);
			return CmdletProxy.TryToProxyOutputObject(cmdletProxyableObject, context, remoteServerForADUser, remoteServerVersion, shouldAsyncProxy, confirmationMessage, changeCmdletProxyParameters);
		}

		public static bool TryToProxyOutputObject(ICmdletProxyable cmdletProxyableObject, TaskContext context, string remoteServerFqdn, int remoteServerVersion, bool shouldAsyncProxy, LocalizedString confirmationMessage, CmdletProxyInfo.ChangeCmdletProxyParametersDelegate changeCmdletProxyParameters)
		{
			if (CmdletProxy.ShouldProxyCmdlet(context, remoteServerFqdn, remoteServerVersion))
			{
				cmdletProxyableObject.SetProxyInfo(new CmdletProxyInfo(remoteServerFqdn, remoteServerVersion, shouldAsyncProxy, confirmationMessage, changeCmdletProxyParameters));
				return true;
			}
			return false;
		}

		public static CmdletProxyInfo.ChangeCmdletProxyParametersDelegate AppendIdentityToProxyCmdlet(ADObject adObject)
		{
			return delegate(PropertyBag parameters)
			{
				if (adObject != null && adObject.Id != null)
				{
					if (parameters.Contains("Identity"))
					{
						parameters.Remove("Identity");
					}
					parameters.Add("Identity", adObject.Id.DistinguishedName);
				}
			};
		}

		public static void AppendForceToProxyCmdlet(PropertyBag parameters)
		{
			if (parameters.Contains("Force"))
			{
				parameters.Remove("Force");
			}
			parameters.Add("Force", true);
		}

		private static bool ShouldProxyCmdlet(TaskContext context, string remoteServerFqdn, int remoteServerVersion)
		{
			if (string.IsNullOrWhiteSpace(remoteServerFqdn))
			{
				return false;
			}
			if (context.ExchangeRunspaceConfig == null)
			{
				return false;
			}
			if (ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
			{
				bool flag = false;
				ProxyHelper.FaultInjection_ShouldForcedlyProxyCmdlet(context.ExchangeRunspaceConfig.ConfigurationSettings.OriginalConnectionUri, remoteServerFqdn, ref flag);
				if (flag)
				{
					return true;
				}
			}
			string localServerFqdn = TaskHelper.GetLocalServerFqdn(null);
			if (string.IsNullOrEmpty(localServerFqdn))
			{
				CmdletLogger.SafeAppendGenericError(context.UniqueId, "ShouldProxyCmdlet", "GetLocalServerFqdn returns null/empty value", false);
				return false;
			}
			if (string.Equals(remoteServerFqdn, localServerFqdn, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (CmdletProxy.CanServerBeProxiedTo(remoteServerVersion))
			{
				return true;
			}
			CmdletLogger.SafeAppendGenericInfo(context.UniqueId, "ShouldProxyCmdlet", string.Format("Remote server version {0} doesn't support be proxied.", ProxyHelper.GetFriendlyVersionInformation(remoteServerVersion)));
			return false;
		}

		private static bool CanServerBeProxiedTo(int serverVersion)
		{
			return serverVersion > CmdletProxy.E14SupportProxyMinimumVersion;
		}

		private static readonly int E14SupportProxyMinimumVersion = new ServerVersion(14, 3, 79, 0).ToInt();
	}
}
