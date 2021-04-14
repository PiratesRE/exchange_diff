using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal enum PropertyType
	{
		ReadOnly = 1,
		WriteOnly,
		ReadWrite,
		ReadAndRequiredForWrite,
		ReadOnlyForNonDraft
	}
}
