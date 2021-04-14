using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[Serializable]
	public class WeightedServiceInstancesWeightedServiceInstance
	{
		[XmlArrayItem("AdditionalServiceInstance", IsNullable = false)]
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
