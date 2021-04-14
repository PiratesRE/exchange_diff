using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal sealed class ObjectFullSyncPageToken : IFullSyncPageToken, ISyncCookie
	{
		public ObjectFullSyncPageToken(Guid invocationId, ICollection<SyncObjectId> objectIds, BackSyncOptions syncOptions, ServiceInstanceId serviceInstanceId) : this(invocationId, objectIds, syncOptions, DateTime.UtcNow, DateTime.MinValue, null, null, serviceInstanceId, Guid.NewGuid(), DateTime.UtcNow)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "New ObjectFullSyncPageToken");
		}

		public ObjectFullSyncPageToken(Guid invocationId, ICollection<SyncObjectId> objectIds, BackSyncOptions syncOptions, DateTime timestamp, DateTime lastReadFailureStartTime, FullSyncObjectCookie objectCookie, Dictionary<string, int> errorObjectsAndCount, ServiceInstanceId serviceInstanceId, Guid sequenceId, DateTime sequenceStartTimestamp)
		{
			this.Version = 2;
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "Version {0}", this.Version);
			this.Timestamp = timestamp;
			ExTraceGlobals.BackSyncTracer.TraceDebug<DateTime>((long)SyncConfiguration.TraceId, "Timestamp {0}", this.Timestamp);
			this.LastReadFailureStartTime = lastReadFailureStartTime;
			ExTraceGlobals.BackSyncTracer.TraceDebug<DateTime>((long)SyncConfiguration.TraceId, "LastReadFailureStartTime {0}", this.LastReadFailureStartTime);
			this.InvocationId = invocationId;
			ExTraceGlobals.BackSyncTracer.TraceDebug<Guid>((long)SyncConfiguration.TraceId, "InvocationId {0}", this.InvocationId);
			this.ObjectIds = new HashSet<SyncObjectId>(objectIds);
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "ObjectIds count = {0}", this.ObjectIds.Count);
			this.SyncOptions = syncOptions;
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "SyncOptions {0}", this.SyncOptions.ToString());
			this.ServiceInstanceId = serviceInstanceId;
			ExTraceGlobals.BackSyncTracer.TraceDebug<ServiceInstanceId>((long)SyncConfiguration.TraceId, "SyncServiceInstanceId {0}", this.ServiceInstanceId);
			this.ObjectCookie = objectCookie;
			this.ErrorObjectsAndFailureCounts = (errorObjectsAndCount ?? new Dictionary<string, int>());
			this.SequenceId = sequenceId;
			this.SequenceStartTimestamp = sequenceStartTimestamp;
			ExTraceGlobals.BackSyncTracer.TraceDebug<Guid, DateTime>((long)SyncConfiguration.TraceId, "BackSyncCookie this.SequenceId = {0} this.SequenceStartTimestamp = {1} ", this.SequenceId, this.SequenceStartTimestamp);
		}

		public BackSyncOptions SyncOptions { get; private set; }

		public bool MoreData
		{
			get
			{
				return this.ObjectIds.Count > 0;
			}
		}

		public DateTime Timestamp { get; set; }

		public DateTime LastReadFailureStartTime { get; set; }

		public Dictionary<string, int> ErrorObjectsAndFailureCounts { get; private set; }

		internal ServiceInstanceId ServiceInstanceId { get; private set; }

		public DateTime SequenceStartTimestamp { get; private set; }

		public Guid SequenceId { get; private set; }

		public byte[] ToByteArray()
		{
			byte[] result = null;
			using (BackSyncCookieWriter backSyncCookieWriter = BackSyncCookieWriter.Create(typeof(ObjectFullSyncPageToken)))
			{
				backSyncCookieWriter.WriteNextAttributeValue(this.Version);
				backSyncCookieWriter.WriteNextAttributeValue(this.ServiceInstanceId.InstanceId);
				backSyncCookieWriter.WriteNextAttributeValue(this.Timestamp.ToBinary());
				backSyncCookieWriter.WriteNextAttributeValue(this.LastReadFailureStartTime.ToBinary());
				backSyncCookieWriter.WriteNextAttributeValue(this.InvocationId);
				backSyncCookieWriter.WriteNextAttributeValue((int)this.SyncOptions);
				List<string> list = new List<string>();
				foreach (SyncObjectId syncObjectId in this.ObjectIds)
				{
					list.Add(syncObjectId.ToString());
				}
				backSyncCookieWriter.WriteNextAttributeValue(list.ToArray());
				if (this.ObjectCookie != null)
				{
					backSyncCookieWriter.WriteNextAttributeValue(this.ObjectCookie.ToByteArray());
				}
				else
				{
					backSyncCookieWriter.WriteNextAttributeValue(null);
				}
				string[] attributeValue = BackSyncCookie.ConvertErrorObjectsAndFailureCountsToArray(this.ErrorObjectsAndFailureCounts);
				backSyncCookieWriter.WriteNextAttributeValue(attributeValue);
				backSyncCookieWriter.WriteNextAttributeValue(this.SequenceStartTimestamp.ToBinary());
				backSyncCookieWriter.WriteNextAttributeValue(this.SequenceId);
				result = backSyncCookieWriter.GetBinaryCookie();
			}
			return result;
		}

		public void PrepareForFailover()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncPageToken PrepareForFailover ...");
		}

		public int Version { get; private set; }

		public Guid InvocationId { get; private set; }

		public ICollection<SyncObjectId> ObjectIds { get; private set; }

		public FullSyncObjectCookie ObjectCookie { get; internal set; }

		public void RemoveObjectFromList(SyncObjectId objectId, bool clearObjectCookie)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "ObjectFullSyncPageToken RemoveObjectFromList objectId {0}", (objectId != null) ? objectId.ObjectId : "NULL");
			this.ObjectIds.Remove(objectId);
			if (clearObjectCookie)
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "ObjectFullSyncPageToken clear object cookie");
				this.ObjectCookie = null;
			}
		}

		internal static ObjectFullSyncPageToken Parse(byte[] tokenBytes)
		{
			if (tokenBytes == null)
			{
				throw new ArgumentNullException("tokenBytes");
			}
			Exception innerException;
			try
			{
				using (BackSyncCookieReader backSyncCookieReader = BackSyncCookieReader.Create(tokenBytes, typeof(ObjectFullSyncPageToken)))
				{
					int num = (int)backSyncCookieReader.GetNextAttributeValue();
					ServiceInstanceId serviceInstanceId = new ServiceInstanceId((string)backSyncCookieReader.GetNextAttributeValue());
					long dateData = (long)backSyncCookieReader.GetNextAttributeValue();
					long dateData2 = (long)backSyncCookieReader.GetNextAttributeValue();
					Guid invocationId = (Guid)backSyncCookieReader.GetNextAttributeValue();
					BackSyncOptions syncOptions = (BackSyncOptions)((int)backSyncCookieReader.GetNextAttributeValue());
					string[] array = (string[])backSyncCookieReader.GetNextAttributeValue();
					byte[] array2 = (byte[])backSyncCookieReader.GetNextAttributeValue();
					string[] errorObjects = (string[])backSyncCookieReader.GetNextAttributeValue();
					Dictionary<string, int> errorObjectsAndCount = BackSyncCookie.ParseErrorObjectsAndFailureCounts(errorObjects);
					DateTime sequenceStartTimestamp = DateTime.FromBinary((long)backSyncCookieReader.GetNextAttributeValue());
					Guid sequenceId = (Guid)backSyncCookieReader.GetNextAttributeValue();
					HashSet<SyncObjectId> hashSet = new HashSet<SyncObjectId>();
					if (array != null)
					{
						foreach (string identity in array)
						{
							hashSet.Add(SyncObjectId.Parse(identity));
						}
					}
					FullSyncObjectCookie objectCookie = (array2 == null) ? null : FullSyncObjectCookie.Parse(array2);
					return new ObjectFullSyncPageToken(invocationId, hashSet, syncOptions, DateTime.FromBinary(dateData), DateTime.FromBinary(dateData2), objectCookie, errorObjectsAndCount, serviceInstanceId, sequenceId, sequenceStartTimestamp);
				}
			}
			catch (ArgumentException ex)
			{
				innerException = ex;
			}
			catch (IOException ex2)
			{
				innerException = ex2;
			}
			catch (FormatException ex3)
			{
				innerException = ex3;
			}
			catch (InvalidCookieException ex4)
			{
				innerException = ex4;
			}
			throw new InvalidCookieException(innerException);
		}

		internal const int CurrentVersion = 2;

		internal static BackSyncCookieAttribute[] ObjectFullSyncPageTokenAttributeSchema_Version_1 = new BackSyncCookieAttribute[]
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
				Name = "SyncOptions",
				DataType = typeof(int),
				DefaultValue = 0
			},
			new BackSyncCookieAttribute
			{
				Name = "ObjectIds",
				DataType = typeof(string[]),
				DefaultValue = null
			},
			new BackSyncCookieAttribute
			{
				Name = "ObjectCookie",
				DataType = typeof(byte[]),
				DefaultValue = null
			}
		};

		internal static BackSyncCookieAttribute[] ObjectFullSyncPageTokenAttributeSchema_Version_2 = new BackSyncCookieAttribute[]
		{
			new BackSyncCookieAttribute
			{
				Name = "ErrorObjectsAndFailureCounts",
				DataType = typeof(string[]),
				DefaultValue = null
			}
		};

		internal static BackSyncCookieAttribute[] ObjectFullSyncPageTokenAttributeSchema_Version_3 = new BackSyncCookieAttribute[]
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

		internal static BackSyncCookieAttribute[][] ObjectFullSyncPageTokenAttributeSchemaByVersions = new BackSyncCookieAttribute[][]
		{
			BackSyncCookieAttribute.BackSyncCookieVersionSchema,
			ObjectFullSyncPageToken.ObjectFullSyncPageTokenAttributeSchema_Version_1,
			ObjectFullSyncPageToken.ObjectFullSyncPageTokenAttributeSchema_Version_2,
			ObjectFullSyncPageToken.ObjectFullSyncPageTokenAttributeSchema_Version_3
		};
	}
}
