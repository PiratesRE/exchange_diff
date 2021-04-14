using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	public class JetSearchCriteriaOr : SearchCriteriaOr, IJetSearchCriteria
	{
		public JetSearchCriteriaOr(params SearchCriteria[] nestedCriteria) : base(nestedCriteria)
		{
		}
	}
}
