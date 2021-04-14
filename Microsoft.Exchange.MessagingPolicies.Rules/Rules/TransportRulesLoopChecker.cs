using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class TransportRulesLoopChecker
	{
		protected TransportRulesLoopChecker()
		{
		}

		internal static bool Fork(QueuedMessageEventSource eventSource, MailItem mailItem, IList<EnvelopeRecipient> recipients)
		{
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = mailItem as ITransportMailItemWrapperFacade;
			if (transportMailItemWrapperFacade != null)
			{
				TransportMailItem transportMailItem = transportMailItemWrapperFacade.TransportMailItem as TransportMailItem;
				if (transportMailItem != null)
				{
					if (transportMailItem.TransportRulesForkCount == null)
					{
						transportMailItem.TransportRulesForkCount = new ForkCount();
					}
					int num = transportMailItem.TransportRulesForkCount.Increment();
					if (num > Components.TransportAppConfig.TransportRuleConfig.TransportRuleMaxForkCount)
					{
						ExTraceGlobals.TransportRulesEngineTracer.TraceDebug(0L, "Message fork loop is detected by the Loop Checker. Fork skipped.");
						TransportAction.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_RuleDetectedExcessiveBifurcation, null, new object[]
						{
							num,
							mailItem.InternetMessageId
						});
						mailItem.Recipients.Clear();
						return false;
					}
				}
			}
			eventSource.Fork(recipients);
			return true;
		}

		internal static void ForkAddedRecipients(QueuedMessageEventSource eventSource, MailItem mailItem)
		{
			OrganizationId organizationID = TransportUtils.GetOrganizationID(mailItem);
			if (organizationID == OrganizationId.ForestWideOrgId)
			{
				return;
			}
			int num = TransportRulesLoopChecker.GetMessageLoopCount(mailItem);
			List<EnvelopeRecipient> recipientsAddedByRules = TransportRulesLoopChecker.GetRecipientsAddedByRules(mailItem);
			if (recipientsAddedByRules.Count == 0)
			{
				return;
			}
			if (mailItem.Recipients.Count > recipientsAddedByRules.Count && !TransportRulesLoopChecker.Fork(eventSource, mailItem, recipientsAddedByRules))
			{
				return;
			}
			num++;
			if (TransportRulesLoopChecker.IsLoopCountExceeded(num))
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceDebug<int>(0L, "Message Transport Loop Count exceeded. Message rejected: {0}", num);
				TransportRulesLoopChecker.RejectLoopedMessage(mailItem);
				return;
			}
			TransportRulesLoopChecker.StampLoopCountHeader(num, mailItem);
		}

		internal static List<EnvelopeRecipient> GetRecipientsAddedByRules(MailItem mailItem)
		{
			List<EnvelopeRecipient> list = new List<EnvelopeRecipient>();
			foreach (EnvelopeRecipient envelopeRecipient in mailItem.Recipients)
			{
				string value = null;
				object obj;
				if (envelopeRecipient.Properties.TryGetValue("Microsoft.Exchange.Transport.AddedByTransportRule", out obj))
				{
					value = (string)obj;
				}
				if (!string.IsNullOrEmpty(value))
				{
					list.Add(envelopeRecipient);
				}
			}
			return list;
		}

		internal static int GetMessageLoopCount(MailItem mailItem)
		{
			int result = 0;
			string s;
			if (TransportUtils.TryGetHeaderValue(mailItem.Message, "X-MS-Exchange-Transport-Rules-Loop", out s))
			{
				int.TryParse(s, out result);
			}
			return result;
		}

		internal static bool IsLoopCountExceeded(int loopCount)
		{
			return loopCount > TransportRulesLoopChecker.MaxLoopCount;
		}

		internal static bool IsLoopCountExceeded(MailItem mailItem)
		{
			return TransportRulesLoopChecker.IsLoopCountExceeded(TransportRulesLoopChecker.GetMessageLoopCount(mailItem));
		}

		internal static bool IsIncidentReportLoopCountExceeded(MailItem mailItem)
		{
			int num = (TransportRulesLoopChecker.MaxLoopCount == 1) ? 2 : TransportRulesLoopChecker.MaxLoopCount;
			return TransportRulesLoopChecker.GetMessageLoopCount(mailItem) > num;
		}

		internal static void RejectLoopedMessage(MailItem mailItem)
		{
			RejectMessage.Reject(mailItem, TransportRulesLoopChecker.LoopExceededStatusCode, TransportRulesLoopChecker.LoopExceededEnhancedStatus, TransportRulesLoopChecker.LoopExceededReasonText);
		}

		internal static void StampLoopCountHeader(int loopCount, MailItem mailItem)
		{
			HeaderList headers = mailItem.Message.MimeDocument.RootPart.Headers;
			Header header = headers.FindFirst("X-MS-Exchange-Transport-Rules-Loop");
			if (header != null)
			{
				header.Value = loopCount.ToString();
				return;
			}
			TransportUtils.AddHeaderToMail(mailItem.Message, "X-MS-Exchange-Transport-Rules-Loop", loopCount.ToString());
		}

		internal static void StampLoopCountHeader(int loopCount, TransportMailItem mailItem)
		{
			HeaderList headers = mailItem.RootPart.Headers;
			Header header = headers.FindFirst("X-MS-Exchange-Transport-Rules-Loop");
			if (header != null)
			{
				header.Value = loopCount.ToString();
				return;
			}
			header = Header.Create("X-MS-Exchange-Transport-Rules-Loop");
			header.Value = loopCount.ToString();
			mailItem.MimeDocument.RootPart.Headers.AppendChild(header);
		}

		private static readonly int MaxLoopCount = 1;

		private static readonly string LoopExceededStatusCode = "550";

		private static readonly string LoopExceededReasonText = "Transport rules loop count exceeded";

		private static readonly string LoopExceededEnhancedStatus = "5.7.1";
	}
}
