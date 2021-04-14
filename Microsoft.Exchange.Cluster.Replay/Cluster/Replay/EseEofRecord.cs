using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class EseEofRecord : EseLogRecord
	{
		internal static string Identifier
		{
			get
			{
				return "LTEL";
			}
		}

		public override string LogRecType
		{
			get
			{
				return EseEofRecord.Identifier;
			}
		}

		protected override Regex Regex()
		{
			return EseEofRecord.regex;
		}

		internal EseEofRecord(string input)
		{
			base.Match(input);
		}

		public override string ToString()
		{
			return "End of dump";
		}

		private static readonly Regex regex = new Regex(string.Format(CultureInfo.InvariantCulture, "^{0}\\s*$", new object[]
		{
			EseEofRecord.Identifier
		}), RegexOptions.CultureInvariant);
	}
}
