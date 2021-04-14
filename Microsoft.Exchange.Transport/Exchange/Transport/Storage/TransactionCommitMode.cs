using System;

namespace Microsoft.Exchange.Transport.Storage
{
	internal enum TransactionCommitMode
	{
		Lazy,
		ShortLatencyLazy,
		MediumLatencyLazy,
		Immediate
	}
}
