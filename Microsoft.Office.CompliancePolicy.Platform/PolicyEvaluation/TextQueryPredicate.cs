using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class TextQueryPredicate : PredicateCondition
	{
		public TextQueryPredicate(string textQuery) : this(new Property("QueryProperty", typeof(string)), new List<string>
		{
			textQuery
		})
		{
			this.TextQuery = textQuery;
		}

		internal TextQueryPredicate(Property property, List<string> entries) : base(property, entries)
		{
			this.TextQuery = string.Join(" OR ", entries);
		}

		public override string Name
		{
			get
			{
				return "textQueryMatch";
			}
		}

		public override ConditionType ConditionType
		{
			get
			{
				return ConditionType.Predicate;
			}
		}

		public string TextQuery
		{
			get
			{
				return this.textQuery;
			}
			set
			{
				if (value.Length > this.MaxSize)
				{
					throw new CompliancePolicyValidationException("Text query length of {0} exceeds maximum allowed length {1}", new object[]
					{
						value.Length,
						this.MaxSize
					});
				}
				this.textQuery = value;
			}
		}

		public int MaxSize
		{
			get
			{
				return this.maxSize;
			}
			set
			{
				this.maxSize = value;
			}
		}

		public override bool Evaluate(PolicyEvaluationContext context)
		{
			return true;
		}

		private int maxSize = 16384;

		private string textQuery;
	}
}
