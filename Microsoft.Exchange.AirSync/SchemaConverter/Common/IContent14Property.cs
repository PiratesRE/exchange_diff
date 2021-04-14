using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IContent14Property : IContentProperty, IMIMEDataProperty, IMIMERelatedProperty, IProperty
	{
		string Preview { get; }
	}
}
