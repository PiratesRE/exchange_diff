using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class MailboxData
	{
		public EmailAddress Email
		{
			get
			{
				return this.emailField;
			}
			set
			{
				this.emailField = value;
			}
		}

		public MeetingAttendeeType AttendeeType
		{
			get
			{
				return this.attendeeTypeField;
			}
			set
			{
				this.attendeeTypeField = value;
			}
		}

		public bool ExcludeConflicts
		{
			get
			{
				return this.excludeConflictsField;
			}
			set
			{
				this.excludeConflictsField = value;
			}
		}

		[XmlIgnore]
		public bool ExcludeConflictsSpecified
		{
			get
			{
				return this.excludeConflictsFieldSpecified;
			}
			set
			{
				this.excludeConflictsFieldSpecified = value;
			}
		}

		private EmailAddress emailField;

		private MeetingAttendeeType attendeeTypeField;

		private bool excludeConflictsField;

		private bool excludeConflictsFieldSpecified;
	}
}
