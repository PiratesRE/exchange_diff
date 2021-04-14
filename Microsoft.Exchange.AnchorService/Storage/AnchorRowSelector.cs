using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.AnchorService.Storage
{
	internal delegate AnchorRowSelectorResult AnchorRowSelector(IDictionary<PropertyDefinition, object> rowData);
}
