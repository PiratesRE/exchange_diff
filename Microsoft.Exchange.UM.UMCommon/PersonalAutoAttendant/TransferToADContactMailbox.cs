using System;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class TransferToADContactMailbox : TransferToADContact
	{
		internal TransferToADContactMailbox(int key, string context, string legacyExchangeDN) : base(KeyMappingTypeEnum.TransferToADContactMailbox, key, context, legacyExchangeDN)
		{
		}

		public override bool Validate(IDataValidator validator)
		{
			IDataValidationResult dataValidationResult;
			bool result = validator.ValidateADContactForTransferToMailbox(base.LegacyExchangeDN, out dataValidationResult);
			base.ValidationResult = dataValidationResult.PAAValidationResult;
			return result;
		}
	}
}
