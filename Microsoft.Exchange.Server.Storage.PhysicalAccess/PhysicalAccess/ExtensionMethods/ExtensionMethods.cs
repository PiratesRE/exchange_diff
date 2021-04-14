using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess.ExtensionMethods
{
	public static class ExtensionMethods
	{
		public static IEnumerable<SimpleQueryOperator> CreateOperators(this SimpleQueryOperator.SimpleQueryOperatorDefinition[] definitions, IConnectionProvider connectionProvider)
		{
			SimpleQueryOperator[] array = new SimpleQueryOperator[definitions.Length];
			for (int i = 0; i < definitions.Length; i++)
			{
				if (definitions[i] != null)
				{
					array[i] = definitions[i].CreateOperator(connectionProvider);
				}
			}
			return array;
		}
	}
}
