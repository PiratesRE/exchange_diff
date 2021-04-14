using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class ServicePlan : DirectoryObject
	{
		public DirectoryPropertyXmlAnySingle Capability
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

		public DirectoryPropertyReferenceServicePlan DependsOnServicePlan
		{
			get
			{
				return this.dependsOnServicePlanField;
			}
			set
			{
				this.dependsOnServicePlanField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To64 LicenseType
		{
			get
			{
				return this.licenseTypeField;
			}
			set
			{
				this.licenseTypeField = value;
			}
		}

		public DirectoryPropertyGuidSingle PlanId
		{
			get
			{
				return this.planIdField;
			}
			set
			{
				this.planIdField = value;
			}
		}

		public DirectoryPropertyStringLength1To3 ProhibitedUsageLocations
		{
			get
			{
				return this.prohibitedUsageLocationsField;
			}
			set
			{
				this.prohibitedUsageLocationsField = value;
			}
		}

		public DirectoryPropertyStringLength1To3 RestrictedUsageLocations
		{
			get
			{
				return this.restrictedUsageLocationsField;
			}
			set
			{
				this.restrictedUsageLocationsField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 ServicePlanName
		{
			get
			{
				return this.servicePlanNameField;
			}
			set
			{
				this.servicePlanNameField = value;
			}
		}

		public DirectoryPropertyStringSingleLength1To256 ServiceType
		{
			get
			{
				return this.serviceTypeField;
			}
			set
			{
				this.serviceTypeField = value;
			}
		}

		public DirectoryPropertyReferenceServicePlan SubsetOf
		{
			get
			{
				return this.subsetOfField;
			}
			set
			{
				this.subsetOfField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 TargetClass
		{
			get
			{
				return this.targetClassField;
			}
			set
			{
				this.targetClassField = value;
			}
		}

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr
		{
			get
			{
				return this.anyAttrField;
			}
			set
			{
				this.anyAttrField = value;
			}
		}

		private DirectoryPropertyXmlAnySingle capabilityField;

		private DirectoryPropertyReferenceServicePlan dependsOnServicePlanField;

		private DirectoryPropertyStringSingleLength1To64 licenseTypeField;

		private DirectoryPropertyGuidSingle planIdField;

		private DirectoryPropertyStringLength1To3 prohibitedUsageLocationsField;

		private DirectoryPropertyStringLength1To3 restrictedUsageLocationsField;

		private DirectoryPropertyStringSingleLength1To256 servicePlanNameField;

		private DirectoryPropertyStringSingleLength1To256 serviceTypeField;

		private DirectoryPropertyReferenceServicePlan subsetOfField;

		private DirectoryPropertyInt32SingleMin0 targetClassField;

		private XmlAttribute[] anyAttrField;
	}
}
