using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class MapValueRegionCountry
	{
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
