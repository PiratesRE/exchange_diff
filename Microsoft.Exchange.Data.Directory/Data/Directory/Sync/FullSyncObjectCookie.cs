using System;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal sealed class FullSyncObjectCookie
	{
		private FullSyncObjectCookie()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "New FullSyncObjectCookie");
			this.Version = 1;
		}

		internal FullSyncObjectCookie(SyncObjectId objectId, LinkMetadata overlapLink, int nextRangeStart, long usnChanged, ServiceInstanceId serviceInstanceid) : this()
		{
			this.ReadRestartsCount = 0;
			this.SetNextPageData(objectId, overlapLink, nextRangeStart, usnChanged);
			this.ServiceInstanceId = serviceInstanceid;
		}

		internal int Version { get; private set; }

		internal long UsnChanged { get; private set; }

		internal int ReadRestartsCount { get; private set; }

		internal bool EnumerateLinks { get; private set; }

		internal int LinkRangeStart { get; private set; }

		internal int LinkVersion { get; private set; }

		internal string LinkTarget { get; private set; }

		internal string LinkName { get; private set; }

		internal SyncObjectId ObjectId { get; private set; }

		internal ServiceInstanceId ServiceInstanceId { get; private set; }

		public static FullSyncObjectCookie Parse(byte[] bytes)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "FullSyncObjectCookie.Parse entering");
			if (bytes == null)
			{
				ExTraceGlobals.BackSyncTracer.TraceError((long)SyncConfiguration.TraceId, "FullSyncObjectCookie.Parse bytes is NULL");
				throw new ArgumentNullException("bytes");
			}
			Exception ex2;
			try
			{
				using (BackSyncCookieReader backSyncCookieReader = BackSyncCookieReader.Create(bytes, typeof(FullSyncObjectCookie)))
				{
					return new FullSyncObjectCookie
					{
						Version = (int)backSyncCookieReader.GetNextAttributeValue(),
						ServiceInstanceId = new ServiceInstanceId((string)backSyncCookieReader.GetNextAttributeValue()),
						ObjectId = SyncObjectId.Parse((string)backSyncCookieReader.GetNextAttributeValue()),
						ReadRestartsCount = (int)backSyncCookieReader.GetNextAttributeValue(),
						UsnChanged = (long)backSyncCookieReader.GetNextAttributeValue(),
						EnumerateLinks = (bool)backSyncCookieReader.GetNextAttributeValue(),
						LinkRangeStart = (int)backSyncCookieReader.GetNextAttributeValue(),
						LinkVersion = (int)backSyncCookieReader.GetNextAttributeValue(),
						LinkTarget = (string)backSyncCookieReader.GetNextAttributeValue(),
						LinkName = (string)backSyncCookieReader.GetNextAttributeValue()
					};
				}
			}
			catch (ArgumentException ex)
			{
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "FullSyncObjectCookie.Parse ArgumentException {0}", ex.ToString());
				ex2 = ex;
			}
			catch (IOException ex3)
			{
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "FullSyncObjectCookie.Parse IOException {0}", ex3.ToString());
				ex2 = ex3;
			}
			catch (FormatException ex4)
			{
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "FullSyncObjectCookie.Parse FormatException {0}", ex4.ToString());
				ex2 = ex4;
			}
			ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "FullSyncObjectCookie.Parse throw InvalidCookieException {0}", ex2.ToString());
			throw new InvalidCookieException(ex2);
		}

		public byte[] ToByteArray()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "FullSyncObjectCookie.ToByteArray entering");
			byte[] result = null;
			using (BackSyncCookieWriter backSyncCookieWriter = BackSyncCookieWriter.Create(typeof(FullSyncObjectCookie)))
			{
				backSyncCookieWriter.WriteNextAttributeValue(this.Version);
				backSyncCookieWriter.WriteNextAttributeValue(this.ServiceInstanceId.InstanceId);
				backSyncCookieWriter.WriteNextAttributeValue(this.ObjectId.ToString());
				backSyncCookieWriter.WriteNextAttributeValue(this.ReadRestartsCount);
				backSyncCookieWriter.WriteNextAttributeValue(this.UsnChanged);
				backSyncCookieWriter.WriteNextAttributeValue(this.EnumerateLinks);
				backSyncCookieWriter.WriteNextAttributeValue(this.LinkRangeStart);
				backSyncCookieWriter.WriteNextAttributeValue(this.LinkVersion);
				backSyncCookieWriter.WriteNextAttributeValue(this.LinkTarget);
				backSyncCookieWriter.WriteNextAttributeValue(this.LinkName);
				result = backSyncCookieWriter.GetBinaryCookie();
			}
			return result;
		}

		public bool ContainsOverlapLink(LinkMetadata metadata)
		{
			bool flag = metadata != null && metadata.AttributeName.Equals(this.LinkName, StringComparison.OrdinalIgnoreCase) && metadata.TargetDistinguishedName.Equals(this.LinkTarget, StringComparison.OrdinalIgnoreCase) && metadata.Version == this.LinkVersion;
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool>((long)SyncConfiguration.TraceId, "FullSyncObjectCookie.ContainsOverlapLink return {0}", flag);
			return flag;
		}

		public void RestartObjectReadAfterTargetLinksChange()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "FullSyncObjectCookie.RestartObjectReadAfterTargetLinksChange entering");
			this.RestartObjectRead(true);
		}

		public void RestartObjectReadAfterObjectChange(long usnChanged)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "FullSyncObjectCookie.RestartObjectReadAfterObjectChange entering");
			this.RestartObjectRead(false);
			this.UsnChanged = usnChanged;
		}

		public void RestartObjectRead(bool restartOnlyLinkEnumeration)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug<bool>((long)SyncConfiguration.TraceId, "FullSyncObjectCookie.RestartObjectRead (restartOnlyLinkEnumeration = {0}) entering", restartOnlyLinkEnumeration);
			this.EnumerateLinks = restartOnlyLinkEnumeration;
			this.LinkName = null;
			this.LinkRangeStart = 0;
			this.LinkTarget = null;
			this.LinkVersion = 0;
			this.ReadRestartsCount++;
		}

		public void SetNextPageData(SyncObjectId objectId, LinkMetadata overlapLink, int nextRangeStart, long usnChanged)
		{
			this.ObjectId = objectId;
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "FullSyncObjectCookie.SetNextPageData this.ObjectId = {0}", (this.ObjectId != null) ? this.ObjectId.ObjectId : "NULL");
			this.EnumerateLinks = true;
			this.LinkName = overlapLink.AttributeName;
			this.LinkRangeStart = nextRangeStart;
			this.LinkTarget = overlapLink.TargetDistinguishedName;
			this.LinkVersion = overlapLink.Version;
			this.UsnChanged = usnChanged;
		}

		internal const int CurrentVersion = 1;

		internal static BackSyncCookieAttribute[] FullSyncObjectCookieAttributeSchema_Version_1 = new BackSyncCookieAttribute[]
		{
			new BackSyncCookieAttribute
			{
				Name = "ObjectId",
				DataType = typeof(string),
				DefaultValue = string.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "ReadRestartsCount",
				DataType = typeof(int),
				DefaultValue = 0
			},
			new BackSyncCookieAttribute
			{
				Name = "UsnChanged",
				DataType = typeof(long),
				DefaultValue = Convert.ToInt64(0)
			},
			new BackSyncCookieAttribute
			{
				Name = "EnumerateLinks",
				DataType = typeof(bool),
				DefaultValue = false
			},
			new BackSyncCookieAttribute
			{
				Name = "LinkRangeStart",
				DataType = typeof(int),
				DefaultValue = 0
			},
			new BackSyncCookieAttribute
			{
				Name = "LinkVersion",
				DataType = typeof(int),
				DefaultValue = 0
			},
			new BackSyncCookieAttribute
			{
				Name = "LinkTarget",
				DataType = typeof(string),
				DefaultValue = string.Empty
			},
			new BackSyncCookieAttribute
			{
				Name = "LinkName",
				DataType = typeof(string),
				DefaultValue = string.Empty
			}
		};

		internal static BackSyncCookieAttribute[][] FullSyncObjectCookieAttributeSchemaByVersions = new BackSyncCookieAttribute[][]
		{
			BackSyncCookieAttribute.BackSyncCookieVersionSchema,
			FullSyncObjectCookie.FullSyncObjectCookieAttributeSchema_Version_1
		};
	}
}
