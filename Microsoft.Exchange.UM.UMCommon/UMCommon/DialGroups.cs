using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.UM.UMCommon
{
	public class DialGroups
	{
		public DialGroups()
		{
			this.dialGroups = new Dictionary<string, List<DialGroupEntry>>(StringComparer.OrdinalIgnoreCase);
		}

		public Dictionary<string, List<DialGroupEntry>> DialPermissionGroups
		{
			get
			{
				return this.dialGroups;
			}
		}

		public static bool HaveIntersection(MultiValuedProperty<DialGroupEntry> configuredEntries, MultiValuedProperty<string> selectedEntries)
		{
			if (configuredEntries.Count == 0 || selectedEntries.Count == 0)
			{
				return false;
			}
			DialGroups dialGroups = new DialGroups();
			dialGroups.Add(configuredEntries);
			List<DialGroupEntry> list = null;
			foreach (string name in selectedEntries)
			{
				bool flag = dialGroups.TryGetValue(name, out list);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public List<DialGroupEntry> Get(string name)
		{
			return this.dialGroups[name];
		}

		public bool TryGetValue(string name, out List<DialGroupEntry> entryList)
		{
			return this.dialGroups.TryGetValue(name, out entryList);
		}

		public void Add(MultiValuedProperty<DialGroupEntry> entries)
		{
			if (entries == null)
			{
				return;
			}
			foreach (DialGroupEntry dg in entries)
			{
				this.Add(dg);
			}
		}

		internal void Set(string name, List<DialGroupEntry> rules)
		{
			this.dialGroups[name] = rules;
		}

		internal void Add(DialGroupEntry dg)
		{
			if (dg == null)
			{
				return;
			}
			List<DialGroupEntry> list = null;
			if (!this.dialGroups.TryGetValue(dg.Name, out list))
			{
				list = new List<DialGroupEntry>();
				this.dialGroups[dg.Name] = list;
			}
			list.Add(dg);
		}

		internal void Add(DialGroupEntry[] entries)
		{
			if (entries == null)
			{
				return;
			}
			foreach (DialGroupEntry dg in entries)
			{
				this.Add(dg);
			}
		}

		private Dictionary<string, List<DialGroupEntry>> dialGroups;
	}
}
