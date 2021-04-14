using System;
using System.Net;
using System.Text;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class ServerResponse
	{
		public ServerResponse(Uri uri, HttpStatusCode statusCode, string contentType, TimeSpan responseTime, string text, string headerText)
		{
			this.uri = uri;
			this.statusCode = statusCode;
			this.contentType = contentType;
			this.responseTime = responseTime;
			this.text = text;
			this.headerText = headerText;
		}

		public string ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public string Text
		{
			get
			{
				return this.text;
			}
		}

		public HttpStatusCode StatusCode
		{
			get
			{
				return this.statusCode;
			}
		}

		public TimeSpan ResponseTime
		{
			get
			{
				return this.responseTime;
			}
		}

		public Uri Uri
		{
			get
			{
				return this.uri;
			}
		}

		public string HeaderText
		{
			get
			{
				return this.headerText;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Uri: ");
			stringBuilder.AppendLine(this.uri.AbsoluteUri);
			stringBuilder.Append("Status code: ");
			stringBuilder.AppendLine(this.statusCode.ToString());
			stringBuilder.Append("Content type: ");
			stringBuilder.AppendLine(this.contentType);
			stringBuilder.Append("Response time: ");
			stringBuilder.AppendLine(this.responseTime.ToString());
			stringBuilder.Append("Response text: ");
			stringBuilder.AppendLine(this.text);
			stringBuilder.Append("Headers: ");
			stringBuilder.AppendLine(this.headerText);
			return stringBuilder.ToString();
		}

		private readonly string contentType;

		private readonly string headerText;

		private readonly string text;

		private readonly HttpStatusCode statusCode;

		private readonly TimeSpan responseTime;

		private readonly Uri uri;
	}
}
