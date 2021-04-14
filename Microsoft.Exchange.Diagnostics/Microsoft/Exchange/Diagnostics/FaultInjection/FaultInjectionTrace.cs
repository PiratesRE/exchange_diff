using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Diagnostics.FaultInjection
{
	public class FaultInjectionTrace : BaseTrace
	{
		public FaultInjectionTrace(Guid guid, int traceTag) : base(guid, traceTag)
		{
		}

		public void TraceTest(uint lid)
		{
			object obj = null;
			this.TraceTest<object>(lid, ref obj);
		}

		public void TraceTest<T>(uint lid, T objectToCompare)
		{
			this.TraceTest<T>(lid, ref objectToCompare);
		}

		public void TraceTest<T>(uint lid, ref T objectToChange)
		{
			string text = ExTraceConfiguration.Instance.ComponentInjection();
			ExTraceGlobals.TracingTracer.TraceDebug<uint, string>(35221, (long)((ulong)lid), "TraceTest called for LID:{0}, TagIdentifier:'{1}'.", lid, text);
			if (base.IsTraceEnabled(TraceType.FaultInjection))
			{
				if (ExTraceConfiguration.DisableAllTraces != null && ExTraceConfiguration.DisableAllTraces.Contains(this.category))
				{
					ExTraceGlobals.TracingTracer.TraceDebug<Guid>(56316L, "Component: {0} has all traces temporary disabled for this thread.", this.category);
					return;
				}
				ExTraceGlobals.TracingTracer.TraceDebug<Guid, uint, string>(51605, (long)((ulong)lid), "FI tracing is enabled for category:{0}, lid:{1}, tagIdentifier:'{2}'.", this.category, lid, text);
				FaultInjectionTagComponentConfig faultInjectionTagComponentConfig = null;
				if (ExTraceConfiguration.Instance.FaultInjectionConfiguration.TryGetValue(this.category, out faultInjectionTagComponentConfig))
				{
					FaultInjectionComponentConfig faultInjectionComponentConfig = null;
					if (faultInjectionTagComponentConfig.TryGetValue(text, out faultInjectionComponentConfig) || faultInjectionTagComponentConfig.TryGetValue(string.Empty, out faultInjectionComponentConfig))
					{
						ExTraceGlobals.TracingTracer.TraceDebug<string>(33320, (long)((ulong)lid), "Tracing found for tag identifier:'{0}'", text);
						if (ExTraceConfiguration.DisabledLids != null && ExTraceConfiguration.DisabledLids.Contains(lid))
						{
							ExTraceGlobals.TracingTracer.TraceDebug<uint>(43932, (long)((ulong)lid), "LID:{0} is temporary disabled for this thread.", lid);
							return;
						}
						FaultInjectionPointConfig faultInjectionPointConfig = null;
						if (faultInjectionComponentConfig.TryGetValue(lid, out faultInjectionPointConfig))
						{
							ExTraceGlobals.TracingTracer.TraceDebug<FaultInjectionType, uint>(45461, (long)((ulong)lid), "Tracing found for a configuration of type:{0} for lid:{1}", faultInjectionPointConfig.Type, lid);
							string valueToCompareTo = null;
							InjectionComparisonOperator comparisonOperator = InjectionComparisonOperator.Skip;
							List<string> expectedCallStack = null;
							switch (faultInjectionPointConfig.Type)
							{
							case FaultInjectionType.None:
								break;
							case FaultInjectionType.Sync:
							{
								int condition = FaultInjectionTrace.GetCondition(faultInjectionPointConfig.Parameters, out comparisonOperator, out valueToCompareTo);
								if (condition < faultInjectionPointConfig.Parameters.Count)
								{
									expectedCallStack = faultInjectionPointConfig.Parameters.GetRange(condition, faultInjectionPointConfig.Parameters.Count - condition);
								}
								if (FaultInjectionTrace.ValidateCondition<T>(valueToCompareTo, ref objectToChange, comparisonOperator) && FaultInjectionTrace.ValidateCallStack(lid, expectedCallStack))
								{
									ExTraceGlobals.TracingTracer.TraceInformation<uint>(59951, (long)((ulong)lid), "FI.Sync[lid:{0}]", lid);
									FaultInjectionTrace.InjectSync(lid, text);
									return;
								}
								break;
							}
							case FaultInjectionType.Exception:
								if (faultInjectionPointConfig.Parameters != null && 0 < faultInjectionPointConfig.Parameters.Count)
								{
									int condition2 = FaultInjectionTrace.GetCondition(faultInjectionPointConfig.Parameters, out comparisonOperator, out valueToCompareTo);
									if (condition2 < faultInjectionPointConfig.Parameters.Count)
									{
										string text2 = faultInjectionPointConfig.Parameters[condition2];
										if (condition2 + 1 < faultInjectionPointConfig.Parameters.Count)
										{
											expectedCallStack = faultInjectionPointConfig.Parameters.GetRange(condition2 + 1, faultInjectionPointConfig.Parameters.Count - condition2 - 1);
										}
										if (FaultInjectionTrace.ValidateCondition<T>(valueToCompareTo, ref objectToChange, comparisonOperator) && FaultInjectionTrace.ValidateCallStack(lid, expectedCallStack))
										{
											this.EnsureExceptionInjectionCallbackExists();
											ExTraceGlobals.TracingTracer.TraceInformation<uint, string>(43567, (long)((ulong)lid), "FI.Exception[lid:{0}] {1}", lid, text2);
											FaultInjectionTrace.InjectException(this.category, text2);
											return;
										}
									}
								}
								break;
							case FaultInjectionType.Investigate:
								ExTraceGlobals.TracingTracer.TraceInformation<StackTrace>(53653, 0L, "Tracing FaultInjectionType.Investigate with stack trace: {0}", new StackTrace());
								return;
							case FaultInjectionType.ChangeValue:
								if (faultInjectionPointConfig.Parameters != null && 0 < faultInjectionPointConfig.Parameters.Count)
								{
									int condition3 = FaultInjectionTrace.GetCondition(faultInjectionPointConfig.Parameters, out comparisonOperator, out valueToCompareTo);
									string text3 = null;
									if (condition3 < faultInjectionPointConfig.Parameters.Count)
									{
										text3 = faultInjectionPointConfig.Parameters[condition3];
									}
									if (string.IsNullOrEmpty(text3))
									{
										throw new InvalidOperationException("Expected non-null and non-empty string: string.IsNullOrEmpty(valueToChangeTo)");
									}
									if (condition3 + 1 < faultInjectionPointConfig.Parameters.Count)
									{
										expectedCallStack = faultInjectionPointConfig.Parameters.GetRange(1, faultInjectionPointConfig.Parameters.Count - 1);
									}
									object obj = null;
									if (FaultInjectionTrace.ValidateCondition<object>(valueToCompareTo, ref obj, comparisonOperator) && FaultInjectionTrace.ValidateCallStack(lid, expectedCallStack))
									{
										ExTraceGlobals.TracingTracer.TraceInformation<uint, T, string>(55855, (long)((ulong)lid), "FI.ChangeValue[lid:{0}] {1} => {2}", lid, objectToChange, text3);
										FaultInjectionTrace.StringToValue<T>(text3, ref objectToChange);
									}
								}
								break;
							default:
								return;
							}
						}
					}
				}
			}
		}

		public IDisposable DisableTraceTest(uint lid)
		{
			ExTraceGlobals.TracingTracer.TraceDebug<uint>(60316, (long)((ulong)lid), "DisableTraceTest called for LID:{0}.", lid);
			if (base.IsTraceEnabled(TraceType.FaultInjection))
			{
				return new FaultInjectionTrace.TemporaryDisableFaultInjection(lid);
			}
			return null;
		}

		public IDisposable DisableAllTraces()
		{
			ExTraceGlobals.TracingTracer.TraceDebug<Guid>(62364L, "DisableAllTraces for component:{0}.", this.category);
			if (base.IsTraceEnabled(TraceType.FaultInjection))
			{
				return new FaultInjectionTrace.TemporaryDisableAllFaultInjection(this.category);
			}
			return null;
		}

		public bool IsStatisticalFaultInjectionEnabled(uint lid)
		{
			bool result = false;
			this.TraceTest<bool>(lid, ref result);
			return result;
		}

		[Conditional("DEBUG")]
		public void DebugInjectDelay(uint lid)
		{
			if (!base.IsTraceEnabled(TraceType.FaultInjection))
			{
				return;
			}
			int num = 0;
			this.TraceTest<int>(lid, ref num);
			if (0 < num)
			{
				using (ManualResetEvent manualResetEvent = new ManualResetEvent(false))
				{
					manualResetEvent.WaitOne(num);
				}
			}
		}

		public void RegisterExceptionInjectionCallback(ExceptionInjectionCallback callback)
		{
			lock (ExTraceConfiguration.Instance.ExceptionInjection)
			{
				ExTraceConfiguration.Instance.ExceptionInjection[this.category] = callback;
			}
		}

		public void RegisterComponentInjectionCallback(ComponentInjectionCallback callback)
		{
			lock (ExTraceConfiguration.Instance.ComponentInjection)
			{
				ExTraceConfiguration.Instance.ComponentInjection = callback;
			}
		}

		internal static void CreateSyncEventNames(uint lid, string tagIdentifier, out string devEventName, out string testEventName)
		{
			devEventName = "Global\\IFXE" + lid.ToString() + "_" + tagIdentifier;
			testEventName = "Global\\IFXE_" + lid.ToString() + "_" + tagIdentifier;
		}

		private static Exception GenericExceptionCallback(string exceptionTypeNameParams)
		{
			Exception ex = null;
			if (exceptionTypeNameParams != null)
			{
				Regex regex = new Regex("([A-Za-z.]+)(?:\\((?:(?:((?:[0-9]+)|(?:'[A-Za-z0-9,.?:;!@_ ]*')))[, ]*)+\\))*");
				char[] trimChars = new char[]
				{
					'\''
				};
				string text = string.Empty;
				string[] array = new string[0];
				if (regex.IsMatch(exceptionTypeNameParams))
				{
					using (IEnumerator enumerator = regex.Matches(exceptionTypeNameParams).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							Match match = (Match)obj;
							if (match.Groups.Count == 3)
							{
								text = match.Groups[1].Value;
								if (match.Groups[1].Captures.Count > 0)
								{
									array = new string[match.Groups[2].Captures.Count];
									int num = 0;
									foreach (object obj2 in match.Groups[2].Captures)
									{
										Capture capture = (Capture)obj2;
										array[num] = capture.Value.Trim(trimChars);
										num++;
									}
								}
							}
						}
						goto IL_157;
					}
					goto IL_146;
					IL_157:
					Type targetType = FaultInjectionTrace.LookUpType(text);
					if (text == null)
					{
						throw new InvalidOperationException("No Type Exists In Current Calling Assembly Or Referenced Assemblies With The Name : " + exceptionTypeNameParams);
					}
					if (array.Length == 0)
					{
						ex = (FaultInjectionTrace.InvokeConstructor(targetType) as Exception);
					}
					else
					{
						ex = (FaultInjectionTrace.InvokeConstructorWithParams(targetType, array) as Exception);
					}
					if (ex == null)
					{
						throw new InvalidOperationException("Type Name Specified For The Exception Callback Is Not A Valid Exception Type : " + exceptionTypeNameParams);
					}
					return ex;
				}
				IL_146:
				throw new InvalidOperationException("The exception string is not valid. ExceptionTypeNameParams: " + exceptionTypeNameParams);
			}
			return ex;
		}

		private static object InvokeConstructor(Type targetType)
		{
			object result = null;
			bool flag = false;
			try
			{
				result = ((targetType == typeof(string)) ? string.Empty : Activator.CreateInstance(targetType));
			}
			catch (MissingMethodException)
			{
				using (IEnumerator<ConstructorInfo> enumerator = targetType.GetTypeInfo().DeclaredConstructors.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						ConstructorInfo constructorInfo = enumerator.Current;
						flag = true;
						ParameterInfo[] parameters = constructorInfo.GetParameters();
						object[] array = new object[parameters.Length];
						int num = 0;
						foreach (ParameterInfo parameterInfo in parameters)
						{
							array[num] = FaultInjectionTrace.InvokeConstructor(parameterInfo.ParameterType);
							num++;
						}
						result = constructorInfo.Invoke(array);
					}
				}
				if (!flag)
				{
					throw new InvalidOperationException("Type Specified Does Not Have Any Constructors : " + targetType.FullName);
				}
			}
			return result;
		}

		private static object InvokeConstructorWithParams(Type targetType, string[] exceptionParameters)
		{
			object result = null;
			bool flag = false;
			try
			{
				result = ((targetType == typeof(string)) ? string.Empty : Activator.CreateInstance(targetType, exceptionParameters));
			}
			catch (MissingMethodException)
			{
				foreach (ConstructorInfo constructorInfo in targetType.GetTypeInfo().DeclaredConstructors)
				{
					ParameterInfo[] parameters = constructorInfo.GetParameters();
					if (parameters.Length == exceptionParameters.Length)
					{
						flag = true;
						object[] array = new object[parameters.Length];
						for (int i = 0; i < parameters.Length; i++)
						{
							array[i] = FaultInjectionTrace.StringToParameter(exceptionParameters[i], parameters[i].ParameterType);
						}
						result = constructorInfo.Invoke(array);
						break;
					}
				}
				if (!flag)
				{
					throw new InvalidOperationException("Type Specified Does Not Have Any Constructors that match the parameters number: " + targetType.FullName);
				}
			}
			return result;
		}

		private static Type LookUpType(string typeName)
		{
			Type type = null;
			StackTrace stackTrace = new StackTrace();
			Assembly assembly = null;
			foreach (StackFrame stackFrame in stackTrace.GetFrames())
			{
				Assembly assembly2 = stackFrame.GetMethod().DeclaringType.GetTypeInfo().Assembly;
				if (assembly2 != assembly)
				{
					assembly = assembly2;
					type = assembly.GetType(typeName);
					if (type == null)
					{
						type = FaultInjectionTrace.CheckReferencedAssemblies(assembly, typeName, 0);
					}
					if (type != null)
					{
						break;
					}
				}
			}
			return type;
		}

		private static Type CheckReferencedAssemblies(Assembly assembly, string typeName, int depth)
		{
			Type type = null;
			foreach (AssemblyName assemblyRef in assembly.GetReferencedAssemblies())
			{
				Assembly assembly2 = Assembly.Load(assemblyRef);
				type = assembly2.GetType(typeName);
				if (type != null)
				{
					break;
				}
				if (depth > 0)
				{
					type = FaultInjectionTrace.CheckReferencedAssemblies(assembly2, typeName, depth--);
					if (type != null)
					{
						break;
					}
				}
			}
			return type;
		}

		private static int GetCondition(List<string> parameters, out InjectionComparisonOperator comparisonOperator, out string valueToCompareTo)
		{
			valueToCompareTo = null;
			comparisonOperator = InjectionComparisonOperator.Skip;
			int result = 0;
			if (2 < parameters.Count && parameters[0].Equals("CONDITION"))
			{
				valueToCompareTo = parameters[1];
				comparisonOperator = (InjectionComparisonOperator)Enum.Parse(typeof(InjectionComparisonOperator), parameters[2], true);
				result = 3;
			}
			return result;
		}

		private static bool ValidateCallStack(uint lid, List<string> expectedCallStack)
		{
			bool result = true;
			if (expectedCallStack != null && 0 < expectedCallStack.Count)
			{
				string text = new StackTrace().ToString();
				ExTraceGlobals.TracingTracer.TraceInformation<string>(37269, 0L, "Tracing was called with this callstack:{0}", text);
				foreach (string text2 in expectedCallStack)
				{
					ExTraceGlobals.TracingTracer.TraceInformation<string>(61845, 0L, "Tracing expects this string in the callstack:{0}", text2);
					if (!text.Contains(text2))
					{
						ExTraceGlobals.TracingTracer.TraceDebug<string, string>(52567, (long)((ulong)lid), "FI call stack mismatch: {0} was not found in\r\n{1}", text2, text);
						result = false;
						break;
					}
				}
			}
			return result;
		}

		private static bool ValidateCondition<T>(string valueToCompareTo, ref T objectToCompare, InjectionComparisonOperator comparisonOperator)
		{
			bool result;
			try
			{
				T t = default(T);
				if (valueToCompareTo != null)
				{
					FaultInjectionTrace.StringToValue<T>(valueToCompareTo, ref t);
				}
				string text = objectToCompare as string;
				switch (comparisonOperator)
				{
				case InjectionComparisonOperator.Skip:
					result = true;
					break;
				case InjectionComparisonOperator.Equals:
					result = (Comparer<T>.Default.Compare(objectToCompare, t) == 0);
					break;
				case InjectionComparisonOperator.NotEquals:
					result = (Comparer<T>.Default.Compare(objectToCompare, t) != 0);
					break;
				case InjectionComparisonOperator.GreaterThan:
					result = (Comparer<T>.Default.Compare(objectToCompare, t) == 1);
					break;
				case InjectionComparisonOperator.GreaterThanOrEqual:
					result = (Comparer<T>.Default.Compare(objectToCompare, t) >= 0);
					break;
				case InjectionComparisonOperator.LessThan:
					result = (Comparer<T>.Default.Compare(objectToCompare, t) == -1);
					break;
				case InjectionComparisonOperator.LessThanOrEqual:
					result = (Comparer<T>.Default.Compare(objectToCompare, t) <= 0);
					break;
				case InjectionComparisonOperator.Contains:
					if (text != null)
					{
						result = text.Contains(valueToCompareTo);
					}
					else
					{
						result = false;
					}
					break;
				case InjectionComparisonOperator.NotContains:
					if (text != null)
					{
						result = !text.Contains(valueToCompareTo);
					}
					else
					{
						result = false;
					}
					break;
				case InjectionComparisonOperator.IsNull:
					result = (objectToCompare == null);
					break;
				case InjectionComparisonOperator.IsNotNull:
					result = (objectToCompare != null);
					break;
				case InjectionComparisonOperator.PPM:
				{
					int num = FaultInjectionTrace.random.Next(1000000);
					int num2;
					bool flag = int.TryParse((string)((object)t), out num2);
					if (flag && num < num2)
					{
						result = true;
					}
					else
					{
						result = false;
					}
					break;
				}
				default:
					result = false;
					break;
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.TracingTracer.TraceInformation<string>(63039, 0L, "ValidateCondition<T>: An exception occured while doing the comparison: {0}", ex.ToString());
				result = false;
			}
			return result;
		}

		private static void InjectException(Guid componentGuid, string exceptionTypeParams)
		{
			lock (ExTraceConfiguration.Instance.ExceptionInjection)
			{
				ExceptionInjectionCallback exceptionInjectionCallback;
				if (ExTraceConfiguration.Instance.ExceptionInjection.TryGetValue(componentGuid, out exceptionInjectionCallback))
				{
					Exception ex = exceptionInjectionCallback(exceptionTypeParams);
					if (ex == null)
					{
						ex = FaultInjectionTrace.GenericExceptionCallback(exceptionTypeParams);
					}
					throw ex;
				}
			}
		}

		private static void InjectSync(uint lid, string tagIdentifier)
		{
			string name;
			string name2;
			FaultInjectionTrace.CreateSyncEventNames(lid, tagIdentifier, out name, out name2);
			try
			{
				using (EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting(name))
				{
					using (EventWaitHandle eventWaitHandle2 = EventWaitHandle.OpenExisting(name2))
					{
						eventWaitHandle.Reset();
						eventWaitHandle2.Set();
						eventWaitHandle.WaitOne();
						eventWaitHandle2.Reset();
					}
				}
			}
			catch
			{
			}
		}

		private static void StringToValue<T>(string valueToChangeTo, ref T objectToChange)
		{
			try
			{
				if (typeof(T).GetTypeInfo().IsEnum)
				{
					objectToChange = (T)((object)Enum.Parse(typeof(T), valueToChangeTo));
				}
				else if (typeof(T).Equals(typeof(Guid)))
				{
					objectToChange = (T)((object)new Guid(valueToChangeTo));
				}
				else if (typeof(T).Equals(typeof(TimeSpan)))
				{
					objectToChange = (T)((object)TimeSpan.FromSeconds(Convert.ToDouble(valueToChangeTo)));
				}
				else if (typeof(T).Equals(typeof(DateTime)))
				{
					objectToChange = (T)((object)new DateTime(Convert.ToInt64(valueToChangeTo), DateTimeKind.Utc));
				}
				else if (typeof(T).Equals(typeof(OperatingSystem)))
				{
					Version version = new Version(valueToChangeTo);
					objectToChange = (T)((object)new OperatingSystem(PlatformID.Win32NT, version));
				}
				else if (typeof(T).Equals(typeof(Version)))
				{
					objectToChange = (T)((object)new Version(valueToChangeTo));
				}
				else if (typeof(T).Equals(typeof(int)))
				{
					objectToChange = (T)((object)Convert.ToInt32(valueToChangeTo));
				}
				else if (typeof(T).Equals(typeof(string)))
				{
					objectToChange = (T)((object)valueToChangeTo);
				}
				else if (typeof(T).GetTypeInfo().IsClass && string.Equals(valueToChangeTo, "null"))
				{
					objectToChange = (T)((object)null);
				}
				else
				{
					objectToChange = (T)((object)Convert.ChangeType(valueToChangeTo, typeof(T)));
				}
			}
			catch
			{
				ExTraceGlobals.TracingTracer.TraceInformation<Type>(34741, 0L, "StringToValue<T>: Unable to change string into type {0}.  Currently, Enum, TimeSpan, Classes where valueToChangeTo='null', int, and bool have been tested. If your type is not one of these, please add logic to parse into this type from string", typeof(T));
			}
		}

		private static object StringToParameter(string stringToParse, Type resultType)
		{
			object result = null;
			try
			{
				if (resultType.GetTypeInfo().IsEnum)
				{
					result = Enum.Parse(resultType, stringToParse);
				}
				else if (resultType.Equals(typeof(Guid)))
				{
					result = new Guid(stringToParse);
				}
				else if (resultType.Equals(typeof(TimeSpan)))
				{
					result = TimeSpan.FromSeconds(Convert.ToDouble(stringToParse));
				}
				else if (resultType.Equals(typeof(DateTime)))
				{
					result = new DateTime(Convert.ToInt64(stringToParse), DateTimeKind.Utc);
				}
				else if (resultType.Equals(typeof(OperatingSystem)))
				{
					Version version = new Version(stringToParse);
					result = new OperatingSystem(PlatformID.Win32NT, version);
				}
				else if (resultType.Equals(typeof(Version)))
				{
					result = new Version(stringToParse);
				}
				else if (resultType.Equals(typeof(int)))
				{
					result = Convert.ToInt32(stringToParse);
				}
				else if (resultType.Equals(typeof(string)))
				{
					result = stringToParse;
				}
				else if (resultType.GetTypeInfo().IsClass && string.Equals(stringToParse, "null"))
				{
					result = null;
				}
				else
				{
					result = Convert.ChangeType(stringToParse, resultType);
				}
			}
			catch
			{
				ExTraceGlobals.TracingTracer.TraceInformation<Type>(35740, 0L, "StringToParameter: Unable to translate string into type {0}.  Currently, Enum, TimeSpan, Classes where valueToChangeTo='null', int, and bool are supported. If your type is not one of these, please add logic to parse into this type from string", resultType);
			}
			return result;
		}

		private void EnsureExceptionInjectionCallbackExists()
		{
			if (!ExTraceConfiguration.Instance.ExceptionInjection.ContainsKey(this.category) || ExTraceConfiguration.Instance.ExceptionInjection[this.category] == null)
			{
				this.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(FaultInjectionTrace.GenericExceptionCallback));
			}
		}

		public const string Condition = "CONDITION";

		private const int OneMillion = 1000000;

		private static Random random = new Random(Environment.TickCount);

		private class TemporaryDisableFaultInjection : IDisposable
		{
			public TemporaryDisableFaultInjection(uint lid)
			{
				this.lid = lid;
				if (ExTraceConfiguration.DisabledLids == null)
				{
					ExTraceConfiguration.DisabledLids = new HashSet<uint>();
				}
				this.prevDisabled = ExTraceConfiguration.DisabledLids.Contains(lid);
				if (!this.prevDisabled)
				{
					ExTraceGlobals.TracingTracer.TraceDebug<uint>(52124, (long)((ulong)lid), "Disable fault injection config for lid:{0}.", this.lid);
					ExTraceConfiguration.DisabledLids.Add(lid);
				}
			}

			public void Dispose()
			{
				if (!this.prevDisabled)
				{
					ExTraceGlobals.TracingTracer.TraceDebug<uint>(45980, (long)((ulong)this.lid), "Enable fault injection config for lid:{0}.", this.lid);
					ExTraceConfiguration.DisabledLids.Remove(this.lid);
				}
			}

			private readonly uint lid;

			private readonly bool prevDisabled;
		}

		private class TemporaryDisableAllFaultInjection : IDisposable
		{
			public TemporaryDisableAllFaultInjection(Guid component)
			{
				this.component = component;
				if (ExTraceConfiguration.DisableAllTraces == null)
				{
					ExTraceConfiguration.DisableAllTraces = new HashSet<Guid>();
				}
				this.prevDisabled = ExTraceConfiguration.DisableAllTraces.Contains(this.component);
				if (!this.prevDisabled)
				{
					ExTraceGlobals.TracingTracer.TraceDebug<Guid>(64508L, "Disable all traces for component:{0}.", this.component);
					ExTraceConfiguration.DisableAllTraces.Add(this.component);
				}
			}

			public void Dispose()
			{
				if (!this.prevDisabled)
				{
					ExTraceGlobals.TracingTracer.TraceDebug<Guid>(39932L, "Enable fault injection config for component:{0}.", this.component);
					ExTraceConfiguration.DisableAllTraces.Remove(this.component);
				}
			}

			private readonly Guid component;

			private readonly bool prevDisabled;
		}
	}
}
