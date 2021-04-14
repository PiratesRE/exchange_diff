using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CmdletLogEntries
	{
		internal CmdletLogEntries()
		{
			this.InitCmdletLogEntries();
		}

		internal void AddEntry(string entry)
		{
			this.logEntries[this.currentIndentation].Add(entry);
		}

		internal void ClearAllEntries()
		{
			this.InitCmdletLogEntries();
		}

		internal void ClearCurrentIndentationEntries()
		{
			this.logEntries[this.currentIndentation].Clear();
		}

		internal void IncreaseIndentation()
		{
			this.currentIndentation++;
			this.logEntries.Add(this.currentIndentation, new List<string>());
		}

		internal void DecreaseIndentation()
		{
			this.ClearCurrentIndentationEntries();
			this.logEntries.Remove(this.currentIndentation--);
			if (this.currentIndentation < 0)
			{
				throw new InvalidOperationException("Cannot decrease Indentation below 0");
			}
		}

		internal IEnumerable<string> GetAllEntries()
		{
			for (int i = 0; i < this.currentIndentation + 1; i++)
			{
				for (int j = 0; j < this.logEntries[i].Count; j++)
				{
					yield return this.logEntries[i][j];
				}
			}
			yield break;
		}

		internal IEnumerable<string> GetCurrentIndentationEntries()
		{
			for (int i = 0; i < this.logEntries[this.currentIndentation].Count; i++)
			{
				yield return this.logEntries[this.currentIndentation][i];
			}
			yield break;
		}

		private void InitCmdletLogEntries()
		{
			this.currentIndentation = 0;
			this.logEntries = new Dictionary<int, List<string>>();
			this.logEntries.Add(this.currentIndentation, new List<string>());
		}

		private Dictionary<int, List<string>> logEntries;

		private int currentIndentation;
	}
}
