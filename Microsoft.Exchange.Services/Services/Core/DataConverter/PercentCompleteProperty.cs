using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class PercentCompleteProperty : SimpleProperty
	{
		private PercentCompleteProperty(CommandContext commandContext, BaseConverter converter) : base(commandContext, converter)
		{
		}

		public new static PercentCompleteProperty CreateCommand(CommandContext commandContext)
		{
			return new PercentCompleteProperty(commandContext, new DoubleConverter());
		}

		protected override object GetPropertyValue(StoreObject storeObject)
		{
			double decimalValue = (double)PropertyCommand.GetPropertyValueFromStoreObject(storeObject, this.propertyDefinition);
			return this.DecimalToPercent(decimalValue);
		}

		protected override object PreparePropertyBagValue(object propertyValue)
		{
			if (propertyValue is double)
			{
				double decimalValue = (double)propertyValue;
				return this.DecimalToPercent(decimalValue);
			}
			return propertyValue;
		}

		protected override object Parse(string propertyString)
		{
			object result;
			try
			{
				result = double.Parse(propertyString, CultureInfo.InvariantCulture) / 100.0;
			}
			catch (OverflowException innerException)
			{
				throw new InvalidValueForPropertyException(CoreResources.IDs.ErrorInvalidValueForProperty, innerException);
			}
			return result;
		}

		protected override void SetStoreObjectProperty(StoreObject storeObject, PropertyDefinition propertyDefinition, object value)
		{
			Task task = storeObject as Task;
			double num = (double)value;
			if (num < 0.0 || num > 1.0)
			{
				throw new InvalidPercentCompleteValueException();
			}
			if (num == 1.0)
			{
				CompleteDateProperty.SetStatusCompleted(task, ExDateTime.GetNow(task.Session.ExTimeZone));
				return;
			}
			task.PercentComplete = num;
		}

		private string DecimalToPercent(double decimalValue)
		{
			return (decimalValue * 100.0).ToString(CultureInfo.InvariantCulture);
		}
	}
}
