using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.Properties.XSO
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IXSOPropertyManager
	{
		PropertyDefinition[] AllProperties { get; }

		void AddPropertyDefinition(PropertyDefinition propertyDefinition);
	}
}
