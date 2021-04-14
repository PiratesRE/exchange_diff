using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class SubscribedPlan : DirectoryObject
	{
		internal override void ForEachProperty(IPropertyProcessor processor)
		{
			processor.Process<DirectoryPropertyGuidSingle>(SyncSubscribedPlanSchema.AccountId, ref this.accountIdField);
			processor.Process<DirectoryPropertyXmlAnySingle>(SyncSubscribedPlanSchema.Capability, ref this.capabilityField);
			processor.Process<DirectoryPropertyStringSingleLength1To256>(SyncSubscribedPlanSchema.ServiceType, ref this.serviceTypeField);
			processor.Process<DirectoryPropertyXmlLicenseUnitsDetailSingle>(SyncSubscribedPlanSchema.MaximumOverageUnitsDetail, ref this.maximumOverageUnitsDetailField);
			processor.Process<DirectoryPropertyXmlLicenseUnitsDetailSingle>(SyncSubscribedPlanSchema.PrepaidUnitsDetail, ref this.prepaidUnitsDetailField);
			processor.Process<DirectoryPropertyXmlLicenseUnitsDetailSingle>(SyncSubscribedPlanSchema.TotalTrialUnitsDetail, ref this.totalTrialUnitsDetailField);
		}

		public override string ToString()
		{
			return string.Format("accountIdField={0} capabilityField={1} serviceTypeField={2} maximumOverageUnitsDetailField={3} prepaidUnitsDetailField={4} totalTrialUnitsDetailField={5}", new object[]
			{
				this.accountIdField,
				this.capabilityField,
				this.serviceTypeField,
				this.maximumOverageUnitsDetailField,
				this.prepaidUnitsDetailField,
				this.totalTrialUnitsDetailField
			});
		}

		[XmlElement(Order = 0)]
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

		[XmlElement(Order = 1)]
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

		[XmlElement(Order = 2)]
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

		[XmlElement(Order = 3)]
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

		[XmlElement(Order = 4)]
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

		[XmlElement(Order = 5)]
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

		[XmlElement(Order = 6)]
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

		[XmlElement(Order = 7)]
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

		private DirectoryPropertyXmlAnySingle capabilityField;

		private DirectoryPropertyInt32SingleMin0 maximumOverageUnitsField;

		private DirectoryPropertyXmlLicenseUnitsDetailSingle maximumOverageUnitsDetailField;

		private DirectoryPropertyInt32SingleMin0 prepaidUnitsField;

		private DirectoryPropertyXmlLicenseUnitsDetailSingle prepaidUnitsDetailField;

		private DirectoryPropertyStringSingleLength1To256 serviceTypeField;

		private DirectoryPropertyXmlLicenseUnitsDetailSingle totalTrialUnitsDetailField;

		private XmlAttribute[] anyAttrField;
	}
}
