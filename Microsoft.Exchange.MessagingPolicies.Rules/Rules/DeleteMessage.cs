using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class DeleteMessage : TransportAction
	{
		public DeleteMessage(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "DeleteMessage";
			}
		}

		public override TransportActionType Type
		{
			get
			{
				return TransportActionType.RecipientRelated;
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			transportRulesEvaluationContext.ShouldExecuteActions = false;
			return DeleteMessage.Delete(transportRulesEvaluationContext, DeleteMessage.TrackingLogResponse);
		}

		internal static ExecutionControl Delete(TransportRulesEvaluationContext context, SmtpResponse trackingLogResponse)
		{
			if (context.EventType == EventType.EndOfData)
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug(0L, "Message is deleted at EOD");
				context.EdgeRejectResponse = new SmtpResponse?(SmtpResponse.QueuedMailForDelivery(context.MailItem.Message.MimeDocument.RootPart.Headers.FindFirst("Message-Id").Value));
				return ExecutionControl.Execute;
			}
			TransportRulesEvaluator.Trace(context.TransportRulesTracer, context.MailItem, "Message is deleted at OnRoutedMessage");
			if (context.MatchedRecipients == null)
			{
				List<EnvelopeRecipient> list = new List<EnvelopeRecipient>(context.MailItem.Recipients);
				foreach (EnvelopeRecipient envelopeRecipient in list)
				{
					envelopeRecipient.Properties["Microsoft.Exchange.DsnGenerator.DsnSource"] = DsnSource.TransportRuleAgent;
					context.MailItem.Recipients.Remove(envelopeRecipient, DsnType.Expanded, trackingLogResponse);
				}
				return ExecutionControl.Execute;
			}
			foreach (EnvelopeRecipient recipient in context.MatchedRecipients)
			{
				context.MailItem.Recipients.Remove(recipient, DsnType.Expanded, trackingLogResponse);
			}
			context.MatchedRecipients.Clear();
			return ExecutionControl.Execute;
		}

		private static readonly SmtpResponse TrackingLogResponse = new SmtpResponse("550", "5.2.1", new string[]
		{
			"Message deleted by the transport rules agent"
		});
	}
}
