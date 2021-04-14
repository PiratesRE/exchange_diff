using System;

namespace Microsoft.Exchange.TextMatching
{
	internal sealed class RegexCharacterClass
	{
		public RegexCharacterClass(char ch)
		{
			this.ch = ch;
			this.type = RegexCharacterClass.ValueType.Character;
		}

		public RegexCharacterClass(RegexCharacterClass.ValueType type)
		{
			this.type = type;
		}

		public RegexCharacterClass.ValueType Type
		{
			get
			{
				return this.type;
			}
		}

		public override int GetHashCode()
		{
			int result;
			switch (this.type)
			{
			case RegexCharacterClass.ValueType.BeginCharacterClass:
				result = 568260;
				break;
			case RegexCharacterClass.ValueType.EndCharacterClass:
				result = 568261;
				break;
			case RegexCharacterClass.ValueType.SpaceCharacterClass:
				result = 32;
				break;
			case RegexCharacterClass.ValueType.NonSpaceCharacterClass:
				result = 568257;
				break;
			case RegexCharacterClass.ValueType.NonDigitCharacterClass:
				result = 568258;
				break;
			case RegexCharacterClass.ValueType.DigitCharacterClass:
				result = 568256;
				break;
			case RegexCharacterClass.ValueType.WordCharacterClass:
				result = 568259;
				break;
			case RegexCharacterClass.ValueType.NonWordCharacterClass:
				result = 568262;
				break;
			default:
				result = (int)this.ch;
				break;
			}
			return result;
		}

		public override bool Equals(object obj)
		{
			RegexCharacterClass regexCharacterClass = obj as RegexCharacterClass;
			return regexCharacterClass != null && this.GetHashCode() == regexCharacterClass.GetHashCode();
		}

		public override string ToString()
		{
			string result;
			switch (this.type)
			{
			case RegexCharacterClass.ValueType.BeginCharacterClass:
				result = "^";
				break;
			case RegexCharacterClass.ValueType.EndCharacterClass:
				result = "$";
				break;
			case RegexCharacterClass.ValueType.SpaceCharacterClass:
				result = "\\s";
				break;
			case RegexCharacterClass.ValueType.NonSpaceCharacterClass:
				result = "\\S";
				break;
			case RegexCharacterClass.ValueType.NonDigitCharacterClass:
				result = "\\D";
				break;
			case RegexCharacterClass.ValueType.DigitCharacterClass:
				result = "\\d";
				break;
			case RegexCharacterClass.ValueType.WordCharacterClass:
				result = "\\w";
				break;
			case RegexCharacterClass.ValueType.NonWordCharacterClass:
				result = "\\W";
				break;
			default:
				result = char.ToString(this.ch);
				break;
			}
			return result;
		}

		private char ch;

		private RegexCharacterClass.ValueType type;

		internal enum ValueType
		{
			Character,
			BeginCharacterClass,
			EndCharacterClass,
			SpaceCharacterClass,
			NonSpaceCharacterClass,
			NonDigitCharacterClass,
			DigitCharacterClass,
			WordCharacterClass,
			NonWordCharacterClass
		}
	}
}
