using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Office.CompliancePolicy
{
	public abstract class ExecutionLog
	{
		public static void GetExceptionTypeAndDetails(Exception e, out List<string> types, out List<string> messages, out string chain, bool chainOnly)
		{
			Exception ex = e;
			chain = string.Empty;
			types = null;
			messages = null;
			if (!chainOnly)
			{
				types = new List<string>();
				messages = new List<string>();
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			for (;;)
			{
				string text = ex.GetType().ToString();
				string message = ex.Message;
				if (!chainOnly)
				{
					types.Add(text);
					messages.Add(message);
				}
				stringBuilder.Append("[Type:");
				stringBuilder.Append(text);
				stringBuilder.Append("]");
				stringBuilder.Append("[Message:");
				stringBuilder.Append(message);
				stringBuilder.Append("]");
				stringBuilder.Append("[Stack:");
				stringBuilder.Append(string.IsNullOrEmpty(ex.StackTrace) ? string.Empty : ex.StackTrace.Replace("\r\n", string.Empty));
				stringBuilder.Append("]");
				if (ex.InnerException == null || num > 10)
				{
					break;
				}
				ex = ex.InnerException;
				num++;
			}
			chain = stringBuilder.ToString();
		}

		public virtual void LogInformation(string client, string tenantId, string correlationId, string contextData, params KeyValuePair<string, object>[] customData)
		{
			throw new NotImplementedException();
		}

		public virtual void LogVerbose(string client, string tenantId, string correlationId, string contextData, params KeyValuePair<string, object>[] customData)
		{
			throw new NotImplementedException();
		}

		public virtual void LogWarnining(string client, string tenantId, string correlationId, string contextData, params KeyValuePair<string, object>[] customData)
		{
			throw new NotImplementedException();
		}

		public virtual void LogError(string client, string tenantId, string correlationId, Exception exception, string contextData, params KeyValuePair<string, object>[] customData)
		{
			throw new NotImplementedException();
		}

		public abstract void LogOneEntry(string client, string tenantId, string correlationId, ExecutionLog.EventType eventType, string tag, string contextData, Exception exception, params KeyValuePair<string, object>[] customData);

		public abstract void LogOneEntry(string client, string correlationId, ExecutionLog.EventType eventType, string contextData, Exception exception);

		public void LogOneEntry(ExecutionLog.EventType eventType, string client, string correlationId, Exception exception, string format, params object[] args)
		{
			this.LogOneEntry(client, correlationId, eventType, string.Format(CultureInfo.InvariantCulture, format, args), exception);
		}

		public void LogOneEntry(ExecutionLog.EventType eventType, string client, string correlationId, string format, params object[] args)
		{
			this.LogOneEntry(eventType, client, correlationId, null, format, args);
		}

		public const string TagKey = "Tag";

		private const string CRLF = "\r\n";

		public enum EventType
		{
			Verbose,
			Information,
			Warning,
			Error,
			CriticalError
		}
	}
}
