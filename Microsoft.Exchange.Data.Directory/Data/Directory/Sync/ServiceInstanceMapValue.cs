using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class ServiceInstanceMapValue
	{
		[XmlElement(Order = 0)]
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
		[XmlArray(Order = 1)]
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

		[XmlArray(Order = 2)]
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
