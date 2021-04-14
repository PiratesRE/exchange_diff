using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ICalOutboundContext : ICalContext
	{
		internal ICalOutboundContext(Charset charset, IList<LocalizedString> errorStream, OutboundAddressCache addressCache, OutboundConversionOptions options, CalendarWriter calendarWriter, string calendarName, ReadOnlyCollection<AttachmentLink> attachmentLinks, bool suppressExceptionAndAttachmentDemotion) : base(charset, errorStream, addressCache)
		{
			Util.ThrowOnNullArgument(options, "options");
			Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			Util.ThrowOnNullArgument(calendarWriter, "calendarWriter");
			this.Options = options;
			this.Writer = calendarWriter;
			this.CalendarName = calendarName;
			this.AttachmentLinks = attachmentLinks;
			this.SuppressExceptionAndAttachmentDemotion = suppressExceptionAndAttachmentDemotion;
		}

		internal OutboundAddressCache AddressCache
		{
			get
			{
				return (OutboundAddressCache)base.BaseAddressCache;
			}
		}

		internal readonly OutboundConversionOptions Options;

		internal readonly CalendarWriter Writer;

		internal readonly ReadOnlyCollection<AttachmentLink> AttachmentLinks;

		internal readonly bool SuppressExceptionAndAttachmentDemotion;
	}
}
