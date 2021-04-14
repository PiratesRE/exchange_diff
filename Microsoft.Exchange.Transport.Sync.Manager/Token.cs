using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct Token : IEquatable<Token>
	{
		internal Token(Guid value)
		{
			this.value = value;
		}

		public static bool operator ==(Token x, Token y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(Token x, Token y)
		{
			return !x.Equals(y);
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public override bool Equals(object o)
		{
			if (!(o is Token))
			{
				return false;
			}
			Token token = (Token)o;
			return this.Equals(token);
		}

		public bool Equals(Token token)
		{
			return object.Equals(this.value, token.value);
		}

		public override string ToString()
		{
			return this.value.ToString();
		}

		private Guid value;
	}
}
