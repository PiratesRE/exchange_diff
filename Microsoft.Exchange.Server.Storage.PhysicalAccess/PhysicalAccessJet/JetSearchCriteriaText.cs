using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	public class JetSearchCriteriaText : SearchCriteriaText, IJetSearchCriteria
	{
		public JetSearchCriteriaText(Column lhs, SearchCriteriaText.SearchTextFullness fullnessFlags, SearchCriteriaText.SearchTextFuzzyLevel fuzzynessFlags, Column rhs) : base(lhs, fullnessFlags, fuzzynessFlags, rhs)
		{
		}
	}
}
