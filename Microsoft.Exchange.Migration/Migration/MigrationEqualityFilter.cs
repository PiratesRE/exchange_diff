using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationEqualityFilter
	{
		internal MigrationEqualityFilter(PropertyDefinition definition, object value)
		{
			this.Property = definition;
			this.Value = value;
			this.Filter = new ComparisonFilter(ComparisonOperator.Equal, definition, value);
		}

		internal MigrationEqualityFilter(PropertyDefinition definition, string value, bool ignoreCase)
		{
			this.Property = definition;
			this.Value = value;
			MatchFlags matchFlags = ignoreCase ? MatchFlags.IgnoreCase : MatchFlags.Default;
			this.Filter = new TextFilter(definition, value, MatchOptions.FullString, matchFlags);
		}

		internal MigrationEqualityFilter(PropertyDefinition definition, object value, ComparisonOperator op)
		{
			this.Property = definition;
			this.Value = value;
			this.Filter = new ComparisonFilter(op, definition, value);
		}

		internal readonly PropertyDefinition Property;

		internal readonly object Value;

		internal readonly QueryFilter Filter;
	}
}
