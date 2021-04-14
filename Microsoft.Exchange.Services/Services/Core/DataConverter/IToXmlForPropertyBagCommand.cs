using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal interface IToXmlForPropertyBagCommand : IPropertyCommand
	{
		void ToXmlForPropertyBag();
	}
}
