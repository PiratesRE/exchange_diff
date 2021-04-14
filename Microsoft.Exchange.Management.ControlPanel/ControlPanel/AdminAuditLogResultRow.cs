using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AdminAuditLogResultRow : BaseRow
	{
		public AdminAuditLogResultRow(AdminAuditLogEvent searchResult)
		{
			this.AuditReportSearchBaseResult = searchResult;
		}

		internal AdminAuditLogResultRow(Identity id, AdminAuditLogEvent searchResult) : base(id, searchResult)
		{
			this.AuditReportSearchBaseResult = searchResult;
		}

		public AdminAuditLogEvent AuditReportSearchBaseResult { get; private set; }

		[DataMember]
		public string ObjectModified
		{
			get
			{
				return this.AuditReportSearchBaseResult.ObjectModified;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Cmdlet
		{
			get
			{
				return this.AuditReportSearchBaseResult.CmdletName;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SearchObject
		{
			get
			{
				return this.AuditReportSearchBaseResult.SearchObject;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string FriendlyObjectModified
		{
			get
			{
				if (this.internalFriendlyObjectModified == null)
				{
					return AuditHelper.MakeUserFriendly(this.AuditReportSearchBaseResult.ModifiedObjectResolvedName);
				}
				return this.internalFriendlyObjectModified;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("FriendlyObjectModified value");
				}
				this.internalFriendlyObjectModified = value;
			}
		}

		[DataMember]
		public string RunDate
		{
			get
			{
				if (this.internalRunDate == null)
				{
					return this.AuditReportSearchBaseResult.RunDate.Value.ToUniversalTime().UtcToUserDateTimeString();
				}
				return this.internalRunDate;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("RunDate value");
				}
				this.internalRunDate = value;
			}
		}

		private string internalFriendlyObjectModified;

		private string internalRunDate;
	}
}
