using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface INestedProperty
	{
		INestedData NestedData { get; }
	}
}
