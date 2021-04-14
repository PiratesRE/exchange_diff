using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface IInboundProxyDestinationTracker
	{
		void IncrementProxyCount(string destination);

		void DecrementProxyCount(string destination);

		bool ShouldRejectMessage(string destination, out SmtpResponse rejectResponse);

		bool TryGetDiagnosticInfo(DiagnosableParameters parameters, out XElement diagnosticInfo);

		void UpdateReceiveConnectors(IEnumerable<ReceiveConnector> receiveConnectors);
	}
}
