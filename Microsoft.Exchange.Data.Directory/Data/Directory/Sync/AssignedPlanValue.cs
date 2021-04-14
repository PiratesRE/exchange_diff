using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class AssignedPlanValue
	{
		public override string ToString()
		{
			return string.Format("subscribedPlanIdField={0} serviceInstanceField={1} assignedTimestampField={2} capabilityStatusField={3} servicePlanIdField={4} Capability={5}", new object[]
			{
				this.subscribedPlanIdField,
				this.serviceInstanceField,
				this.assignedTimestampField,
				this.capabilityStatusField,
				this.servicePlanIdField,
				this.Capability.OuterXml
			});
		}

		[XmlElement(Order = 0)]
		public XmlElement InitialState
		{
			get
			{
				return this.initialStateField;
			}
			set
			{
				this.initialStateField = value;
			}
		}

		[XmlElement(Order = 1)]
		public XmlElement Capability
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

		private XmlElement initialStateField;

		private XmlElement capabilityField;

		private string subscribedPlanIdField;

		private string serviceInstanceField;

		private AssignedCapabilityStatus capabilityStatusField;

		private DateTime assignedTimestampField;

		private string servicePlanIdField;
	}
}
