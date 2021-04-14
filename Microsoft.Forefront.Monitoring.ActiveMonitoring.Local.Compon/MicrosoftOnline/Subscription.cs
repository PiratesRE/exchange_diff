using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://www.ccs.com/TestServices/")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class Subscription
	{
		public Guid ContextId
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

		public Guid AccountId
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

		public Guid SubscriptionId
		{
			get
			{
				return this.subscriptionIdField;
			}
			set
			{
				this.subscriptionIdField = value;
			}
		}

		public Guid SkuId
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

		public int PrepaidUnits
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

		public int AllowedOverageUnits
		{
			get
			{
				return this.allowedOverageUnitsField;
			}
			set
			{
				this.allowedOverageUnitsField = value;
			}
		}

		public SubscriptionState LifecycleState
		{
			get
			{
				return this.lifecycleStateField;
			}
			set
			{
				this.lifecycleStateField = value;
			}
		}

		public DateTime LifecycleNextStateChangeDate
		{
			get
			{
				return this.lifecycleNextStateChangeDateField;
			}
			set
			{
				this.lifecycleNextStateChangeDateField = value;
			}
		}

		public DateTime StartDate
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

		public string OfferType
		{
			get
			{
				return this.offerTypeField;
			}
			set
			{
				this.offerTypeField = value;
			}
		}

		public string PartNumber
		{
			get
			{
				return this.partNumberField;
			}
			set
			{
				this.partNumberField = value;
			}
		}

		private Guid contextIdField;

		private Guid accountIdField;

		private Guid subscriptionIdField;

		private Guid skuIdField;

		private int prepaidUnitsField;

		private int allowedOverageUnitsField;

		private SubscriptionState lifecycleStateField;

		private DateTime lifecycleNextStateChangeDateField;

		private DateTime startDateField;

		private string offerTypeField;

		private string partNumberField;
	}
}
