using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal interface ISetCommand : IPropertyCommand
	{
		void Set();

		void SetPhase2();

		void SetPhase3();
	}
}
