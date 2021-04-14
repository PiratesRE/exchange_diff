using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public abstract class PsContainsWordsPredicate : PsComplianceRulePredicateBase
	{
		protected PsContainsWordsPredicate(IEnumerable<string> words)
		{
			this.Words = new MultiValuedProperty<string>(words);
			this.Words.SetIsReadOnly(false, null);
		}

		public MultiValuedProperty<string> Words { get; protected set; }
	}
}
