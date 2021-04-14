using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NonOwnerAccessFilter : AdminAuditLogSearchFilter
	{
		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Search-MailboxAuditLog";
			}
		}

		[DataMember]
		public override string ObjectIds
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

		public new const string RbacParameters = "?ResultSize&StartDate&EndDate&Mailboxes&LogonTypes&ExternalAccess";
	}
}
