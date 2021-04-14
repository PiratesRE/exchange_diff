using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ConversationIdProperty : SmartPropertyDefinition
	{
		internal ConversationIdProperty(PropertyTagPropertyDefinition conversationIdPropertyDefinition, string propertyName) : base(propertyName, typeof(ConversationId), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(conversationIdPropertyDefinition, PropertyDependencyType.NeedForRead)
		})
		{
			this.conversationIdPropertyDefinition = conversationIdPropertyDefinition;
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object value = propertyBag.GetValue(this.conversationIdPropertyDefinition);
			byte[] array = value as byte[];
			if (array != null)
			{
				object result;
				try
				{
					result = ConversationId.Create(array);
				}
				catch (CorruptDataException)
				{
					result = new PropertyError(this, PropertyErrorCode.CorruptedData);
				}
				return result;
			}
			PropertyError propertyError = (PropertyError)value;
			if (propertyError.PropertyErrorCode == PropertyErrorCode.NotEnoughMemory)
			{
				return new PropertyError(this, PropertyErrorCode.CorruptedData);
			}
			return new PropertyError(this, propertyError.PropertyErrorCode);
		}

		internal override QueryFilter NativeFilterToSmartFilter(QueryFilter filter)
		{
			return this.NativeFilterToConversationIdBasedSmartFilter(filter, this.conversationIdPropertyDefinition);
		}

		internal override QueryFilter SmartFilterToNativeFilter(SinglePropertyFilter filter)
		{
			return this.ConversationIdBasedSmartFilterToNativeFilter(filter, this.conversationIdPropertyDefinition);
		}

		protected override NativeStorePropertyDefinition GetSortProperty()
		{
			return this.conversationIdPropertyDefinition;
		}

		public override StorePropertyCapabilities Capabilities
		{
			get
			{
				return StorePropertyCapabilities.All;
			}
		}

		private PropertyTagPropertyDefinition conversationIdPropertyDefinition;
	}
}
