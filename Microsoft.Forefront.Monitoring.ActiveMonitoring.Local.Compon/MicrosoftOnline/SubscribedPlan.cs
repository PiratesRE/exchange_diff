using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class SubscribedPlan : DirectoryObject
	{
		public DirectoryPropertyGuidSingle AccountId
		{
			get
			{
				return this.accountIdField;
			}
			set
			{
				this.accountIdField = value;
			}
		}

		public DirectoryPropertyStringLength1To256 AuthorizedServiceInstance
		{
			get
			{
				return this.authorizedServiceInstanceField;
			}
			set
			{
				this.authorizedServiceInstanceField = value;
			}
		}

		public DirectoryPropertyBooleanSingle BelongsToFirstLoginObjectSet
		{
			get
			{
				return this.belongsToFirstLoginObjectSetField;
			}
			set
			{
				this.belongsToFirstLoginObjectSetField = value;
			}
		}

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

		public DirectoryPropertyInt32SingleMin0 MaximumOverageUnits
		{
			get
			{
				return this.maximumOverageUnitsField;
			}
			set
			{
				this.maximumOverageUnitsField = value;
			}
		}

		public DirectoryPropertyXmlLicenseUnitsDetailSingle MaximumOverageUnitsDetail
		{
			get
			{
				return this.maximumOverageUnitsDetailField;
			}
			set
			{
				this.maximumOverageUnitsDetailField = value;
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

		public DirectoryPropertyInt32SingleMin0 PrepaidUnits
		{
			get
			{
				return this.prepaidUnitsField;
			}
			set
			{
				this.prepaidUnitsField = value;
			}
		}

		public DirectoryPropertyXmlLicenseUnitsDetailSingle PrepaidUnitsDetail
		{
			get
			{
				return this.prepaidUnitsDetailField;
			}
			set
			{
				this.prepaidUnitsDetailField = value;
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

		public DirectoryPropertyXmlLicenseUnitsDetailSingle TotalTrialUnitsDetail
		{
			get
			{
				return this.totalTrialUnitsDetailField;
			}
			set
			{
				this.totalTrialUnitsDetailField = value;
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

		private DirectoryPropertyGuidSingle accountIdField;

		private DirectoryPropertyStringLength1To256 authorizedServiceInstanceField;

		private DirectoryPropertyBooleanSingle belongsToFirstLoginObjectSetField;

		private DirectoryPropertyXmlAnySingle capabilityField;

		private DirectoryPropertyInt32SingleMin0 maximumOverageUnitsField;

		private DirectoryPropertyXmlLicenseUnitsDetailSingle maximumOverageUnitsDetailField;

		private DirectoryPropertyGuidSingle planIdField;

		private DirectoryPropertyInt32SingleMin0 prepaidUnitsField;

		private DirectoryPropertyXmlLicenseUnitsDetailSingle prepaidUnitsDetailField;

		private DirectoryPropertyStringSingleLength1To256 serviceTypeField;

		private DirectoryPropertyXmlLicenseUnitsDetailSingle totalTrialUnitsDetailField;

		private XmlAttribute[] anyAttrField;
	}
}
