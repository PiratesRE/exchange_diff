using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class WindowsLiveIDLocalPartConstraint : PropertyDefinitionConstraint
	{
		public WindowsLiveIDLocalPartConstraint(bool allowEmptyLocalPart)
		{
			this.allowEmptyLocalPart = allowEmptyLocalPart;
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value != null)
			{
				string text = value.ToString();
				if (this.allowEmptyLocalPart && string.IsNullOrEmpty(text))
				{
					return null;
				}
				if (!WindowsLiveIDLocalPartConstraint.IsValidLocalPartOfWindowsLiveID(text))
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationInvalidWindowsLiveIDLocalPart, propertyDefinition, value, this);
				}
			}
			return null;
		}

		public static bool IsValidLocalPartOfWindowsLiveID(string liveID)
		{
			if (string.IsNullOrEmpty(liveID) || !char.IsLetter(liveID[0]) || liveID[liveID.Length - 1] == '.' || liveID.IndexOf("..") >= 0 || liveID.Length > 63)
			{
				return false;
			}
			foreach (char c in liveID)
			{
				if (!WindowsLiveIDLocalPartConstraint.IsValidCharForWindowsLiveID(c))
				{
					return false;
				}
			}
			return true;
		}

		public static string RemoveInvalidPartOfWindowsLiveID(string liveID)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(liveID))
			{
				foreach (char c in liveID)
				{
					if (stringBuilder.Length >= 63)
					{
						break;
					}
					if (WindowsLiveIDLocalPartConstraint.IsValidCharForWindowsLiveID(c) && (stringBuilder.Length != 0 || char.IsLetter(c)) && (stringBuilder.Length <= 0 || c != '.' || c != stringBuilder[stringBuilder.Length - 1]))
					{
						stringBuilder.Append(c);
					}
				}
				if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == '.')
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
				}
			}
			return stringBuilder.ToString();
		}

		private static bool IsValidCharForWindowsLiveID(char c)
		{
			return c <= 'ÿ' && (char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '_');
		}

		public const int MaxLengthOfLiveID = 63;

		private readonly bool allowEmptyLocalPart;
	}
}
