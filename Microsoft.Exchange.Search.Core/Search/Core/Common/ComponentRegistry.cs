using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal static class ComponentRegistry
	{
		static ComponentRegistry()
		{
			ComponentRegistry.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("ComponentRegistry", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.CoreComponentRegistryTracer, (long)"RegisterComponent".GetHashCode());
		}

		private static Dictionary<Type, ComponentInfo> ComponentInfoMap
		{
			get
			{
				if (ComponentRegistry.componentInfoMap == null)
				{
					lock (ComponentRegistry.locker)
					{
						if (ComponentRegistry.componentInfoMap == null)
						{
							ComponentRegistry.componentInfoMap = new Dictionary<Type, ComponentInfo>();
						}
					}
				}
				return ComponentRegistry.componentInfoMap;
			}
		}

		internal static MethodInfo GetRegisterMethod(Type type)
		{
			MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			ComponentRegistry.diagnosticsSession.TraceDebug<Type>("Looking for RegisterComponent method for type {0}", type);
			foreach (MethodInfo methodInfo in methods)
			{
				if (StringComparer.OrdinalIgnoreCase.Compare(methodInfo.Name, "RegisterComponent") == 0)
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters != null && parameters.Length == 1 && parameters[0].ParameterType == typeof(ComponentInfo) && methodInfo.ReturnParameter.ParameterType == typeof(void))
					{
						return methodInfo;
					}
				}
			}
			ComponentRegistry.diagnosticsSession.TraceDebug<Type>("Could not find RegisterComponent method for type {0}", type);
			return null;
		}

		internal static void Register<T>() where T : StatefulComponent
		{
			lock (ComponentRegistry.locker)
			{
				ComponentRegistry.diagnosticsSession.TraceDebug<Type>("Registering component {0}", typeof(T));
				ComponentInfo componentInfo = ComponentInfo.Create<T>();
				Stack<MethodInfo> stack = new Stack<MethodInfo>();
				ComponentRegistry.diagnosticsSession.TraceDebug<Type>("Collecting RegisterComponent methods for class hierarchy for type {0}", typeof(T));
				Type type = typeof(T);
				while (type != typeof(object))
				{
					MethodInfo registerMethod = ComponentRegistry.GetRegisterMethod(type);
					if (registerMethod != null)
					{
						stack.Push(registerMethod);
					}
					type = type.BaseType;
				}
				ComponentRegistry.diagnosticsSession.TraceDebug<int, Type>("{0} RegisterComponent methods found for type {1}", stack.Count, typeof(T));
				if (stack.Count == 0)
				{
					throw new InvalidOperationException(string.Format("No register methods were found for type {0}", typeof(T)));
				}
				while (stack.Count > 0)
				{
					MethodInfo methodInfo = stack.Pop();
					methodInfo.Invoke(null, new object[]
					{
						componentInfo
					});
				}
				ComponentRegistry.diagnosticsSession.TraceDebug<Type>("Registering type {0} in component registry", typeof(T));
				ComponentRegistry.ComponentInfoMap.Add(typeof(T), componentInfo);
			}
		}

		internal static bool TryGetTransitionInfo(Type type, uint state, uint signal, out List<TransitionInfo> transitionInfos)
		{
			transitionInfos = null;
			ComponentInfo componentInfo = null;
			if (ComponentRegistry.ComponentInfoMap.TryGetValue(type, out componentInfo))
			{
				if (componentInfo.TransitionInfoMap.TryGetValue(new KeyValuePair<uint, uint>(state, signal), out transitionInfos))
				{
					return true;
				}
				if (componentInfo.TransitionInfoMap.TryGetValue(new KeyValuePair<uint, uint>(4294967295U, signal), out transitionInfos))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool TryGetSignalInfo(Type type, uint signal, out SignalInfo signalInfo)
		{
			signalInfo = null;
			ComponentInfo componentInfo = null;
			return ComponentRegistry.ComponentInfoMap.TryGetValue(type, out componentInfo) && componentInfo.SignalInfoMap.TryGetValue(signal, out signalInfo);
		}

		internal static bool TryGetStateInfo(Type type, uint state, out StateInfo stateInfo)
		{
			stateInfo = null;
			ComponentInfo componentInfo = null;
			return ComponentRegistry.ComponentInfoMap.TryGetValue(type, out componentInfo) && componentInfo.StateInfoMap.TryGetValue(state, out stateInfo);
		}

		private const string RegisterMethodName = "RegisterComponent";

		private static readonly IDiagnosticsSession diagnosticsSession;

		private static Dictionary<Type, ComponentInfo> componentInfoMap;

		private static object locker = new object();
	}
}
