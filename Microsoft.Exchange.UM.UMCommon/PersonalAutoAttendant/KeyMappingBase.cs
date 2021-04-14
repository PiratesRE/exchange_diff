using System;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal abstract class KeyMappingBase : IDataItem
	{
		internal KeyMappingBase(KeyMappingTypeEnum type, int k, string context)
		{
			this.keyMappingType = type;
			this.key = k;
			this.context = context;
		}

		internal KeyMappingTypeEnum KeyMappingType
		{
			get
			{
				return this.keyMappingType;
			}
			set
			{
				this.keyMappingType = value;
			}
		}

		internal int Key
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		internal string Context
		{
			get
			{
				return this.context;
			}
			set
			{
				this.context = value;
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

		public abstract bool Validate(IDataValidator validator);

		internal static TransferToNumber CreateTransferToNumber(int key, string context, string number)
		{
			return new TransferToNumber(key, context, number);
		}

		internal static TransferToADContactMailbox CreateTransferToADContactMailbox(int key, string context, string legacyExchangeDN)
		{
			return new TransferToADContactMailbox(key, context, legacyExchangeDN);
		}

		internal static TransferToADContactPhone CreateTransferToADContactPhone(int key, string context, string legacyExchangeDN)
		{
			return new TransferToADContactPhone(key, context, legacyExchangeDN);
		}

		internal static TransferToFindMe CreateFindMe(int key, string context, string number, int timeout)
		{
			return new TransferToFindMe(key, context, new FindMeNumbers(number, timeout));
		}

		internal static TransferToFindMe CreateFindMe(int key, string context, string number, int timeout, string label)
		{
			return new TransferToFindMe(key, context, new FindMeNumbers(number, timeout, label));
		}

		internal static TransferToVoicemail CreateTransferToVoicemail(string context)
		{
			return new TransferToVoicemail(context);
		}

		private KeyMappingTypeEnum keyMappingType;

		private string context;

		private int key;

		private PAAValidationResult validationResult;
	}
}
