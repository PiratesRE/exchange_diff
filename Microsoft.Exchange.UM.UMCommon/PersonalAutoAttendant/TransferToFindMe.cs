using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class TransferToFindMe : KeyMapping<FindMeNumbers>
	{
		internal TransferToFindMe(int key, string context, FindMeNumbers findmenumbers) : base(KeyMappingTypeEnum.FindMe, key, context, findmenumbers)
		{
		}

		internal FindMeNumbers Numbers
		{
			get
			{
				return base.Data;
			}
		}

		public override bool Validate(IDataValidator validator)
		{
			PAAValidationResult validationResult = PAAValidationResult.Valid;
			bool result = true;
			for (int i = 0; i < this.Numbers.Count; i++)
			{
				FindMe findMe = this.Numbers[i];
				IDataValidationResult dataValidationResult;
				if (!validator.ValidatePhoneNumberForOutdialing(findMe.Number, out dataValidationResult))
				{
					validationResult = dataValidationResult.PAAValidationResult;
					result = false;
				}
				findMe.PhoneNumber = dataValidationResult.PhoneNumber;
				findMe.ValidationResult = dataValidationResult.PAAValidationResult;
			}
			base.ValidationResult = validationResult;
			return result;
		}

		internal void AddFindMe(string number, int timeout)
		{
			this.AddFindMe(number, timeout, string.Empty);
		}

		internal void AddFindMe(string number, int timeout, string label)
		{
			FindMeNumbers data = base.Data;
			if (data.NumberList.Length >= 3)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot add more than {0} findme numbers", new object[]
				{
					3
				}));
			}
			data.Add(number, timeout, label);
		}

		private const int MaxFindMeNumbers = 3;
	}
}
