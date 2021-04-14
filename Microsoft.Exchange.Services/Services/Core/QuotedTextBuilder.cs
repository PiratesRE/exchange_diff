using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class QuotedTextBuilder
	{
		internal static string GetForwardReplyHeader(CallContext context, string messageId)
		{
			IdConverter idConverter = new IdConverter(context);
			ItemId baseItemId = new ItemId(messageId, null);
			IdAndSession idAndSession = idConverter.ConvertItemIdToIdAndSessionReadOnly(baseItemId);
			ExTimeZone exTimeZone = idAndSession.Session.ExTimeZone;
			idAndSession.Session.ExTimeZone = ExTimeZoneValue.Parse("Pacific Standard Time").ExTimeZone;
			Item xsoItem = ServiceCommandBase.GetXsoItem(idAndSession.Session, idAndSession.Id, new PropertyDefinition[0]);
			idAndSession.Session.ExTimeZone = exTimeZone;
			CalendarItemBase calendarItemBase;
			XsoDataConverter.TryGetStoreObject<CalendarItemBase>(xsoItem, out calendarItemBase);
			ForwardReplyHeaderOptions headerOptions = new ForwardReplyHeaderOptions();
			return ForwardReplyUtilities.CreateForwardReplyHeader(BodyFormat.TextHtml, xsoItem, headerOptions, xsoItem is MeetingMessage || calendarItemBase != null, context.ClientCulture, "t", null);
		}

		internal static string GetBodyWithQuotedText(CallContext context, string messageBody, List<string> messageIds, List<string> messageBodyColl)
		{
			HtmlConversationBodyScanner htmlConversationBodyScanner = new HtmlConversationBodyScanner();
			htmlConversationBodyScanner.DetectEncodingFromMetaTag = false;
			StringBuilder stringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				using (HtmlWriter htmlWriter = new HtmlWriter(stringWriter))
				{
					for (int i = messageIds.Count - 1; i >= 0; i--)
					{
						string forwardReplyHeader = QuotedTextBuilder.GetForwardReplyHeader(context, messageIds[i]);
						htmlConversationBodyScanner.Load(new StringReader(messageBodyColl[i]));
						htmlWriter.WriteMarkupText("<div>&nbsp;</div>");
						htmlWriter.WriteMarkupText(forwardReplyHeader);
						htmlConversationBodyScanner.WriteAll(htmlWriter);
					}
					htmlWriter.Flush();
				}
				stringWriter.Flush();
			}
			htmlConversationBodyScanner.Load(new StringReader(messageBody));
			StringBuilder stringBuilder2 = new StringBuilder();
			using (StringWriter stringWriter2 = new StringWriter(stringBuilder2))
			{
				using (HtmlWriter htmlWriter2 = new HtmlWriter(stringWriter2))
				{
					htmlConversationBodyScanner.WriteAll(htmlWriter2);
					htmlWriter2.WriteMarkupText(stringBuilder.ToString());
					htmlWriter2.Flush();
				}
				stringWriter2.Flush();
			}
			return stringBuilder2.ToString();
		}
	}
}
