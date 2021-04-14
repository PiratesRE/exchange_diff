using System;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MailUtilities
	{
		public static DateTime ToDateTime(string dateTimeString)
		{
			return new DateHeader("<empty>", DateTime.UtcNow)
			{
				Value = dateTimeString
			}.UtcDateTime;
		}

		private const string EmptyDateHeader = "<empty>";
	}
}
