using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class RejectMessage : TransportAction
	{
		public RejectMessage(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return RejectMessage.argumentTypes;
			}
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
				return "RejectMessage";
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			string status = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			string enhancedStatus = (string)base.Arguments[1].GetValue(transportRulesEvaluationContext);
			string reason = (string)base.Arguments[2].GetValue(transportRulesEvaluationContext);
			transportRulesEvaluationContext.ShouldExecuteActions = false;
			return RejectMessage.Reject(transportRulesEvaluationContext, status, enhancedStatus, reason);
		}

		internal static ExecutionControl Reject(TransportRulesEvaluationContext context, string status, string enhancedStatus, string reason)
		{
			SmtpResponse smtpResponse = new SmtpResponse(status, enhancedStatus, reason, true, new string[]
			{
				RejectMessage.responseDebugInfo
			});
			if (context.EventType == EventType.EndOfData)
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug(0L, "Message is rejected at EOD");
				context.EdgeRejectResponse = new SmtpResponse?(smtpResponse);
				return ExecutionControl.Execute;
			}
			ExTraceGlobals.TransportRulesEngineTracer.TraceDebug(0L, "Message is rejected at OnRoutedMessage");
			if (context.MatchedRecipients == null)
			{
				List<EnvelopeRecipient> list = new List<EnvelopeRecipient>(context.MailItem.Recipients.Count);
				foreach (EnvelopeRecipient item in context.MailItem.Recipients)
				{
					list.Add(item);
				}
				foreach (EnvelopeRecipient envelopeRecipient in list)
				{
					envelopeRecipient.Properties["Microsoft.Exchange.DsnGenerator.DsnSource"] = DsnSource.TransportRuleAgent;
					context.MailItem.Recipients.Remove(envelopeRecipient, DsnType.Failure, smtpResponse);
				}
				return ExecutionControl.Execute;
			}
			foreach (EnvelopeRecipient envelopeRecipient2 in context.MatchedRecipients)
			{
				envelopeRecipient2.Properties["Microsoft.Exchange.DsnGenerator.DsnSource"] = DsnSource.TransportRuleAgent;
				context.MailItem.Recipients.Remove(envelopeRecipient2, DsnType.Failure, smtpResponse);
			}
			context.MatchedRecipients.Clear();
			return ExecutionControl.Execute;
		}

		internal static void Reject(MailItem mailItem, string status, string enhancedStatus, string reason)
		{
			SmtpResponse smtpResponse = new SmtpResponse(status, enhancedStatus, reason, true, new string[]
			{
				RejectMessage.responseDebugInfo
			});
			List<EnvelopeRecipient> list = new List<EnvelopeRecipient>(mailItem.Recipients.Count);
			list.AddRange(mailItem.Recipients);
			foreach (EnvelopeRecipient recipient in list)
			{
				mailItem.Recipients.Remove(recipient, DsnType.Failure, smtpResponse);
			}
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string)
		};

		private static string responseDebugInfo = "TRANSPORT.RULES.RejectMessage; the message was rejected by organization policy";
	}
}
