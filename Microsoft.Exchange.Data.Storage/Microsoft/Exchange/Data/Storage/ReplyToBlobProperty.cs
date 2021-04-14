using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ReplyToBlobProperty : SmartPropertyDefinition
	{
		internal ReplyToBlobProperty() : base("ReplyTo", typeof(byte[]), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.MapiReplyToBlob, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			MessageItem messageItem = propertyBag.Context.StoreObject as MessageItem;
			if (messageItem == null)
			{
				return propertyBag.GetValue(InternalSchema.MapiReplyToBlob);
			}
			object blob = ((ReplyTo)messageItem.ReplyTo).Blob;
			if (blob == null)
			{
				return new PropertyError(this, PropertyErrorCode.NotFound);
			}
			return blob;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			MessageItem messageItem = propertyBag.Context.StoreObject as MessageItem;
			if (messageItem != null)
			{
				try
				{
					((ReplyTo)messageItem.ReplyTo).Blob = (byte[])value;
					return;
				}
				catch (CorruptDataException ex)
				{
					throw PropertyError.ToException(ex.LocalizedString, new PropertyError[]
					{
						new PropertyError(InternalSchema.ReplyToBlob, PropertyErrorCode.SetCalculatedPropertyError)
					});
				}
			}
			propertyBag.SetValueWithFixup(InternalSchema.MapiReplyToBlob, (byte[])value);
		}
	}
}
