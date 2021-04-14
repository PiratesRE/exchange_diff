using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ConverterFlags
	{
		None = 0,
		IsEmbeddedMessage = 1,
		IsStreamToStreamConversion = 2,
		GenerateSkeleton = 4
	}
}
