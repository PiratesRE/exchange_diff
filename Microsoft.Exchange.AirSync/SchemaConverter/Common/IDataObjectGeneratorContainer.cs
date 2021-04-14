using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IDataObjectGeneratorContainer
	{
		IDataObjectGenerator SchemaState { get; set; }
	}
}
