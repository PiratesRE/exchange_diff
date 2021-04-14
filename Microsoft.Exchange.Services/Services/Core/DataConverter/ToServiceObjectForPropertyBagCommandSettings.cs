using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ToServiceObjectForPropertyBagCommandSettings : ToServiceObjectCommandSettingsBase
	{
		public ToServiceObjectForPropertyBagCommandSettings()
		{
		}

		public ToServiceObjectForPropertyBagCommandSettings(PropertyPath propertyPath) : base(propertyPath)
		{
		}

		public IDictionary<PropertyDefinition, object> PropertyBag { get; set; }
	}
}
