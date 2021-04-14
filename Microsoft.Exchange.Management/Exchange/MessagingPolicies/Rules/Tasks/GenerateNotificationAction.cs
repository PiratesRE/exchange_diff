using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class GenerateNotificationAction : TransportRuleAction, IEquatable<GenerateNotificationAction>
	{
		public GenerateNotificationAction()
		{
		}

		internal GenerateNotificationAction(DisclaimerText notificationContent)
		{
			this.NotificationContent = notificationContent;
		}

		public override int GetHashCode()
		{
			return this.NotificationContent.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as GenerateNotificationAction)));
		}

		public bool Equals(GenerateNotificationAction other)
		{
			return this.NotificationContent.Equals(other.NotificationContent);
		}

		[LocDescription(RulesTasksStrings.IDs.GenerateNotificationDescription)]
		[ActionParameterName("GenerateNotification")]
		[LocDisplayName(RulesTasksStrings.IDs.GenerateNotificationDisplayName)]
		public DisclaimerText NotificationContent { get; set; }

		internal override string Description
		{
			get
			{
				return RulesTasksStrings.RuleDescriptionGenerateNotification(this.NotificationContent.Value.ToString());
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "GenerateNotification" || !action.Arguments.Any<Argument>())
			{
				return null;
			}
			TransportRuleAction result;
			try
			{
				result = new GenerateNotificationAction(new DisclaimerText(TransportRuleAction.GetStringValue(action.Arguments[0])));
			}
			catch (ArgumentOutOfRangeException)
			{
				result = null;
			}
			catch (ArgumentException)
			{
				result = null;
			}
			return result;
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (string.IsNullOrWhiteSpace(this.NotificationContent.ToString()))
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
				new Value(this.NotificationContent.ToString())
			};
			return TransportRuleParser.Instance.CreateAction("GenerateNotification", arguments, Utils.GetActionName(this));
		}

		internal override string ToCmdletParameter()
		{
			return string.Format("-{0} {1}", "GenerateNotification", Utils.QuoteCmdletParameter(this.NotificationContent.ToString()));
		}

		internal override void SuppressPiiData()
		{
			this.NotificationContent = SuppressingPiiProperty.TryRedactValue<DisclaimerText>(RuleSchema.GenerateNotification, this.NotificationContent);
		}
	}
}
