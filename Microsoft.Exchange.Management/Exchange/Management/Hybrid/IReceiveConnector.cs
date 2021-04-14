using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IReceiveConnector : IEntity<IReceiveConnector>
	{
		ADObjectId Server { get; }

		SmtpX509Identifier TlsCertificateName { get; }

		SmtpReceiveDomainCapabilities TlsDomainCapabilities { get; }
	}
}
