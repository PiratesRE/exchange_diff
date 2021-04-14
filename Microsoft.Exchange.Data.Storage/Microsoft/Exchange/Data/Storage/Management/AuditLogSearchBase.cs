using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class AuditLogSearchBase : EwsStoreObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return AuditLogSearchBase.schema;
			}
		}

		internal override SearchFilter ItemClassFilter
		{
			get
			{
				return null;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		protected new string AlternativeId
		{
			get
			{
				return base.AlternativeId;
			}
			set
			{
				base.AlternativeId = value;
			}
		}

		public new AuditLogSearchId Identity
		{
			get
			{
				return (AuditLogSearchId)this[AuditLogSearchBaseEwsSchema.Identity];
			}
			set
			{
				this[AuditLogSearchBaseEwsSchema.Identity] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[AuditLogSearchBaseEwsSchema.Name];
			}
			set
			{
				this[AuditLogSearchBaseEwsSchema.Name] = value;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return (DateTime)this[AuditLogSearchBaseEwsSchema.CreationTime];
			}
			set
			{
				this[AuditLogSearchBaseEwsSchema.CreationTime] = value;
			}
		}

		public DateTime? StartDateUtc
		{
			get
			{
				return (DateTime?)this[AuditLogSearchBaseEwsSchema.StartDateUtc];
			}
			set
			{
				this[AuditLogSearchBaseEwsSchema.StartDateUtc] = value;
			}
		}

		public DateTime? EndDateUtc
		{
			get
			{
				return (DateTime?)this[AuditLogSearchBaseEwsSchema.EndDateUtc];
			}
			set
			{
				this[AuditLogSearchBaseEwsSchema.EndDateUtc] = value;
			}
		}

		public MultiValuedProperty<string> StatusMailRecipients
		{
			get
			{
				return (MultiValuedProperty<string>)this[AuditLogSearchBaseEwsSchema.StatusMailRecipients];
			}
			set
			{
				this[AuditLogSearchBaseEwsSchema.StatusMailRecipients] = value;
			}
		}

		public string CreatedBy
		{
			get
			{
				return (string)this[AuditLogSearchBaseEwsSchema.CreatedBy];
			}
			set
			{
				this[AuditLogSearchBaseEwsSchema.CreatedBy] = value;
			}
		}

		public string ExternalAccess
		{
			get
			{
				return (string)this[AuditLogSearchBaseEwsSchema.ExternalAccess];
			}
			set
			{
				this[AuditLogSearchBaseEwsSchema.ExternalAccess] = value;
			}
		}

		public ADObjectId CreatedByEx
		{
			get
			{
				return (ADObjectId)this[AuditLogSearchBaseEwsSchema.CreatedByEx];
			}
			set
			{
				this[AuditLogSearchBaseEwsSchema.CreatedByEx] = value;
			}
		}

		public string Type
		{
			get
			{
				return (string)this[AuditLogSearchBaseEwsSchema.Type];
			}
			set
			{
				this[AuditLogSearchBaseEwsSchema.Type] = value;
			}
		}

		private static ObjectSchema schema = new AuditLogSearchBaseEwsSchema();
	}
}
