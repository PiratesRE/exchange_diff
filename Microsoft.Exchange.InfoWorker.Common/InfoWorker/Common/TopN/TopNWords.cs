using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.TopN
{
	internal class TopNWords
	{
		internal TopNWords(MailboxSession mailboxSession)
		{
			this.mailboxSession = mailboxSession;
			this.config = new TopNConfiguration(mailboxSession);
		}

		internal MailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
		}

		internal int Version
		{
			get
			{
				if (this.version == 0)
				{
					this.GetMetaData();
				}
				return this.version;
			}
		}

		internal ExDateTime LastScanTime
		{
			get
			{
				if (this.lastScanTime == null)
				{
					this.GetMetaData();
				}
				return this.lastScanTime.Value;
			}
		}

		internal List<KeyValuePair<string, int>> WordList
		{
			get
			{
				if (this.wordList == null)
				{
					this.wordList = this.GetWordList();
				}
				return this.wordList;
			}
		}

		private void GetMetaData()
		{
			this.config.ReadMetaData();
			this.version = this.config.Version;
			this.lastScanTime = new ExDateTime?(this.config.LastScanTime);
		}

		private List<KeyValuePair<string, int>> GetWordList()
		{
			List<KeyValuePair<string, int>> list = null;
			if (this.config.ReadWordFrequencyMap())
			{
				if (this.config.WordFrequency == null || this.config.WordFrequency.Length == 0)
				{
					return null;
				}
				list = new List<KeyValuePair<string, int>>(10);
				foreach (KeyValuePair<string, int> keyValuePair in this.config.WordFrequency)
				{
					list.Add(new KeyValuePair<string, int>(keyValuePair.Key, keyValuePair.Value));
				}
			}
			if (list == null || this.config.LastScanTime + TopNConfiguration.UpdateInterval < ExDateTime.Now)
			{
				this.config.ScanRequested = true;
				this.config.Save(true);
			}
			return list;
		}

		private MailboxSession mailboxSession;

		private TopNConfiguration config;

		private int version;

		private ExDateTime? lastScanTime;

		private List<KeyValuePair<string, int>> wordList;
	}
}
