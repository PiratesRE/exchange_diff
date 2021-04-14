using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class ErrorResult
	{
		private ErrorResult(Exception e)
		{
			this.ExceptionMessage = e.Message;
			this.ExceptionType = e.GetType().ToString();
			this.ExceptionStack = (string.IsNullOrEmpty(e.StackTrace) ? string.Empty : e.StackTrace.Replace("\r\n", string.Empty));
		}

		public static List<ErrorResult> GetErrorResultListFromException(Exception e)
		{
			List<ErrorResult> list = new List<ErrorResult>();
			for (Exception ex = e; ex != null; ex = ex.InnerException)
			{
				list.Add(new ErrorResult(ex));
			}
			return list;
		}

		public static string GetSerializedString(IList<ErrorResult> errorResults)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ErrorResult errorResult in errorResults)
			{
				stringBuilder.Append("[Type:");
				stringBuilder.Append(errorResult.ExceptionType);
				stringBuilder.Append("]");
				stringBuilder.Append("[Message:");
				stringBuilder.Append(errorResult.ExceptionMessage);
				stringBuilder.Append("]");
				stringBuilder.Append("[ExceptionStack:");
				stringBuilder.Append(errorResult.ExceptionStack);
				stringBuilder.Append("]");
			}
			return stringBuilder.ToString();
		}

		public string ExceptionMessage;

		public string ExceptionType;

		public string ExceptionStack;
	}
}
