using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[Serializable]
	public class Subscription : DirectoryObject
	{
		internal override void ForEachProperty(IPropertyProcessor processor)
		{
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
		public DirectoryPropertyStringSingleLength1To1024 CommerceSubscriptionContext
		{
			get
			{
				return this.commerceSubscriptionContextField;
			}
			set
			{
				this.commerceSubscriptionContextField = value;
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
		public DirectoryPropertyDateTimeSingle NextLifecycleDate
		{
			get
			{
				return this.nextLifecycleDateField;
			}
			set
			{
				this.nextLifecycleDateField = value;
			}
		}

		[XmlElement(Order = 4)]
		public DirectoryPropertyGuidSingle OcpSubscriptionId
		{
			get
			{
				return this.ocpSubscriptionIdField;
			}
			set
			{
				this.ocpSubscriptionIdField = value;
			}
		}

		[XmlElement(Order = 5)]
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

		[XmlElement(Order = 6)]
		public DirectoryPropertyGuidSingle SkuId
		{
			get
			{
				return this.skuIdField;
			}
			set
			{
				this.skuIdField = value;
			}
		}

		[XmlElement(Order = 7)]
		public DirectoryPropertyDateTimeSingle StartDate
		{
			get
			{
				return this.startDateField;
			}
			set
			{
				this.startDateField = value;
			}
		}

		[XmlElement(Order = 8)]
		public DirectoryPropertyGuidSingle SubscriptionChangeNotificationId
		{
			get
			{
				return this.subscriptionChangeNotificationIdField;
			}
			set
			{
				this.subscriptionChangeNotificationIdField = value;
			}
		}

		[XmlElement(Order = 9)]
		public DirectoryPropertyInt32SingleMin0 SubscriptionStatus
		{
			get
			{
				return this.subscriptionStatusField;
			}
			set
			{
				this.subscriptionStatusField = value;
			}
		}

		[XmlElement(Order = 10)]
		public DirectoryPropertyBooleanSingle TrialSubscription
		{
			get
			{
				return this.trialSubscriptionField;
			}
			set
			{
				this.trialSubscriptionField = value;
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

		private DirectoryPropertyStringSingleLength1To1024 commerceSubscriptionContextField;

		private DirectoryPropertyInt32SingleMin0 maximumOverageUnitsField;

		private DirectoryPropertyDateTimeSingle nextLifecycleDateField;

		private DirectoryPropertyGuidSingle ocpSubscriptionIdField;

		private DirectoryPropertyInt32SingleMin0 prepaidUnitsField;

		private DirectoryPropertyGuidSingle skuIdField;

		private DirectoryPropertyDateTimeSingle startDateField;

		private DirectoryPropertyGuidSingle subscriptionChangeNotificationIdField;

		private DirectoryPropertyInt32SingleMin0 subscriptionStatusField;

		private DirectoryPropertyBooleanSingle trialSubscriptionField;

		private XmlAttribute[] anyAttrField;
	}
}
