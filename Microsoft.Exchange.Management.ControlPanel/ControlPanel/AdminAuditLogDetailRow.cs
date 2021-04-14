using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AdminAuditLogDetailRow : BaseRow
	{
		public AdminAuditLogDetailRow(AdminAuditLogEvent logEvent) : base(logEvent)
		{
			this.AdminAuditLogEvent = logEvent;
			this.selectedObjectName = logEvent.ObjectModified;
			this.searchObjectName = logEvent.SearchObject;
		}

		internal AdminAuditLogDetailRow(Identity id, AdminAuditLogEvent searchResult) : base(id, searchResult)
		{
			this.AdminAuditLogEvent = searchResult;
			this.selectedObjectName = searchResult.ObjectModified;
			this.searchObjectName = searchResult.SearchObject;
		}

		internal AdminAuditLogDetailRow(Identity id, string objectName, AdminAuditLogEvent searchResult) : base(id, searchResult)
		{
			this.AdminAuditLogEvent = searchResult;
			this.selectedObjectName = objectName;
			this.searchObjectName = searchResult.SearchObject;
		}

		public AdminAuditLogEvent AdminAuditLogEvent { get; private set; }

		public string UserFriendlyObjectSelected
		{
			get
			{
				return AuditHelper.MakeUserFriendly(this.selectedObjectName);
			}
		}

		public string UserFriendlyCaller
		{
			get
			{
				return AuditHelper.MakeUserFriendly(this.AdminAuditLogEvent.Caller);
			}
		}

		public string SearchObjectName
		{
			get
			{
				return this.searchObjectName;
			}
		}

		private readonly string searchObjectName;

		private string selectedObjectName;
	}
}
