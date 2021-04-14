using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IContent16Property : IContent14Property, IContentProperty, IMIMEDataProperty, IMIMERelatedProperty, IProperty
	{
		string BodyString { get; }
	}
}
