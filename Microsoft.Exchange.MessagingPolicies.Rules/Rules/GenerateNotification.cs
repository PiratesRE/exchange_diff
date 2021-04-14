using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class GenerateNotification : TransportAction
	{
		public GenerateNotification(ShortList<Argument> arguments) : base(arguments)
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
				return "GenerateNotification";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return GenerateNotification.GenerateNotificationActionVersion;
			}
		}

		public override void ValidateArguments(ShortList<Argument> inputArguments)
		{
			if (inputArguments.Count != 1)
			{
				throw new RulesValidationException(RulesStrings.ActionArgumentMismatch(this.Name));
			}
			if (inputArguments[0].Type != typeof(string))
			{
				throw new RulesValidationException(RulesStrings.ActionArgumentMismatch(this.Name));
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			if (GenerateNotification.IsEtrNotification(transportRulesEvaluationContext.MailItem.Message))
			{
				TransportRulesEvaluator.Trace(transportRulesEvaluationContext.TransportRulesTracer, transportRulesEvaluationContext.MailItem, "GenerateNotification: Skipping notification on a notification message");
				return ExecutionControl.Execute;
			}
			if (TransportRulesLoopChecker.IsIncidentReportLoopCountExceeded(transportRulesEvaluationContext.MailItem))
			{
				TransportRulesEvaluator.Trace(transportRulesEvaluationContext.TransportRulesTracer, transportRulesEvaluationContext.MailItem, "GenerateNotification: Message loop count limit exceeded. Skipping notification generation");
				return ExecutionControl.Execute;
			}
			string contentTemplate = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			string body = GenerateNotification.GenerateContent(contentTemplate, transportRulesEvaluationContext.MailItem.Message);
			GenerateNotification.GenerateMessage(transportRulesEvaluationContext.MailItem, transportRulesEvaluationContext.Server.Name, new EmailRecipient("Microsoft Outlook", "<>"), transportRulesEvaluationContext.Message.EnvelopeRecipients, transportRulesEvaluationContext.MailItem.Message.Subject, body);
			return ExecutionControl.Execute;
		}

		internal static void GenerateMessage(MailItem sourceMailItem, string serverName, EmailRecipient sender, IEnumerable<string> recipients, string subject, string body)
		{
			TransportMailItem transportMailItem = TransportMailItem.NewAgentMailItem(TransportUtils.GetTransportMailItemFacade(sourceMailItem));
			transportMailItem.From = RoutingAddress.NullReversePath;
			foreach (string smtpAddress in recipients)
			{
				transportMailItem.Recipients.Add(smtpAddress);
				transportMailItem.Message.To.Add(new EmailRecipient(string.Empty, smtpAddress));
			}
			transportMailItem.Message.Sender = sender;
			Header header = Header.Create(HeaderId.MessageId);
			header.Value = string.Format("<{0}@{1}>", Guid.NewGuid(), "EtrNotification");
			transportMailItem.Message.MimeDocument.RootPart.Headers.AppendChild(header);
			transportMailItem.Message.Date = DateTime.UtcNow;
			transportMailItem.Message.Subject = subject;
			GenerateNotification.SetEtrHeaders(transportMailItem, serverName, sourceMailItem);
			GenerateNotification.SetNotificationContent(transportMailItem, body);
			transportMailItem.CommitLazy();
			TransportFacades.TrackReceiveByAgent(transportMailItem, "Transport Rule", null, new long?(transportMailItem.RecordId));
			Components.CategorizerComponent.EnqueueSideEffectMessage(sourceMailItem, transportMailItem, "Transport Rule Agent");
		}

		internal static void SetEtrHeaders(TransportMailItem notificationMessage, string serverName, MailItem mailItem)
		{
			Header header = Header.Create("X-MS-Exchange-Transport-Rules-Notification");
			header.Value = "1";
			notificationMessage.Message.MimeDocument.RootPart.Headers.AppendChild(header);
			Header header2 = Header.Create("X-MS-Exchange-Forest-RulesExecuted");
			header2.Value = serverName;
			notificationMessage.Message.MimeDocument.RootPart.Headers.AppendChild(header2);
			TransportRulesLoopChecker.StampLoopCountHeader(TransportRulesLoopChecker.GetMessageLoopCount(mailItem) + 1, notificationMessage);
		}

		private static void SetNotificationContent(TransportMailItem notificationMessage, string content)
		{
			int num = 0;
			Charset charset = IncidentReport.DetectCharset(new List<string>
			{
				notificationMessage.Message.Subject,
				content
			}, out num);
			Encoding encoding = charset.GetEncoding();
			IncidentReport.SetMultipartAlternativeAndCharsetName(notificationMessage.Message, charset.Name);
			ContentTransferEncoding transferEncoding = ContentTransferEncoding.SevenBit;
			if (IncidentReport.Is8BitEncoded(notificationMessage.Message.Subject, encoding))
			{
				transferEncoding = ContentTransferEncoding.QuotedPrintable;
			}
			if (IncidentReport.Is8BitEncoded(content, encoding))
			{
				transferEncoding = ContentTransferEncoding.QuotedPrintable;
			}
			MimePart mimePart = (MimePart)notificationMessage.Message.RootPart.FirstChild;
			using (Stream contentWriteStream = mimePart.GetContentWriteStream(transferEncoding))
			{
				using (TextWriter textWriter = new StreamWriter(contentWriteStream, encoding))
				{
					string textPresentation = GenerateNotification.GetTextPresentation(content, encoding);
					textWriter.Write(textPresentation);
				}
			}
			MimePart mimePart2 = (MimePart)notificationMessage.Message.RootPart.LastChild;
			using (Stream contentWriteStream2 = mimePart2.GetContentWriteStream(transferEncoding))
			{
				using (TextWriter textWriter2 = new StreamWriter(contentWriteStream2, encoding))
				{
					string htmlPresentation = GenerateNotification.GetHtmlPresentation(content, encoding);
					textWriter2.Write(htmlPresentation);
				}
			}
		}

		internal static bool IsEtrNotification(EmailMessage message)
		{
			string text;
			return TransportUtils.TryGetHeaderValue(message, "X-MS-Exchange-Transport-Rules-Notification", out text);
		}

		internal static string GenerateContent(string contentTemplate, EmailMessage message)
		{
			return GenerateNotification.dynamicParameters.Aggregate(contentTemplate, (string current, string parameter) => Regex.Replace(current, parameter, GenerateNotification.GetParameterValue(parameter, message), RegexOptions.IgnoreCase));
		}

		internal static string GetParameterValue(string parameter, EmailMessage message)
		{
			switch (parameter)
			{
			case "%%From%%":
				return GenerateNotification.GetRecipientString(message.From);
			case "%%To%%":
				return GenerateNotification.GetRecipients(message.To);
			case "%%Cc%%":
				return GenerateNotification.GetRecipients(message.Cc);
			case "%%Subject%%":
				return message.Subject;
			case "%%MessageDate%%":
				return message.Date.ToString("G");
			case "%%Headers%%":
				return GenerateNotification.GetHeaders(message.MimeDocument);
			}
			return string.Empty;
		}

		internal static string GetTextPresentation(string content, Encoding encoding)
		{
			HtmlToText htmlToText = new HtmlToText
			{
				InputEncoding = encoding
			};
			StringBuilder stringBuilder = new StringBuilder();
			using (StringReader stringReader = new StringReader(content))
			{
				using (StringWriter stringWriter = new StringWriter(stringBuilder))
				{
					htmlToText.Convert(stringReader, stringWriter);
				}
			}
			return stringBuilder.ToString();
		}

		internal static string GetHtmlPresentation(string content, Encoding encoding)
		{
			HtmlToHtml htmlToHtml = new HtmlToHtml
			{
				InputEncoding = encoding,
				NormalizeHtml = true,
				FilterHtml = false
			};
			StringBuilder stringBuilder = new StringBuilder();
			using (StringReader stringReader = new StringReader(content))
			{
				using (StringWriter stringWriter = new StringWriter(stringBuilder))
				{
					htmlToHtml.Convert(stringReader, stringWriter);
				}
			}
			return stringBuilder.ToString();
		}

		internal static string PlainTextToHtml(string content, Encoding encoding)
		{
			TextToHtml textToHtml = new TextToHtml
			{
				InputEncoding = encoding,
				FilterHtml = false
			};
			StringBuilder stringBuilder = new StringBuilder();
			using (StringReader stringReader = new StringReader(content))
			{
				using (StringWriter stringWriter = new StringWriter(stringBuilder))
				{
					textToHtml.Convert(stringReader, stringWriter);
				}
			}
			return stringBuilder.ToString();
		}

		internal static string GetRecipients(IEnumerable<EmailRecipient> recipients)
		{
			if (recipients == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (EmailRecipient recipient in recipients)
			{
				stringBuilder.Append(GenerateNotification.GetRecipientString(recipient));
				stringBuilder.Append(", ");
			}
			string text = stringBuilder.ToString();
			if (!string.IsNullOrEmpty(text))
			{
				return text.Substring(0, text.Length - ", ".Length);
			}
			return text;
		}

		internal static string GetRecipientString(EmailRecipient recipient)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(recipient.DisplayName) && !recipient.DisplayName.Equals(recipient.SmtpAddress))
			{
				stringBuilder.Append(recipient.DisplayName);
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(recipient.SmtpAddress);
			return stringBuilder.ToString();
		}

		internal static string GetHeaders(MimeDocument mimeDocument)
		{
			if (mimeDocument != null && mimeDocument.RootPart != null && mimeDocument.RootPart.Headers != null)
			{
				using (MimeNode.Enumerator<Header> enumerator = mimeDocument.RootPart.Headers.GetEnumerator())
				{
					StringBuilder stringBuilder = new StringBuilder();
					while (enumerator.MoveNext())
					{
						Header header = enumerator.Current;
						stringBuilder.Append(header.Name);
						stringBuilder.Append(": ");
						stringBuilder.Append(enumerator.Current.Value);
						stringBuilder.Append("<br />");
					}
					return stringBuilder.ToString();
				}
			}
			return string.Empty;
		}

		private const string MessageIdPrefix = "EtrNotification";

		private const string NotificationHeaderValue = "1";

		private const string ItemSeparator = ", ";

		private const string LineSeparator = "<br />";

		public static readonly Version GenerateNotificationActionVersion = new Version("15.00.0013.00");

		private static readonly List<string> dynamicParameters = new List<string>
		{
			"%%From%%",
			"%%To%%",
			"%%Cc%%",
			"%%Subject%%",
			"%%Headers%%",
			"%%MessageDate%%"
		};

		internal static class ContentParameters
		{
			internal const string From = "%%From%%";

			internal const string To = "%%To%%";

			internal const string Cc = "%%Cc%%";

			internal const string Subject = "%%Subject%%";

			internal const string Headers = "%%Headers%%";

			internal const string MessageDate = "%%MessageDate%%";
		}
	}
}
