using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal class LogEntry
	{
		private Dictionary<string, int> exceptions { get; set; }

		internal LogEntry()
		{
			this.exceptions = new Dictionary<string, int>();
		}

		public bool IsDeadlineExpired { get; set; }

		internal void AddExceptionToLog(Exception ex)
		{
			if (ex != null)
			{
				StringBuilder stringBuilder = new StringBuilder(ex.GetType().Name);
				if (ex.InnerException != null)
				{
					stringBuilder.AppendFormat("_{0}_{1}", ex.InnerException.GetType().Name, ex.InnerException.Message);
				}
				else
				{
					stringBuilder.AppendFormat("_{0}_{1}", "NoInnerException", ex.Message);
				}
				this.AddToFailuresDictionary(stringBuilder.ToString());
			}
		}

		private void AddToFailuresDictionary(string key)
		{
			if (this.exceptions.ContainsKey(key))
			{
				int num = this.exceptions[key];
				this.exceptions[key] = num++;
				return;
			}
			this.exceptions.Add(key, 1);
		}

		internal string BuildFailureString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, int> keyValuePair in this.exceptions)
			{
				stringBuilder.Append(string.Format("{0}_{1}|", keyValuePair.Key, keyValuePair.Value));
			}
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				'|'
			});
		}

		private const string noInnerExceptionStr = "NoInnerException";
	}
}
