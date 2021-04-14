using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class FindMeSubscriberManager
	{
		internal static void GetScope(ActivityManager manager, out FindMeSubscriberManager scope)
		{
			for (scope = (manager as FindMeSubscriberManager); scope == null; scope = (manager as FindMeSubscriberManager))
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

		internal static TransitionBase SendDtmf(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			FindMeSubscriberManager findMeSubscriberManager = manager as FindMeSubscriberManager;
			if (findMeSubscriberManager == null)
			{
				FindMeSubscriberManager.GetScope(manager, out findMeSubscriberManager);
			}
			return manager.GetTransition(findMeSubscriberManager.SendDtmf(vo));
		}

		internal static TransitionBase TerminateFindMe(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			FindMeSubscriberManager findMeSubscriberManager = manager as FindMeSubscriberManager;
			if (findMeSubscriberManager == null)
			{
				FindMeSubscriberManager.GetScope(manager, out findMeSubscriberManager);
			}
			return manager.GetTransition(findMeSubscriberManager.TerminateFindMe(vo));
		}

		internal static TransitionBase AcceptFindMe(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			FindMeSubscriberManager findMeSubscriberManager = manager as FindMeSubscriberManager;
			if (findMeSubscriberManager == null)
			{
				FindMeSubscriberManager.GetScope(manager, out findMeSubscriberManager);
			}
			return manager.GetTransition(findMeSubscriberManager.AcceptFindMe(vo));
		}

		internal static object CalleeRecordName(ActivityManager manager, string variableName)
		{
			FindMeSubscriberManager findMeSubscriberManager = manager as FindMeSubscriberManager;
			if (findMeSubscriberManager == null)
			{
				FindMeSubscriberManager.GetScope(manager, out findMeSubscriberManager);
			}
			return findMeSubscriberManager.CalleeRecordName;
		}

		internal static object CallerRecordedName(ActivityManager manager, string variableName)
		{
			FindMeSubscriberManager findMeSubscriberManager = manager as FindMeSubscriberManager;
			if (findMeSubscriberManager == null)
			{
				FindMeSubscriberManager.GetScope(manager, out findMeSubscriberManager);
			}
			return findMeSubscriberManager.CallerRecordedName;
		}
	}
}
