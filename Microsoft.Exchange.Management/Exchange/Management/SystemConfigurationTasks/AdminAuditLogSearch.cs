using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public class AdminAuditLogSearch : AuditLogSearchBase
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<AdminAuditLogSearchSchema>();
			}
		}

		public MultiValuedProperty<string> Cmdlets
		{
			get
			{
				return (MultiValuedProperty<string>)this[AdminAuditLogSearchSchema.Cmdlets];
			}
			set
			{
				this[AdminAuditLogSearchSchema.Cmdlets] = value;
			}
		}

		public MultiValuedProperty<string> Parameters
		{
			get
			{
				return (MultiValuedProperty<string>)this[AdminAuditLogSearchSchema.Parameters];
			}
			set
			{
				this[AdminAuditLogSearchSchema.Parameters] = value;
			}
		}

		public MultiValuedProperty<string> ObjectIds
		{
			get
			{
				return (MultiValuedProperty<string>)this[AdminAuditLogSearchSchema.ObjectIds];
			}
			set
			{
				this[AdminAuditLogSearchSchema.ObjectIds] = value;
			}
		}

		public MultiValuedProperty<string> UserIds
		{
			get
			{
				return (MultiValuedProperty<string>)this[AdminAuditLogSearchSchema.UserIds];
			}
			set
			{
				this[AdminAuditLogSearchSchema.UserIds] = value;
			}
		}

		internal MultiValuedProperty<string> ResolvedUsers
		{
			get
			{
				return (MultiValuedProperty<string>)this[AdminAuditLogSearchSchema.ResolvedUsers];
			}
			set
			{
				this[AdminAuditLogSearchSchema.ResolvedUsers] = value;
			}
		}

		internal bool RedactDatacenterAdmins
		{
			get
			{
				return (bool)this[AdminAuditLogSearchSchema.RedactDatacenterAdmins];
			}
			set
			{
				this[AdminAuditLogSearchSchema.RedactDatacenterAdmins] = value;
			}
		}

		internal MultiValuedProperty<SecurityPrincipalIdParameter> UserIdsUserInput { get; set; }

		internal bool? Succeeded { get; set; }

		internal int StartIndex { get; set; }

		internal int ResultSize { get; set; }

		internal override void Initialize(AuditLogSearchItemBase item)
		{
			AdminAuditLogSearchItem adminAuditLogSearchItem = (AdminAuditLogSearchItem)item;
			base.Initialize(item);
			this.Cmdlets = adminAuditLogSearchItem.Cmdlets;
			this.Parameters = adminAuditLogSearchItem.Parameters;
			this.ObjectIds = adminAuditLogSearchItem.ObjectIds;
			this.UserIds = adminAuditLogSearchItem.RawUserIds;
			this.ResolvedUsers = adminAuditLogSearchItem.ResolvedUsers;
			this.RedactDatacenterAdmins = adminAuditLogSearchItem.RedactDatacenterAdmins;
			this.Succeeded = null;
			this.StartIndex = 0;
			this.ResultSize = 50000;
		}

		internal override void Initialize(AuditLogSearchBase item)
		{
			AdminAuditLogSearch adminAuditLogSearch = (AdminAuditLogSearch)item;
			base.Initialize(item);
			this.Cmdlets = adminAuditLogSearch.Cmdlets;
			this.Parameters = adminAuditLogSearch.Parameters;
			this.ObjectIds = adminAuditLogSearch.ObjectIds;
			this.UserIds = adminAuditLogSearch.UserIds;
			this.ResolvedUsers = adminAuditLogSearch.ResolvedUsers;
			this.RedactDatacenterAdmins = adminAuditLogSearch.RedactDatacenterAdmins;
			this.Succeeded = null;
			this.StartIndex = 0;
			this.ResultSize = 50000;
		}

		internal void Validate(Task.TaskErrorLoggingDelegate writeError)
		{
			if ((this.Cmdlets == null || this.Cmdlets.Count == 0) && this.Parameters != null && this.Parameters.Count != 0)
			{
				writeError(new ArgumentException(Strings.AdminAuditLogSearchMissingCmdletsWhileParameterProvided), ErrorCategory.InvalidArgument, null);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.ToString());
			stringBuilder.AppendLine();
			AuditLogSearchBase.AppendStringSearchTerm(stringBuilder, "Cmdlets", this.Cmdlets);
			stringBuilder.AppendLine();
			AuditLogSearchBase.AppendStringSearchTerm(stringBuilder, "Parameters", this.Parameters);
			stringBuilder.AppendLine();
			AuditLogSearchBase.AppendStringSearchTerm(stringBuilder, "ObjectIds", this.ObjectIds);
			stringBuilder.AppendLine();
			AuditLogSearchBase.AppendStringSearchTerm(stringBuilder, "UserIds", this.UserIds);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("Succeeded={0}", this.Succeeded);
			return stringBuilder.ToString();
		}

		internal const int MaxSearchResultSize = 250000;

		internal const int MaxLogsForEmailAttachment = 50000;
	}
}
