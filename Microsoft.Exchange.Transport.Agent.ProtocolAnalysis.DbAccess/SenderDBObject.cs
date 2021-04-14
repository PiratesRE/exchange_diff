using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class SenderDBObject : Database
	{
		public SenderDBObject(IPAddress address, Trace traceTag)
		{
			if (traceTag == null)
			{
				throw new LocalizedException(DbStrings.InvalidTraceObject);
			}
			this.loaded = false;
			this.senderAddress = address;
			this.dataBlob = null;
			this.detectionTime = DateTime.MinValue;
			this.detectionPending = false;
			this.status = OPDetectionResult.Unknown;
			this.reverseDns = string.Empty;
			this.queryTime = DateTime.MinValue;
			this.queryPending = false;
			this.srl = 0;
			this.extOpenProxy = false;
			this.expirationTime = DateTime.MinValue;
			this.traceTag = traceTag;
		}

		public byte[] ProtocolAnalysisDataBlob
		{
			get
			{
				return this.dataBlob;
			}
		}

		public DateTime OpenProxyDetectionTime
		{
			get
			{
				return this.detectionTime;
			}
		}

		public bool OpenProxyDetectionPending
		{
			get
			{
				return this.detectionPending;
			}
		}

		public OPDetectionResult OpenProxyStatus
		{
			get
			{
				return this.status;
			}
		}

		public string ReverseDns
		{
			get
			{
				return this.reverseDns;
			}
		}

		public DateTime ReverseDnsQueryTime
		{
			get
			{
				return this.queryTime;
			}
		}

		public bool ReverseDnsQueryPending
		{
			get
			{
				return this.queryPending;
			}
		}

		public int SenderReputationLevel
		{
			get
			{
				return this.srl;
			}
		}

		public bool SenderReputationIsOpenProxy
		{
			get
			{
				return this.extOpenProxy;
			}
		}

		public DateTime SenderReputationExpirationTime
		{
			get
			{
				return this.expirationTime;
			}
		}

		public int WorkQueueSize
		{
			get
			{
				return 200;
			}
		}

		public IPAddress SenderAddress
		{
			get
			{
				return this.senderAddress;
			}
		}

		public bool Load()
		{
			this.traceTag.TraceDebug<IPAddress>((long)this.GetHashCode(), "Loading data for sender {0}.", this.senderAddress);
			this.loaded = false;
			try
			{
				lock (Database.syncObject)
				{
					if (Database.IsDbClosed)
					{
						return false;
					}
					this.traceTag.TraceDebug<IPAddress>((long)this.GetHashCode(), "Loading protocol analysis data for sender {0}.", this.senderAddress);
					ProtocolAnalysisRowData protocolAnalysisRowData = DataRowAccessBase<ProtocolAnalysisTable, ProtocolAnalysisRowData>.Find(this.senderAddress.ToString());
					if (protocolAnalysisRowData == null)
					{
						this.traceTag.TraceDebug<IPAddress>((long)this.GetHashCode(), "Failed to find {0}. Creating a new record", this.senderAddress);
						protocolAnalysisRowData = DataRowAccessBase<ProtocolAnalysisTable, ProtocolAnalysisRowData>.NewData(this.senderAddress.ToString());
					}
					else
					{
						this.dataBlob = protocolAnalysisRowData.DataBlob;
						this.reverseDns = protocolAnalysisRowData.ReverseDns;
						this.queryTime = protocolAnalysisRowData.LastQueryTime;
						this.queryPending = protocolAnalysisRowData.Processing;
					}
					protocolAnalysisRowData.LastUpdateTime = DateTime.UtcNow;
					protocolAnalysisRowData.Commit();
					this.traceTag.TraceDebug<IPAddress>((long)this.GetHashCode(), "Loading open proxy status data for sender {0}.", this.senderAddress);
					OpenProxyStatusRowData openProxyStatusRowData = DataRowAccessBase<OpenProxyStatusTable, OpenProxyStatusRowData>.Find(this.senderAddress.ToString());
					if (openProxyStatusRowData != null)
					{
						this.status = (OPDetectionResult)openProxyStatusRowData.OpenProxyStatus;
						this.detectionTime = openProxyStatusRowData.LastDetectionTime;
						this.detectionPending = openProxyStatusRowData.Processing;
					}
					if (this.senderAddress.AddressFamily == AddressFamily.InterNetwork)
					{
						this.traceTag.TraceDebug<IPAddress>((long)this.GetHashCode(), "Loading sender reputation data for sender {0}.", this.senderAddress);
						SenderReputationRowData senderReputationRowData = DataRowAccessBase<SenderReputationTable, SenderReputationRowData>.Find(CMD5.GetHash(this.senderAddress.GetAddressBytes()));
						if (senderReputationRowData != null)
						{
							this.srl = senderReputationRowData.Srl;
							this.extOpenProxy = senderReputationRowData.OpenProxy;
							this.expirationTime = senderReputationRowData.ExpirationTime;
						}
					}
					this.loaded = true;
				}
			}
			catch
			{
				this.traceTag.TraceDebug<IPAddress>((long)this.GetHashCode(), "Failed to load sender data for {0}. Exception thrown.", this.senderAddress);
				throw;
			}
			return this.loaded;
		}

		public bool Update(byte[] senderData, bool openProxyDetection, bool reverseDnsQuery)
		{
			if (!this.loaded)
			{
				throw new LocalizedException(DbStrings.PaRecordNotLoaded);
			}
			this.dataBlob = senderData;
			try
			{
				lock (Database.syncObject)
				{
					if (Database.IsDbClosed)
					{
						return false;
					}
					this.traceTag.TraceDebug<IPAddress>((long)this.GetHashCode(), "Updating protocol analysis data for sender {0}.", this.senderAddress);
					ProtocolAnalysisRowData protocolAnalysisRowData = DataRowAccessBase<ProtocolAnalysisTable, ProtocolAnalysisRowData>.Find(this.senderAddress.ToString());
					if (protocolAnalysisRowData != null)
					{
						protocolAnalysisRowData.DataBlob = this.dataBlob;
						if (openProxyDetection)
						{
							protocolAnalysisRowData.LastUpdateTime = DateTime.UtcNow;
						}
						else
						{
							protocolAnalysisRowData.Processing = true;
						}
						protocolAnalysisRowData.Commit();
					}
					else if (reverseDnsQuery)
					{
						protocolAnalysisRowData = DataRowAccessBase<ProtocolAnalysisTable, ProtocolAnalysisRowData>.NewData(this.senderAddress.ToString());
						protocolAnalysisRowData.DataBlob = this.dataBlob;
						protocolAnalysisRowData.Processing = true;
						protocolAnalysisRowData.LastUpdateTime = DateTime.UtcNow;
						protocolAnalysisRowData.Commit();
					}
				}
				if (!openProxyDetection)
				{
					return true;
				}
				this.traceTag.TraceDebug<IPAddress>((long)this.GetHashCode(), "Updating open proxy data for sender {0}.", this.senderAddress);
				lock (Database.syncObject)
				{
					if (Database.IsDbClosed)
					{
						return false;
					}
					OpenProxyStatusRowData openProxyStatusRowData = DataRowAccessBase<OpenProxyStatusTable, OpenProxyStatusRowData>.Find(this.senderAddress.ToString());
					if (openProxyStatusRowData == null)
					{
						openProxyStatusRowData = DataRowAccessBase<OpenProxyStatusTable, OpenProxyStatusRowData>.NewData(this.senderAddress.ToString());
						openProxyStatusRowData.LastAccessTime = DateTime.UtcNow;
					}
					openProxyStatusRowData.OpenProxyStatus = 0;
					openProxyStatusRowData.Processing = true;
					openProxyStatusRowData.Commit();
				}
			}
			catch
			{
				this.traceTag.TraceDebug<IPAddress>((long)this.GetHashCode(), "Failed to update protocol analysis data for {0}. Exception thrown.", this.senderAddress);
				throw;
			}
			return true;
		}

		private const int WorkQueueSizeValue = 200;

		private IPAddress senderAddress;

		private bool loaded;

		private byte[] dataBlob;

		private DateTime detectionTime;

		private bool detectionPending;

		private OPDetectionResult status;

		private string reverseDns;

		private DateTime queryTime;

		private bool queryPending;

		private int srl;

		private bool extOpenProxy;

		private DateTime expirationTime;

		private Trace traceTag;
	}
}
