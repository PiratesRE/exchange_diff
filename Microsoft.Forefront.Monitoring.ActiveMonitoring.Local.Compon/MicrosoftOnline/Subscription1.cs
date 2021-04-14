using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(TypeName = "Subscription", Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class Subscription1 : DirectoryObject
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

		private DirectoryPropertyBooleanSingle belongsToFirstLoginObjectSetField;

		private DirectoryPropertyInt32SingleMin0 maximumOverageUnitsField;

		private DirectoryPropertyDateTimeSingle nextLifecycleDateField;

		private DirectoryPropertyGuidSingle ocpSubscriptionIdField;

		private DirectoryPropertyInt32SingleMin0 prepaidUnitsField;

		private DirectoryPropertyGuidSingle skuIdField;

		private DirectoryPropertyDateTimeSingle startDateField;

		private DirectoryPropertyInt32SingleMin0 subscriptionStatusField;

		private DirectoryPropertyBooleanSingle trialSubscriptionField;

		private XmlAttribute[] anyAttrField;
	}
}
