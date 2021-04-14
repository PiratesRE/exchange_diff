using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class CompanyTagLevelMapsValueCompanyTagLevelMap
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

		[XmlArray(Order = 1)]
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
