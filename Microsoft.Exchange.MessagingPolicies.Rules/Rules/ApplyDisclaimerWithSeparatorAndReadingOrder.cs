using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class ApplyDisclaimerWithSeparatorAndReadingOrder : TransportAction
	{
		public ApplyDisclaimerWithSeparatorAndReadingOrder(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Version MinimumVersion
		{
			get
			{
				return ApplyDisclaimerWithSeparatorAndReadingOrder.minimumVersion;
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return ApplyDisclaimerWithSeparatorAndReadingOrder.argumentTypes;
			}
		}

		public override string Name
		{
			get
			{
				return "ApplyDisclaimerWithSeparatorAndReadingOrder";
			}
		}

		public override TransportActionType Type
		{
			get
			{
				return TransportActionType.BifurcationNeeded;
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			string a = (string)base.Arguments[0].GetValue(transportRulesEvaluationContext);
			string text = (string)base.Arguments[1].GetValue(transportRulesEvaluationContext);
			string font = (string)base.Arguments[2].GetValue(transportRulesEvaluationContext);
			string size = (string)base.Arguments[3].GetValue(transportRulesEvaluationContext);
			string color = (string)base.Arguments[4].GetValue(transportRulesEvaluationContext);
			string fallbackAction = (string)base.Arguments[5].GetValue(transportRulesEvaluationContext);
			string separator = this.GetSeparator(transportRulesEvaluationContext);
			string readingOrder = this.GetReadingOrder(transportRulesEvaluationContext);
			string name = transportRulesEvaluationContext.CurrentRule.Name;
			DisclaimerEntry entry;
			bool flag2;
			lock (ApplyDisclaimerWithSeparatorAndReadingOrder.disclaimerLockVar)
			{
				flag2 = ApplyDisclaimerWithSeparatorAndReadingOrder.disclaimerLookupTable.TryGetValue(name, out entry);
			}
			if (!flag2)
			{
				entry = this.AddDisclaimerValuesToLookup(name, text, font, color, size, separator, readingOrder);
			}
			Header[] array = transportRulesEvaluationContext.MailItem.Message.MimeDocument.RootPart.Headers.FindAll("X-MS-Exchange-Organization-Disclaimer-Hash");
			foreach (Header header in array)
			{
				if (header.Value == entry.TextHash)
				{
					return ExecutionControl.Execute;
				}
			}
			if (!transportRulesEvaluationContext.CanModify)
			{
				return this.HandleFallbackOption(transportRulesEvaluationContext, fallbackAction, entry.AppendedPlainText);
			}
			Body body = transportRulesEvaluationContext.MailItem.Message.Body;
			Encoding encoding = null;
			if (body.BodyFormat != BodyFormat.None)
			{
				encoding = ApplyDisclaimerWithSeparatorAndReadingOrder.GetMessageBodyEncoding(transportRulesEvaluationContext);
			}
			else if (transportRulesEvaluationContext.MailItem.Message.TnefPart != null && transportRulesEvaluationContext.MailItem.Message.MapiMessageClass.StartsWith("IPM.Schedule.Meeting.Resp."))
			{
				return ExecutionControl.Execute;
			}
			if (encoding == null)
			{
				return this.HandleFallbackOption(transportRulesEvaluationContext, fallbackAction, entry.AppendedPlainText);
			}
			switch (body.BodyFormat)
			{
			case BodyFormat.Text:
			{
				TextToText textToText = new TextToText();
				if (a == "Append")
				{
					textToText.Footer = entry.AppendedPlainText;
				}
				else
				{
					textToText.Header = entry.PrependedPlainText;
				}
				textToText.InputEncoding = encoding;
				textToText.HeaderFooterFormat = HeaderFooterFormat.Text;
				if (!ApplyDisclaimerWithSeparatorAndReadingOrder.ConvertTextBody(transportRulesEvaluationContext, entry, textToText, encoding))
				{
					return this.HandleFallbackOption(transportRulesEvaluationContext, fallbackAction, entry.AppendedPlainText);
				}
				break;
			}
			case BodyFormat.Rtf:
			{
				RtfToRtf rtfToRtf = new RtfToRtf();
				if (a == "Append")
				{
					rtfToRtf.Footer = entry.AppendedHtmlText;
				}
				else
				{
					rtfToRtf.Header = entry.PrependedHtmlText;
				}
				rtfToRtf.HeaderFooterFormat = HeaderFooterFormat.Html;
				Stream stream = null;
				Stream stream2 = null;
				if (!body.TryGetContentReadStream(out stream))
				{
					return this.HandleFallbackOption(transportRulesEvaluationContext, fallbackAction, entry.AppendedPlainText);
				}
				try
				{
					stream2 = body.GetContentWriteStream();
					try
					{
						rtfToRtf.Convert(stream, stream2);
					}
					catch (ExchangeDataException)
					{
						return this.HandleFallbackOption(transportRulesEvaluationContext, fallbackAction, entry.AppendedPlainText);
					}
				}
				finally
				{
					if (stream2 != null)
					{
						stream2.Close();
					}
					if (stream != null)
					{
						stream.Close();
					}
				}
				break;
			}
			case BodyFormat.Html:
			{
				HtmlToHtml htmlToHtml = new HtmlToHtml();
				if (a == "Append")
				{
					htmlToHtml.Footer = entry.AppendedHtmlText;
				}
				else
				{
					htmlToHtml.Header = entry.PrependedHtmlText;
				}
				htmlToHtml.HeaderFooterFormat = HeaderFooterFormat.Html;
				htmlToHtml.InputEncoding = encoding;
				if (!ApplyDisclaimerWithSeparatorAndReadingOrder.ConvertHtmlBody(transportRulesEvaluationContext, entry, htmlToHtml, encoding))
				{
					return this.HandleFallbackOption(transportRulesEvaluationContext, fallbackAction, entry.AppendedPlainText);
				}
				break;
			}
			}
			if (transportRulesEvaluationContext.MailItem.Message.CalendarPart == null && body.BodyFormat == BodyFormat.None)
			{
				return this.HandleFallbackOption(transportRulesEvaluationContext, fallbackAction, entry.AppendedPlainText);
			}
			TransportUtils.AddHeaderToMail(transportRulesEvaluationContext.MailItem.Message, "X-MS-Exchange-Organization-Disclaimer-Hash", entry.TextHash);
			return ExecutionControl.Execute;
		}

		protected virtual string GetSeparator(RulesEvaluationContext context)
		{
			return (string)base.Arguments[6].GetValue(context);
		}

		protected virtual string GetReadingOrder(RulesEvaluationContext context)
		{
			return (string)base.Arguments[7].GetValue(context);
		}

		private static Encoding GetMessageBodyEncoding(TransportRulesEvaluationContext context)
		{
			Encoding result = null;
			bool flag;
			try
			{
				string charsetName = context.MailItem.Message.Body.CharsetName;
				if (string.IsNullOrEmpty(charsetName))
				{
					flag = true;
					result = Encoding.ASCII;
				}
				else
				{
					flag = Charset.TryGetEncoding(charsetName, out result);
				}
			}
			catch (ExchangeDataException)
			{
				flag = false;
			}
			if (flag)
			{
				return result;
			}
			return null;
		}

		private static bool ConvertTextBody(TransportRulesEvaluationContext context, DisclaimerEntry entry, TextToText t2tConverter, Encoding bodyEncoding)
		{
			Stream stream = null;
			Stream stream2 = null;
			Body body = context.MailItem.Message.Body;
			if (!body.TryGetContentReadStream(out stream))
			{
				return false;
			}
			Encoding utf = Encoding.UTF8;
			bool flag = ApplyDisclaimerWithSeparatorAndReadingOrder.ComputeRequiredOutputEncoding(entry, bodyEncoding, ref utf);
			if (!flag)
			{
				flag = body.ConversionNeeded(entry.ValidCodePages);
			}
			try
			{
				if (flag)
				{
					stream2 = body.GetContentWriteStream(utf.WebName);
					t2tConverter.OutputEncoding = utf;
				}
				else
				{
					stream2 = body.GetContentWriteStream();
				}
				try
				{
					t2tConverter.Convert(stream, stream2);
				}
				catch (ExchangeDataException)
				{
					return false;
				}
			}
			finally
			{
				if (stream2 != null)
				{
					stream2.Close();
				}
				if (stream != null)
				{
					stream.Close();
				}
			}
			return true;
		}

		private static bool ConvertHtmlBody(TransportRulesEvaluationContext context, DisclaimerEntry entry, HtmlToHtml h2hConverter, Encoding bodyEncoding)
		{
			Stream stream = null;
			Stream stream2 = null;
			Body body = context.MailItem.Message.Body;
			if (!body.TryGetContentReadStream(out stream))
			{
				return false;
			}
			Encoding utf = Encoding.UTF8;
			bool flag = ApplyDisclaimerWithSeparatorAndReadingOrder.ComputeRequiredOutputEncoding(entry, bodyEncoding, ref utf);
			try
			{
				if (flag)
				{
					stream2 = body.GetContentWriteStream(utf.WebName);
					h2hConverter.OutputEncoding = utf;
				}
				else
				{
					stream2 = body.GetContentWriteStream();
				}
				try
				{
					h2hConverter.Convert(stream, stream2);
				}
				catch (ExchangeDataException)
				{
					return false;
				}
			}
			finally
			{
				if (stream2 != null)
				{
					stream2.Close();
				}
				if (stream != null)
				{
					stream.Close();
				}
			}
			return true;
		}

		private static bool ComputeRequiredOutputEncoding(DisclaimerEntry entry, Encoding bodyEncoding, ref Encoding outputEncoding)
		{
			bool result;
			if (bodyEncoding == Encoding.Unicode || bodyEncoding == Encoding.UTF8 || bodyEncoding == Encoding.UTF7 || bodyEncoding.CodePage == 65001 || bodyEncoding.CodePage == 65000 || bodyEncoding.CodePage == 1200 || bodyEncoding.CodePage == 1201 || bodyEncoding.CodePage == 54936 || bodyEncoding.CodePage == 12000 || bodyEncoding.CodePage == 12001)
			{
				result = false;
			}
			else
			{
				result = true;
				foreach (int codePage in entry.ValidCodePages)
				{
					Encoding encoding;
					if (Charset.TryGetEncoding(codePage, out encoding) && encoding == bodyEncoding)
					{
						outputEncoding = encoding;
						result = false;
						break;
					}
				}
			}
			return result;
		}

		private DisclaimerEntry AddDisclaimerValuesToLookup(string key, string text, string font, string color, string size, string separator, string readingOrder)
		{
			bool flag = separator == "WithSeparator";
			bool flag2 = readingOrder == "RightToLeft";
			DisclaimerEntry disclaimerEntry;
			disclaimerEntry.AppendedPlainText = "\r\n" + text;
			disclaimerEntry.PrependedPlainText = text + "\r\n";
			if (string.Equals(font, "CourierNew"))
			{
				font = "Courier New";
			}
			if (string.Equals(size, "smallest", StringComparison.InvariantCultureIgnoreCase))
			{
				size = "1";
			}
			else if (string.Equals(size, "smaller", StringComparison.InvariantCultureIgnoreCase))
			{
				size = "2";
			}
			else if (string.Equals(size, "larger", StringComparison.InvariantCultureIgnoreCase))
			{
				size = "5";
			}
			else if (string.Equals(size, "largest", StringComparison.InvariantCultureIgnoreCase))
			{
				size = "6";
			}
			else
			{
				size = "3";
			}
			string text2 = string.Format("<font face='{0}' color='{1}' size='{2}'>", font, color, size);
			TextToText textToText = new TextToText();
			textToText.HtmlEscapeOutput = true;
			StringBuilder stringBuilder = new StringBuilder();
			using (StringReader stringReader = new StringReader(text.ToString()))
			{
				using (StringWriter stringWriter = new StringWriter(stringBuilder))
				{
					textToText.Convert(stringReader, stringWriter);
				}
			}
			string text3 = stringBuilder.ToString();
			text3 = text3.Replace("\r\n", "<br>");
			text3 = text3.Replace("\n", "<br>");
			text2 += text3;
			text2 += "</font>";
			if (flag2)
			{
				text2 = "<p align='right'><span dir='rtl'>" + text2;
				text2 += "</span></p>";
			}
			if (flag)
			{
				disclaimerEntry.AppendedHtmlText = "<br><hr>" + text2;
				disclaimerEntry.PrependedHtmlText = text2 + "<hr><br>";
			}
			else
			{
				disclaimerEntry.AppendedHtmlText = "<br>" + text2;
				disclaimerEntry.PrependedHtmlText = text2 + "<br>";
			}
			disclaimerEntry.TextHash = TransportUtils.GenerateHashString(key);
			OutboundCodePageDetector outboundCodePageDetector = new OutboundCodePageDetector();
			outboundCodePageDetector.AddText(disclaimerEntry.AppendedPlainText);
			disclaimerEntry.ValidCodePages = outboundCodePageDetector.GetCodePages();
			lock (ApplyDisclaimerWithSeparatorAndReadingOrder.disclaimerLockVar)
			{
				if (!ApplyDisclaimerWithSeparatorAndReadingOrder.disclaimerLookupTable.ContainsKey(key))
				{
					ApplyDisclaimerWithSeparatorAndReadingOrder.disclaimerLookupTable.Add(key, disclaimerEntry);
				}
			}
			return disclaimerEntry;
		}

		private void HandleDisclaimerWrap(TransportRulesEvaluationContext context, string disclaimerText)
		{
			MimePart rootPart = context.MailItem.Message.RootPart;
			MimePart mimePart = new MimePart("message/rfc822");
			using (Stream rawContentWriteStream = mimePart.GetRawContentWriteStream())
			{
				rootPart.WriteTo(rawContentWriteStream);
			}
			OutboundCodePageDetector outboundCodePageDetector = new OutboundCodePageDetector();
			outboundCodePageDetector.AddText(disclaimerText);
			Header header = rootPart.Headers.FindFirst(HeaderId.Subject);
			if (header != null && !string.IsNullOrEmpty(header.Value))
			{
				outboundCodePageDetector.AddText(header.Value);
			}
			Encoding utf;
			if (!Charset.TryGetEncoding(outboundCodePageDetector.GetCodePage(), out utf))
			{
				utf = Encoding.UTF8;
			}
			MimePart mimePart2 = new MimePart("text/plain", ContentTransferEncoding.QuotedPrintable, new MemoryStream(utf.GetBytes(disclaimerText)), CachingMode.SourceTakeOwnership);
			ContentTypeHeader contentTypeHeader = mimePart2.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
			contentTypeHeader.AppendChild(new MimeParameter("charset", utf.WebName));
			rootPart.Headers.AppendChild(new ContentTypeHeader("multipart/mixed"));
			ContentTransferEncoding contentTransferEncoding = rootPart.ContentTransferEncoding;
			bool flag = contentTransferEncoding == ContentTransferEncoding.EightBit || contentTransferEncoding == ContentTransferEncoding.SevenBit || contentTransferEncoding == ContentTransferEncoding.Binary;
			if (flag)
			{
				if (contentTransferEncoding != ContentTransferEncoding.SevenBit)
				{
					mimePart.Headers.AppendChild(rootPart.Headers.FindFirst(HeaderId.ContentTransferEncoding).Clone());
				}
			}
			else
			{
				rootPart.Headers.RemoveAll(HeaderId.ContentTransferEncoding);
			}
			for (int i = 0; i < ApplyDisclaimerWithSeparatorAndReadingOrder.HeadersToClear.Length; i++)
			{
				rootPart.Headers.RemoveAll(ApplyDisclaimerWithSeparatorAndReadingOrder.HeadersToClear[i]);
			}
			rootPart.RemoveAll();
			rootPart.AppendChild(mimePart2);
			rootPart.AppendChild(mimePart);
			DisclaimerEntry disclaimerEntry;
			ApplyDisclaimerWithSeparatorAndReadingOrder.disclaimerLookupTable.TryGetValue(context.CurrentRule.Name, out disclaimerEntry);
			Header header2 = Header.Create("X-MS-Exchange-Organization-Disclaimer-Hash");
			header2.Value = disclaimerEntry.TextHash;
			rootPart.Headers.AppendChild(header2);
		}

		private ExecutionControl HandleFallbackOption(TransportRulesEvaluationContext context, string fallbackAction, string disclaimerText)
		{
			if (fallbackAction != null && !(fallbackAction == "Ignore"))
			{
				if (!(fallbackAction == "Wrap"))
				{
					if (fallbackAction == "Reject")
					{
						return RejectMessage.Reject(context, "550", "5.7.1", "Delivery not authorized, message refused");
					}
				}
				else
				{
					this.HandleDisclaimerWrap(context, disclaimerText);
				}
			}
			return ExecutionControl.Execute;
		}

		private const string DisclaimerHashHeader = "X-MS-Exchange-Organization-Disclaimer-Hash";

		private const string MeetingResponseString = "IPM.Schedule.Meeting.Resp.";

		private static readonly HeaderId[] HeadersToClear = new HeaderId[]
		{
			HeaderId.ContentDisposition,
			HeaderId.ContentDescription,
			HeaderId.ContentMD5
		};

		private static Version minimumVersion = new Version("1.3");

		private static object disclaimerLockVar = new object();

		private static Dictionary<string, DisclaimerEntry> disclaimerLookupTable = new Dictionary<string, DisclaimerEntry>();

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string)
		};
	}
}
