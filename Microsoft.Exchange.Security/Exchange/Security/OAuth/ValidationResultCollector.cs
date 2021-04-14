using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Security.OAuth
{
	internal class ValidationResultCollector
	{
		public static ValidationResultCollector NullInstance
		{
			get
			{
				return ValidationResultCollector.nullInstance;
			}
		}

		public ValidationResultCollector()
		{
			this.nodes = new List<ValidationResultNode>(20);
		}

		public virtual void Add(LocalizedString task, LocalizedString detail, ResultType resultType)
		{
			this.nodes.Add(new ValidationResultNode(task, detail, resultType));
		}

		public IEnumerable<ValidationResultNode> Results
		{
			get
			{
				return this.nodes;
			}
		}

		private static readonly ValidationResultCollector nullInstance = new ValidationResultCollector.NullValidationResultCollector();

		private List<ValidationResultNode> nodes;

		internal sealed class NullValidationResultCollector : ValidationResultCollector
		{
			public override void Add(LocalizedString task, LocalizedString detail, ResultType resultType)
			{
			}
		}
	}
}
