using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Migration
{
	internal delegate MigrationRowSelectorResult MigrationRowSelector(IDictionary<PropertyDefinition, object> rowData);
}
