using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class Service : DirectoryObject
	{
		public DirectoryPropertyInt32SingleMin0 ContextPriorityQuotaLimit
		{
			get
			{
				return this.contextPriorityQuotaLimitField;
			}
			set
			{
				this.contextPriorityQuotaLimitField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 DeferredSyncCompanyQuotaLimit
		{
			get
			{
				return this.deferredSyncCompanyQuotaLimitField;
			}
			set
			{
				this.deferredSyncCompanyQuotaLimitField = value;
			}
		}

		public DirectoryPropertyGuidSingle PartnerSkuId
		{
			get
			{
				return this.partnerSkuIdField;
			}
			set
			{
				this.partnerSkuIdField = value;
			}
		}

		public DirectoryPropertyStringLength1To64 AuthorizedRestrictedSyncAttributeSet
		{
			get
			{
				return this.authorizedRestrictedSyncAttributeSetField;
			}
			set
			{
				this.authorizedRestrictedSyncAttributeSetField = value;
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

		public DirectoryPropertyXmlServiceInstanceMap ServiceInstanceMap
		{
			get
			{
				return this.serviceInstanceMapField;
			}
			set
			{
				this.serviceInstanceMapField = value;
			}
		}

		public DirectoryPropertyInt32SingleMin0 ServiceOptions
		{
			get
			{
				return this.serviceOptionsField;
			}
			set
			{
				this.serviceOptionsField = value;
			}
		}

		public DirectoryPropertyXmlAnySingle ServicePrincipalTemplate
		{
			get
			{
				return this.servicePrincipalTemplateField;
			}
			set
			{
				this.servicePrincipalTemplateField = value;
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

		private DirectoryPropertyInt32SingleMin0 contextPriorityQuotaLimitField;

		private DirectoryPropertyInt32SingleMin0 deferredSyncCompanyQuotaLimitField;

		private DirectoryPropertyGuidSingle partnerSkuIdField;

		private DirectoryPropertyStringLength1To64 authorizedRestrictedSyncAttributeSetField;

		private DirectoryPropertyStringSingleLength1To256 serviceTypeField;

		private DirectoryPropertyXmlServiceInstanceMap serviceInstanceMapField;

		private DirectoryPropertyInt32SingleMin0 serviceOptionsField;

		private DirectoryPropertyXmlAnySingle servicePrincipalTemplateField;

		private XmlAttribute[] anyAttrField;
	}
}
