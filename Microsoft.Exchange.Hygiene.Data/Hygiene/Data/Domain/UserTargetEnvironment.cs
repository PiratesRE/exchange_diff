using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	[Serializable]
	internal class UserTargetEnvironment : ConfigurablePropertyBag, ISerializable
	{
		public UserTargetEnvironment()
		{
			this.WhenChangedUTC = null;
		}

		public UserTargetEnvironment(SerializationInfo info, StreamingContext ctxt)
		{
			this.UserTargetEnvironmentId = (Guid)info.GetValue(DomainSchema.UserTargetEnvironmentId.Name, typeof(Guid));
			this.UserKey = (string)info.GetValue(DomainSchema.UserKey.Name, typeof(string));
			this.MSAUserName = (string)info.GetValue(DomainSchema.MSAUserName.Name, typeof(string));
			this.TenantId = (Guid)info.GetValue(DomainSchema.TenantId.Name, typeof(Guid));
			this.WhenChangedUTC = (DateTime?)info.GetValue(DomainSchema.WhenChangedProp.Name, typeof(DateTime?));
		}

		public override ObjectId Identity
		{
			get
			{
				return DomainSchema.GetObjectId(this.UserTargetEnvironmentId);
			}
		}

		public override ObjectState ObjectState
		{
			get
			{
				return (ObjectState)this[DomainSchema.ObjectStateProp];
			}
		}

		public Guid UserTargetEnvironmentId
		{
			get
			{
				return (Guid)this[DomainSchema.UserTargetEnvironmentId];
			}
			private set
			{
				this[DomainSchema.UserTargetEnvironmentId] = value;
			}
		}

		public string UserKey
		{
			get
			{
				return this[DomainSchema.UserKey] as string;
			}
			set
			{
				this[DomainSchema.UserKey] = DomainSchema.GetNullIfStringEmpty(value);
			}
		}

		public string MSAUserName
		{
			get
			{
				return this[DomainSchema.MSAUserName] as string;
			}
			set
			{
				this[DomainSchema.MSAUserName] = DomainSchema.GetNullIfStringEmpty(value);
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

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(DomainSchema.UserTargetEnvironmentId.Name, this.UserTargetEnvironmentId);
			info.AddValue(DomainSchema.UserKey.Name, this.UserKey);
			info.AddValue(DomainSchema.MSAUserName.Name, this.MSAUserName);
			info.AddValue(DomainSchema.TenantId.Name, this.TenantId);
			info.AddValue(DomainSchema.WhenChangedProp.Name, this.WhenChangedUTC);
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			if (isChangedOnly)
			{
				return base.GetPropertyDefinitions(isChangedOnly);
			}
			return UserTargetEnvironment.propertydefinitions;
		}

		public override string ToString()
		{
			return this.ConvertToString();
		}

		private static readonly PropertyDefinition[] propertydefinitions = new PropertyDefinition[]
		{
			DomainSchema.UserTargetEnvironmentId,
			DomainSchema.UserKey,
			DomainSchema.MSAUserName,
			DomainSchema.TenantId,
			DomainSchema.ObjectStateProp,
			DomainSchema.IsTracerTokenProp,
			DomainSchema.WhenChangedProp
		};
	}
}
