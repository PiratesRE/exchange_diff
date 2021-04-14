using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IIntegerProperty : IProperty
	{
		int IntegerData { get; }
	}
}
