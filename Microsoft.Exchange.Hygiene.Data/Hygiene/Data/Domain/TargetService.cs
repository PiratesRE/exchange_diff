using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	[Serializable]
	internal class TargetService : ConfigurablePropertyBag, ISerializable
	{
		public TargetService()
		{
			this.WhenChangedUTC = null;
		}

		public TargetService(SerializationInfo info, StreamingContext ctxt)
		{
			this.TargetServiceId = (Guid)info.GetValue(DomainSchema.TargetServiceId.Name, typeof(Guid));
			this.DomainKey = (string)info.GetValue(DomainSchema.DomainKey.Name, typeof(string));
			this.DomainName = (string)info.GetValue(DomainSchema.DomainName.Name, typeof(string));
			this.TenantId = (Guid)info.GetValue(DomainSchema.TenantId.Name, typeof(Guid));
			this.WhenChangedUTC = (DateTime?)info.GetValue(DomainSchema.WhenChangedProp.Name, typeof(DateTime?));
			Dictionary<int, Dictionary<int, string>> dictionary = (Dictionary<int, Dictionary<int, string>>)info.GetValue(DomainSchema.PropertiesAsId.Name, typeof(Dictionary<int, Dictionary<int, string>>));
			this.Properties = new Dictionary<int, Dictionary<int, string>>(dictionary);
		}

		public override ObjectId Identity
		{
			get
			{
				return DomainSchema.GetObjectId(this.TargetServiceId);
			}
		}

		public override ObjectState ObjectState
		{
			get
			{
				return (ObjectState)this[DomainSchema.ObjectStateProp];
			}
		}

		public Guid TargetServiceId
		{
			get
			{
				return (Guid)this[DomainSchema.TargetServiceId];
			}
			private set
			{
				this[DomainSchema.TargetServiceId] = value;
			}
		}

		public string DomainKey
		{
			get
			{
				return this[DomainSchema.DomainKey] as string;
			}
			set
			{
				this[DomainSchema.DomainKey] = DomainSchema.GetNullIfStringEmpty(value);
			}
		}

		public string DomainName
		{
			get
			{
				return this[DomainSchema.DomainName] as string;
			}
			set
			{
				this[DomainSchema.DomainName] = DomainSchema.GetNullIfStringEmpty(value);
			}
		}

		public Guid TenantId
		{
			get
			{
				return DomainSchema.GetGuidEmptyIfNull(this[DomainSchema.TenantId]);
			}
			set
			{
				this[DomainSchema.TenantId] = DomainSchema.GetNullIfGuidEmpty(value);
			}
		}

		public DateTime? WhenChangedUTC
		{
			get
			{
				return (DateTime?)this[DomainSchema.WhenChangedProp];
			}
			set
			{
				this[DomainSchema.WhenChangedProp] = value;
			}
		}

		public Dictionary<int, Dictionary<int, string>> Properties
		{
			get
			{
				return this[DomainSchema.PropertiesAsId] as Dictionary<int, Dictionary<int, string>>;
			}
			set
			{
				this[DomainSchema.PropertiesAsId] = value;
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(DomainSchema.TargetServiceId.Name, this.TargetServiceId);
			info.AddValue(DomainSchema.DomainKey.Name, this.DomainKey);
			info.AddValue(DomainSchema.DomainName.Name, this.DomainName);
			info.AddValue(DomainSchema.TenantId.Name, this.TenantId);
			info.AddValue(DomainSchema.WhenChangedProp.Name, this.WhenChangedUTC);
			info.AddValue(DomainSchema.PropertiesAsId.Name, this.Properties);
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			if (isChangedOnly)
			{
				return base.GetPropertyDefinitions(isChangedOnly);
			}
			return TargetService.propertydefinitions;
		}

		public override string ToString()
		{
			return this.ConvertToString();
		}

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			DomainSchema.TargetServiceId,
			DomainSchema.DomainKey,
			DomainSchema.DomainName,
			DomainSchema.TenantId,
			DomainSchema.PropertiesAsId,
			DomainSchema.ObjectStateProp,
			DomainSchema.IsTracerTokenProp,
			DomainSchema.WhenChangedProp
		};
	}
}
