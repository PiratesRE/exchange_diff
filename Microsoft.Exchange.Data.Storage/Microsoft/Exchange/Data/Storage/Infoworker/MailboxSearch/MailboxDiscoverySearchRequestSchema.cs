using System;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxDiscoverySearchRequestSchema : EwsStoreObjectSchema
	{
		public static GuidNamePropertyDefinition CreateStorePropertyDefinition(EwsStoreObjectPropertyDefinition ewsStorePropertyDefinition)
		{
			ExtendedPropertyDefinition extendedPropertyDefinition = (ExtendedPropertyDefinition)ewsStorePropertyDefinition.StorePropertyDefinition;
			Type propertyType = ((ewsStorePropertyDefinition.PropertyDefinitionFlags & PropertyDefinitionFlags.MultiValued) == PropertyDefinitionFlags.MultiValued) ? ewsStorePropertyDefinition.Type.MakeArrayType() : ewsStorePropertyDefinition.Type;
			return GuidNamePropertyDefinition.InternalCreate(ewsStorePropertyDefinition.Name, propertyType, MailboxDiscoverySearchRequestSchema.GetMapiPropType(extendedPropertyDefinition.MapiType), extendedPropertyDefinition.PropertySetId.Value, extendedPropertyDefinition.Name, PropertyFlags.None, NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck, false, PropertyDefinitionConstraint.None);
		}

		private static PropType GetMapiPropType(MapiPropertyType ewsMapiPropertyType)
		{
			PropType result;
			if (ewsMapiPropertyType != 14)
			{
				switch (ewsMapiPropertyType)
				{
				case 25:
					result = PropType.String;
					break;
				case 26:
					result = PropType.StringArray;
					break;
				default:
					throw new NotImplementedException();
				}
			}
			else
			{
				result = PropType.Int;
			}
			return result;
		}

		public static readonly EwsStoreObjectPropertyDefinition AsynchronousActionRequest = new EwsStoreObjectPropertyDefinition("AsynchronousActionRequest", ExchangeObjectVersion.Exchange2012, typeof(ActionRequestType), PropertyDefinitionFlags.None, ActionRequestType.None, ActionRequestType.None, MailboxDiscoverySearchRequestExtendedStoreSchema.AsynchronousActionRequest);

		public static readonly EwsStoreObjectPropertyDefinition AsynchronousActionRbacContext = new EwsStoreObjectPropertyDefinition("AsynchronousActionRbacContext", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, null, null, MailboxDiscoverySearchRequestExtendedStoreSchema.AsynchronousActionRbacContext);
	}
}
