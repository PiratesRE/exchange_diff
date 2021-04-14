using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public class ScenarioData : ConcurrentDictionary<string, string>, IDisposable
	{
		public ScenarioData()
		{
			base["SID"] = Guid.NewGuid().ToString();
			base["S"] = "DEF";
			this.AssignOnThread();
		}

		public ScenarioData(IEnumerable<KeyValuePair<string, string>> dictionary) : base(dictionary)
		{
			this.AssignOnThread();
		}

		public static ScenarioData Current
		{
			get
			{
				if (ScenarioData.current == null)
				{
					ScenarioData.current = new ScenarioData();
				}
				return ScenarioData.current;
			}
		}

		public static ScenarioData FromString(string data)
		{
			return new ScenarioData(UserAgentSerializer.FromUserAgent(data));
		}

		public override string ToString()
		{
			return UserAgentSerializer.ToUserAgent(this);
		}

		public bool Remove(string key)
		{
			string text;
			return base.TryRemove(key, out text);
		}

		public void Dispose()
		{
			if (ScenarioData.current == this)
			{
				ScenarioData.current = null;
			}
		}

		private void AssignOnThread()
		{
			if (ScenarioData.current == null)
			{
				ScenarioData.current = this;
			}
		}

		[ThreadStatic]
		private static ScenarioData current;
	}
}
