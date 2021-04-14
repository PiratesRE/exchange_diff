using System;
using System.Linq.Expressions;

namespace Microsoft.Exchange.Management.SystemManager
{
	internal class DynamicOrdering
	{
		public Expression Selector;

		public bool Ascending;
	}
}
