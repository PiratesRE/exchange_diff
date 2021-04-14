using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class EnhancedStatusCode
	{
		public EnhancedStatusCode(string code)
		{
			this.enhancedStatusCodeImpl = new EnhancedStatusCodeImpl(code);
		}

		private EnhancedStatusCode(EnhancedStatusCodeImpl escImpl)
		{
			this.enhancedStatusCodeImpl = escImpl;
		}

		public string Value
		{
			get
			{
				return this.enhancedStatusCodeImpl.Value;
			}
		}

		public static EnhancedStatusCode Parse(string code)
		{
			return new EnhancedStatusCode(code);
		}

		public static bool TryParse(string code, out EnhancedStatusCode enhancedStatusCode)
		{
			return EnhancedStatusCode.TryParse(code, 0, out enhancedStatusCode);
		}

		public static bool TryParse(string line, int startIndex, out EnhancedStatusCode enhancedStatusCode)
		{
			EnhancedStatusCodeImpl escImpl;
			bool flag = EnhancedStatusCodeImpl.TryParse(line, startIndex, out escImpl);
			enhancedStatusCode = null;
			if (flag)
			{
				enhancedStatusCode = new EnhancedStatusCode(escImpl);
			}
			return flag;
		}

		public static bool IsValid(string status)
		{
			return EnhancedStatusCodeImpl.IsValid(status);
		}

		public static bool operator ==(EnhancedStatusCode val1, EnhancedStatusCode val2)
		{
			return object.ReferenceEquals(val1, val2) || (!object.ReferenceEquals(val1, null) && !object.ReferenceEquals(val2, null) && val1.Value == val2.Value);
		}

		public static bool operator !=(EnhancedStatusCode val1, EnhancedStatusCode val2)
		{
			return !(val1 == val2);
		}

		public override string ToString()
		{
			return this.enhancedStatusCodeImpl.Value;
		}

		public override bool Equals(object other)
		{
			EnhancedStatusCode enhancedStatusCode = other as EnhancedStatusCode;
			return enhancedStatusCode != null && this == enhancedStatusCode;
		}

		public override int GetHashCode()
		{
			return this.enhancedStatusCodeImpl.Value.GetHashCode();
		}

		private EnhancedStatusCodeImpl enhancedStatusCodeImpl;
	}
}
