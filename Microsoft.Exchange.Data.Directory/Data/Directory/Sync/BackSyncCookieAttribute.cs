using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class BackSyncCookieAttribute
	{
		internal static void CreateBackSyncCookieAttributeDefinitions(byte[] binaryCookie, Type cookieType, out int cookieAttributeCount, out BackSyncCookieAttribute[] backSyncCookieAttributeDefinitions)
		{
			BackSyncCookieAttribute[][] array = null;
			int num = 0;
			if (cookieType.Equals(typeof(BackSyncCookie)))
			{
				array = BackSyncCookie.BackSyncCookieAttributeSchemaByVersions;
				num = 4;
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Cookie schema BackSyncCookie.BackSyncCookieAttributeSchemaByVersions");
			}
			else if (cookieType.Equals(typeof(ObjectFullSyncPageToken)))
			{
				array = ObjectFullSyncPageToken.ObjectFullSyncPageTokenAttributeSchemaByVersions;
				num = 2;
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Cookie schema ObjectFullSyncPageToken.ObjectFullSyncPageTokenAttributeSchemaByVersions");
			}
			else if (cookieType.Equals(typeof(TenantFullSyncPageToken)))
			{
				array = TenantFullSyncPageToken.TenantFullSyncPageTokenAttributeSchemaByVersions;
				num = 3;
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Cookie schema TenantFullSyncPageToken.TenantFullSyncPageTokenAttributeSchemaByVersions");
			}
			else if (cookieType.Equals(typeof(FullSyncObjectCookie)))
			{
				array = FullSyncObjectCookie.FullSyncObjectCookieAttributeSchemaByVersions;
				num = 1;
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Cookie schema FullSyncObjectCookie.FullSyncObjectCookieAttributeSchemaByVersions");
			}
			else if (cookieType.Equals(typeof(MergePageToken)))
			{
				array = MergePageToken.MergePageTokenAttributeSchemaByVersions;
				num = 2;
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Cookie schema MergePageToken.MergePageTokenAttributeSchemaByVersions");
			}
			else if (cookieType.Equals(typeof(TenantRelocationSyncPageToken)))
			{
				array = TenantRelocationSyncPageToken.TenantRelocationSyncPageTokenAttributeSchemaByVersions;
				num = 1;
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Cookie schema TenantRelocationSyncPageToken.TenantRelocationSyncPageTokenAttributeSchemaByVersions");
			}
			else
			{
				ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "Invalid cookie type {0}", cookieType.Name);
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "Cookie schema attribute version count = {0}", array.Length);
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "Cookie schema current version (version starts from 0) = {0}", num);
			int num2 = num;
			if (binaryCookie != null)
			{
				ServiceInstanceId arg;
				BackSyncCookieAttribute.ReadCookieVersion(binaryCookie, out num2, out arg);
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "Binary cookie version = {0}", num2);
				ExTraceGlobals.BackSyncTracer.TraceDebug<ServiceInstanceId>((long)SyncConfiguration.TraceId, "Binary cookie service instance id = {0}", arg);
				if (num2 < 0)
				{
					ExTraceGlobals.BackSyncTracer.TraceError((long)SyncConfiguration.TraceId, "Cookie version is less than zero");
					throw new InvalidCookieException();
				}
				num2 = Math.Min(num2, num);
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "Parse cookie using version {0}", num2);
			int num3 = 0;
			int num4 = 0;
			List<BackSyncCookieAttribute> list = new List<BackSyncCookieAttribute>();
			foreach (BackSyncCookieAttribute[] array3 in array)
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "Add attribute from cookie schema version {0}", num4);
				foreach (BackSyncCookieAttribute backSyncCookieAttribute in array3)
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug<string, string>((long)SyncConfiguration.TraceId, "Add attribute {0} ({1})", backSyncCookieAttribute.Name, backSyncCookieAttribute.DataType.Name);
					list.Add(backSyncCookieAttribute);
					if (num4 <= num2)
					{
						num3++;
						ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "Attribute count = {0}", num3);
					}
				}
				num4++;
			}
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "Return attribute count = {0}", num3);
			cookieAttributeCount = num3;
			backSyncCookieAttributeDefinitions = list.ToArray();
		}

		private static void ReadCookieVersion(byte[] binaryCookie, out int cookieVersion, out ServiceInstanceId cookieServiceInstanceId)
		{
			int num = 0;
			string serviceInstanceId;
			using (MemoryStream memoryStream = new MemoryStream(binaryCookie))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					num = binaryReader.ReadInt32();
					serviceInstanceId = binaryReader.ReadString();
				}
			}
			cookieVersion = num;
			cookieServiceInstanceId = new ServiceInstanceId(serviceInstanceId);
		}

		internal string Name { get; set; }

		internal Type DataType { get; set; }

		internal object DefaultValue { get; set; }

		public override string ToString()
		{
			return string.Format("Name ({0}), Data Type ({1}), Default Value ({2})", this.Name, this.DataType, (this.DefaultValue != null) ? this.DefaultValue.ToString() : "NULL");
		}

		internal static BackSyncCookieAttribute[] BackSyncCookieVersionSchema = new BackSyncCookieAttribute[]
		{
			new BackSyncCookieAttribute
			{
				Name = "Version",
				DataType = typeof(int),
				DefaultValue = 0
			},
			new BackSyncCookieAttribute
			{
				Name = "ServiceInstanceId",
				DataType = typeof(string),
				DefaultValue = string.Empty
			}
		};
	}
}
