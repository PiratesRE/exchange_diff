using System;
using System.Collections.Generic;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class NotifySenderAction : TransportRuleAction, IEquatable<NotifySenderAction>
	{
		public NotifySenderAction()
		{
			this.Reset();
		}

		public override int GetHashCode()
		{
			return this.SenderNotificationType.GetHashCode() ^ this.RejectReason.GetHashCode() ^ this.EnhancedStatusCode.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as NotifySenderAction)));
		}

		public bool Equals(NotifySenderAction other)
		{
			return this.SenderNotificationType.Equals(other.SenderNotificationType) && this.RejectReason.Equals(other.RejectReason) && this.EnhancedStatusCode.Equals(other.EnhancedStatusCode);
		}

		[LocDisplayName(RulesTasksStrings.IDs.SenderNotificationTypeDisplayName)]
		[ActionParameterName("NotifySender")]
		[LocDescription(RulesTasksStrings.IDs.SenderNotificationTypeDescription)]
		public NotifySenderType SenderNotificationType { get; set; }

		[ActionParameterName("RejectMessageReasonText")]
		[LocDisplayName(RulesTasksStrings.IDs.RejectReasonDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.RejectReasonDescription)]
		public DsnText RejectReason { get; set; }

		[LocDescription(RulesTasksStrings.IDs.EnhancedStatusCodeDescription)]
		[ActionParameterName("RejectMessageEnhancedStatusCode")]
		[LocDisplayName(RulesTasksStrings.IDs.EnhancedStatusCodeDisplayName)]
		public RejectEnhancedStatus EnhancedStatusCode { get; set; }

		internal override string Description
		{
			get
			{
				switch (this.SenderNotificationType)
				{
				case NotifySenderType.NotifyOnly:
					return RulesTasksStrings.RuleDescriptionNotifySenderNotifyOnly;
				case NotifySenderType.RejectMessage:
					return RulesTasksStrings.RuleDescriptionNotifySenderRejectMessage(this.RejectReason.Value, this.EnhancedStatusCode.ToString());
				case NotifySenderType.RejectUnlessFalsePositiveOverride:
					return RulesTasksStrings.RuleDescriptionNotifySenderRejectUnlessFalsePositiveOverride(this.RejectReason.Value, this.EnhancedStatusCode.ToString());
				case NotifySenderType.RejectUnlessSilentOverride:
					return RulesTasksStrings.RuleDescriptionNotifySenderRejectUnlessSilentOverride(this.RejectReason.Value, this.EnhancedStatusCode.ToString());
				case NotifySenderType.RejectUnlessExplicitOverride:
					return RulesTasksStrings.RuleDescriptionNotifySenderRejectUnlessExplicitOverride(this.RejectReason.Value, this.EnhancedStatusCode.ToString());
				default:
					return string.Empty;
				}
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "SenderNotify" || !TransportRuleAction.GetStringValue(action.Arguments[1]).Equals("550"))
			{
				return null;
			}
			NotifySenderAction notifySenderAction = new NotifySenderAction();
			try
			{
				notifySenderAction.SenderNotificationType = (NotifySenderType)Enum.Parse(typeof(NotifySenderType), TransportRuleAction.GetStringValue(action.Arguments[0]));
				notifySenderAction.EnhancedStatusCode = new RejectEnhancedStatus(TransportRuleAction.GetStringValue(action.Arguments[2]));
				notifySenderAction.RejectReason = new DsnText(TransportRuleAction.GetStringValue(action.Arguments[3]));
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
			catch (ArgumentException)
			{
				return null;
			}
			catch (OverflowException)
			{
				return null;
			}
			return notifySenderAction;
		}

		internal override void Reset()
		{
			this.RejectReason = new DsnText(NotifySenderAction.defaultRejectText.Value);
			this.EnhancedStatusCode = NotifySenderAction.defaultEnhancedStatusCode;
			this.SenderNotificationType = NotifySenderType.NotifyOnly;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (string.IsNullOrWhiteSpace(this.RejectReason.Value) || this.EnhancedStatusCode == RejectEnhancedStatus.Empty)
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
				new Value(Enum.GetName(typeof(NotifySenderType), this.SenderNotificationType)),
				new Value("550"),
				new Value(this.EnhancedStatusCode.ToString()),
				new Value(this.RejectReason.ToString())
			};
			return TransportRuleParser.Instance.CreateAction("SenderNotify", arguments, Utils.GetActionName(this));
		}

		internal override string ToCmdletParameter()
		{
			return string.Format("-{0} {1} -{2} {3} -{4} {5}", new object[]
			{
				"NotifySender",
				Enum.GetName(typeof(NotifySenderType), this.SenderNotificationType),
				"RejectMessageReasonText",
				Utils.QuoteCmdletParameter(this.RejectReason.ToString()),
				"RejectMessageEnhancedStatusCode",
				Utils.QuoteCmdletParameter(this.EnhancedStatusCode.ToString())
			});
		}

		internal override void SuppressPiiData()
		{
			this.RejectReason = SuppressingPiiProperty.TryRedactValue<DsnText>(RuleSchema.RejectMessageReasonText, this.RejectReason);
		}

		private const string StatusCode = "550";

		private static readonly RejectText defaultRejectText = new RejectText("Delivery not authorized, message refused");

		private static readonly RejectEnhancedStatus defaultEnhancedStatusCode = new RejectEnhancedStatus("5.7.1");
	}
}
