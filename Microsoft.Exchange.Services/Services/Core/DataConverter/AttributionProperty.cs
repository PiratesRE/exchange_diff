using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class AttributionProperty : SimpleProperty
	{
		protected AttributionProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static AttributionProperty CreateCommand(CommandContext commandContext)
		{
			return new AttributionProperty(commandContext);
		}

		protected override object Parse(string propertyString)
		{
			return null;
		}

		protected override void SetProperty(ServiceObject serviceObject, StoreObject storeObject)
		{
		}

		protected override void WriteServiceProperty(object propertyValue, ServiceObject serviceObject, PropertyInformation propInfo)
		{
		}
	}
}
