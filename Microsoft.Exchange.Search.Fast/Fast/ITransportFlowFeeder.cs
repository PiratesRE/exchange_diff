using System;
using System.IO;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Fast
{
	internal interface ITransportFlowFeeder
	{
		void ProcessMessage(Stream mimeStream, Stream propertyStream, TransportFlowMessageFlags transportFlowMessageFlags);
	}
}
