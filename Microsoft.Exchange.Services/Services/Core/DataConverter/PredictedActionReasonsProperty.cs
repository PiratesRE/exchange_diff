using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PredictedActionReasonsProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		public PredictedActionReasonsProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static PredictedActionReasonsProperty CreateCommand(CommandContext commandContext)
		{
			return new PredictedActionReasonsProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("PredictedActionReasonsProperty.ToXml should not be called.");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("PredictedActionReasonsProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObject()
		{
			PredictedActionReasonType[] array = new PredictedActionReasonType[1];
			PredictedActionReasonType[] value = array;
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			if (PropertyCommand.StorePropertyExists(storeObject, ItemSchema.IsClutter))
			{
				bool flag = (bool)PropertyCommand.GetPropertyValueFromStoreObject(storeObject, ItemSchema.IsClutter);
				if (flag && PropertyCommand.StorePropertyExists(storeObject, ItemSchema.InferencePredictedDeleteReasons))
				{
					byte[] rawPredictedActionReasonsArray = (byte[])PropertyCommand.GetPropertyValueFromStoreObject(storeObject, ItemSchema.InferencePredictedDeleteReasons);
					value = PredictedActionReasonsProperty.ConvertStoreValueToServiceValue(rawPredictedActionReasonsArray);
				}
			}
			serviceObject[this.commandContext.PropertyInformation] = value;
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			PredictedActionReasonType[] array = new PredictedActionReasonType[1];
			PredictedActionReasonType[] value = array;
			object obj;
			if (propertyBag.TryGetValue(ItemSchema.IsClutter, out obj) && obj is bool)
			{
				bool flag = (bool)obj;
				object obj2 = null;
				if (flag && propertyBag.TryGetValue(ItemSchema.InferencePredictedDeleteReasons, out obj2))
				{
					value = PredictedActionReasonsProperty.ConvertStoreValueToServiceValue((byte[])obj2);
				}
			}
			serviceObject[this.commandContext.PropertyInformation] = value;
		}

		internal static PredictedActionReasonType[] ToServiceObjectForStorePropertyBag(IStorePropertyBag storePropertyBag)
		{
			bool valueOrDefault = storePropertyBag.GetValueOrDefault<bool>(ItemSchema.IsClutter, false);
			if (valueOrDefault)
			{
				try
				{
					object valueOrDefault2 = storePropertyBag.GetValueOrDefault<object>(ItemSchema.InferencePredictedDeleteReasons, null);
					if (valueOrDefault2 != null)
					{
						return PredictedActionReasonsProperty.ConvertStoreValueToServiceValue(valueOrDefault2 as byte[]);
					}
				}
				catch (NotInBagPropertyErrorException)
				{
				}
			}
			return null;
		}

		internal static PredictedActionReasonType[] ConvertStoreValueToServiceValue(byte[] rawPredictedActionReasonsArray)
		{
			PredictedActionReasonType[] array = new PredictedActionReasonType[1];
			PredictedActionReasonType[] array2 = array;
			if (rawPredictedActionReasonsArray == null || rawPredictedActionReasonsArray.Length <= 3)
			{
				return array2;
			}
			ushort[] predictedActionReasonsArray = PredictedActionReasonsProperty.GetPredictedActionReasonsArray(rawPredictedActionReasonsArray);
			if (predictedActionReasonsArray != null && predictedActionReasonsArray.Length > 0)
			{
				array2 = new PredictedActionReasonType[predictedActionReasonsArray.Length];
				for (int i = 0; i < predictedActionReasonsArray.Length; i++)
				{
					array2[i] = PredictedActionReasonConverter.ConvertToServiceObjectValue(predictedActionReasonsArray[i]);
				}
			}
			return array2;
		}

		private static ushort[] GetPredictedActionReasonsArray(byte[] rawPredictedActionReasonsArray)
		{
			if (rawPredictedActionReasonsArray == null || rawPredictedActionReasonsArray.Length <= 3)
			{
				return null;
			}
			ushort[] array = new ushort[(rawPredictedActionReasonsArray.Length - 2) / 2];
			int num = 0;
			byte[] array2 = new byte[2];
			for (int i = 2; i < rawPredictedActionReasonsArray.Length; i += 2)
			{
				array2[0] = rawPredictedActionReasonsArray[i];
				array2[1] = rawPredictedActionReasonsArray[i + 1];
				array[num] = BitConverter.ToUInt16(array2, 0);
				num++;
			}
			return array;
		}
	}
}
