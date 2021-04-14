using System;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal delegate void AmReportStatusDelegate(IADDatabase db, AmDbActionStatus status);
}
