using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal interface IServerDataObject : IPropertyContainer, IProperty
	{
		void Bind(object item);

		void Unbind();

		bool CanConvertItemClassUsingCurrentSchema(string itemClass);

		PropertyDefinition[] GetPrefetchProperties();

		void SetChangedProperties(PropertyDefinition[] changedProperties);

		IProperty GetPropBySchemaLinkId(int id);
	}
}
