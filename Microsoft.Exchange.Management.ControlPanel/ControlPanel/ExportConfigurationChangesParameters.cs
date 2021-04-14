using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ExportConfigurationChangesParameters : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "New-AdminAuditLogSearch";
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

		public const string RbacParameters = "?StartDate&EndDate&StatusMailRecipients";
	}
}
