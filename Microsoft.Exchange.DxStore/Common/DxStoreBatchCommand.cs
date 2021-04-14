using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[KnownType(typeof(PropertyValue))]
	[KnownType(typeof(DxStoreBatchCommand.CreateKey))]
	[KnownType(typeof(DxStoreBatchCommand.DeleteKey))]
	[KnownType(typeof(DxStoreBatchCommand.SetProperty))]
	[KnownType(typeof(DxStoreBatchCommand.DeleteProperty))]
	[Serializable]
	public class DxStoreBatchCommand
	{
		public virtual WellKnownBatchCommandName GetTypeId()
		{
			return WellKnownBatchCommandName.Unknown;
		}

		public virtual string GetDebugString()
		{
			return string.Empty;
		}

		[Serializable]
		public class CreateKey : DxStoreBatchCommand
		{
			[DataMember]
			public string Name { get; set; }

			public override WellKnownBatchCommandName GetTypeId()
			{
				return WellKnownBatchCommandName.CreateKey;
			}

			public override string GetDebugString()
			{
				return string.Format("Key={0}", this.Name);
			}
		}

		[Serializable]
		public class DeleteKey : DxStoreBatchCommand
		{
			[DataMember]
			public string Name { get; set; }

			public override WellKnownBatchCommandName GetTypeId()
			{
				return WellKnownBatchCommandName.DeleteKey;
			}

			public override string GetDebugString()
			{
				return string.Format("Key={0}", this.Name);
			}
		}

		[Serializable]
		public class SetProperty : DxStoreBatchCommand
		{
			[DataMember]
			public string Name { get; set; }

			[DataMember]
			public PropertyValue Value { get; set; }

			public override WellKnownBatchCommandName GetTypeId()
			{
				return WellKnownBatchCommandName.SetProperty;
			}

			public override string GetDebugString()
			{
				return string.Format("Name={0} {1}", this.Name, this.Value.GetDebugString());
			}
		}

		[Serializable]
		public class DeleteProperty : DxStoreBatchCommand
		{
			[DataMember]
			public string Name { get; set; }

			public override WellKnownBatchCommandName GetTypeId()
			{
				return WellKnownBatchCommandName.DeleteProperty;
			}

			public override string GetDebugString()
			{
				return string.Format("Name={0}", this.Name);
			}
		}
	}
}
