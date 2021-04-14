using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NullCoreItemContext : ICoreItemContext
	{
		public static ICoreItemContext Instance
		{
			get
			{
				return NullCoreItemContext.instance;
			}
		}

		public void GetContextCharsetDetectionData(StringBuilder stringBuilder, CharsetDetectionDataFlags flags)
		{
			Util.ThrowOnNullArgument(stringBuilder, "stringBuilder");
			EnumValidator.ThrowIfInvalid<CharsetDetectionDataFlags>(flags, "flags");
		}

		private NullCoreItemContext()
		{
		}

		private static NullCoreItemContext instance = new NullCoreItemContext();
	}
}
