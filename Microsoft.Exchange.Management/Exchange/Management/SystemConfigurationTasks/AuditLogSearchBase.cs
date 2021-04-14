using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public class AuditLogSearchBase : ConfigurableObject
	{
		public AuditLogSearchBase() : base(new SimplePropertyBag(AuditLogSearchBaseSchema.Identity, AuditLogSearchBaseSchema.ObjectState, AuditLogSearchBaseSchema.ExchangeVersion))
		{
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<AuditLogSearchBaseSchema>();
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal OrganizationId OrganizationId { get; set; }

		public string Name
		{
			get
			{
				return (string)this[AuditLogSearchBaseSchema.Name];
			}
			set
			{
				this[AuditLogSearchBaseSchema.Name] = value;
			}
		}

		public DateTime? StartDateUtc
		{
			get
			{
				return (DateTime?)this[AuditLogSearchBaseSchema.StartDateUtc];
			}
			set
			{
				this[AuditLogSearchBaseSchema.StartDateUtc] = value;
			}
		}

		public DateTime? EndDateUtc
		{
			get
			{
				return (DateTime?)this[AuditLogSearchBaseSchema.EndDateUtc];
			}
			set
			{
				this[AuditLogSearchBaseSchema.EndDateUtc] = value;
			}
		}

		public MultiValuedProperty<SmtpAddress> StatusMailRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[AuditLogSearchBaseSchema.StatusMailRecipients];
			}
			set
			{
				this[AuditLogSearchBaseSchema.StatusMailRecipients] = value;
			}
		}

		internal ADObjectId CreatedByEx
		{
			get
			{
				return (ADObjectId)this[AuditLogSearchBaseSchema.CreatedByEx];
			}
			set
			{
				this[AuditLogSearchBaseSchema.CreatedByEx] = value;
			}
		}

		public string CreatedBy
		{
			get
			{
				return (string)this[AuditLogSearchBaseSchema.CreatedBy];
			}
			set
			{
				this[AuditLogSearchBaseSchema.CreatedBy] = value;
			}
		}

		public bool? ExternalAccess
		{
			get
			{
				return (bool?)this[AuditLogSearchBaseSchema.ExternalAccess];
			}
			set
			{
				this[AuditLogSearchBaseSchema.ExternalAccess] = value;
			}
		}

		internal DateTime CreationTime { get; private set; }

		internal VersionedId MessageId { get; private set; }

		public int QueryComplexity { get; set; }

		internal void SetId(AuditLogSearchId id)
		{
			this[AuditLogSearchBaseSchema.Identity] = id;
		}

		internal virtual void Initialize(AuditLogSearchItemBase item)
		{
			this.SetId(new AuditLogSearchId(item.Identity));
			this.Name = item.Name;
			this.StartDateUtc = new DateTime?(item.StartDate.UniversalTime);
			this.EndDateUtc = new DateTime?(item.EndDate.UniversalTime);
			this.StatusMailRecipients = item.StatusMailRecipients;
			this.CreatedBy = item.CreatedBy;
			this.CreatedByEx = item.CreatedByEx;
			this.ExternalAccess = item.ExternalAccess;
			this.CreationTime = item.CreationTime.UniversalTime;
			this.MessageId = item.MessageId;
		}

		internal virtual void Initialize(AuditLogSearchBase item)
		{
			this.SetId(item.Identity);
			this.Name = item.Name;
			this.StartDateUtc = item.StartDateUtc;
			this.EndDateUtc = item.EndDateUtc;
			this.StatusMailRecipients = AuditLogSearchBase.ConvertToSmtpAddressMVP(item.StatusMailRecipients);
			this.CreatedBy = item.CreatedBy;
			this.CreatedByEx = item.CreatedByEx;
			this.ExternalAccess = (string.IsNullOrEmpty(item.ExternalAccess) ? null : new bool?(bool.Parse(item.ExternalAccess)));
			this.CreationTime = item.CreationTime;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("OrganizationId={0}", this.OrganizationId);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("StartDateUtc={0}", this.StartDateUtc);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("EndDateUtc={0}", this.EndDateUtc);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("ExternalAccess={0}", (this.ExternalAccess != null) ? this.ExternalAccess.Value.ToString() : "NULL");
			return stringBuilder.ToString();
		}

		protected static void AppendStringSearchTerm(StringBuilder stringBuilder, string name, IEnumerable<string> values)
		{
			stringBuilder.Append(name + "=");
			if (values != null)
			{
				foreach (string value in values)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(",");
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
		}

		protected static MultiValuedProperty<SmtpAddress> ConvertToSmtpAddressMVP(MultiValuedProperty<string> mvp)
		{
			MultiValuedProperty<SmtpAddress> multiValuedProperty = new MultiValuedProperty<SmtpAddress>();
			if (mvp != null)
			{
				foreach (string address in mvp)
				{
					multiValuedProperty.Add(SmtpAddress.Parse(address));
				}
			}
			return multiValuedProperty;
		}
	}
}
