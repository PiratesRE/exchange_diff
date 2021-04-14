using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public enum RowStatsTableClassIndex
	{
		Other,
		Temp,
		TableFunction,
		LazyIndex,
		Message,
		Attachment,
		Folder,
		PseudoIndexMaintenance,
		Events,
		MaxValue
	}
}
