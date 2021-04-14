using System;
using System.Configuration;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class LightWeightLogSession
	{
		internal LightWeightLogSession(LightWeightLog lightLog, LogRowFormatter row)
		{
			this.lightLog = lightLog;
			this.row = row;
			this.row[2] = 0;
			this.Context = new StringBuilder(1024);
			string text = ConfigurationManager.AppSettings["ProxyTrafficBatchDuration"];
			int num;
			if (!string.IsNullOrEmpty(text) && int.TryParse(text, out num) && (double)num <= TimeSpan.FromMinutes(15.0).TotalSeconds && num >= 0)
			{
				this.proxyTrafficInterval = num;
			}
			else
			{
				ProtocolBaseServices.ServerTracer.TraceDebug<string>(0L, "Invalid Config value '{0}' for ProxyTrafficBatchDuration.  Use Default value.", text);
			}
			this.flushProxyTrafficTime = DateTime.UtcNow.AddSeconds((double)this.proxyTrafficInterval);
		}

		public string User { get; set; }

		public string OrganizationId
		{
			get
			{
				return this.organizationId;
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(this.organizationId))
				{
					this.organizationId = value;
				}
			}
		}

		public int ProcessingTime { get; set; }

		public long RequestSize { get; set; }

		public long ResponseSize { get; set; }

		public byte[] Command { get; private set; }

		public string Parameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && value.Length > 2048)
				{
					this.parameters = value.Remove(2048);
					return;
				}
				this.parameters = value;
			}
		}

		public string Result { get; set; }

		public ActivityScope ActivityScope { get; set; }

		public IBudget Budget { get; set; }

		public int? RowsProcessed { get; set; }

		public int? Recent { get; set; }

		public int? Unseen { get; set; }

		public int? ItemsDeleted { get; set; }

		public long? TotalSize { get; set; }

		public int? FolderCount { get; set; }

		public int? SearchType { get; set; }

		public string ClientIp { get; set; }

		public string CafeActivityId { get; set; }

		public Exception ExceptionCaught { get; set; }

		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.message = string.Empty;
					return;
				}
				if (string.IsNullOrEmpty(this.message))
				{
					this.message = value;
					return;
				}
				this.message += ";" + value;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.errorMessage = string.Empty;
					return;
				}
				if (string.IsNullOrEmpty(this.errorMessage))
				{
					this.errorMessage = value;
					return;
				}
				this.errorMessage += ":" + value;
			}
		}

		public string LiveIdAuthResult { get; set; }

		public string ProxyDestination { get; set; }

		private StringBuilder Context { get; set; }

		public void BeginCommand(byte[] command)
		{
			this.Command = command;
		}

		public void FlushProxyTraffic()
		{
			if (this.flushProxyTrafficTime <= DateTime.UtcNow)
			{
				lock (this)
				{
					if (this.flushProxyTrafficTime <= DateTime.UtcNow)
					{
						this.CompleteCommand();
						this.Command = LightWeightLogSession.ProxyBuf;
						this.Parameters = this.ProxyDestination;
						this.flushProxyTrafficTime = DateTime.UtcNow.AddSeconds((double)this.proxyTrafficInterval);
					}
				}
			}
		}

		public void CompleteCommand()
		{
			if (this.Command == null && this.Result == null)
			{
				return;
			}
			this.AppendContextString("R", this.Result);
			this.AppendContextInt("Rows", this.RowsProcessed);
			this.AppendContextInt("Recent", this.Recent);
			this.AppendContextLong("TotalSize", this.TotalSize);
			this.AppendContextInt("Search", this.SearchType);
			this.AppendContextString("ClientIp", this.ClientIp);
			this.AppendContextString("Msg", this.Message);
			this.AppendContextInt("Unseen", this.Unseen);
			this.AppendContextInt("FolderCount", this.FolderCount);
			this.AppendContextInt("ItemsDeleted", this.ItemsDeleted);
			if (string.IsNullOrEmpty(this.ProxyDestination))
			{
				this.AppendContextString("CafeActivityId", this.CafeActivityId);
			}
			if (!string.IsNullOrEmpty(this.ErrorMessage))
			{
				this.AppendContextString("ErrMsg", this.ErrorMessage);
			}
			if (!string.IsNullOrEmpty(this.LiveIdAuthResult))
			{
				this.AppendContextString("LiveIdAR", this.LiveIdAuthResult);
			}
			if (!string.IsNullOrEmpty(this.organizationId))
			{
				this.AppendContextString("Oid", this.organizationId);
			}
			if (this.ExceptionCaught != null)
			{
				StringBuilder stringBuilder = new StringBuilder(1024);
				for (Exception ex = this.ExceptionCaught; ex != null; ex = ex.InnerException)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append('/');
					}
					else
					{
						stringBuilder.Append(ex.Message);
						stringBuilder.Append('-');
					}
					stringBuilder.Append(ex.GetType().FullName);
				}
				this.AppendContextString("Excpt", stringBuilder.ToString());
				this.AppendContextString("ExStk", ExceptionTools.GetCompressedStackTrace(this.ExceptionCaught));
			}
			if (this.ActivityScope != null)
			{
				if (string.IsNullOrEmpty(this.ProxyDestination))
				{
					string text = LogRowFormatter.FormatCollection(this.ActivityScope.GetFormattableStatistics());
					text = text.Replace(';', ',');
					this.AppendContextString("ActivityContextData", text);
				}
				else
				{
					this.AppendContextString("ActivityContextData", this.ActivityScope.ActivityId.ToString());
				}
			}
			if (this.Budget != null && string.IsNullOrEmpty(this.ProxyDestination))
			{
				this.AppendContextString("Budget", this.Budget.ToString());
			}
			this.WriteLog();
			this.Clear();
		}

		private void WriteLog()
		{
			lock (this.loggingLock)
			{
				this.row[5] = this.User;
				this.row[6] = this.ProcessingTime;
				this.row[7] = this.RequestSize;
				this.row[8] = this.ResponseSize;
				this.row[9] = this.Command;
				this.row[10] = this.Parameters;
				this.row[11] = this.Context.ToString();
				this.lightLog.Append(this.row);
				this.row[2] = (int)this.row[2] + 1;
				this.row[6] = null;
				this.row[7] = null;
				this.row[8] = null;
				this.row[9] = null;
				this.row[10] = null;
				this.row[11] = null;
			}
		}

		private void AppendContextString(string dataName, string dataValue)
		{
			if (!string.IsNullOrEmpty(dataValue))
			{
				if (this.Context.Length > 0)
				{
					this.Context.Append(';');
				}
				if (dataValue.IndexOfAny(LightWeightLogSession.QuotableChars) > -1)
				{
					this.Context.AppendFormat("{0}=\"{1}\"", dataName, dataValue);
					return;
				}
				this.Context.AppendFormat("{0}={1}", dataName, dataValue);
			}
		}

		private void AppendContextInt(string dataName, int dataValue)
		{
			if (dataValue != 0)
			{
				if (this.Context.Length > 0)
				{
					this.Context.Append(';');
				}
				this.Context.Append(dataName);
				this.Context.Append('=');
				this.Context.Append(dataValue);
			}
		}

		private void AppendContextInt(string dataName, int? dataValue)
		{
			if (dataValue != null)
			{
				if (this.Context.Length > 0)
				{
					this.Context.Append(';');
				}
				this.Context.Append(dataName);
				this.Context.Append('=');
				this.Context.Append(dataValue);
			}
		}

		private void AppendContextLong(string dataName, long dataValue)
		{
			if (dataValue != 0L)
			{
				if (this.Context.Length > 0)
				{
					this.Context.Append(';');
				}
				this.Context.Append(dataName);
				this.Context.Append('=');
				this.Context.Append(dataValue);
			}
		}

		private void AppendContextLong(string dataName, long? dataValue)
		{
			if (dataValue != null)
			{
				if (this.Context.Length > 0)
				{
					this.Context.Append(';');
				}
				this.Context.Append(dataName);
				this.Context.Append('=');
				this.Context.Append(dataValue);
			}
		}

		private void AppendContextUint(string dataName, uint dataValue)
		{
			this.AppendContextInt(dataName, (int)dataValue);
		}

		private void Clear()
		{
			this.Context.Length = 0;
			this.ProcessingTime = 0;
			this.RequestSize = 0L;
			this.ResponseSize = 0L;
			this.Command = null;
			this.Parameters = null;
			this.Result = null;
			this.ActivityScope = null;
			this.RowsProcessed = null;
			this.Recent = null;
			this.Unseen = null;
			this.ItemsDeleted = null;
			this.TotalSize = null;
			this.FolderCount = null;
			this.SearchType = null;
			this.ClientIp = null;
			this.CafeActivityId = null;
			this.ExceptionCaught = null;
			this.Message = null;
			this.ErrorMessage = null;
			this.LiveIdAuthResult = null;
		}

		internal static readonly byte[] ProxyBuf = Encoding.ASCII.GetBytes("proxy");

		private static readonly char[] QuotableChars = new char[]
		{
			' ',
			'"',
			',',
			';'
		};

		private LightWeightLog lightLog;

		private LogRowFormatter row;

		private string parameters;

		private string message;

		private string errorMessage;

		private string organizationId;

		private object loggingLock = new object();

		private DateTime flushProxyTrafficTime;

		private readonly int proxyTrafficInterval = 30;
	}
}
