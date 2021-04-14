using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class MailCommandMessageContextParameters
	{
		public MailCommandMessageContextParameters(string messageContextMailParameterString, Version adrc, Version eprop, Version fastIndex, List<IInboundMessageContextBlob> blobs)
		{
			this.verbatimParameters = messageContextMailParameterString;
			this.AdrcVersion = adrc;
			this.EpropVersion = eprop;
			this.FastIndexVersion = fastIndex;
			this.OrderedListOfBlobs = blobs;
		}

		public readonly string verbatimParameters;

		public readonly Version AdrcVersion;

		public readonly Version EpropVersion;

		public readonly Version FastIndexVersion;

		public readonly List<IInboundMessageContextBlob> OrderedListOfBlobs = new List<IInboundMessageContextBlob>();
	}
}
