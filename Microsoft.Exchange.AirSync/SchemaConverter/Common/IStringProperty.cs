using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IStringProperty : IProperty
	{
		string StringData { get; }
	}
}
