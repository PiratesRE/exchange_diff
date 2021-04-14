using System;
using System.Configuration;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public class AnalyzerConfigurationCollection : ConfigurationElementCollection<AnalyzerConfigurationElement>
	{
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.AddRemoveClearMap;
			}
		}

		protected override string ElementName
		{
			get
			{
				return "name";
			}
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			if (element != null)
			{
				return ((AnalyzerConfigurationElement)element).Name;
			}
			return null;
		}
	}
}
