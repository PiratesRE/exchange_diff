using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
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
