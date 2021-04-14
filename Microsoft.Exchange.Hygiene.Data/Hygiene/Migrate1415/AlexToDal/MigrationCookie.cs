using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data;

namespace Microsoft.Exchange.Hygiene.Migrate1415.AlexToDal
{
	internal class MigrationCookie : ConfigurablePropertyBag
	{
		public MigrationCookie()
		{
			this.ID = Guid.NewGuid();
			this.Cookie = MigrationCookie.MinimumCookieValue;
			this.DirectionId = MailDirection.Inbound;
		}

		internal MigrationCookie(ADObjectId tenantId, ObjectId configId, string name)
		{
			this.ID = Guid.NewGuid();
			this.ConfigurationId = configId;
			this.Name = name;
			this.Cookie = MigrationCookie.MinimumCookieValue;
			this.DirectionId = MailDirection.Inbound;
			this.TenantId = tenantId;
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.ID.ToString());
			}
		}

		internal string Name
		{
			get
			{
				return (string)this[MigrationCookie.CookieNameProperty];
			}
			set
			{
				this[MigrationCookie.CookieNameProperty] = value;
			}
		}

		internal byte[] Cookie
		{
			get
			{
				return (byte[])this[MigrationCookie.CookieValueProperty];
			}
			set
			{
				this[MigrationCookie.CookieValueProperty] = value;
			}
		}

		internal ObjectId ConfigurationId
		{
			get
			{
				return (ObjectId)this[MigrationCookie.ConfigurationIdProp];
			}
			set
			{
				this[MigrationCookie.ConfigurationIdProp] = value;
			}
		}

		internal Guid ID
		{
			get
			{
				return (Guid)this[MigrationCookie.IDProperty];
			}
			set
			{
				this[MigrationCookie.IDProperty] = value;
			}
		}

		internal MailDirection DirectionId
		{
			get
			{
				return (MailDirection)this[MigrationCookie.DirectionIdProp];
			}
			set
			{
				this[MigrationCookie.DirectionIdProp] = value;
			}
		}

		internal ADObjectId TenantId
		{
			get
			{
				return this[ADObjectSchema.OrganizationalUnitRoot] as ADObjectId;
			}
			set
			{
				this[ADObjectSchema.OrganizationalUnitRoot] = value;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MigrationCookie()
		{
			byte[] minimumCookieValue = new byte[8];
			MigrationCookie.MinimumCookieValue = minimumCookieValue;
			MigrationCookie.CookieNameProperty = new HygienePropertyDefinition("Name", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
			MigrationCookie.CookieValueProperty = new HygienePropertyDefinition("cookieValue", typeof(byte[]), MigrationCookie.MinimumCookieValue, ADPropertyDefinitionFlags.PersistDefaultValue);
			MigrationCookie.ConfigurationIdProp = new HygienePropertyDefinition("configId", typeof(ADObjectId));
			MigrationCookie.IDProperty = new HygienePropertyDefinition("ID", typeof(Guid));
			MigrationCookie.DirectionIdProp = new HygienePropertyDefinition("directionId", typeof(MailDirection), MailDirection.Inbound, ADPropertyDefinitionFlags.PersistDefaultValue);
		}

		internal static readonly byte[] MinimumCookieValue;

		internal static readonly HygienePropertyDefinition CookieNameProperty;

		internal static readonly HygienePropertyDefinition CookieValueProperty;

		internal static readonly HygienePropertyDefinition ConfigurationIdProp;

		internal static readonly HygienePropertyDefinition IDProperty;

		internal static readonly HygienePropertyDefinition DirectionIdProp;
	}
}
