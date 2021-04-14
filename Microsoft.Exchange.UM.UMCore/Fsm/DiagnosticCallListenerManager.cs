using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class DiagnosticCallListenerManager
	{
		internal static void GetScope(ActivityManager manager, out DiagnosticCallListenerManager scope)
		{
			for (scope = (manager as DiagnosticCallListenerManager); scope == null; scope = (manager as DiagnosticCallListenerManager))
			{
				if (manager.Manager == null)
				{
					throw new FsmConfigurationException(string.Empty);
				}
				manager = manager.Manager;
			}
		}

		internal static TransitionBase Disconnect(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static object DiagnosticTUILogonCheck(ActivityManager manager, string variableName)
		{
			return manager.ReadVariable(variableName) ?? null;
		}
	}
}
