using System;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal sealed class ParameterNames
	{
		private ParameterNames()
		{
			throw new NotSupportedException();
		}

		public const string Boundary = "boundary";

		public const string Charset = "charset";

		public const string FileName = "filename";

		public const string Micalg = "micalg";

		public const string Name = "name";

		public const string Protocol = "protocol";

		public const string Range = "range";

		public const string SMimeType = "smime-type";

		public const string Start = "start";

		public const string Url = "url";

		public const string Version = "version";

		public const string ReportType = "report-type";
	}
}
