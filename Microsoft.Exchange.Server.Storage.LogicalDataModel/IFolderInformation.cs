using System;
using System.Collections.Generic;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public interface IFolderInformation
	{
		ExchangeShortId Fid { get; }

		ExchangeShortId ParentFid { get; }

		int MailboxNumber { get; }

		bool IsSearchFolder { get; }

		string DisplayName { get; }

		bool IsPartOfContentIndexing { get; }

		bool IsInternalAccess { get; }

		long MessageCount { get; }

		SecurityDescriptor SecurityDescriptor { get; }

		IEnumerable<ExchangeShortId> AllDescendantFolderIds();
	}
}
