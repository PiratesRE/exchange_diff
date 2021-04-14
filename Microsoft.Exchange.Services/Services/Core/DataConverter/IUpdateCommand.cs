using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal interface IUpdateCommand : IPropertyCommand
	{
		void Update();

		void PostUpdate();
	}
}
