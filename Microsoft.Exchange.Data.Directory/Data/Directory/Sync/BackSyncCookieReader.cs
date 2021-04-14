using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class BackSyncCookieReader : IDisposable
	{
		internal static BackSyncCookieReader Create(byte[] binaryCookie, Type cookieType)
		{
			int attributeCount = 0;
			BackSyncCookieAttribute[] cookieAttributeDefinitions = null;
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "Cookie Type {0}", cookieType.Name);
			BackSyncCookieAttribute.CreateBackSyncCookieAttributeDefinitions(binaryCookie, cookieType, out attributeCount, out cookieAttributeDefinitions);
			return new BackSyncCookieReader(attributeCount, cookieAttributeDefinitions, binaryCookie);
		}

		internal BackSyncCookieReader(int attributeCount, BackSyncCookieAttribute[] cookieAttributeDefinitions, byte[] binaryCookie)
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "New BackSyncCookieReader");
			this.cookieAttributeCount = attributeCount;
			this.currentAttributeIndex = 0;
			this.attributeDefinitions = cookieAttributeDefinitions;
			this.cookieMemoryStream = new MemoryStream(binaryCookie);
			this.cookieBinaryReader = new BinaryReader(this.cookieMemoryStream);
			this.disposed = false;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal object GetNextAttributeValue()
		{
			ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Get next attribute value");
			BackSyncCookieAttribute backSyncCookieAttribute = this.attributeDefinitions[this.currentAttributeIndex];
			ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "Attribute {0}", backSyncCookieAttribute.ToString());
			object obj = backSyncCookieAttribute.DefaultValue;
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "this.cookieAttributeCount = {0}", this.cookieAttributeCount);
			ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "this.currentAttributeIndex = {0}", this.currentAttributeIndex);
			if (this.currentAttributeIndex < this.cookieAttributeCount)
			{
				if (backSyncCookieAttribute.DataType.Equals(typeof(int)))
				{
					obj = this.cookieBinaryReader.ReadInt32();
					ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "attributeValue (Int32) = {0}", new object[]
					{
						obj
					});
				}
				else if (backSyncCookieAttribute.DataType.Equals(typeof(long)))
				{
					obj = this.cookieBinaryReader.ReadInt64();
					ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "attributeValue (Int64) = {0}", new object[]
					{
						obj
					});
				}
				else if (backSyncCookieAttribute.DataType.Equals(typeof(bool)))
				{
					obj = this.cookieBinaryReader.ReadBoolean();
					ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "attributeValue (Boolean) = {0}", new object[]
					{
						obj
					});
				}
				else if (backSyncCookieAttribute.DataType.Equals(typeof(Guid)))
				{
					obj = new Guid(this.cookieBinaryReader.ReadBytes(16));
					ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "attributeValue (Guid) = {0}", new object[]
					{
						obj
					});
				}
				else if (backSyncCookieAttribute.DataType.Equals(typeof(string)))
				{
					obj = this.cookieBinaryReader.ReadString();
					ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "attributeValue (string) = {0}", new object[]
					{
						obj
					});
				}
				else if (backSyncCookieAttribute.DataType.Equals(typeof(byte[])))
				{
					int num = this.cookieBinaryReader.ReadInt32();
					ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "attributeValue (bypte[]) size = {0}", num);
					if (num > 0)
					{
						obj = this.cookieBinaryReader.ReadBytes(num);
						ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, "  Base64String {0}", Convert.ToBase64String((byte[])obj));
					}
					else
					{
						obj = null;
					}
				}
				else
				{
					if (!backSyncCookieAttribute.DataType.Equals(typeof(string[])))
					{
						ExTraceGlobals.BackSyncTracer.TraceError<string>((long)SyncConfiguration.TraceId, "Invalid attribute data type {0}", backSyncCookieAttribute.DataType.Name);
						throw new InvalidCookieException();
					}
					int num2 = this.cookieBinaryReader.ReadInt32();
					ExTraceGlobals.BackSyncTracer.TraceDebug<int>((long)SyncConfiguration.TraceId, "attributeValue (string[]) size = {0}", num2);
					if (num2 > 0)
					{
						List<string> list = new List<string>();
						for (int i = 0; i < num2; i++)
						{
							string text = this.cookieBinaryReader.ReadString();
							ExTraceGlobals.BackSyncTracer.TraceDebug<int, string>((long)SyncConfiguration.TraceId, "  value[{0}] = \"{1}\"", i, text);
							list.Add(text);
						}
						obj = list.ToArray();
					}
					else
					{
						obj = null;
					}
				}
			}
			else
			{
				obj = backSyncCookieAttribute.DefaultValue;
				ExTraceGlobals.BackSyncTracer.TraceDebug<string>((long)SyncConfiguration.TraceId, " attributeValue (Default) = {0}", (obj != null) ? obj.ToString() : "NULL");
			}
			this.currentAttributeIndex++;
			return obj;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "Dispose BackSyncCookieReader");
					if (this.cookieBinaryReader != null)
					{
						this.cookieBinaryReader.Close();
					}
					if (this.cookieMemoryStream != null)
					{
						this.cookieMemoryStream.Close();
					}
				}
				this.disposed = true;
			}
		}

		private readonly int cookieAttributeCount;

		private int currentAttributeIndex;

		private BackSyncCookieAttribute[] attributeDefinitions;

		private MemoryStream cookieMemoryStream;

		private BinaryReader cookieBinaryReader;

		private bool disposed;
	}
}
