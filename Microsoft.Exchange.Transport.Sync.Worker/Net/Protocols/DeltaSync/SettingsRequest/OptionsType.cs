using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMTypes;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest
{
	[XmlType(TypeName = "OptionsType", Namespace = "HMSETTINGS:")]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Serializable]
	public class OptionsType
	{
		[XmlIgnore]
		public AlertStateType AlertState
		{
			get
			{
				return this.internalAlertState;
			}
			set
			{
				this.internalAlertState = value;
				this.internalAlertStateSpecified = true;
			}
		}

		[XmlIgnore]
		public BitType ConfirmSent
		{
			get
			{
				return this.internalConfirmSent;
			}
			set
			{
				this.internalConfirmSent = value;
				this.internalConfirmSentSpecified = true;
			}
		}

		[XmlIgnore]
		public HeaderDisplayType HeaderDisplay
		{
			get
			{
				return this.internalHeaderDisplay;
			}
			set
			{
				this.internalHeaderDisplay = value;
				this.internalHeaderDisplaySpecified = true;
			}
		}

		[XmlIgnore]
		public IncludeOriginalInReplyType IncludeOriginalInReply
		{
			get
			{
				return this.internalIncludeOriginalInReply;
			}
			set
			{
				this.internalIncludeOriginalInReply = value;
				this.internalIncludeOriginalInReplySpecified = true;
			}
		}

		[XmlIgnore]
		public JunkLevelType JunkLevel
		{
			get
			{
				return this.internalJunkLevel;
			}
			set
			{
				this.internalJunkLevel = value;
				this.internalJunkLevelSpecified = true;
			}
		}

		[XmlIgnore]
		public JunkMailDestinationType JunkMailDestination
		{
			get
			{
				return this.internalJunkMailDestination;
			}
			set
			{
				this.internalJunkMailDestination = value;
				this.internalJunkMailDestinationSpecified = true;
			}
		}

		[XmlIgnore]
		public ReplyIndicatorType ReplyIndicator
		{
			get
			{
				return this.internalReplyIndicator;
			}
			set
			{
				this.internalReplyIndicator = value;
				this.internalReplyIndicatorSpecified = true;
			}
		}

		[XmlIgnore]
		public string ReplyToAddress
		{
			get
			{
				return this.internalReplyToAddress;
			}
			set
			{
				this.internalReplyToAddress = value;
			}
		}

		[XmlIgnore]
		public BitType SaveSentMail
		{
			get
			{
				return this.internalSaveSentMail;
			}
			set
			{
				this.internalSaveSentMail = value;
				this.internalSaveSentMailSpecified = true;
			}
		}

		[XmlIgnore]
		public BitType UseReplyToAddress
		{
			get
			{
				return this.internalUseReplyToAddress;
			}
			set
			{
				this.internalUseReplyToAddress = value;
				this.internalUseReplyToAddressSpecified = true;
			}
		}

		[XmlIgnore]
		public VacationResponse VacationResponse
		{
			get
			{
				if (this.internalVacationResponse == null)
				{
					this.internalVacationResponse = new VacationResponse();
				}
				return this.internalVacationResponse;
			}
			set
			{
				this.internalVacationResponse = value;
			}
		}

		[XmlIgnore]
		public MailForwarding MailForwarding
		{
			get
			{
				if (this.internalMailForwarding == null)
				{
					this.internalMailForwarding = new MailForwarding();
				}
				return this.internalMailForwarding;
			}
			set
			{
				this.internalMailForwarding = value;
			}
		}

		[XmlElement(ElementName = "AlertState", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public AlertStateType internalAlertState;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalAlertStateSpecified;

		[XmlElement(ElementName = "ConfirmSent", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public BitType internalConfirmSent;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalConfirmSentSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "HeaderDisplay", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public HeaderDisplayType internalHeaderDisplay;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalHeaderDisplaySpecified;

		[XmlElement(ElementName = "IncludeOriginalInReply", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IncludeOriginalInReplyType internalIncludeOriginalInReply;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalIncludeOriginalInReplySpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "JunkLevel", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public JunkLevelType internalJunkLevel;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalJunkLevelSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "JunkMailDestination", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public JunkMailDestinationType internalJunkMailDestination;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalJunkMailDestinationSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "ReplyIndicator", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public ReplyIndicatorType internalReplyIndicator;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalReplyIndicatorSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "ReplyToAddress", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string", Namespace = "HMSETTINGS:")]
		public string internalReplyToAddress;

		[XmlElement(ElementName = "SaveSentMail", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public BitType internalSaveSentMail;

		[XmlIgnore]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool internalSaveSentMailSpecified;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(ElementName = "UseReplyToAddress", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public BitType internalUseReplyToAddress;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlIgnore]
		public bool internalUseReplyToAddressSpecified;

		[XmlElement(Type = typeof(VacationResponse), ElementName = "VacationResponse", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public VacationResponse internalVacationResponse;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[XmlElement(Type = typeof(MailForwarding), ElementName = "MailForwarding", IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "HMSETTINGS:")]
		public MailForwarding internalMailForwarding;
	}
}
