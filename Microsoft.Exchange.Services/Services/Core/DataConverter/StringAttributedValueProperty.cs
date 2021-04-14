using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class StringAttributedValueProperty : SimpleProperty
	{
		protected StringAttributedValueProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static StringAttributedValueProperty CreateCommand(CommandContext commandContext)
		{
			return new StringAttributedValueProperty(commandContext);
		}

		protected override object Parse(string propertyString)
		{
			return null;
		}

		protected override void SetProperty(ServiceObject serviceObject, StoreObject storeObject)
		{
			string propertyString = serviceObject[this.commandContext.PropertyInformation] as string;
			object value = this.Parse(propertyString);
			base.SetPropertyValueOnStoreObject(storeObject, this.propertyDefinition, value);
		}

		protected override void WriteServiceProperty(object propertyValue, ServiceObject serviceObject, PropertyInformation propInfo)
		{
			string propertyValue2 = this.ToString(propertyValue);
			base.WriteServiceProperty(propertyValue2, serviceObject, propInfo);
		}
	}
}
