using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ExportMailboxChangesParameters : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "New-MailboxAuditLogSearch";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}

		[DataMember]
		public string StartDate
		{
			get
			{
				return base["StartDate"].ToStringWithNull();
			}
			set
			{
				base["StartDate"] = AuditHelper.GetDateForAuditReportsFilter(value, false);
			}
		}

		[DataMember]
		public string EndDate
		{
			get
			{
				return base["EndDate"].ToStringWithNull();
			}
			set
			{
				base["EndDate"] = AuditHelper.GetDateForAuditReportsFilter(value, true);
			}
		}

		[DataMember]
		public string Mailboxes
		{
			get
			{
				return base["Mailboxes"].StringArrayJoin(",");
			}
			set
			{
				base["Mailboxes"] = value.ToArrayOfStrings();
			}
		}

		[DataMember]
		public string LogonTypes
		{
			get
			{
				if (base["LogonTypes"] == null)
				{
					return null;
				}
				return base["LogonTypes"].StringArrayJoin(",");
			}
			set
			{
				if (value == null)
				{
					base["LogonTypes"] = value;
					return;
				}
				base["LogonTypes"] = value.ToArrayOfStrings();
			}
		}

		[DataMember]
		public bool? ExternalAccess
		{
			get
			{
				return (bool?)base["ExternalAccess"];
			}
			set
			{
				base["ExternalAccess"] = value;
			}
		}

		[DataMember]
		public string StatusMailRecipients
		{
			get
			{
				return base["StatusMailRecipients"].StringArrayJoin(",");
			}
			set
			{
				base["StatusMailRecipients"] = value.ToArrayOfStrings();
			}
		}

		[DataMember]
		public bool ShowDetails
		{
			get
			{
				return base.ParameterIsSpecified("ShowDetails") && ((SwitchParameter)base["ShowDetails"]).ToBool();
			}
			set
			{
				base["ShowDetails"] = new SwitchParameter(value);
			}
		}

		public const string RbacParameters = "?StartDate&EndDate&Mailboxes&StatusMailRecipients&LogonTypes&ExternalAccess&ShowDetails";
	}
}
