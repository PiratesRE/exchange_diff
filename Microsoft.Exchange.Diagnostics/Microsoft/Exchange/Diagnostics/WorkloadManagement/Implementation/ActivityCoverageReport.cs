using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation
{
	internal sealed class ActivityCoverageReport
	{
		public static bool IsReportEnabled
		{
			get
			{
				return ActivityCoverageReport.rollupActivityCycleCount > 0;
			}
		}

		public static ExEventLog EventLogger
		{
			get
			{
				return ActivityCoverageReport.eventLogger.Value;
			}
		}

		public static string CachedAppName
		{
			get
			{
				if (ActivityCoverageReport.cachedAppName == null)
				{
					ActivityCoverageReport.cachedAppName = ActivityCoverageReport.GetAppName();
				}
				return ActivityCoverageReport.cachedAppName;
			}
		}

		public static void Configure(int globalActivityLifetimeMS, int rollupActivityCycleCount)
		{
			ActivityCoverageReport.rollupActivityCycleCount = rollupActivityCycleCount;
			ActivityCoverageReport.rollupTime = new TimeSpan(0, 0, 0, 0, rollupActivityCycleCount * globalActivityLifetimeMS);
			ActivityCoverageReport.Clear();
		}

		public static void OnGlobalActivityEnded(ActivityScope globalActivityScope)
		{
			if (!ActivityCoverageReport.IsReportEnabled || globalActivityScope.ActivityType != ActivityType.Global)
			{
				return;
			}
			foreach (KeyValuePair<OperationKey, OperationStatistics> keyValuePair in globalActivityScope.Statistics)
			{
				OperationStatistics operationStatistics = null;
				if (ActivityCoverageReport.statistics.TryGetValue(keyValuePair.Key, out operationStatistics))
				{
					operationStatistics.Merge(keyValuePair.Value);
				}
				else
				{
					ActivityCoverageReport.statistics[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			ActivityCoverageReport.currentActivityCycleCount++;
			if (ActivityCoverageReport.currentActivityCycleCount == ActivityCoverageReport.rollupActivityCycleCount)
			{
				ActivityCoverageReport.LogReport();
				ActivityCoverageReport.Clear();
			}
		}

		internal static string GetAppPoolName(Process w3wpProcess)
		{
			string result = null;
			try
			{
				if (w3wpProcess != null && w3wpProcess.ProcessName.Contains("w3wp"))
				{
					int id = w3wpProcess.Id;
					string assemblyFile = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\inetsrv\\Microsoft.Web.Administration.dll");
					Assembly assembly = Assembly.LoadFrom(assemblyFile);
					Type type = assembly.GetType("Microsoft.Web.Administration.ServerManager");
					ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
					object arg = constructor.Invoke(null);
					if (ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site1 == null)
					{
						ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site1 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "WorkerProcesses", typeof(ActivityCoverageReport), new CSharpArgumentInfo[]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
						}));
					}
					object arg2 = ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site1.Target(ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site1, arg);
					if (ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site2 == null)
					{
						ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site2 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof(IEnumerable), typeof(ActivityCoverageReport)));
					}
					foreach (object arg3 in ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site2.Target(ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site2, arg2))
					{
						if (ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site3 == null)
						{
							ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site3 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(ActivityCoverageReport), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						Func<CallSite, object, bool> target = ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site3.Target;
						CallSite <>p__Site = ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site3;
						if (ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site4 == null)
						{
							ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site4 = CallSite<Func<CallSite, int, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof(ActivityCoverageReport), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						Func<CallSite, int, object, object> target2 = ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site4.Target;
						CallSite <>p__Site2 = ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site4;
						int arg4 = id;
						if (ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site5 == null)
						{
							ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site5 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "ProcessId", typeof(ActivityCoverageReport), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						if (target(<>p__Site, target2(<>p__Site2, arg4, ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site5.Target(ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site5, arg3))))
						{
							if (ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site6 == null)
							{
								ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site6 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof(string), typeof(ActivityCoverageReport)));
							}
							Func<CallSite, object, string> target3 = ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site6.Target;
							CallSite <>p__Site3 = ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site6;
							if (ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site7 == null)
							{
								ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site7 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "AppPoolName", typeof(ActivityCoverageReport), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							result = target3(<>p__Site3, ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site7.Target(ActivityCoverageReport.<GetAppPoolName>o__SiteContainer0.<>p__Site7, arg3));
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.ActivityContextTracer.TraceDebug<string>(0L, "Unexpected exception in GetAppPoolName: '{0}'.", ex.ToString());
			}
			return result;
		}

		internal static IEnumerable<KeyValuePair<OperationKey, OperationStatistics>> TestHook_GetStatistics()
		{
			foreach (KeyValuePair<OperationKey, OperationStatistics> pair in ActivityCoverageReport.statistics)
			{
				yield return pair;
			}
			yield break;
		}

		private static void Clear()
		{
			ActivityCoverageReport.currentActivityCycleCount = 0;
			ActivityCoverageReport.statistics.Clear();
		}

		private static void LogReport()
		{
			Process currentProcess = Process.GetCurrentProcess();
			string processName = currentProcess.ProcessName;
			if (ActivityCoverageReport.statistics.Count > 0)
			{
				ActivityCoverageReport.TryLogActivityRollupReportWithUsageEvent(new object[]
				{
					processName,
					ActivityCoverageReport.CachedAppName,
					ActivityCoverageReport.rollupTime,
					LogRowFormatter.FormatCollection(ActivityScopeImpl.GetFormattableStatistics(ActivityCoverageReport.statistics))
				});
				return;
			}
			ActivityCoverageReport.TryLogActivityRollupReportWithNoUsageEvent(new object[]
			{
				processName,
				ActivityCoverageReport.CachedAppName,
				ActivityCoverageReport.rollupTime
			});
		}

		private static string GetAppName()
		{
			Process currentProcess = Process.GetCurrentProcess();
			string text = ActivityCoverageReport.GetAppPoolName(currentProcess);
			if (text == null)
			{
				text = AppDomain.CurrentDomain.FriendlyName;
			}
			if (text == null)
			{
				text = "Unknown";
			}
			return text;
		}

		private static void TryLogActivityRollupReportWithUsageEvent(params object[] parameters)
		{
			try
			{
				ActivityCoverageReport.EventLogger.LogEvent(CommonEventLogConstants.Tuple_ActivityRollupReportWithUsageEvent, null, parameters);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.ActivityContextTracer.TraceDebug<string>(0L, "Unexpected exception in TryLogActivityRollupReportWithUsageEvent: '{0}'.", ex.ToString());
			}
		}

		private static void TryLogActivityRollupReportWithNoUsageEvent(params object[] parameters)
		{
			try
			{
				ActivityCoverageReport.EventLogger.LogEvent(CommonEventLogConstants.Tuple_ActivityRollupReportWithNoUsageEvent, null, parameters);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.ActivityContextTracer.TraceDebug<string>(0L, "Unexpected exception in TryLogActivityRollupReportWithNoUsageEvent: '{0}'.", ex.ToString());
			}
		}

		private static Lazy<ExEventLog> eventLogger = new Lazy<ExEventLog>(() => new ExEventLog(new Guid("{51553C07-BBF8-43A9-8EAC-BD219B516B48}"), "MSExchange Common"));

		private static int rollupActivityCycleCount = 72;

		private static TimeSpan rollupTime = TimeSpan.Zero;

		private static int currentActivityCycleCount = 0;

		private static Dictionary<OperationKey, OperationStatistics> statistics = new Dictionary<OperationKey, OperationStatistics>();

		private static string cachedAppName = null;

		[CompilerGenerated]
		private static class <GetAppPoolName>o__SiteContainer0
		{
			public static CallSite<Func<CallSite, object, object>> <>p__Site1;

			public static CallSite<Func<CallSite, object, IEnumerable>> <>p__Site2;

			public static CallSite<Func<CallSite, object, bool>> <>p__Site3;

			public static CallSite<Func<CallSite, int, object, object>> <>p__Site4;

			public static CallSite<Func<CallSite, object, object>> <>p__Site5;

			public static CallSite<Func<CallSite, object, string>> <>p__Site6;

			public static CallSite<Func<CallSite, object, object>> <>p__Site7;
		}
	}
}
