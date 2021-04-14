using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IPictureProperty : IProperty
	{
		string PictureData { get; }
	}
}
