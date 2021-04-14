using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Directory.DirSync;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal sealed class BackSyncCookie : ISyncCookie
	{
		public BackSyncCookie(DateTime timestamp, DateTime lastReadFailureStartTime, DateTime lastWhenChanged, Guid invocationId, bool moreData, byte[] rawCookie, Dictionary<string, int> errorObjects, byte[] lastDirSyncCookieWithReplicationVectors, ServiceInstanceId serviceInstanceId, Guid sequenceId, DateTime sequenceStartTimestamp)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "New BackSyncCookie");
			this.MoreDirSyncData = moreData;
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool>((long)SyncConfiguration.TraceId, "BackSyncCookie moreData {0}", moreData);
			this.Timestamp = timestamp;
			ExTraceGlobals.BackSyncTracer.TraceDebug<DateTime>((long)SyncConfiguration.TraceId, "BackSyncCookie timestamp {0}", timestamp);
			this.InvocationId = invocationId;
			ExTraceGlobals.BackSyncTracer.TraceDebug<Guid>((long)SyncConfiguration.TraceId, "BackSyncCookie invocationId {0}", invocationId);
			this.DirSyncCookie = rawCookie;
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "BackSyncCookie rawCookie {0}", (rawCookie != null) ? Convert.ToBase64String(rawCookie) : "NULL");
			this.Version = 4;
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "BackSyncCookie version {0}", 4);
			this.LastReadFailureStartTime = lastReadFailureStartTime;
			ExTraceGlobals.BackSyncTracer.TraceDebug<DateTime>((long)SyncConfiguration.TraceId, "BackSyncCookie lastReadFailureStartTime {0}", lastReadFailureStartTime);
			this.LastWhenChanged = lastWhenChanged;
			ExTraceGlobals.BackSyncTracer.TraceDebug<DateTime>((long)SyncConfiguration.TraceId, "BackSyncCookie lastWhenChanged {0}", lastWhenChanged);
			this.ServiceInstanceId = serviceInstanceId;
			ExTraceGlobals.BackSyncTracer.TraceDebug<ServiceInstanceId>((long)SyncConfiguration.TraceId, "BackSyncCookie serviceInstanceId {0}", serviceInstanceId);
			if (errorObjects != null)
			{
				this.ErrorObjectsAndFailureCounts = errorObjects;
			}
			else
			{
				this.ErrorObjectsAndFailureCounts = new Dictionary<string, int>();
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "BackSyncCookie ErrorObjectsAndFailureCounts count = {0}", this.ErrorObjectsAndFailureCounts.Count);
			this.LastDirSyncCookieWithReplicationVectors = lastDirSyncCookieWithReplicationVectors;
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "BackSyncCookie lastDirSyncCookieWithReplicationVectors {0}", (lastDirSyncCookieWithReplicationVectors != null) ? Convert.ToBase64String(lastDirSyncCookieWithReplicationVectors) : "NULL");
			this.SequenceId = (moreData ? sequenceId : Guid.NewGuid());
			this.SequenceStartTimestamp = (moreData ? sequenceStartTimestamp : DateTime.UtcNow);
			ExTraceGlobals.BackSyncTracer.TraceDebug<Guid, DateTime>((long)SyncConfiguration.TraceId, "BackSyncCookie this.SequenceId = {0} this.SequenceStartTimestamp = {1} ", this.SequenceId, this.SequenceStartTimestamp);
		}

		public BackSyncCookie(ServiceInstanceId serviceInstanceId) : this(DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, Guid.Empty, true, null, null, null, serviceInstanceId, Guid.NewGuid(), DateTime.UtcNow)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Create BackSyncCookie for NULL cookie input");
		}

		public bool MoreDirSyncData { get; private set; }

		public int Version { get; private set; }

		public DateTime Timestamp { get; private set; }

		public DateTime LastReadFailureStartTime { get; private set; }

		public DateTime LastWhenChanged { get; internal set; }

		public Guid InvocationId { get; private set; }

		public byte[] DirSyncCookie { get; private set; }

		public byte[] LastDirSyncCookieWithReplicationVectors { get; private set; }

		public Dictionary<string, int> ErrorObjectsAndFailureCounts { get; private set; }

		public ServiceInstanceId ServiceInstanceId { get; private set; }

		public DateTime SequenceStartTimestamp { get; private set; }

		public Guid SequenceId { get; private set; }

		public bool ReadyToMerge
		{
			get
			{
				return !this.MoreDirSyncData;
			}
		}

		public static BackSyncCookie Parse(byte[] binaryCookie)
		{
			if (binaryCookie == null)
			{
				ExTraceGlobals.BackSyncTracer.TraceError((long)SyncConfiguration.TraceId, "BackSyncCookie.Parse input binaryCookie is NULL");
				throw new ArgumentNullException("binaryCookie");
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "BackSyncCookie.Parse Read BackSync binary cookie \"{0}\"", Convert.ToBase64String(binaryCookie));
			Exception ex2;
			try
			{
				using (BackSyncCookieReader backSyncCookieReader = BackSyncCookieReader.Create(binaryCookie, typeof(BackSyncCookie)))
				{
					int num = (int)backSyncCookieReader.GetNextAttributeValue();
					ServiceInstanceId serviceInstanceId = new ServiceInstanceId((string)backSyncCookieReader.GetNextAttributeValue());
					long dateData = (long)backSyncCookieReader.GetNextAttributeValue();
					long dateData2 = (long)backSyncCookieReader.GetNextAttributeValue();
					Guid invocationId = (Guid)backSyncCookieReader.GetNextAttributeValue();
					bool moreData = (bool)backSyncCookieReader.GetNextAttributeValue();
					byte[] array = (byte[])backSyncCookieReader.GetNextAttributeValue();
					if (array != null)
					{
						ADDirSyncCookie.Parse(array);
					}
					string[] errorObjects = (string[])backSyncCookieReader.GetNextAttributeValue();
					Dictionary<string, int> errorObjects2 = BackSyncCookie.ParseErrorObjectsAndFailureCounts(errorObjects);
					long dateData3 = (long)backSyncCookieReader.GetNextAttributeValue();
					byte[] lastDirSyncCookieWithReplicationVectors = (byte[])backSyncCookieReader.GetNextAttributeValue();
					long dateData4 = (long)backSyncCookieReader.GetNextAttributeValue();
					Guid sequenceId = (Guid)backSyncCookieReader.GetNextAttributeValue();
					return new BackSyncCookie(DateTime.FromBinary(dateData), DateTime.FromBinary(dateData2), DateTime.FromBinary(dateData3), invocationId, moreData, array, errorObjects2, lastDirSyncCookieWithReplicationVectors, serviceInstanceId, sequenceId, DateTime.FromBinary(dateData4));
				}
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "BackSyncCookie.Parse ArgumentException {0}", ex.ToString());
				ex2 = ex;
			}
			catch (IOException ex3)
			{
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "BackSyncCookie.Parse IOException {0}", ex3.ToString());
				ex2 = ex3;
			}
			catch (FormatException ex4)
			{
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "BackSyncCookie.Parse FormatException {0}", ex4.ToString());
				ex2 = ex4;
			}
			ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "BackSyncCookie.Parse throw InvalidCookieException {0}", ex2.ToString());
			throw new InvalidCookieException(ex2);
		}

		public byte[] ToByteArray()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "BackSyncCookie.ToByteArray entering");
			byte[] result = null;
			using (BackSyncCookieWriter backSyncCookieWriter = BackSyncCookieWriter.Create(typeof(BackSyncCookie)))
			{
				backSyncCookieWriter.WriteNextAttributeValue(this.Version);
				backSyncCookieWriter.WriteNextAttributeValue(this.ServiceInstanceId.InstanceId);
				backSyncCookieWriter.WriteNextAttributeValue(this.Timestamp.ToBinary());
				backSyncCookieWriter.WriteNextAttributeValue(this.LastReadFailureStartTime.ToBinary());
				backSyncCookieWriter.WriteNextAttributeValue(this.InvocationId);
				backSyncCookieWriter.WriteNextAttributeValue(this.MoreDirSyncData);
				backSyncCookieWriter.WriteNextAttributeValue(this.DirSyncCookie);
				string[] attributeValue = BackSyncCookie.ConvertErrorObjectsAndFailureCountsToArray(this.ErrorObjectsAndFailureCounts);
				backSyncCookieWriter.WriteNextAttributeValue(attributeValue);
				backSyncCookieWriter.WriteNextAttributeValue(this.LastWhenChanged.ToBinary());
				backSyncCookieWriter.WriteNextAttributeValue(this.LastDirSyncCookieWithReplicationVectors);
				backSyncCookieWriter.WriteNextAttributeValue(this.SequenceStartTimestamp.ToBinary());
				backSyncCookieWriter.WriteNextAttributeValue(this.SequenceId);
				result = backSyncCookieWriter.GetBinaryCookie();
			}
			return result;
		}

		internal static Dictionary<string, int> ParseErrorObjectsAndFailureCounts(string[] errorObjects)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "BackSyncCookie.ParseErrorObjectsAndFailureCounts parse cookie errorObjects ...");
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "BackSyncCookie.ParseErrorObjectsAndFailureCounts errorObjects.Count = {0}", (errorObjects != null) ? errorObjects.Length : 0);
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			if (errorObjects != null)
			{
				foreach (string text in errorObjects)
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "BackSyncCookie.ParseErrorObjectsAndFailureCounts parse errorObject \"{0}\"", text);
					string[] array = text.Split(new char[]
					{
						';'
					}, StringSplitOptions.RemoveEmptyEntries);
					string text2 = array[0].Trim();
					ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "BackSyncCookie.ParseErrorObjectsAndFailureCounts object id \"{0}\"", text2);
					int num = Convert.ToInt32(array[1].Trim());
					ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "BackSyncCookie.ParseErrorObjectsAndFailureCounts failure count \"{0}\"", num);
					dictionary.Add(text2, num);
				}
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "BackSyncCookie.ParseErrorObjectsAndFailureCounts errorObjectsAndFailureCount.Count = {0}", dictionary.Count);
			return dictionary;
		}

		internal static string[] ConvertErrorObjectsAndFailureCountsToArray(Dictionary<string, int> errorObjectsAndFailureCounts)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "BackSyncCookie.ConvertErrorObjectsAndFailureCountsToArray this.ErrorObjectsAndFailureCounts.Count = {0}", errorObjectsAndFailureCounts.Count);
			string[] array = new string[errorObjectsAndFailureCounts.Count];
			int num = 0;
			foreach (KeyValuePair<string, int> keyValuePair in errorObjectsAndFailureCounts)
			{
				array[num] = string.Format("{0}{1}{2}", keyValuePair.Key, ';', keyValuePair.Value);
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "BackSyncCookie.ConvertErrorObjectsAndFailureCountsToArray \"{0}\"", array[num]);
				num++;
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "BackSyncCookie.ConvertErrorObjectsAndFailureCountsToArray errorObjects.Count = {0}", array.Length);
			return array;
		}

		internal const int CurrentVersion = 4;

		internal const char BACKSYNC_ERROR_OBJECT_COUNT_SEPARATOR = ';';

		internal static BackSyncCookieAttribute[] BackSyncCookieAttributeSchema_Version_1 = new BackSyncCookieAttribute[]
		{
			new BackSyncCookieAttribute
			{
				Name = "TimeStampRaw",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "LastReadFailureStartTimeRaw",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "InvocationId",
				DataType = typeof(Guid),
				DefaultValue = Guid.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "MoreDirSyncData",
				DataType = typeof(bool),
				DefaultValue = false
			},
			new BackSyncCookieAttribute
			{
				Name = "DirSyncCookie",
				DataType = typeof(byte[]),
				DefaultValue = null
			},
			new BackSyncCookieAttribute
			{
				Name = "ErrorObjectsAndFailureCounts",
				DataType = typeof(string[]),
				DefaultValue = null
			}
		};

		internal static BackSyncCookieAttribute[] BackSyncCookieAttributeSchema_Version_2 = new BackSyncCookieAttribute[]
		{
			new BackSyncCookieAttribute
			{
				Name = "LastWhenChanged",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			}
		};

		internal static BackSyncCookieAttribute[] BackSyncCookieAttributeSchema_Version_3 = new BackSyncCookieAttribute[]
		{
			new BackSyncCookieAttribute
			{
				Name = "LastDirSyncCookieWithReplicationVectors",
				DataType = typeof(byte[]),
				DefaultValue = null
			}
		};

		internal static BackSyncCookieAttribute[] BackSyncCookieAttributeSchema_Version_4 = new BackSyncCookieAttribute[]
		{
			new BackSyncCookieAttribute
			{
				Name = "SequenceStartTimeRaw",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "SequenceId",
				DataType = typeof(Guid),
				DefaultValue = Guid.Empty
			}
		};

		internal static BackSyncCookieAttribute[][] BackSyncCookieAttributeSchemaByVersions = new BackSyncCookieAttribute[][]
		{
			BackSyncCookieAttribute.BackSyncCookieVersionSchema,
			BackSyncCookie.BackSyncCookieAttributeSchema_Version_1,
			BackSyncCookie.BackSyncCookieAttributeSchema_Version_2,
			BackSyncCookie.BackSyncCookieAttributeSchema_Version_3,
			BackSyncCookie.BackSyncCookieAttributeSchema_Version_4
		};
	}
}
