using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal interface IToServiceObjectCommand : IPropertyCommand
	{
		void ToServiceObject();
	}
}
