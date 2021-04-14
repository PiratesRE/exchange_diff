using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ConversionAddressCollection
	{
		protected ConversionAddressCollection(bool useSimpleDisplayName, bool ewsOutboundMimeConversion)
		{
			this.participantLists = new List<IConversionParticipantList>();
			this.useSimpleDisplayName = useSimpleDisplayName;
			this.ewsOutboundMimeConversion = ewsOutboundMimeConversion;
		}

		internal void AddParticipantList(IConversionParticipantList list)
		{
			this.participantLists.Add(list);
		}

		protected ConversionAddressCollection.ParticipantResolutionList CreateResolutionList()
		{
			ConversionAddressCollection.ParticipantResolutionList participantResolutionList = new ConversionAddressCollection.ParticipantResolutionList();
			foreach (IConversionParticipantList conversionParticipantList in this.participantLists)
			{
				int count = conversionParticipantList.Count;
				for (int num = 0; num != count; num++)
				{
					Participant participant = null;
					Participant participant2 = conversionParticipantList[num];
					if (conversionParticipantList.IsConversionParticipantAlwaysResolvable(num) || this.CanResolveParticipant(participant2))
					{
						participant = participant2;
					}
					if (!this.disableLengthValidation && participant2 != null && (participant2.ValidationStatus == ParticipantValidationStatus.EmailAddressTooBig || participant2.ValidationStatus == ParticipantValidationStatus.RoutingTypeTooBig))
					{
						StorageGlobals.ContextTraceError(ExTraceGlobals.CcInboundMimeTracer, "ConversionAddressCollection::CreateResolutionList: participant validation failed.");
						throw new ConversionFailedException(ConversionFailureReason.ExceedsLimit);
					}
					participantResolutionList.AddParticipantForResolution(participant);
				}
			}
			return participantResolutionList;
		}

		protected void ResolveParticipants(ConversionAddressCollection.ParticipantResolutionList resolutionList)
		{
			Participant.Job job = resolutionList.CreateResolutionJob();
			if (this.ewsOutboundMimeConversion)
			{
				PropertyDefinition propertyDefinition;
				Participant.BatchBuilder.Execute(job, new Participant.BatchBuilder[]
				{
					Participant.BatchBuilder.ConvertRoutingType(this.TargetResolutionType, out propertyDefinition),
					Participant.BatchBuilder.RequestAllProperties(),
					Participant.BatchBuilder.CopyPropertiesFromInput(),
					Participant.BatchBuilder.GetPropertiesFromAD(this.GetRecipientCache(job.Count), null),
					this.useSimpleDisplayName ? Participant.BatchBuilder.ReplaceProperty(ParticipantSchema.DisplayName, ParticipantSchema.SimpleDisplayName) : null
				});
			}
			else
			{
				PropertyDefinition propertyDefinition;
				Participant.BatchBuilder.Execute(job, new Participant.BatchBuilder[]
				{
					Participant.BatchBuilder.ConvertRoutingType(this.TargetResolutionType, out propertyDefinition),
					Participant.BatchBuilder.RequestAllProperties(),
					Participant.BatchBuilder.GetPropertiesFromAD(this.GetRecipientCache(job.Count), null),
					Participant.BatchBuilder.CopyPropertiesFromInput(),
					this.useSimpleDisplayName ? Participant.BatchBuilder.ReplaceProperty(ParticipantSchema.DisplayName, ParticipantSchema.SimpleDisplayName) : null
				});
			}
			job.ApplyResults();
		}

		protected void SetResolvedParticipants(ConversionAddressCollection.ParticipantResolutionList participantList)
		{
			foreach (IConversionParticipantList conversionParticipantList in this.participantLists)
			{
				int count = conversionParticipantList.Count;
				for (int num = 0; num != count; num++)
				{
					Participant nextResolvedParticipant = participantList.GetNextResolvedParticipant();
					if (nextResolvedParticipant != null)
					{
						if (nextResolvedParticipant.ValidationStatus == ParticipantValidationStatus.NoError)
						{
							conversionParticipantList[num] = nextResolvedParticipant;
						}
						else
						{
							StorageGlobals.ContextTraceError<Participant, Participant>(ExTraceGlobals.CcGenericTracer, "The resolved Participant is invalid. The source Participant will be used instead\r\n\tSource: {0}\r\n\tResolved: {1}", conversionParticipantList[num], nextResolvedParticipant);
						}
					}
				}
			}
		}

		protected abstract IADRecipientCache GetRecipientCache(int count);

		protected abstract bool CanResolveParticipant(Participant participant);

		protected abstract string TargetResolutionType { get; }

		private List<IConversionParticipantList> participantLists;

		protected bool disableLengthValidation;

		private readonly bool useSimpleDisplayName;

		protected bool ewsOutboundMimeConversion;

		internal enum ParticipantListId
		{
			Recipients,
			TnefRecipients,
			ReplyTo,
			ItemAddressProperties
		}

		protected class ParticipantResolutionList
		{
			internal ParticipantResolutionList()
			{
				this.originalParticipantList = new List<Participant>();
				this.resolvedParticipantList = new List<Participant>();
			}

			internal void AddParticipantForResolution(Participant participant)
			{
				this.originalParticipantList.Add(participant);
			}

			private void AddResolvedParticipant(Result<Participant> result)
			{
				this.resolvedParticipantList.Add(result.Data);
			}

			internal Participant.Job CreateResolutionJob()
			{
				Participant.Job job = new Participant.Job(this.originalParticipantList.Count);
				foreach (Participant input in this.originalParticipantList)
				{
					job.Add(new Participant.JobItem(input, new Action<Result<Participant>>(this.AddResolvedParticipant)));
				}
				return job;
			}

			internal Participant GetNextResolvedParticipant()
			{
				return this.resolvedParticipantList[this.resolvedIndex++];
			}

			private readonly List<Participant> originalParticipantList;

			private readonly List<Participant> resolvedParticipantList;

			private int resolvedIndex;
		}
	}
}
