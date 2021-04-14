using System;

namespace Microsoft.Exchange.Data.Storage.Compliance.DAR
{
	public interface IStoreObject
	{
		string Id { get; set; }

		DateTime LastModifiedTime { get; }
	}
}
