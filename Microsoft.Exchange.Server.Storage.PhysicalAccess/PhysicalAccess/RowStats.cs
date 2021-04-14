using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public struct RowStats : IEquatable<RowStats>
	{
		internal RowStats(string inputString)
		{
			this.counters = RowStats.ParseFromString(inputString);
		}

		public int ReadTotal
		{
			get
			{
				return this.GetTotalCounter(RowStatsCounterType.Read);
			}
		}

		public int SeekTotal
		{
			get
			{
				return this.GetTotalCounter(RowStatsCounterType.Seek);
			}
		}

		public int AcceptTotal
		{
			get
			{
				return this.GetTotalCounter(RowStatsCounterType.Accept);
			}
		}

		public int WriteTotal
		{
			get
			{
				return this.GetTotalCounter(RowStatsCounterType.Write);
			}
		}

		public int ReadBytesTotal
		{
			get
			{
				return this.GetTotalCounter(RowStatsCounterType.ReadBytes);
			}
		}

		public int WriteBytesTotal
		{
			get
			{
				return this.GetTotalCounter(RowStatsCounterType.WriteBytes);
			}
		}

		public bool IsEmpty
		{
			get
			{
				if (this.counters != null)
				{
					foreach (int num in this.counters)
					{
						if (num != 0)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		internal static bool AppendCounterToString(TraceContentBuilder cb, string name, int counter, bool continueList)
		{
			if (counter != 0)
			{
				if (continueList)
				{
					cb.Append(" ");
				}
				cb.Append(name);
				cb.Append(":");
				cb.Append((uint)counter);
				return true;
			}
			return continueList;
		}

		private static bool DumpOneCounter(int currentValue, int dumpedValue, ExPerformanceCounter perfCounter)
		{
			if (currentValue != dumpedValue)
			{
				perfCounter.IncrementBy((long)((ulong)(currentValue - dumpedValue)));
				return true;
			}
			return false;
		}

		private static RowStatsTableClassIndex GetTableClassIndex(TableClass tableClass)
		{
			if (tableClass >= TableClass.PseudoIndexControl)
			{
				return RowStatsTableClassIndex.Other;
			}
			return (RowStatsTableClassIndex)tableClass;
		}

		private static int GetCounterIndex(RowStatsTableClassIndex tableClassIndex, RowStatsCounterType counterType)
		{
			return (int)(tableClassIndex * RowStatsTableClassIndex.Folder + (int)counterType);
		}

		private static int GetCounterIndex(TableClass tableClass, RowStatsCounterType counterType)
		{
			return RowStats.GetCounterIndex(RowStats.GetTableClassIndex(tableClass), counterType);
		}

		private static int[] ParseFromString(string inputString)
		{
			int[] array = new int[54];
			Regex regex = new Regex("(?<table>\\w+):\\[(r:(?<read>\\d+))?\\s*(s:(?<seek>\\d+))?\\s*(a:(?<accept>\\d+))?\\s*(w:(?<write>\\d+))?\\s*(rb:(?<readBytes>\\d+))?\\s*(wb:(?<writeBytes>\\d+))?\\]");
			Match match = Regex.Match(inputString, "\\w+:\\[[^\\]]+\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			while (match.Success)
			{
				Match match2 = regex.Match(match.Groups[0].Value);
				if (match2.Success)
				{
					RowStatsTableClassIndex tableClassIndex = RowStatsTableClassIndex.Other;
					string key;
					switch (key = match2.Groups["table"].Value.ToUpper())
					{
					case "TF":
						tableClassIndex = RowStatsTableClassIndex.TableFunction;
						break;
					case "LI":
						tableClassIndex = RowStatsTableClassIndex.LazyIndex;
						break;
					case "MSG":
						tableClassIndex = RowStatsTableClassIndex.Message;
						break;
					case "ATT":
						tableClassIndex = RowStatsTableClassIndex.Attachment;
						break;
					case "FLD":
						tableClassIndex = RowStatsTableClassIndex.Folder;
						break;
					case "PIM":
						tableClassIndex = RowStatsTableClassIndex.PseudoIndexMaintenance;
						break;
					case "EVT":
						tableClassIndex = RowStatsTableClassIndex.Events;
						break;
					case "OTH":
						tableClassIndex = RowStatsTableClassIndex.Other;
						break;
					case "TMP":
						tableClassIndex = RowStatsTableClassIndex.Temp;
						break;
					}
					int num2 = match2.Groups["read"].Success ? int.Parse(match2.Groups["read"].Value) : 0;
					int num3 = match2.Groups["seek"].Success ? int.Parse(match2.Groups["seek"].Value) : 0;
					int num4 = match2.Groups["accept"].Success ? int.Parse(match2.Groups["accept"].Value) : 0;
					int num5 = match2.Groups["write"].Success ? int.Parse(match2.Groups["write"].Value) : 0;
					int num6 = match2.Groups["readBytes"].Success ? int.Parse(match2.Groups["readBytes"].Value) : 0;
					int num7 = match2.Groups["writeBytes"].Success ? int.Parse(match2.Groups["writeBytes"].Value) : 0;
					array[RowStats.GetCounterIndex(tableClassIndex, RowStatsCounterType.Read)] = num2;
					array[RowStats.GetCounterIndex(tableClassIndex, RowStatsCounterType.Seek)] = num3;
					array[RowStats.GetCounterIndex(tableClassIndex, RowStatsCounterType.Accept)] = num4;
					array[RowStats.GetCounterIndex(tableClassIndex, RowStatsCounterType.Write)] = num5;
					array[RowStats.GetCounterIndex(tableClassIndex, RowStatsCounterType.ReadBytes)] = num6;
					array[RowStats.GetCounterIndex(tableClassIndex, RowStatsCounterType.WriteBytes)] = num7;
				}
				match = match.NextMatch();
			}
			return array;
		}

		public void Initialize()
		{
			this.counters = new int[54];
		}

		public void Reset()
		{
			Array.Clear(this.counters, 0, this.counters.Length);
		}

		public void IncrementCount(TableClass tableClass, RowStatsCounterType counterType)
		{
			this.counters[RowStats.GetCounterIndex(tableClass, counterType)]++;
		}

		public void AddCount(TableClass tableClass, RowStatsCounterType counterType, int value)
		{
			this.counters[RowStats.GetCounterIndex(tableClass, counterType)] += value;
		}

		public override bool Equals(object other)
		{
			return other is RowStats && this.Equals((RowStats)other);
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (this.counters != null)
			{
				for (int i = 0; i < this.counters.Length; i++)
				{
					num ^= this.counters[i];
				}
			}
			return num;
		}

		public int GetCounter(RowStatsTableClassIndex tableClassIndex, RowStatsCounterType counterType)
		{
			return this.counters[RowStats.GetCounterIndex(tableClassIndex, counterType)];
		}

		public bool Equals(RowStats other)
		{
			return ValueHelper.ArraysEqual<int>(this.counters, other.counters);
		}

		internal bool DumpStats(PhysicalAccessPerformanceCountersInstance perfInstance, RowStats dumpedStats)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			flag |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Other, RowStatsCounterType.Read), dumpedStats.GetCounter(RowStatsTableClassIndex.Other, RowStatsCounterType.Read), perfInstance.OtherRowsReadRate);
			flag2 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Other, RowStatsCounterType.Seek), dumpedStats.GetCounter(RowStatsTableClassIndex.Other, RowStatsCounterType.Seek), perfInstance.OtherRowsSeekRate);
			flag3 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Other, RowStatsCounterType.Accept), dumpedStats.GetCounter(RowStatsTableClassIndex.Other, RowStatsCounterType.Accept), perfInstance.OtherRowsAcceptRate);
			flag4 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Other, RowStatsCounterType.Write), dumpedStats.GetCounter(RowStatsTableClassIndex.Other, RowStatsCounterType.Write), perfInstance.OtherRowsWriteRate);
			flag5 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Other, RowStatsCounterType.ReadBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.Other, RowStatsCounterType.ReadBytes), perfInstance.OtherBytesReadRate);
			flag6 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Other, RowStatsCounterType.WriteBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.Other, RowStatsCounterType.WriteBytes), perfInstance.OtherBytesWriteRate);
			flag |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Temp, RowStatsCounterType.Read), dumpedStats.GetCounter(RowStatsTableClassIndex.Temp, RowStatsCounterType.Read), perfInstance.TempRowsReadRate);
			flag2 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Temp, RowStatsCounterType.Seek), dumpedStats.GetCounter(RowStatsTableClassIndex.Temp, RowStatsCounterType.Seek), perfInstance.TempRowsSeekRate);
			flag3 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Temp, RowStatsCounterType.Accept), dumpedStats.GetCounter(RowStatsTableClassIndex.Temp, RowStatsCounterType.Accept), perfInstance.TempRowsAcceptRate);
			flag4 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Temp, RowStatsCounterType.Write), dumpedStats.GetCounter(RowStatsTableClassIndex.Temp, RowStatsCounterType.Write), perfInstance.TempRowsWriteRate);
			flag5 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Temp, RowStatsCounterType.ReadBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.Temp, RowStatsCounterType.ReadBytes), perfInstance.TempBytesReadRate);
			flag6 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Temp, RowStatsCounterType.WriteBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.Temp, RowStatsCounterType.WriteBytes), perfInstance.TempBytesWriteRate);
			flag |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.TableFunction, RowStatsCounterType.Read), dumpedStats.GetCounter(RowStatsTableClassIndex.TableFunction, RowStatsCounterType.Read), perfInstance.TableFunctionRowsReadRate);
			flag3 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.TableFunction, RowStatsCounterType.Accept), dumpedStats.GetCounter(RowStatsTableClassIndex.TableFunction, RowStatsCounterType.Accept), perfInstance.TableFunctionRowsAcceptRate);
			flag5 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.TableFunction, RowStatsCounterType.ReadBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.TableFunction, RowStatsCounterType.ReadBytes), perfInstance.TableFunctionBytesReadRate);
			flag |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Read), dumpedStats.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Read), perfInstance.LazyIndexRowsReadRate);
			flag2 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Seek), dumpedStats.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Seek), perfInstance.LazyIndexRowsSeekRate);
			flag3 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Accept), dumpedStats.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Accept), perfInstance.LazyIndexRowsAcceptRate);
			flag4 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Write), dumpedStats.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Write), perfInstance.LazyIndexRowsWriteRate);
			flag5 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.ReadBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.ReadBytes), perfInstance.LazyIndexBytesReadRate);
			flag6 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.WriteBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.WriteBytes), perfInstance.LazyIndexBytesWriteRate);
			flag |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Read), dumpedStats.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Read), perfInstance.MessageRowsReadRate);
			flag2 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Seek), dumpedStats.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Seek), perfInstance.MessageRowsSeekRate);
			flag3 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Accept), dumpedStats.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Accept), perfInstance.MessageRowsAcceptRate);
			flag4 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Write), dumpedStats.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Write), perfInstance.MessageRowsWriteRate);
			flag5 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.ReadBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.ReadBytes), perfInstance.MessageBytesReadRate);
			flag6 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.WriteBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.WriteBytes), perfInstance.MessageBytesWriteRate);
			flag |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Attachment, RowStatsCounterType.Read), dumpedStats.GetCounter(RowStatsTableClassIndex.Attachment, RowStatsCounterType.Read), perfInstance.AttachmentRowsReadRate);
			flag2 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Attachment, RowStatsCounterType.Seek), dumpedStats.GetCounter(RowStatsTableClassIndex.Attachment, RowStatsCounterType.Seek), perfInstance.AttachmentRowsSeekRate);
			flag3 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Attachment, RowStatsCounterType.Accept), dumpedStats.GetCounter(RowStatsTableClassIndex.Attachment, RowStatsCounterType.Accept), perfInstance.AttachmentRowsAcceptRate);
			flag4 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Attachment, RowStatsCounterType.Write), dumpedStats.GetCounter(RowStatsTableClassIndex.Attachment, RowStatsCounterType.Write), perfInstance.AttachmentRowsWriteRate);
			flag5 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Attachment, RowStatsCounterType.ReadBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.Attachment, RowStatsCounterType.ReadBytes), perfInstance.AttachmentBytesReadRate);
			flag6 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Attachment, RowStatsCounterType.WriteBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.Attachment, RowStatsCounterType.WriteBytes), perfInstance.AttachmentBytesWriteRate);
			flag |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Folder, RowStatsCounterType.Read), dumpedStats.GetCounter(RowStatsTableClassIndex.Folder, RowStatsCounterType.Read), perfInstance.FolderRowsReadRate);
			flag2 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Folder, RowStatsCounterType.Seek), dumpedStats.GetCounter(RowStatsTableClassIndex.Folder, RowStatsCounterType.Seek), perfInstance.FolderRowsSeekRate);
			flag3 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Folder, RowStatsCounterType.Accept), dumpedStats.GetCounter(RowStatsTableClassIndex.Folder, RowStatsCounterType.Accept), perfInstance.FolderRowsAcceptRate);
			flag4 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Folder, RowStatsCounterType.Write), dumpedStats.GetCounter(RowStatsTableClassIndex.Folder, RowStatsCounterType.Write), perfInstance.FolderRowsWriteRate);
			flag5 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Folder, RowStatsCounterType.ReadBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.Folder, RowStatsCounterType.ReadBytes), perfInstance.FolderBytesReadRate);
			flag6 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Folder, RowStatsCounterType.WriteBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.Folder, RowStatsCounterType.WriteBytes), perfInstance.FolderBytesWriteRate);
			flag |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.PseudoIndexMaintenance, RowStatsCounterType.Read), dumpedStats.GetCounter(RowStatsTableClassIndex.PseudoIndexMaintenance, RowStatsCounterType.Read), perfInstance.PseudoIndexMaintenanceRowsReadRate);
			flag2 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.PseudoIndexMaintenance, RowStatsCounterType.Seek), dumpedStats.GetCounter(RowStatsTableClassIndex.PseudoIndexMaintenance, RowStatsCounterType.Seek), perfInstance.PseudoIndexMaintenanceRowsSeekRate);
			flag3 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.PseudoIndexMaintenance, RowStatsCounterType.Accept), dumpedStats.GetCounter(RowStatsTableClassIndex.PseudoIndexMaintenance, RowStatsCounterType.Accept), perfInstance.PseudoIndexMaintenanceRowsAcceptRate);
			flag4 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.PseudoIndexMaintenance, RowStatsCounterType.Write), dumpedStats.GetCounter(RowStatsTableClassIndex.PseudoIndexMaintenance, RowStatsCounterType.Write), perfInstance.PseudoIndexMaintenanceRowsWriteRate);
			flag5 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.PseudoIndexMaintenance, RowStatsCounterType.ReadBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.PseudoIndexMaintenance, RowStatsCounterType.ReadBytes), perfInstance.PseudoIndexMaintenanceBytesReadRate);
			flag6 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.PseudoIndexMaintenance, RowStatsCounterType.WriteBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.PseudoIndexMaintenance, RowStatsCounterType.WriteBytes), perfInstance.PseudoIndexMaintenanceBytesWriteRate);
			flag |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Events, RowStatsCounterType.Read), dumpedStats.GetCounter(RowStatsTableClassIndex.Events, RowStatsCounterType.Read), perfInstance.EventsRowsReadRate);
			flag2 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Events, RowStatsCounterType.Seek), dumpedStats.GetCounter(RowStatsTableClassIndex.Events, RowStatsCounterType.Seek), perfInstance.EventsRowsSeekRate);
			flag3 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Events, RowStatsCounterType.Accept), dumpedStats.GetCounter(RowStatsTableClassIndex.Events, RowStatsCounterType.Accept), perfInstance.EventsRowsAcceptRate);
			flag4 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Events, RowStatsCounterType.Write), dumpedStats.GetCounter(RowStatsTableClassIndex.Events, RowStatsCounterType.Write), perfInstance.EventsRowsWriteRate);
			flag5 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Events, RowStatsCounterType.ReadBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.Events, RowStatsCounterType.ReadBytes), perfInstance.EventsBytesReadRate);
			flag6 |= RowStats.DumpOneCounter(this.GetCounter(RowStatsTableClassIndex.Events, RowStatsCounterType.WriteBytes), dumpedStats.GetCounter(RowStatsTableClassIndex.Events, RowStatsCounterType.WriteBytes), perfInstance.EventsBytesWriteRate);
			if (flag)
			{
				RowStats.DumpOneCounter(this.ReadTotal, dumpedStats.ReadTotal, perfInstance.RowsReadRate);
			}
			if (flag2)
			{
				RowStats.DumpOneCounter(this.SeekTotal, dumpedStats.SeekTotal, perfInstance.RowsSeekRate);
			}
			if (flag3)
			{
				RowStats.DumpOneCounter(this.AcceptTotal, dumpedStats.AcceptTotal, perfInstance.RowsAcceptRate);
			}
			if (flag4)
			{
				RowStats.DumpOneCounter(this.WriteTotal, dumpedStats.WriteTotal, perfInstance.RowsWriteRate);
			}
			if (flag5)
			{
				RowStats.DumpOneCounter(this.ReadBytesTotal, dumpedStats.ReadBytesTotal, perfInstance.BytesReadRate);
			}
			if (flag6)
			{
				RowStats.DumpOneCounter(this.WriteBytesTotal, dumpedStats.WriteBytesTotal, perfInstance.BytesWriteRate);
			}
			return flag || flag2 || flag3 || flag4 || flag5 || flag6;
		}

		private bool IsTableClassEmpty(RowStatsTableClassIndex tableClassIndex)
		{
			return this.GetCounter(tableClassIndex, RowStatsCounterType.Read) == 0 && this.GetCounter(tableClassIndex, RowStatsCounterType.Seek) == 0 && this.GetCounter(tableClassIndex, RowStatsCounterType.Accept) == 0 && this.GetCounter(tableClassIndex, RowStatsCounterType.Write) == 0 && this.GetCounter(tableClassIndex, RowStatsCounterType.ReadBytes) == 0 && 0 == this.GetCounter(tableClassIndex, RowStatsCounterType.WriteBytes);
		}

		private int GetTotalCounter(RowStatsCounterType counterType)
		{
			return this.GetCounter(RowStatsTableClassIndex.Other, counterType) + this.GetCounter(RowStatsTableClassIndex.Temp, counterType) + this.GetCounter(RowStatsTableClassIndex.TableFunction, counterType) + this.GetCounter(RowStatsTableClassIndex.LazyIndex, counterType) + this.GetCounter(RowStatsTableClassIndex.Message, counterType) + this.GetCounter(RowStatsTableClassIndex.Attachment, counterType) + this.GetCounter(RowStatsTableClassIndex.Folder, counterType) + this.GetCounter(RowStatsTableClassIndex.PseudoIndexMaintenance, counterType) + this.GetCounter(RowStatsTableClassIndex.Events, counterType);
		}

		public void Aggregate(RowStats other)
		{
			for (int i = 0; i < this.counters.Length; i++)
			{
				this.counters[i] += other.counters[i];
			}
		}

		public void CopyFrom(RowStats other)
		{
			for (int i = 0; i < this.counters.Length; i++)
			{
				this.counters[i] = other.counters[i];
			}
		}

		public void AppendToTraceContentBuilder(TraceContentBuilder cb)
		{
			this.AppendToString(cb);
		}

		public override string ToString()
		{
			TraceContentBuilder traceContentBuilder = TraceContentBuilder.Create();
			this.AppendToString(traceContentBuilder);
			return traceContentBuilder.ToString();
		}

		public bool AppendToString(TraceContentBuilder cb)
		{
			bool flag = false;
			if (this.counters != null)
			{
				flag |= this.AppendCountersToString(cb, "TF", RowStatsTableClassIndex.TableFunction, flag);
				flag |= this.AppendCountersToString(cb, "LI", RowStatsTableClassIndex.LazyIndex, flag);
				flag |= this.AppendCountersToString(cb, "MSG", RowStatsTableClassIndex.Message, flag);
				flag |= this.AppendCountersToString(cb, "ATT", RowStatsTableClassIndex.Attachment, flag);
				flag |= this.AppendCountersToString(cb, "FLD", RowStatsTableClassIndex.Folder, flag);
				flag |= this.AppendCountersToString(cb, "PIM", RowStatsTableClassIndex.PseudoIndexMaintenance, flag);
				flag |= this.AppendCountersToString(cb, "EVT", RowStatsTableClassIndex.Events, flag);
				flag |= this.AppendCountersToString(cb, "OTH", RowStatsTableClassIndex.Other, flag);
				flag |= this.AppendCountersToString(cb, "TMP", RowStatsTableClassIndex.Temp, flag);
			}
			return flag;
		}

		private bool AppendCountersToString(TraceContentBuilder cb, string name, RowStatsTableClassIndex tableClassIndex, bool continueList)
		{
			if (!this.IsTableClassEmpty(tableClassIndex))
			{
				if (name != null)
				{
					if (continueList)
					{
						cb.Append(" ");
					}
					cb.Append(name);
					cb.Append(":[");
				}
				continueList = false;
				continueList = RowStats.AppendCounterToString(cb, "r", this.GetCounter(tableClassIndex, RowStatsCounterType.Read), continueList);
				continueList = RowStats.AppendCounterToString(cb, "s", this.GetCounter(tableClassIndex, RowStatsCounterType.Seek), continueList);
				continueList = RowStats.AppendCounterToString(cb, "a", this.GetCounter(tableClassIndex, RowStatsCounterType.Accept), continueList);
				continueList = RowStats.AppendCounterToString(cb, "w", this.GetCounter(tableClassIndex, RowStatsCounterType.Write), continueList);
				continueList = RowStats.AppendCounterToString(cb, "rb", this.GetCounter(tableClassIndex, RowStatsCounterType.ReadBytes), continueList);
				continueList = RowStats.AppendCounterToString(cb, "wb", this.GetCounter(tableClassIndex, RowStatsCounterType.WriteBytes), continueList);
				if (name != null)
				{
					cb.Append("]");
				}
				return true;
			}
			return continueList;
		}

		private int[] counters;
	}
}
