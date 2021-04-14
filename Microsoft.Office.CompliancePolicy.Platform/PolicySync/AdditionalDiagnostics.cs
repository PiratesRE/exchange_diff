using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[Serializable]
	public sealed class AdditionalDiagnostics
	{
		public AdditionalDiagnostics()
		{
		}

		public AdditionalDiagnostics(string server, Exception exception)
		{
			this.Server = server;
			int num = 0;
			List<ExceptionRecord> list = null;
			while (exception != null && num < 10)
			{
				num++;
				if (list == null)
				{
					list = new List<ExceptionRecord>();
				}
				list.Add(new ExceptionRecord(exception));
				exception = exception.InnerException;
			}
			if (list != null)
			{
				this.ExceptionRecords = list.ToArray();
			}
		}

		public string Server { get; set; }

		public ExceptionRecord[] ExceptionRecords { get; set; }

		public string Serialize()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AdditionalDiagnostics));
			string result;
			using (StringWriter stringWriter = new StringWriter())
			{
				xmlSerializer.Serialize(stringWriter, this);
				stringWriter.Flush();
				result = stringWriter.ToString();
			}
			return result;
		}
	}
}
