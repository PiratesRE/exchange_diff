using System;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class FaxRequestConfig : ActivityConfig
	{
		internal FaxRequestConfig(ActivityManagerConfig manager) : base(manager)
		{
		}

		internal override void Load(XmlNode rootNode)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Loading a new FaxRequestConfig from XML.", new object[0]);
			base.Load(rootNode);
		}

		internal override ActivityBase CreateActivity(ActivityManager manager)
		{
			return new FaxRequest(manager, this);
		}

		protected override void LoadComplete()
		{
			if (!ActivityConfig.TransitionMap.ContainsKey(ActivityConfig.BuildTransitionMapKey(this, "faxRequestAccepted")))
			{
				CallIdTracer.TraceError(ExTraceGlobals.StateMachineTracer, this, "Fax Activity id={0} doesn't have a FaxRequestAccepted Transition", new object[]
				{
					base.ActivityId
				});
				throw new FsmConfigurationException(Strings.FaxRequestActivityWithoutFaxRequestAccepted(base.ActivityId));
			}
		}
	}
}
