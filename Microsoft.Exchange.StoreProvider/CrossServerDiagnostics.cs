using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CrossServerDiagnostics : ICrossServerDiagnostics
	{
		internal static ICrossServerDiagnostics Instance
		{
			get
			{
				return CrossServerDiagnostics.instance;
			}
		}

		private string AppName
		{
			get
			{
				if (this.appName == null)
				{
					this.appName = ExWatson.AppName;
				}
				return this.appName;
			}
		}

		private string AppVersion
		{
			get
			{
				if (this.appVersion == null)
				{
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						Version version;
						if (ExWatson.TryGetRealApplicationVersion(currentProcess, out version))
						{
							this.appVersion = version.ToString();
						}
						else
						{
							this.appVersion = "0";
						}
					}
				}
				return this.appVersion;
			}
		}

		private string GetSuitableMethodNameFromStackTrace(StackTrace stackTrace)
		{
			StackFrame[] frames = stackTrace.GetFrames();
			int i;
			MethodBase method;
			for (i = 0; i < frames.Length; i++)
			{
				method = frames[i].GetMethod();
				string fullName = method.ReflectedType.FullName;
				if (!fullName.StartsWith("Microsoft.Mapi") && !fullName.StartsWith("Microsoft.Exchange.Data.Storage"))
				{
					break;
				}
			}
			if (i == frames.Length)
			{
				i = 0;
			}
			method = frames[i].GetMethod();
			return method.ReflectedType.FullName + "." + method.Name;
		}

		public void LogInfoWatson(ExRpcConnectionInfo connectionInfo)
		{
			if (!CrossServerConnectionRegistryParameters.IsCrossServerLoggingEnabled())
			{
				return;
			}
			StackTrace stackTrace = new StackTrace(1);
			MethodBase method = stackTrace.GetFrame(0).GetMethod();
			AssemblyName assemblyName = (method.DeclaringType != null) ? method.DeclaringType.Assembly.GetName() : Assembly.GetCallingAssembly().GetName();
			string suitableMethodNameFromStackTrace = this.GetSuitableMethodNameFromStackTrace(stackTrace);
			string normalizedClientInfo = connectionInfo.ApplicationId.GetNormalizedClientInfo();
			int hashCode = (suitableMethodNameFromStackTrace + normalizedClientInfo).GetHashCode();
			string detailedExceptionInformation = string.Format("Connection from {0} to {1} is disallowed. ConnectionInfo: {2}", ExRpcConnectionInfo.GetLocalServerFQDN(), connectionInfo.GetDestinationServerName(), connectionInfo.ToString());
			bool flag;
			ExWatson.SendThrottledGenericWatsonReport("E12", this.AppVersion, this.AppName, assemblyName.Version.ToString(), assemblyName.Name, "IllegalCrossServerConnectionEx4: " + normalizedClientInfo, stackTrace.ToString(), hashCode.ToString("x"), suitableMethodNameFromStackTrace, detailedExceptionInformation, CrossServerConnectionRegistryParameters.GetInfoWatsonThrottlingInterval(), out flag);
		}

		public void TraceCrossServerCall(string serverDn)
		{
			if (ComponentTrace<MapiNetTags>.IsTraceEnabled(80))
			{
				ComponentTrace<MapiNetTags>.Trace<string, string, string>(36048, 80, 0L, "CrossServerDiagnostics::TraceCrossServerCall. Cross-server call found. serverDn = {0}; machineName = {1}; Call stack:\n{2}", serverDn, Environment.MachineName, Environment.StackTrace);
			}
		}

		public void BlockCrossServerCall(ExRpcConnectionInfo connectionInfo, string mailboxDescription)
		{
			if (CrossServerConnectionRegistryParameters.IsCrossServerBlockEnabled())
			{
				string message;
				if (!string.IsNullOrEmpty(mailboxDescription))
				{
					message = string.Format("{0} mailbox [{1}] with application ID [{2}] is not allowed to make cross-server calls from [{3}] to [{4}]", new object[]
					{
						mailboxDescription,
						connectionInfo.UserName,
						connectionInfo.ApplicationId.GetNormalizedClientInfo(),
						ExRpcConnectionInfo.GetLocalServerFQDN(),
						connectionInfo.ServerDn
					});
				}
				else
				{
					message = string.Format("Mailbox [{0}] with application ID [{1}] is not allowed to make cross-server calls from [{2}] to [{3}]", new object[]
					{
						connectionInfo.UserName,
						connectionInfo.ApplicationId.GetNormalizedClientInfo(),
						ExRpcConnectionInfo.GetLocalServerFQDN(),
						connectionInfo.ServerDn
					});
				}
				throw MapiExceptionHelper.IllegalCrossServerConnection(message);
			}
		}

		public void BlockMonitoringCrossServerCall(ExRpcConnectionInfo connectionInfo)
		{
			if (CrossServerConnectionRegistryParameters.IsCrossServerMonitoringBlockEnabled())
			{
				this.BlockCrossServerCall(connectionInfo, "Monitoring");
			}
		}

		public void BlockCrossServerCall(ExRpcConnectionInfo connectionInfo)
		{
			if (CrossServerConnectionRegistryParameters.IsCrossServerBlockEnabled())
			{
				this.BlockCrossServerCall(connectionInfo, null);
			}
		}

		private static ICrossServerDiagnostics instance = new CrossServerDiagnostics();

		private string appName;

		private string appVersion;
	}
}
