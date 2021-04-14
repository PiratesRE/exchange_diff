using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[KnownType(typeof(DxStoreCommand.CreateKey))]
	[KnownType(typeof(DxStoreCommand.ExecuteBatch))]
	[KnownType(typeof(DxStoreCommand.ApplySnapshot))]
	[KnownType(typeof(DxStoreCommand.PromoteToLeader))]
	[KnownType(typeof(DxStoreCommand.DeleteKey))]
	[KnownType(typeof(DxStoreCommand.SetProperty))]
	[KnownType(typeof(DxStoreCommand.DummyCommand))]
	[KnownType(typeof(DxStoreCommand.DeleteProperty))]
	[Serializable]
	public class DxStoreCommand
	{
		public DxStoreCommand()
		{
			this.CommandId = Guid.NewGuid();
		}

		[DataMember]
		public bool IsNotifyInitiator { get; set; }

		[DataMember]
		public string Initiator { get; set; }

		[DataMember]
		public DateTimeOffset TimeInitiated { get; set; }

		[DataMember]
		public Guid CommandId { get; set; }

		public virtual string GetDebugString()
		{
			return string.Format("Id={0}, From={1}, Time={2}, IsNotify={3}", new object[]
			{
				this.CommandId,
				this.Initiator,
				this.TimeInitiated.ToShortString(),
				this.IsNotifyInitiator
			});
		}

		public virtual WellKnownCommandName GetTypeId()
		{
			return WellKnownCommandName.Unknown;
		}

		public void Initialize(string self, WriteOptions options)
		{
			this.TimeInitiated = DateTimeOffset.Now;
			this.Initiator = self;
			if (options != null)
			{
				this.IsNotifyInitiator = options.IsWaitRequired();
			}
		}

		[Serializable]
		public class CreateKey : DxStoreCommand
		{
			[DataMember]
			public string KeyName { get; set; }

			[DataMember]
			public string SubkeyName { get; set; }

			public string FullKeyName
			{
				get
				{
					return Utils.CombinePathNullSafe(this.KeyName, this.SubkeyName);
				}
			}

			public override string GetDebugString()
			{
				return string.Format("{0}, ParentKey={1}, SubKey={2}", base.GetDebugString(), this.KeyName, this.SubkeyName);
			}

			public override WellKnownCommandName GetTypeId()
			{
				return WellKnownCommandName.CreateKey;
			}
		}

		[Serializable]
		public class DeleteKey : DxStoreCommand
		{
			[DataMember]
			public string KeyName { get; set; }

			[DataMember]
			public string SubkeyName { get; set; }

			public string FullKeyName
			{
				get
				{
					return Utils.CombinePathNullSafe(this.KeyName, this.SubkeyName);
				}
			}

			public override string GetDebugString()
			{
				return string.Format("{0}, ParentKey={1}, SubKey={2}", base.GetDebugString(), this.KeyName, this.SubkeyName);
			}

			public override WellKnownCommandName GetTypeId()
			{
				return WellKnownCommandName.DeleteKey;
			}
		}

		[Serializable]
		public class SetProperty : DxStoreCommand
		{
			[DataMember]
			public string KeyName { get; set; }

			[DataMember]
			public string Name { get; set; }

			[DataMember]
			public PropertyValue Value { get; set; }

			public override WellKnownCommandName GetTypeId()
			{
				return WellKnownCommandName.SetProperty;
			}

			public override string GetDebugString()
			{
				return string.Format("{0}, Key={1}, Property={2}, {3}", new object[]
				{
					base.GetDebugString(),
					this.KeyName,
					this.Name,
					this.Value.GetDebugString()
				});
			}
		}

		[Serializable]
		public class DeleteProperty : DxStoreCommand
		{
			[DataMember]
			public string KeyName { get; set; }

			[DataMember]
			public string Name { get; set; }

			public override WellKnownCommandName GetTypeId()
			{
				return WellKnownCommandName.DeleteProperty;
			}

			public override string GetDebugString()
			{
				return string.Format("{0}, Key={1}, Property={2}, ", this.KeyName, this.Name, base.GetDebugString());
			}
		}

		[Serializable]
		public class ExecuteBatch : DxStoreCommand
		{
			[DataMember]
			public string KeyName { get; set; }

			[DataMember]
			public DxStoreBatchCommand[] Commands { get; set; }

			public override WellKnownCommandName GetTypeId()
			{
				return WellKnownCommandName.ExecuteBatch;
			}

			public override string GetDebugString()
			{
				return string.Format("{0}, Key={1}, CommandsCount={2}, ", this.KeyName, this.Commands.Length, base.GetDebugString());
			}
		}

		[Serializable]
		public class ApplySnapshot : DxStoreCommand
		{
			[DataMember]
			public InstanceSnapshotInfo SnapshotInfo { get; set; }

			public override WellKnownCommandName GetTypeId()
			{
				return WellKnownCommandName.ApplySnapshot;
			}

			public override string GetDebugString()
			{
				return string.Format("{0}, Key={1}, Size={2}, IsCompressed={3}", new object[]
				{
					base.GetDebugString(),
					this.SnapshotInfo.FullKeyName,
					this.SnapshotInfo.Snapshot.Length,
					this.SnapshotInfo.IsCompressed
				});
			}
		}

		[Serializable]
		public class PromoteToLeader : DxStoreCommand
		{
			public override WellKnownCommandName GetTypeId()
			{
				return WellKnownCommandName.PromoteToLeader;
			}
		}

		[Serializable]
		public class DummyCommand : DxStoreCommand
		{
			[DataMember]
			public Guid OriginalDbCommandId { get; set; }

			public override WellKnownCommandName GetTypeId()
			{
				return WellKnownCommandName.DummyCmd;
			}

			public override string GetDebugString()
			{
				return string.Format("{0}, OrgCmdId={1}", base.GetDebugString(), this.OriginalDbCommandId);
			}
		}
	}
}
