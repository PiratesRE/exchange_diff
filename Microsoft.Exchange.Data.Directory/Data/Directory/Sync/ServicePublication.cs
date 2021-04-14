using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class ServicePublication
	{
		[XmlElement(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11", Order = 0)]
		public DirectoryPropertyStringSingleLength1To454 CloudSipProxyAddress
		{
			get
			{
				return this.cloudSipProxyAddressField;
			}
			set
			{
				this.cloudSipProxyAddressField = value;
			}
		}

		[XmlElement(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11", Order = 1)]
		public DirectoryPropertyXmlProvisionedPlan ProvisionedPlan
		{
			get
			{
				return this.provisionedPlanField;
			}
			set
			{
				this.provisionedPlanField = value;
			}
		}

		[XmlElement(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11", Order = 2)]
		public DirectoryPropertyXmlServiceInfo ServiceInfo
		{
			get
			{
				return this.serviceInfoField;
			}
			set
			{
				this.serviceInfoField = value;
			}
		}

		[XmlElement(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11", Order = 3)]
		public DirectoryPropertyXmlValidationError ValidationError
		{
			get
			{
				return this.validationErrorField;
			}
			set
			{
				this.validationErrorField = value;
			}
		}

		[XmlAttribute]
		public string ContextId
		{
			get
			{
				return this.contextIdField;
			}
			set
			{
				this.contextIdField = value;
			}
		}

		[XmlAttribute]
		public DirectoryObjectClassCapabilityTarget ObjectClass
		{
			get
			{
				return this.objectClassField;
			}
			set
			{
				this.objectClassField = value;
			}
		}

		[XmlAttribute]
		public string ObjectId
		{
			get
			{
				return this.objectIdField;
			}
			set
			{
				this.objectIdField = value;
			}
		}

		[XmlIgnore]
		public Guid BatchId { get; set; }

		[XmlIgnore]
		public SyncObjectId SyncObjectId { get; set; }

		private DirectoryPropertyStringSingleLength1To454 cloudSipProxyAddressField;

		private DirectoryPropertyXmlProvisionedPlan provisionedPlanField;

		private DirectoryPropertyXmlServiceInfo serviceInfoField;

		private DirectoryPropertyXmlValidationError validationErrorField;

		private string contextIdField;

		private DirectoryObjectClassCapabilityTarget objectClassField;

		private string objectIdField;
	}
}
