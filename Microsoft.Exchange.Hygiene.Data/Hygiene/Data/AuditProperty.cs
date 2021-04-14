using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class AuditProperty : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.InstanceId.ToString() + this.Sequence.ToString());
			}
		}

		public string UserId
		{
			get
			{
				return this[AuditProperty.UserIdProp] as string;
			}
			set
			{
				this[AuditProperty.UserIdProp] = value;
			}
		}

		public Guid AuditId
		{
			get
			{
				return (Guid)this[AuditProperty.AuditIdProp];
			}
			set
			{
				this[AuditProperty.AuditIdProp] = value;
			}
		}

		public int Sequence
		{
			get
			{
				return (int)this[AuditProperty.SequenceProp];
			}
			set
			{
				this[AuditProperty.SequenceProp] = value;
			}
		}

		public Guid? InstanceId
		{
			get
			{
				return this[AuditProperty.InstanceIdProp] as Guid?;
			}
			set
			{
				this[AuditProperty.InstanceIdProp] = value;
			}
		}

		public string EntityName
		{
			get
			{
				return this[AuditProperty.EntityNameProp] as string;
			}
			set
			{
				this[AuditProperty.EntityNameProp] = value;
			}
		}

		public string PropertyName
		{
			get
			{
				return this[AuditProperty.PropertyNameProp] as string;
			}
			set
			{
				this[AuditProperty.PropertyNameProp] = value;
			}
		}

		public int PropertyIndex
		{
			get
			{
				return (int)this[AuditProperty.PropertyIndexProp];
			}
			set
			{
				this[AuditProperty.PropertyIndexProp] = value;
			}
		}

		public int? IntegerChange
		{
			get
			{
				return this[AuditProperty.IntegerChangeProp] as int?;
			}
			set
			{
				this[AuditProperty.IntegerChangeProp] = value;
			}
		}

		public string StringChange
		{
			get
			{
				return this[AuditProperty.StringChangeProp] as string;
			}
			set
			{
				this[AuditProperty.StringChangeProp] = value;
			}
		}

		public DateTime? DateTimeChange
		{
			get
			{
				return this[AuditProperty.DateTimeChangeProp] as DateTime?;
			}
			set
			{
				this[AuditProperty.DateTimeChangeProp] = value;
			}
		}

		public double? DecimalChange
		{
			get
			{
				return this[AuditProperty.DecimalChangeProp] as double?;
			}
			set
			{
				this[AuditProperty.DecimalChangeProp] = value;
			}
		}

		public Guid? IdChange
		{
			get
			{
				return this[AuditProperty.IdChangeProp] as Guid?;
			}
			set
			{
				this[AuditProperty.EntityNameProp] = value;
			}
		}

		public string BlobChange
		{
			get
			{
				return this[AuditProperty.BlobChangeProp] as string;
			}
			set
			{
				this[AuditProperty.BlobChangeProp] = value;
			}
		}

		public bool? BoolChange
		{
			get
			{
				return this[AuditProperty.BoolChangeProp] as bool?;
			}
			set
			{
				this[AuditProperty.BoolChangeProp] = value;
			}
		}

		public bool Deleted
		{
			get
			{
				return (bool)this[AuditProperty.RecordSoftDeleted];
			}
			set
			{
				this[AuditProperty.RecordSoftDeleted] = value;
			}
		}

		public static readonly HygienePropertyDefinition PartitionIdProp = new HygienePropertyDefinition("id_PartitionId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition UserIdProp = new HygienePropertyDefinition("nvc_UserId", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition AuditIdProp = new HygienePropertyDefinition("id_AuditId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition SequenceProp = new HygienePropertyDefinition("i_Sequence", typeof(int), -1, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition InstanceIdProp = new HygienePropertyDefinition("id_InstanceId", typeof(Guid));

		public static readonly HygienePropertyDefinition EntityNameProp = new HygienePropertyDefinition("nvc_EntityName", typeof(string));

		public static readonly HygienePropertyDefinition PropertyNameProp = new HygienePropertyDefinition("nvc_PropertyName", typeof(string));

		public static readonly HygienePropertyDefinition PropertyIndexProp = new HygienePropertyDefinition("nvc_PropertyIndex", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition IntegerChangeProp = new HygienePropertyDefinition("i_PropertyValueInteger", typeof(int?));

		public static readonly HygienePropertyDefinition StringChangeProp = new HygienePropertyDefinition("nvc_PropertyValueString", typeof(string));

		public static readonly HygienePropertyDefinition DateTimeChangeProp = new HygienePropertyDefinition("dt_PropertyValueDateTime", typeof(DateTime?));

		public static readonly HygienePropertyDefinition DecimalChangeProp = new HygienePropertyDefinition("d_PropertyValueDecimal", typeof(double?));

		public static readonly HygienePropertyDefinition IdChangeProp = new HygienePropertyDefinition("id_PropertyValueGuid", typeof(Guid));

		public static readonly HygienePropertyDefinition BlobChangeProp = new HygienePropertyDefinition("nvc_PropertyValueBlob", typeof(string));

		public static readonly HygienePropertyDefinition BoolChangeProp = new HygienePropertyDefinition("f_PropertyValueBit", typeof(bool?));

		public static readonly HygienePropertyDefinition RecordSoftDeleted = new HygienePropertyDefinition("f_deleted", typeof(bool));

		public static readonly Dictionary<Type, HygienePropertyDefinition> TypeToProertyMap = new Dictionary<Type, HygienePropertyDefinition>
		{
			{
				AuditProperty.IdChangeProp.Type,
				AuditProperty.IdChangeProp
			},
			{
				AuditProperty.IntegerChangeProp.Type,
				AuditProperty.IntegerChangeProp
			},
			{
				AuditProperty.StringChangeProp.Type,
				AuditProperty.StringChangeProp
			},
			{
				AuditProperty.DateTimeChangeProp.Type,
				AuditProperty.DateTimeChangeProp
			},
			{
				AuditProperty.BoolChangeProp.Type,
				AuditProperty.BoolChangeProp
			},
			{
				AuditProperty.DecimalChangeProp.Type,
				AuditProperty.DecimalChangeProp
			}
		};
	}
}
