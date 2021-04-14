using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class TransportConfigContainerCreator : ConfigurableObjectCreator
	{
		internal override IList<string> GetProperties(string fullName)
		{
			return new string[]
			{
				"Name",
				"WhenChanged",
				"MaxSendSize",
				"MaxReceiveSize",
				"MaxRecipientEnvelopeLimit",
				"MaxDumpsterSizePerDatabase",
				"MaxDumpsterTime",
				"ExternalPostmasterAddress",
				"GenerateCopyOfDSNFor",
				"InternalSMTPServers"
			};
		}
	}
}
