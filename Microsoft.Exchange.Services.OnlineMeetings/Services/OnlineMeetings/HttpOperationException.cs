using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	[Serializable]
	internal class HttpOperationException : OnlineMeetingSchedulerException
	{
		public HttpOperationException() : this(null)
		{
		}

		public HttpOperationException(string message) : this(message, null)
		{
		}

		public HttpOperationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected HttpOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public ErrorInformation ErrorInformation { get; set; }

		public HttpWebResponse HttpResponse
		{
			get
			{
				return this.httpResponse;
			}
			set
			{
				if (this.httpResponse != value)
				{
					this.httpResponse = value;
					if (this.httpResponse == null)
					{
						this.ErrorInformation = null;
						return;
					}
					this.ErrorInformation = new ErrorInformation();
					this.ErrorInformation.Code = ErrorInformation.TryGetErrorFromHttpStatusCode(this.httpResponse.StatusCode);
					this.ErrorInformation.Message = this.httpResponse.GetReasonHeader();
				}
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		private HttpWebResponse httpResponse;
	}
}
