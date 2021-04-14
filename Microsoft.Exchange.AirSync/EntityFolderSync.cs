using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EntityFolderSync : FolderSync
	{
		public EntityFolderSync(ISyncProvider syncProvider, IFolderSyncState syncState, ConflictResolutionPolicy policy, bool deferStateModifications) : base(syncProvider, syncState, policy, deferStateModifications)
		{
		}

		internal AirSyncCalendarSyncState AirSyncCalendarSyncState
		{
			get
			{
				if (!this.syncState.Contains(CustomStateDatumType.CalendarSyncState))
				{
					return AirSyncCalendarSyncState.Empty;
				}
				return (AirSyncCalendarSyncState)this.syncState[CustomStateDatumType.CalendarSyncState];
			}
			set
			{
				this.syncState[CustomStateDatumType.CalendarSyncState] = value;
			}
		}

		internal AirSyncCalendarSyncState AirSyncRecoveryCalendarSyncState
		{
			get
			{
				if (!this.syncState.Contains(CustomStateDatumType.RecoveryCalendarSyncState))
				{
					return AirSyncCalendarSyncState.Empty;
				}
				return (AirSyncCalendarSyncState)this.syncState[CustomStateDatumType.RecoveryCalendarSyncState];
			}
			set
			{
				this.syncState[CustomStateDatumType.RecoveryCalendarSyncState] = value;
			}
		}

		internal Dictionary<ISyncItemId, EntityFolderSync.OcurrenceInformation> MasterItems
		{
			get
			{
				if (!this.syncState.Contains(CustomStateDatumType.CalendarMasterItems))
				{
					return new Dictionary<ISyncItemId, EntityFolderSync.OcurrenceInformation>();
				}
				return ((GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, EntityFolderSync.OcurrenceInformation>)this.syncState[CustomStateDatumType.CalendarMasterItems]).Data;
			}
			set
			{
				this.syncState[CustomStateDatumType.CalendarMasterItems] = new GenericDictionaryData<DerivedData<ISyncItemId>, ISyncItemId, EntityFolderSync.OcurrenceInformation>(value);
			}
		}

		public override void Recover(ISyncClientOperation[] clientOperations)
		{
			base.Recover(clientOperations);
			this.AirSyncCalendarSyncState = this.AirSyncRecoveryCalendarSyncState;
		}

		protected override void SavePreviousState()
		{
			base.SavePreviousState();
			this.AirSyncRecoveryCalendarSyncState = this.AirSyncCalendarSyncState;
		}

		protected override bool GetNewOperations(int windowSize, Dictionary<ISyncItemId, ServerManifestEntry> tempServerManifest)
		{
			bool newOperations = base.GetNewOperations(windowSize, tempServerManifest);
			Dictionary<ISyncItemId, EntityFolderSync.OcurrenceInformation> masterItems = this.MasterItems;
			Dictionary<ISyncItemId, ServerManifestEntry> dictionary = new Dictionary<ISyncItemId, ServerManifestEntry>();
			foreach (ServerManifestEntry serverManifestEntry in tempServerManifest.Values)
			{
				if (serverManifestEntry.ChangeType == ChangeType.Delete)
				{
					if (masterItems.ContainsKey(serverManifestEntry.Id))
					{
						EntityFolderSync.OcurrenceInformation ocurrenceInformation = masterItems[serverManifestEntry.Id];
						foreach (ISyncItemId syncItemId in ocurrenceInformation.Ocurrences)
						{
							dictionary.Add(syncItemId, new ServerManifestEntry(ChangeType.Delete, syncItemId, null));
						}
						masterItems.Remove(serverManifestEntry.Id);
					}
				}
				else if (serverManifestEntry.ChangeType == ChangeType.Add)
				{
					if (serverManifestEntry.CalendarItemType == CalendarItemType.RecurringMaster)
					{
						if (masterItems.ContainsKey(serverManifestEntry.Id))
						{
							EntityFolderSync.OcurrenceInformation ocurrenceInformation = masterItems[serverManifestEntry.Id];
							for (int i = ocurrenceInformation.Ocurrences.Count - 1; i >= 0; i--)
							{
								if (!tempServerManifest.ContainsKey(ocurrenceInformation.Ocurrences[i]))
								{
									dictionary.Add(ocurrenceInformation.Ocurrences[i], new ServerManifestEntry(ChangeType.Delete, ocurrenceInformation.Ocurrences[i], null));
									ocurrenceInformation.Ocurrences.RemoveAt(i);
								}
							}
						}
					}
					else if (serverManifestEntry.CalendarItemType == CalendarItemType.Occurrence || serverManifestEntry.CalendarItemType == CalendarItemType.Exception)
					{
						EntitySyncItemId key = EntitySyncItemId.CreateFromId(serverManifestEntry.SeriesMasterId);
						EntityFolderSync.OcurrenceInformation ocurrenceInformation;
						if (masterItems.ContainsKey(key))
						{
							ocurrenceInformation = masterItems[key];
						}
						else
						{
							ocurrenceInformation = new EntityFolderSync.OcurrenceInformation();
							masterItems[key] = ocurrenceInformation;
						}
						if (!ocurrenceInformation.Ocurrences.Contains(serverManifestEntry.Id))
						{
							ocurrenceInformation.Ocurrences.Add(serverManifestEntry.Id);
						}
					}
				}
			}
			foreach (ServerManifestEntry serverManifestEntry2 in dictionary.Values)
			{
				if (!tempServerManifest.ContainsKey(serverManifestEntry2.Id))
				{
					tempServerManifest.Add(serverManifestEntry2.Id, serverManifestEntry2);
				}
			}
			this.MasterItems = masterItems;
			return newOperations;
		}

		public sealed class OcurrenceInformation : ICustomSerializable
		{
			public OcurrenceInformation()
			{
				this.Ocurrences = new List<ISyncItemId>();
			}

			public List<ISyncItemId> Ocurrences { get; set; }

			public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
			{
				GenericListData<DerivedData<ISyncItemId>, ISyncItemId> genericListData = new GenericListData<DerivedData<ISyncItemId>, ISyncItemId>();
				genericListData.DeserializeData(reader, componentDataPool);
				this.Ocurrences = genericListData.Data;
			}

			public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
			{
				GenericListData<DerivedData<ISyncItemId>, ISyncItemId> genericListData = new GenericListData<DerivedData<ISyncItemId>, ISyncItemId>(this.Ocurrences);
				genericListData.SerializeData(writer, componentDataPool);
			}
		}
	}
}
