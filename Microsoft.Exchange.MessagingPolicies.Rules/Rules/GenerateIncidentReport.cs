using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class GenerateIncidentReport : TransportAction
	{
		public GenerateIncidentReport(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override TransportActionType Type
		{
			get
			{
				return TransportActionType.RecipientRelated;
			}
		}

		public override string Name
		{
			get
			{
				return "GenerateIncidentReport";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return GenerateIncidentReport.GenerateIncidentReportActionVersion;
			}
		}

		public override void ValidateArguments(ShortList<Argument> inputArguments)
		{
			if (inputArguments.Count < 1)
			{
				throw new RulesValidationException(RulesStrings.ActionArgumentMismatch(this.Name));
			}
			if (inputArguments.Any((Argument argument) => argument.Type != typeof(string)))
			{
				throw new RulesValidationException(RulesStrings.ActionArgumentMismatch(this.Name));
			}
		}

		public override bool ShouldExecute(RuleMode mode, RulesEvaluationContext context)
		{
			return true;
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			if (IncidentReport.IsIncidentReport(transportRulesEvaluationContext.MailItem.Message))
			{
				TransportRulesEvaluator.Trace(transportRulesEvaluationContext.TransportRulesTracer, transportRulesEvaluationContext.MailItem, "GenerateIncidentReport: Skipping incident report generation on an incident report message");
				return ExecutionControl.Execute;
			}
			if (TransportRulesLoopChecker.IsIncidentReportLoopCountExceeded(transportRulesEvaluationContext.MailItem))
			{
				TransportRulesEvaluator.Trace(transportRulesEvaluationContext.TransportRulesTracer, transportRulesEvaluationContext.MailItem, "GenerateIncidentReport: Message loop count limit exceeded. Skipping incident report generation");
				return ExecutionControl.Execute;
			}
			string text = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			if (!SmtpAddress.IsValidSmtpAddress(text))
			{
				string text2 = TransportRulesStrings.InvalidReportDestinationArgument(base.Arguments[0].GetValue(transportRulesEvaluationContext));
				TransportRulesEvaluator.Trace(transportRulesEvaluationContext.TransportRulesTracer, transportRulesEvaluationContext.MailItem, "GenerateIncidentReport error: " + text2);
				throw new TransportRulePermanentException(text2);
			}
			List<IncidentReportContent> list = new List<IncidentReportContent>();
			IncidentReportOriginalMail incidentReportOriginalMail;
			if (base.Arguments.Count == 2 && GenerateIncidentReport.TryGetIncidentReportOriginalMailParameter((string)base.Arguments[1].GetValue(transportRulesEvaluationContext), out incidentReportOriginalMail))
			{
				list = GenerateIncidentReport.GetLegacyContentItems(incidentReportOriginalMail);
				if (incidentReportOriginalMail == IncidentReportOriginalMail.DoNotIncludeOriginalMail)
				{
					list.Remove(IncidentReportContent.AttachOriginalMail);
				}
			}
			else
			{
				for (int i = 1; i < base.Arguments.Count; i++)
				{
					IncidentReportContent item;
					if (!Enum.TryParse<IncidentReportContent>((string)base.Arguments[i].GetValue(transportRulesEvaluationContext), out item))
					{
						TransportRulesEvaluator.Trace(transportRulesEvaluationContext.TransportRulesTracer, transportRulesEvaluationContext.MailItem, string.Format("GenerateIncidentReport: Unrecognized incident report content item '{0}'. Item is ignored.", (string)base.Arguments[i].GetValue(transportRulesEvaluationContext)));
					}
					else
					{
						list.Add(item);
					}
				}
			}
			ITransportMailItemFacade transportMailItemFacade = IncidentReport.CreateReport(text, list, transportRulesEvaluationContext);
			TransportRulesLoopChecker.StampLoopCountHeader(TransportRulesLoopChecker.GetMessageLoopCount(transportRulesEvaluationContext.MailItem) + 1, (TransportMailItem)transportMailItemFacade);
			transportMailItemFacade.CommitLazy();
			TransportFacades.TrackReceiveByAgent(transportMailItemFacade, "Transport Rule", null, new long?(transportMailItemFacade.RecordId));
			Components.CategorizerComponent.EnqueueSideEffectMessage(transportRulesEvaluationContext.MailItem, (TransportMailItem)transportMailItemFacade, "Transport Rule Agent");
			return ExecutionControl.Execute;
		}

		internal bool IsLegacyFormat(TransportRulesEvaluationContext context)
		{
			IncidentReportOriginalMail incidentReportOriginalMail;
			return base.Arguments.Count == 2 && GenerateIncidentReport.TryGetIncidentReportOriginalMailParameter((string)base.Arguments[1].GetValue(context), out incidentReportOriginalMail);
		}

		internal static bool TryGetIncidentReportOriginalMailParameter(string value, out IncidentReportOriginalMail result)
		{
			result = IncidentReportOriginalMail.DoNotIncludeOriginalMail;
			return !string.IsNullOrEmpty(value) && Enum.TryParse<IncidentReportOriginalMail>(value, out result);
		}

		internal static List<IncidentReportContent> GetLegacyContentItems(IncidentReportOriginalMail legacyReportOriginalMail)
		{
			List<IncidentReportContent> list = new List<IncidentReportContent>
			{
				IncidentReportContent.Sender,
				IncidentReportContent.Recipients,
				IncidentReportContent.Subject,
				IncidentReportContent.Cc,
				IncidentReportContent.Bcc,
				IncidentReportContent.Severity,
				IncidentReportContent.Override,
				IncidentReportContent.RuleDetections,
				IncidentReportContent.FalsePositive,
				IncidentReportContent.DataClassifications,
				IncidentReportContent.IdMatch
			};
			if (legacyReportOriginalMail == IncidentReportOriginalMail.IncludeOriginalMail)
			{
				list.Add(IncidentReportContent.AttachOriginalMail);
			}
			return list;
		}

		public static readonly Version GenerateIncidentReportActionVersion = new Version("15.00.0005.03");
	}
}
