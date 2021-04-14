using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class CustomMenuKeyMapping : IComparable, IEquatable<CustomMenuKeyMapping>
	{
		[XmlIgnore]
		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public CustomMenuKey MappedKey
		{
			get
			{
				return this.mappedKey;
			}
			set
			{
				this.mappedKey = value;
				this.key = CustomMenuKeyMapping.MapKeyToString(this.mappedKey);
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
				CustomMenuKeyMapping.validateDescription(value);
			}
		}

		public string Extension
		{
			get
			{
				return this.extension;
			}
			set
			{
				this.extension = value;
				CustomMenuKeyMapping.validateExtension(value);
			}
		}

		public string AutoAttendantName
		{
			get
			{
				return this.autoAttendantName;
			}
			set
			{
				this.autoAttendantName = value;
			}
		}

		public string LeaveVoicemailFor
		{
			get
			{
				return this.leaveVoicemailFor;
			}
			set
			{
				this.leaveVoicemailFor = value;
			}
		}

		public string LegacyDNToUseForLeaveVoicemailFor
		{
			get
			{
				return this.legacyDNToUseForLeaveVoicemailFor;
			}
			set
			{
				this.legacyDNToUseForLeaveVoicemailFor = value;
			}
		}

		public string TransferToMailbox
		{
			get
			{
				return this.transferToMailbox;
			}
			set
			{
				this.transferToMailbox = value;
			}
		}

		public string LegacyDNToUseForTransferToMailbox
		{
			get
			{
				return this.legacyDNToUseForTransferToMailbox;
			}
			set
			{
				this.legacyDNToUseForTransferToMailbox = value;
			}
		}

		public string PromptFileName
		{
			get
			{
				return this.promptFileName;
			}
			set
			{
				this.promptFileName = value;
				CustomMenuKeyMapping.ValidatePromptFileName("PromptFileName", value);
			}
		}

		public string AsrPhrases
		{
			get
			{
				return this.asrPhrases;
			}
			set
			{
				this.asrPhrases = CustomMenuKeyMapping.TrimAndMapEmptyToNull(value);
			}
		}

		[XmlIgnore]
		public string[] AsrPhraseList
		{
			get
			{
				string[] result = null;
				if (!string.IsNullOrEmpty(this.asrPhrases))
				{
					result = this.asrPhrases.Split(new char[]
					{
						';'
					});
				}
				return result;
			}
		}

		public string AnnounceBusinessLocation
		{
			get
			{
				return this.announceBusinessLocation;
			}
			set
			{
				CustomMenuKeyMapping.ValidateFlag(CustomMenuKeyMapping.TrimAndMapEmptyToNull(value), "AnnounceBusinessLocation");
				this.announceBusinessLocation = value;
			}
		}

		public string AnnounceBusinessHours
		{
			get
			{
				return this.announceBusinessHours;
			}
			set
			{
				CustomMenuKeyMapping.ValidateFlag(CustomMenuKeyMapping.TrimAndMapEmptyToNull(value), "AnnounceBusinessHours");
				this.announceBusinessHours = value;
			}
		}

		public CustomMenuKeyMapping()
		{
		}

		public CustomMenuKeyMapping(string key, string name, string extension, string autoAttendant, string promptFileName) : this(key, name, extension, autoAttendant, promptFileName, null)
		{
		}

		public CustomMenuKeyMapping(string key, string name, string extension, string autoAttendant, string promptFileName, string asrPhrases) : this(key, name, extension, autoAttendant, promptFileName, asrPhrases, null, null, null, null)
		{
		}

		public CustomMenuKeyMapping(PSObject importedObject) : this(CustomMenuKeyMapping.GetObjectProperty(importedObject, "Key"), CustomMenuKeyMapping.GetObjectProperty(importedObject, "Description"), CustomMenuKeyMapping.GetObjectProperty(importedObject, "Extension"), CustomMenuKeyMapping.GetObjectProperty(importedObject, "AutoAttendantName"), CustomMenuKeyMapping.GetObjectProperty(importedObject, "PromptFileName"), CustomMenuKeyMapping.GetObjectProperty(importedObject, "AsrPhrases"), CustomMenuKeyMapping.GetObjectProperty(importedObject, "LeaveVoicemailFor"), CustomMenuKeyMapping.GetObjectProperty(importedObject, "TransferToMailbox"), CustomMenuKeyMapping.GetObjectProperty(importedObject, "AnnounceBusinessLocation"), CustomMenuKeyMapping.GetObjectProperty(importedObject, "AnnounceBusinessHours"))
		{
		}

		public CustomMenuKeyMapping(string key, string name, string extension, string autoAttendant, string promptFileName, string asrPhrases, string leaveVoicemailFor, string transferToMailbox) : this(key, name, extension, autoAttendant, promptFileName, asrPhrases, leaveVoicemailFor, transferToMailbox, null, null)
		{
		}

		public CustomMenuKeyMapping(string key, string name, string extension, string autoAttendant, string promptFileName, string asrPhrases, string leaveVoicemailFor, string transferToMailbox, string announceBusinessLocation, string announceBusinessHours)
		{
			this.key = key;
			this.description = CustomMenuKeyMapping.TrimAndMapEmptyToNull(name);
			this.extension = CustomMenuKeyMapping.TrimAndMapEmptyToNull(extension);
			this.autoAttendantName = CustomMenuKeyMapping.TrimAndMapEmptyToNull(autoAttendant);
			this.promptFileName = CustomMenuKeyMapping.TrimAndMapEmptyToNull(promptFileName);
			this.asrPhrases = CustomMenuKeyMapping.TrimAndMapEmptyToNull(asrPhrases);
			this.leaveVoicemailFor = CustomMenuKeyMapping.TrimAndMapEmptyToNull(leaveVoicemailFor);
			this.transferToMailbox = CustomMenuKeyMapping.TrimAndMapEmptyToNull(transferToMailbox);
			this.announceBusinessLocation = CustomMenuKeyMapping.TrimAndMapEmptyToNull(announceBusinessLocation);
			this.announceBusinessHours = CustomMenuKeyMapping.TrimAndMapEmptyToNull(announceBusinessHours);
			CustomMenuKeyMapping.validateDescription(this.description);
			this.mappedKey = CustomMenuKeyMapping.MapStringToKey(this.key);
			CustomMenuKeyMapping.ValidateFlag(this.announceBusinessLocation, "AnnounceBusinessLocation");
			CustomMenuKeyMapping.ValidateFlag(this.announceBusinessHours, "AnnounceBusinessHours");
			CustomMenuKeyMapping.validateExtension(this.extension);
			CustomMenuKeyMapping.ValidatePromptFileName("PromptFileName", this.promptFileName);
			this.Validate();
		}

		private static bool IsNumberofTokensValid(int number)
		{
			return number == 5 || number == 6 || number == 8 || number == 10;
		}

		private static string GetObjectProperty(PSObject importedObject, string propertyName)
		{
			if (importedObject.Properties.Match(propertyName).Count == 0)
			{
				return string.Empty;
			}
			return (string)importedObject.Properties[propertyName].Value;
		}

		public static CustomMenuKeyMapping Parse(string customExtension)
		{
			if (customExtension == null)
			{
				throw new ArgumentNullException(customExtension);
			}
			string[] array = customExtension.Split(new char[]
			{
				','
			});
			if (array == null || !CustomMenuKeyMapping.IsNumberofTokensValid(array.Length))
			{
				throw new ArgumentException(DataStrings.KeyMappingInvalidArgument);
			}
			string text = (array.Length < 6) ? null : array[5];
			string text2 = (array.Length < 7) ? null : array[6];
			string text3 = (array.Length < 8) ? null : array[7];
			string text4 = (array.Length < 9) ? null : array[8];
			string text5 = (array.Length < 10) ? null : array[9];
			return new CustomMenuKeyMapping(array[0], array[1], array[2], array[3], array[4], text, text2, text3, text4, text5);
		}

		public void Validate()
		{
			switch (this.AreMultipleOptionsSet(new List<string>
			{
				this.autoAttendantName,
				this.extension,
				this.transferToMailbox,
				this.leaveVoicemailFor,
				this.announceBusinessLocation,
				this.announceBusinessHours
			}))
			{
			case CustomMenuKeyMapping.OptionSpecified.None:
				if (this.PromptFileName == null)
				{
					throw new FormatException(DataStrings.KeyMappingInvalidArgument);
				}
				break;
			case CustomMenuKeyMapping.OptionSpecified.Single:
				break;
			case CustomMenuKeyMapping.OptionSpecified.Multiple:
				throw new FormatException(DataStrings.InvalidCustomMenuKeyMappingA);
			default:
				return;
			}
		}

		private CustomMenuKeyMapping.OptionSpecified AreMultipleOptionsSet(List<string> options)
		{
			int num = 0;
			foreach (string text in options)
			{
				if (text != null)
				{
					num++;
					if (num == 2)
					{
						return CustomMenuKeyMapping.OptionSpecified.Multiple;
					}
				}
			}
			if (num != 0)
			{
				return CustomMenuKeyMapping.OptionSpecified.Single;
			}
			return CustomMenuKeyMapping.OptionSpecified.None;
		}

		private static void validateDescription(string description)
		{
			if (string.IsNullOrEmpty(description))
			{
				throw new ArgumentNullException("Description");
			}
			if (description.IndexOf(",") != -1)
			{
				throw new StrongTypeFormatException(DataStrings.InvalidCharInString("Description", ","), "Description");
			}
		}

		private static void validateExtension(string extension)
		{
			if (!string.IsNullOrEmpty(extension) && !CustomMenuKeyMapping.IsNumeric(extension))
			{
				throw new StrongTypeFormatException(DataStrings.InvalidNumber(extension, "Extension"), "Extension");
			}
		}

		public static void ValidatePromptFileName(string propertyName, string fileName)
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				if (fileName.Length > 255)
				{
					throw new StrongTypeFormatException(DataStrings.ConstraintViolationStringLengthTooLong(255, fileName.Length), "PromptFileName");
				}
				Regex regex = new Regex("^$|\\.wav|\\.wma$", RegexOptions.IgnoreCase);
				if (!regex.IsMatch(fileName))
				{
					string pattern = DataStrings.CustomGreetingFilePatternDescription.ToString();
					throw new StrongTypeFormatException(DataStrings.ConstraintViolationStringDoesNotMatchRegularExpression(pattern, fileName).ToString(), "PromptFileName");
				}
			}
		}

		public static string TrimAndMapEmptyToNull(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return null;
			}
			s = s.Trim();
			if (s.Length == 0)
			{
				return null;
			}
			return s;
		}

		private static bool IsNumeric(string digits)
		{
			for (int i = 0; i < digits.Length; i++)
			{
				if (!char.IsDigit(digits[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static void ValidateCustomMenuKey(string keyString, out int key)
		{
			key = -1;
			int num = -1;
			if (!int.TryParse(keyString, out num))
			{
				throw new StrongTypeFormatException(DataStrings.InvalidKeySelectionA, "Key");
			}
			if (num == 0)
			{
				throw new StrongTypeFormatException(DataStrings.InvalidKeySelection_Zero, "Key");
			}
			if (num < 0 || num > 9)
			{
				throw new StrongTypeFormatException(DataStrings.InvalidKeySelectionA, "Key");
			}
			key = num;
		}

		private static void ValidateFlag(string flagString, string flagName)
		{
			if (!string.IsNullOrEmpty(flagString) && !string.Equals(flagString, "1", StringComparison.OrdinalIgnoreCase))
			{
				throw new StrongTypeFormatException(DataStrings.InvalidFlagValue, flagName);
			}
		}

		public override string ToString()
		{
			return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", new object[]
			{
				CustomMenuKeyMapping.MapKeyToString(this.mappedKey),
				this.description,
				this.extension,
				this.autoAttendantName,
				this.promptFileName,
				this.asrPhrases,
				this.leaveVoicemailFor,
				this.transferToMailbox,
				this.announceBusinessLocation,
				this.announceBusinessHours
			});
		}

		private static string MapKeyToString(CustomMenuKey key)
		{
			if (key == CustomMenuKey.Timeout)
			{
				return "-";
			}
			if (key != CustomMenuKey.InvalidKey)
			{
				return Convert.ToString((int)key);
			}
			return null;
		}

		private static CustomMenuKey MapStringToKey(string keyString)
		{
			string text = CustomMenuKeyMapping.TrimAndMapEmptyToNull(keyString);
			if (text == null)
			{
				throw new StrongTypeFormatException(DataStrings.InvalidKeySelectionA, "Key");
			}
			if (string.Compare(text, "-", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return CustomMenuKey.Timeout;
			}
			int result = -1;
			CustomMenuKeyMapping.ValidateCustomMenuKey(text, out result);
			return (CustomMenuKey)result;
		}

		public int CompareTo(object obj)
		{
			int result;
			if (obj == null)
			{
				result = 1;
			}
			else
			{
				CustomMenuKeyMapping customMenuKeyMapping = obj as CustomMenuKeyMapping;
				if (customMenuKeyMapping == null)
				{
					result = -1;
				}
				else if (customMenuKeyMapping.MappedKey == CustomMenuKey.Timeout)
				{
					result = -1;
				}
				else if (this.MappedKey == CustomMenuKey.Timeout)
				{
					result = 1;
				}
				else if (this.MappedKey == CustomMenuKey.NotSpecified && this.MappedKey == customMenuKeyMapping.MappedKey)
				{
					result = string.CompareOrdinal(this.Description, customMenuKeyMapping.Description);
				}
				else
				{
					result = this.MappedKey - customMenuKeyMapping.MappedKey;
				}
			}
			return result;
		}

		public override bool Equals(object obj)
		{
			CustomMenuKeyMapping customMenuKeyMapping = obj as CustomMenuKeyMapping;
			return customMenuKeyMapping != null && this.Equals(customMenuKeyMapping);
		}

		public bool Equals(CustomMenuKeyMapping comparand)
		{
			return comparand != null && string.Equals(this.ToString(), comparand.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		internal const string CustomGreetingFileRegEx = "^$|\\.wav|\\.wma$";

		internal const int FileNameMaxLength = 255;

		private string key;

		private CustomMenuKey mappedKey;

		private string description;

		private string extension;

		private string autoAttendantName;

		private string promptFileName;

		private string asrPhrases;

		private string leaveVoicemailFor;

		private string transferToMailbox;

		private string legacyDNToUseForLeaveVoicemailFor;

		private string legacyDNToUseForTransferToMailbox;

		private string announceBusinessLocation;

		private string announceBusinessHours;

		private enum OptionSpecified
		{
			None,
			Single,
			Multiple
		}

		private enum KeyMappingTokens
		{
			Invalid = -1,
			Key,
			Name,
			Extension,
			AutoAttendant,
			PromptFileName,
			AsrPhrases,
			LeaveVoicemailFor,
			TransferToMailbox,
			AnnounceBusinessLocation,
			AnnounceBusinessHours,
			Count
		}
	}
}
