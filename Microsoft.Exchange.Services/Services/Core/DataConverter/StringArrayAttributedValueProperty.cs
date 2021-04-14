using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class StringArrayAttributedValueProperty : SimpleProperty
	{
		protected StringArrayAttributedValueProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static StringArrayAttributedValueProperty CreateCommand(CommandContext commandContext)
		{
			return new StringArrayAttributedValueProperty(commandContext);
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
