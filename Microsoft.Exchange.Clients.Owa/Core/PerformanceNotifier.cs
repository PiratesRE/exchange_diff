using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class PerformanceNotifier : IPendingRequestNotifier
	{
		public bool ShouldThrottle
		{
			get
			{
				return false;
			}
		}

		internal void RegisterWithPendingRequestNotifier()
		{
			UserContext userContext = OwaContext.Current.UserContext;
			try
			{
				userContext.PendingRequestManager.AddPendingRequestNotifier(this);
			}
			catch (Exception)
			{
			}
			lock (this.list)
			{
				this.registered = true;
			}
		}

		internal void UnregisterWithPendingRequestNotifier()
		{
			this.registered = false;
		}

		public event DataAvailableEventHandler DataAvailable;

		public string ReadDataAndResetState()
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			lock (this.list)
			{
				int count = this.list.Count;
				for (int i = 0; i < count; i++)
				{
					int serverId = this.GetServerId(i);
					OwaPerformanceData owaPerformanceData = this.list[i];
					if (!this.initialized[i])
					{
						owaPerformanceData.GenerateInitialPayload(stringBuilder, serverId);
						this.initialized[i] = true;
					}
					if (this.dirty[i])
					{
						string value;
						owaPerformanceData.RetrieveJSforPerfData(serverId, out value);
						stringBuilder.Append("excPrfCnsl(\"");
						stringBuilder.Append(value);
						stringBuilder.Append("\");");
						this.dirty[i] = false;
						if (this.finished[i])
						{
							owaPerformanceData.RetrieveFinishJS(serverId, out value);
							stringBuilder.Append("excPrfCnsl(\"");
							stringBuilder.Append(value);
							stringBuilder.Append("\");");
						}
					}
				}
				this.hasData = false;
			}
			return stringBuilder.ToString();
		}

		private int GetServerId(int index)
		{
			if (this.lastInList % 40 > index)
			{
				return this.lastInList + 40 + index - this.lastInList % 40;
			}
			return this.lastInList + index - this.lastInList % 40;
		}

		public void ReadDataAsHtml(TextWriter writer)
		{
			lock (this.list)
			{
				this.initialized = new bool[40];
				int count = this.list.Count;
				for (int i = count - 1; i >= 0; i--)
				{
					int serverId = this.GetServerId(i);
					OwaPerformanceData owaPerformanceData = this.list[i];
					this.initialized[i] = true;
					string value;
					owaPerformanceData.RetrieveHtmlForPerfData(serverId, out value, this.finished[i], i + 1);
					writer.Write(value);
				}
			}
		}

		public void ConnectionAliveTimer()
		{
		}

		internal void UpdatePerformanceData(OwaPerformanceData performanceData, bool finishedRequest)
		{
			if (performanceData == null)
			{
				throw new ArgumentNullException("performanceData");
			}
			lock (this.list)
			{
				int num = this.list.IndexOf(performanceData);
				if (num < 0)
				{
					this.AddPerformanceData(performanceData, finishedRequest);
				}
				else
				{
					this.dirty[num] = true;
					if (finishedRequest)
					{
						this.finished[num] = true;
					}
					if (this.registered && !this.hasData)
					{
						this.hasData = true;
						this.DataAvailable(this, null);
					}
				}
			}
		}

		internal void FinishPerformanceData(OwaPerformanceData performanceData)
		{
			lock (this.list)
			{
				int num = this.list.IndexOf(performanceData);
				if (num >= 0)
				{
					this.finished[num] = true;
					if (this.registered && !this.hasData)
					{
						this.hasData = true;
						this.DataAvailable(this, null);
					}
				}
			}
		}

		internal void AddPerformanceData(OwaPerformanceData performanceData)
		{
			this.AddPerformanceData(performanceData, false);
		}

		internal void AddPerformanceData(OwaPerformanceData performanceData, bool finishedRequest)
		{
			if (performanceData == null)
			{
				throw new ArgumentNullException("performanceData");
			}
			lock (this.list)
			{
				if (this.list.Count >= 40)
				{
					this.list[this.lastInList % 40] = performanceData;
					this.initialized[this.lastInList % 40] = false;
					this.dirty[this.lastInList % 40] = true;
					if (finishedRequest)
					{
						this.finished[this.lastInList % 40] = true;
					}
					else
					{
						this.finished[this.lastInList % 40] = false;
					}
					this.lastInList++;
				}
				else
				{
					this.list.Add(performanceData);
					this.dirty[this.list.Count - 1] = true;
					if (finishedRequest)
					{
						this.finished[this.list.Count - 1] = true;
					}
					else
					{
						this.finished[this.list.Count - 1] = false;
					}
				}
				if (this.registered && !this.hasData)
				{
					this.hasData = true;
					try
					{
						this.DataAvailable(this, null);
					}
					catch (OwaNotificationPipeWriteException)
					{
					}
				}
			}
		}

		private const int MaxSize = 40;

		private List<OwaPerformanceData> list = new List<OwaPerformanceData>();

		private bool hasData;

		private int lastInList;

		private bool[] initialized = new bool[40];

		private bool[] finished = new bool[40];

		private bool[] dirty = new bool[40];

		private volatile bool registered;
	}
}
