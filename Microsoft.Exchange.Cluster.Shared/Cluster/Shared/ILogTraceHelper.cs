using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal interface ILogTraceHelper
	{
		void AppendLogMessage(LocalizedString locMessage);

		void AppendLogMessage(string englishMessage, params object[] args);

		void WriteVerbose(LocalizedString locString);
	}
}
