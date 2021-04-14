using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class MapValueRegion
	{
		[XmlArrayItem("WeightedServiceInstance", IsNullable = false)]
		[XmlArray(Order = 0)]
		public WeightedServiceInstancesWeightedServiceInstance[] WeightedServiceInstances
		{
			get
			{
				return this.weightedServiceInstancesField;
			}
			set
			{
				this.weightedServiceInstancesField = value;
			}
		}

		[XmlArray(Order = 1)]
		[XmlArrayItem("Country", IsNullable = false)]
		public MapValueRegionCountry[] Countries
		{
			get
			{
				return this.countriesField;
			}
			set
			{
				this.countriesField = value;
			}
		}

		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		private WeightedServiceInstancesWeightedServiceInstance[] weightedServiceInstancesField;

		private MapValueRegionCountry[] countriesField;

		private string nameField;
	}
}
