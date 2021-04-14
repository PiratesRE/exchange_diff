using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class KeyMapping
	{
		public KeyMappingType KeyMappingType { get; set; }

		public string Context { get; set; }

		public int Key { get; set; }

		public string FindMeFirstNumber { get; set; }

		public int FindMeFirstNumberDuration { get; set; }

		public string FindMeSecondNumber { get; set; }

		public int FindMeSecondNumberDuration { get; set; }

		public string TransferToNumber { get; set; }

		public string TransferToGALContactLegacyDN { get; set; }

		private KeyMapping()
		{
		}

		public KeyMapping(KeyMappingType keyMappingType, int key, string context, string findMeFirstNumber, int findMeFirstNumberDuration, string findMeSecondNumber, int findMeSecondNumberDuration, string transferToNumber, string transferToGALContactLegacyDN)
		{
			this.KeyMappingType = keyMappingType;
			this.Key = key;
			this.Context = context;
			this.FindMeFirstNumber = findMeFirstNumber;
			this.FindMeFirstNumberDuration = findMeFirstNumberDuration;
			this.FindMeSecondNumber = findMeSecondNumber;
			this.FindMeSecondNumberDuration = findMeSecondNumberDuration;
			this.TransferToNumber = transferToNumber;
			this.TransferToGALContactLegacyDN = transferToGALContactLegacyDN;
			this.Validate();
		}

		public override string ToString()
		{
			return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", new object[]
			{
				(int)this.KeyMappingType,
				this.Key,
				this.Context ?? string.Empty,
				this.FindMeFirstNumber ?? string.Empty,
				this.FindMeFirstNumberDuration,
				this.FindMeSecondNumber ?? string.Empty,
				this.FindMeSecondNumberDuration,
				this.TransferToNumber ?? string.Empty,
				this.TransferToGALContactLegacyDN ?? string.Empty
			});
		}

		public void Validate()
		{
			this.TrimAllStringMembers();
			switch (this.KeyMappingType)
			{
			case KeyMappingType.TransferToNumber:
				this.ValidateForTransferToNumber();
				return;
			case KeyMappingType.TransferToGALContact:
				this.ValidateForTransferToGALContact();
				return;
			case KeyMappingType.TransferToGALContactVoiceMail:
				this.ValidateForTransferToGALContactVoiceMail();
				return;
			case KeyMappingType.VoiceMail:
				this.ValidateForVoiceMail();
				return;
			case KeyMappingType.FindMe:
				this.ValidateForFindMe();
				return;
			default:
				throw new Exception("Unknown KeyMappingType enum value");
			}
		}

		public static KeyMapping Parse(string keyMappingValue)
		{
			if (string.IsNullOrEmpty(keyMappingValue))
			{
				throw new FormatException(DataStrings.InvalidKeyMappingFormat);
			}
			string[] array = keyMappingValue.Split(new char[]
			{
				','
			});
			if (array == null || array.Length != 9)
			{
				throw new FormatException(DataStrings.InvalidKeyMappingFormat);
			}
			KeyMappingType keyMappingType = (KeyMappingType)CallerIdItem.ValidateEnumValue(array[0], "KeyMappingType", 1, 5);
			if (string.IsNullOrEmpty(array[1]))
			{
				throw new FormatException(DataStrings.InvalidKeyMappingKey);
			}
			int key = int.Parse(array[1]);
			int findMeFirstNumberDuration = 0;
			if (!string.IsNullOrEmpty(array[4]))
			{
				findMeFirstNumberDuration = int.Parse(array[4]);
			}
			int findMeSecondNumberDuration = 0;
			if (!string.IsNullOrEmpty(array[6]))
			{
				findMeSecondNumberDuration = int.Parse(array[6]);
			}
			return new KeyMapping(keyMappingType, key, array[2], array[3], findMeFirstNumberDuration, array[5], findMeSecondNumberDuration, array[7], array[8]);
		}

		private void TrimAllStringMembers()
		{
			this.Context = CustomMenuKeyMapping.TrimAndMapEmptyToNull(this.Context);
			this.FindMeFirstNumber = CustomMenuKeyMapping.TrimAndMapEmptyToNull(this.FindMeFirstNumber);
			this.FindMeSecondNumber = CustomMenuKeyMapping.TrimAndMapEmptyToNull(this.FindMeSecondNumber);
			this.TransferToNumber = CustomMenuKeyMapping.TrimAndMapEmptyToNull(this.TransferToNumber);
			this.TransferToGALContactLegacyDN = CustomMenuKeyMapping.TrimAndMapEmptyToNull(this.TransferToGALContactLegacyDN);
		}

		private void ValidateForVoiceMail()
		{
			if (this.Key != 10 || !string.IsNullOrEmpty(this.TransferToNumber) || !string.IsNullOrEmpty(this.FindMeFirstNumber) || !string.IsNullOrEmpty(this.FindMeSecondNumber) || !string.IsNullOrEmpty(this.Context) || !string.IsNullOrEmpty(this.TransferToGALContactLegacyDN))
			{
				throw new FormatException(DataStrings.InvalidKeyMappingVoiceMail);
			}
		}

		private void ValidateForFindMe()
		{
			this.ValidateKey();
			this.ValidateContext();
			if (!string.IsNullOrEmpty(this.TransferToNumber) || string.IsNullOrEmpty(this.FindMeFirstNumber) || !string.IsNullOrEmpty(this.TransferToGALContactLegacyDN))
			{
				throw new FormatException(DataStrings.InvalidKeyMappingFindMe);
			}
			this.ValidateFindMeDuration();
		}

		private void ValidateForTransferToGALContactVoiceMail()
		{
			this.ValidateForTransferToGALContact();
		}

		private void ValidateForTransferToGALContact()
		{
			this.ValidateKey();
			this.ValidateContext();
			if (!string.IsNullOrEmpty(this.TransferToNumber) || !string.IsNullOrEmpty(this.FindMeFirstNumber) || !string.IsNullOrEmpty(this.FindMeSecondNumber) || string.IsNullOrEmpty(this.TransferToGALContactLegacyDN))
			{
				throw new FormatException(DataStrings.InvalidKeyMappingTransferToGalContact);
			}
		}

		private void ValidateForTransferToNumber()
		{
			this.ValidateKey();
			this.ValidateContext();
			if (string.IsNullOrEmpty(this.TransferToNumber) || !string.IsNullOrEmpty(this.FindMeFirstNumber) || !string.IsNullOrEmpty(this.FindMeSecondNumber) || !string.IsNullOrEmpty(this.TransferToGALContactLegacyDN))
			{
				throw new FormatException(DataStrings.InvalidKeyMappingTransferToNumber);
			}
		}

		private void ValidateKey()
		{
			if (this.Key < 1 || this.Key > 9)
			{
				throw new FormatException(DataStrings.InvalidKeyMappingKey);
			}
		}

		private void ValidateContext()
		{
			if (!string.IsNullOrEmpty(this.Context) && this.Context.Length > 80)
			{
				throw new FormatException(DataStrings.InvalidKeyMappingContext);
			}
		}

		private void ValidateFindMeDuration()
		{
			if (this.FindMeFirstNumberDuration < 20 || this.FindMeFirstNumberDuration > 99)
			{
				throw new FormatException(DataStrings.InvalidKeyMappingFindMeFirstNumberDuration);
			}
			if ((string.IsNullOrEmpty(this.FindMeSecondNumber) && this.FindMeSecondNumberDuration != 0) || (!string.IsNullOrEmpty(this.FindMeSecondNumber) && (this.FindMeSecondNumberDuration < 20 || this.FindMeSecondNumberDuration > 99)))
			{
				throw new FormatException(DataStrings.InvalidKeyMappingFindMeSecondNumber);
			}
		}

		private const int RequiredNumberOfTokens = 9;

		private const int MinFindMeDuration = 20;

		private const int MaxFindMeDuration = 99;

		private const int MaxContextLength = 80;

		private const int MinKey = 1;

		private const int MaxKey = 9;

		private const int KeyForVoiceMailKeyType = 10;

		private enum KeyMappingStringToken
		{
			Type,
			Key,
			Context,
			FindMeFirstNumber,
			FindMeFirstNumberDuration,
			FindMeSecondNumber,
			FindMeSecondNumberDuration,
			TransferToNumber,
			TransferToGALContactLegacyDN
		}
	}
}
