using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class SearchFolderScopeViolationException : StoreException
	{
		public SearchFolderScopeViolationException(LID lid, string message) : base(lid, ErrorCodeValue.SearchFolderScopeViolation, message)
		{
		}

		public SearchFolderScopeViolationException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.SearchFolderScopeViolation, message, innerException)
		{
		}
	}
}
