using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class MapValue
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

		[XmlArray(Order = 1)]
		[XmlArrayItem("Region", IsNullable = false)]
		public MapValueRegion[] Regions
		{
			get
			{
				return this.regionsField;
			}
			set
			{
				this.regionsField = value;
			}
		}

		private WeightedServiceInstancesWeightedServiceInstance[] weightedServiceInstancesField;

		private MapValueRegion[] regionsField;
	}
}
