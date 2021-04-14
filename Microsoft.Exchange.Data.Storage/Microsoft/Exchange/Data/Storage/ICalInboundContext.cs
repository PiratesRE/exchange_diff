using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ICalInboundContext : ICalContext
	{
		public ICalInboundContext(Charset charset, IList<LocalizedString> errorStream, InboundAddressCache addressCache, InboundConversionOptions options, CalendarReader reader, uint? maxBodyLength, bool hasExceptionPromotion) : base(charset, errorStream, addressCache)
		{
			Util.ThrowOnNullArgument(options, "options");
			if (!options.IgnoreImceaDomain)
			{
				Util.ThrowOnNullOrEmptyArgument(options.ImceaEncapsulationDomain, "options.ImceaEncapsulationDomain");
			}
			Util.ThrowOnNullArgument(reader, "reader");
			this.Options = options;
			this.Reader = reader;
			this.MaxBodyLength = maxBodyLength;
			this.HasExceptionPromotion = hasExceptionPromotion;
			this.DeclaredTimeZones = new Dictionary<string, ExTimeZone>(StringComparer.OrdinalIgnoreCase);
		}

		internal InboundAddressCache AddressCache
		{
			get
			{
				return (InboundAddressCache)base.BaseAddressCache;
			}
		}

		internal readonly InboundConversionOptions Options;

		internal readonly CalendarReader Reader;

		internal readonly uint? MaxBodyLength;

		internal readonly bool HasExceptionPromotion;

		internal readonly Dictionary<string, ExTimeZone> DeclaredTimeZones;
	}
}
