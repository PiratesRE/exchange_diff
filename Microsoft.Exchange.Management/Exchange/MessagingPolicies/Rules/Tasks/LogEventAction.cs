using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class LogEventAction : TransportRuleAction, IEquatable<LogEventAction>
	{
		public override int GetHashCode()
		{
			return this.EventMessage.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as LogEventAction)));
		}

		public bool Equals(LogEventAction other)
		{
			return this.EventMessage.Equals(other.EventMessage);
		}

		[ActionParameterName("LogEventText")]
		[LocDisplayName(RulesTasksStrings.IDs.EventMessageDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.EventMessageDescription)]
		public EventLogText EventMessage
		{
			get
			{
				return this.eventMessage;
			}
			set
			{
				this.eventMessage = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionLogEvent(this.EventMessage.ToString());
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "LogEvent")
			{
				return null;
			}
			LogEventAction logEventAction = new LogEventAction();
			try
			{
				logEventAction.EventMessage = new EventLogText(TransportRuleAction.GetStringValue(action.Arguments[0]));
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
			return logEventAction;
		}

		internal override void Reset()
		{
			this.eventMessage = EventLogText.Empty;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.EventMessage == EventLogText.Empty)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			int index;
			if (!Utils.CheckIsUnicodeStringWellFormed(this.eventMessage.Value, out index))
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.CommentsHaveInvalidChars((int)this.eventMessage.Value[index]), base.Name));
				return;
			}
			base.ValidateRead(errors);
		}

		internal override Action ToInternalAction()
		{
			return TransportRuleParser.Instance.CreateAction("LogEvent", new ShortList<Argument>
			{
				new Value(this.EventMessage.ToString())
			}, Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return Utils.QuoteCmdletParameter(this.EventMessage.ToString());
		}

		internal override void SuppressPiiData()
		{
			this.EventMessage = SuppressingPiiProperty.TryRedactValue<EventLogText>(RuleSchema.LogEventText, this.EventMessage);
		}

		private EventLogText eventMessage;
	}
}
