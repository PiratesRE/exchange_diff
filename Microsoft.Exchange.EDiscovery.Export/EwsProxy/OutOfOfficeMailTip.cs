using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class OutOfOfficeMailTip
	{
		public ReplyBody ReplyBody
		{
			get
			{
				return this.replyBodyField;
			}
			set
			{
				this.replyBodyField = value;
			}
		}

		public Duration Duration
		{
			get
			{
				return this.durationField;
			}
			set
			{
				this.durationField = value;
			}
		}

		private ReplyBody replyBodyField;

		private Duration durationField;
	}
}
