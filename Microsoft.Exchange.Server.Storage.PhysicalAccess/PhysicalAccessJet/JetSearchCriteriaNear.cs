using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	public class JetSearchCriteriaNear : SearchCriteriaNear, IJetSearchCriteria
	{
		public JetSearchCriteriaNear(int distance, bool ordered, SearchCriteriaAnd criteria) : base(distance, ordered, criteria)
		{
		}
	}
}
