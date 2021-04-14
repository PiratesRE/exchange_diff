using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class EmailAddressValueConverter : BaseConverter
	{
		public override object ConvertToObject(string propertyString)
		{
			return null;
		}

		public override string ConvertToString(object propertyValue)
		{
			return string.Empty;
		}

		protected override object ConvertToServiceObjectValue(object propertyValue)
		{
			EmailAddress emailAddress = (EmailAddress)propertyValue;
			return new EmailAddressWrapper
			{
				Name = emailAddress.Name,
				EmailAddress = emailAddress.Address,
				RoutingType = emailAddress.RoutingType,
				OriginalDisplayName = emailAddress.OriginalDisplayName
			};
		}
	}
}
