using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal static class Helper
	{
		public static string GetIdFromUrl(string url)
		{
			string result = string.Empty;
			Regex regex = new Regex("^.*'([^']+)'.*$");
			Match match = regex.Match(url);
			if (1 < match.Groups.Count)
			{
				result = match.Groups[1].Captures[0].Value;
			}
			return result;
		}

		public static HttpOperationException AsHttpOperationException(this Exception exception)
		{
			if (exception is AggregateException)
			{
				exception = ((AggregateException)exception).Flatten().InnerException;
			}
			HttpOperationException ex = exception as HttpOperationException;
			if (exception is WebException)
			{
				WebException ex2 = (WebException)exception;
				HttpWebResponse httpWebResponse = ex2.Response as HttpWebResponse;
				ex = new HttpOperationException(ex2.Message, ex2);
				if (httpWebResponse != null && Helper.IsUcwaErrorResponse(httpWebResponse))
				{
					try
					{
						ex.ErrorInformation = ErrorJsonDeserializer.Deserialize(ex2.Response.GetResponseStream());
						return ex;
					}
					catch (Exception)
					{
						return ex;
					}
				}
				ex.HttpResponse = (ex2.Response as HttpWebResponse);
			}
			return ex;
		}

		public static Task<T> CompletedTask<T>(T result)
		{
			TaskCompletionSource<T> taskCompletionSource = new TaskCompletionSource<T>();
			taskCompletionSource.SetResult(result);
			return taskCompletionSource.Task;
		}

		public static OperationFailureException AsOperationFailureException(this Exception exception, string message)
		{
			return new OperationFailureException(message, exception);
		}

		private static bool IsUcwaErrorResponse(HttpWebResponse errorResponse)
		{
			return "application/vnd.microsoft.com.ucwa+json".Equals(errorResponse.ContentType, StringComparison.OrdinalIgnoreCase);
		}
	}
}
