using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay.DatabaseCopyLayout
{
	internal class CopyLayoutGenerator
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.DatabaseCopyLayoutTracer;
			}
		}

		internal static Dictionary<int, List<DatabaseGroupLayoutEntry>> GenerateCopyLayoutTable(DatabaseCopyLayoutEntry copyLayoutEntry, int serversPerDag, int databasesPerDag, int databaseDrivesPerServer, int databasesPerDrive, int copiesPerDatabase, int numberOfExtraCopiesOnSpares = 0)
		{
			CopyLayoutGenerator.Tracer.TraceDebug(0L, "CopyLayoutGenerator: GenerateCopyLayoutTable() ServersPerDag = {0}, DatabasesPerDag = {1}, databaseDrivesPerServer = {2}, DatabasesPerDrive = {3}, CopiesPerDatabase = {4}, numberOfExtraCopiesOnSpares = {5}", new object[]
			{
				serversPerDag,
				databasesPerDag,
				databaseDrivesPerServer,
				databasesPerDrive,
				copiesPerDatabase,
				numberOfExtraCopiesOnSpares
			});
			Dictionary<int, List<DatabaseGroupLayoutEntry>> dictionary = CopyLayoutGenerator.GenerateCopyLayoutWithEmptySpares(copyLayoutEntry, serversPerDag, databasesPerDag, databaseDrivesPerServer, databasesPerDrive, copiesPerDatabase);
			if (numberOfExtraCopiesOnSpares > 0)
			{
				CopyLayoutGenerator.Tracer.TraceDebug<int>(0L, "CopyLayoutGenerator: GenerateCopyLayoutTable() Generating {0} extra copy on the spare disks.", numberOfExtraCopiesOnSpares);
				try
				{
					CopyLayoutGenerator.GenerateAdditionalCopyOnSpares(numberOfExtraCopiesOnSpares, databaseDrivesPerServer, dictionary);
				}
				catch (DatabaseCopyLayoutException ex)
				{
					CopyLayoutGenerator.Tracer.TraceDebug<string>(0L, "CopyLayoutGenerator: Call to GenerateAdditionalCopyOnSpares() failed. Error '{0}'.", ex.Message);
					throw new CallWithoutnumberOfExtraCopiesOnSparesException(ex.Message);
				}
			}
			return dictionary;
		}

		internal static Dictionary<int, List<DatabaseGroupLayoutEntry>> GenerateCopyLayoutWithEmptySpares(DatabaseCopyLayoutEntry copyLayoutEntry, int serversPerDag, int databasesPerDag, int databaseDrivesPerServer, int databasesPerDrive, int copiesPerDatabase)
		{
			CopyLayoutGenerator.Tracer.TraceDebug(0L, "CopyLayoutGenerator: GenerateCopyLayoutWithEmptySpares() ServersPerDag = {0}, DatabasesPerDag = {1}, databaseDrivesPerServer = {2}, DatabasesPerDrive = {3}, CopiesPerDatabase = {4}", new object[]
			{
				serversPerDag,
				databasesPerDag,
				databaseDrivesPerServer,
				databasesPerDrive,
				copiesPerDatabase
			});
			Dictionary<int, List<DatabaseGroupLayoutEntry>> dictionary = new Dictionary<int, List<DatabaseGroupLayoutEntry>>();
			Dictionary<string, int> databaseGroups = new Dictionary<string, int>();
			int num = serversPerDag / copiesPerDatabase;
			int num2 = databasesPerDag / databasesPerDrive;
			Math.Floor((double)num2 / (double)num);
			int num3 = (int)Math.Ceiling((double)num2 / (double)num);
			int num4 = num2 % num;
			int num5 = 0;
			int num6 = 0;
			int i = 0;
			int num7 = 0;
			for (int j = 0; j < num3; j++)
			{
				num7++;
				CopyLayoutGenerator.SeedStandardRand(num6 + i + j + 371293);
				for (i = 0; i <= copiesPerDatabase - 1; i++)
				{
					int num8 = i * num;
					int num9 = CopyLayoutGenerator.StandardRand();
					num6 = (num7 - 1) * num;
					int num10 = 371293 * (i + 1) + 1;
					if (num10 % num == 0 || CopyLayoutGenerator.GreatestCommonDivisor(num10, num) != 1)
					{
						num10 = 371293;
					}
					for (int k = 0; k < num; k++)
					{
						if (j != num3 - 1 || num4 <= 0 || k < num4)
						{
							int num11 = num8 + (k * num10 + num9) % num;
							num5++;
							DatabaseGroupLayoutEntry databaseGroupInstance = CopyLayoutGenerator.GetDatabaseGroupInstance(num6, databasesPerDrive, num5, copyLayoutEntry, databaseGroups);
							List<DatabaseGroupLayoutEntry> list;
							if (dictionary.TryGetValue(num11 + 1, out list))
							{
								list.Add(databaseGroupInstance);
							}
							else
							{
								List<DatabaseGroupLayoutEntry> list2 = new List<DatabaseGroupLayoutEntry>();
								list2.Add(databaseGroupInstance);
								dictionary.Add(num11 + 1, list2);
							}
							num6++;
						}
					}
				}
				i = copiesPerDatabase - 1;
			}
			return dictionary;
		}

		internal static void GenerateAdditionalCopyOnSpares(int numberOfExtraCopiesOnSpares, int databaseDrivesPerServer, Dictionary<int, List<DatabaseGroupLayoutEntry>> layoutTable)
		{
			CopyLayoutGenerator.Tracer.TraceDebug<int>(0L, "CopyLayoutGenerator: GenerateAdditionalCopyOnSpares() databaseDrivesPerServer = {0}", databaseDrivesPerServer);
			if (layoutTable == null || layoutTable.Count == 0)
			{
				CopyLayoutGenerator.Tracer.TraceError(0L, "CopyLayoutGenerator: GenerateAdditionalCopyOnSpares() CopylayoutTable passed in is null or empty.");
				throw new DatabaseCopyLayoutTableNullException();
			}
			int num = 0;
			int count = layoutTable.Count;
			int num2 = databaseDrivesPerServer * count;
			int num3 = layoutTable.Values.Sum((List<DatabaseGroupLayoutEntry> dbGroups) => dbGroups.Count);
			HashSet<DatabaseGroupLayoutEntry> hashSet = new HashSet<DatabaseGroupLayoutEntry>(layoutTable.Values.SelectMany((List<DatabaseGroupLayoutEntry> dbGroups) => dbGroups).ToList<DatabaseGroupLayoutEntry>(), DatabaseGroupEqualityComparer.Instance);
			int num4 = hashSet.Count * numberOfExtraCopiesOnSpares;
			int num5 = num2 - num3;
			if (num5 < num4)
			{
				CopyLayoutGenerator.Tracer.TraceError<int, int>(0L, "CopyLayoutGenerator: GenerateAdditionalCopyOnSpares() Insufficient spares. Required = {0}, available = {1}", num4, num5);
				throw new InsufficientSparesForExtraCopiesException(num5, num4);
			}
			CopyLayoutGenerator.Tracer.TraceDebug<int, int>(0L, "CopyLayoutGenerator: GenerateAdditionalCopyOnSpares() additionalCopyCount = {0}, availableSpares = {1}", num4, num5);
			for (int i = 1; i <= numberOfExtraCopiesOnSpares; i++)
			{
				foreach (DatabaseGroupLayoutEntry item in hashSet)
				{
					bool flag = false;
					for (int j = 0; j < count; j++)
					{
						if (num >= count)
						{
							num = 1;
						}
						else
						{
							num++;
						}
						if (layoutTable[num].Count < databaseDrivesPerServer && !layoutTable[num].Contains(item))
						{
							DatabaseGroupLayoutEntry item2 = new DatabaseGroupLayoutEntry(item.DatabaseGroupId, item.DatabaseNameList, true);
							layoutTable[num].Add(item2);
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						throw new SpareConflictInLayoutException(num5);
					}
				}
			}
		}

		private static DatabaseGroupLayoutEntry GetDatabaseGroupInstance(int databaseInstance, int databasesPerDrive, int defaultGroupNumber, DatabaseCopyLayoutEntry copyLayoutEntry, Dictionary<string, int> databaseGroups)
		{
			CopyLayoutGenerator.Tracer.TraceDebug<int, int, int>(0L, "CopyLayoutGenerator: GetDatabaseGroupInstance() databaseInstance = {0}, databasesPerDrive = {1}, defaultGroupNumber = {2}", databaseInstance, databasesPerDrive, defaultGroupNumber);
			List<string> list = new List<string>();
			for (int i = 0; i < databasesPerDrive; i++)
			{
				int databaseNumber = databaseInstance * databasesPerDrive + i + 1;
				list.Add(copyLayoutEntry.GetDatabaseName(databaseNumber));
			}
			int num;
			DatabaseGroupLayoutEntry result;
			if (databaseGroups.TryGetValue(list.FirstOrDefault<string>(), out num))
			{
				result = new DatabaseGroupLayoutEntry(num.ToString(), list, false);
			}
			else
			{
				if (databaseGroups.Count != 0)
				{
					defaultGroupNumber = databaseGroups.Values.Max() + 1;
				}
				result = new DatabaseGroupLayoutEntry(defaultGroupNumber.ToString(), list, false);
				foreach (string key in list)
				{
					databaseGroups.Add(key, defaultGroupNumber);
				}
			}
			CopyLayoutGenerator.Tracer.TraceDebug<string, string>(0L, "CopyLayoutGenerator: GetDatabaseGroupInstance() Returned database groupid - {0} databaseNames - {1}", result.DatabaseGroupId, result.DatabaseNameList.ToString());
			return result;
		}

		private static void SeedStandardRand(int seed)
		{
			if (seed != 0)
			{
				CopyLayoutGenerator.m_gStandardRandSeed = seed;
				return;
			}
			CopyLayoutGenerator.m_gStandardRandSeed = 1;
		}

		private static int StandardRand()
		{
			int num = 16807;
			int maxValue = int.MaxValue;
			int num2 = 127773;
			int num3 = 2836;
			int num4 = (int)Math.Round((double)CopyLayoutGenerator.m_gStandardRandSeed / (double)num2, MidpointRounding.AwayFromZero);
			int num5 = CopyLayoutGenerator.m_gStandardRandSeed % num2;
			int num6 = num * num5 - num3 * num4;
			if (0 <= num6)
			{
				CopyLayoutGenerator.m_gStandardRandSeed = num6;
			}
			else
			{
				CopyLayoutGenerator.m_gStandardRandSeed = num6 + maxValue;
			}
			return CopyLayoutGenerator.m_gStandardRandSeed - 1;
		}

		private static int GreatestCommonDivisor(int number1, int number2)
		{
			while (number2 != 0)
			{
				int num = number2;
				number2 = number1 % number2;
				number1 = num;
			}
			return number1;
		}

		private const int m_stride = 371293;

		private static int m_gStandardRandSeed;
	}
}
