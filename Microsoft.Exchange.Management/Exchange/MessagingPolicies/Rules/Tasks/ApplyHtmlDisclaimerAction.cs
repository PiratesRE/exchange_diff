using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class ApplyHtmlDisclaimerAction : TransportRuleAction, IEquatable<ApplyHtmlDisclaimerAction>
	{
		public override int GetHashCode()
		{
			return this.Location.GetHashCode() ^ this.Text.GetHashCode() ^ this.FallbackAction.GetHashCode();
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as ApplyHtmlDisclaimerAction)));
		}

		public bool Equals(ApplyHtmlDisclaimerAction other)
		{
			return this.Location.Equals(other.Location) && this.Text.Equals(other.Text) && this.FallbackAction.Equals(other.FallbackAction);
		}

		[LocDisplayName(RulesTasksStrings.IDs.DisclaimerLocationDisplayName)]
		[ActionParameterName("ApplyHtmlDisclaimerLocation")]
		[LocDescription(RulesTasksStrings.IDs.DisclaimerLocationDescription)]
		public DisclaimerLocation Location
		{
			get
			{
				return this.disclaimerLocation;
			}
			set
			{
				this.disclaimerLocation = value;
			}
		}

		[ActionParameterName("ApplyHtmlDisclaimerText")]
		[LocDisplayName(RulesTasksStrings.IDs.DisclaimerTextDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.DisclaimerTextDescription)]
		public DisclaimerText Text
		{
			get
			{
				return this.disclaimerText;
			}
			set
			{
				this.disclaimerText = value;
			}
		}

		[ActionParameterName("ApplyHtmlDisclaimerFallbackAction")]
		[LocDescription(RulesTasksStrings.IDs.FallbackActionDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.FallbackActionDisplayName)]
		public DisclaimerFallbackAction FallbackAction
		{
			get
			{
				return this.fallbackAction;
			}
			set
			{
				this.fallbackAction = value;
			}
		}

		internal override string Description
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(200);
				if (this.Location == DisclaimerLocation.Prepend)
				{
					stringBuilder.Append(RulesTasksStrings.RuleDescriptionPrependHtmlDisclaimer(this.Text.ToString()));
				}
				else
				{
					stringBuilder.Append(RulesTasksStrings.RuleDescriptionApplyHtmlDisclaimer(this.Text.ToString()));
				}
				if (this.FallbackAction == DisclaimerFallbackAction.Wrap)
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(RulesTasksStrings.RuleDescriptionDisclaimerWrapFallback);
				}
				else if (this.FallbackAction == DisclaimerFallbackAction.Ignore)
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(RulesTasksStrings.RuleDescriptionDisclaimerIgnoreFallback);
				}
				else if (this.FallbackAction == DisclaimerFallbackAction.Reject)
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(RulesTasksStrings.RuleDescriptionDisclaimerRejectFallback);
				}
				return stringBuilder.ToString();
			}
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "ApplyHtmlDisclaimer")
			{
				return null;
			}
			DisclaimerLocation location;
			DisclaimerText text2;
			DisclaimerFallbackAction disclaimerFallbackAction;
			try
			{
				string text = TransportRuleAction.GetStringValue(action.Arguments[0]);
				if (text.Equals("Inline", StringComparison.InvariantCultureIgnoreCase))
				{
					text = "Append";
				}
				location = (DisclaimerLocation)Enum.Parse(typeof(DisclaimerLocation), text);
				text2 = new DisclaimerText(TransportRuleAction.GetStringValue(action.Arguments[1]));
				disclaimerFallbackAction = (DisclaimerFallbackAction)Enum.Parse(typeof(DisclaimerFallbackAction), TransportRuleAction.GetStringValue(action.Arguments[2]));
			}
			catch (ArgumentException)
			{
				return null;
			}
			catch (OverflowException)
			{
				return null;
			}
			return new ApplyHtmlDisclaimerAction
			{
				Location = location,
				Text = text2,
				FallbackAction = disclaimerFallbackAction
			};
		}

		internal override void Reset()
		{
			this.disclaimerLocation = DisclaimerLocation.Append;
			this.disclaimerText = DisclaimerText.Empty;
			this.fallbackAction = DisclaimerFallbackAction.Wrap;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.disclaimerText == DisclaimerText.Empty)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			int index;
			if (!Utils.CheckIsUnicodeStringWellFormed(this.disclaimerText.Value, out index))
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.CommentsHaveInvalidChars((int)this.disclaimerText.Value[index]), base.Name));
				return;
			}
			base.ValidateRead(errors);
		}

		internal override Action ToInternalAction()
		{
			ShortList<Argument> arguments = new ShortList<Argument>
			{
				new Value(this.disclaimerLocation.ToString()),
				new Value(this.disclaimerText.ToString()),
				new Value(this.fallbackAction.ToString())
			};
			return TransportRuleParser.Instance.CreateAction("ApplyHtmlDisclaimer", arguments, Utils.GetActionName(this));
		}

		internal override string ToCmdletParameter()
		{
			return string.Format("-{0} {1} -{2} {3} -{4} {5}", new object[]
			{
				"ApplyHtmlDisclaimerLocation",
				Enum.GetName(typeof(DisclaimerLocation), this.Location),
				"ApplyHtmlDisclaimerFallbackAction",
				Enum.GetName(typeof(DisclaimerFallbackAction), this.FallbackAction),
				"ApplyHtmlDisclaimerText",
				Utils.QuoteCmdletParameter(this.Text.ToString())
			});
		}

		internal override void SuppressPiiData()
		{
			this.Text = SuppressingPiiProperty.TryRedactValue<DisclaimerText>(RuleSchema.ApplyHtmlDisclaimerText, this.Text);
		}

		private DisclaimerLocation disclaimerLocation;

		private DisclaimerText disclaimerText;

		private DisclaimerFallbackAction fallbackAction;
	}
}
