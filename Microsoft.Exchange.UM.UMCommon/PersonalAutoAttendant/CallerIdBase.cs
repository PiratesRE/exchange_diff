using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal abstract class CallerIdBase : IDataItem
	{
		internal CallerIdBase(CallerIdTypeEnum type)
		{
			this.callerIdType = type;
		}

		internal CallerIdTypeEnum CallerIdType
		{
			get
			{
				return this.callerIdType;
			}
			set
			{
				this.callerIdType = value;
			}
		}

		internal PAAValidationResult ValidationResult
		{
			get
			{
				return this.validationResult;
			}
			set
			{
				this.validationResult = value;
			}
		}

		internal virtual int EvaluationCost
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public abstract bool Validate(IDataValidator validator);

		internal static ContactFolderCallerId CreateDefaultContactFolder()
		{
			return new ContactFolderCallerId();
		}

		internal static ContactItemCallerId CreateContactItem(StoreObjectId contactItem)
		{
			return new ContactItemCallerId(contactItem);
		}

		internal static PhoneNumberCallerId CreatePhoneNumber(string phoneNumber)
		{
			return new PhoneNumberCallerId(phoneNumber);
		}

		internal abstract bool Evaluate(IRuleEvaluator evaluator);

		private CallerIdTypeEnum callerIdType;

		private PAAValidationResult validationResult;
	}
}
