using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface IContextualBatchDataWriter<T> : IBatchDataWriter<T>, IDisposable
	{
		void EnterDataContext(DataContext dataContext);

		void ExitDataContext(bool errorHappened);

		void ExitPFDataContext(bool errorHappened);
	}
}
