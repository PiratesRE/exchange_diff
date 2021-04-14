using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class ActivityConfig
	{
		internal ActivityConfig(ActivityManagerConfig configManager)
		{
			this.manager = configManager;
			this.prompts = new ArrayList(16);
			this.conditionalStopPatterns = new StopPatterns();
			this.invariantStopPatterns = new StopPatterns();
		}

		private ActivityConfig()
		{
		}

		internal string ActivityId
		{
			get
			{
				return this.activityId;
			}
		}

		internal string UniqueId
		{
			get
			{
				return this.uniqueId;
			}
		}

		internal ActivityManagerConfig ManagerConfig
		{
			get
			{
				return this.manager;
			}
		}

		protected static Dictionary<string, List<TransitionBase>> TransitionMap
		{
			get
			{
				return ActivityConfig.transitionMap;
			}
			set
			{
				ActivityConfig.transitionMap = value;
			}
		}

		protected StopPatterns InvariantStopPatterns
		{
			get
			{
				return this.invariantStopPatterns;
			}
		}

		protected ArrayList Prompts
		{
			get
			{
				return this.prompts;
			}
			set
			{
				this.prompts = value;
			}
		}

		protected string ConditionScope
		{
			get
			{
				if (this.conditionScope == null)
				{
					return string.Empty;
				}
				return this.conditionScope;
			}
		}

		private ArrayList ConditionStack
		{
			get
			{
				if (this.conditionStack == null)
				{
					this.conditionStack = new ArrayList();
				}
				return this.conditionStack;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type: {0}, ID: {1}", new object[]
			{
				base.GetType().ToString(),
				this.ActivityId
			});
		}

		internal static string BuildCondition(string scope, XmlNode localNode)
		{
			string localCondition = string.Empty;
			if (localNode.Attributes["condition"] != null)
			{
				localCondition = localNode.Attributes["condition"].Value.Trim();
			}
			return ActivityConfig.BuildCondition(scope, localCondition);
		}

		internal static string BuildCondition(string scope, string localCondition)
		{
			string text;
			if (scope.Length == 0)
			{
				text = localCondition;
			}
			else if (localCondition.Length == 0)
			{
				text = scope;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(scope);
				stringBuilder.Append(" ");
				stringBuilder.Append(ConditionParser.AndOperator);
				stringBuilder.Append(" ");
				stringBuilder.Append("(");
				stringBuilder.Append(localCondition);
				stringBuilder.Append(")");
				text = stringBuilder.ToString();
			}
			return text.Trim();
		}

		internal StopPatterns ComputeStopPatterns(ActivityManager m)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Computing stop patterns for Activity {0}.", new object[]
			{
				this
			});
			StopPatterns stopPatterns = null;
			foreach (string text in this.conditionalStopPatterns)
			{
				TransitionBase transition = this.GetTransition(text, m);
				if (transition != null)
				{
					if (stopPatterns == null)
					{
						stopPatterns = new StopPatterns(this.conditionalStopPatterns.Count + this.invariantStopPatterns.Count);
					}
					stopPatterns.Add(text, transition.BargeIn);
				}
			}
			if (stopPatterns == null)
			{
				return this.invariantStopPatterns;
			}
			stopPatterns.Add(this.invariantStopPatterns);
			return stopPatterns;
		}

		internal virtual void Load(XmlNode rootNode)
		{
			this.LoadAttributes(rootNode);
			this.LoadChildren(rootNode);
			this.LoadComplete();
		}

		internal virtual TransitionBase GetTransition(string rawInput, ActivityManager manager)
		{
			PIIMessage data = PIIMessage.Create(PIIType._PII, rawInput);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, data, "Getting transition for input _PII in Activity {0}.", new object[]
			{
				this
			});
			TransitionBase result = null;
			if (rawInput == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Null input in ActivityConfig.GetTransition... returning null.", new object[0]);
				result = null;
			}
			else if (string.Equals(rawInput, "stopEvent", StringComparison.OrdinalIgnoreCase))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Stop transition found in activityconfig.GetTransition().", new object[0]);
				result = new StopTransition();
			}
			else
			{
				string key = ActivityConfig.BuildTransitionMapKey(this, rawInput);
				List<TransitionBase> list;
				if (ActivityConfig.TransitionMap.TryGetValue(key, out list))
				{
					for (int i = 0; i < list.Count; i++)
					{
						TransitionBase transitionBase = list[i];
						ExpressionParser.Expression condition = transitionBase.Condition;
						object obj;
						if (condition == null || ((obj = condition.Eval(manager)) is bool && (bool)obj))
						{
							result = transitionBase;
							break;
						}
					}
				}
			}
			return result;
		}

		internal abstract ActivityBase CreateActivity(ActivityManager manager);

		protected static string BuildTransitionMapKey(ActivityConfig activity, string eventName)
		{
			return activity.UniqueId + "::" + eventName;
		}

		protected void PushCondition(string condition)
		{
			this.ConditionStack.Add(condition);
			this.RebuildConditionScope();
		}

		protected void PopCondition()
		{
			this.ConditionStack.RemoveAt(this.ConditionStack.Count - 1);
			this.RebuildConditionScope();
		}

		protected void ParseTransitionNode(XmlNode nodeCurrent, ActivityManagerConfig managerConfig)
		{
			string text = string.Intern(nodeCurrent.Attributes["event"].Value);
			string text2 = string.Intern(nodeCurrent.Attributes["refId"].Value);
			string action = string.Intern(nodeCurrent.Attributes["action"].Value);
			bool heavy = false;
			if (nodeCurrent.Attributes["heavyaction"] != null)
			{
				heavy = bool.Parse(nodeCurrent.Attributes["heavyaction"].Value);
			}
			bool playback = false;
			if (nodeCurrent.Attributes["playbackAction"] != null)
			{
				playback = bool.Parse(nodeCurrent.Attributes["playbackAction"].Value);
			}
			string refInfo = null;
			if (nodeCurrent.Attributes["refInfo"] != null)
			{
				refInfo = string.Intern(nodeCurrent.Attributes["refInfo"].Value);
			}
			FsmAction action2 = this.ParseAction(action);
			if (!ConstantValidator.Instance.ValidateEvent(text))
			{
				throw new FsmConfigurationException(Strings.InvalidEvent(text));
			}
			TransitionBase transitionBase = null;
			try
			{
				string text3 = ActivityConfig.BuildCondition(this.ConditionScope, nodeCurrent);
				ExpressionParser.Expression condition = null;
				if (text3.Length > 0)
				{
					condition = ConditionParser.Instance.Parse(text3, managerConfig);
				}
				transitionBase = TransitionBase.Create(action2, text2, text, heavy, playback, managerConfig, condition, refInfo);
			}
			catch (FsmConfigurationException innerException)
			{
				throw new FsmConfigurationException(Strings.UnknownTransitionId(text2, this.ActivityId), innerException);
			}
			string key = ActivityConfig.BuildTransitionMapKey(this, text);
			List<TransitionBase> list;
			if (!ActivityConfig.TransitionMap.TryGetValue(key, out list))
			{
				list = new List<TransitionBase>();
				ActivityConfig.TransitionMap[key] = list;
				if (transitionBase.Condition != null)
				{
					this.conditionalStopPatterns.Add(text, transitionBase.BargeIn);
				}
				else
				{
					this.invariantStopPatterns.Add(text, transitionBase.BargeIn);
				}
			}
			else
			{
				for (int i = 0; i < list.Count; i++)
				{
					TransitionBase transitionBase2 = list[i];
					if (transitionBase2.Condition == transitionBase.Condition)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Duplicate event transition: {0}.", new object[]
						{
							text
						});
						throw new FsmConfigurationException(Strings.DuplicateTransition(this.ActivityId, text));
					}
				}
			}
			list.Add(transitionBase);
		}

		protected virtual void ParseConditionNode(XmlNode nodeCurrent)
		{
			string condition = string.Intern("(" + nodeCurrent.Attributes["value"].Value + ")");
			this.PushCondition(condition);
			this.LoadChildren(nodeCurrent);
			this.PopCondition();
		}

		protected virtual void LoadChildren(XmlNode rootNode)
		{
			foreach (object obj in rootNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string name = string.Intern(xmlNode.Name);
				this.LoadChild(name, xmlNode);
			}
		}

		protected virtual void LoadAttributes(XmlNode rootNode)
		{
			this.activityId = string.Intern(rootNode.Attributes["id"].Value);
			if (this.ManagerConfig != null)
			{
				this.uniqueId = this.ManagerConfig.UniqueId + this.ActivityId;
				return;
			}
			this.uniqueId = this.ActivityId;
		}

		protected virtual void LoadChild(string name, XmlNode node)
		{
			if (string.Equals(name, "Transition", StringComparison.OrdinalIgnoreCase))
			{
				this.ParseTransitionNode(node, this.manager);
				return;
			}
			if (string.Equals(name, "Condition", StringComparison.OrdinalIgnoreCase))
			{
				this.ParseConditionNode(node);
			}
		}

		protected virtual void LoadComplete()
		{
		}

		private void RebuildConditionScope()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.conditionStack.Count; i++)
			{
				stringBuilder.Append((string)this.conditionStack[i]);
				stringBuilder.Append(" ");
				if (i < this.conditionStack.Count - 1)
				{
					stringBuilder.Append(ConditionParser.AndOperator);
					stringBuilder.Append(" ");
				}
			}
			this.conditionScope = stringBuilder.ToString();
		}

		private FsmAction ParseAction(string action)
		{
			if (string.Compare("null", action, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return null;
			}
			ActivityManagerConfig activityManagerConfig = this as ActivityManagerConfig;
			if (activityManagerConfig == null)
			{
				activityManagerConfig = this.ManagerConfig;
			}
			QualifiedName actionName = new QualifiedName(action, activityManagerConfig.ClassName);
			return FsmAction.Create(actionName, activityManagerConfig);
		}

		private static Dictionary<string, List<TransitionBase>> transitionMap = new Dictionary<string, List<TransitionBase>>();

		private string activityId;

		private string uniqueId;

		private ArrayList prompts;

		private ActivityManagerConfig manager;

		private ArrayList conditionStack;

		private string conditionScope;

		private StopPatterns conditionalStopPatterns;

		private StopPatterns invariantStopPatterns;
	}
}
