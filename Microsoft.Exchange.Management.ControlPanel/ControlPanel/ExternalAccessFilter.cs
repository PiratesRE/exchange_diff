using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ExternalAccessFilter : AdminAuditLogSearchFilter
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
				return "Search-AdminAuditLog";
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

		public new const string RbacParameters = "?ResultSize&StartDate&EndDate&ExternalAccess";
	}
}
