using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class DataProviderCallLogEvent : ILogEvent
	{
		public DataProviderCallLogEvent(string eventId, UserContext userContext)
		{
			this.userContext = userContext;
			this.EventId = eventId;
			this.fileSize = -1;
			this.spCallsBuilder = new StringBuilder();
		}

		public string EventId { get; private set; }

		public WebHeaderCollection ErrorResponseHeaders { get; set; }

		public AttachmentResultCode ResultCode { get; set; }

		public int? NumberOfItems { get; set; }

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
			set
			{
				this.errorMessage = this.errorMessage + value + "| ";
			}
		}

		public void Reset()
		{
			this.stopWatch = Stopwatch.StartNew();
			this.fileSize = -1;
			this.ResultCode = AttachmentResultCode.Success;
			this.error = null;
			this.errorMessage = string.Empty;
			this.spCallStopWatch = null;
			this.NumberOfItems = null;
			this.ErrorResponseHeaders = null;
			this.spCallsBuilder = new StringBuilder();
			this.totalSPCallTime = 0L;
		}

		public void SetFinish()
		{
			if (this.stopWatch.IsRunning)
			{
				this.stopWatch.Stop();
			}
		}

		public void TrackSPCallBegin()
		{
			this.spCallStopWatch = Stopwatch.StartNew();
		}

		public void TrackSPCallEnd(string callName, string correlationId)
		{
			if (this.spCallStopWatch != null && this.spCallStopWatch.IsRunning)
			{
				this.spCallStopWatch.Stop();
				this.totalSPCallTime += this.spCallStopWatch.ElapsedMilliseconds;
				this.spCallsBuilder.AppendFormat("{0}_{1}_{2}|", callName, this.spCallStopWatch.ElapsedMilliseconds, correlationId);
			}
		}

		public void SetFileSize(int size)
		{
			this.fileSize = size;
		}

		public void SetError(string error)
		{
			this.error = error;
		}

		public void SetError(Exception exception)
		{
			this.error = exception.GetType().Name;
			this.ErrorMessage = exception.ToString();
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("result", (int)this.ResultCode);
			if (this.userContext != null)
			{
				if (this.userContext.ExchangePrincipal != null)
				{
					dictionary.Add("user", this.userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
				}
				if (this.error != null)
				{
					dictionary.Add("organizationId", this.userContext.LogonIdentity.GetOWAMiniRecipient().OrganizationId);
				}
			}
			if (this.stopWatch != null)
			{
				dictionary.Add("time", this.stopWatch.ElapsedMilliseconds.ToString());
			}
			if (this.spCallsBuilder != null)
			{
				dictionary.Add("spCalls", this.spCallsBuilder.ToString());
				dictionary.Add("totalSPCallTime", this.totalSPCallTime.ToString());
			}
			if (this.fileSize > -1)
			{
				dictionary.Add("size", this.fileSize);
			}
			if (this.NumberOfItems != null)
			{
				dictionary.Add("items", this.NumberOfItems.Value);
			}
			if (this.error != null)
			{
				dictionary.Add("error", this.error);
			}
			if (!string.IsNullOrEmpty(this.errorMessage))
			{
				dictionary.Add("errorMsg", this.errorMessage);
			}
			if (this.ErrorResponseHeaders != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("[");
				foreach (string text in this.ErrorResponseHeaders.AllKeys)
				{
					string text2 = this.ErrorResponseHeaders[text];
					if (!string.IsNullOrEmpty(text2))
					{
						stringBuilder.AppendFormat("{0}={1};", text, text2);
					}
				}
				stringBuilder.Append("]");
				dictionary.Add("responseHeaders", stringBuilder.ToString());
			}
			return dictionary;
		}

		private Stopwatch stopWatch;

		private Stopwatch spCallStopWatch;

		private int fileSize;

		private string error;

		private string errorMessage;

		private readonly UserContext userContext;

		private StringBuilder spCallsBuilder;

		private long totalSPCallTime;
	}
}
