using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class GenerateIncidentReportAction : TransportRuleAction, IEquatable<GenerateIncidentReportAction>
	{
		public override int GetHashCode()
		{
			return this.ReportDestination.GetHashCode() ^ this.IncidentReportOriginalMail.GetHashCode() ^ Utils.GetHashCodeForArray<IncidentReportContent>(this.IncidentReportContent);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as GenerateIncidentReportAction)));
		}

		public bool Equals(GenerateIncidentReportAction other)
		{
			return this.ReportDestination.Equals(other.ReportDestination) && this.IncidentReportOriginalMail.Equals(other.IncidentReportOriginalMail) && this.IncidentReportContent.Length == other.IncidentReportContent.Length && !this.IncidentReportContent.Except(other.IncidentReportContent).Any<IncidentReportContent>() && !other.IncidentReportContent.Except(this.IncidentReportContent).Any<IncidentReportContent>();
		}

		[LocDescription(RulesTasksStrings.IDs.ReportDestinationDescription)]
		[LocDisplayName(RulesTasksStrings.IDs.ReportDestinationDisplayName)]
		[ActionParameterName("GenerateIncidentReport")]
		public SmtpAddress ReportDestination { get; set; }

		[LocDisplayName(RulesTasksStrings.IDs.IncidentReportOriginalMailnDisplayName)]
		[ActionParameterName("IncidentReportOriginalMail")]
		[LocDescription(RulesTasksStrings.IDs.IncidentReportOriginalMailDescription)]
		public IncidentReportOriginalMail IncidentReportOriginalMail { get; set; }

		[ActionParameterName("IncidentReportContent")]
		[LocDisplayName(RulesTasksStrings.IDs.IncidentReportContentDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.IncidentReportContentDescription)]
		public IncidentReportContent[] IncidentReportContent
		{
			get
			{
				return this.incidentReportContent.ToArray();
			}
			set
			{
				this.incidentReportContent = value.ToList<IncidentReportContent>();
			}
		}

		private bool AddIncidentReportContentItem(IncidentReportContent contentItem)
		{
			if (this.incidentReportContent.Contains(contentItem))
			{
				return false;
			}
			this.incidentReportContent.Add(contentItem);
			return true;
		}

		internal override string Description
		{
			get
			{
				string includeOriginalMail = string.Join(GenerateIncidentReportAction.IncidentReportContentDescriptionSeparator, (from item in this.IncidentReportContent
				select LocalizedDescriptionAttribute.FromEnum(typeof(IncidentReportContent), item)).ToArray<string>());
				return RulesTasksStrings.RuleDescriptionGenerateIncidentReport(this.ReportDestination.ToString(), includeOriginalMail);
			}
		}

		public GenerateIncidentReportAction()
		{
			this.Reset();
		}

		internal static TransportRuleAction CreateFromInternalAction(Action action)
		{
			if (action.Name != "GenerateIncidentReport" || action.Arguments.Count == 0)
			{
				return null;
			}
			GenerateIncidentReportAction generateIncidentReportAction = new GenerateIncidentReportAction();
			string stringValue = TransportRuleAction.GetStringValue(action.Arguments[0]);
			if (!SmtpAddress.IsValidSmtpAddress(stringValue))
			{
				return null;
			}
			generateIncidentReportAction.ReportDestination = new SmtpAddress(stringValue);
			IncidentReportOriginalMail legacyReportOriginalMail;
			if (action.Arguments.Count == 2 && GenerateIncidentReport.TryGetIncidentReportOriginalMailParameter(TransportRuleAction.GetStringValue(action.Arguments[1]), out legacyReportOriginalMail))
			{
				generateIncidentReportAction.IncidentReportContent = GenerateIncidentReport.GetLegacyContentItems(legacyReportOriginalMail).ToArray();
			}
			else
			{
				for (int i = 1; i < action.Arguments.Count; i++)
				{
					IncidentReportContent contentItem;
					if (Enum.TryParse<IncidentReportContent>(TransportRuleAction.GetStringValue(action.Arguments[i]), out contentItem))
					{
						generateIncidentReportAction.AddIncidentReportContentItem(contentItem);
					}
				}
				if (generateIncidentReportAction.IncidentReportContent.Length == 0)
				{
					generateIncidentReportAction.AddIncidentReportContentItem(Microsoft.Exchange.MessagingPolicies.Rules.IncidentReportContent.RuleDetections);
				}
			}
			generateIncidentReportAction.IncidentReportOriginalMail = (generateIncidentReportAction.IncidentReportContent.Contains(Microsoft.Exchange.MessagingPolicies.Rules.IncidentReportContent.AttachOriginalMail) ? IncidentReportOriginalMail.IncludeOriginalMail : IncidentReportOriginalMail.DoNotIncludeOriginalMail);
			return generateIncidentReportAction;
		}

		internal override void Reset()
		{
			this.ReportDestination = default(SmtpAddress);
			this.IncidentReportOriginalMail = IncidentReportOriginalMail.DoNotIncludeOriginalMail;
			this.IncidentReportContent = new IncidentReportContent[0];
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.ReportDestination == SmtpAddress.Empty)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			if (this.IncidentReportOriginalMail == IncidentReportOriginalMail.DoNotIncludeOriginalMail)
			{
				if (this.IncidentReportContent.Any((IncidentReportContent reportContent) => reportContent == Microsoft.Exchange.MessagingPolicies.Rules.IncidentReportContent.AttachOriginalMail))
				{
					errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.IncidentReportConflictingParameters("IncidentReportOriginalMail", "IncidentReportContent"), base.Name));
				}
			}
			base.ValidateRead(errors);
		}

		internal override Action ToInternalAction()
		{
			ShortList<Argument> shortList = new ShortList<Argument>
			{
				new Value(this.ReportDestination.ToString())
			};
			foreach (IncidentReportContent incidentReportContent in this.IncidentReportContent)
			{
				shortList.Add(new Value(Enum.GetName(typeof(IncidentReportContent), incidentReportContent)));
			}
			if (this.IncidentReportOriginalMail == IncidentReportOriginalMail.IncludeOriginalMail && !this.IncidentReportContent.Contains(Microsoft.Exchange.MessagingPolicies.Rules.IncidentReportContent.AttachOriginalMail))
			{
				this.AddIncidentReportContentItem(Microsoft.Exchange.MessagingPolicies.Rules.IncidentReportContent.AttachOriginalMail);
				shortList.Add(new Value(Enum.GetName(typeof(IncidentReportContent), Microsoft.Exchange.MessagingPolicies.Rules.IncidentReportContent.AttachOriginalMail)));
			}
			return TransportRuleParser.Instance.CreateAction("GenerateIncidentReport", shortList, null);
		}

		internal override string ToCmdletParameter()
		{
			string text = string.Empty;
			if (this.incidentReportContent.Any<IncidentReportContent>())
			{
				text = string.Format("-{0} ", "IncidentReportContent");
				text += string.Join(",", (from item in this.IncidentReportContent
				select Enum.GetName(typeof(IncidentReportContent), item)).ToArray<string>());
			}
			return string.Format("-{0} {1} {2}", "GenerateIncidentReport", Utils.QuoteCmdletParameter(this.ReportDestination.ToString()), text);
		}

		internal override void SuppressPiiData()
		{
			string text;
			string text2;
			this.ReportDestination = SuppressingPiiData.Redact(this.ReportDestination, out text, out text2);
		}

		private static readonly string IncidentReportContentDescriptionSeparator = ", ";

		private List<IncidentReportContent> incidentReportContent = new List<IncidentReportContent>();
	}
}
