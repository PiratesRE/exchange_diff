using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SnapIn
{
	public class CommunicationChannelCollection
	{
		public MMCCommunicationChannel this[string key]
		{
			get
			{
				if (!this.channels.ContainsKey(key))
				{
					throw new IndexOutOfRangeException();
				}
				return this.channels[key];
			}
		}

		public void Add(string key, MMCCommunicationChannel channel)
		{
			this.channels.Add(key, channel);
		}

		public string LocalOnPremiseKey { get; set; }

		public bool AllInitiated
		{
			get
			{
				bool result = true;
				foreach (string key in this.channels.Keys)
				{
					if (!this.channels[key].Initiated)
					{
						result = false;
						break;
					}
				}
				return result;
			}
		}

		public bool ContainsKey(string key)
		{
			return this.channels.ContainsKey(key);
		}

		public Dictionary<string, MMCCommunicationChannel>.KeyCollection Keys
		{
			get
			{
				return this.channels.Keys;
			}
		}

		private Dictionary<string, MMCCommunicationChannel> channels = new Dictionary<string, MMCCommunicationChannel>();
	}
}
