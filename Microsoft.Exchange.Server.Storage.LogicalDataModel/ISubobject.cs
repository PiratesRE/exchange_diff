using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	internal interface ISubobject
	{
		int ChildNumber { get; }

		long? CurrentInid { get; }

		long? OriginalInid { get; }

		long CurrentSize { get; }

		long OriginalSize { get; }

		SubobjectCollection Subobjects { get; }
	}
}
