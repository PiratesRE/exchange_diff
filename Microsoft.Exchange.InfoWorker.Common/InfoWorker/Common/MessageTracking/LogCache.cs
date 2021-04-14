using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class LogCache
	{
		public LogCache(DateTime startTime, DateTime endTime, TrackingEventBudget eventBudget)
		{
			this.startTime = startTime;
			this.endTime = endTime;
			this.eventBudget = eventBudget;
			this.cache = new Dictionary<Fqdn, Dictionary<string, List<MessageTrackingLogEntry>[]>>();
			this.cacheBySender = new Dictionary<Fqdn, Dictionary<ProxyAddressCollection, List<MessageTrackingLogEntry>[]>>();
		}

		public List<MessageTrackingLogEntry> GetMessageLog(RpcReason rpcReason, ILogReader reader, TrackingLogPrefix logPrefix, string messageId, long mailItemId)
		{
			List<MessageTrackingLogEntry> messageLog = this.GetMessageLog(rpcReason, reader, logPrefix, messageId);
			List<MessageTrackingLogEntry> list = new List<MessageTrackingLogEntry>();
			if (messageLog == null)
			{
				return list;
			}
			foreach (MessageTrackingLogEntry messageTrackingLogEntry in messageLog)
			{
				if (mailItemId == messageTrackingLogEntry.ServerLogKeyMailItemId)
				{
					list.Add(messageTrackingLogEntry);
				}
			}
			return list;
		}

		public List<MessageTrackingLogEntry> GetMessageLog(RpcReason rpcReason, ILogReader reader, TrackingLogPrefix logPrefix, string messageId, MessageTrackingSource? sourceFilter, HashSet<MessageTrackingEvent> eventIdFilterSet)
		{
			List<MessageTrackingLogEntry> messageLog = this.GetMessageLog(rpcReason, reader, logPrefix, messageId);
			List<MessageTrackingLogEntry> list = new List<MessageTrackingLogEntry>();
			if (messageLog == null)
			{
				return list;
			}
			foreach (MessageTrackingLogEntry messageTrackingLogEntry in messageLog)
			{
				if ((sourceFilter == null || !(messageTrackingLogEntry.Source != sourceFilter)) && eventIdFilterSet.Contains(messageTrackingLogEntry.EventId))
				{
					list.Add(messageTrackingLogEntry);
				}
			}
			return list;
		}

		public List<MessageTrackingLogEntry> GetMessageLog(RpcReason rpcReason, ILogReader reader, TrackingLogPrefix logPrefix, ProxyAddressCollection senderAddresses, MessageTrackingSource eventSource, HashSet<MessageTrackingEvent> eventIdFilterSet, ProxyAddressCollection[] recipientAddresses, string messageId)
		{
			List<MessageTrackingLogEntry> list = new List<MessageTrackingLogEntry>();
			string text = null;
			bool flag = false;
			if (senderAddresses == null && string.IsNullOrEmpty(messageId))
			{
				throw new ArgumentException("Either senderAddresses or messageId must be specified to use log cache");
			}
			if (!string.IsNullOrEmpty(messageId))
			{
				text = TrackingSearch.RemoveAngleBracketsIfNeeded(messageId);
			}
			List<MessageTrackingLogEntry> messageLog;
			if (!string.IsNullOrEmpty(text))
			{
				messageLog = this.GetMessageLog(rpcReason, reader, logPrefix, text);
				flag = true;
			}
			else
			{
				messageLog = this.GetMessageLog(rpcReason, reader, logPrefix, senderAddresses);
			}
			foreach (MessageTrackingLogEntry messageTrackingLogEntry in messageLog)
			{
				if (messageTrackingLogEntry.Source == eventSource && (eventIdFilterSet == null || eventIdFilterSet.Contains(messageTrackingLogEntry.EventId)) && (!flag || senderAddresses == null || LogCache.AtLeastOneAddressFoundInProxy(new string[]
				{
					messageTrackingLogEntry.SenderAddress
				}, senderAddresses)) && (recipientAddresses == null || LogCache.AtLeastOneAddressFoundInProxies(messageTrackingLogEntry.RecipientAddresses, recipientAddresses)))
				{
					list.Add(messageTrackingLogEntry);
				}
			}
			return list;
		}

		private static bool AtLeastOneAddressFoundInProxies(string[] addresses, ProxyAddressCollection[] proxies)
		{
			foreach (ProxyAddressCollection proxyCollection in proxies)
			{
				if (LogCache.AtLeastOneAddressFoundInProxy(addresses, proxyCollection))
				{
					return true;
				}
			}
			return false;
		}

		private static bool AtLeastOneAddressFoundInProxy(string[] addresses, ProxyAddressCollection proxyCollection)
		{
			foreach (ProxyAddress proxyAddress in proxyCollection)
			{
				if (!(proxyAddress.Prefix != ProxyAddressPrefix.Smtp))
				{
					foreach (string b in addresses)
					{
						if (string.Equals(proxyAddress.AddressString, b, StringComparison.OrdinalIgnoreCase))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private List<MessageTrackingLogEntry> GetMessageLog(RpcReason rpcReason, ILogReader logReader, TrackingLogPrefix logPrefix, string messageId)
		{
			Fqdn key = Fqdn.Parse(logReader.Server);
			Dictionary<string, List<MessageTrackingLogEntry>[]> dictionary = null;
			List<MessageTrackingLogEntry>[] array = null;
			if (!this.cache.TryGetValue(key, out dictionary))
			{
				dictionary = new Dictionary<string, List<MessageTrackingLogEntry>[]>();
				this.cache.Add(key, dictionary);
			}
			if (!dictionary.TryGetValue(messageId, out array))
			{
				array = new List<MessageTrackingLogEntry>[LogCache.prefixValues.Length];
				for (int i = 0; i < LogCache.prefixValues.Length; i++)
				{
					List<MessageTrackingLogEntry> list = logReader.ReadLogs(rpcReason, LogCache.prefixValues[i], messageId, this.startTime, this.endTime, this.eventBudget);
					if (logReader.MtrSchemaVersion >= MtrSchemaVersion.E15RTM)
					{
						list.RemoveAll((MessageTrackingLogEntry entry) => entry.EventId == MessageTrackingEvent.RECEIVE && entry.Source == MessageTrackingSource.STOREDRIVER);
						list.RemoveAll((MessageTrackingLogEntry entry) => entry.EventId == MessageTrackingEvent.HADISCARD && entry.Source == MessageTrackingSource.SMTP);
						list.RemoveAll((MessageTrackingLogEntry entry) => entry.EventId == MessageTrackingEvent.AGENTINFO && entry.Source == MessageTrackingSource.AGENT);
					}
					array[i] = list;
				}
				dictionary.Add(messageId, array);
			}
			return array[(int)logPrefix];
		}

		private List<MessageTrackingLogEntry> GetMessageLog(RpcReason rpcReason, ILogReader logReader, TrackingLogPrefix logPrefix, ProxyAddressCollection senderAddresses)
		{
			Fqdn key = Fqdn.Parse(logReader.Server);
			Dictionary<ProxyAddressCollection, List<MessageTrackingLogEntry>[]> dictionary = null;
			List<MessageTrackingLogEntry>[] array = null;
			if (!this.cacheBySender.TryGetValue(key, out dictionary))
			{
				dictionary = new Dictionary<ProxyAddressCollection, List<MessageTrackingLogEntry>[]>(1, LogCache.proxyAddressCollectionComparer);
				this.cacheBySender.Add(key, dictionary);
			}
			if (!dictionary.TryGetValue(senderAddresses, out array))
			{
				array = new List<MessageTrackingLogEntry>[LogCache.prefixValues.Length];
				for (int i = 0; i < LogCache.prefixValues.Length; i++)
				{
					List<MessageTrackingLogEntry> list = logReader.ReadLogs(rpcReason, LogCache.prefixValues[i], senderAddresses, this.startTime, this.endTime, this.eventBudget);
					array[i] = list;
					Dictionary<string, List<MessageTrackingLogEntry>[]> dictionary2 = null;
					bool flag = !this.cache.TryGetValue(key, out dictionary2);
					HashSet<string> hashSet = new HashSet<string>();
					foreach (MessageTrackingLogEntry messageTrackingLogEntry in list)
					{
						if (!flag && !dictionary2.ContainsKey(messageTrackingLogEntry.MessageId) && !hashSet.Contains(messageTrackingLogEntry.MessageId))
						{
							hashSet.Add(messageTrackingLogEntry.MessageId);
						}
					}
					if (flag)
					{
						dictionary2 = new Dictionary<string, List<MessageTrackingLogEntry>[]>(hashSet.Count);
						this.cache.Add(key, dictionary2);
					}
					foreach (MessageTrackingLogEntry messageTrackingLogEntry2 in list)
					{
						if (hashSet.Contains(messageTrackingLogEntry2.MessageId))
						{
							List<MessageTrackingLogEntry>[] array2;
							if (!dictionary2.TryGetValue(messageTrackingLogEntry2.MessageId, out array2))
							{
								array2 = new List<MessageTrackingLogEntry>[LogCache.prefixValues.Length];
								dictionary2.Add(messageTrackingLogEntry2.MessageId, array2);
							}
							if (array2[i] == null)
							{
								array2[i] = new List<MessageTrackingLogEntry>();
							}
							array2[i].Add(messageTrackingLogEntry2);
						}
					}
				}
				dictionary.Add(senderAddresses, array);
			}
			return array[(int)logPrefix];
		}

		private static readonly ProxyAddressCollectionComparer proxyAddressCollectionComparer = new ProxyAddressCollectionComparer();

		private static string[] prefixValues = Enum.GetNames(typeof(TrackingLogPrefix));

		private DateTime startTime;

		private DateTime endTime;

		private Dictionary<Fqdn, Dictionary<string, List<MessageTrackingLogEntry>[]>> cache;

		private Dictionary<Fqdn, Dictionary<ProxyAddressCollection, List<MessageTrackingLogEntry>[]>> cacheBySender;

		private TrackingEventBudget eventBudget;
	}
}
