using System;
using System.Collections;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface INestedData
	{
		IDictionary SubProperties { get; }

		void Clear();
	}
}
