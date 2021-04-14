using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.Delivery;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class DeliveryThrottlingLogWorker : IDeliveryThrottlingLogWorker
	{
		public KeyValuePair<Dictionary<string, DeliveryThrottlingLogData>, ReaderWriterLockSlim>[] ResourceDicts
		{
			get
			{
				return this.resourceDicts;
			}
		}

		public DeliveryThrottlingLogWorker(IDeliveryThrottlingLog deliveryThrottlingLog)
		{
			ArgumentValidator.ThrowIfNull("deliveryThrottlingLog", deliveryThrottlingLog);
			this.resourceDicts = new KeyValuePair<Dictionary<string, DeliveryThrottlingLogData>, ReaderWriterLockSlim>[]
			{
				new KeyValuePair<Dictionary<string, DeliveryThrottlingLogData>, ReaderWriterLockSlim>(this.serverThrottleInfo, this.serverThrottleInfoLock),
				new KeyValuePair<Dictionary<string, DeliveryThrottlingLogData>, ReaderWriterLockSlim>(this.mdbThrottleInfoDynamicThrottleDisabled, this.mdbThrottleInfoDynamicThrottleDisabledLock),
				new KeyValuePair<Dictionary<string, DeliveryThrottlingLogData>, ReaderWriterLockSlim>(this.mdbThrottleInfoPendingConnections, this.mdbThrottleInfoPendingConnectionsLock),
				new KeyValuePair<Dictionary<string, DeliveryThrottlingLogData>, ReaderWriterLockSlim>(this.mdbThrottleInfoTimeout, this.mdbThrottleInfoTimeoutLock),
				new KeyValuePair<Dictionary<string, DeliveryThrottlingLogData>, ReaderWriterLockSlim>(this.recipientThrottleInfo, this.recipientThrottleInfoLock),
				new KeyValuePair<Dictionary<string, DeliveryThrottlingLogData>, ReaderWriterLockSlim>(this.concurrentMsgSizeThrottleInfo, this.concurrentMsgSizeThrottleInfoLock)
			};
			this.deliveryThrottlingLog = deliveryThrottlingLog;
			if (this.deliveryThrottlingLog.Enabled)
			{
				this.deliveryThrottlingLog.TrackSummary += this.LogSummaryData;
			}
		}

		public IDeliveryThrottlingLog DeliveryThrottlingLog
		{
			get
			{
				return this.deliveryThrottlingLog;
			}
		}

		public void TrackMDBServerThrottle(bool isThrottle, double mdbServerThreadThreshold)
		{
			if (!this.deliveryThrottlingLog.Enabled)
			{
				return;
			}
			List<KeyValuePair<string, string>> list = null;
			if (mdbServerThreadThreshold <= 0.0)
			{
				list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>("InvalidServerThreadThreshold", mdbServerThreadThreshold.ToString("F0", NumberFormatInfo.InvariantInfo)));
				mdbServerThreadThreshold = double.NaN;
			}
			this.TrackData(this.serverThrottleInfo, this.serverThrottleInfoLock, "localhost", isThrottle, ThrottlingScope.MBServer, ThrottlingResource.Threads, mdbServerThreadThreshold, ThrottlingImpactUnits.Sessions, 1U, Guid.Empty, string.Empty, string.Empty, null, list);
		}

		public void TrackMDBThrottle(bool isThrottle, string mdbName, double mdbResourceThreshold, List<KeyValuePair<string, double>> healthMonitorList, ThrottlingResource throttleResource)
		{
			if (!this.deliveryThrottlingLog.Enabled)
			{
				return;
			}
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			if (string.IsNullOrEmpty(mdbName))
			{
				list.Add(new KeyValuePair<string, string>("InvalidMDBName", (mdbName == null) ? "NULL" : "EMPTY"));
				mdbName = string.Empty;
			}
			if (!mdbResourceThreshold.Equals(double.NaN) && mdbResourceThreshold <= 0.0)
			{
				list.Add(new KeyValuePair<string, string>("InvalidMDBResourceThreshold", mdbResourceThreshold.ToString("F0", NumberFormatInfo.InvariantInfo)));
				mdbResourceThreshold = double.NaN;
			}
			if (list.Count == 0)
			{
				list = null;
			}
			Dictionary<string, DeliveryThrottlingLogData> dictionary = null;
			ReaderWriterLockSlim dictionaryLock = null;
			switch (throttleResource)
			{
			case ThrottlingResource.Threads:
				dictionary = this.mdbThrottleInfoDynamicThrottleDisabled;
				dictionaryLock = this.mdbThrottleInfoDynamicThrottleDisabledLock;
				break;
			case ThrottlingResource.Threads_MaxPerHub:
				dictionary = this.mdbThrottleInfoPendingConnections;
				dictionaryLock = this.mdbThrottleInfoPendingConnectionsLock;
				break;
			case ThrottlingResource.Threads_PendingConnectionTimedOut:
				dictionary = this.mdbThrottleInfoTimeout;
				dictionaryLock = this.mdbThrottleInfoTimeoutLock;
				break;
			}
			this.TrackData(dictionary, dictionaryLock, mdbName, isThrottle, ThrottlingScope.MDB, throttleResource, mdbResourceThreshold, ThrottlingImpactUnits.Sessions, 1U, Guid.Empty, string.Empty, mdbName, healthMonitorList, list);
		}

		public void TrackRecipientThrottle(bool isThrottle, string recipient, Guid orgID, string mdbName, double recipientThreadThreshold)
		{
			if (!this.deliveryThrottlingLog.Enabled)
			{
				return;
			}
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			if (string.IsNullOrEmpty(recipient))
			{
				list.Add(new KeyValuePair<string, string>("InvalidRecipient", (recipient == null) ? "NULL" : "EMPTY"));
				recipient = string.Empty;
			}
			if (string.IsNullOrEmpty(mdbName))
			{
				list.Add(new KeyValuePair<string, string>("InvalidMDBName", (mdbName == null) ? "NULL" : "EMPTY"));
				mdbName = string.Empty;
			}
			if (recipientThreadThreshold <= 0.0)
			{
				list.Add(new KeyValuePair<string, string>("InvalidRecipientThreadThreshold", recipientThreadThreshold.ToString("F0", NumberFormatInfo.InvariantInfo)));
				recipientThreadThreshold = double.NaN;
			}
			if (list.Count == 0)
			{
				list = null;
			}
			this.TrackData(this.recipientThrottleInfo, this.recipientThrottleInfoLock, recipient, isThrottle, ThrottlingScope.Recipient, ThrottlingResource.Threads, recipientThreadThreshold, ThrottlingImpactUnits.Recipients, 1U, orgID, recipient, mdbName, null, list);
		}

		public void TrackConcurrentMessageSizeThrottle(bool isThrottle, ulong concurrentMessageSizeThreshold, int numOfRecipients)
		{
			if (!this.deliveryThrottlingLog.Enabled)
			{
				return;
			}
			List<KeyValuePair<string, string>> list = null;
			if (numOfRecipients <= 0)
			{
				list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>("InvalidRecipientCount", numOfRecipients.ToString("D", NumberFormatInfo.InvariantInfo)));
				numOfRecipients = 1;
			}
			this.TrackData(this.concurrentMsgSizeThrottleInfo, this.concurrentMsgSizeThrottleInfoLock, "localhost", isThrottle, ThrottlingScope.MBServer, ThrottlingResource.Memory, concurrentMessageSizeThreshold, ThrottlingImpactUnits.Recipients, Convert.ToUInt32(numOfRecipients), Guid.Empty, string.Empty, string.Empty, null, list);
		}

		private void LogSummaryData(string sequenceNumber)
		{
			foreach (KeyValuePair<Dictionary<string, DeliveryThrottlingLogData>, ReaderWriterLockSlim> keyValuePair in this.resourceDicts)
			{
				this.LogSummaryDataPerDictionaryAndClean(keyValuePair.Key, keyValuePair.Value, sequenceNumber);
			}
		}

		private void LogSummaryDataPerDictionaryAndClean(Dictionary<string, DeliveryThrottlingLogData> dictionary, ReaderWriterLockSlim dictonaryLock, string sequenceNumber)
		{
			dictonaryLock.EnterWriteLock();
			try
			{
				foreach (KeyValuePair<string, DeliveryThrottlingLogData> keyValuePair in dictionary)
				{
					if (keyValuePair.Value.Impact > 0U)
					{
						this.DeliveryThrottlingLog.LogSummary(sequenceNumber, keyValuePair.Value.ThrottlingScope, keyValuePair.Value.ThrottlingResource, keyValuePair.Value.Threshold, keyValuePair.Value.ImpactUnits, keyValuePair.Value.Impact, Math.Round(keyValuePair.Value.Impact / (double)keyValuePair.Value.Total, 4, MidpointRounding.AwayFromZero), keyValuePair.Value.ExternalOrganizationId, keyValuePair.Value.Recipient, keyValuePair.Value.MDBName, keyValuePair.Value.MDBHealth, keyValuePair.Value.CustomData);
					}
				}
				dictionary.Clear();
			}
			finally
			{
				dictonaryLock.ExitWriteLock();
			}
		}

		private void TrackData(Dictionary<string, DeliveryThrottlingLogData> dictionary, ReaderWriterLockSlim dictionaryLock, string key, bool isThrottle, ThrottlingScope scope, ThrottlingResource resource, double threshold, ThrottlingImpactUnits impactUnits, uint impactDelta, Guid tenantID, string recipient, string mdbName, IList<KeyValuePair<string, double>> health, IList<KeyValuePair<string, string>> customData)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			if (dictionaryLock == null)
			{
				throw new ArgumentNullException("dictionaryLock");
			}
			dictionaryLock.EnterWriteLock();
			try
			{
				if (dictionary.ContainsKey(key))
				{
					if (isThrottle)
					{
						dictionary[key].Impact += impactDelta;
					}
					dictionary[key].Total += (long)((ulong)impactDelta);
					if (dictionary[key].MDBHealth == null && health != null && health.Count > 0)
					{
						dictionary[key].MDBHealth = health;
					}
					if (dictionary[key].CustomData == null && customData != null && customData.Count > 0)
					{
						dictionary[key].CustomData = customData;
					}
				}
				else
				{
					DeliveryThrottlingLogData value;
					if (isThrottle)
					{
						value = new DeliveryThrottlingLogData(scope, resource, threshold, impactUnits, impactDelta, (long)((ulong)impactDelta), tenantID, recipient, mdbName, health, customData);
					}
					else
					{
						value = new DeliveryThrottlingLogData(scope, resource, threshold, impactUnits, 0U, (long)((ulong)impactDelta), tenantID, recipient, mdbName, health, customData);
					}
					dictionary.Add(key, value);
				}
			}
			finally
			{
				dictionaryLock.ExitWriteLock();
			}
		}

		private const string MDBServer = "localhost";

		private const int NumOfResourceDictionaries = 6;

		private const string InvalidMDBName = "InvalidMDBName";

		private const string InvalidRecipient = "InvalidRecipient";

		private const string InvalidServerThreadThreshold = "InvalidServerThreadThreshold";

		private const string InvalidMDBResourceThreshold = "InvalidMDBResourceThreshold";

		private const string InvalidOrgID = "InvalidOrgID";

		private const string InvalidNumOfRecipients = "InvalidRecipientCount";

		private const string InvalidRecipientThreadThreshold = "InvalidRecipientThreadThreshold";

		private const string InvalidNullValue = "NULL";

		private const string InvalidEmptyValue = "EMPTY";

		private KeyValuePair<Dictionary<string, DeliveryThrottlingLogData>, ReaderWriterLockSlim>[] resourceDicts;

		private readonly Dictionary<string, DeliveryThrottlingLogData> serverThrottleInfo = new Dictionary<string, DeliveryThrottlingLogData>();

		private readonly ReaderWriterLockSlim serverThrottleInfoLock = new ReaderWriterLockSlim();

		private readonly Dictionary<string, DeliveryThrottlingLogData> mdbThrottleInfoDynamicThrottleDisabled = new Dictionary<string, DeliveryThrottlingLogData>();

		private readonly ReaderWriterLockSlim mdbThrottleInfoDynamicThrottleDisabledLock = new ReaderWriterLockSlim();

		private readonly Dictionary<string, DeliveryThrottlingLogData> mdbThrottleInfoPendingConnections = new Dictionary<string, DeliveryThrottlingLogData>();

		private readonly ReaderWriterLockSlim mdbThrottleInfoPendingConnectionsLock = new ReaderWriterLockSlim();

		private readonly Dictionary<string, DeliveryThrottlingLogData> mdbThrottleInfoTimeout = new Dictionary<string, DeliveryThrottlingLogData>();

		private readonly ReaderWriterLockSlim mdbThrottleInfoTimeoutLock = new ReaderWriterLockSlim();

		private readonly Dictionary<string, DeliveryThrottlingLogData> recipientThrottleInfo = new Dictionary<string, DeliveryThrottlingLogData>();

		private readonly ReaderWriterLockSlim recipientThrottleInfoLock = new ReaderWriterLockSlim();

		private readonly Dictionary<string, DeliveryThrottlingLogData> concurrentMsgSizeThrottleInfo = new Dictionary<string, DeliveryThrottlingLogData>();

		private readonly ReaderWriterLockSlim concurrentMsgSizeThrottleInfoLock = new ReaderWriterLockSlim();

		private readonly IDeliveryThrottlingLog deliveryThrottlingLog;
	}
}
