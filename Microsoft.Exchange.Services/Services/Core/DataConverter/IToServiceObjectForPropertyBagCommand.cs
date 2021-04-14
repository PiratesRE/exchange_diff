using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal interface IToServiceObjectForPropertyBagCommand : IPropertyCommand
	{
		void ToServiceObjectForPropertyBag();
	}
}
