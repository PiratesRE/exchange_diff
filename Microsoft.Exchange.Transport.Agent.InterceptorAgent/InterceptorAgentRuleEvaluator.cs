using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Data.Transport.Storage;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal static class InterceptorAgentRuleEvaluator
	{
		public static InterceptorAgentRule Evaluate(IEnumerable<InterceptorAgentRule> rules, MailCommandEventArgs arg)
		{
			if (rules == null || !rules.Any<InterceptorAgentRule>() || arg == null)
			{
				return null;
			}
			return InterceptorAgentRule.InternalEvaluate(rules, InterceptorAgentEvent.OnMailFrom, string.Empty, arg.FromAddress.ToString(), string.Empty, null, RoutingAddress.Empty, null, arg.SmtpSession.HelloDomain, InterceptorAgentRuleEvaluator.GetTenantId(arg.SmtpSession), InterceptorAgentRuleEvaluator.GetDirectionality(arg.SmtpSession), null);
		}

		public static InterceptorAgentRule Evaluate(IEnumerable<InterceptorAgentRule> rules, RcptCommandEventArgs arg)
		{
			if (rules == null || !rules.Any<InterceptorAgentRule>() || arg == null)
			{
				return null;
			}
			TransportMailItem transportMailItem = InterceptorAgentRuleEvaluator.GetTransportMailItem(arg.MailItem);
			return InterceptorAgentRule.InternalEvaluate(rules, InterceptorAgentEvent.OnRcptTo, string.Empty, arg.MailItem.FromAddress.ToString(), string.Empty, arg.MailItem.Recipients, arg.RecipientAddress, null, arg.SmtpSession.HelloDomain, InterceptorAgentRuleEvaluator.GetTenantId(transportMailItem), InterceptorAgentRuleEvaluator.GetDirectionality(transportMailItem), InterceptorAgentRuleEvaluator.GetAccountForest(transportMailItem));
		}

		public static InterceptorAgentRule Evaluate(IEnumerable<InterceptorAgentRule> rules, EndOfHeadersEventArgs arg)
		{
			if (rules == null || !rules.Any<InterceptorAgentRule>() || arg == null)
			{
				return null;
			}
			HeaderList headers = arg.Headers;
			TransportMailItem transportMailItem = InterceptorAgentRuleEvaluator.GetTransportMailItem(arg.MailItem);
			return InterceptorAgentRule.InternalEvaluate(rules, InterceptorAgentEvent.OnEndOfHeaders, InterceptorAgentRuleEvaluator.GetValueFromHeaderId(HeaderId.Subject, headers), arg.MailItem.FromAddress.ToString(), InterceptorAgentRuleEvaluator.GetValueFromHeaderId(HeaderId.MessageId, headers), arg.MailItem.Recipients, RoutingAddress.Empty, headers, arg.SmtpSession.HelloDomain, InterceptorAgentRuleEvaluator.GetTenantId(transportMailItem), InterceptorAgentRuleEvaluator.GetDirectionality(transportMailItem), InterceptorAgentRuleEvaluator.GetAccountForest(transportMailItem));
		}

		public static InterceptorAgentRule Evaluate(IEnumerable<InterceptorAgentRule> rules, EndOfDataEventArgs arg)
		{
			if (rules == null || !rules.Any<InterceptorAgentRule>() || arg == null)
			{
				return null;
			}
			HeaderList headerList = InterceptorAgentRuleEvaluator.GetHeaderList(arg.MailItem);
			TransportMailItem transportMailItem = InterceptorAgentRuleEvaluator.GetTransportMailItem(arg.MailItem);
			return InterceptorAgentRule.InternalEvaluate(rules, InterceptorAgentEvent.OnEndOfData, InterceptorAgentRuleEvaluator.GetValueFromHeaderId(HeaderId.Subject, headerList), arg.MailItem.FromAddress.ToString(), InterceptorAgentRuleEvaluator.GetValueFromHeaderId(HeaderId.MessageId, headerList), arg.MailItem.Recipients, RoutingAddress.Empty, headerList, arg.SmtpSession.HelloDomain, InterceptorAgentRuleEvaluator.GetTenantId(transportMailItem), InterceptorAgentRuleEvaluator.GetDirectionality(transportMailItem), InterceptorAgentRuleEvaluator.GetAccountForest(transportMailItem));
		}

		public static InterceptorAgentRule Evaluate(IEnumerable<InterceptorAgentRule> rules, QueuedMessageEventArgs arg, InterceptorAgentEvent evt)
		{
			if (rules == null || !rules.Any<InterceptorAgentRule>() || arg == null)
			{
				return null;
			}
			HeaderList headerList = InterceptorAgentRuleEvaluator.GetHeaderList(arg.MailItem);
			TransportMailItem transportMailItem = InterceptorAgentRuleEvaluator.GetTransportMailItem(arg.MailItem);
			return InterceptorAgentRule.InternalEvaluate(rules, evt, InterceptorAgentRuleEvaluator.GetValueFromHeaderId(HeaderId.Subject, headerList), arg.MailItem.FromAddress.ToString(), InterceptorAgentRuleEvaluator.GetValueFromHeaderId(HeaderId.MessageId, headerList), arg.MailItem.Recipients, RoutingAddress.Empty, headerList, null, InterceptorAgentRuleEvaluator.GetTenantId(transportMailItem), InterceptorAgentRuleEvaluator.GetDirectionality(transportMailItem), InterceptorAgentRuleEvaluator.GetAccountForest(transportMailItem));
		}

		internal static InterceptorAgentRule Evaluate(IEnumerable<InterceptorAgentRule> rules, StoreDriverDeliveryEventArgs arg, InterceptorAgentEvent evt)
		{
			if (rules == null || !rules.Any<InterceptorAgentRule>() || arg == null)
			{
				return null;
			}
			MailItem mailItem = new DeliverableMailItemWrapper(arg.MailItem);
			HeaderList headerList = InterceptorAgentRuleEvaluator.GetHeaderList(mailItem);
			TransportMailItem transportMailItem = InterceptorAgentRuleEvaluator.GetTransportMailItem(mailItem);
			return InterceptorAgentRule.InternalEvaluate(rules, evt, InterceptorAgentRuleEvaluator.GetValueFromHeaderId(HeaderId.Subject, headerList), arg.MailItem.FromAddress.ToString(), InterceptorAgentRuleEvaluator.GetValueFromHeaderId(HeaderId.MessageId, headerList), new EnvelopeRecipientCollectionWrapper(arg.MailItem.Recipients), RoutingAddress.Empty, headerList, null, InterceptorAgentRuleEvaluator.GetTenantId(transportMailItem), InterceptorAgentRuleEvaluator.GetDirectionality(transportMailItem), InterceptorAgentRuleEvaluator.GetAccountForest(transportMailItem));
		}

		internal static InterceptorAgentRule Evaluate(IEnumerable<InterceptorAgentRule> rules, StorageEventArgs arg)
		{
			if (rules == null || !rules.Any<InterceptorAgentRule>() || arg == null)
			{
				return null;
			}
			HeaderList headerList = InterceptorAgentRuleEvaluator.GetHeaderList(arg.MailItem);
			TransportMailItem transportMailItem = InterceptorAgentRuleEvaluator.GetTransportMailItem(arg.MailItem);
			return InterceptorAgentRule.InternalEvaluate(rules, InterceptorAgentEvent.OnLoadedMessage, InterceptorAgentRuleEvaluator.GetValueFromHeaderId(HeaderId.Subject, headerList), arg.MailItem.FromAddress.ToString(), InterceptorAgentRuleEvaluator.GetValueFromHeaderId(HeaderId.MessageId, headerList), new EnvelopeRecipientCollectionWrapper(arg.MailItem.Recipients), RoutingAddress.Empty, headerList, null, InterceptorAgentRuleEvaluator.GetTenantId(transportMailItem), InterceptorAgentRuleEvaluator.GetDirectionality(transportMailItem), InterceptorAgentRuleEvaluator.GetAccountForest(transportMailItem));
		}

		internal static InterceptorAgentRule Evaluate(IEnumerable<InterceptorAgentRule> rules, StoreDriverSubmissionEventArgs arg)
		{
			if (rules == null || !rules.Any<InterceptorAgentRule>() || arg == null)
			{
				return null;
			}
			HeaderList headerList = InterceptorAgentRuleEvaluator.GetHeaderList(arg.MailItem);
			TransportMailItem transportMailItem = InterceptorAgentRuleEvaluator.GetTransportMailItem(arg.MailItem);
			return InterceptorAgentRule.InternalEvaluate(rules, InterceptorAgentEvent.OnDemotedMessage, InterceptorAgentRuleEvaluator.GetValueFromHeaderId(HeaderId.Subject, headerList), arg.MailItem.FromAddress.ToString(), InterceptorAgentRuleEvaluator.GetValueFromHeaderId(HeaderId.MessageId, headerList), new EnvelopeRecipientCollectionWrapper(arg.MailItem.Recipients), RoutingAddress.Empty, headerList, null, InterceptorAgentRuleEvaluator.GetTenantId(transportMailItem), InterceptorAgentRuleEvaluator.GetDirectionality(transportMailItem), InterceptorAgentRuleEvaluator.GetAccountForest(transportMailItem));
		}

		private static Guid GetTenantId(TransportMailItem transportMailItem)
		{
			Guid result = Guid.Empty;
			if (transportMailItem != null)
			{
				result = transportMailItem.ExternalOrganizationId;
			}
			return result;
		}

		private static Guid GetTenantId(SmtpSession smtpSession)
		{
			Guid empty = Guid.Empty;
			object obj;
			if (smtpSession != null && smtpSession.Properties.TryGetValue("X-MS-Exchange-Organization-Id", out obj))
			{
				Guid.TryParse(obj.ToString(), out empty);
			}
			return empty;
		}

		private static MailDirectionality GetDirectionality(TransportMailItem transportMailItem)
		{
			MailDirectionality result = MailDirectionality.Undefined;
			if (transportMailItem != null)
			{
				result = transportMailItem.Directionality;
			}
			return result;
		}

		private static MailDirectionality GetDirectionality(SmtpSession smtpSession)
		{
			MailDirectionality result = MailDirectionality.Undefined;
			object obj;
			if (smtpSession != null && smtpSession.Properties.TryGetValue("X-MS-Exchange-Organization-MessageDirectionality", out obj))
			{
				InterceptorAgentCondition.ValidateDirectionality(obj.ToString(), out result);
			}
			return result;
		}

		private static string GetAccountForest(TransportMailItem transportMailItem)
		{
			if (transportMailItem != null)
			{
				return transportMailItem.ExoAccountForest;
			}
			return null;
		}

		private static TransportMailItem GetTransportMailItem(MailItem mailItem)
		{
			TransportMailItem result = null;
			ITransportMailItemWrapperFacade transportMailItemWrapperFacade = mailItem as ITransportMailItemWrapperFacade;
			if (transportMailItemWrapperFacade != null)
			{
				result = (transportMailItemWrapperFacade.TransportMailItem as TransportMailItem);
			}
			return result;
		}

		private static HeaderList GetHeaderList(MailItem mailItem)
		{
			HeaderList result = null;
			if (mailItem.MimeDocument != null && mailItem.MimeDocument.RootPart != null)
			{
				result = mailItem.MimeDocument.RootPart.Headers;
			}
			return result;
		}

		private static HeaderList GetHeaderList(DeliverableMailItem mailItem)
		{
			HeaderList result = null;
			if (mailItem.Message != null && mailItem.Message.RootPart != null)
			{
				result = mailItem.Message.RootPart.Headers;
			}
			return result;
		}

		private static string GetValueFromHeaderId(HeaderId headerId, HeaderList headers)
		{
			string empty = string.Empty;
			if (headers == null)
			{
				return empty;
			}
			Header header = headers.FindFirst(headerId);
			return EmailMessageHelpers.GetHeaderValue(header) ?? string.Empty;
		}
	}
}
