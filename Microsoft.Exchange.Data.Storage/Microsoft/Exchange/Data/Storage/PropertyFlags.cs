using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum PropertyFlags
	{
		None = 0,
		ReadOnly = 1,
		FilterOnly = 2,
		HasMimeEncoding = 4,
		Sortable = 8,
		Streamable = 16,
		SetIfNotChanged = 32,
		TrackChange = 64,
		UserDefined = 65535,
		Multivalued = 65536,
		Binary = 131072,
		Transmittable = 262144,
		Custom = 524288,
		Automatic = 2147418112
	}
}
