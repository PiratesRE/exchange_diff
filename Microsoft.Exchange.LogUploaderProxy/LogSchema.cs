using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public class LogSchema
	{
		public LogSchema(string software, string version, string logType, string[] fields)
		{
			this.logSchemaImpl = new LogSchema(software, version, logType, fields);
		}

		internal LogSchema LogSchemaImpl
		{
			get
			{
				return this.logSchemaImpl;
			}
		}

		internal string Software
		{
			get
			{
				return this.logSchemaImpl.Software;
			}
		}

		internal string Version
		{
			get
			{
				return this.logSchemaImpl.Version;
			}
		}

		internal string LogType
		{
			get
			{
				return this.logSchemaImpl.LogType;
			}
		}

		internal string[] Fields
		{
			get
			{
				return this.logSchemaImpl.Fields;
			}
		}

		private LogSchema logSchemaImpl;
	}
}
