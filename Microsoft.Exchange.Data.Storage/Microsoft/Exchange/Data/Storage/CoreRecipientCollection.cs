using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CoreRecipientCollection : DisposableObject, IEnumerable<CoreRecipient>, IEnumerable, ILocationIdentifierController
	{
		internal CoreRecipientCollection(ICoreItem coreItem)
		{
			bool flag = false;
			try
			{
				this.recipientTable = new RecipientTable(coreItem);
				this.coreItem = coreItem;
				if (this.CoreItem.Session != null)
				{
					this.recipientTable.BuildRecipientCollection(new Action<IList<NativeStorePropertyDefinition>, object[]>(this.AddRecipientFromTable));
					this.nextRecipientRowId = this.recipientList.Count;
					for (int i = this.recipientList.Count - 1; i >= 0; i--)
					{
						CoreRecipient coreRecipient = this.recipientList[i];
						if (coreRecipient.Participant == null)
						{
							this.RemoveAt(i, !this.CoreItem.IsReadOnly);
						}
					}
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					base.Dispose();
				}
			}
		}

		public int Count
		{
			get
			{
				this.CheckDisposed(null);
				return this.recipientList.Count;
			}
		}

		public LocationIdentifierHelper LocationIdentifierHelperInstance
		{
			get
			{
				this.CheckDisposed(null);
				return this.coreItem.LocationIdentifierHelperInstance;
			}
		}

		internal ICoreItem CoreItem
		{
			get
			{
				this.CheckDisposed(null);
				return this.coreItem;
			}
		}

		internal bool IsDirty
		{
			get
			{
				this.CheckDisposed(null);
				return this.recipientTable.IsDirty;
			}
		}

		public void Remove(int rowId)
		{
			this.CheckDisposed(null);
			if (rowId < 0)
			{
				throw new ArgumentOutOfRangeException("rowId", rowId, "RowId cannot be negative");
			}
			int index = 0;
			if (this.TryFindRecipient(rowId, out index))
			{
				this.RemoveAt(index);
			}
		}

		public CoreRecipient CreateOrReplace(int rowId)
		{
			this.CheckDisposed(null);
			if (rowId < 0)
			{
				throw new ArgumentOutOfRangeException("rowId", rowId, "RowId cannot be negative");
			}
			int index = 0;
			CoreRecipient coreRecipient2;
			if (this.TryFindRecipient(rowId, out index))
			{
				CoreRecipient coreRecipient = this.recipientList[index];
				coreRecipient.OnRemoveRecipient();
				coreRecipient2 = new CoreRecipient(this.recipientTable, rowId);
				this.recipientList[index] = coreRecipient2;
			}
			else
			{
				coreRecipient2 = new CoreRecipient(this.recipientTable, rowId);
				this.recipientList.Insert(index, coreRecipient2);
				if (rowId >= this.nextRecipientRowId)
				{
					this.nextRecipientRowId = rowId + 1;
				}
			}
			return coreRecipient2;
		}

		public void Clear()
		{
			this.CheckDisposed(null);
			for (int i = this.recipientList.Count - 1; i >= 0; i--)
			{
				this.RemoveAt(i);
			}
		}

		public IEnumerator<CoreRecipient> GetEnumerator()
		{
			this.CheckDisposed(null);
			return this.recipientList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			this.CheckDisposed(null);
			return this.GetEnumerator();
		}

		internal bool Contains(CoreRecipient coreRecipient)
		{
			this.CheckDisposed(null);
			return this.IndexOf(coreRecipient) != -1;
		}

		internal int IndexOf(CoreRecipient coreRecipient)
		{
			this.CheckDisposed(null);
			return this.recipientList.IndexOf(coreRecipient);
		}

		internal int IndexOf(RecipientId id)
		{
			this.CheckDisposed(null);
			for (int i = 0; i < this.recipientList.Count; i++)
			{
				CoreRecipient coreRecipient = this.recipientList[i];
				if (coreRecipient.Id.Equals(id))
				{
					return i;
				}
			}
			return -1;
		}

		internal CoreRecipient GetCoreRecipient(int index)
		{
			this.CheckDisposed(null);
			return this.recipientList[index];
		}

		internal bool FindRemovedRecipient(Participant participant, out CoreRecipient recipient)
		{
			this.CheckDisposed(null);
			return this.recipientTable.FindRemovedRecipient(participant, out recipient);
		}

		internal void RemoveAt(int index)
		{
			this.RemoveAt(index, true);
		}

		private void RemoveAt(int index, bool trackRecipientChanges)
		{
			this.CheckDisposed(null);
			CoreRecipient coreRecipient = this.recipientList[index];
			this.recipientList.RemoveAt(index);
			if (trackRecipientChanges)
			{
				coreRecipient.OnRemoveRecipient();
			}
		}

		internal void CopyRecipientsFrom(CoreRecipientCollection recipientCollection)
		{
			this.CheckDisposed(null);
			foreach (CoreRecipient sourceCoreRecipient in recipientCollection.recipientList)
			{
				this.CreateCoreRecipient(sourceCoreRecipient);
			}
		}

		internal void Save()
		{
			this.CheckDisposed(null);
			this.LookupMandatoryPropertiesIfNeeded();
			this.recipientTable.Save();
		}

		internal CoreRecipient CreateCoreRecipient(CoreRecipient sourceCoreRecipient)
		{
			this.CheckDisposed(null);
			CoreRecipient coreRecipient = new CoreRecipient(sourceCoreRecipient, this.nextRecipientRowId++, this.recipientTable.RecipientChangeTracker, this.recipientTable.ExTimeZone);
			this.recipientList.Add(coreRecipient);
			return coreRecipient;
		}

		internal CoreRecipient CreateCoreRecipient(CoreRecipient.SetDefaultPropertiesDelegate setDefaultPropertiesDelegate, Participant participant)
		{
			this.CheckDisposed(null);
			CoreRecipient coreRecipient = new CoreRecipient(this.recipientTable, this.nextRecipientRowId++, setDefaultPropertiesDelegate, participant);
			this.recipientList.Add(coreRecipient);
			return coreRecipient;
		}

		internal void FilterRecipients(Predicate<CoreRecipient> shouldKeepRecipient)
		{
			for (int i = this.recipientList.Count - 1; i >= 0; i--)
			{
				if (!shouldKeepRecipient(this.recipientList[i]))
				{
					this.RemoveAt(i);
				}
			}
		}

		internal void LoadAdditionalParticipantProperties(PropertyDefinition[] keyProperties)
		{
			this.CheckDisposed(null);
			if (StandaloneFuzzing.IsEnabled)
			{
				return;
			}
			Participant.Job job = new Participant.Job(this.Count);
			foreach (CoreRecipient coreRecipient in this)
			{
				if (coreRecipient.Participant == null)
				{
					throw new InvalidOperationException("The Participant is not present. This recipient has not been fully formed.");
				}
				job.Add(new Participant.JobItem((coreRecipient.Participant.RoutingType == "EX") ? coreRecipient.Participant : null));
			}
			this.ExecuteJob(job, keyProperties);
			for (int i = 0; i < this.Count; i++)
			{
				if (job[i].Result != null && job[i].Error == null)
				{
					Participant participant = job[i].Result.ToParticipant();
					if (participant.ValidationStatus == ParticipantValidationStatus.NoError)
					{
						this.recipientList[i].InternalUpdateParticipant(participant);
					}
				}
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CoreRecipientCollection>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.recipientTable != null)
			{
				this.recipientTable.Dispose();
			}
			base.InternalDispose(disposing);
		}

		private bool TryFindRecipient(int rowId, out int indexOfInsertionPoint)
		{
			indexOfInsertionPoint = 0;
			int num = Math.Min(rowId, this.recipientList.Count - 1);
			int i;
			for (i = num; i >= 0; i--)
			{
				if (this.recipientList[i].RowId == rowId)
				{
					indexOfInsertionPoint = i;
					return true;
				}
				if (this.recipientList[i].RowId < rowId)
				{
					indexOfInsertionPoint = i + 1;
					return false;
				}
			}
			indexOfInsertionPoint = Math.Max(0, i);
			return false;
		}

		private void LookupMandatoryPropertiesIfNeeded()
		{
			this.LoadAdditionalParticipantProperties(CoreRecipientCollection.AdditionalParticipantProperties);
		}

		private void AddRecipientFromTable(IList<NativeStorePropertyDefinition> propertyDefinitions, object[] values)
		{
			CoreRecipient item = new CoreRecipient(this.recipientTable, propertyDefinitions, values);
			this.recipientList.Add(item);
		}

		internal void ExecuteJob(Participant.Job job, PropertyDefinition[] keyProperties)
		{
			ADSessionSettings adsessionSettings = Participant.BatchBuilder.GetADSessionSettings(this.CoreItem);
			Participant.BatchBuilder.Execute(job, new Participant.BatchBuilder[]
			{
				Participant.BatchBuilder.RequestAllProperties(),
				Participant.BatchBuilder.CopyPropertiesFromInput(),
				Participant.BatchBuilder.GetPropertiesFromAD(null, adsessionSettings, keyProperties)
			});
		}

		private static readonly PropertyDefinition[] AdditionalParticipantProperties = new PropertyDefinition[]
		{
			ParticipantSchema.IsDistributionList
		};

		private readonly IList<CoreRecipient> recipientList = new List<CoreRecipient>();

		private readonly RecipientTable recipientTable;

		private readonly ICoreItem coreItem;

		private int nextRecipientRowId;
	}
}
