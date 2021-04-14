using System;

namespace Microsoft.Exchange.Connections.Eas.Commands
{
	public enum AllowableEasCommandHttpStatus
	{
		OK = 200,
		ActiveSyncRedirect = 451,
		ServiceUnavailable = 503
	}
}
