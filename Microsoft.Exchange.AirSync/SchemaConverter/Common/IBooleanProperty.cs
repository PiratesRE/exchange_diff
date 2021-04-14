using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IBooleanProperty : IProperty
	{
		bool BooleanData { get; }
	}
}
