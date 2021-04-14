using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Monitoring
{
	internal class FileShareQuorumCheck : DagMemberCheck
	{
		public FileShareQuorumCheck(string serverName, IEventManager eventManager, string momeventsource, uint ignoreTransientErrorsThreshold, IADDatabaseAvailabilityGroup dag) : base(serverName, "FileShareQuorum", CheckId.FileShareQuorum, Strings.FileShareQuorumCheckDesc, CheckCategory.SystemMediumPriority, eventManager, momeventsource, new uint?(ignoreTransientErrorsThreshold), dag, true)
		{
		}

		protected override void RunCheck()
		{
			bool flag = false;
			string text = null;
			IEnumerable<AmClusterResource> enumerable = base.Cluster.EnumerateResources();
			try
			{
				foreach (AmClusterResource amClusterResource in enumerable)
				{
					if (amClusterResource.GetTypeName() == "File Share Witness")
					{
						text = amClusterResource.GetPrivateProperty<string>("SharePath");
						if (string.IsNullOrEmpty(text))
						{
							base.Fail(Strings.FileShareWitnessPathNotSet(amClusterResource.Name));
						}
						flag = true;
						break;
					}
				}
			}
			finally
			{
				foreach (AmClusterResource amClusterResource2 in enumerable)
				{
					amClusterResource2.Dispose();
				}
			}
			if (flag)
			{
				text = FileShareQuorumCheck.s_regexTrailingBackslash.Replace(text, string.Empty);
				int ec;
				if (!this.IsPathReachable(text, out ec))
				{
					base.Fail(Strings.FileShareWitnessPathDown(text, Environment.MachineName, base.Cluster.Name, ec));
					return;
				}
			}
			else
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "InternalRun(): Cluster '{0}' does not have a File Share Witness. Skipping check {1}.", base.Cluster.Name, base.Title);
				base.Skip();
			}
		}

		private bool IsPathReachable(string directoryName, out int errorCode)
		{
			errorCode = 0;
			SafeFileHandle safeFileHandle = null;
			bool result;
			try
			{
				safeFileHandle = NativeMethods.CreateFile(directoryName, FileAccess.Read, FileShare.Read, IntPtr.Zero, FileMode.Open, (FileFlags)0U, IntPtr.Zero);
				errorCode = Marshal.GetLastWin32Error();
				if (!safeFileHandle.IsInvalid)
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "IsPathReachable(): Successfully able to access path '{0}'.", directoryName);
					result = true;
				}
				else if (errorCode == 5)
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "IsPathReachable(): Got ERROR_ACCESS_DENIED when trying to access path '{0}'.", directoryName);
					result = true;
				}
				else if (errorCode == 53)
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "IsPathReachable(): Got ERROR_BAD_NETPATH when trying to access path '{0}'.", directoryName);
					result = false;
				}
				else if ((long)errorCode == 67L)
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "IsPathReachable(): Got ERROR_BAD_NET_NAME when trying to access path '{0}'.", directoryName);
					result = false;
				}
				else if (errorCode == 3)
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "IsPathReachable(): Got ERROR_PATH_NOT_FOUND when trying to access path '{0}'.", directoryName);
					result = false;
				}
				else
				{
					ExTraceGlobals.HealthChecksTracer.TraceDebug<int, string>((long)this.GetHashCode(), "IsPathReachable{}: Got unexpected error code {0} when trying to access path '{1}'.", errorCode, directoryName);
					result = false;
				}
			}
			finally
			{
				if (safeFileHandle != null)
				{
					safeFileHandle.Dispose();
				}
			}
			return result;
		}

		private static Regex s_regexTrailingBackslash = new Regex("(?<=([^\\\\]+))\\\\$", RegexOptions.CultureInvariant);
	}
}
