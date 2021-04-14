using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class PhoneCallInformationType
	{
		public PhoneCallStateType PhoneCallState
		{
			get
			{
				return this.phoneCallStateField;
			}
			set
			{
				this.phoneCallStateField = value;
			}
		}

		public ConnectionFailureCauseType ConnectionFailureCause
		{
			get
			{
				return this.connectionFailureCauseField;
			}
			set
			{
				this.connectionFailureCauseField = value;
			}
		}

		public string SIPResponseText
		{
			get
			{
				return this.sIPResponseTextField;
			}
			set
			{
				this.sIPResponseTextField = value;
			}
		}

		public int SIPResponseCode
		{
			get
			{
				return this.sIPResponseCodeField;
			}
			set
			{
				this.sIPResponseCodeField = value;
			}
		}

		[XmlIgnore]
		public bool SIPResponseCodeSpecified
		{
			get
			{
				return this.sIPResponseCodeFieldSpecified;
			}
			set
			{
				this.sIPResponseCodeFieldSpecified = value;
			}
		}

		private PhoneCallStateType phoneCallStateField;

		private ConnectionFailureCauseType connectionFailureCauseField;

		private string sIPResponseTextField;

		private int sIPResponseCodeField;

		private bool sIPResponseCodeFieldSpecified;
	}
}
