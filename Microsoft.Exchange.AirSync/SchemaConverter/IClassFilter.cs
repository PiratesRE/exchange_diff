using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.AirSync.SchemaConverter
{
	internal interface IClassFilter
	{
		QueryFilter SupportedClassFilter { get; }
	}
}
