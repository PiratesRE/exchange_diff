using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public enum RowStatsCounterType
	{
		Read,
		Seek,
		Accept,
		Write,
		ReadBytes,
		WriteBytes,
		MaxValue
	}
}
