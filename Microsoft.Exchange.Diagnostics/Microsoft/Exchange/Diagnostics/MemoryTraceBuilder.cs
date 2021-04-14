using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Diagnostics
{
	public class MemoryTraceBuilder : ITraceBuilder
	{
		public MemoryTraceBuilder(int maximumEntries, int maximumBufferSize)
		{
			this.traceBuffer = new char[maximumBufferSize + 1];
			this.entries = new TraceEntry[maximumEntries + 1];
			this.bufferLength = maximumBufferSize + 1;
			this.entryArrayLength = maximumEntries + 1;
			this.nativeThreadId = DiagnosticsNativeMethods.GetCurrentThreadId();
		}

		private int FreeSize
		{
			get
			{
				int num = this.freeBufferIndex - this.startBufferIndex;
				if (num < 0)
				{
					return -num;
				}
				return this.bufferLength - num;
			}
		}

		public int NativeThreadId
		{
			get
			{
				return this.nativeThreadId;
			}
		}

		public bool InsideTraceCall
		{
			get
			{
				return this.insideTraceCall;
			}
		}

		public static List<object> GetTraceArguments(char[] buffer, int startIndex, int length)
		{
			List<object> list = new List<object>();
			int i = length;
			int num = startIndex;
			while (i > 0)
			{
				i -= MemoryTraceBuilder.GetTraceArgument(list, buffer, ref num);
			}
			return list;
		}

		public void AddArgument<T>(T traceArgument)
		{
			TraceFormatter<T>.Default.Format(this, traceArgument);
		}

		public void AddArgument(int traceArgument)
		{
			if (!this.ReserveSpace(3))
			{
				return;
			}
			this.AppendWithoutCheck('\0');
			this.AppendWithoutCheck((char)traceArgument);
			this.AppendWithoutCheck((char)(traceArgument >> 16));
		}

		public void AddArgument(long traceArgument)
		{
			if (!this.ReserveSpace(5))
			{
				return;
			}
			this.AppendWithoutCheck('\u0001');
			this.AppendWithoutCheck((char)traceArgument);
			this.AppendWithoutCheck((char)(traceArgument >> 16));
			this.AppendWithoutCheck((char)(traceArgument >> 32));
			this.AppendWithoutCheck((char)(traceArgument >> 48));
		}

		public unsafe void AddArgument(Guid traceArgument)
		{
			if (!this.ReserveSpace(9))
			{
				return;
			}
			this.AppendWithoutCheck('\u0002');
			char* ptr = (char*)(&traceArgument);
			for (int i = 0; i < sizeof(Guid) / 2; i++)
			{
				this.AppendWithoutCheck(ptr[i]);
			}
		}

		public void AddArgument(string traceArgument)
		{
			if (string.IsNullOrEmpty(traceArgument))
			{
				traceArgument = string.Empty;
			}
			if (!this.PrepareStringArgument(traceArgument.Length))
			{
				return;
			}
			for (int i = 0; i < traceArgument.Length; i++)
			{
				this.AppendWithoutCheck(traceArgument[i]);
			}
		}

		public void AddArgument(char[] traceArgument)
		{
			if (traceArgument == null)
			{
				this.AddArgument(null);
				return;
			}
			if (!this.PrepareStringArgument(traceArgument.Length))
			{
				return;
			}
			for (int i = 0; i < traceArgument.Length; i++)
			{
				this.AppendWithoutCheck(traceArgument[i]);
			}
		}

		public unsafe void AddArgument(char* traceArgument, int length)
		{
			if (traceArgument == null)
			{
				this.AddArgument(null);
				return;
			}
			if (!this.PrepareStringArgument(length))
			{
				return;
			}
			for (int i = 0; i < length; i++)
			{
				this.AppendWithoutCheck(traceArgument[i]);
			}
		}

		public void BeginEntry(TraceType traceType, Guid componentGuid, int traceTag, long id, string format)
		{
			this.PrepareForBeginEntry();
			this.entries[this.freeEntry] = new TraceEntry(traceType, componentGuid, traceTag, id, format, this.freeBufferIndex, this.nativeThreadId);
		}

		public void EndEntry()
		{
			if (this.entrySizeOverLimit)
			{
				this.entries[this.freeEntry].FormatString = "The tracing entry is too big to fit into the tracing buffer.";
				this.entries[this.freeEntry].Length = 0;
				this.freeBufferIndex = this.entries[this.freeEntry].StartIndex;
			}
			else
			{
				this.entries[this.freeEntry].Length = (int)this.UsedBufferSizeSince(this.entries[this.freeEntry].StartIndex);
			}
			this.IncrementFreeEntryIndex();
			this.insideTraceCall = false;
		}

		public void Reset()
		{
			this.insideTraceCall = false;
			this.startBufferIndex = 0;
			this.freeBufferIndex = 0;
			this.startEntry = 0;
			this.freeEntry = 0;
		}

		public void Dump(TextWriter writer, bool addHeader, bool verbose)
		{
			if (writer == null)
			{
				return;
			}
			if (addHeader)
			{
				if (verbose)
				{
					writer.WriteLine("ThreadId;ComponentGuid;Instance ID;TraceTag;TraceType;TimeStamp;Message;");
				}
				else
				{
					writer.WriteLine("ThreadId;TimeStamp;Message;");
				}
			}
			List<KeyValuePair<TraceEntry, List<object>>> traceEntries = this.GetTraceEntries();
			foreach (KeyValuePair<TraceEntry, List<object>> keyValuePair in traceEntries)
			{
				TraceEntry key = keyValuePair.Key;
				writer.Write(key.NativeThreadId);
				writer.Write(';');
				if (verbose)
				{
					writer.Write(key.ComponentGuid);
					writer.Write(';');
					writer.Write(key.Id);
					writer.Write(';');
					writer.Write(key.TraceTag);
					writer.Write(';');
					writer.Write(key.TraceType);
					writer.Write(';');
				}
				writer.Write(key.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));
				writer.Write(';');
				if (keyValuePair.Value.Count == 0)
				{
					writer.Write(key.FormatString);
				}
				else
				{
					writer.Write(string.Format(key.FormatString, keyValuePair.Value.ToArray()));
				}
				writer.WriteLine(';');
			}
		}

		public IEnumerable<TraceEntry> GetTraces()
		{
			List<KeyValuePair<TraceEntry, List<object>>> entries = this.GetTraceEntries();
			foreach (KeyValuePair<TraceEntry, List<object>> pair in entries)
			{
				KeyValuePair<TraceEntry, List<object>> keyValuePair = pair;
				TraceEntry entry = keyValuePair.Key;
				KeyValuePair<TraceEntry, List<object>> keyValuePair2 = pair;
				string traceMessage;
				if (keyValuePair2.Value.Count == 0)
				{
					traceMessage = entry.FormatString;
				}
				else
				{
					string formatString = entry.FormatString;
					KeyValuePair<TraceEntry, List<object>> keyValuePair3 = pair;
					traceMessage = string.Format(formatString, keyValuePair3.Value.ToArray());
				}
				TraceEntry result = new TraceEntry(entry.TraceType, entry.ComponentGuid, entry.TraceTag, entry.Id, traceMessage, 0, entry.NativeThreadId);
				yield return result;
			}
			yield break;
		}

		public int FindLastEntryIndex(int entriesToFind, TraceType traceType, int traceTag, Guid componentGuid, string formatString)
		{
			int num = 0;
			int num2 = this.freeEntry - 1;
			if (num2 < 0)
			{
				num2 = this.entryArrayLength - 1;
			}
			while (num2 != this.startEntry)
			{
				TraceEntry traceEntry = this.entries[num2];
				if (traceEntry.TraceType == traceType && traceEntry.TraceTag == traceTag && traceEntry.ComponentGuid == componentGuid && object.Equals(traceEntry.FormatString, formatString) && ++num == entriesToFind)
				{
					break;
				}
				if (--num2 < 0)
				{
					num2 = this.entryArrayLength - 1;
				}
			}
			return num2;
		}

		public void CopyTo(MemoryTraceBuilder destination, int startIndex)
		{
			if ((this.startEntry < this.freeEntry && (startIndex < this.startEntry || startIndex >= this.freeEntry)) || (this.startEntry > this.freeEntry && startIndex < this.startEntry && startIndex >= this.freeEntry))
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			int num = startIndex;
			while (num != this.freeEntry)
			{
				TraceEntry traceEntry = this.entries[num];
				destination.BeginEntry(traceEntry);
				if (destination.ReserveSpace(traceEntry.Length))
				{
					int startIndex2 = traceEntry.StartIndex;
					for (int i = traceEntry.Length; i > 0; i--)
					{
						char nextCharInTraceBuffer = MemoryTraceBuilder.GetNextCharInTraceBuffer(this.traceBuffer, ref startIndex2);
						destination.AppendWithoutCheck(nextCharInTraceBuffer);
					}
				}
				destination.EndEntry();
				if (++num == this.entryArrayLength)
				{
					num = 0;
				}
			}
		}

		internal List<KeyValuePair<TraceEntry, List<object>>> GetTraceEntries()
		{
			List<KeyValuePair<TraceEntry, List<object>>> list = new List<KeyValuePair<TraceEntry, List<object>>>();
			for (int num = this.startEntry; num != this.freeEntry; num = (num + 1) % this.entryArrayLength)
			{
				List<object> traceArguments = MemoryTraceBuilder.GetTraceArguments(this.traceBuffer, this.entries[num].StartIndex, this.entries[num].Length);
				list.Add(new KeyValuePair<TraceEntry, List<object>>(this.entries[num], traceArguments));
			}
			return list;
		}

		private static char GetNextCharInTraceBuffer(char[] buffer, ref int index)
		{
			char result = buffer[index];
			index = (index + 1) % buffer.Length;
			return result;
		}

		private static int GetTraceArgument(List<object> arguments, char[] buffer, ref int startIndex)
		{
			int result = 0;
			int nextCharInTraceBuffer = (int)MemoryTraceBuilder.GetNextCharInTraceBuffer(buffer, ref startIndex);
			if (nextCharInTraceBuffer == 0)
			{
				int num = (int)MemoryTraceBuilder.GetNextCharInTraceBuffer(buffer, ref startIndex);
				num += (int)((int)MemoryTraceBuilder.GetNextCharInTraceBuffer(buffer, ref startIndex) << 16);
				result = 3;
				arguments.Add(num);
			}
			else if (nextCharInTraceBuffer == 1)
			{
				long num2 = (long)((ulong)MemoryTraceBuilder.GetNextCharInTraceBuffer(buffer, ref startIndex));
				num2 += (long)((long)((ulong)MemoryTraceBuilder.GetNextCharInTraceBuffer(buffer, ref startIndex)) << 16);
				num2 += (long)((long)((ulong)MemoryTraceBuilder.GetNextCharInTraceBuffer(buffer, ref startIndex)) << 32);
				num2 += (long)((long)((ulong)MemoryTraceBuilder.GetNextCharInTraceBuffer(buffer, ref startIndex)) << 48);
				result = 5;
				arguments.Add(num2);
			}
			else if (nextCharInTraceBuffer == 2)
			{
				byte[] array = new byte[16];
				for (int i = 0; i < 16; i += 2)
				{
					char nextCharInTraceBuffer2 = MemoryTraceBuilder.GetNextCharInTraceBuffer(buffer, ref startIndex);
					array[i] = (byte)nextCharInTraceBuffer2;
					array[i + 1] = (byte)(nextCharInTraceBuffer2 >> 8);
				}
				Guid guid = new Guid(array);
				arguments.Add(guid);
				result = 9;
			}
			else if (nextCharInTraceBuffer == 3)
			{
				int nextCharInTraceBuffer3 = (int)MemoryTraceBuilder.GetNextCharInTraceBuffer(buffer, ref startIndex);
				StringBuilder stringBuilder = new StringBuilder();
				for (int j = 0; j < nextCharInTraceBuffer3; j++)
				{
					stringBuilder.Append(MemoryTraceBuilder.GetNextCharInTraceBuffer(buffer, ref startIndex));
				}
				result = nextCharInTraceBuffer3 + 2;
				arguments.Add(stringBuilder.ToString());
			}
			return result;
		}

		private void PrepareForBeginEntry()
		{
			this.insideTraceCall = true;
			this.currentEntrySize = 0;
			this.entrySizeOverLimit = false;
			if (this.IsEntryArrayFull())
			{
				this.FreeOneEntry();
			}
		}

		private void BeginEntry(TraceEntry traceEntry)
		{
			this.PrepareForBeginEntry();
			this.entries[this.freeEntry] = new TraceEntry(traceEntry.TraceType, traceEntry.ComponentGuid, traceEntry.TraceTag, traceEntry.Id, traceEntry.FormatString, this.freeBufferIndex, traceEntry.NativeThreadId);
		}

		private void IncrementFreeEntryIndex()
		{
			if (++this.freeEntry == this.entryArrayLength)
			{
				this.freeEntry = 0;
			}
		}

		private bool PrepareStringArgument(int length)
		{
			int size = length + 2;
			if (!this.ReserveSpace(size))
			{
				return false;
			}
			this.AppendWithoutCheck('\u0003');
			this.AppendWithoutCheck((char)length);
			return true;
		}

		private char UsedBufferSizeSince(int startIndex)
		{
			int num = this.freeBufferIndex - startIndex;
			if (num < 0)
			{
				num += this.bufferLength;
			}
			return (char)num;
		}

		private void Append(char ch)
		{
			while (this.IsBufferFull())
			{
				this.FreeOneEntry();
			}
			this.AppendWithoutCheck(ch);
		}

		private void AppendWithoutCheck(char ch)
		{
			this.traceBuffer[this.freeBufferIndex] = ch;
			this.IncrementFreeBufferIndex();
		}

		private void IncrementFreeBufferIndex()
		{
			if (++this.freeBufferIndex == this.bufferLength)
			{
				this.freeBufferIndex = 0;
			}
		}

		private bool ReserveSpace(int size)
		{
			if (this.entrySizeOverLimit)
			{
				return false;
			}
			if (this.currentEntrySize >= this.bufferLength - size)
			{
				this.entrySizeOverLimit = true;
				return false;
			}
			this.currentEntrySize += size;
			while (this.FreeSize <= size)
			{
				this.FreeOneEntry();
			}
			return true;
		}

		private bool IsEntryArrayFull()
		{
			int num = this.freeEntry + 1;
			if (num == this.entryArrayLength)
			{
				num = 0;
			}
			return num == this.startEntry;
		}

		private bool IsBufferFull()
		{
			int num = this.freeBufferIndex + 1;
			if (num == this.bufferLength)
			{
				num = 0;
			}
			return num == this.startBufferIndex;
		}

		private void FreeOneEntry()
		{
			this.IncrementBufferStartIndexBy(this.entries[this.startEntry].Length);
			this.entries[this.startEntry].Clear();
			this.IncrementStartEntryIndex();
		}

		private void IncrementStartEntryIndex()
		{
			if (++this.startEntry == this.entryArrayLength)
			{
				this.startEntry = 0;
			}
		}

		private void IncrementBufferStartIndexBy(int length)
		{
			this.startBufferIndex += length;
			if (this.startBufferIndex >= this.bufferLength)
			{
				this.startBufferIndex -= this.bufferLength;
			}
		}

		private const string BigEntryFormatString = "The tracing entry is too big to fit into the tracing buffer.";

		private const int CharsForInt32 = 3;

		private const int CharsForInt64 = 5;

		private const int CharsForGuid = 9;

		private char[] traceBuffer;

		private int bufferLength;

		private int startBufferIndex;

		private int freeBufferIndex;

		private TraceEntry[] entries;

		private int entryArrayLength;

		private int startEntry;

		private int freeEntry;

		private int nativeThreadId;

		private bool insideTraceCall;

		private int currentEntrySize;

		private bool entrySizeOverLimit;
	}
}
