using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class MapValueRegionCountry
	{
		[XmlArray(Order = 0)]
		[XmlArrayItem("WeightedServiceInstance", IsNullable = false)]
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

		[XmlArrayItem("State", IsNullable = false)]
		[XmlArray(Order = 1)]
		public MapValueRegionCountryState[] States
		{
			get
			{
				return this.statesField;
			}
			set
			{
				this.statesField = value;
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

		private MapValueRegionCountryState[] statesField;

		private string nameField;
	}
}
