using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.LogAnalyzer.Extensions.OABDownloadLog
{
	public class OABDownloadLogLine : LogLine
	{
		public OABDownloadLogLine(List<string> header, LogSourceLine line) : base(line)
		{
			this.data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			IList<string> columns = line.GetColumns();
			if (columns.Count < header.Count)
			{
				base.RaiseException(string.Format("Headers collection provided does not match the schema. Header count: {0}, Columns count: {1}", header.Count, columns.Count), new object[0]);
			}
			if (line.Timestamp != null)
			{
				this.timestamp = line.Timestamp.Value;
			}
			else
			{
				base.RaiseException("First column of line should be a well-formatted DateTime, column-value: {0}", new object[]
				{
					columns[0]
				});
			}
			for (int i = 0; i < header.Count; i++)
			{
				this.data[header[i]] = columns[i];
			}
		}

		public override DateTime Timestamp
		{
			get
			{
				return this.timestamp;
			}
		}

		public string Organization
		{
			get
			{
				return this.GetColumnValue("Domain");
			}
		}

		public string HttpStatus
		{
			get
			{
				return this.GetColumnValue("HttpStatus");
			}
		}

		public string FailureCode
		{
			get
			{
				return this.GetColumnValue("FailureCode");
			}
		}

		public string LastRequestedTime
		{
			get
			{
				return this.GetColumnValue("LastRequestedTime");
			}
		}

		public string LastTouchedTime
		{
			get
			{
				return this.GetColumnValue("LastTouchedTime");
			}
		}

		public string GenericInfo
		{
			get
			{
				return this.GetColumnValue("GenericInfo");
			}
		}

		public string GenericErrors
		{
			get
			{
				return this.GetColumnValue("GenericErrors");
			}
		}

		public OrganizationStatus OrgStatus
		{
			get
			{
				string columnValue = this.GetColumnValue("OrganizationStatus");
				OrganizationStatus result = OrganizationStatus.Invalid;
				if (!string.IsNullOrEmpty(columnValue) && !Enum.TryParse<OrganizationStatus>(columnValue, out result))
				{
					result = OrganizationStatus.Invalid;
				}
				return result;
			}
		}

		public bool IsAddressListDeleted
		{
			get
			{
				bool flag;
				return bool.TryParse(this.GetColumnValue("IsAddressListDeleted"), out flag) && flag;
			}
		}

		private string GetColumnValue(string columnName)
		{
			string result;
			if (this.data != null && this.data.TryGetValue(columnName, out result))
			{
				return result;
			}
			return null;
		}

		private readonly DateTime timestamp;

		private readonly Dictionary<string, string> data;
	}
}
