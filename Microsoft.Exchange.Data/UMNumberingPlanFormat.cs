using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class UMNumberingPlanFormat
	{
		private UMNumberingPlanFormat(string numberPlanFormat)
		{
			this.numberPlanFormat = numberPlanFormat;
		}

		public bool TryMapNumber(string number, out string mappedNumber)
		{
			mappedNumber = null;
			if (!string.IsNullOrEmpty(number))
			{
				char[] array = this.numberPlanFormat.ToCharArray();
				int num = number.Length - 1;
				int num2 = this.numberPlanFormat.Length - 1;
				while (num >= 0 && num2 >= 0)
				{
					if (number[num] != array[num2])
					{
						if (array[num2] != 'x')
						{
							return false;
						}
						array[num2] = number[num];
					}
					num--;
					num2--;
				}
				if (num < 0 && -1 == Array.IndexOf<char>(array, 'x'))
				{
					mappedNumber = new string(array);
				}
			}
			return null != mappedNumber;
		}

		public static UMNumberingPlanFormat Parse(string numberPlanFormat)
		{
			if (string.IsNullOrEmpty(numberPlanFormat) || !Regex.IsMatch(numberPlanFormat, "^\\+?[x\\d]+$"))
			{
				string message = DataStrings.ConstraintViolationStringDoesNotMatchRegularExpression(DataStrings.NumberingPlanPatternDescription, numberPlanFormat);
				throw new ArgumentException(message);
			}
			return new UMNumberingPlanFormat(numberPlanFormat);
		}

		public override string ToString()
		{
			return this.numberPlanFormat;
		}

		public const string RegexPattern = "^\\+?[x\\d]+$";

		public const int MaxLength = 128;

		private string numberPlanFormat;
	}
}
