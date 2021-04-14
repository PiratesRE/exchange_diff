using System;
using System.Xml;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal abstract class ToXmlCommandSettingsBase : CommandSettings
	{
		public ToXmlCommandSettingsBase()
		{
		}

		public ToXmlCommandSettingsBase(PropertyPath propertyPath)
		{
			this.propertyPath = propertyPath;
		}

		public PropertyPath PropertyPath
		{
			get
			{
				return this.propertyPath;
			}
		}

		public XmlElement ServiceItem
		{
			get
			{
				return this.serviceItem;
			}
			set
			{
				this.serviceItem = value;
			}
		}

		private PropertyPath propertyPath;

		private XmlElement serviceItem;
	}
}
