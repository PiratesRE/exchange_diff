using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class PhoneNumberConverter : BaseConverter
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
			PhoneNumber phoneNumber = propertyValue as PhoneNumber;
			if (phoneNumber == null)
			{
				return null;
			}
			return new PhoneNumber(phoneNumber.Number, phoneNumber.Type);
		}
	}
}
