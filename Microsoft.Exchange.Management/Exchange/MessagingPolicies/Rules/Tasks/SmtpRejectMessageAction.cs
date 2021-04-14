using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class SmtpRejectMessageAction : TransportRuleAction, IEquatable<SmtpRejectMessageAction>
	{
		public override int GetHashCode()
		{
			return this.StatusCode.GetHashCode() ^ this.RejectReason.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as SmtpRejectMessageAction)));
		}

		public bool Equals(SmtpRejectMessageAction other)
		{
			return this.StatusCode.Equals(other.StatusCode) && this.RejectReason.Equals(other.RejectReason);
		}

		[ActionParameterName("SmtpRejectMessageRejectStatusCode")]
		[LocDescription(RulesTasksStrings.IDs.StatusCodeDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.StatusCodeDisplayName)]
		public RejectStatusCode StatusCode
		{
			get
			{
				return this.statusCode;
			}
			set
			{
				this.statusCode = value;
			}
		}

		[ActionParameterName("SmtpRejectMessageRejectText")]
		[LocDisplayName(RulesTasksStrings.IDs.RejectReasonDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.RejectReasonDescription)]
		public RejectText RejectReason
		{
			get
			{
				return this.rejectReason;
			}
			set
			{
				this.rejectReason = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionRejectMessage(this.RejectReason.ToString(), this.StatusCode.ToString());
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "RejectMessage" || !TransportRuleAction.GetStringValue(action.Arguments[1]).Equals(SmtpRejectMessageAction.EnhancedStatusCode))
			{
				return null;
			}
			SmtpRejectMessageAction smtpRejectMessageAction = new SmtpRejectMessageAction();
			try
			{
				smtpRejectMessageAction.StatusCode = new RejectStatusCode(TransportRuleAction.GetStringValue(action.Arguments[0]));
				smtpRejectMessageAction.RejectReason = new RejectText(TransportRuleAction.GetStringValue(action.Arguments[2]));
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
			return smtpRejectMessageAction;
		}

		internal override void Reset()
		{
			this.statusCode = SmtpRejectMessageAction.defaultStatusCode;
			this.rejectReason = SmtpRejectMessageAction.defaultRejectText;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.RejectReason == RejectText.Empty || this.StatusCode == RejectStatusCode.Empty)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			base.ValidateRead(errors);
		}

		internal override Action ToInternalAction()
		{
			ShortList<Argument> arguments = new ShortList<Argument>
			{
				new Value(this.StatusCode.ToString()),
				new Value(SmtpRejectMessageAction.EnhancedStatusCode),
				new Value(this.RejectReason.ToString())
			};
			return TransportRuleParser.Instance.CreateAction("RejectMessage", arguments, Utils.GetActionName(this));
		}

		internal override string ToCmdletParameter()
		{
			return string.Format("-{0} {1} -{2} {3}", new object[]
			{
				"SmtpRejectMessageRejectText",
				Utils.QuoteCmdletParameter(this.RejectReason.ToString()),
				"SmtpRejectMessageRejectStatusCode",
				this.StatusCode.ToString()
			});
		}

		internal override void SuppressPiiData()
		{
			this.RejectReason = SuppressingPiiProperty.TryRedactValue<RejectText>(RuleSchema.SmtpRejectMessageRejectText, this.RejectReason);
		}

		private static readonly string EnhancedStatusCode = string.Empty;

		private static readonly RejectStatusCode defaultStatusCode = new RejectStatusCode("550");

		private static readonly RejectText defaultRejectText = new RejectText("Delivery not authorized, message refused");

		private RejectStatusCode statusCode;

		private RejectText rejectReason;
	}
}
