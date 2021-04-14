using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class LogSchema
	{
		public LogSchema(string software, string version, string logType, string[] fields)
		{
			this.software = software;
			this.version = version;
			this.logType = logType;
			this.fields = fields;
		}

		internal string Software
		{
			get
			{
				return this.software;
			}
		}

		internal string Version
		{
			get
			{
				return this.version;
			}
		}

		internal string LogType
		{
			get
			{
				return this.logType;
			}
		}

		internal string[] Fields
		{
			get
			{
				return this.fields;
			}
		}

		private string software;

		private string version;

		private string logType;

		private string[] fields;
	}
}
