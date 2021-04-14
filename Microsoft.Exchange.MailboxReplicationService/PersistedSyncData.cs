using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class PersistedSyncData : XMLSerializableBase
	{
		public PersistedSyncData() : this(Guid.Empty)
		{
		}

		internal PersistedSyncData(Guid moveRequestGuid)
		{
			this.MoveRequestGuid = moveRequestGuid;
			this.BadItems = new EntryIdMap<BadItemMarker>();
		}

		[XmlElement(ElementName = "MoveRequestGuid")]
		public Guid MoveRequestGuid { get; set; }

		[XmlIgnore]
		public SyncStage SyncStage { get; set; }

		[XmlElement(ElementName = "SyncStage")]
		public int SyncStageInt
		{
			get
			{
				return (int)this.SyncStage;
			}
			set
			{
				this.SyncStage = (SyncStage)value;
			}
		}

		[XmlIgnore]
		public EntryIdMap<BadItemMarker> BadItems { get; private set; }

		[XmlArray("BadItems")]
		[XmlArrayItem("BadItem")]
		public BadItemMarker[] BadItemsArray
		{
			get
			{
				BadItemMarker[] array = new BadItemMarker[this.BadItems.Count];
				int num = 0;
				foreach (BadItemMarker badItemMarker in this.BadItems.Values)
				{
					array[num++] = badItemMarker;
				}
				return array;
			}
			set
			{
				this.BadItems.Clear();
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						BadItemMarker badItemMarker = value[i];
						this.BadItems.Add(badItemMarker.EntryId, badItemMarker);
					}
				}
			}
		}

		[XmlIgnore]
		public PostMoveCleanupStatusFlags CompletedCleanupTasks { get; set; }

		[XmlElement(ElementName = "CompletedCleanupTasks")]
		public int CompletedCleanupTasksInt
		{
			get
			{
				return (int)this.CompletedCleanupTasks;
			}
			set
			{
				this.CompletedCleanupTasks = (PostMoveCleanupStatusFlags)value;
			}
		}

		[XmlElement(ElementName = "CleanupRetryAttempts")]
		public int CleanupRetryAttempts { get; set; }

		[XmlElement(ElementName = "MailboxSignatureVersion")]
		public uint MailboxSignatureVersion { get; set; }

		[XmlElement(ElementName = "InternalLegacyExchangeDN")]
		public string InternalLegacyExchangeDN { get; set; }

		[XmlElement(ElementName = "ExternalLegacyExchangeDN")]
		public string ExternalLegacyExchangeDN { get; set; }

		internal static PersistedSyncData Deserialize(string data)
		{
			return XMLSerializableBase.Deserialize<PersistedSyncData>(data, true);
		}
	}
}
