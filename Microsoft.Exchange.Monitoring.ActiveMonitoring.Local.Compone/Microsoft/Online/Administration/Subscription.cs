using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "Subscription", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class Subscription : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public DateTime? DateCreated
		{
			get
			{
				return this.DateCreatedField;
			}
			set
			{
				this.DateCreatedField = value;
			}
		}

		[DataMember]
		public DateTime? NextLifecycleDate
		{
			get
			{
				return this.NextLifecycleDateField;
			}
			set
			{
				this.NextLifecycleDateField = value;
			}
		}

		[DataMember]
		public Guid? ObjectId
		{
			get
			{
				return this.ObjectIdField;
			}
			set
			{
				this.ObjectIdField = value;
			}
		}

		[DataMember]
		public Guid? OcpSubscriptionId
		{
			get
			{
				return this.OcpSubscriptionIdField;
			}
			set
			{
				this.OcpSubscriptionIdField = value;
			}
		}

		[DataMember]
		public ServiceStatus[] ServiceStatus
		{
			get
			{
				return this.ServiceStatusField;
			}
			set
			{
				this.ServiceStatusField = value;
			}
		}

		[DataMember]
		public Guid? SkuId
		{
			get
			{
				return this.SkuIdField;
			}
			set
			{
				this.SkuIdField = value;
			}
		}

		[DataMember]
		public string SkuPartNumber
		{
			get
			{
				return this.SkuPartNumberField;
			}
			set
			{
				this.SkuPartNumberField = value;
			}
		}

		[DataMember]
		public SubscriptionStatus Status
		{
			get
			{
				return this.StatusField;
			}
			set
			{
				this.StatusField = value;
			}
		}

		[DataMember]
		public int TotalLicenses
		{
			get
			{
				return this.TotalLicensesField;
			}
			set
			{
				this.TotalLicensesField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private DateTime? DateCreatedField;

		private DateTime? NextLifecycleDateField;

		private Guid? ObjectIdField;

		private Guid? OcpSubscriptionIdField;

		private ServiceStatus[] ServiceStatusField;

		private Guid? SkuIdField;

		private string SkuPartNumberField;

		private SubscriptionStatus StatusField;

		private int TotalLicensesField;
	}
}
