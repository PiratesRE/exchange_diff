using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class UnpublishedObject : ADObject
	{
		public override ObjectId Identity
		{
			get
			{
				return this.ObjectId;
			}
		}

		public ADObjectId ObjectId
		{
			get
			{
				return this[UnpublishedObjectSchema.ObjectIdProp] as ADObjectId;
			}
			set
			{
				this[UnpublishedObjectSchema.ObjectIdProp] = value;
			}
		}

		public ADObjectId TenantId
		{
			get
			{
				return this[UnpublishedObjectSchema.TenantIdProp] as ADObjectId;
			}
			set
			{
				this[UnpublishedObjectSchema.TenantIdProp] = value;
			}
		}

		public DirectoryObjectClass ObjectType
		{
			get
			{
				return (DirectoryObjectClass)this[UnpublishedObjectSchema.ObjectTypeProp];
			}
			set
			{
				this[UnpublishedObjectSchema.ObjectTypeProp] = value;
			}
		}

		public string ServiceInstance
		{
			get
			{
				return this[UnpublishedObjectSchema.ServiceInstanceProp] as string;
			}
			set
			{
				this[UnpublishedObjectSchema.ServiceInstanceProp] = value;
			}
		}

		public DateTime CreatedDate
		{
			get
			{
				return (DateTime)this[UnpublishedObjectSchema.CreatedDateProp];
			}
			set
			{
				this[UnpublishedObjectSchema.CreatedDateProp] = value;
			}
		}

		public DateTime LastRetriedDate
		{
			get
			{
				return (DateTime)this[UnpublishedObjectSchema.LastRetriedDateProp];
			}
			set
			{
				this[UnpublishedObjectSchema.LastRetriedDateProp] = value;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return (string)this[UnpublishedObjectSchema.ErrorMessageProp];
			}
			set
			{
				this[UnpublishedObjectSchema.ErrorMessageProp] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return UnpublishedObject.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return UnpublishedObject.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly UnpublishedObjectSchema schema = ObjectSchema.GetInstance<UnpublishedObjectSchema>();

		private static string mostDerivedClass = "UnpublishedObject";
	}
}
