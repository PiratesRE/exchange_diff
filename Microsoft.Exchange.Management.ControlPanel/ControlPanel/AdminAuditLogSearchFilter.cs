using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AdminAuditLogSearchFilter : WebServiceParameters
	{
		public AdminAuditLogSearchFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

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
		public string StartDate
		{
			get
			{
				return (string)base["StartDate"];
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
				return (string)base["EndDate"];
			}
			set
			{
				base["EndDate"] = AuditHelper.GetDateForAuditReportsFilter(value, true);
			}
		}

		[DataMember]
		public virtual string ObjectIds
		{
			get
			{
				return base["ObjectIds"].StringArrayJoin(",");
			}
			set
			{
				base["ObjectIds"] = value.ToArrayOfStrings();
			}
		}

		public string Cmdlets
		{
			get
			{
				return base["Cmdlets"].StringArrayJoin(",");
			}
			set
			{
				base["Cmdlets"] = value.ToArrayOfStrings();
			}
		}

		public string Parameters
		{
			get
			{
				return base["Parameters"].StringArrayJoin(",");
			}
			set
			{
				base["Parameters"] = value.ToArrayOfStrings();
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			if (!(this is NonOwnerAccessFilter))
			{
				base["IsSuccess"] = true;
			}
		}

		public const string RbacParameters = "?StartDate&EndDate&ObjectIds&Cmdlets";
	}
}
