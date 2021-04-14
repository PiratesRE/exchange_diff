using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ParticipantList : Collection<Participant>, IConversionParticipantList
	{
		internal ParticipantList(PropertyBag propertyBag, AtomicStorePropertyDefinition participantsBlobStorePropertyDefinition, AtomicStorePropertyDefinition participantsNamesStorePropertyDefinition, AtomicStorePropertyDefinition participantCountStorePropertyDefinition, bool suppressCorruptDataException = false)
		{
			if (participantsBlobStorePropertyDefinition == null)
			{
				throw new ArgumentNullException("participantsBlobStorePropertyDefinition");
			}
			this.participantsBlobStorePropertyDefinition = participantsBlobStorePropertyDefinition;
			this.participantsNamesStorePropertyDefinition = participantsNamesStorePropertyDefinition;
			this.participantsCountStorePropertyDefinition = participantCountStorePropertyDefinition;
			this.propertyBag = propertyBag;
			this.Resync(suppressCorruptDataException);
		}

		internal byte[] Blob
		{
			get
			{
				this.CalculateMapiProperties();
				return this.blob;
			}
			set
			{
				this.CalculateMapiProperties();
				this.blob = value;
				this.isPropertiesChanged = true;
				this.ParseMapiProperties();
			}
		}

		internal string Names
		{
			get
			{
				this.CalculateMapiProperties();
				return this.names;
			}
			set
			{
				this.CalculateMapiProperties();
				this.names = value;
				this.isPropertiesChanged = true;
				this.ParseMapiProperties();
			}
		}

		internal bool IsDirty
		{
			get
			{
				return this.isPropertiesChanged || this.isListChanged || this.isCorrectionNeeded;
			}
		}

		public bool IsConversionParticipantAlwaysResolvable(int index)
		{
			return true;
		}

		internal void Save()
		{
			this.isListChanged |= this.isCorrectionNeeded;
			this.UpdateNativeProperties();
		}

		internal void Resync(bool suppressCorruptDataException)
		{
			if (!this.isListChanged && !this.isPropertiesChanged)
			{
				try
				{
					this.blob = this.propertyBag.GetValueOrDefault<byte[]>(this.participantsBlobStorePropertyDefinition);
					this.names = ((this.participantsNamesStorePropertyDefinition != null) ? this.propertyBag.GetValueOrDefault<string>(this.participantsNamesStorePropertyDefinition) : null);
					this.ParseMapiProperties();
				}
				catch (CorruptDataException)
				{
					if (!suppressCorruptDataException)
					{
						throw;
					}
					base.ClearItems();
					this.isListChanged = false;
					this.isPropertiesChanged = false;
				}
			}
			this.UpdateNativeProperties();
		}

		protected override void InsertItem(int index, Participant participant)
		{
			ParticipantList.VerifyParticipant(participant);
			base.InsertItem(index, participant);
			this.isListChanged = true;
		}

		protected override void SetItem(int index, Participant participant)
		{
			ParticipantList.VerifyParticipant(participant);
			base.SetItem(index, participant);
			this.isListChanged = true;
		}

		protected override void RemoveItem(int index)
		{
			base.RemoveItem(index);
			this.isListChanged = true;
		}

		protected override void ClearItems()
		{
			base.ClearItems();
			this.isListChanged = true;
		}

		private static void VerifyParticipant(Participant participant)
		{
			if (participant.RoutingType == "MAPIPDL" || participant.RoutingType == null)
			{
				throw new InvalidParticipantException(ServerStrings.ExOperationNotSupportedForRoutingType("ParticipantList: Add/Insert/Replace", participant.RoutingType), ParticipantValidationStatus.OperationNotSupportedForRoutingType);
			}
		}

		private void ParseMapiProperties()
		{
			base.Clear();
			if (this.blob != null)
			{
				IList<ParticipantEntryId> list;
				try
				{
					list = ParticipantEntryId.FromFlatEntryList(this.blob);
				}
				catch (CorruptDataException)
				{
					this.isCorrectionNeeded = true;
					throw;
				}
				string[] array = null;
				if (this.names != null)
				{
					array = this.names.Split(new char[]
					{
						';'
					});
					if (array.Length != list.Count)
					{
						array = null;
					}
				}
				int num = 0;
				foreach (ParticipantEntryId participantEntryId in list)
				{
					Participant.Builder builder = new Participant.Builder();
					string text = null;
					if (array != null)
					{
						text = array[num].Trim();
					}
					num++;
					if (participantEntryId is OneOffParticipantEntryId || participantEntryId is ADParticipantEntryId)
					{
						builder.SetPropertiesFrom(participantEntryId);
					}
					else
					{
						this.isCorrectionNeeded = true;
						if (text == null)
						{
							continue;
						}
						builder.EmailAddress = text;
						builder.RoutingType = "SMTP";
					}
					if (text != null)
					{
						builder.DisplayName = text;
					}
					base.Items.Add(builder.ToParticipant());
				}
			}
			this.isListChanged = false;
		}

		private void CalculateMapiProperties()
		{
			if (!this.isListChanged && !this.isCorrectionNeeded)
			{
				return;
			}
			if (base.Count == 0)
			{
				this.names = null;
				this.blob = null;
			}
			else
			{
				ParticipantEntryId[] array = new ParticipantEntryId[base.Count];
				int num = 0;
				StringBuilder stringBuilder = new StringBuilder(base.Count * 16);
				foreach (Participant participant in this)
				{
					if (num != 0)
					{
						stringBuilder.Append(';');
					}
					array[num++] = ParticipantEntryId.FromParticipant(participant, ParticipantEntryIdConsumer.SupportsADParticipantEntryId);
					stringBuilder.Append(participant.DisplayName);
				}
				this.blob = ParticipantEntryId.ToFlatEntryList(array);
				this.names = stringBuilder.ToString();
			}
			this.isListChanged = false;
			this.isCorrectionNeeded = false;
			this.isPropertiesChanged = true;
		}

		private void UpdateNativeProperties()
		{
			if (this.isListChanged)
			{
				this.CalculateMapiProperties();
			}
			if (this.isPropertiesChanged)
			{
				this.propertyBag.SetOrDeleteProperty(this.participantsBlobStorePropertyDefinition, this.blob);
				if (this.participantsNamesStorePropertyDefinition != null)
				{
					this.propertyBag.SetOrDeleteProperty(this.participantsNamesStorePropertyDefinition, this.names);
				}
				if (this.participantsCountStorePropertyDefinition != null)
				{
					if (base.Count == 0)
					{
						this.propertyBag.Delete(this.participantsCountStorePropertyDefinition);
					}
					else
					{
						this.propertyBag.SetProperty(this.participantsCountStorePropertyDefinition, base.Count);
					}
				}
				this.isPropertiesChanged = false;
			}
		}

		private readonly AtomicStorePropertyDefinition participantsNamesStorePropertyDefinition;

		private readonly AtomicStorePropertyDefinition participantsBlobStorePropertyDefinition;

		private readonly AtomicStorePropertyDefinition participantsCountStorePropertyDefinition;

		private readonly PropertyBag propertyBag;

		private bool isPropertiesChanged;

		private bool isListChanged;

		private bool isCorrectionNeeded;

		private byte[] blob;

		private string names;
	}
}
