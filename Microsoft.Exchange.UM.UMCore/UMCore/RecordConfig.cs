using System;
using System.Xml;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class RecordConfig : ActivityConfig
	{
		internal RecordConfig(ActivityManagerConfig manager) : base(manager)
		{
		}

		internal string DtmfStopTones
		{
			get
			{
				return this.dtmfStopTones;
			}
		}

		internal string Type
		{
			get
			{
				return this.recType;
			}
		}

		internal override ActivityBase CreateActivity(ActivityManager manager)
		{
			return new Record(manager, this);
		}

		protected override void LoadAttributes(XmlNode rootNode)
		{
			base.LoadAttributes(rootNode);
			this.dtmfStopTones = rootNode.Attributes["dtmfStopTones"].Value;
			this.recType = string.Intern(rootNode.Attributes["type"].Value);
		}

		protected override void LoadComplete()
		{
			if (!ActivityConfig.TransitionMap.ContainsKey(ActivityConfig.BuildTransitionMapKey(this, "anyKey")) || !ActivityConfig.TransitionMap.ContainsKey(ActivityConfig.BuildTransitionMapKey(this, "timeout")) || !ActivityConfig.TransitionMap.ContainsKey(ActivityConfig.BuildTransitionMapKey(this, "silence")))
			{
				throw new FsmConfigurationException(Strings.RecordMissingTransitions(base.ActivityId));
			}
		}

		private string dtmfStopTones;

		private string recType;
	}
}
