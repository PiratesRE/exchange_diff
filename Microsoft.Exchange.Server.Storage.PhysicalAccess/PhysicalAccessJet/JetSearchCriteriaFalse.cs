using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	public class JetSearchCriteriaFalse : SearchCriteriaFalse, IJetSearchCriteria
	{
		internal JetSearchCriteriaFalse()
		{
		}

		public static readonly JetSearchCriteriaFalse Instance = new JetSearchCriteriaFalse();
	}
}
