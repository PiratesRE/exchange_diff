using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class MenuConfig : ActivityConfig
	{
		internal MenuConfig(ActivityManagerConfig manager) : base(manager)
		{
			this.promptConfigGroups = new Dictionary<string, ArrayList>();
			base.Prompts = new ArrayList();
			this.promptConfigGroups.Add("Main", base.Prompts);
		}

		internal int InputTimeout
		{
			get
			{
				return this.inputTimeout;
			}
		}

		internal int InitialSilenceTimeout
		{
			get
			{
				return this.initialSilenceTimeout;
			}
		}

		internal int InterDigitTimeout
		{
			get
			{
				return this.interDigitTimeout;
			}
		}

		internal byte MinDtmfSize
		{
			get
			{
				return MenuConfig.DtmfSizeToByte(this.minDtmfSize);
			}
		}

		internal uint MaxNumeric
		{
			get
			{
				return this.maxNumeric;
			}
		}

		internal uint MinNumeric
		{
			get
			{
				return this.minNumeric;
			}
		}

		internal string DtmfInputValue
		{
			get
			{
				return this.dtmfInputValue;
			}
		}

		internal string DtmfStopTones
		{
			get
			{
				return this.dtmfStopTones;
			}
		}

		internal bool Uninterruptible
		{
			get
			{
				return this.uninterruptible;
			}
		}

		internal bool StopPromptOnBargeIn
		{
			get
			{
				return this.stopPromptOnBargeIn;
			}
		}

		protected internal byte MaxDtmfSize
		{
			get
			{
				return MenuConfig.DtmfSizeToByte(this.maxDtmfSize);
			}
			protected set
			{
				this.maxDtmfSize = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public bool KeepDtmfOnNoMatch { get; private set; }

		protected Dictionary<string, ArrayList> PromptConfigGroups
		{
			get
			{
				return this.promptConfigGroups;
			}
			set
			{
				this.promptConfigGroups = value;
			}
		}

		internal override ActivityBase CreateActivity(ActivityManager manager)
		{
			return new Menu(manager, this);
		}

		internal virtual ArrayList GetPrompts(ActivityManager manager, string promptGroupName, CultureInfo culture, IPromptCounter counter)
		{
			ArrayList promptConfigArray = this.promptConfigGroups[promptGroupName];
			return PromptConfigBase.BuildConditionalPrompts(promptConfigArray, manager, culture, counter);
		}

		protected override void LoadAttributes(XmlNode rootNode)
		{
			base.LoadAttributes(rootNode);
			this.maxDtmfSize = rootNode.Attributes["maxDtmfSize"].Value;
			this.minDtmfSize = rootNode.Attributes["minDtmfSize"].Value;
			this.dtmfInputValue = string.Intern(rootNode.Attributes["dtmfInputValue"].Value);
			this.dtmfStopTones = rootNode.Attributes["dtmfStopTones"].Value;
			this.maxNumeric = uint.Parse(rootNode.Attributes["maxNumericInput"].Value, CultureInfo.InvariantCulture);
			this.minNumeric = uint.Parse(rootNode.Attributes["minNumericInput"].Value, CultureInfo.InvariantCulture);
			this.uninterruptible = bool.Parse(rootNode.Attributes["uninterruptible"].Value);
			this.stopPromptOnBargeIn = bool.Parse(rootNode.Attributes["stopPromptOnBargeIn"].Value);
			this.KeepDtmfOnNoMatch = bool.Parse(rootNode.Attributes["keepDtmfOnNoMatch"].Value);
			if (rootNode.Attributes["interDigitTimeout"] != null)
			{
				this.interDigitTimeout = int.Parse(rootNode.Attributes["interDigitTimeout"].Value, CultureInfo.InvariantCulture);
			}
			else
			{
				this.interDigitTimeout = 1;
			}
			if (rootNode.Attributes["inputTimeout"] != null)
			{
				this.inputTimeout = int.Parse(rootNode.Attributes["inputTimeout"].Value, CultureInfo.InvariantCulture);
			}
			else
			{
				this.inputTimeout = 10;
			}
			if (rootNode.Attributes["initialSilenceTimeout"] != null)
			{
				this.initialSilenceTimeout = int.Parse(rootNode.Attributes["initialSilenceTimeout"].Value, CultureInfo.InvariantCulture);
			}
			else
			{
				this.initialSilenceTimeout = 6;
			}
			if (this.initialSilenceTimeout > this.inputTimeout)
			{
				this.initialSilenceTimeout = this.inputTimeout;
			}
		}

		protected override void LoadChild(string name, XmlNode node)
		{
			if (string.Equals(name, "Prompt", StringComparison.OrdinalIgnoreCase))
			{
				string a = string.Intern(node.Attributes["type"].Value);
				if (string.Equals(a, "group", StringComparison.OrdinalIgnoreCase))
				{
					string value = node.Attributes["name"].Value;
					using (IEnumerator enumerator = this.promptConfigGroups[value].GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							PromptConfigBase promptConfigBase = (PromptConfigBase)obj;
							PromptConfigBase promptConfigBase2 = (PromptConfigBase)promptConfigBase.Clone();
							string scope = ActivityConfig.BuildCondition(base.ConditionScope, node);
							promptConfigBase2.SetConditionString(ActivityConfig.BuildCondition(scope, promptConfigBase.GetConditionString()), base.ManagerConfig);
							base.Prompts.Add(promptConfigBase2);
						}
						return;
					}
				}
				base.Prompts.Add(PromptConfigBase.Create(node, base.ConditionScope, base.ManagerConfig));
				return;
			}
			if (string.Equals(name, "PromptGroup", StringComparison.OrdinalIgnoreCase))
			{
				string value2 = node.Attributes["name"].Value;
				ArrayList prompts = base.Prompts;
				base.Prompts = new ArrayList();
				this.promptConfigGroups.Add(value2, base.Prompts);
				this.LoadChildren(node);
				base.Prompts = prompts;
				return;
			}
			base.LoadChild(name, node);
		}

		protected override void LoadComplete()
		{
			if (this.inputTimeout < this.interDigitTimeout)
			{
				throw new FsmConfigurationException(Strings.InputTimeoutLessThanInterdigit(base.ActivityId));
			}
			if (this.MinDtmfSize > this.MaxDtmfSize)
			{
				throw new FsmConfigurationException(Strings.MinDtmfGreaterThanMax(base.ActivityId));
			}
			if (this.minNumeric > this.maxNumeric)
			{
				throw new FsmConfigurationException(Strings.MinNumericGreaterThanMax(base.ActivityId));
			}
			if (this.MinDtmfSize == 0 && !ActivityConfig.TransitionMap.ContainsKey(ActivityConfig.BuildTransitionMapKey(this, "noKey")))
			{
				CallIdTracer.TraceError(ExTraceGlobals.StateMachineTracer, this, "Activity id={0} has minDtmfSize=0 without a NoKey Transition.", new object[]
				{
					base.ActivityId
				});
				throw new FsmConfigurationException(Strings.MinDtmfZeroWithoutNoKey(base.ActivityId));
			}
			if (this.MinDtmfSize != 0 && ActivityConfig.TransitionMap.ContainsKey(ActivityConfig.BuildTransitionMapKey(this, "noKey")))
			{
				CallIdTracer.TraceError(ExTraceGlobals.StateMachineTracer, this, "Activity id={0} has minDtmfSize != 0 with a NoKey Transition.", new object[]
				{
					base.ActivityId
				});
				throw new FsmConfigurationException(Strings.MinDtmfNotZeroWithNoKey(base.ActivityId));
			}
			base.LoadComplete();
		}

		private static byte DtmfSizeToByte(string dtmfSize)
		{
			if (dtmfSize != null)
			{
				if (dtmfSize == "extension")
				{
					return 16;
				}
				if (dtmfSize == "password")
				{
					return 25;
				}
				if (dtmfSize == "name")
				{
					return 76;
				}
			}
			return byte.Parse(dtmfSize, CultureInfo.InvariantCulture);
		}

		private uint maxNumeric;

		private uint minNumeric;

		private string maxDtmfSize;

		private string minDtmfSize;

		private string dtmfInputValue;

		private string dtmfStopTones;

		private int interDigitTimeout;

		private int inputTimeout;

		private int initialSilenceTimeout;

		private bool uninterruptible;

		private bool stopPromptOnBargeIn;

		private Dictionary<string, ArrayList> promptConfigGroups;
	}
}
