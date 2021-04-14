using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal interface IRestartable
	{
		bool CanRestart();

		void Restart();

		void DisableRestart();
	}
}
