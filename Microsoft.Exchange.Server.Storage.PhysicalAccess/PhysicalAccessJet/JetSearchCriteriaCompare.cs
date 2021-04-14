using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	public class JetSearchCriteriaCompare : SearchCriteriaCompare, IJetSearchCriteria
	{
		public JetSearchCriteriaCompare(Column lhs, SearchCriteriaCompare.SearchRelOp op, Column rhs) : base(lhs, op, rhs)
		{
		}
	}
}
