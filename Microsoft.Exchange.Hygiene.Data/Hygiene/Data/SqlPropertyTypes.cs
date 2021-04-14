using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	public enum SqlPropertyTypes : sbyte
	{
		Int = 1,
		String,
		DateTime,
		Decimal,
		Blob,
		Boolean,
		Guid,
		Long
	}
}
