using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface IInterruptControl
	{
		bool WantToInterrupt { get; }

		void RegisterRead(bool probe, TableClass tableClass);

		void RegisterWrite(TableClass tableClass);

		void Reset();
	}
}
