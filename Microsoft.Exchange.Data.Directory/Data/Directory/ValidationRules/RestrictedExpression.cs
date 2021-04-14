using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal class RestrictedExpression : ValidationRuleExpression
	{
		public RestrictedExpression(string queryFilter, ObjectSchema schema, List<Type> applicableObjects) : base(queryFilter, schema, applicableObjects)
		{
		}
	}
}
