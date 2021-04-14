using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Data.Storage.ReliableActions
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ActionQueueProperty : SmartPropertyDefinition
	{
		public ActionQueueProperty(NativeStorePropertyDefinition rawQueueProperty, NativeStorePropertyDefinition queueHasDataFlagProperty) : base("ActionQueue", typeof(ActionInfo[]), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(rawQueueProperty, PropertyDependencyType.AllRead),
			new PropertyDependency(queueHasDataFlagProperty, PropertyDependencyType.AllRead),
			new PropertyDependency(InternalSchema.LastExecutedCalendarInteropAction, PropertyDependencyType.AllRead)
		})
		{
			ArgumentValidator.ThrowIfNull("rawQueueProperty", rawQueueProperty);
			ArgumentValidator.ThrowIfNull("queueHasDataFlagProperty", queueHasDataFlagProperty);
			this.rawQueueProperty = rawQueueProperty;
			this.queueHasDataFlagProperty = queueHasDataFlagProperty;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			ActionInfo[] array = (ActionInfo[])value;
			bool flag = array.IsNullOrEmpty<ActionInfo>();
			propertyBag.SetValue(this.queueHasDataFlagProperty, !flag);
			byte[] propertyValue = null;
			if (!flag)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					JsonConverter.Serialize<ActionInfo[]>(array, memoryStream, null);
					propertyValue = memoryStream.ToArray();
				}
				propertyBag.SetValue(InternalSchema.LastExecutedCalendarInteropAction, array.Last<ActionInfo>().Id);
			}
			propertyBag.SetOrDeleteProperty(this.rawQueueProperty, propertyValue);
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			byte[] largeBinaryProperty = propertyBag.Context.StoreObject.PropertyBag.GetLargeBinaryProperty(this.rawQueueProperty);
			if (largeBinaryProperty.IsNullOrEmpty<byte>())
			{
				return ActionQueueProperty.EmptyQueue;
			}
			object result;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(largeBinaryProperty))
				{
					result = JsonConverter.Deserialize<ActionInfo[]>(memoryStream, null);
				}
			}
			catch (SerializationException ex)
			{
				result = new PropertyError(this, PropertyErrorCode.CorruptedData, ex.ToString());
			}
			return result;
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(this.rawQueueProperty);
			propertyBag.SetValue(this.queueHasDataFlagProperty, false);
		}

		private static readonly ActionInfo[] EmptyQueue = Array<ActionInfo>.Empty;

		private readonly NativeStorePropertyDefinition queueHasDataFlagProperty;

		private readonly NativeStorePropertyDefinition rawQueueProperty;
	}
}
