using System;

namespace Microsoft.Exchange.Data.TextConverters
{
	internal interface IProducerConsumer
	{
		void Run();

		bool Flush();
	}
}
