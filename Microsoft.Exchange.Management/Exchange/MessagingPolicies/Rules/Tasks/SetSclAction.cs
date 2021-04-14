using System;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SetSclAction : TransportRuleAction, IEquatable<SetSclAction>
	{
		public override int GetHashCode()
		{
			return this.SclValue.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SetSclAction)));
		}

		public bool Equals(SetSclAction other)
		{
			return this.SclValue.Equals(other.SclValue);
		}

		[LocDescription(RulesTasksStrings.IDs.SclValueDescription)]
		[ActionParameterName("SetSCL")]
		[LocDisplayName(RulesTasksStrings.IDs.SclValueDisplayName)]
		public SclValue SclValue
		{
			get
			{
				return this.sclValue;
			}
			set
			{
				this.sclValue = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionSetScl(this.SclValue.ToString());
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "SetHeader" || TransportRuleAction.GetStringValue(action.Arguments[0]) != "X-MS-Exchange-Organization-SCL")
			{
				return null;
			}
			int input;
			if (!int.TryParse(TransportRuleAction.GetStringValue(action.Arguments[1]), out input))
			{
				return null;
			}
			SetSclAction setSclAction = new SetSclAction();
			try
			{
				setSclAction.SclValue = new SclValue(input);
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
			return setSclAction;
		}

		internal override void Reset()
		{
			this.sclValue = new SclValue(0);
			base.Reset();
		}

		internal override Action ToInternalAction()
		{
			ShortList<Argument> arguments = new ShortList<Argument>
			{
				new Value("X-MS-Exchange-Organization-SCL"),
				new Value(this.SclValue.ToString())
			};
			return TransportRuleParser.Instance.CreateAction("SetHeader", arguments, Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return this.SclValue.ToString();
		}

		private SclValue sclValue;
	}
}
