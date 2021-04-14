using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	[Serializable]
	internal class TenantTargetEnvironment : ConfigurablePropertyBag, ISerializable
	{
		public TenantTargetEnvironment()
		{
			this.WhenChangedUTC = null;
		}

		public TenantTargetEnvironment(SerializationInfo info, StreamingContext ctxt)
		{
			this.TenantTargetEnvironmentId = (Guid)info.GetValue(DomainSchema.TenantTargetEnvironmentId.Name, typeof(Guid));
			this.TenantId = (Guid)info.GetValue(DomainSchema.TenantId.Name, typeof(Guid));
			this.WhenChangedUTC = (DateTime?)info.GetValue(DomainSchema.WhenChangedProp.Name, typeof(DateTime?));
			Dictionary<int, Dictionary<int, string>> dictionary = (Dictionary<int, Dictionary<int, string>>)info.GetValue(DomainSchema.PropertiesAsId.Name, typeof(Dictionary<int, Dictionary<int, string>>));
			this.Properties = new Dictionary<int, Dictionary<int, string>>(dictionary);
		}

		public override ObjectId Identity
		{
			get
			{
				return DomainSchema.GetObjectId(this.TenantTargetEnvironmentId);
			}
		}

		public override ObjectState ObjectState
		{
			get
			{
				return (ObjectState)this[DomainSchema.ObjectStateProp];
			}
		}

		public Guid TenantTargetEnvironmentId
		{
			get
			{
				return (Guid)this[DomainSchema.TenantTargetEnvironmentId];
			}
			private set
			{
				this[DomainSchema.TenantTargetEnvironmentId] = value;
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
			info.AddValue(DomainSchema.TenantTargetEnvironmentId.Name, this.TenantTargetEnvironmentId);
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
			return TenantTargetEnvironment.propertydefinitions;
		}

		public override string ToString()
		{
			return this.ConvertToString();
		}

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			DomainSchema.TenantTargetEnvironmentId,
			DomainSchema.TenantId,
			DomainSchema.PropertiesAsId,
			DomainSchema.ObjectStateProp,
			DomainSchema.IsTracerTokenProp,
			DomainSchema.WhenChangedProp
		};
	}
}
