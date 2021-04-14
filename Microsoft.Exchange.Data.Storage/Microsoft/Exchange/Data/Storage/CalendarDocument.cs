using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarDocument
	{
		private CalendarDocument()
		{
		}

		internal static bool ICalToItem(Stream iCalStream, Item item, InboundAddressCache addressCache, bool suppressBodyPromotion, string charsetName, out LocalizedString errorMessage)
		{
			IList<LocalizedString> list = new List<LocalizedString>();
			string text;
			IEnumerable<Item> enumerable = CalendarDocument.InternalICalToItems(iCalStream, charsetName, addressCache, suppressBodyPromotion ? new uint?(0U) : null, false, () => item, list, out text);
			bool flag = false;
			using (IEnumerator<Item> enumerator = enumerable.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Item item2 = enumerator.Current;
					if (item2.Validate().Length != 0)
					{
						ExTraceGlobals.ICalTracer.TraceDebug(0L, "CalendarDocument::ICalToItem. Validation error for promoted item.");
						list.Add(ServerStrings.ValidationFailureAfterPromotion(string.Empty));
					}
					flag = (list.Count == 0);
				}
			}
			if (!flag)
			{
				CalendarDocument.AttachICalToItem(iCalStream, item);
			}
			if (list.Count == 0)
			{
				errorMessage = LocalizedString.Empty;
			}
			else if (list.Count == 1)
			{
				errorMessage = list[0];
			}
			else
			{
				errorMessage = LocalizedString.Join(Environment.NewLine, list.Cast<object>().ToArray<object>());
			}
			return flag;
		}

		internal static IEnumerable<Item> InternalICalToItems(Stream iCalStream, string charsetName, InboundAddressCache addressCache, uint? maxBodyLength, bool hasExceptionPromotion, Func<Item> getItem, IList<LocalizedString> errorStream, out string calendarName)
		{
			Util.ThrowOnNullArgument(iCalStream, "iCalStream");
			Util.ThrowOnNullArgument(addressCache, "addressCache");
			Util.ThrowOnNullArgument(errorStream, "errorStream");
			bool flag = false;
			LocalizedException ex = null;
			Charset charset = Charset.GetCharset(charsetName ?? "utf-8");
			CalendarDocument calendarDocument = new CalendarDocument();
			IEnumerable<Item> result;
			using (CalendarReader calendarReader = new CalendarReader(new StreamWrapper(iCalStream, false), charset.Name, CalendarComplianceMode.Loose))
			{
				ICalInboundContext calInboundContext = new ICalInboundContext(charset, errorStream, addressCache, addressCache.Options, calendarReader, maxBodyLength, hasExceptionPromotion);
				try
				{
					flag = (calendarDocument.Parse(calInboundContext) && calendarDocument.vCalendar.Validate());
				}
				catch (InvalidCalendarDataException ex2)
				{
					ex = ex2;
				}
				calendarName = calInboundContext.CalendarName;
				if (!flag)
				{
					if (ex != null)
					{
						ExTraceGlobals.ICalTracer.TraceError<string>((long)calendarDocument.GetHashCode(), "CalendarDocument::InternalICalToItems. Found exception: '{0}'.", ex.Message);
						errorStream.Add(ex.LocalizedString);
					}
					else if (errorStream.Count != 0)
					{
						ExTraceGlobals.ICalTracer.TraceError<int>((long)calendarDocument.GetHashCode(), "CalendarDocument::InternalICalToItems. {0} error found.", errorStream.Count);
						errorStream.Add(ServerStrings.InvalidICalElement("VCALENDAR"));
					}
					result = Array<CalendarItemBase>.Empty;
				}
				else
				{
					result = calendarDocument.vCalendar.Promote(getItem);
				}
			}
			return result;
		}

		internal static ReadOnlyCollection<AttachmentLink> ItemToICal(Item item, ReadOnlyCollection<AttachmentLink> existingAttachmentLinks, OutboundAddressCache addressCache, Stream stream, string charsetName, OutboundConversionOptions outboundConversionOptions)
		{
			ReadOnlyCollection<AttachmentLink> readOnlyCollection = AttachmentLink.MergeAttachmentLinks(existingAttachmentLinks, item.AttachmentCollection.CoreAttachmentCollection);
			foreach (AttachmentLink attachmentLink in readOnlyCollection)
			{
				if (string.IsNullOrEmpty(attachmentLink.ContentId))
				{
					attachmentLink.ContentId = AttachmentLink.CreateContentId(item.CoreItem, attachmentLink.AttachmentId, outboundConversionOptions.ImceaEncapsulationDomain);
				}
			}
			return CalendarDocument.InternalItemsToICal(null, new Item[]
			{
				item
			}, readOnlyCollection, addressCache, false, stream, new List<LocalizedString>(), charsetName, outboundConversionOptions);
		}

		internal static ReadOnlyCollection<AttachmentLink> InternalItemsToICal(string calendarName, IList<Item> items, ReadOnlyCollection<AttachmentLink> existingAttachmentLinks, OutboundAddressCache addressCache, bool suppressExceptionAndAttachmentDemotion, Stream stream, List<LocalizedString> errorStream, string charsetName, OutboundConversionOptions outboundConversionOptions)
		{
			Util.ThrowOnNullArgument(items, "items");
			if (!suppressExceptionAndAttachmentDemotion && items.Count != 1)
			{
				throw new ArgumentException("Non suppressExceptionAndAttachmentDemotion mode should have one and only one item to demote");
			}
			Charset charset = Charset.GetCharset(charsetName);
			CalendarDocument calendarDocument = new CalendarDocument();
			using (CalendarWriter calendarWriter = new CalendarWriter(new StreamWrapper(stream, false), charset.Name))
			{
				ICalOutboundContext outboundContext = new ICalOutboundContext(charset, errorStream, addressCache, outboundConversionOptions, calendarWriter, calendarName, existingAttachmentLinks, suppressExceptionAndAttachmentDemotion);
				calendarDocument.Demote(outboundContext, items);
			}
			return existingAttachmentLinks;
		}

		private static void AttachICalToItem(Stream iCalStream, Item item)
		{
			iCalStream.Seek(0L, SeekOrigin.Begin);
			using (StreamAttachment streamAttachment = (StreamAttachment)item.AttachmentCollection.Create(AttachmentType.Stream))
			{
				streamAttachment.FileName = CalendarUtil.NotSupportedInboundIcal;
				streamAttachment[AttachmentSchema.FailedInboundICalAsAttachment] = true;
				using (Stream contentStream = streamAttachment.GetContentStream(PropertyOpenMode.Create))
				{
					Util.StreamHandler.CopyStreamData(iCalStream, contentStream);
				}
				streamAttachment.Save();
			}
		}

		private bool Parse(ICalInboundContext inboundContext)
		{
			bool result = false;
			if (inboundContext.Reader.ReadNextComponent() && inboundContext.Reader.ComponentId == ComponentId.VCalendar)
			{
				this.vCalendar = new VCalendar(inboundContext);
				if (this.vCalendar.Parse(inboundContext.Reader) && !inboundContext.Reader.ReadNextComponent())
				{
					result = true;
				}
			}
			return result;
		}

		private void Demote(ICalOutboundContext outboundContext, IList<Item> items)
		{
			this.vCalendar = new VCalendar(outboundContext);
			this.vCalendar.Demote(items);
		}

		private VCalendar vCalendar;
	}
}
