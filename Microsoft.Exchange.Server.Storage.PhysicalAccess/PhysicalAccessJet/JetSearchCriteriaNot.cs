using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	public class JetSearchCriteriaNot : SearchCriteriaNot, IJetSearchCriteria
	{
		public JetSearchCriteriaNot(SearchCriteria criteria) : base(criteria)
		{
		}
	}
}
