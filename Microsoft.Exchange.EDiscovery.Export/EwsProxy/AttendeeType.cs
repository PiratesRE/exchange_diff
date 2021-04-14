using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class AttendeeType
	{
		public EmailAddressType Mailbox
		{
			get
			{
				return this.mailboxField;
			}
			set
			{
				this.mailboxField = value;
			}
		}

		public ResponseTypeType ResponseType
		{
			get
			{
				return this.responseTypeField;
			}
			set
			{
				this.responseTypeField = value;
			}
		}

		[XmlIgnore]
		public bool ResponseTypeSpecified
		{
			get
			{
				return this.responseTypeFieldSpecified;
			}
			set
			{
				this.responseTypeFieldSpecified = value;
			}
		}

		public DateTime LastResponseTime
		{
			get
			{
				return this.lastResponseTimeField;
			}
			set
			{
				this.lastResponseTimeField = value;
			}
		}

		[XmlIgnore]
		public bool LastResponseTimeSpecified
		{
			get
			{
				return this.lastResponseTimeFieldSpecified;
			}
			set
			{
				this.lastResponseTimeFieldSpecified = value;
			}
		}

		public DateTime ProposedStart
		{
			get
			{
				return this.proposedStartField;
			}
			set
			{
				this.proposedStartField = value;
			}
		}

		[XmlIgnore]
		public bool ProposedStartSpecified
		{
			get
			{
				return this.proposedStartFieldSpecified;
			}
			set
			{
				this.proposedStartFieldSpecified = value;
			}
		}

		public DateTime ProposedEnd
		{
			get
			{
				return this.proposedEndField;
			}
			set
			{
				this.proposedEndField = value;
			}
		}

		[XmlIgnore]
		public bool ProposedEndSpecified
		{
			get
			{
				return this.proposedEndFieldSpecified;
			}
			set
			{
				this.proposedEndFieldSpecified = value;
			}
		}

		private EmailAddressType mailboxField;

		private ResponseTypeType responseTypeField;

		private bool responseTypeFieldSpecified;

		private DateTime lastResponseTimeField;

		private bool lastResponseTimeFieldSpecified;

		private DateTime proposedStartField;

		private bool proposedStartFieldSpecified;

		private DateTime proposedEndField;

		private bool proposedEndFieldSpecified;
	}
}
