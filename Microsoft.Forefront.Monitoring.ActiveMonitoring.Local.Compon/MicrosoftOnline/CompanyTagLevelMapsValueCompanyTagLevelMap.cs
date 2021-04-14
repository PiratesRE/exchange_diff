using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class CompanyTagLevelMapsValueCompanyTagLevelMap
	{
		public MapValue Map
		{
			get
			{
				return this.mapField;
			}
			set
			{
				this.mapField = value;
			}
		}

		[XmlArrayItem("ServicePlanLevelMap", IsNullable = false)]
		public ServicePlanLevelMapsValueServicePlanLevelMap[] ServicePlanLevelMaps
		{
			get
			{
				return this.servicePlanLevelMapsField;
			}
			set
			{
				this.servicePlanLevelMapsField = value;
			}
		}

		[XmlAttribute]
		public string CompanyTag
		{
			get
			{
				return this.companyTagField;
			}
			set
			{
				this.companyTagField = value;
			}
		}

		[XmlAttribute]
		public uint Priority
		{
			get
			{
				return this.priorityField;
			}
			set
			{
				this.priorityField = value;
			}
		}

		private MapValue mapField;

		private ServicePlanLevelMapsValueServicePlanLevelMap[] servicePlanLevelMapsField;

		private string companyTagField;

		private uint priorityField;
	}
}
