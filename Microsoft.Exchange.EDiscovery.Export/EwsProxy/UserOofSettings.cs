using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class UserOofSettings
	{
		public OofState OofState
		{
			get
			{
				return this.oofStateField;
			}
			set
			{
				this.oofStateField = value;
			}
		}

		public ExternalAudience ExternalAudience
		{
			get
			{
				return this.externalAudienceField;
			}
			set
			{
				this.externalAudienceField = value;
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

		public ReplyBody InternalReply
		{
			get
			{
				return this.internalReplyField;
			}
			set
			{
				this.internalReplyField = value;
			}
		}

		public ReplyBody ExternalReply
		{
			get
			{
				return this.externalReplyField;
			}
			set
			{
				this.externalReplyField = value;
			}
		}

		private OofState oofStateField;

		private ExternalAudience externalAudienceField;

		private Duration durationField;

		private ReplyBody internalReplyField;

		private ReplyBody externalReplyField;
	}
}
