using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class TransferToNumber : KeyMapping<string>, IPhoneNumberTarget
	{
		internal TransferToNumber(int key, string context, string number) : base(KeyMappingTypeEnum.TransferToNumber, key, context, number)
		{
		}

		internal string PhoneNumberString
		{
			get
			{
				return base.Data;
			}
		}

		public override bool Validate(IDataValidator validator)
		{
			IDataValidationResult dataValidationResult;
			bool result = validator.ValidatePhoneNumberForOutdialing(this.PhoneNumberString, out dataValidationResult);
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
