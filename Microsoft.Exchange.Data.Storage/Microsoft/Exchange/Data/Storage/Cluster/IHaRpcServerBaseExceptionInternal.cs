using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Cluster
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IHaRpcServerBaseExceptionInternal
	{
		string MessageInternal { get; }

		string OriginatingServer { set; }

		string OriginatingStackTrace { set; }

		string DatabaseName { set; }
	}
}
