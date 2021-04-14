using System;

namespace Microsoft.Exchange.Data
{
	internal sealed class AdminAuditLogEventFacade : ConfigurableObject
	{
		public AdminAuditLogEventFacade() : base(new SimpleProviderPropertyBag())
		{
		}

		public AdminAuditLogEventFacade(ConfigObjectId identity) : base(new SimpleProviderPropertyBag())
		{
			this.propertyBag[SimpleProviderObjectSchema.Identity] = identity;
		}

		public string ObjectModified
		{
			get
			{
				return this.propertyBag[AdminAuditLogFacadeSchema.ObjectModified] as string;
			}
			set
			{
				this.propertyBag[AdminAuditLogFacadeSchema.ObjectModified] = value;
			}
		}

		internal string ModifiedObjectResolvedName
		{
			get
			{
				return this.propertyBag[AdminAuditLogFacadeSchema.ModifiedObjectResolvedName] as string;
			}
			set
			{
				this.propertyBag[AdminAuditLogFacadeSchema.ModifiedObjectResolvedName] = value;
			}
		}

		public string CmdletName
		{
			get
			{
				return this.propertyBag[AdminAuditLogFacadeSchema.CmdletName] as string;
			}
			set
			{
				this.propertyBag[AdminAuditLogFacadeSchema.CmdletName] = value;
			}
		}

		public MultiValuedProperty<AdminAuditLogCmdletParameter> CmdletParameters
		{
			get
			{
				return this.propertyBag[AdminAuditLogFacadeSchema.CmdletParameters] as MultiValuedProperty<AdminAuditLogCmdletParameter>;
			}
			set
			{
				this.propertyBag[AdminAuditLogFacadeSchema.CmdletParameters] = value;
			}
		}

		public MultiValuedProperty<AdminAuditLogModifiedProperty> ModifiedProperties
		{
			get
			{
				return this.propertyBag[AdminAuditLogFacadeSchema.ModifiedProperties] as MultiValuedProperty<AdminAuditLogModifiedProperty>;
			}
			set
			{
				this.propertyBag[AdminAuditLogFacadeSchema.ModifiedProperties] = value;
			}
		}

		public string Caller
		{
			get
			{
				return this.propertyBag[AdminAuditLogFacadeSchema.Caller] as string;
			}
			set
			{
				this.propertyBag[AdminAuditLogFacadeSchema.Caller] = value;
			}
		}

		public bool? Succeeded
		{
			get
			{
				return this.propertyBag[AdminAuditLogFacadeSchema.Succeeded] as bool?;
			}
			set
			{
				this.propertyBag[AdminAuditLogFacadeSchema.Succeeded] = value;
			}
		}

		public string Error
		{
			get
			{
				return this.propertyBag[AdminAuditLogFacadeSchema.Error] as string;
			}
			set
			{
				this.propertyBag[AdminAuditLogFacadeSchema.Error] = value;
			}
		}

		public DateTime? RunDate
		{
			get
			{
				return this.propertyBag[AdminAuditLogFacadeSchema.RunDate] as DateTime?;
			}
			set
			{
				this.propertyBag[AdminAuditLogFacadeSchema.RunDate] = value;
			}
		}

		public string OriginatingServer
		{
			get
			{
				return this.propertyBag[AdminAuditLogFacadeSchema.OriginatingServer] as string;
			}
			set
			{
				this.propertyBag[AdminAuditLogFacadeSchema.OriginatingServer] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return AdminAuditLogEventFacade.schema;
			}
		}

		private static readonly ObjectSchema schema = ObjectSchema.GetInstance<AdminAuditLogFacadeSchema>();
	}
}
