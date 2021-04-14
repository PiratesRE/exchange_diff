using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AppointmentTombstone
	{
		public void LoadTombstoneRecords(byte[] tombstoneInfo, int monthsCount)
		{
			if (tombstoneInfo != null)
			{
				List<TombstoneRecord> list = new List<TombstoneRecord>();
				ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, ExDateTime.Now.Year, ExDateTime.Now.Month, 1);
				ExDateTime exDateTime2 = ExDateTime.UtcNow.AddDays((double)(CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - ExDateTime.UtcNow.DayOfWeek));
				exDateTime2 = new ExDateTime(ExTimeZone.UtcTimeZone, exDateTime2.Year, exDateTime2.Month, exDateTime2.Day);
				ExDateTime t = (exDateTime < exDateTime2) ? exDateTime : exDateTime2;
				ExDateTime t2 = t.AddMonths(monthsCount);
				try
				{
					using (MemoryStream memoryStream = new MemoryStream(tombstoneInfo))
					{
						using (BinaryReader binaryReader = new BinaryReader(memoryStream, CTSGlobals.AsciiEncoding))
						{
							byte[] array = new byte[4];
							binaryReader.Read(array, 0, 4);
							if (BitConverter.ToUInt32(array, 0) != 3202265037U)
							{
								ExTraceGlobals.MeetingMessageTracer.TraceError<string>((long)this.GetHashCode(), "AppointmentTombstone.Identifier field MUST have a value of 0xBEDEAFCD. Value found in the stream: {0}", BitConverter.ToUInt32(array, 0).ToString("X"));
								throw new CorruptDataException(ServerStrings.AppointmentTombstoneCorrupt);
							}
							binaryReader.Read(array, 0, 4);
							if (BitConverter.ToUInt32(array, 0) != 20U)
							{
								ExTraceGlobals.MeetingMessageTracer.TraceError<string>((long)this.GetHashCode(), "AppointmentTombstone.Headersize field MUST have a value of 0x00000014. Value found in the stream: {0}", BitConverter.ToUInt32(array, 0).ToString("X"));
								throw new CorruptDataException(ServerStrings.AppointmentTombstoneCorrupt);
							}
							binaryReader.Read(array, 0, 4);
							if (BitConverter.ToUInt32(array, 0) != 3U)
							{
								ExTraceGlobals.MeetingMessageTracer.TraceError<string>((long)this.GetHashCode(), "AppointmentTombstone.Version field MUST have a value of 0x00000003. Value found in the stream: {0}", BitConverter.ToUInt32(array, 0).ToString("X"));
								throw new CorruptDataException(ServerStrings.AppointmentTombstoneCorrupt);
							}
							binaryReader.Read(array, 0, 4);
							int num = BitConverter.ToInt32(array, 0);
							if (num <= 0)
							{
								ExTraceGlobals.MeetingMessageTracer.TraceError<int>((long)this.GetHashCode(), "AppointmentTombstone.Record count field MUST have a positive value. Value found in the stream {0}", num);
								throw new CorruptDataException(ServerStrings.AppointmentTombstoneCorrupt);
							}
							binaryReader.Read(array, 0, 4);
							if (BitConverter.ToUInt32(array, 0) != 20U)
							{
								ExTraceGlobals.MeetingMessageTracer.TraceError<string>((long)this.GetHashCode(), "AppointmentTombstone.RecordSize field MUST have a value of 0x00000014. Value found in the stream: {0}", BitConverter.ToUInt32(array, 0).ToString("X"));
								throw new CorruptDataException(ServerStrings.AppointmentTombstoneCorrupt);
							}
							for (int i = 0; i < num; i++)
							{
								binaryReader.Read(array, 0, 4);
								int num2 = BitConverter.ToInt32(array, 0);
								if (num2 <= 0)
								{
									ExTraceGlobals.MeetingMessageTracer.TraceError((long)this.GetHashCode(), "Tombstone record MUST have a positive start time value");
									break;
								}
								binaryReader.Read(array, 0, 4);
								int num3 = BitConverter.ToInt32(array, 0);
								if (num3 <= 0)
								{
									ExTraceGlobals.MeetingMessageTracer.TraceError((long)this.GetHashCode(), "Tombstone record MUST have a positive end time value");
									break;
								}
								binaryReader.Read(array, 0, 4);
								int num4 = BitConverter.ToInt32(array, 0);
								if (num4 <= 0)
								{
									ExTraceGlobals.MeetingMessageTracer.TraceError((long)this.GetHashCode(), "Tombstone record MUST have a positive global object id value");
									break;
								}
								byte[] array2 = new byte[num4];
								binaryReader.Read(array2, 0, num4);
								binaryReader.Read(array, 0, 2);
								short num5 = BitConverter.ToInt16(array, 0);
								if (num5 <= 0)
								{
									ExTraceGlobals.MeetingMessageTracer.TraceError((long)this.GetHashCode(), "Tombstone record MUST have a positive user name size value");
									break;
								}
								byte[] array3 = new byte[(int)num5];
								binaryReader.Read(array3, 0, (int)num5);
								TombstoneRecord tombstoneRecord = new TombstoneRecord
								{
									StartTime = ExDateTime.FromFileTimeUtc((long)num2 * 600000000L),
									EndTime = ExDateTime.FromFileTimeUtc((long)num3 * 600000000L),
									GlobalObjectId = array2,
									UserName = array3
								};
								if (tombstoneRecord.StartTime <= t2 && tombstoneRecord.EndTime >= t)
								{
									list.Add(tombstoneRecord);
								}
							}
						}
					}
				}
				catch (IOException ex)
				{
					ExTraceGlobals.MeetingMessageTracer.TraceError<string>((long)this.GetHashCode(), "Unable to process the tombstone record with exception:'{0}'", ex.Message);
				}
				catch (ArgumentOutOfRangeException ex2)
				{
					ExTraceGlobals.MeetingMessageTracer.TraceError<string>((long)this.GetHashCode(), "Unable to process the tombstone record with exception:'{0}'", ex2.Message);
				}
				catch (ArgumentException ex3)
				{
					ExTraceGlobals.MeetingMessageTracer.TraceError<string>((long)this.GetHashCode(), "Unable to process the tombstone record with exception:'{0}'", ex3.Message);
				}
				ExTraceGlobals.MeetingMessageTracer.Information<int>((long)this.GetHashCode(), "Number of tombstone records loaded: {0}", list.Count);
				this.tombstoneRecords = list.ToArray();
			}
		}

		public int AppendTombstoneRecord(TombstoneRecord tombstoneRecord)
		{
			if (tombstoneRecord == null)
			{
				throw new ArgumentNullException("tombstoneRecord", "tombstoneRecord is expected!");
			}
			List<TombstoneRecord> list;
			if (this.tombstoneRecords != null)
			{
				list = new List<TombstoneRecord>(this.tombstoneRecords);
			}
			else
			{
				list = new List<TombstoneRecord>();
			}
			list.Add(tombstoneRecord);
			this.tombstoneRecords = list.ToArray();
			return this.tombstoneRecords.Length;
		}

		public byte[] ToByteArray()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					int num = 0;
					binaryWriter.Write(3202265037U);
					binaryWriter.Write(20U);
					binaryWriter.Write(3U);
					binaryWriter.Write(this.tombstoneRecords.Length);
					binaryWriter.Write(20U);
					for (int i = 0; i < this.tombstoneRecords.Length; i++)
					{
						byte[] buffer;
						if (this.tombstoneRecords[i].TryGetBytes(out buffer))
						{
							binaryWriter.Write(buffer);
							num++;
						}
					}
					if (this.tombstoneRecords.Length != num)
					{
						memoryStream.Seek(12L, SeekOrigin.Begin);
						binaryWriter.Write(num);
					}
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		public TombstoneRecord[] TombstoneRecords
		{
			get
			{
				return this.tombstoneRecords;
			}
		}

		private const uint RecordCountOffset = 12U;

		private const uint Identifier = 3202265037U;

		private const uint HeaderSize = 20U;

		private const uint Version = 3U;

		private const uint RecordSize = 20U;

		private TombstoneRecord[] tombstoneRecords;
	}
}
