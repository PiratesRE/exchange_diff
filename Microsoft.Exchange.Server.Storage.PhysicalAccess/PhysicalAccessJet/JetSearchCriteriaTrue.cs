using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	public class JetSearchCriteriaTrue : SearchCriteriaTrue, IJetSearchCriteria
	{
		internal JetSearchCriteriaTrue()
		{
		}

		public static readonly JetSearchCriteriaTrue Instance = new JetSearchCriteriaTrue();
	}
}
