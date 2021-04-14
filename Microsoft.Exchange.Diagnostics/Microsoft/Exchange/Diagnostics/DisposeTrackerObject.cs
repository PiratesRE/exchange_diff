using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Internal;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DisposeTrackerObject<T> : DisposeTracker where T : IDisposable
	{
		public DisposeTrackerObject()
		{
			DisposeTrackerOptions.RefreshNowIfNecessary();
			bool flag = DisposeTrackerObject<T>.isTypeRisky && DisposeTracker.CanCollectAnotherStackTraceYet();
			if (flag)
			{
				if (Interlocked.Increment(ref DisposeTrackerObject<T>.skipCounter) <= DisposeTrackerOptions.NumberOfStackTracesToSkip)
				{
					flag = false;
				}
				else
				{
					DisposeTrackerObject<T>.skipCounter = 0;
				}
			}
			flag |= (Debugger.IsAttached && DisposeTrackerOptions.DebugBreakOnLeak);
			flag |= (DisposeTrackerOptions.CollectStackTracesForLeakDetection && DisposeTracker.OnLeakDetected != null);
			if (flag)
			{
				base.StackTrace = new StackTrace(2, DisposeTrackerOptions.UseFullSymbols);
				DisposeTracker.RecordStackTraceTimestamp();
				base.HasCollectedStackTrace = true;
				if (!DisposeTrackerOptions.DontStopCollecting && Interlocked.Increment(ref DisposeTrackerObject<T>.numCollectedCounter) >= DisposeTrackerOptions.NumberOfStackTracesToCollect)
				{
					DisposeTrackerObject<T>.isTypeRisky = false;
				}
			}
		}

		~DisposeTrackerObject()
		{
			this.Dispose(false);
		}

		public static void Reset()
		{
			DisposeTrackerObject<T>.isTypeRisky = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				base.StackTrace = null;
				return;
			}
			if (DisposeTracker.OnLeakDetected != null && !base.WasProperlyDisposed)
			{
				string arg;
				if (base.StackTrace != null)
				{
					arg = DisposeTrackerObject<T>.StackTraceToString(base.StackTrace);
				}
				else
				{
					arg = "No stack collected. Under HKLM\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\DisposeTrackerOptions set CollectStackTracesForLeakDetection (DWORD) to 1 and run this whole module again.";
				}
				DisposeTracker.OnLeakDetected(typeof(T).ToString(), arg);
				return;
			}
			try
			{
				if (DisposeTracker.Suppressed || this.IsShuttingDown())
				{
					return;
				}
			}
			catch
			{
				return;
			}
			if (base.StackTrace != null)
			{
				if (Debugger.IsAttached && DisposeTrackerOptions.DebugBreakOnLeak)
				{
					Debugger.Break();
					return;
				}
				if (DisposeTracker.CheckAndUpdateIfCanWatsonThisSecond())
				{
					Task.Factory.StartNew(DisposeTrackerObject<T>.watsonDelegate, new DisposeTrackerObject<T>.WatsonThreadStateInfo(base.StackTrace, base.StackTraceWasReset, base.ExtraDataList));
					return;
				}
			}
			else
			{
				if (DisposeTrackerOptions.Enabled)
				{
					DisposeTrackerObject<T>.isTypeRisky = true;
					DisposeTrackerObject<T>.numCollectedCounter = 0;
					return;
				}
				DisposeTrackerObject<T>.isTypeRisky = false;
			}
		}

		private static void AppendParamsToString(ParameterInfo[] parameters, StringBuilder stringBuilder)
		{
			for (int i = 0; i < parameters.Length - 1; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				stringBuilder.Append(parameterInfo.ParameterType.ToString());
				stringBuilder.Append(" ");
				stringBuilder.Append(parameterInfo.Name);
				stringBuilder.Append(", ");
			}
			if (parameters.Length > 0)
			{
				ParameterInfo parameterInfo = parameters[parameters.Length - 1];
				stringBuilder.Append(parameterInfo.ParameterType.ToString());
				stringBuilder.Append(" ");
				stringBuilder.Append(parameterInfo.Name);
			}
		}

		private static string StackTraceToString(StackTrace stackTrace)
		{
			DisposeTrackerObject<T>.<>c__DisplayClass8 CS$<>8__locals1 = new DisposeTrackerObject<T>.<>c__DisplayClass8();
			CS$<>8__locals1.stackTrace = stackTrace;
			int k = 0;
			CS$<>8__locals1.method = null;
			int num = Util.EvaluateOrDefault<int>(() => CS$<>8__locals1.stackTrace.FrameCount, 0);
			while (k < num)
			{
				try
				{
					CS$<>8__locals1.method = CS$<>8__locals1.stackTrace.GetFrame(k++).GetMethod();
					Type declaringType = CS$<>8__locals1.method.DeclaringType;
					Type type = typeof(T);
					if (declaringType == null)
					{
						CS$<>8__locals1.method = null;
					}
					else
					{
						if (declaringType.GetTypeInfo().ContainsGenericParameters)
						{
							if (!type.GetTypeInfo().ContainsGenericParameters)
							{
								CS$<>8__locals1.method = null;
								continue;
							}
							type = type.GetGenericTypeDefinition();
						}
						if (CS$<>8__locals1.method.IsConstructor && declaringType == type)
						{
							break;
						}
					}
				}
				catch
				{
					CS$<>8__locals1.method = null;
				}
			}
			if (k == num)
			{
				return null;
			}
			string text = Util.EvaluateOrDefault<string>(() => CS$<>8__locals1.method.Name, string.Empty);
			if (k > 0 && !text.Equals(".ctor", StringComparison.Ordinal) && !text.Equals(".cctor", StringComparison.Ordinal))
			{
				k--;
			}
			int j = k;
			Type type2 = null;
			while (j < CS$<>8__locals1.stackTrace.FrameCount)
			{
				CS$<>8__locals1.method = CS$<>8__locals1.stackTrace.GetFrame(j++).GetMethod();
				Type declaringType2 = CS$<>8__locals1.method.DeclaringType;
				if (declaringType2 != typeof(T))
				{
					type2 = declaringType2;
					break;
				}
			}
			if (type2 != null && !type2.GetTypeInfo().ContainsGenericParameters && DisposeTrackerObject<T>.IsDisposeTrackable(type2))
			{
				bool flag = false;
				Type type3 = typeof(DisposeTrackerObject<>).MakeGenericType(new Type[]
				{
					type2
				});
				if (type3 != null)
				{
					FieldInfo declaredField = type3.GetTypeInfo().GetDeclaredField("isTypeRisky");
					if (declaredField != null && declaredField.IsStatic && !declaredField.IsPublic)
					{
						flag = (bool)declaredField.GetValue(null);
					}
				}
				if (flag)
				{
					return null;
				}
			}
			StringBuilder stringBuilder = new StringBuilder((CS$<>8__locals1.stackTrace.FrameCount - k) * 80);
			int i;
			for (i = k; i < CS$<>8__locals1.stackTrace.FrameCount; i++)
			{
				StackFrame currFrame = null;
				MethodBase currMethod = null;
				string text2 = null;
				string text3 = null;
				int num2 = 0;
				currFrame = Util.EvaluateOrDefault<StackFrame>(() => CS$<>8__locals1.stackTrace.GetFrame(i), null);
				if (currFrame != null)
				{
					currMethod = Util.EvaluateOrDefault<MethodBase>(() => currFrame.GetMethod(), null);
					if (currMethod != null)
					{
						if (currMethod.DeclaringType != null)
						{
							text3 = Util.EvaluateOrDefault<string>(() => currMethod.DeclaringType.ToString(), null);
						}
						text = Util.EvaluateOrDefault<string>(() => currMethod.Name, null);
					}
					text2 = Util.EvaluateOrDefault<string>(() => currFrame.GetFileName(), null);
					num2 = Util.EvaluateOrDefault<int>(() => currFrame.GetFileLineNumber(), 0);
				}
				stringBuilder.Append("   at ");
				stringBuilder.Append(text3 ?? "<UnknownType>");
				stringBuilder.Append(".");
				stringBuilder.Append(text ?? "<UnknownMethod>");
				stringBuilder.Append("(");
				try
				{
					DisposeTrackerObject<T>.AppendParamsToString(currMethod.GetParameters(), stringBuilder);
				}
				catch
				{
					stringBuilder.Append("<Error Getting Params>");
				}
				stringBuilder.Append(")");
				if (text2 != null)
				{
					stringBuilder.Append(" in ");
					stringBuilder.Append(text2);
					stringBuilder.Append(":line ");
					stringBuilder.Append(num2.ToString(CultureInfo.InvariantCulture));
				}
				stringBuilder.Append(Environment.NewLine);
			}
			return stringBuilder.ToString();
		}

		private static void WatsonThreadProc(object stateInfo)
		{
			Thread.CurrentThread.IsBackground = false;
			DisposeTrackerObject<T>.WatsonThreadStateInfo watsonThreadStateInfo = (DisposeTrackerObject<T>.WatsonThreadStateInfo)stateInfo;
			string text = DisposeTrackerObject<T>.StackTraceToString(watsonThreadStateInfo.StackTrace);
			if (text != null)
			{
				ObjectNotDisposedException<T> exception = new ObjectNotDisposedException<T>(text, watsonThreadStateInfo.StackTraceWasReset);
				try
				{
					if (watsonThreadStateInfo.ExtraDataList != null)
					{
						foreach (WatsonExtraDataReportAction action in watsonThreadStateInfo.ExtraDataList)
						{
							ExWatson.RegisterReportAction(action, WatsonActionScope.Thread);
						}
					}
					ExWatson.HandleException(new UnhandledExceptionEventArgs(exception, DisposeTrackerOptions.TerminateOnReport), ReportOptions.DoNotCollectDumps | ReportOptions.DeepStackTraceHash | ReportOptions.DoNotLogProcessAndThreadIds | ReportOptions.DoNotFreezeThreads);
				}
				finally
				{
					if (watsonThreadStateInfo.ExtraDataList != null)
					{
						foreach (WatsonExtraDataReportAction action2 in watsonThreadStateInfo.ExtraDataList)
						{
							ExWatson.UnregisterReportAction(action2, WatsonActionScope.Thread);
						}
					}
				}
			}
			Thread.CurrentThread.IsBackground = true;
		}

		private static bool IsDisposeTrackable(Type type)
		{
			if (type == null)
			{
				return false;
			}
			foreach (Type left in type.GetTypeInfo().ImplementedInterfaces)
			{
				if (left == DisposeTrackerObject<T>.DisposeTrackableType)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsShuttingDown()
		{
			return Environment.HasShutdownStarted || AppDomain.CurrentDomain.IsFinalizingForUnload();
		}

		private static readonly Action<object> watsonDelegate = new Action<object>(DisposeTrackerObject<T>.WatsonThreadProc);

		private static readonly Type DisposeTrackableType = typeof(IDisposeTrackable);

		private static bool isTypeRisky = false;

		private static int numCollectedCounter = 0;

		private static int skipCounter = 0;

		internal class WatsonThreadStateInfo
		{
			internal WatsonThreadStateInfo(StackTrace stackTrace, bool stackTraceWasReset, IList<WatsonExtraDataReportAction> extraDataList)
			{
				this.StackTrace = stackTrace;
				this.StackTraceWasReset = stackTraceWasReset;
				this.ExtraDataList = extraDataList;
			}

			internal StackTrace StackTrace { get; private set; }

			internal bool StackTraceWasReset { get; private set; }

			internal IList<WatsonExtraDataReportAction> ExtraDataList { get; private set; }
		}
	}
}
