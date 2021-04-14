using System;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class AdminAuditLogSearch : AuditLogSearchBase
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return AdminAuditLogSearch.schema;
			}
		}

		internal override SearchFilter ItemClassFilter
		{
			get
			{
				return new SearchFilter.ContainsSubstring(ItemSchema.ItemClass, "IPM.AuditLogSearch.Admin", 1, 0);
			}
		}

		public MultiValuedProperty<string> Cmdlets
		{
			get
			{
				return (MultiValuedProperty<string>)this[AdminAuditLogSearchEwsSchema.Cmdlets];
			}
			set
			{
				this[AdminAuditLogSearchEwsSchema.Cmdlets] = value;
			}
		}

		public MultiValuedProperty<string> Parameters
		{
			get
			{
				return (MultiValuedProperty<string>)this[AdminAuditLogSearchEwsSchema.Parameters];
			}
			set
			{
				this[AdminAuditLogSearchEwsSchema.Parameters] = value;
			}
		}

		public MultiValuedProperty<string> ObjectIds
		{
			get
			{
				return (MultiValuedProperty<string>)this[AdminAuditLogSearchEwsSchema.ObjectIds];
			}
			set
			{
				this[AdminAuditLogSearchEwsSchema.ObjectIds] = value;
			}
		}

		public MultiValuedProperty<string> UserIds
		{
			get
			{
				return (MultiValuedProperty<string>)this[AdminAuditLogSearchEwsSchema.UserIds];
			}
			set
			{
				this[AdminAuditLogSearchEwsSchema.UserIds] = value;
			}
		}

		internal MultiValuedProperty<string> ResolvedUsers
		{
			get
			{
				return (MultiValuedProperty<string>)this[AdminAuditLogSearchEwsSchema.ResolvedUsers];
			}
			set
			{
				this[AdminAuditLogSearchEwsSchema.ResolvedUsers] = value;
			}
		}

		internal bool RedactDatacenterAdmins
		{
			get
			{
				return (bool)this[AdminAuditLogSearchEwsSchema.RedactDatacenterAdmins];
			}
			set
			{
				this[AdminAuditLogSearchEwsSchema.RedactDatacenterAdmins] = value;
			}
		}

		internal override string ItemClass
		{
			get
			{
				return "IPM.AuditLogSearch.Admin";
			}
		}

		private const string ItemClassPrefix = "IPM.AuditLogSearch.Admin";

		private static ObjectSchema schema = new AdminAuditLogSearchEwsSchema();
	}
}
