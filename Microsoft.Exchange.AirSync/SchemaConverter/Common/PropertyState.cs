using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal enum PropertyState
	{
		Uninitialized,
		SetToDefault,
		Modified,
		Unmodified,
		Stream,
		NotSupported
	}
}
