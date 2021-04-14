using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ExternalRmsServerInfoMap
	{
		public ExternalRmsServerInfoMap(string path, int maxCount, IMruDictionaryPerfCounters perfCounters)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			string uniqueFileNameForProcess = RmsClientManagerUtils.GetUniqueFileNameForProcess("ExternalRmsServerInfo_1.dat");
			this.dictionary = new MruDictionary<Uri, ExternalRMSServerInfo>(maxCount, new ExternalRmsServerInfoMap.UriComparer(), perfCounters);
			this.serializer = new MruDictionarySerializer<Uri, ExternalRMSServerInfo>(path, uniqueFileNameForProcess, ExternalRMSServerInfo.ColumnNames, new MruDictionarySerializerRead<Uri, ExternalRMSServerInfo>(ExternalRmsServerInfoMap.TryReadValues), new MruDictionarySerializerWrite<Uri, ExternalRMSServerInfo>(ExternalRmsServerInfoMap.TryWriteValues), perfCounters);
			if (!this.serializer.TryReadFromDisk(this.dictionary))
			{
				ExternalRmsServerInfoMap.Tracer.TraceError<string>(0L, "External Rms Server Info Map failed to read map-file ({0}).", uniqueFileNameForProcess);
			}
		}

		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		public bool TryGet(Uri key, out ExternalRMSServerInfo entry)
		{
			if (key == null)
			{
				entry = null;
				return false;
			}
			if (!this.dictionary.TryGetValue(key, out entry))
			{
				return false;
			}
			if (entry.ExpiryTime != DateTime.MaxValue && entry.ExpiryTime < DateTime.UtcNow)
			{
				ExternalRmsServerInfoMap.Tracer.TraceDebug<Uri>(0L, "External Rms Server Info Map removed expired negative entry for key ({0}).", key);
				this.Remove(key);
				entry = null;
				return false;
			}
			return true;
		}

		public void Add(ExternalRMSServerInfo entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			this.dictionary.Add(entry.KeyUri, entry);
			this.serializer.TryWriteToDisk(this.dictionary);
		}

		public void Remove(Uri key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this.dictionary.Remove(key))
			{
				this.serializer.TryWriteToDisk(this.dictionary);
			}
		}

		internal static bool TryReadValues(string[] values, out Uri key, out ExternalRMSServerInfo value)
		{
			if (ExternalRMSServerInfo.TryParse(values, out value))
			{
				key = value.KeyUri;
				return true;
			}
			key = null;
			return false;
		}

		internal static bool TryWriteValues(Uri key, ExternalRMSServerInfo value, out string[] values)
		{
			if (key == null || value == null)
			{
				ExternalRmsServerInfoMap.Tracer.TraceDebug(0L, "External Rms Server Info Map failed to write values.");
				values = null;
				return false;
			}
			values = value.ToStringArray();
			return true;
		}

		private const string MapFileSuffix = "ExternalRmsServerInfo_1.dat";

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		private MruDictionary<Uri, ExternalRMSServerInfo> dictionary;

		private MruDictionarySerializer<Uri, ExternalRMSServerInfo> serializer;

		internal sealed class UriComparer : IComparer<Uri>
		{
			int IComparer<Uri>.Compare(Uri x, Uri y)
			{
				if (x == null && y == null)
				{
					return 0;
				}
				if (x == null)
				{
					return -1;
				}
				if (y == null)
				{
					return 1;
				}
				return Uri.Compare(x, y, UriComponents.HostAndPort, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase);
			}
		}
	}
}
