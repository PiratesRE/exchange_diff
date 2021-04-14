using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	internal abstract class MapiObjectSchema : ObjectSchema
	{
		public static readonly MapiPropertyDefinition Identity = new MapiPropertyDefinition("Identity", typeof(MapiObjectId), PropTag.Null, MapiPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ObjectState = new MapiPropertyDefinition("ObjectState", typeof(ObjectState), PropTag.Null, MapiPropertyDefinitionFlags.None, Microsoft.Exchange.Data.ObjectState.New, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly MapiPropertyDefinition ExchangeVersion = new MapiPropertyDefinition("ExchangeVersion", typeof(ExchangeObjectVersion), PropTag.Null, MapiPropertyDefinitionFlags.None, ExchangeObjectVersion.Exchange2003, ExchangeObjectVersion.Exchange2003, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
