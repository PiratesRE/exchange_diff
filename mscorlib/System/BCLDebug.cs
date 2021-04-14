using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace System
{
	internal static class BCLDebug
	{
		[Conditional("_DEBUG")]
		public static void Assert(bool condition, string message)
		{
		}

		[Conditional("_LOGGING")]
		[SecuritySafeCritical]
		public static void Log(string message)
		{
			if (AppDomain.CurrentDomain.IsUnloadingForcedFinalize())
			{
				return;
			}
			if (!BCLDebug.m_registryChecked)
			{
				BCLDebug.CheckRegistry();
			}
			System.Diagnostics.Log.Trace(message);
			System.Diagnostics.Log.Trace(Environment.NewLine);
		}

		[Conditional("_LOGGING")]
		[SecuritySafeCritical]
		public static void Log(string switchName, string message)
		{
			if (AppDomain.CurrentDomain.IsUnloadingForcedFinalize())
			{
				return;
			}
			if (!BCLDebug.m_registryChecked)
			{
				BCLDebug.CheckRegistry();
			}
			try
			{
				LogSwitch @switch = LogSwitch.GetSwitch(switchName);
				if (@switch != null)
				{
					System.Diagnostics.Log.Trace(@switch, message);
					System.Diagnostics.Log.Trace(@switch, Environment.NewLine);
				}
			}
			catch
			{
				System.Diagnostics.Log.Trace("Exception thrown in logging." + Environment.NewLine);
				System.Diagnostics.Log.Trace("Switch was: " + ((switchName == null) ? "<null>" : switchName) + Environment.NewLine);
				System.Diagnostics.Log.Trace("Message was: " + ((message == null) ? "<null>" : message) + Environment.NewLine);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetRegistryLoggingValues(out bool loggingEnabled, out bool logToConsole, out int logLevel, out bool perfWarnings, out bool correctnessWarnings, out bool safeHandleStackTraces);

		[SecuritySafeCritical]
		private static void CheckRegistry()
		{
			if (AppDomain.CurrentDomain.IsUnloadingForcedFinalize())
			{
				return;
			}
			if (BCLDebug.m_registryChecked)
			{
				return;
			}
			BCLDebug.m_registryChecked = true;
			bool flag;
			bool isConsoleEnabled;
			int num;
			int registryLoggingValues = BCLDebug.GetRegistryLoggingValues(out flag, out isConsoleEnabled, out num, out BCLDebug.m_perfWarnings, out BCLDebug.m_correctnessWarnings, out BCLDebug.m_safeHandleStackTraces);
			if (!flag)
			{
				BCLDebug.m_loggingNotEnabled = true;
			}
			if (flag && BCLDebug.levelConversions != null)
			{
				try
				{
					num = (int)BCLDebug.levelConversions[num];
					if (registryLoggingValues > 0)
					{
						for (int i = 0; i < BCLDebug.switches.Length; i++)
						{
							if ((BCLDebug.switches[i].value & registryLoggingValues) != 0)
							{
								LogSwitch logSwitch = new LogSwitch(BCLDebug.switches[i].name, BCLDebug.switches[i].name, System.Diagnostics.Log.GlobalSwitch);
								logSwitch.MinimumLevel = (LoggingLevels)num;
							}
						}
						System.Diagnostics.Log.GlobalSwitch.MinimumLevel = (LoggingLevels)num;
						System.Diagnostics.Log.IsConsoleEnabled = isConsoleEnabled;
					}
				}
				catch
				{
				}
			}
		}

		[SecuritySafeCritical]
		internal static bool CheckEnabled(string switchName)
		{
			if (AppDomain.CurrentDomain.IsUnloadingForcedFinalize())
			{
				return false;
			}
			if (!BCLDebug.m_registryChecked)
			{
				BCLDebug.CheckRegistry();
			}
			LogSwitch @switch = LogSwitch.GetSwitch(switchName);
			return @switch != null && @switch.MinimumLevel <= LoggingLevels.TraceLevel0;
		}

		[SecuritySafeCritical]
		private static bool CheckEnabled(string switchName, LogLevel level, out LogSwitch logSwitch)
		{
			if (AppDomain.CurrentDomain.IsUnloadingForcedFinalize())
			{
				logSwitch = null;
				return false;
			}
			logSwitch = LogSwitch.GetSwitch(switchName);
			return logSwitch != null && logSwitch.MinimumLevel <= (LoggingLevels)level;
		}

		[Conditional("_LOGGING")]
		[SecuritySafeCritical]
		public static void Log(string switchName, LogLevel level, params object[] messages)
		{
			if (AppDomain.CurrentDomain.IsUnloadingForcedFinalize())
			{
				return;
			}
			if (!BCLDebug.m_registryChecked)
			{
				BCLDebug.CheckRegistry();
			}
			LogSwitch logswitch;
			if (!BCLDebug.CheckEnabled(switchName, level, out logswitch))
			{
				return;
			}
			StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
			for (int i = 0; i < messages.Length; i++)
			{
				string value;
				try
				{
					if (messages[i] == null)
					{
						value = "<null>";
					}
					else
					{
						value = messages[i].ToString();
					}
				}
				catch
				{
					value = "<unable to convert>";
				}
				stringBuilder.Append(value);
			}
			System.Diagnostics.Log.LogMessage((LoggingLevels)level, logswitch, StringBuilderCache.GetStringAndRelease(stringBuilder));
		}

		[Conditional("_LOGGING")]
		public static void Trace(string switchName, params object[] messages)
		{
			if (BCLDebug.m_loggingNotEnabled)
			{
				return;
			}
			LogSwitch logswitch;
			if (!BCLDebug.CheckEnabled(switchName, LogLevel.Trace, out logswitch))
			{
				return;
			}
			StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
			for (int i = 0; i < messages.Length; i++)
			{
				string value;
				try
				{
					if (messages[i] == null)
					{
						value = "<null>";
					}
					else
					{
						value = messages[i].ToString();
					}
				}
				catch
				{
					value = "<unable to convert>";
				}
				stringBuilder.Append(value);
			}
			stringBuilder.Append(Environment.NewLine);
			System.Diagnostics.Log.LogMessage(LoggingLevels.TraceLevel0, logswitch, StringBuilderCache.GetStringAndRelease(stringBuilder));
		}

		[Conditional("_LOGGING")]
		public static void Trace(string switchName, string format, params object[] messages)
		{
			if (BCLDebug.m_loggingNotEnabled)
			{
				return;
			}
			LogSwitch logswitch;
			if (!BCLDebug.CheckEnabled(switchName, LogLevel.Trace, out logswitch))
			{
				return;
			}
			StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
			stringBuilder.AppendFormat(format, messages);
			stringBuilder.Append(Environment.NewLine);
			System.Diagnostics.Log.LogMessage(LoggingLevels.TraceLevel0, logswitch, StringBuilderCache.GetStringAndRelease(stringBuilder));
		}

		[Conditional("_LOGGING")]
		public static void DumpStack(string switchName)
		{
			if (!BCLDebug.m_registryChecked)
			{
				BCLDebug.CheckRegistry();
			}
			LogSwitch logswitch;
			if (!BCLDebug.CheckEnabled(switchName, LogLevel.Trace, out logswitch))
			{
				return;
			}
			StackTrace stackTrace = new StackTrace();
			System.Diagnostics.Log.LogMessage(LoggingLevels.TraceLevel0, logswitch, stackTrace.ToString());
		}

		[SecuritySafeCritical]
		[Conditional("_DEBUG")]
		internal static void ConsoleError(string msg)
		{
			if (AppDomain.CurrentDomain.IsUnloadingForcedFinalize())
			{
				return;
			}
			if (BCLDebug.m_MakeConsoleErrorLoggingWork == null)
			{
				PermissionSet permissionSet = new PermissionSet();
				permissionSet.AddPermission(new EnvironmentPermission(PermissionState.Unrestricted));
				permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, Path.GetFullPath(".")));
				BCLDebug.m_MakeConsoleErrorLoggingWork = permissionSet;
			}
			BCLDebug.m_MakeConsoleErrorLoggingWork.Assert();
			using (TextWriter textWriter = File.AppendText("ConsoleErrors.log"))
			{
				textWriter.WriteLine(msg);
			}
		}

		[Conditional("_DEBUG")]
		[SecuritySafeCritical]
		internal static void Perf(bool expr, string msg)
		{
			if (AppDomain.CurrentDomain.IsUnloadingForcedFinalize())
			{
				return;
			}
			if (!BCLDebug.m_registryChecked)
			{
				BCLDebug.CheckRegistry();
			}
			if (!BCLDebug.m_perfWarnings)
			{
				return;
			}
			System.Diagnostics.Assert.Check(expr, "BCL Perf Warning: Your perf may be less than perfect because...", msg);
		}

		[Conditional("_DEBUG")]
		internal static void Correctness(bool expr, string msg)
		{
		}

		internal static bool CorrectnessEnabled()
		{
			return false;
		}

		internal static bool SafeHandleStackTracesEnabled
		{
			get
			{
				return false;
			}
		}

		internal static volatile bool m_registryChecked = false;

		internal static volatile bool m_loggingNotEnabled = false;

		internal static bool m_perfWarnings;

		internal static bool m_correctnessWarnings;

		internal static bool m_safeHandleStackTraces;

		internal static volatile PermissionSet m_MakeConsoleErrorLoggingWork;

		private static readonly SwitchStructure[] switches = new SwitchStructure[]
		{
			new SwitchStructure("NLS", 1),
			new SwitchStructure("SER", 2),
			new SwitchStructure("DYNIL", 4),
			new SwitchStructure("REMOTE", 8),
			new SwitchStructure("BINARY", 16),
			new SwitchStructure("SOAP", 32),
			new SwitchStructure("REMOTINGCHANNELS", 64),
			new SwitchStructure("CACHE", 128),
			new SwitchStructure("RESMGRFILEFORMAT", 256),
			new SwitchStructure("PERF", 512),
			new SwitchStructure("CORRECTNESS", 1024),
			new SwitchStructure("MEMORYFAILPOINT", 2048),
			new SwitchStructure("DATETIME", 4096),
			new SwitchStructure("INTEROP", 8192)
		};

		private static readonly LogLevel[] levelConversions = new LogLevel[]
		{
			LogLevel.Panic,
			LogLevel.Error,
			LogLevel.Error,
			LogLevel.Warning,
			LogLevel.Warning,
			LogLevel.Status,
			LogLevel.Status,
			LogLevel.Trace,
			LogLevel.Trace,
			LogLevel.Trace,
			LogLevel.Trace
		};
	}
}
