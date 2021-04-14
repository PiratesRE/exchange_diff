using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.PeopleIKnowService
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PersonaComparerByRelevanceScore : IComparer<string>
	{
		internal PersonaComparerByRelevanceScore(PeopleIKnowGraph peopleIKnowGraph)
		{
			ArgumentValidator.ThrowIfNull("peopleIKnowGraph", peopleIKnowGraph);
			this.emailAddressToRelevanceScoreMapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			RelevantPerson[] relevantPeople = peopleIKnowGraph.RelevantPeople;
			if (relevantPeople != null)
			{
				foreach (RelevantPerson relevantPerson in relevantPeople)
				{
					if (!string.IsNullOrEmpty(relevantPerson.EmailAddress))
					{
						this.emailAddressToRelevanceScoreMapping[relevantPerson.EmailAddress] = relevantPerson.RelevanceScore;
					}
				}
			}
		}

		public int Compare(string emailAddress1, string emailAddress2)
		{
			int maxValue;
			if (!this.emailAddressToRelevanceScoreMapping.TryGetValue(emailAddress1, out maxValue))
			{
				maxValue = int.MaxValue;
			}
			int maxValue2;
			if (!this.emailAddressToRelevanceScoreMapping.TryGetValue(emailAddress2, out maxValue2))
			{
				maxValue2 = int.MaxValue;
			}
			return Comparer<int>.Default.Compare(maxValue, maxValue2);
		}

		internal const int IrrelevantScore = 2147483647;

		private readonly Dictionary<string, int> emailAddressToRelevanceScoreMapping;
	}
}
