using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal abstract class ValidationRuleExpression
	{
		protected ValidationRuleExpression(string queryFilter, ObjectSchema schema, List<Type> applicableObjects)
		{
			if (string.IsNullOrEmpty(queryFilter))
			{
				throw new ArgumentNullException("queryFilter");
			}
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			this.QueryString = queryFilter;
			this.Schema = schema;
			this.ApplicableObjects = applicableObjects;
		}

		public string QueryString { get; private set; }

		public ObjectSchema Schema { get; private set; }

		public ICollection<Type> ApplicableObjects { get; private set; }

		public QueryFilter QueryFilter { get; internal set; }
	}
}
