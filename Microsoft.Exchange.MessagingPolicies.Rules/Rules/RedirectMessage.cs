using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class RedirectMessage : TransportAction
	{
		public RedirectMessage(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override string Name
		{
			get
			{
				return "RedirectMessage";
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return RedirectMessage.argumentTypes;
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
			string text = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			RedirectMessage.AddTrackingInfo(transportRulesEvaluationContext, text);
			DeleteMessage.Delete(transportRulesEvaluationContext, new SmtpResponse("550", "5.2.1", new string[]
			{
				string.Format("Message redirected to '{0}' by the transport rules agent", text)
			}));
			ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<string>(0L, "Redirect To: {0}", text);
			transportRulesEvaluationContext.RecipientsToAdd.Add(new TransportRulesEvaluationContext.AddedRecipient(text, text, RecipientP2Type.Redirect));
			return ExecutionControl.Execute;
		}

		private static void AddTrackingInfo(TransportRulesEvaluationContext context, string redirectAddress)
		{
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = context.MailItem as ITransportMailItemWrapperFacade;
			if (transportMailItemWrapperFacade != null)
			{
				RoutingAddress redirectDestination = new RoutingAddress(redirectAddress);
				string sourceContext = string.Format("Redirected by transport rule '{0}'", context.CurrentRule.ImmutableId);
				if (context.MatchedRecipients == null)
				{
					List<EnvelopeRecipient> list = new List<EnvelopeRecipient>(context.MailItem.Recipients);
					using (List<EnvelopeRecipient>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							EnvelopeRecipient envelopeRecipient = enumerator.Current;
							RedirectMessage.TrackRedirect(sourceContext, transportMailItemWrapperFacade, envelopeRecipient.Address, redirectDestination);
						}
						return;
					}
				}
				foreach (EnvelopeRecipient envelopeRecipient2 in context.MatchedRecipients)
				{
					RedirectMessage.TrackRedirect(sourceContext, transportMailItemWrapperFacade, envelopeRecipient2.Address, redirectDestination);
				}
			}
		}

		private static void TrackRedirect(string sourceContext, ITransportMailItemWrapperFacade tmiFacade, RoutingAddress redirectSource, RoutingAddress redirectDestination)
		{
			MsgTrackRedirectInfo msgTrackInfo = new MsgTrackRedirectInfo(sourceContext, redirectSource, redirectDestination, null);
			MessageTrackingLog.TrackRedirect(MessageTrackingSource.AGENT, (IReadOnlyMailItem)tmiFacade.TransportMailItem, msgTrackInfo);
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string)
		};
	}
}
