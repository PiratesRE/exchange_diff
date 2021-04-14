using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class PageToReaderTaskMapping : Dictionary<string, List<string>>
	{
		public PageToReaderTaskMapping(IList<ReaderTaskProfile> list, Dictionary<string, List<string>> pageToObjectMapping)
		{
			using (Dictionary<string, List<string>>.KeyCollection.Enumerator enumerator = pageToObjectMapping.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string key = enumerator.Current;
					this.pageExecutionStatus.Add(key, false);
					base.Add(key, (from c in list
					where pageToObjectMapping[key].Contains(c.DataObjectName)
					select c.Name).ToList<string>());
					this.isReadOnDemand = true;
				}
			}
		}

		public bool IsExecuted(string page)
		{
			if (!this.isReadOnDemand)
			{
				return this.allTasksExecuted;
			}
			return page != null && this.pageExecutionStatus.ContainsKey(page) && this.pageExecutionStatus[page];
		}

		public void Execute(string page)
		{
			if (!this.isReadOnDemand)
			{
				this.allTasksExecuted = true;
				return;
			}
			this.pageExecutionStatus[page] = true;
		}

		public void Reset()
		{
			List<string> list = this.pageExecutionStatus.Keys.ToList<string>();
			foreach (string key in list)
			{
				this.pageExecutionStatus[key] = false;
			}
			this.allTasksExecuted = false;
		}

		public bool CanTaskExecuted(string pageName, string taskName)
		{
			if (!this.isReadOnDemand)
			{
				return !this.allTasksExecuted;
			}
			return base[pageName].Contains(taskName) && (from c in this.pageExecutionStatus
			where c.Value && this[c.Key].Contains(taskName)
			select c).Count<KeyValuePair<string, bool>>() == 0;
		}

		private Dictionary<string, bool> pageExecutionStatus = new Dictionary<string, bool>();

		private bool isReadOnDemand;

		private bool allTasksExecuted;
	}
}
