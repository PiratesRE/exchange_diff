using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ClientIntentType
	{
		public ItemIdType ItemId
		{
			get
			{
				return this.itemIdField;
			}
			set
			{
				this.itemIdField = value;
			}
		}

		public int Intent
		{
			get
			{
				return this.intentField;
			}
			set
			{
				this.intentField = value;
			}
		}

		public int ItemVersion
		{
			get
			{
				return this.itemVersionField;
			}
			set
			{
				this.itemVersionField = value;
			}
		}

		public bool WouldRepair
		{
			get
			{
				return this.wouldRepairField;
			}
			set
			{
				this.wouldRepairField = value;
			}
		}

		public ClientIntentMeetingInquiryActionType PredictedAction
		{
			get
			{
				return this.predictedActionField;
			}
			set
			{
				this.predictedActionField = value;
			}
		}

		[XmlIgnore]
		public bool PredictedActionSpecified
		{
			get
			{
				return this.predictedActionFieldSpecified;
			}
			set
			{
				this.predictedActionFieldSpecified = value;
			}
		}

		private ItemIdType itemIdField;

		private int intentField;

		private int itemVersionField;

		private bool wouldRepairField;

		private ClientIntentMeetingInquiryActionType predictedActionField;

		private bool predictedActionFieldSpecified;
	}
}
