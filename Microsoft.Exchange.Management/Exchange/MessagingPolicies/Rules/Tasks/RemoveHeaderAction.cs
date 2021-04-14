using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class RemoveHeaderAction : TransportRuleAction, IEquatable<RemoveHeaderAction>
	{
		public override int GetHashCode()
		{
			return this.MessageHeader.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as RemoveHeaderAction)));
		}

		public bool Equals(RemoveHeaderAction other)
		{
			return this.MessageHeader.Equals(other.MessageHeader);
		}

		[LocDescription(RulesTasksStrings.IDs.MessageHeaderDescription)]
		[ActionParameterName("RemoveHeader")]
		[LocDisplayName(RulesTasksStrings.IDs.MessageHeaderDisplayName)]
		public HeaderName MessageHeader
		{
			get
			{
				return this.messageHeader;
			}
			set
			{
				this.messageHeader = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionRemoveHeader(this.MessageHeader.ToString());
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "RemoveHeader")
			{
				return null;
			}
			RemoveHeaderAction removeHeaderAction = new RemoveHeaderAction();
			try
			{
				removeHeaderAction.MessageHeader = new HeaderName(TransportRuleAction.GetStringValue(action.Arguments[0]));
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
			return removeHeaderAction;
		}

		internal override void Reset()
		{
			this.messageHeader = HeaderName.Empty;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.MessageHeader == HeaderName.Empty)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			base.ValidateRead(errors);
		}

		internal override Action ToInternalAction()
		{
			return TransportRuleParser.Instance.CreateAction("RemoveHeader", new ShortList<Argument>
			{
				new Value(this.MessageHeader.ToString())
			}, Utils.GetActionName(this));
		}

		internal override string GetActionParameters()
		{
			return Utils.QuoteCmdletParameter(this.MessageHeader.ToString());
		}

		internal override void SuppressPiiData()
		{
			this.MessageHeader = SuppressingPiiProperty.TryRedactValue<HeaderName>(RuleSchema.RemoveHeader, this.MessageHeader);
		}

		private HeaderName messageHeader;
	}
}
