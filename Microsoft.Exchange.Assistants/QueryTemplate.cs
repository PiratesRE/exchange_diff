using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Assistants
{
	internal class QueryTemplate
	{
		public SimpleProviderPropertyDefinition[] Parameters { get; private set; }

		public Func<object[], QueryFilter, List<QueryableObject>> Executor { get; private set; }

		public QueryTemplate(Func<object[], QueryFilter, List<QueryableObject>> executor, params SimpleProviderPropertyDefinition[] parameters)
		{
			this.Executor = executor;
			this.Parameters = parameters;
		}
	}
}
