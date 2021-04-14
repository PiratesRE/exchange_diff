using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal class NullTaskOutputHelper : NullLogTraceHelper, ITaskOutputHelper, ILogTraceHelper
	{
		public new static ITaskOutputHelper GetNullLogger()
		{
			return NullTaskOutputHelper.s_nullLogger;
		}

		public void WriteErrorSimple(Exception error)
		{
			base.AppendLogMessage("Exception! {0}", new object[]
			{
				error
			});
			throw error;
		}

		public void WriteWarning(LocalizedString locString)
		{
			base.AppendLogMessage(locString);
		}

		public void WriteProgressSimple(LocalizedString locString)
		{
			base.AppendLogMessage(locString);
		}

		private static NullTaskOutputHelper s_nullLogger = new NullTaskOutputHelper();
	}
}
