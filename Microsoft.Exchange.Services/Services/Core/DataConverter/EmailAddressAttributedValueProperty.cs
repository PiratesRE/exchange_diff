using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class EmailAddressAttributedValueProperty : SimpleProperty
	{
		protected EmailAddressAttributedValueProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static EmailAddressAttributedValueProperty CreateCommand(CommandContext commandContext)
		{
			return new EmailAddressAttributedValueProperty(commandContext);
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
