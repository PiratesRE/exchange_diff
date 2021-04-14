using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Kes
{
	internal class SyncWatermark : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.Id.ToString());
			}
		}

		public Guid Id
		{
			get
			{
				return (Guid)this[SyncWatermark.IdProperty];
			}
			set
			{
				this[SyncWatermark.IdProperty] = value;
			}
		}

		public string SyncContext
		{
			get
			{
				return (string)this[SyncWatermark.SyncContextProperty];
			}
			set
			{
				this[SyncWatermark.SyncContextProperty] = value;
			}
		}

		public string Data
		{
			get
			{
				return (string)this[SyncWatermark.DataProperty];
			}
			set
			{
				this[SyncWatermark.DataProperty] = value;
			}
		}

		public Guid? Owner
		{
			get
			{
				return (Guid?)this[SyncWatermark.OwnerProperty];
			}
			set
			{
				this[SyncWatermark.OwnerProperty] = value;
			}
		}

		public DateTime? ChangedDateTime
		{
			get
			{
				object obj;
				if (base.TryGetValue(SyncWatermark.ChangedDateTimeProperty, out obj))
				{
					return (DateTime?)obj;
				}
				return null;
			}
		}

		public static readonly HygienePropertyDefinition IdProperty = new HygienePropertyDefinition("id_Identity", typeof(Guid));

		public static readonly HygienePropertyDefinition SyncContextProperty = new HygienePropertyDefinition("nvc_SyncContext", typeof(string));

		public static readonly HygienePropertyDefinition DataProperty = new HygienePropertyDefinition("nvc_Data", typeof(string));

		public static readonly HygienePropertyDefinition OwnerProperty = new HygienePropertyDefinition("id_Owner", typeof(Guid?));

		public static readonly HygienePropertyDefinition ChangedDateTimeProperty = new HygienePropertyDefinition("dt_ChangedDateTime", typeof(DateTime?));
	}
}
