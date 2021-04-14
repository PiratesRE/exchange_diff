using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	internal class PiiMapManager
	{
		public static PiiMapManager Instance
		{
			get
			{
				if (PiiMapManager.instance == null)
				{
					lock (PiiMapManager.syncObject)
					{
						if (PiiMapManager.instance == null)
						{
							PiiMapManager.instance = new PiiMapManager();
						}
					}
				}
				return PiiMapManager.instance;
			}
		}

		private PiiMapManager()
		{
		}

		public static bool ContainsRedactedPiiValue(string value)
		{
			return !string.IsNullOrEmpty(value) && PiiMapManager.detectRedactedPii.IsMatch(value);
		}

		public string ResolveRedactedValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}
			string text = value.Replace(SuppressingPiiConfig.RedactedDataPrefix, string.Empty);
			string text2 = PiiMapManager.detectRedactedPii.Replace(text, (Match x) => this.Lookup(x.Value));
			if (!(text2 == text))
			{
				return text2;
			}
			return value;
		}

		public PiiMap GetOrAdd(string session)
		{
			if (string.IsNullOrEmpty(session))
			{
				throw new ArgumentException("User name should not be null or empty");
			}
			return this.piiMaps.GetOrAdd(session, (string x) => new PiiMap());
		}

		public void Remove(string session)
		{
			if (string.IsNullOrEmpty(session))
			{
				throw new ArgumentException("Session ID should not be null or empty");
			}
			PiiMap piiMap;
			this.piiMaps.TryRemove(session, out piiMap);
		}

		private string Lookup(string redactedValue)
		{
			if (string.IsNullOrEmpty(redactedValue))
			{
				return redactedValue;
			}
			string text = null;
			foreach (PiiMap piiMap in from x in this.piiMaps.Values
			where x != null
			select x)
			{
				text = piiMap[redactedValue];
				if (text != null)
				{
					break;
				}
			}
			if (text == null)
			{
				return redactedValue;
			}
			return text;
		}

		private static readonly Regex detectRedactedPii = new Regex("[0-9A-Fa-f]{32}", RegexOptions.Compiled);

		private ConcurrentDictionary<string, PiiMap> piiMaps = new ConcurrentDictionary<string, PiiMap>();

		private static PiiMapManager instance;

		private static object syncObject = new object();
	}
}
