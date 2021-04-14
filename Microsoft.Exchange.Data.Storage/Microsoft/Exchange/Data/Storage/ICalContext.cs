using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ICalContext
	{
		public ICalContext(Charset charset, IList<LocalizedString> errorStream, ConversionAddressCache addressCache)
		{
			Util.ThrowOnNullArgument(charset, "charset");
			Util.ThrowOnNullArgument(errorStream, "errorStream");
			Util.ThrowOnNullArgument(addressCache, "addressCache");
			this.Charset = charset;
			this.errors = errorStream;
			this.BaseAddressCache = addressCache;
		}

		internal void AddError(LocalizedString error)
		{
			this.errors.Add(error);
		}

		private protected ConversionAddressCache BaseAddressCache { protected get; private set; }

		internal readonly Charset Charset;

		internal CalendarMethod Method = CalendarMethod.Publish;

		internal CalendarType Type;

		internal string CalendarName = string.Empty;

		private readonly IList<LocalizedString> errors;
	}
}
