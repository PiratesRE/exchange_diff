using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[Serializable]
	public class ServiceInstanceMapValue
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

		[XmlArrayItem("CompanyTagLevelMap", IsNullable = false)]
		public CompanyTagLevelMapsValueCompanyTagLevelMap[] CompanyTagLevelMaps
		{
			get
			{
				return this.companyTagLevelMapsField;
			}
			set
			{
				this.companyTagLevelMapsField = value;
			}
		}

		private MapValue mapField;

		private ServicePlanLevelMapsValueServicePlanLevelMap[] servicePlanLevelMapsField;

		private CompanyTagLevelMapsValueCompanyTagLevelMap[] companyTagLevelMapsField;
	}
}
