using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal class OverridingAllowExpression : ValidationRuleExpression
	{
		public OverridingAllowExpression(string queryFilter, ObjectSchema schema, List<Type> applicableObjects) : base(queryFilter, schema, applicableObjects)
		{
		}
	}
}
