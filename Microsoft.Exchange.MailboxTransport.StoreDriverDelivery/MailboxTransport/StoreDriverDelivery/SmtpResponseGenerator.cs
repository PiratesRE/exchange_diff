using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal static class SmtpResponseGenerator
	{
		public static SmtpResponse GenerateResponse(MessageAction messageLevelAction, IReadOnlyMailRecipientCollection recipients, SmtpResponse smtpResponse, TimeSpan? retryInterval)
		{
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients", "Required argument is not provided");
			}
			bool flag = SmtpResponseGenerator.ShouldGenerateAllRecipients(recipients);
			if (!flag && retryInterval == null && messageLevelAction != MessageAction.RetryQueue)
			{
				return smtpResponse;
			}
			StringBuilder stringBuilder = new StringBuilder();
			SmtpResponseGenerator.GenerateBanner(stringBuilder, smtpResponse.StatusCode, smtpResponse.EnhancedStatusCode);
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}{2} {3} {4}{5}{6}", new object[]
			{
				smtpResponse.StatusCode,
				flag ? "-" : " ",
				flag ? smtpResponse.StatusCode : string.Empty,
				smtpResponse.EnhancedStatusCode,
				SmtpResponseGenerator.FlattenStatusText(smtpResponse),
				SmtpResponseGenerator.GenerateAdditionalContextForMessage(messageLevelAction, retryInterval),
				"\r\n"
			});
			if (flag)
			{
				IEnumerator<MailRecipient> enumerator = recipients.GetEnumerator();
				bool flag2 = enumerator.MoveNext();
				while (flag2)
				{
					MailRecipient mailRecipient = enumerator.Current;
					flag2 = enumerator.MoveNext();
					bool value = mailRecipient.ExtendedProperties.GetValue<bool>("Microsoft.Exchange.Transport.MailboxTransport.RetryOnDuplicateDelivery ", false);
					mailRecipient.ExtendedProperties.Remove("Microsoft.Exchange.Transport.MailboxTransport.RetryOnDuplicateDelivery ");
					SmtpResponse responseForRecipient = SmtpResponseGenerator.GetResponseForRecipient(mailRecipient, value);
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}{2} {3} {4}{5}{6}", new object[]
					{
						smtpResponse.StatusCode,
						flag2 ? "-" : " ",
						responseForRecipient.StatusCode,
						responseForRecipient.EnhancedStatusCode,
						SmtpResponseGenerator.FlattenStatusText(responseForRecipient),
						SmtpResponseGenerator.GenerateAdditionalContextForRecipient(mailRecipient, value),
						"\r\n"
					});
				}
			}
			return SmtpResponse.Parse(stringBuilder.ToString());
		}

		public static string FlattenStatusText(SmtpResponse response)
		{
			if (response.StatusText == null || response.StatusText.Length == 0)
			{
				return string.Empty;
			}
			if (response.StatusText.Length == 1)
			{
				return response.StatusText[0];
			}
			IEnumerator<string> enumerator = ((IEnumerable<string>)response.StatusText).GetEnumerator();
			if (!enumerator.MoveNext())
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(enumerator.Current);
			while (enumerator.MoveNext())
			{
				stringBuilder.Append("; ");
				stringBuilder.Append(enumerator.Current);
			}
			return stringBuilder.ToString();
		}

		private static string GenerateAdditionalContextForRecipient(MailRecipient recipient, bool retryOnDuplicateDelivery)
		{
			string text = "resubmit";
			bool flag = false;
			if (recipient.AckStatus == AckStatus.Resubmit)
			{
				text = "resubmit";
				flag = true;
			}
			else if (retryOnDuplicateDelivery)
			{
				text = "retryonduplicatedelivery";
				flag = true;
			}
			else if (recipient.AckStatus == AckStatus.SuccessNoDsn)
			{
				text = "skipdsn";
				flag = true;
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(64608U, text);
			return string.Format(CultureInfo.InvariantCulture, "[{0}={1}]", new object[]
			{
				text,
				flag
			});
		}

		private static string GenerateAdditionalContextForMessage(MessageAction messageLevelAction, TimeSpan? retryInterval)
		{
			string text = "resubmit";
			object obj = false;
			if (messageLevelAction == MessageAction.RetryQueue)
			{
				text = "retryqueue";
				obj = true;
			}
			else if (retryInterval != null)
			{
				text = "retryinterval";
				obj = retryInterval.Value.ToString();
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(48224U, text);
			return string.Format(CultureInfo.InvariantCulture, "[{0}={1}]", new object[]
			{
				text,
				obj
			});
		}

		private static bool ShouldGenerateAllRecipients(IReadOnlyMailRecipientCollection recipients)
		{
			foreach (MailRecipient mailRecipient in recipients)
			{
				if (mailRecipient.AckStatus != AckStatus.Success || mailRecipient.ExtendedProperties.GetValue<bool>("Microsoft.Exchange.Transport.MailboxTransport.RetryOnDuplicateDelivery ", false))
				{
					return true;
				}
			}
			return false;
		}

		private static SmtpResponse CreateStoreDriverRetireResponse()
		{
			StringBuilder stringBuilder = new StringBuilder();
			SmtpResponseGenerator.GenerateBanner(stringBuilder, "432", "4.3.2");
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}{2} {3} {4}{5}{6}", new object[]
			{
				"432",
				" ",
				"432",
				"4.3.2",
				"STOREDRV.Deliver; Component has been retired",
				string.Format(CultureInfo.InvariantCulture, "[{0}={1}]", new object[]
				{
					"resubmit",
					true
				}),
				"\r\n"
			});
			return SmtpResponse.Parse(stringBuilder.ToString());
		}

		private static void GenerateBanner(StringBuilder responseBuilder, string statusCode, string enhancedStatusCode)
		{
			responseBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}{2} {3} {4}{5}{6}", new object[]
			{
				statusCode,
				"-",
				statusCode,
				enhancedStatusCode,
				"STOREDRV.Deliver; delivery result banner",
				string.Empty,
				"\r\n"
			});
		}

		private static SmtpResponse GetResponseForRecipient(MailRecipient recipient, bool retryOnDuplicateDelivery)
		{
			SmtpResponse result = recipient.SmtpResponse;
			if (result.Equals(SmtpResponse.Empty))
			{
				result = SmtpResponse.NoopOk;
			}
			if (retryOnDuplicateDelivery)
			{
				result = AckReason.DeliverAgentTransientFailure;
				if (recipient.ExtendedProperties.Contains("ExceptionAgentName"))
				{
					result.StatusText[0] = string.Format("{0}[Agent: {1}]", AckReason.DeliverAgentTransientFailure.StatusText[0], recipient.ExtendedProperties.GetValue<string>("ExceptionAgentName", string.Empty));
				}
			}
			return result;
		}

		private const string CRLF = "\r\n";

		public static readonly SmtpResponse StoreDriverRetireResponse = SmtpResponseGenerator.CreateStoreDriverRetireResponse();
	}
}
