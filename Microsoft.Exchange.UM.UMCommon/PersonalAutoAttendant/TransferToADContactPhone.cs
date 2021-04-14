using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class TransferToADContactPhone : TransferToADContact, IPhoneNumberTarget
	{
		internal TransferToADContactPhone(int key, string context, string legacyExchangeDN) : base(KeyMappingTypeEnum.TransferToADContactPhone, key, context, legacyExchangeDN)
		{
		}

		public override bool Validate(IDataValidator validator)
		{
			IDataValidationResult dataValidationResult;
			bool result = validator.ValidateADContactForOutdialing(base.LegacyExchangeDN, out dataValidationResult);
			this.numberToDial = dataValidationResult.PhoneNumber;
			base.ValidationResult = dataValidationResult.PAAValidationResult;
			return result;
		}

		public PhoneNumber GetDialableNumber()
		{
			return this.numberToDial;
		}

		private PhoneNumber numberToDial;
	}
}
