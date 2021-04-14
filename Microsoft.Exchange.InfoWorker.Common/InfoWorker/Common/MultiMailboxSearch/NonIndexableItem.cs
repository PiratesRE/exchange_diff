using System;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Mdb;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class NonIndexableItem
	{
		public MdbItemIdentity CompositeId { get; set; }

		public ItemId ItemId { get; set; }

		public ItemIndexError ErrorCode { get; set; }

		public string ErrorDescription { get; set; }

		public bool IsPartiallyIndexed { get; set; }

		public bool IsPermanentFailure { get; set; }

		public int AttemptCount { get; set; }

		public DateTime? LastAttemptTime { get; set; }

		public string AdditionalInfo { get; set; }

		public string SortValue { get; set; }

		public static ItemIndexError ConvertSearchErrorCode(EvaluationErrors searchErrorCode)
		{
			return NonIndexableItem.ConvertSearchErrorCode(searchErrorCode.ToString());
		}

		public static ItemIndexError ConvertSearchErrorCode(string searchErrorCode)
		{
			ItemIndexError result = ItemIndexError.None;
			Enum.TryParse<ItemIndexError>(searchErrorCode, true, out result);
			return result;
		}
	}
}
