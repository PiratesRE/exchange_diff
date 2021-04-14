using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class ToServiceObjectCommandSettingsBase : CommandSettings
	{
		public ToServiceObjectCommandSettingsBase()
		{
		}

		public ToServiceObjectCommandSettingsBase(PropertyPath propertyPath)
		{
			this.PropertyPath = propertyPath;
		}

		public ServiceObject ServiceObject { get; set; }

		public PropertyPath PropertyPath { get; set; }

		public IdAndSession IdAndSession { get; set; }
	}
}
