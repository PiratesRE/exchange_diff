using System;
using System.Configuration;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ConfigurationCollection(typeof(DiagnosticsComponent), AddItemName = "DiagnosticsComponent", CollectionType = ConfigurationElementCollectionType.BasicMap)]
	public class DiagnosticsComponents : ConfigurationElementCollection
	{
		public DiagnosticsComponent this[int index]
		{
			get
			{
				return base.BaseGet(index) as DiagnosticsComponent;
			}
			set
			{
				if (base.BaseGet(index) != null)
				{
					base.BaseRemoveAt(index);
				}
				this.BaseAdd(index, value);
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new DiagnosticsComponent();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return (element as DiagnosticsComponent).Name;
		}
	}
}
