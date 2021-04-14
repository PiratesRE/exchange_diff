using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal abstract class ParticipantConverter<TStorageType, TParticipantWrapper, TRecipient> : IConverter<TStorageType, TRecipient> where TParticipantWrapper : ParticipantWrapper<TStorageType> where TRecipient : class, IRecipient, ISchematizedObject<RecipientSchema>, new()
	{
		protected ParticipantConverter(IParticipantRoutingTypeConverter routingTypeConverter)
		{
			this.routingTypeConverter = routingTypeConverter.AssertNotNull("routingTypeConverter");
		}

		protected virtual IParticipantRoutingTypeConverter RoutingTypeConverter
		{
			get
			{
				return this.routingTypeConverter;
			}
		}

		public TRecipient Convert(TStorageType value)
		{
			return this.Convert(new TStorageType[]
			{
				value
			}).First<TRecipient>();
		}

		public Participant Convert(TRecipient value)
		{
			if (value == null)
			{
				throw new ExArgumentNullException("value");
			}
			return new Participant(value.Name, value.EmailAddress, this.GetRoutingType(value));
		}

		public virtual IEnumerable<TRecipient> Convert(IEnumerable<TStorageType> wrappables)
		{
			if (wrappables == null)
			{
				return null;
			}
			TParticipantWrapper[] wrappedParticipants = (from storageObject in wrappables
			select this.WrapStorageType(storageObject)).ToArray<TParticipantWrapper>();
			Participant[] value = (from wrapper in wrappedParticipants
			select wrapper.Participant).ToArray<Participant>();
			IEnumerable<Participant> source = this.routingTypeConverter.ConvertToEntity(value);
			int index = -1;
			return from participant in source
			select this.CopyFromParticipant(participant, wrappedParticipants[++index].Original);
		}

		protected virtual TRecipient CopyFromParticipant(Participant value, TStorageType original)
		{
			if (value == null)
			{
				return default(TRecipient);
			}
			TRecipient result = Activator.CreateInstance<TRecipient>();
			result.EmailAddress = value.EmailAddress;
			result.Name = value.DisplayName;
			result.RoutingType = value.RoutingType;
			return result;
		}

		protected string GetRoutingType(TRecipient recipient)
		{
			return recipient.IsPropertySet(recipient.Schema.RoutingTypeProperty) ? recipient.RoutingType : "SMTP";
		}

		protected abstract TParticipantWrapper WrapStorageType(TStorageType value);

		private readonly IParticipantRoutingTypeConverter routingTypeConverter;
	}
}
