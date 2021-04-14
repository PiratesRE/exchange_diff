using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class UnsyncedObject : UnpublishedObject
	{
		public SyncType SyncType
		{
			get
			{
				return (SyncType)this[UnsyncedObjectSchema.SyncTypeProp];
			}
			set
			{
				this[UnsyncedObjectSchema.SyncTypeProp] = value;
			}
		}

		public Guid BatchId
		{
			get
			{
				return (Guid)this[UnsyncedObjectSchema.BatchIdProp];
			}
			set
			{
				this[UnsyncedObjectSchema.BatchIdProp] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return UnsyncedObject.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return UnsyncedObject.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly UnsyncedObjectSchema schema = ObjectSchema.GetInstance<UnsyncedObjectSchema>();

		private static string mostDerivedClass = "UnsyncedObject";
	}
}
