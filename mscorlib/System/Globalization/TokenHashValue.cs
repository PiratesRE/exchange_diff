using System;

namespace System.Globalization
{
	internal class TokenHashValue
	{
		internal TokenHashValue(string tokenString, TokenType tokenType, int tokenValue)
		{
			this.tokenString = tokenString;
			this.tokenType = tokenType;
			this.tokenValue = tokenValue;
		}

		internal string tokenString;

		internal TokenType tokenType;

		internal int tokenValue;
	}
}
