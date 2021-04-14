using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class InvalidParticipantException : CorruptDataException
	{
		public InvalidParticipantException(LocalizedString message, ParticipantValidationStatus validationStatus) : this(message, validationStatus, null)
		{
		}

		public InvalidParticipantException(LocalizedString message, ParticipantValidationStatus validationStatus, Exception innerException) : base(InvalidParticipantException.CreateMessage(message, validationStatus), innerException)
		{
			EnumValidator.ThrowIfInvalid<ParticipantValidationStatus>(validationStatus, "validationStatus");
			this.validationStatus = validationStatus;
		}

		protected InvalidParticipantException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.validationStatus = (ParticipantValidationStatus)info.GetValue("validationStatus", typeof(ParticipantValidationStatus));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("validationStatus", this.validationStatus);
		}

		public ParticipantValidationStatus ValidationStatus
		{
			get
			{
				return this.validationStatus;
			}
		}

		private static LocalizedString CreateMessage(LocalizedString message, ParticipantValidationStatus status)
		{
			return ServerStrings.InvalidParticipant(message, status);
		}

		private const string ValidationStatusLabel = "validationStatus";

		private readonly ParticipantValidationStatus validationStatus;
	}
}
