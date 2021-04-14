using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ServiceModel.Channels;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;
using Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal static class ActivityContext
	{
		static ActivityContext()
		{
			ActivityContext.RegisterMetadata(typeof(ActivityStandardMetadata));
			ActivityContext.ConfigureGlobalActivityFromRegistry();
		}

		public static event EventHandler<ActivityEventArgs> OnActivityEvent;

		public static Guid? ActivityId
		{
			get
			{
				Guid? result = null;
				IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
				if (currentActivityScope != null && currentActivityScope.ActivityId != Guid.Empty)
				{
					result = new Guid?(currentActivityScope.ActivityId);
				}
				return result;
			}
		}

		public static bool IsStarted
		{
			get
			{
				bool flag = false;
				ActivityScopeImpl activityScopeImpl = null;
				Guid? localId = SingleContext.Singleton.LocalId;
				if (localId != null)
				{
					activityScopeImpl = ActivityScopeImpl.GetScopeImpl(localId.Value);
					if (activityScopeImpl != null)
					{
						flag = (activityScopeImpl.Status == ActivityContextStatus.ActivityStarted);
					}
				}
				ExTraceGlobals.ActivityContextTracer.TraceDebug((long)((localId != null) ? localId.Value.GetHashCode() : 0), "IsStarted = {0}, found ActivityScopeImpl object for Activity {1}, LocalId {2}, activityScopeImpl.Status = {3}", new object[]
				{
					flag,
					(activityScopeImpl != null) ? activityScopeImpl.ActivityId : Guid.Empty,
					(localId != null) ? localId.Value : Guid.Empty,
					(activityScopeImpl != null) ? activityScopeImpl.Status : ((ActivityContextStatus)(-1))
				});
				return flag;
			}
		}

		internal static bool DisableFriendlyWatsonForTesting { get; set; } = false;

		private static bool IsGlobalScopeEnabled { get; set; }

		public static int? InitialMetadataCapacity { get; set; }

		public static void RegisterMetadata(Type customMetadataType)
		{
			ExTraceGlobals.ActivityContextTracer.TraceDebug<string>(0L, "ActivityContext.RegisterMetadata - adding to the list of supported ActivityContext keys, type {0}.", customMetadataType.Name);
			if (!customMetadataType.IsSubclassOf(typeof(Enum)))
			{
				try
				{
					throw new ArgumentException(DiagnosticsResources.ExceptionActivityContextEnumMetadataOnly, "customMetadataType");
				}
				catch (ArgumentException)
				{
				}
				return;
			}
			Array values = Enum.GetValues(customMetadataType);
			ListDictionary listDictionary = new ListDictionary();
			lock (ActivityContext.registeredEnumTypes)
			{
				if (!ActivityContext.registeredEnumTypes.Contains(customMetadataType) && !(customMetadataType == typeof(ActivityReadonlyMetadata)))
				{
					for (int i = 0; i < values.Length; i++)
					{
						Enum @enum = (Enum)values.GetValue(i);
						string text = DisplayNameAttribute.GetEnumName(@enum);
						if (ActivityContext.stringToEnum.ContainsKey(text))
						{
							text = @enum.GetType().FullName + "." + @enum.ToString();
							if (ActivityContext.stringToEnum.ContainsKey(text))
							{
								try
								{
									throw new ActivityContextException(DiagnosticsResources.ExceptionActivityContextKeyCollision);
								}
								catch (ActivityContextException)
								{
								}
								return;
							}
						}
						listDictionary.Add(@enum, text);
					}
					Dictionary<Enum, Enum> dictionary = new Dictionary<Enum, Enum>(ActivityContext.preBoxedEnumValues);
					Dictionary<Enum, string> dictionary2 = new Dictionary<Enum, string>(ActivityContext.enumToString);
					Dictionary<string, Enum> dictionary3 = new Dictionary<string, Enum>(ActivityContext.stringToEnum);
					foreach (object obj2 in listDictionary)
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj2;
						ExTraceGlobals.ActivityContextTracer.TraceDebug(0L, "ActivityContext.RegisterMetadata - ({0}, '{1}').", new object[]
						{
							dictionaryEntry.Key,
							dictionaryEntry.Value
						});
						Enum enum2 = (Enum)dictionaryEntry.Key;
						dictionary2[enum2] = (string)dictionaryEntry.Value;
						dictionary3[(string)dictionaryEntry.Value] = enum2;
						dictionary[enum2] = enum2;
					}
					ActivityContext.preBoxedEnumValues = dictionary;
					ActivityContext.stringToEnum = dictionary3;
					ActivityContext.enumToString = dictionary2;
					ActivityContext.registeredEnumTypes.Add(customMetadataType);
				}
			}
		}

		public static ActivityScope Start(object userState, ActivityType activityType)
		{
			return ActivityContext.AddActivityScope(null, userState, activityType, ActivityContext.OnStartEventArgs, DiagnosticsResources.ExceptionStartInvokedTwice(DebugContext.GetDebugInfo()));
		}

		public static ActivityScope Start(object userState = null)
		{
			return ActivityContext.Start(userState, ActivityType.Request);
		}

		public static ActivityScope Resume(ActivityContextState activityContextState, object userState = null)
		{
			if (activityContextState == null)
			{
				try
				{
					throw new ArgumentNullException("activityContextState was null");
				}
				catch (ArgumentException)
				{
				}
			}
			return ActivityContext.AddActivityScope(activityContextState, userState, (activityContextState != null) ? activityContextState.ActivityType : ActivityType.Request, ActivityContext.OnResumeEventArgs, DiagnosticsResources.ExceptionScopeAlreadyExists(DebugContext.GetDebugInfo()));
		}

		public static IActivityScope GetCurrentActivityScope()
		{
			Guid? localId = SingleContext.Singleton.LocalId;
			if (localId != null)
			{
				return ActivityScopeImpl.GetActivityScope(localId.Value);
			}
			return null;
		}

		public static bool TryGetPreboxedEnum(Enum enumValue, out Enum result)
		{
			return ActivityContext.preBoxedEnumValues.TryGetValue(enumValue, out result);
		}

		public static void AddOperation(ActivityOperationType operation, string instance, float value = 0f, int count = 1)
		{
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			ActivityContext.AddOperation(currentActivityScope, operation, instance, value, count);
		}

		public static void AddOperation(IActivityScope scope, ActivityOperationType operation, string instance, float value = 0f, int count = 1)
		{
			bool flag = false;
			IActivityScope activityScope = ActivityContext.globalScope;
			TimeInResourcePerfCounter.AddOperation(operation, value);
			if (scope != null && scope.Status == ActivityContextStatus.ActivityStarted)
			{
				flag = scope.AddOperation(operation, instance, value, count);
			}
			if (!ActivityContext.IsGlobalScopeEnabled)
			{
				return;
			}
			if (flag)
			{
				activityScope.AddOperation(operation, "INSTR", value, count);
				return;
			}
			if (scope != null || SingleContext.Singleton.LocalId != null)
			{
				activityScope.AddOperation(operation, "MISSED", value, count);
				return;
			}
			if (DebugContext.GetDebugProperty(DebugProperties.ActivityId) == null)
			{
				activityScope.AddOperation(operation, "UNINSTR", value, count);
				return;
			}
			activityScope.AddOperation(operation, "SUPPR", value, count);
		}

		public static void SetThreadScope(IActivityScope activityScope)
		{
			ActivityContext.ClearThreadScope();
			if (activityScope != null && activityScope.Status == ActivityContextStatus.ActivityStarted)
			{
				SingleContext.Singleton.LocalId = new Guid?(activityScope.LocalId);
				DebugContext.UpdateFrom(activityScope);
			}
		}

		public static void ClearThreadScope()
		{
			Guid? localId = SingleContext.Singleton.LocalId;
			ExTraceGlobals.ActivityContextTracer.TraceDebug<Guid?>((long)((localId != null) ? localId.GetHashCode() : 0), "ActivityContext.ClearThreadScope - localId: {0}", (localId != null) ? localId : new Guid?(Guid.Empty));
			SingleContext.Singleton.Clear();
		}

		public static NullScope SuppressThreadScope()
		{
			return new NullScope();
		}

		internal static ActivityScope DeserializeFrom(HttpRequestMessageProperty wcfMessage, object userState = null)
		{
			ActivityContextState activityContextState = ActivityContextState.DeserializeFrom(wcfMessage);
			return ActivityContext.AddActivityScope(activityContextState, userState, (activityContextState != null) ? activityContextState.ActivityType : ActivityType.Request, ActivityContext.OnStartEventArgs, DiagnosticsResources.ExceptionScopeAlreadyExists(DebugContext.GetDebugInfo()));
		}

		internal static ActivityScope DeserializeFrom(HttpRequest httpRequest, object userState = null)
		{
			ActivityContextState activityContextState = ActivityContextState.DeserializeFrom(httpRequest);
			return ActivityContext.AddActivityScope(activityContextState, userState, (activityContextState != null) ? activityContextState.ActivityType : ActivityType.Request, ActivityContext.OnStartEventArgs, DiagnosticsResources.ExceptionScopeAlreadyExists(DebugContext.GetDebugInfo()));
		}

		internal static void RaiseEvent(IActivityScope activityScope, ActivityEventArgs args)
		{
			EventHandler<ActivityEventArgs> onActivityEvent = ActivityContext.OnActivityEvent;
			if (onActivityEvent != null && activityScope != null)
			{
				Guid activityId = activityScope.ActivityId;
				ExTraceGlobals.ActivityContextTracer.TraceDebug<ActivityEventType, Guid, int>(0L, "ActivityContext.RaiseEvent - raising event {0} for ActivityId {1}, and callback {2}.", args.ActivityEventType, activityId, onActivityEvent.GetHashCode());
				onActivityEvent(activityScope, args);
			}
		}

		internal static string LookupEnumName(Enum value)
		{
			string result;
			if (ActivityContext.enumToString.TryGetValue(value, out result))
			{
				return result;
			}
			return null;
		}

		internal static Enum LookupEnum(string enumName)
		{
			Enum result;
			if (ActivityContext.stringToEnum.TryGetValue(enumName, out result))
			{
				return result;
			}
			return null;
		}

		[Conditional("DEBUG")]
		internal static void FriendlyWatson(Exception exception)
		{
			if (Debugger.IsAttached)
			{
				Debugger.Break();
			}
			ActivityContext.RaiseEvent(ActivityContext.GetCurrentActivityScope(), new ActivityEventArgs(ActivityEventType.WatsonActivity, exception.Message));
			if (!ActivityContext.DisableFriendlyWatsonForTesting)
			{
				ExWatson.SendReport(exception, ReportOptions.DoNotCollectDumps | ReportOptions.DeepStackTraceHash, null);
			}
		}

		internal static void TestHook_UpdateTimer(int globalActivityLifetime, int rollupActivityCycleCount)
		{
			lock (ActivityContext.timerLock)
			{
				ActivityContext.ConfigureGlobalActivity(globalActivityLifetime, rollupActivityCycleCount);
			}
		}

		internal static void TestHook_ResetTimer()
		{
			lock (ActivityContext.timerLock)
			{
				ActivityContext.ConfigureGlobalActivityFromRegistry();
			}
		}

		internal static void TestHook_BlockRollover(ref bool lockTaken)
		{
			Monitor.Enter(ActivityContext.timerLock, ref lockTaken);
		}

		internal static void TestHook_AllowRollover(ref bool lockTaken)
		{
			if (lockTaken)
			{
				Monitor.Exit(ActivityContext.timerLock);
				lockTaken = false;
			}
		}

		internal static T ReadDiagnosticsSettingFromRegistry<T>(string keyName, T defaultValue)
		{
			T result = defaultValue;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics"))
				{
					if (registryKey != null)
					{
						result = (T)((object)registryKey.GetValue(keyName, defaultValue));
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.ActivityContextTracer.TraceDebug<string, string, string>(0L, "Exception '{0}' while reading registry key '{1}', value '{2}' most likely key has incorrect value or value type.", ex.ToString(), "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics", keyName);
			}
			return result;
		}

		private static ActivityScope AddActivityScope(ActivityContextState activityContextState, object userState, ActivityType activityType, ActivityEventArgs eventArgs, LocalizedString exceptionString)
		{
			bool flag = false;
			bool flag2 = false;
			ActivityScope activityScope = null;
			if (ActivityContext.IsStarted)
			{
				try
				{
					throw new ActivityContextException(exceptionString);
				}
				catch (ActivityContextException)
				{
					flag2 = true;
				}
				if (SingleContext.Singleton.CheckId())
				{
					ActivityScopeImpl activityScopeImpl = (ActivityScopeImpl)ActivityContext.GetCurrentActivityScope();
					if (activityScopeImpl != null)
					{
						return new ActivityScope(activityScopeImpl);
					}
				}
			}
			if (SingleContext.Singleton.LocalId != null && !flag2)
			{
				try
				{
					throw new ActivityContextException(DiagnosticsResources.ExceptionActivityContextMustBeCleared(DebugContext.GetDebugInfo()));
				}
				catch (ActivityContextException)
				{
				}
			}
			ActivityScope result;
			try
			{
				activityScope = ActivityScopeImpl.AddActivityScope(activityContextState);
				activityScope.UserState = userState;
				activityScope.ActivityType = activityType;
				ActivityContext.RaiseEvent(activityScope.ActivityScopeImpl, eventArgs);
				ExTraceGlobals.ActivityContextTracer.TraceDebug<Guid, bool>((long)activityScope.LocalId.GetHashCode(), "ActivityContext.AddActivityScope - ActivityId {0}, (activityContextState != null) = {1}", activityScope.ActivityId, activityContextState != null);
				flag = true;
				result = activityScope;
			}
			finally
			{
				if (!flag)
				{
					if (activityScope != null)
					{
						activityScope.Dispose();
					}
					ActivityContext.ClearThreadScope();
				}
			}
			return result;
		}

		private static void OnGlobalActivityTimer(object state)
		{
			bool flag = false;
			try
			{
				Monitor.TryEnter(ActivityContext.timerLock, ref flag);
				if (flag)
				{
					ActivityContext.LogGlobalInactive();
					ActivityContext.RolloverGlobalScope();
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(ActivityContext.timerLock);
				}
			}
		}

		private static void LogGlobalInactive()
		{
			try
			{
				ActivityScope activityScope = ActivityContext.inactiveGlobalScope;
				if (activityScope != null && activityScope.Status == ActivityContextStatus.ActivityStarted)
				{
					ActivityContext.SetThreadScope(activityScope);
					ActivityCoverageReport.OnGlobalActivityEnded(activityScope);
					activityScope.ActivityScopeImpl.RemoveInstrInstances();
					activityScope.End();
					ActivityContext.inactiveGlobalScope = null;
				}
			}
			finally
			{
				ActivityContext.ClearThreadScope();
			}
		}

		private static void RolloverGlobalScope()
		{
			try
			{
				ActivityScope activityScope = ActivityContext.Start(null, ActivityType.Global);
				activityScope.Action = "GlobalActivity";
				ActivityContext.inactiveGlobalScope = ActivityContext.globalScope;
				ActivityContext.globalScope = activityScope;
			}
			finally
			{
				ActivityContext.ClearThreadScope();
			}
		}

		private static void ConfigureGlobalActivityFromRegistry()
		{
			int num = ActivityContext.ReadDiagnosticsSettingFromRegistry<int>("GlobalActivityLifetimeMS", 300000);
			int num2 = ActivityContext.ReadDiagnosticsSettingFromRegistry<int>("RollupActivityCycleCount", 72);
			if (num != -1 && num < 60000)
			{
				num = 60000;
			}
			if (num2 != -1 && num2 <= 0)
			{
				num2 = 72;
			}
			ActivityContext.ConfigureGlobalActivity(num, num2);
		}

		private static void ConfigureGlobalActivity(int globalActivityLifetimeMS, int rollupActivityCycleCount)
		{
			Timer timer = ActivityContext.globalScopeTimer;
			ActivityContext.globalActivityLifetime = globalActivityLifetimeMS;
			ActivityContext.rollupActivityCycle = rollupActivityCycleCount;
			ActivityContext.IsGlobalScopeEnabled = (globalActivityLifetimeMS != -1);
			if (ActivityContext.IsGlobalScopeEnabled)
			{
				ActivityContext.OnGlobalActivityTimer(null);
				ActivityContext.globalScopeTimer = new Timer(new TimerCallback(ActivityContext.OnGlobalActivityTimer), null, ActivityContext.globalActivityLifetime, ActivityContext.globalActivityLifetime);
			}
			ActivityCoverageReport.Configure(globalActivityLifetimeMS, rollupActivityCycleCount);
			if (timer != null)
			{
				timer.Dispose();
			}
		}

		internal const string LocalIdPropertyName = "MSExchangeLocalId";

		internal const string ContextIdPropertyName = "SingleContextIdKey";

		internal const string GlobalUninstrInstance = "UNINSTR";

		internal const string GlobalMissedInstance = "MISSED";

		internal const string GlobalSupprInstance = "SUPPR";

		internal const string ActiveSyncComponentName = "ActiveSync";

		internal const string GlobalInstrInstance = "INSTR";

		internal const string GlobalActivity = "GlobalActivity";

		private const int NullMagicNumber = -1;

		private const string DiagnosticsRegKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics";

		private const string GlobalActivityLifetimeRegValue = "GlobalActivityLifetimeMS";

		private const string RollupActivityCycleRegValue = "RollupActivityCycleCount";

		private const int DefaultGlobalActivityLifetime = 300000;

		private const int DefaultRollupActivityCycle = 72;

		internal static readonly ActivityEventArgs OnSuspendEventArgs = new ActivityEventArgs(ActivityEventType.SuspendActivity, null);

		internal static readonly ActivityEventArgs OnEndEventArgs = new ActivityEventArgs(ActivityEventType.EndActivity, null);

		private static readonly ActivityEventArgs OnStartEventArgs = new ActivityEventArgs(ActivityEventType.StartActivity, null);

		private static readonly ActivityEventArgs OnResumeEventArgs = new ActivityEventArgs(ActivityEventType.ResumeActivity, null);

		private static int globalActivityLifetime = 300000;

		private static int rollupActivityCycle = 72;

		private static Dictionary<Enum, string> enumToString = new Dictionary<Enum, string>();

		private static Dictionary<string, Enum> stringToEnum = new Dictionary<string, Enum>();

		private static Dictionary<Enum, Enum> preBoxedEnumValues = new Dictionary<Enum, Enum>();

		private static HashSet<Type> registeredEnumTypes = new HashSet<Type>();

		private static ActivityScope globalScope = null;

		private static ActivityScope inactiveGlobalScope = null;

		private static object timerLock = new object();

		private static Timer globalScopeTimer = null;
	}
}
