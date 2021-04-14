using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class SetEntityDataRequest : ConfigurablePropertyBag
	{
		public Guid PartitionId
		{
			get
			{
				return (Guid)this[SetEntityDataRequest.PartitionIdPropertyDefinition];
			}
			set
			{
				this[SetEntityDataRequest.PartitionIdPropertyDefinition] = value;
			}
		}

		public string TableName
		{
			get
			{
				return (string)this[SetEntityDataRequest.TableNamePropertyDefinition];
			}
			set
			{
				this[SetEntityDataRequest.TableNamePropertyDefinition] = value;
			}
		}

		public string ColumnName
		{
			get
			{
				return (string)this[SetEntityDataRequest.ColumnNamePropertyDefinition];
			}
			set
			{
				this[SetEntityDataRequest.ColumnNamePropertyDefinition] = value;
			}
		}

		public string Condition
		{
			get
			{
				return (string)this[SetEntityDataRequest.ConditionPropertyDefinition];
			}
			set
			{
				this[SetEntityDataRequest.ConditionPropertyDefinition] = value;
			}
		}

		public string NewValue
		{
			get
			{
				return (string)this[SetEntityDataRequest.NewValuePropertyDefinition];
			}
			set
			{
				this[SetEntityDataRequest.NewValuePropertyDefinition] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public static readonly HygienePropertyDefinition PartitionIdPropertyDefinition = new HygienePropertyDefinition("id_PartitionId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition TableNamePropertyDefinition = new HygienePropertyDefinition("nvc_TableName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ColumnNamePropertyDefinition = new HygienePropertyDefinition("nvc_ColumnName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition ConditionPropertyDefinition = new HygienePropertyDefinition("nvc_Condition", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		internal static readonly HygienePropertyDefinition NewValuePropertyDefinition = new HygienePropertyDefinition("nvc_NewValue", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
