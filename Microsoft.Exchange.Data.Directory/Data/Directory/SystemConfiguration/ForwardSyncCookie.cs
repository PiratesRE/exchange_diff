using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ForwardSyncCookie : ForwardSyncCookieHeader
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ForwardSyncCookie.SchemaInstance;
			}
		}

		public int Version
		{
			get
			{
				return (int)this[ForwardSyncCookieSchema.Version];
			}
			set
			{
				this[ForwardSyncCookieSchema.Version] = value;
			}
		}

		public bool IsUpgradingSyncPropertySet
		{
			get
			{
				return (bool)this[ForwardSyncCookieSchema.IsUpgradingSyncPropertySet];
			}
			set
			{
				this[ForwardSyncCookieSchema.IsUpgradingSyncPropertySet] = value;
			}
		}

		public int SyncPropertySetVersion
		{
			get
			{
				return (int)this[ForwardSyncCookieSchema.SyncPropertySetVersion];
			}
			set
			{
				this[ForwardSyncCookieSchema.SyncPropertySetVersion] = value;
			}
		}

		public byte[] Data
		{
			get
			{
				return (byte[])this[ForwardSyncCookieSchema.Data];
			}
			set
			{
				this[ForwardSyncCookieSchema.Data] = value;
			}
		}

		public MultiValuedProperty<string> FilteredContextIds
		{
			get
			{
				return (MultiValuedProperty<string>)this[ForwardSyncCookieSchema.FilteredContextIds];
			}
			set
			{
				this[ForwardSyncCookieSchema.FilteredContextIds] = value;
			}
		}

		private static readonly ForwardSyncCookieSchema SchemaInstance = ObjectSchema.GetInstance<ForwardSyncCookieSchema>();

		internal new static readonly string MostDerivedClass = ForwardSyncCookieHeader.MostDerivedClass;
	}
}
