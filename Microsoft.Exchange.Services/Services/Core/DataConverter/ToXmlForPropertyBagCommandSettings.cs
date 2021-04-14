using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ToXmlForPropertyBagCommandSettings : ToXmlCommandSettingsBase
	{
		public ToXmlForPropertyBagCommandSettings()
		{
		}

		public ToXmlForPropertyBagCommandSettings(PropertyPath propertyPath) : base(propertyPath)
		{
		}

		public IDictionary<PropertyDefinition, object> PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
			set
			{
				this.propertyBag = value;
			}
		}

		public IdAndSession IdAndSession
		{
			get
			{
				return this.idAndSession;
			}
			set
			{
				this.idAndSession = value;
			}
		}

		private IDictionary<PropertyDefinition, object> propertyBag;

		private IdAndSession idAndSession;
	}
}
