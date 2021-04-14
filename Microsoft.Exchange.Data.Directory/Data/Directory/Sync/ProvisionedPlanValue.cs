using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class ProvisionedPlanValue
	{
		[XmlElement(Order = 0)]
		public XmlElement ErrorDetail
		{
			get
			{
				return this.errorDetailField;
			}
			set
			{
				this.errorDetailField = value;
			}
		}

		[XmlAttribute]
		public string SubscribedPlanId
		{
			get
			{
				return this.subscribedPlanIdField;
			}
			set
			{
				this.subscribedPlanIdField = value;
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
		public AssignedCapabilityStatus CapabilityStatus
		{
			get
			{
				return this.capabilityStatusField;
			}
			set
			{
				this.capabilityStatusField = value;
			}
		}

		[XmlAttribute]
		public DateTime AssignedTimestamp
		{
			get
			{
				return this.assignedTimestampField;
			}
			set
			{
				this.assignedTimestampField = value;
			}
		}

		[XmlAttribute]
		public ProvisioningStatus ProvisioningStatus
		{
			get
			{
				return this.provisioningStatusField;
			}
			set
			{
				this.provisioningStatusField = value;
			}
		}

		[XmlAttribute]
		public DateTime ProvisionedTimestamp
		{
			get
			{
				return this.provisionedTimestampField;
			}
			set
			{
				this.provisionedTimestampField = value;
			}
		}

		private XmlElement errorDetailField;

		private string subscribedPlanIdField;

		private string serviceInstanceField;

		private AssignedCapabilityStatus capabilityStatusField;

		private DateTime assignedTimestampField;

		private ProvisioningStatus provisioningStatusField;

		private DateTime provisionedTimestampField;
	}
}
