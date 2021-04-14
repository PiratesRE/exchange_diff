using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal enum RequestType
	{
		Local,
		IntraSite,
		CrossSite,
		CrossForest,
		FederatedCrossForest,
		PublicFolder
	}
}
