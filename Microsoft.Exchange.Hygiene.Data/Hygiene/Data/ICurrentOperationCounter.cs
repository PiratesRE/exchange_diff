using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	public interface ICurrentOperationCounter : IDisposable
	{
		void Increment();

		void Decrement();
	}
}
