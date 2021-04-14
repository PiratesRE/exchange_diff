using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class CharacterConstraint : CharacterRegexConstraint
	{
		public CharacterConstraint(char[] characters, bool valid) : base(CharacterConstraint.ConstructPattern(characters, valid))
		{
			StringBuilder stringBuilder = new StringBuilder(characters.Length * 5);
			for (int i = 0; i < characters.Length - 1; i++)
			{
				this.AppendVisibleString(characters[i], stringBuilder);
				stringBuilder.Append(", ");
			}
			this.AppendVisibleString(characters[characters.Length - 1], stringBuilder);
			this.characterString = stringBuilder.ToString();
			this.Characters = (char[])characters.Clone();
			this.showAsValid = valid;
		}

		public char[] Characters { get; private set; }

		public bool ShowAsValid
		{
			get
			{
				return this.showAsValid;
			}
		}

		internal static string ConstructPattern(char[] characters, bool valid)
		{
			if (characters == null)
			{
				throw new ArgumentNullException("characters");
			}
			if (characters.Length == 0)
			{
				throw new ArgumentOutOfRangeException("characters", "characters must contain at least one character");
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (char c in characters)
			{
				if (c == ']')
				{
					flag = true;
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			if (flag)
			{
				string value = stringBuilder.ToString();
				stringBuilder = new StringBuilder();
				stringBuilder.Append(']');
				stringBuilder.Append(value);
			}
			string text = Regex.Escape(stringBuilder.ToString());
			stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			if (!valid)
			{
				stringBuilder.Append('^');
			}
			if (text.Length > 0 && text[0] == ']')
			{
				stringBuilder.Append('\\');
			}
			stringBuilder.Append(text);
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		private void AppendVisibleString(char c, StringBuilder sb)
		{
			if (char.IsControl(c))
			{
				sb.AppendFormat("'0x{0:X2}'", (int)c);
				return;
			}
			sb.AppendFormat("'{0}'", c);
		}

		protected override LocalizedString CustomErrorMessage(string value, PropertyDefinition propertyDefinition)
		{
			if (this.ShowAsValid)
			{
				return DataStrings.ConstraintViolationStringContainsInvalidCharacters2(this.characterString, value);
			}
			return DataStrings.ConstraintViolationStringContainsInvalidCharacters(this.characterString, value);
		}

		private const char Escape = '\\';

		private const char RightBracket = ']';

		private string characterString;

		private bool showAsValid;
	}
}
