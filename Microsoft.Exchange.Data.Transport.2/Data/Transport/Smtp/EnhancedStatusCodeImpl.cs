using System;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	[Serializable]
	internal class EnhancedStatusCodeImpl
	{
		public EnhancedStatusCodeImpl(string code)
		{
			if (!EnhancedStatusCodeImpl.IsValid(code))
			{
				throw new FormatException(string.Format("This enhanced status code ({0}) isn't properly formatted. Enhanced status codes are in the form of x.y.z where the first digit must be 2, 4, or 5.", code));
			}
			this.code = code;
		}

		private EnhancedStatusCodeImpl()
		{
		}

		public string Value
		{
			get
			{
				return this.code;
			}
		}

		public static EnhancedStatusCodeImpl Parse(string code)
		{
			return new EnhancedStatusCodeImpl(code);
		}

		public static bool TryParse(string code, out EnhancedStatusCodeImpl enhancedStatusCode)
		{
			return EnhancedStatusCodeImpl.TryParse(code, 0, out enhancedStatusCode);
		}

		public static bool TryParse(string line, int startIndex, out EnhancedStatusCodeImpl enhancedStatusCode)
		{
			enhancedStatusCode = null;
			int length;
			if (!EnhancedStatusCodeImpl.CodeLength(line, startIndex, out length))
			{
				return false;
			}
			enhancedStatusCode = new EnhancedStatusCodeImpl();
			enhancedStatusCode.code = line.Substring(startIndex, length);
			return true;
		}

		public static bool IsValid(string status)
		{
			int num;
			return EnhancedStatusCodeImpl.CodeLength(status, 0, out num) && num == status.Length;
		}

		public static bool operator ==(EnhancedStatusCodeImpl val1, EnhancedStatusCodeImpl val2)
		{
			return object.ReferenceEquals(val1, val2) || (!object.ReferenceEquals(val1, null) && !object.ReferenceEquals(val2, null) && val1.code == val2.code);
		}

		public static bool operator !=(EnhancedStatusCodeImpl val1, EnhancedStatusCodeImpl val2)
		{
			return !(val1 == val2);
		}

		public override string ToString()
		{
			return this.code;
		}

		public override bool Equals(object obj)
		{
			EnhancedStatusCodeImpl enhancedStatusCodeImpl = obj as EnhancedStatusCodeImpl;
			return enhancedStatusCodeImpl != null && this == enhancedStatusCodeImpl;
		}

		public override int GetHashCode()
		{
			return this.code.GetHashCode();
		}

		private static bool CodeLength(string line, int startIndex, out int length)
		{
			length = -1;
			if (string.IsNullOrEmpty(line) || 0 > startIndex || line.Length - startIndex < 5)
			{
				return false;
			}
			switch (line[startIndex])
			{
			case '2':
			case '4':
			case '5':
			{
				int num = startIndex + 1;
				for (int num2 = 2; num2 != 0; num2--)
				{
					if (line.Length - num < 2 || line[num++] != '.')
					{
						return false;
					}
					int num3 = Math.Min(3, line.Length - num);
					int num4 = 0;
					while (num4 < num3 && char.IsDigit(line[num + num4]))
					{
						num4++;
					}
					if (num4 == 0)
					{
						return false;
					}
					num += num4;
				}
				if (num < line.Length && line[num] != ' ')
				{
					return false;
				}
				length = num - startIndex;
				return true;
			}
			default:
				return false;
			}
		}

		private string code;
	}
}
