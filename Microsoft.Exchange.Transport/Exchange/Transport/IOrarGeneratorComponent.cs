using System;

namespace Microsoft.Exchange.Transport
{
	internal interface IOrarGeneratorComponent : ITransportComponent
	{
		void GenerateOrarMessage(IReadOnlyMailItem mailItem);

		void GenerateOrarMessage(IReadOnlyMailItem mailItem, bool resetTime);
	}
}
