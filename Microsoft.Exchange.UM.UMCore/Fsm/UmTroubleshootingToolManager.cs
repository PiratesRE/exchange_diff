using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.Fsm
{
	internal class UmTroubleshootingToolManager
	{
		internal static void GetScope(ActivityManager manager, out UmTroubleshootingToolManager scope)
		{
			for (scope = (manager as UmTroubleshootingToolManager); scope == null; scope = (manager as UmTroubleshootingToolManager))
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

		internal static TransitionBase ConfirmAcceptedCallType(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			UmTroubleshootingToolManager umTroubleshootingToolManager = manager as UmTroubleshootingToolManager;
			if (umTroubleshootingToolManager == null)
			{
				UmTroubleshootingToolManager.GetScope(manager, out umTroubleshootingToolManager);
			}
			return manager.GetTransition(umTroubleshootingToolManager.ConfirmAcceptedCallType(vo));
		}

		internal static TransitionBase EchoBackDtmf(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			UmTroubleshootingToolManager umTroubleshootingToolManager = manager as UmTroubleshootingToolManager;
			if (umTroubleshootingToolManager == null)
			{
				UmTroubleshootingToolManager.GetScope(manager, out umTroubleshootingToolManager);
			}
			return manager.GetTransition(umTroubleshootingToolManager.EchoBackDtmf(vo));
		}

		internal static TransitionBase SendStopRecordingDtmf(ActivityManager manager, string actionName, BaseUMCallSession vo)
		{
			UmTroubleshootingToolManager umTroubleshootingToolManager = manager as UmTroubleshootingToolManager;
			if (umTroubleshootingToolManager == null)
			{
				UmTroubleshootingToolManager.GetScope(manager, out umTroubleshootingToolManager);
			}
			return manager.GetTransition(umTroubleshootingToolManager.SendStopRecordingDtmf(vo));
		}
	}
}
