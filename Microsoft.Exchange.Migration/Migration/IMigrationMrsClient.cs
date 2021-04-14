using System;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Migration
{
	internal interface IMigrationMrsClient
	{
		bool CanConnectToMrsProxy(Fqdn serverName, Guid mbxGuid, NetworkCredential credentials, out LocalizedException error);
	}
}
