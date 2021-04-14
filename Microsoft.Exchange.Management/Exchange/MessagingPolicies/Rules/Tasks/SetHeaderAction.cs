using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SetHeaderAction : TransportRuleAction, IEquatable<SetHeaderAction>
	{
		public override int GetHashCode()
		{
			return this.MessageHeader.GetHashCode() ^ this.HeaderValue.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SetHeaderAction)));
		}

		public bool Equals(SetHeaderAction other)
		{
			return this.MessageHeader.Equals(other.MessageHeader) && this.HeaderValue.Equals(other.HeaderValue);
		}

		[ActionParameterName("SetHeaderName")]
		[LocDescription(RulesTasksStrings.IDs.MessageHeaderDescription)]
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

		[ActionParameterName("SetHeaderValue")]
		[LocDescription(RulesTasksStrings.IDs.HeaderValueDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.HeaderValueDisplayName)]
		public HeaderValue HeaderValue
		{
			get
			{
				return this.headerValue;
			}
			set
			{
				this.headerValue = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionSetHeader(this.MessageHeader.ToString(), this.HeaderValue.ToString());
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "SetHeader")
			{
				return null;
			}
			SetHeaderAction setHeaderAction = new SetHeaderAction();
			try
			{
				setHeaderAction.MessageHeader = new HeaderName(TransportRuleAction.GetStringValue(action.Arguments[0]));
				setHeaderAction.HeaderValue = new HeaderValue(TransportRuleAction.GetStringValue(action.Arguments[1]));
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
			return setHeaderAction;
		}

		internal override void Reset()
		{
			this.messageHeader = HeaderName.Empty;
			this.headerValue = HeaderValue.Empty;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.MessageHeader == HeaderName.Empty || this.HeaderValue == HeaderValue.Empty)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			int index;
			if (!Utils.CheckIsUnicodeStringWellFormed(this.headerValue.Value, out index))
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.CommentsHaveInvalidChars((int)this.headerValue.Value[index]), base.Name));
				return;
			}
			base.ValidateRead(errors);
		}

		internal override Action ToInternalAction()
		{
			ShortList<Argument> arguments = new ShortList<Argument>
			{
				new Value(this.MessageHeader.ToString()),
				new Value(this.HeaderValue.ToString())
			};
			return TransportRuleParser.Instance.CreateAction("SetHeader", arguments, Utils.GetActionName(this));
		}

		internal override string ToCmdletParameter()
		{
			return string.Format("-{0} {1} -{2} {3}", new object[]
			{
				"SetHeaderName",
				Utils.QuoteCmdletParameter(this.MessageHeader.ToString()),
				"SetHeaderValue",
				Utils.QuoteCmdletParameter(this.HeaderValue.ToString())
			});
		}

		internal override void SuppressPiiData()
		{
			this.MessageHeader = SuppressingPiiProperty.TryRedactValue<HeaderName>(RuleSchema.SetHeaderName, this.MessageHeader);
			this.HeaderValue = SuppressingPiiProperty.TryRedactValue<HeaderValue>(RuleSchema.SetHeaderValue, this.HeaderValue);
		}

		private HeaderName messageHeader;

		private HeaderValue headerValue;
	}
}
