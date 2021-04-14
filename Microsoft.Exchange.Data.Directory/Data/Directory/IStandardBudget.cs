using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal interface IStandardBudget : IBudget, IDisposable
	{
		CostHandle StartConnection(string callerInfo);

		void EndConnection();
	}
}
