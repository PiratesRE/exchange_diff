using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class RejectMessageAction : TransportRuleAction, IEquatable<RejectMessageAction>
	{
		public override int GetHashCode()
		{
			return this.RejectReason.GetHashCode() ^ this.EnhancedStatusCode.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as RejectMessageAction)));
		}

		public bool Equals(RejectMessageAction other)
		{
			return this.RejectReason.Equals(other.RejectReason) && this.EnhancedStatusCode.Equals(other.EnhancedStatusCode);
		}

		[ActionParameterName("RejectMessageReasonText")]
		[LocDisplayName(RulesTasksStrings.IDs.RejectReasonDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.RejectReasonDescription)]
		public DsnText RejectReason
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

		[ActionParameterName("RejectMessageEnhancedStatusCode")]
		[LocDisplayName(RulesTasksStrings.IDs.EnhancedStatusCodeDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.EnhancedStatusCodeDescription)]
		public RejectEnhancedStatus EnhancedStatusCode
		{
			get
			{
				return this.enhancedStatusCode;
			}
			set
			{
				this.enhancedStatusCode = value;
			}
		}

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionRejectMessage(this.RejectReason.ToString(), this.EnhancedStatusCode.ToString());
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "RejectMessage" || !TransportRuleAction.GetStringValue(action.Arguments[0]).Equals("550"))
			{
				return null;
			}
			RejectMessageAction rejectMessageAction = new RejectMessageAction();
			try
			{
				rejectMessageAction.RejectReason = new DsnText(TransportRuleAction.GetStringValue(action.Arguments[2]));
				rejectMessageAction.EnhancedStatusCode = new RejectEnhancedStatus(TransportRuleAction.GetStringValue(action.Arguments[1]));
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
			catch (ArgumentException)
			{
				return null;
			}
			return rejectMessageAction;
		}

		internal override void Reset()
		{
			this.rejectReason = new DsnText(RejectMessageAction.defaultRejectText.Value);
			this.enhancedStatusCode = RejectMessageAction.defaultEnhancedStatusCode;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (string.IsNullOrEmpty(this.RejectReason.Value) || this.EnhancedStatusCode == RejectEnhancedStatus.Empty)
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
				new Value("550"),
				new Value(this.EnhancedStatusCode.ToString()),
				new Value(this.RejectReason.Value)
			};
			return TransportRuleParser.Instance.CreateAction("RejectMessage", arguments, Utils.GetActionName(this));
		}

		internal override string ToCmdletParameter()
		{
			return string.Format("-{0} {1} -{2} {3}", new object[]
			{
				"RejectMessageReasonText",
				Utils.QuoteCmdletParameter(this.RejectReason.Value),
				"RejectMessageEnhancedStatusCode",
				Utils.QuoteCmdletParameter(this.EnhancedStatusCode.ToString())
			});
		}

		internal override void SuppressPiiData()
		{
			this.RejectReason = SuppressingPiiProperty.TryRedactValue<DsnText>(RuleSchema.RejectMessageReasonText, this.RejectReason);
		}

		private const string StatusCode = "550";

		private static readonly DsnText defaultRejectText = new DsnText("Delivery not authorized, message refused");

		private static readonly RejectEnhancedStatus defaultEnhancedStatusCode = new RejectEnhancedStatus("5.7.1");

		private DsnText rejectReason;

		private RejectEnhancedStatus enhancedStatusCode;
	}
}
