using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class FaxManager
	{
		internal static void GetScope(ActivityManager manager, out FaxManager scope)
		{
			for (scope = (manager as FaxManager); scope == null; scope = (manager as FaxManager))
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

		internal static TransitionBase IsQuotaExceeded(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase IsPipelineHealthy(ActivityManager manager, string methodName, BaseUMCallSession vo)
		{
			return manager.ExecuteAction(methodName, vo);
		}

		internal static TransitionBase HandleFailedTransfer(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			FaxManager faxManager = manager as FaxManager;
			if (faxManager == null)
			{
				FaxManager.GetScope(manager, out faxManager);
			}
			return manager.GetTransition(faxManager.HandleFailedTransfer(vo));
		}

		internal static PhoneNumber TargetPhoneNumber(ActivityManager manager, string variableName)
		{
			FaxManager faxManager = manager as FaxManager;
			if (faxManager == null)
			{
				FaxManager.GetScope(manager, out faxManager);
			}
			return faxManager.TargetPhoneNumber;
		}
	}
}
