using System;

namespace Microsoft.Exchange.Transport
{
	internal interface IStoreDriverSubmission : IStartableTransportComponent, ITransportComponent
	{
		void ExpireOldSubmissionConnections();

		void Retire();
	}
}
