using System;
using System.Reflection;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class FsmVariable<T>
	{
		private FsmVariable(FsmVariable<T>.VariableDelegate d, string variableName)
		{
			this.variableDelegate = d;
			this.variableName = variableName;
		}

		internal static FsmVariable<T> Create(QualifiedName variableName, ActivityManagerConfig variableScope)
		{
			FsmVariable<T> result = null;
			if (!FsmVariable<T>.TryCreate(variableName, variableScope, out result))
			{
				throw new FsmConfigurationException(Strings.InvalidVariable(variableName.FullName));
			}
			return result;
		}

		internal static bool TryCreate(QualifiedName variableName, ActivityManagerConfig variableScope, out FsmVariable<T> fsmVariable)
		{
			fsmVariable = null;
			FsmVariable<T>.VariableDelegate variableDelegate = FsmVariable<T>.FindVariableDelegate(variableName, variableScope);
			if (variableDelegate != null)
			{
				fsmVariable = new FsmVariable<T>(variableDelegate, variableName.ShortName);
				return true;
			}
			return false;
		}

		internal T GetValue(ActivityManager m)
		{
			return this.variableDelegate(m, this.variableName);
		}

		private static FsmVariable<T>.VariableDelegate FindVariableDelegate(QualifiedName variableName, ActivityManagerConfig variableScope)
		{
			FsmVariable<T>.VariableDelegate variableDelegate = null;
			object obj = null;
			string key = variableName + ":" + typeof(T).ToString();
			if (FsmVariableCache.Instance.TryGetValue(key, out obj))
			{
				variableDelegate = (obj as FsmVariable<T>.VariableDelegate);
			}
			else
			{
				while (variableScope != null && string.Compare(variableScope.ClassName, variableName.Namespace, StringComparison.OrdinalIgnoreCase) != 0)
				{
					variableScope = variableScope.ManagerConfig;
				}
				Type fsmProxyType = FsmVariable<T>.GetFsmProxyType(variableScope);
				MethodInfo method = fsmProxyType.GetMethod(variableName.ShortName, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.NonPublic, null, FsmVariable<T>.parameterArray, null);
				if (null != method)
				{
					variableDelegate = (FsmVariable<T>.VariableDelegate)Delegate.CreateDelegate(typeof(FsmVariable<T>.VariableDelegate), method, false);
				}
			}
			if (variableDelegate != null)
			{
				FsmVariableCache.Instance[key] = variableDelegate;
			}
			return variableDelegate;
		}

		private static Type GetFsmProxyType(ActivityManagerConfig variableScope)
		{
			Type result;
			if (variableScope == null)
			{
				result = GlobalActivityManager.ConfigClass.CoreAssembly.GetType("Microsoft.Exchange.UM.Fsm." + typeof(GlobalActivityManager).Name);
			}
			else
			{
				result = variableScope.FsmProxyType;
			}
			return result;
		}

		private static readonly Type[] parameterArray = new Type[]
		{
			typeof(ActivityManager),
			typeof(string)
		};

		private FsmVariable<T>.VariableDelegate variableDelegate;

		private string variableName;

		internal delegate T VariableDelegate(ActivityManager m, string variableName);
	}
}
