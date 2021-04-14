using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsResponse
{
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "HMSETTINGS:")]
	[Serializable]
	public class OptionsType
	{
		public byte ConfirmSent
		{
			get
			{
				return this.confirmSentField;
			}
			set
			{
				this.confirmSentField = value;
			}
		}

		public HeaderDisplayType HeaderDisplay
		{
			get
			{
				return this.headerDisplayField;
			}
			set
			{
				this.headerDisplayField = value;
			}
		}

		public IncludeOriginalInReplyType IncludeOriginalInReply
		{
			get
			{
				return this.includeOriginalInReplyField;
			}
			set
			{
				this.includeOriginalInReplyField = value;
			}
		}

		public JunkLevelType JunkLevel
		{
			get
			{
				return this.junkLevelField;
			}
			set
			{
				this.junkLevelField = value;
			}
		}

		public JunkMailDestinationType JunkMailDestination
		{
			get
			{
				return this.junkMailDestinationField;
			}
			set
			{
				this.junkMailDestinationField = value;
			}
		}

		public ReplyIndicatorType ReplyIndicator
		{
			get
			{
				return this.replyIndicatorField;
			}
			set
			{
				this.replyIndicatorField = value;
			}
		}

		public string ReplyToAddress
		{
			get
			{
				return this.replyToAddressField;
			}
			set
			{
				this.replyToAddressField = value;
			}
		}

		public byte SaveSentMail
		{
			get
			{
				return this.saveSentMailField;
			}
			set
			{
				this.saveSentMailField = value;
			}
		}

		public byte UseReplyToAddress
		{
			get
			{
				return this.useReplyToAddressField;
			}
			set
			{
				this.useReplyToAddressField = value;
			}
		}

		public OptionsTypeVacationResponse VacationResponse
		{
			get
			{
				return this.vacationResponseField;
			}
			set
			{
				this.vacationResponseField = value;
			}
		}

		public OptionsTypeMailForwarding MailForwarding
		{
			get
			{
				return this.mailForwardingField;
			}
			set
			{
				this.mailForwardingField = value;
			}
		}

		private byte confirmSentField;

		private HeaderDisplayType headerDisplayField;

		private IncludeOriginalInReplyType includeOriginalInReplyField;

		private JunkLevelType junkLevelField;

		private JunkMailDestinationType junkMailDestinationField;

		private ReplyIndicatorType replyIndicatorField;

		private string replyToAddressField;

		private byte saveSentMailField;

		private byte useReplyToAddressField;

		private OptionsTypeVacationResponse vacationResponseField;

		private OptionsTypeMailForwarding mailForwardingField;
	}
}
