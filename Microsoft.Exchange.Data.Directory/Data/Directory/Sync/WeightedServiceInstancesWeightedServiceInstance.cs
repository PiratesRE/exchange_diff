using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class WeightedServiceInstancesWeightedServiceInstance
	{
		[XmlArrayItem("AdditionalServiceInstance", IsNullable = false)]
		[XmlArray(Order = 0)]
		public WeightedServiceInstancesWeightedServiceInstanceAdditionalServiceInstance[] AdditionalServiceInstances
		{
			get
			{
				return this.additionalServiceInstancesField;
			}
			set
			{
				this.additionalServiceInstancesField = value;
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

		[XmlAttribute]
		public int Weight
		{
			get
			{
				return this.weightField;
			}
			set
			{
				this.weightField = value;
			}
		}

		private WeightedServiceInstancesWeightedServiceInstanceAdditionalServiceInstance[] additionalServiceInstancesField;

		private string nameField;

		private int weightField;
	}
}
