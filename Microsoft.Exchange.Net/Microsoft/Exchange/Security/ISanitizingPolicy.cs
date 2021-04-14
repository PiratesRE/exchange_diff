using System;
using System.IO;

namespace Microsoft.Exchange.Security
{
	public interface ISanitizingPolicy
	{
		string Sanitize(string str);

		void Sanitize(TextWriter writer, string str);

		string SanitizeFormat(IFormatProvider formatProvider, string format, params object[] args);
	}
}
