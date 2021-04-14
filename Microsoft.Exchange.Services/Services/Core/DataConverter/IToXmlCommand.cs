using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal interface IToXmlCommand : IPropertyCommand
	{
		void ToXml();
	}
}
