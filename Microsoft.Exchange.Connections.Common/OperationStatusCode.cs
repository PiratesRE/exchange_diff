using System;

namespace Microsoft.Exchange.Connections.Common
{
	public enum OperationStatusCode
	{
		None,
		Success,
		ErrorInvalidCredentials,
		ErrorCannotCommunicateWithRemoteServer,
		ErrorInvalidRemoteServer,
		ErrorUnsupportedProtocolVersion
	}
}
