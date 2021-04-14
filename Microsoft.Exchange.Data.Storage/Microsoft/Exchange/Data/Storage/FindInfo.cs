using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct FindInfo<RESULT_TYPE> : IDisposable
	{
		public FindInfo(FindStatus findStatus, RESULT_TYPE result)
		{
			this.FindStatus = findStatus;
			this.Result = result;
		}

		public void Dispose()
		{
			IDisposable disposable = this.Result as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		public readonly FindStatus FindStatus;

		public readonly RESULT_TYPE Result;
	}
}
