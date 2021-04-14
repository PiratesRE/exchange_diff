using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class IOStatistics
	{
		public Dictionary<string, TableLevelIOStats> IOStats
		{
			get
			{
				return this.tableLevelIOStats;
			}
		}

		public void AddStats(string message)
		{
			string[] array = message.Split(new char[]
			{
				'.'
			});
			string text = array[0].Substring(array[0].IndexOf('\''));
			array = array[1].Split(new char[]
			{
				','
			});
			int num = int.Parse(array[1].Substring(array[1].LastIndexOf(' ')));
			int num2 = int.Parse(array[2].Substring(array[2].LastIndexOf(' ')));
			int num3 = int.Parse(array[3].Substring(array[3].LastIndexOf(' ')));
			int num4 = int.Parse(array[4].Substring(array[4].LastIndexOf(' ')));
			int num5 = int.Parse(array[5].Substring(array[5].LastIndexOf(' ')));
			int num6 = int.Parse(array[6].Substring(array[6].LastIndexOf(' ')));
			TableLevelIOStats tableLevelIOStats = null;
			if (!this.tableLevelIOStats.TryGetValue(text, out tableLevelIOStats))
			{
				tableLevelIOStats = new TableLevelIOStats(text);
				this.tableLevelIOStats.Add(text, tableLevelIOStats);
			}
			tableLevelIOStats.LogicalReads += num;
			tableLevelIOStats.PhysicalReads += num2;
			tableLevelIOStats.ReadAheads += num3;
			tableLevelIOStats.LobLogicalReads += num4;
			tableLevelIOStats.LobPhysicalReads += num5;
			tableLevelIOStats.LobReadAheads += num6;
		}

		public void ResetAll()
		{
			foreach (TableLevelIOStats tableLevelIOStats in this.tableLevelIOStats.Values)
			{
				tableLevelIOStats.Reset();
			}
		}

		private const int AverageTablesPerConnection = 10;

		private Dictionary<string, TableLevelIOStats> tableLevelIOStats = new Dictionary<string, TableLevelIOStats>(10);
	}
}
