using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal delegate void BatchOperationCompletedDelegate(List<AmDbOperation> operationList);
}
