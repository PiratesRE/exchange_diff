using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class GetUMSubscriberCallAnsweringDataResponseMessageType : ResponseMessageType
	{
		public bool IsOOF
		{
			get
			{
				return this.isOOFField;
			}
			set
			{
				this.isOOFField = value;
			}
		}

		public UMMailboxTranscriptionEnabledSetting IsTranscriptionEnabledInMailboxConfig
		{
			get
			{
				return this.isTranscriptionEnabledInMailboxConfigField;
			}
			set
			{
				this.isTranscriptionEnabledInMailboxConfigField = value;
			}
		}

		public bool IsMailboxQuotaExceeded
		{
			get
			{
				return this.isMailboxQuotaExceededField;
			}
			set
			{
				this.isMailboxQuotaExceededField = value;
			}
		}

		[XmlElement(DataType = "base64Binary")]
		public byte[] Greeting
		{
			get
			{
				return this.greetingField;
			}
			set
			{
				this.greetingField = value;
			}
		}

		public string GreetingName
		{
			get
			{
				return this.greetingNameField;
			}
			set
			{
				this.greetingNameField = value;
			}
		}

		public bool TaskTimedOut
		{
			get
			{
				return this.taskTimedOutField;
			}
			set
			{
				this.taskTimedOutField = value;
			}
		}

		private bool isOOFField;

		private UMMailboxTranscriptionEnabledSetting isTranscriptionEnabledInMailboxConfigField;

		private bool isMailboxQuotaExceededField;

		private byte[] greetingField;

		private string greetingNameField;

		private bool taskTimedOutField;
	}
}
