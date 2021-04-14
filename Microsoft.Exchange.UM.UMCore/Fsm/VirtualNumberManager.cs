using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class VirtualNumberManager
	{
		internal static void GetScope(ActivityManager manager, out VirtualNumberManager scope)
		{
			for (scope = (manager as VirtualNumberManager); scope == null; scope = (manager as VirtualNumberManager))
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

		internal static TransitionBase CheckIfCallFromBlockedNumber(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			VirtualNumberManager virtualNumberManager = manager as VirtualNumberManager;
			if (virtualNumberManager == null)
			{
				VirtualNumberManager.GetScope(manager, out virtualNumberManager);
			}
			return manager.GetTransition(virtualNumberManager.CheckIfCallFromBlockedNumber(vo));
		}
	}
}
