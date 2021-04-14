using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	public class JetSearchCriteriaAnd : SearchCriteriaAnd, IJetSearchCriteria
	{
		public JetSearchCriteriaAnd(params SearchCriteria[] nestedCriteria) : base(nestedCriteria)
		{
		}
	}
}
