using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class MeetingResponseType : SmartPropertyDefinition
	{
		internal MeetingResponseType() : base("MeetingResponseType", typeof(ResponseType), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.ItemClass, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.MapiResponseType, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			string itemClass = propertyBag.GetValue(InternalSchema.ItemClass) as string;
			ResponseType responseType;
			if (ObjectClass.IsMeetingPositiveResponse(itemClass))
			{
				responseType = ResponseType.Accept;
			}
			else if (ObjectClass.IsMeetingTentativeResponse(itemClass))
			{
				responseType = ResponseType.Tentative;
			}
			else
			{
				if (!ObjectClass.IsMeetingNegativeResponse(itemClass))
				{
					return propertyBag.GetValue(InternalSchema.MapiResponseType);
				}
				responseType = ResponseType.Decline;
			}
			return responseType;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			propertyBag.SetValueWithFixup(InternalSchema.MapiResponseType, value);
		}
	}
}
