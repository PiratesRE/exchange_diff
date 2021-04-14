using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SystemManager
{
	public sealed class ColumnExpression
	{
		public string ResultColumn { get; set; }

		public List<string> DependentColumns
		{
			get
			{
				return this.dependentColumns;
			}
		}

		public string Expression { get; set; }

		internal CachedDelegate CachedDelegate { get; set; }

		private List<string> dependentColumns = new List<string>();
	}
}
