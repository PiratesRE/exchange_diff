using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Serializable]
	internal class ADMiniDomain : ConfigurablePropertyBag, ISerializable
	{
		public ADMiniDomain()
		{
		}

		protected ADMiniDomain(SerializationInfo info, StreamingContext ctxt)
		{
			this.DomainId = (ADObjectId)info.GetValue(ADMiniDomainSchema.DomainIdProp.Name, typeof(ADObjectId));
			this.TenantId = (ADObjectId)info.GetValue(ADMiniDomainSchema.TenantIdProp.Name, typeof(ADObjectId));
			this.ConfigurationId = (ADObjectId)info.GetValue(ADMiniDomainSchema.ConfigurationIdProp.Name, typeof(ADObjectId));
			this.ParentDomainId = (ADObjectId)info.GetValue(ADMiniDomainSchema.ParentDomainIdProp.Name, typeof(ADObjectId));
			this.DomainName = info.GetString(ADMiniDomainSchema.DomainNameProp.Name);
			this.EdgeBlockMode = (EdgeBlockMode)info.GetValue(ADMiniDomainSchema.EdgeBlockModeProp.Name, typeof(EdgeBlockMode));
			this.CatchAll = info.GetBoolean(ADMiniDomainSchema.IsCatchAllProp.Name);
			this.IsInitialDomain = info.GetBoolean(ADMiniDomainSchema.IsInitialDomainProp.Name);
			this.IsDefaultDomain = info.GetBoolean(ADMiniDomainSchema.IsDefaultDomainProp.Name);
			this.MailServer = info.GetString(ADMiniDomainSchema.MailServerProp.Name);
			this.LiveType = info.GetString(ADMiniDomainSchema.LiveTypeProp.Name);
			this.LiveNetId = info.GetString(ADMiniDomainSchema.LiveNetIdProp.Name);
		}

		public override ObjectId Identity
		{
			get
			{
				return this.DomainId;
			}
		}

		public override ObjectState ObjectState
		{
			get
			{
				return (ObjectState)this[ADMiniDomainSchema.ObjectStateProp];
			}
		}

		public ADObjectId DomainId
		{
			get
			{
				return this[ADMiniDomainSchema.DomainIdProp] as ADObjectId;
			}
			set
			{
				this[ADMiniDomainSchema.DomainIdProp] = value;
			}
		}

		public ADObjectId TenantId
		{
			get
			{
				return this[ADMiniDomainSchema.TenantIdProp] as ADObjectId;
			}
			set
			{
				this[ADMiniDomainSchema.TenantIdProp] = value;
			}
		}

		public ADObjectId ConfigurationId
		{
			get
			{
				return this[ADMiniDomainSchema.ConfigurationIdProp] as ADObjectId;
			}
			set
			{
				this[ADMiniDomainSchema.ConfigurationIdProp] = value;
			}
		}

		public ADObjectId ParentDomainId
		{
			get
			{
				return this[ADMiniDomainSchema.ParentDomainIdProp] as ADObjectId;
			}
			set
			{
				this[ADMiniDomainSchema.ParentDomainIdProp] = value;
			}
		}

		public string DomainName
		{
			get
			{
				return this[ADMiniDomainSchema.DomainNameProp] as string;
			}
			set
			{
				this[ADMiniDomainSchema.DomainNameProp] = value;
			}
		}

		public int DomainFlags
		{
			get
			{
				return (int)this[ADMiniDomainSchema.DomainFlagsProperty];
			}
			set
			{
				this[ADMiniDomainSchema.DomainFlagsProperty] = value;
			}
		}

		public EdgeBlockMode EdgeBlockMode
		{
			get
			{
				return (EdgeBlockMode)(this[ADMiniDomainSchema.EdgeBlockModeProp] ?? EdgeBlockMode.None);
			}
			set
			{
				this[ADMiniDomainSchema.EdgeBlockModeProp] = value;
			}
		}

		public ADObjectId HygieneConfigurationLink
		{
			get
			{
				return this[ADMiniDomainSchema.HygieneConfigurationLink] as ADObjectId;
			}
			set
			{
				this[ADMiniDomainSchema.HygieneConfigurationLink] = value;
			}
		}

		public bool CatchAll
		{
			get
			{
				return (bool)(this[ADMiniDomainSchema.IsCatchAllProp] ?? false);
			}
			set
			{
				this[ADMiniDomainSchema.IsCatchAllProp] = value;
			}
		}

		public bool IsDefaultDomain
		{
			get
			{
				return (bool)(this[ADMiniDomainSchema.IsDefaultDomainProp] ?? false);
			}
			set
			{
				this[ADMiniDomainSchema.IsDefaultDomainProp] = value;
			}
		}

		public string MailServer
		{
			get
			{
				return this[ADMiniDomainSchema.MailServerProp] as string;
			}
			set
			{
				this[ADMiniDomainSchema.MailServerProp] = value;
			}
		}

		public bool IsInitialDomain
		{
			get
			{
				return (bool)(this[ADMiniDomainSchema.IsInitialDomainProp] ?? false);
			}
			set
			{
				this[ADMiniDomainSchema.IsInitialDomainProp] = value;
			}
		}

		public string LiveType
		{
			get
			{
				return this[ADMiniDomainSchema.LiveTypeProp] as string;
			}
			set
			{
				this[ADMiniDomainSchema.LiveTypeProp] = value;
			}
		}

		public string LiveNetId
		{
			get
			{
				return this[ADMiniDomainSchema.LiveNetIdProp] as string;
			}
			set
			{
				this[ADMiniDomainSchema.LiveNetIdProp] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(ADMiniDomainSchema);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(ADMiniDomainSchema.DomainIdProp.Name, this.DomainId);
			info.AddValue(ADMiniDomainSchema.TenantIdProp.Name, this.TenantId);
			info.AddValue(ADMiniDomainSchema.ConfigurationIdProp.Name, this.ConfigurationId);
			info.AddValue(ADMiniDomainSchema.ParentDomainIdProp.Name, this.ParentDomainId);
			info.AddValue(ADMiniDomainSchema.DomainNameProp.Name, this.DomainName);
			info.AddValue(ADMiniDomainSchema.EdgeBlockModeProp.Name, this.EdgeBlockMode);
			info.AddValue(ADMiniDomainSchema.IsCatchAllProp.Name, this.CatchAll);
			info.AddValue(ADMiniDomainSchema.IsDefaultDomainProp.Name, this.IsDefaultDomain);
			info.AddValue(ADMiniDomainSchema.IsInitialDomainProp.Name, this.IsInitialDomain);
			info.AddValue(ADMiniDomainSchema.MailServerProp.Name, this.MailServer);
			info.AddValue(ADMiniDomainSchema.LiveTypeProp.Name, this.LiveType);
			info.AddValue(ADMiniDomainSchema.LiveNetIdProp.Name, this.LiveNetId);
		}
	}
}
