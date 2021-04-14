using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class EnumConverter : BaseConverter
	{
		protected override object ConvertToServiceObjectValue(object propertyValue)
		{
			return this.ConvertToString(propertyValue);
		}
	}
}
