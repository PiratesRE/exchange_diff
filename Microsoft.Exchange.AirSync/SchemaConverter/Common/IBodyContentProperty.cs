using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IBodyContentProperty : IBodyProperty, IContentProperty, IMIMEDataProperty, IMIMERelatedProperty, IProperty
	{
	}
}
