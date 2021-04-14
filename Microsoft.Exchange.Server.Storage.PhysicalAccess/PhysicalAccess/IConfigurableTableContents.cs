using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface IConfigurableTableContents
	{
		void Configure(bool backwards, StartStopKey startKey);
	}
}
