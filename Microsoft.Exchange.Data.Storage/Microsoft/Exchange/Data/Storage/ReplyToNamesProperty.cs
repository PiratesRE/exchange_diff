using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ReplyToNamesProperty : SmartPropertyDefinition
	{
		internal ReplyToNamesProperty() : base("ReplyToNames", typeof(string), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiReplyToNames, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			MessageItem messageItem = propertyBag.Context.StoreObject as MessageItem;
			if (messageItem == null)
			{
				return propertyBag.GetValue(InternalSchema.MapiReplyToNames);
			}
			object names = ((ReplyTo)messageItem.ReplyTo).Names;
			if (names == null)
			{
				return new PropertyError(this, PropertyErrorCode.NotFound);
			}
			return names;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			MessageItem messageItem = propertyBag.Context.StoreObject as MessageItem;
			if (messageItem != null)
			{
				try
				{
					((ReplyTo)messageItem.ReplyTo).Names = (string)value;
					return;
				}
				catch (CorruptDataException ex)
				{
					throw PropertyError.ToException(ex.LocalizedString, new PropertyError[]
					{
						new PropertyError(InternalSchema.ReplyToNames, PropertyErrorCode.SetCalculatedPropertyError)
					});
				}
			}
			propertyBag.SetValueWithFixup(InternalSchema.MapiReplyToNames, (string)value);
		}
	}
}
