using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class SearchEvaluationInProgressException : StoreException
	{
		public SearchEvaluationInProgressException(LID lid, string message) : base(lid, ErrorCodeValue.SearchEvaluationInProgress, message)
		{
		}

		public SearchEvaluationInProgressException(LID lid, string message, Exception innerException) : base(lid, ErrorCodeValue.SearchEvaluationInProgress, message, innerException)
		{
		}
	}
}
