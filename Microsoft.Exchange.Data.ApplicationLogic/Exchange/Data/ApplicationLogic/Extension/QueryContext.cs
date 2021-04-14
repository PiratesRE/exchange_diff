using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage.Principal;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	internal abstract class QueryContext
	{
		internal string Domain { get; set; }

		internal string DeploymentId { get; set; }

		internal OrgEmptyMasterTableCache OrgEmptyMasterTableCache { get; set; }

		internal bool IsUserScope { get; set; }

		internal IExchangePrincipal ExchangePrincipal { get; set; }

		internal CultureInfo CultureInfo { get; set; }

		internal string ClientInfoString { get; set; }
	}
}
