using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class FailedToConvertSearchCriteriaToRestrictionException : MultiMailboxSearchException
	{
		public FailedToConvertSearchCriteriaToRestrictionException(string query, Guid database, string error, Exception innerException) : base(Strings.FailedToConvertSearchCriteriaToRestriction(query, database.ToString(), error), innerException)
		{
		}

		protected FailedToConvertSearchCriteriaToRestrictionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
