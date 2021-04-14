using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Configuration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAggregationReValidator
	{
		bool IsTypeReValidationRequired();

		IEnumerable<KeyValuePair<string, SerializableDataBase>> RevalidatedTypes();
	}
}
