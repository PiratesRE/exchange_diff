using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class CallerIdItem
	{
		public CallerIdItemType CallerIdType { get; set; }

		public string PhoneNumber { get; set; }

		public string GALContactLegacyDN { get; set; }

		public string PersonalContactStoreId { get; set; }

		public string PersonaEmailAddress { get; set; }

		public string DisplayName { get; set; }

		private CallerIdItem()
		{
		}

		public CallerIdItem(CallerIdItemType callerIdType, string phoneNumber, string galContactLegacyDN, string personalContactStoreId, string personaEmailAddress = null, string displayName = null)
		{
			this.CallerIdType = callerIdType;
			this.PhoneNumber = phoneNumber;
			this.GALContactLegacyDN = galContactLegacyDN;
			this.PersonalContactStoreId = personalContactStoreId;
			this.PersonaEmailAddress = personaEmailAddress;
			this.DisplayName = displayName;
			this.Validate();
		}

		public static CallerIdItem Parse(string callerIdItem)
		{
			if (string.IsNullOrEmpty(callerIdItem))
			{
				throw new FormatException(DataStrings.InvalidCallerIdItemFormat);
			}
			string[] array = callerIdItem.Split(new char[]
			{
				','
			});
			if (array == null || array.Length != 5)
			{
				throw new FormatException(DataStrings.InvalidCallerIdItemFormat);
			}
			int callerIdType = CallerIdItem.ValidateEnumValue(array[0], "CallerIdItemType", 1, 5);
			return new CallerIdItem((CallerIdItemType)callerIdType, array[1], array[2], array[3], array[4], null);
		}

		public static int ValidateEnumValue(string enumValue, string typeName, int min, int max)
		{
			if (enumValue == null)
			{
				throw new ArgumentNullException("enumValue");
			}
			if (string.IsNullOrEmpty(typeName))
			{
				throw new ArgumentException("typeName");
			}
			int num;
			if (!int.TryParse(enumValue, out num) || num < min || num > max)
			{
				throw new FormatException(DataStrings.ConstraintViolationEnumValueNotDefined(enumValue, typeName));
			}
			return num;
		}

		public override string ToString()
		{
			return string.Format("{0},{1},{2},{3},{4}", new object[]
			{
				(int)this.CallerIdType,
				this.PhoneNumber ?? string.Empty,
				this.GALContactLegacyDN ?? string.Empty,
				this.PersonalContactStoreId ?? string.Empty,
				this.PersonaEmailAddress ?? string.Empty
			});
		}

		public void Validate()
		{
			this.TrimAllStringMembers();
			switch (this.CallerIdType)
			{
			case CallerIdItemType.PhoneNumber:
				if (!string.IsNullOrEmpty(this.GALContactLegacyDN) || !string.IsNullOrEmpty(this.PersonalContactStoreId) || !string.IsNullOrEmpty(this.PersonaEmailAddress) || string.IsNullOrEmpty(this.PhoneNumber))
				{
					throw new FormatException(DataStrings.InvalidCallerIdItemTypePhoneNumber);
				}
				break;
			case CallerIdItemType.GALContact:
				if (!string.IsNullOrEmpty(this.PhoneNumber) || !string.IsNullOrEmpty(this.PersonalContactStoreId) || !string.IsNullOrEmpty(this.PersonaEmailAddress) || string.IsNullOrEmpty(this.GALContactLegacyDN))
				{
					throw new FormatException(DataStrings.InvalidCallerIdItemTypeGALContactr);
				}
				break;
			case CallerIdItemType.PersonalContact:
				if (!string.IsNullOrEmpty(this.GALContactLegacyDN) || !string.IsNullOrEmpty(this.PhoneNumber) || !string.IsNullOrEmpty(this.PersonaEmailAddress) || string.IsNullOrEmpty(this.PersonalContactStoreId))
				{
					throw new FormatException(DataStrings.InvalidCallerIdItemTypePersonalContact);
				}
				break;
			case CallerIdItemType.DefaultContactsFolder:
				if (!string.IsNullOrEmpty(this.GALContactLegacyDN) || !string.IsNullOrEmpty(this.PersonalContactStoreId) || !string.IsNullOrEmpty(this.PhoneNumber) || !string.IsNullOrEmpty(this.PersonaEmailAddress))
				{
					throw new FormatException(DataStrings.InvalidCallerIdItemTypeDefaultContactsFolder);
				}
				break;
			case CallerIdItemType.PersonaContact:
				if (!string.IsNullOrEmpty(this.GALContactLegacyDN) || !string.IsNullOrEmpty(this.PhoneNumber) || !string.IsNullOrEmpty(this.PersonalContactStoreId) || string.IsNullOrEmpty(this.PersonaEmailAddress))
				{
					throw new FormatException(DataStrings.InvalidCallerIdItemTypePersonaContact);
				}
				break;
			default:
				throw new Exception("Unkown Enumeration type.");
			}
		}

		private void TrimAllStringMembers()
		{
			this.GALContactLegacyDN = CustomMenuKeyMapping.TrimAndMapEmptyToNull(this.GALContactLegacyDN);
			this.PersonalContactStoreId = CustomMenuKeyMapping.TrimAndMapEmptyToNull(this.PersonalContactStoreId);
			this.PhoneNumber = CustomMenuKeyMapping.TrimAndMapEmptyToNull(this.PhoneNumber);
			this.PersonaEmailAddress = CustomMenuKeyMapping.TrimAndMapEmptyToNull(this.PersonaEmailAddress);
			this.DisplayName = CustomMenuKeyMapping.TrimAndMapEmptyToNull(this.DisplayName);
		}

		private const int RequiredNumberOfTokens = 5;
	}
}
