using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[Serializable]
	public class ServiceOriginatedResourceValue
	{
		[XmlAttribute]
		public bool LicenseReconciliationNeeded
		{
			get
			{
				return this.licenseReconciliationNeededField;
			}
			set
			{
				this.licenseReconciliationNeededField = value;
			}
		}

		[XmlIgnore]
		public bool LicenseReconciliationNeededSpecified
		{
			get
			{
				return this.licenseReconciliationNeededFieldSpecified;
			}
			set
			{
				this.licenseReconciliationNeededFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string ServiceInstance
		{
			get
			{
				return this.serviceInstanceField;
			}
			set
			{
				this.serviceInstanceField = value;
			}
		}

		[XmlAttribute]
		public string ServicePlanId
		{
			get
			{
				return this.servicePlanIdField;
			}
			set
			{
				this.servicePlanIdField = value;
			}
		}

		[XmlAttribute]
		public string Capability
		{
			get
			{
				return this.capabilityField;
			}
			set
			{
				this.capabilityField = value;
			}
		}

		private bool licenseReconciliationNeededField;

		private bool licenseReconciliationNeededFieldSpecified;

		private string serviceInstanceField;

		private string servicePlanIdField;

		private string capabilityField;
	}
}
