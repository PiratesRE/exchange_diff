using System;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class FsmAction
	{
		private FsmAction(FsmAction.ActionDelegate d, string actionName)
		{
			this.actionDelegate = d;
			this.actionName = actionName;
		}

		internal static FsmAction Create(QualifiedName actionName, ActivityManagerConfig actionScope)
		{
			return new FsmAction(FsmAction.FindActionDelegate(actionName, actionScope), actionName.ShortName);
		}

		public override string ToString()
		{
			return this.actionName;
		}

		internal TransitionBase Execute(ActivityManager m, BaseUMCallSession vo)
		{
			TransitionBase result;
			try
			{
				m.PreActionExecute(vo);
				result = this.actionDelegate(m, this.actionName, vo);
			}
			finally
			{
				m.PostActionExecute(vo);
			}
			return result;
		}

		private static FsmAction.ActionDelegate FindActionDelegate(QualifiedName actionName, ActivityManagerConfig actionScope)
		{
			while (actionScope != null && string.Compare(actionScope.ClassName, actionName.Namespace, StringComparison.OrdinalIgnoreCase) != 0)
			{
				actionScope = actionScope.ManagerConfig;
			}
			FsmAction.ActionDelegate actionDelegate = (FsmAction.ActionDelegate)Delegate.CreateDelegate(typeof(FsmAction.ActionDelegate), actionScope.FsmProxyType, actionName.ShortName, true, false);
			if (actionDelegate == null)
			{
				throw new FsmConfigurationException(Strings.InvalidAction(actionName.FullName));
			}
			return actionDelegate;
		}

		private FsmAction.ActionDelegate actionDelegate;

		private string actionName;

		internal delegate TransitionBase ActionDelegate(ActivityManager manager, string variableName, BaseUMCallSession vo);
	}
}
