using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	[Serializable]
	internal class DomainTargetEnvironment : ConfigurablePropertyBag, ISerializable
	{
		public DomainTargetEnvironment()
		{
			this.WhenChangedUTC = null;
		}

		public DomainTargetEnvironment(SerializationInfo info, StreamingContext ctxt)
		{
			this.DomainTargetEnvironmentId = (Guid)info.GetValue(DomainSchema.DomainTargetEnvironmentId.Name, typeof(Guid));
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
				return DomainSchema.GetObjectId(this.DomainTargetEnvironmentId);
			}
		}

		public override ObjectState ObjectState
		{
			get
			{
				return (ObjectState)this[DomainSchema.ObjectStateProp];
			}
		}

		public Guid DomainTargetEnvironmentId
		{
			get
			{
				return (Guid)this[DomainSchema.DomainTargetEnvironmentId];
			}
			private set
			{
				this[DomainSchema.DomainTargetEnvironmentId] = value;
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

		public int DomainKeyFlags
		{
			get
			{
				return (int)this[DomainSchema.DomainKeyFlags];
			}
			set
			{
				this[DomainSchema.DomainKeyFlags] = value;
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
			info.AddValue(DomainSchema.DomainTargetEnvironmentId.Name, this.DomainTargetEnvironmentId);
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
			return DomainTargetEnvironment.propertydefinitions;
		}

		public override string ToString()
		{
			return this.ConvertToString();
		}

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			DomainSchema.DomainTargetEnvironmentId,
			DomainSchema.DomainKey,
			DomainSchema.DomainName,
			DomainSchema.TenantId,
			DomainSchema.PropertiesAsId,
			DomainSchema.ObjectStateProp,
			DomainSchema.DomainKeyFlags,
			DomainSchema.UpdateDomainKey,
			DomainSchema.IsTracerTokenProp,
			DomainSchema.WhenChangedProp
		};
	}
}
