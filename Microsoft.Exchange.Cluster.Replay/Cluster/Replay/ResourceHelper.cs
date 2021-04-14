using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.Cluster;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class ResourceHelper
	{
		public static void CheckCrossOSRemoteClusterCompatability(string[] servers)
		{
			if (servers == null || servers.Length == 0)
			{
				return;
			}
			ITopologyConfigurationSession topologyConfigurationSession = null;
			Version localOSVersion = null;
			foreach (string text in servers)
			{
				if (!string.IsNullOrEmpty(text) && !Cluster.StringIEquals(text, Environment.MachineName))
				{
					try
					{
						if (topologyConfigurationSession == null)
						{
							topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 84, "CheckCrossOSRemoteClusterCompatability", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Replay\\Core\\ResourceHelper.cs");
							topologyConfigurationSession.UseConfigNC = false;
							topologyConfigurationSession.UseGlobalCatalog = false;
							OperatingSystem osversion = Environment.OSVersion;
							localOSVersion = osversion.Version;
						}
						ResourceHelper.CheckOSVersionUsingAD(text, localOSVersion, topologyConfigurationSession);
					}
					catch (DataValidationException ex)
					{
						ExTraceGlobals.ClusterTracer.TraceError<string>(0L, "CheckCrossOSRemoteClusterCompatability(): Unexpected exception has occurred: {0}", ex.ToString());
					}
					catch (DataSourceOperationException ex2)
					{
						ExTraceGlobals.ClusterTracer.TraceError<string>(0L, "CheckCrossOSRemoteClusterCompatability(): Unexpected exception has occurred: {0}", ex2.ToString());
					}
					catch (DataSourceTransientException ex3)
					{
						ExTraceGlobals.ClusterTracer.TraceError<string>(0L, "CheckCrossOSRemoteClusterCompatability(): Unexpected exception has occurred: {0}", ex3.ToString());
					}
				}
			}
		}

		private static void CheckOSVersionUsingAD(string server, Version localOSVersion, ITopologyConfigurationSession session)
		{
			if (session == null)
			{
				throw new ArgumentException("session should not be null.", "session");
			}
			ADComputer adcomputer = session.FindComputerByHostName(server);
			if (adcomputer != null)
			{
				string operatingSystemVersion = adcomputer.OperatingSystemVersion;
				if (string.IsNullOrEmpty(operatingSystemVersion))
				{
					ExTraceGlobals.ClusterTracer.TraceDebug<string>(0L, "CheckOSVersionUsingAD(): Could not read OperatingSystemVersion from AD for server '{0}'.", server);
					return;
				}
				ExTraceGlobals.ClusterTracer.TraceDebug<string, string>(0L, "CheckOSVersionUsingAD(): Server '{0}' has OperatingSystemVersion '{1}' from AD.", server, operatingSystemVersion);
				string[] array = operatingSystemVersion.Split(new char[]
				{
					'.',
					' ',
					'(',
					')'
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array == null || array.Length != 3)
				{
					ExTraceGlobals.ClusterTracer.TraceDebug<string>(0L, "CheckOSVersionUsingAD(): Found unexpected OS version format from AD for server '{0}'.", server);
					return;
				}
				Version version = new Version(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2]));
				if (version.Major != localOSVersion.Major)
				{
					if (version.Major == 6 && localOSVersion.Major == 5)
					{
						throw new RemoteClusterWin2k3ToWin2k8NotSupportedException();
					}
					if (version.Major == 5 && localOSVersion.Major == 6)
					{
						throw new RemoteClusterWin2k8ToWin2k3NotSupportedException();
					}
				}
			}
			else
			{
				ExTraceGlobals.ClusterTracer.TraceDebug<string>(0L, "CheckOSVersionUsingAD(): Could not find ADComputer object for server '{0}'.", server);
			}
		}

		private const int StaticGetHashCode = 0;
	}
}
