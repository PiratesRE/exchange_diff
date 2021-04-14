using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal interface ITaskOutputHelper : ILogTraceHelper
	{
		void WriteErrorSimple(Exception error);

		void WriteWarning(LocalizedString locString);

		void WriteProgressSimple(LocalizedString locString);
	}
}
