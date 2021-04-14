using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.FastTransfer;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class FastTransferPropertyBag : DisposableBase, IPropertyBag
	{
		public FastTransferPropertyBag(FastTransferDownloadContext downloadContext, MapiPropBagBase mapiPropBag, bool excludeProps, HashSet<StorePropTag> propList)
		{
			this.context = downloadContext;
			this.mapiPropBag = mapiPropBag;
			this.excludeProps = excludeProps;
			this.propList = propList;
			this.forUpload = ((byte)(downloadContext.SendOptions & FastTransferSendOption.Upload) == 3);
		}

		public FastTransferPropertyBag(FastTransferUploadContext uploadContext, MapiPropBagBase mapiPropBag)
		{
			this.context = uploadContext;
			this.mapiPropBag = mapiPropBag;
			this.excludeProps = true;
			this.propList = null;
		}

		public MapiPropBagBase MapiPropBag
		{
			get
			{
				return this.mapiPropBag;
			}
			set
			{
				this.mapiPropBag = value;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return this.context is FastTransferDownloadContext;
			}
		}

		public FastTransferContext Context
		{
			get
			{
				return this.context;
			}
		}

		public FastTransferDownloadContext DownloadContext
		{
			get
			{
				return this.context as FastTransferDownloadContext;
			}
		}

		public FastTransferUploadContext UploadContext
		{
			get
			{
				return this.context as FastTransferUploadContext;
			}
		}

		public bool ExcludeProps
		{
			get
			{
				return this.excludeProps;
			}
		}

		public HashSet<StorePropTag> PropList
		{
			get
			{
				return this.propList;
			}
		}

		public ISession Session
		{
			get
			{
				return this.context;
			}
		}

		internal IEnumerable<PropertyTag> GetPropertyList()
		{
			this.loadedProperties = this.LoadAllPropertiesImp();
			if (ExTraceGlobals.SourceSendTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(200);
				stringBuilder.Append("Send Props=[");
				stringBuilder.AppendAsString(this.loadedProperties);
				stringBuilder.Append("]");
				ExTraceGlobals.SourceSendTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			foreach (Property property in this.loadedProperties)
			{
				Property property2 = property;
				yield return new PropertyTag(property2.Tag.PropTag);
			}
			yield break;
		}

		public AnnotatedPropertyValue GetAnnotatedProperty(PropertyTag propertyTag)
		{
			PropertyValue property = this.GetProperty(propertyTag);
			PropertyTag propertyTag2 = (property.PropertyTag.PropertyType == PropertyType.Error) ? propertyTag : property.PropertyTag;
			NamedProperty namedProperty = null;
			if (propertyTag2.IsNamedProperty)
			{
				this.Session.TryResolveToNamedProperty(propertyTag2, out namedProperty);
			}
			return new AnnotatedPropertyValue(propertyTag2, property, namedProperty);
		}

		public IEnumerable<AnnotatedPropertyValue> GetAnnotatedProperties()
		{
			foreach (PropertyTag propertyTag in this.GetPropertyList())
			{
				yield return this.GetAnnotatedProperty(propertyTag);
			}
			yield break;
		}

		protected bool ForUpload
		{
			get
			{
				return this.forUpload;
			}
		}

		private PropertyValue GetProperty(PropertyTag propertyTag)
		{
			StorePropTag storePropTag = LegacyHelper.ConvertFromLegacyPropTag(propertyTag, this.GetObjectTypeImp(), this.Context.Logon.MapiMailbox, true);
			Property prop;
			if (storePropTag == PropTag.Folder.PackedNamedProps)
			{
				prop = this.packedNamedProperties;
			}
			else
			{
				bool flag = false;
				int num = -1;
				if (this.loadedProperties != null)
				{
					num = this.loadedProperties.BinarySearch(new Property(storePropTag, null), PropertyComparerByTag.Comparer);
					flag = (num >= 0 && this.loadedProperties[num].Value != null);
				}
				if (flag)
				{
					prop = this.loadedProperties[num];
				}
				else
				{
					prop = this.GetPropertyImp(storePropTag);
				}
				if (prop.Tag.PropType == PropertyType.SvrEid)
				{
					prop = new Property(prop.Tag, Helper.ConvertServerEIdFromOursToExportFormat(this.mapiPropBag.Logon, (byte[])prop.Value));
				}
			}
			if (ExTraceGlobals.SourceSendTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("Send Property=[");
				prop.AppendToString(stringBuilder);
				stringBuilder.Append("]]");
				ExTraceGlobals.SourceSendTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			return RcaTypeHelpers.MassageOutgoingProperty(prop, true);
		}

		public void SetProperty(PropertyValue propertyValue)
		{
			StorePropTag tag = LegacyHelper.ConvertFromLegacyPropTag(propertyValue.PropertyTag, this.GetObjectTypeImp(), this.Context.Logon.MapiMailbox, true);
			object value = propertyValue.Value;
			RcaTypeHelpers.MassageIncomingPropertyValue(propertyValue.PropertyTag, ref value);
			this.SetPropertyImp(new Property(tag, value));
		}

		public void Delete(PropertyTag propertyTag)
		{
			StorePropTag propTag = LegacyHelper.ConvertFromLegacyPropTag(propertyTag, this.GetObjectTypeImp(), this.Context.Logon.MapiMailbox, true);
			this.DeleteImp(propTag);
		}

		public Stream GetPropertyStream(PropertyTag propertyTag)
		{
			StorePropTag propTag = LegacyHelper.ConvertFromLegacyPropTag(propertyTag, this.GetObjectTypeImp(), this.Context.Logon.MapiMailbox, true);
			return this.GetPropertyStreamImp(propTag);
		}

		public Stream SetPropertyStream(PropertyTag propertyTag, long dataSizeEstimate)
		{
			StorePropTag propTag = LegacyHelper.ConvertFromLegacyPropTag(propertyTag, this.GetObjectTypeImp(), this.Context.Logon.MapiMailbox, true);
			return this.SetPropertyStreamImp(propTag, dataSizeEstimate);
		}

		protected static void MovePropertyTagToSpecificPosition(List<StorePropTag> propertyList, StorePropTag propTag, int position)
		{
			int i = 0;
			while (i < propertyList.Count)
			{
				if (propertyList[i] == propTag)
				{
					if (i != position)
					{
						propertyList[i] = propertyList[position];
						propertyList[position] = propTag;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}

		protected static void ResetPropertyIfPresent(List<Property> propertyList, StorePropTag propTag)
		{
			Property property = new Property(propTag, null);
			int num = propertyList.BinarySearch(property, PropertyComparerByTag.Comparer);
			if (num >= 0)
			{
				propertyList[num] = property;
			}
		}

		protected static void AddNullPropertyIfNotPresent(List<Property> propertyList, StorePropTag propTag)
		{
			Property item = new Property(propTag, null);
			int num = propertyList.BinarySearch(item, PropertyComparerByTag.Comparer);
			if (num < 0)
			{
				propertyList.Insert(~num, item);
			}
		}

		protected void Reinitialize(MapiPropBagBase mapiPropBag)
		{
			this.mapiPropBag = mapiPropBag;
		}

		protected bool ForMoveUser
		{
			get
			{
				return this.mapiPropBag.Logon.IsMoveUser;
			}
		}

		protected virtual ObjectType GetObjectTypeImp()
		{
			return Helper.GetPropTagObjectType(this.mapiPropBag.MapiObjectType);
		}

		protected virtual List<Property> LoadAllPropertiesImp()
		{
			List<StorePropTag> list = null;
			List<Property> list2;
			if (!this.excludeProps && this.propList != null)
			{
				list2 = new List<Property>(this.propList.Count);
				foreach (StorePropTag propTag in this.propList)
				{
					list2.Add(this.GetPropertyImp(propTag));
				}
				ValueHelper.SortAndRemoveDuplicates<Property>(list2, PropertyComparerByTag.Comparer);
			}
			else
			{
				list2 = this.mapiPropBag.GetAllProperties(this.Context.CurrentOperationContext, GetPropListFlags.FastTransfer, true);
			}
			if (list2.Count != 0)
			{
				int num = 0;
				for (int i = 0; i < list2.Count; i++)
				{
					StorePropTag tag = list2[i].Tag;
					if (this.IncludeTag(tag))
					{
						if (num == 0 || list2[num - 1].Tag.PropId != tag.PropId)
						{
							if (i != num)
							{
								list2[num] = list2[i];
							}
							num++;
						}
						else if (tag.PropType == PropertyType.Binary)
						{
							list2[num - 1] = list2[i];
						}
						else if (tag.PropType == PropertyType.Int64)
						{
							list2[num - 1] = list2[i];
						}
					}
					else if (tag.IsNamedProperty && this.IncludeToPackedNamedProperties(tag))
					{
						if (list == null)
						{
							list = new List<StorePropTag>();
						}
						list.Add(tag);
					}
				}
				if (num != list2.Count)
				{
					list2.RemoveRange(num, list2.Count - num);
				}
				if (list != null && list.Count != 0)
				{
					this.packedNamedProperties = this.ComputePackedNamedPropertiesProperty(list);
					Property property = new Property(PropTag.Folder.PackedNamedProps, this.packedNamedProperties);
					int num2 = list2.BinarySearch(property, PropertyComparerByTag.Comparer);
					if (num2 < 0)
					{
						list2.Insert(~num2, property);
					}
					else
					{
						list2[num2] = property;
					}
				}
			}
			return list2;
		}

		protected virtual Property GetPropertyImp(StorePropTag propTag)
		{
			return this.mapiPropBag.GetOneProp(this.Context.CurrentOperationContext, propTag);
		}

		protected virtual void SetPropertyImp(Property property)
		{
			object obj = property.Value;
			if (property.Tag.PropType == PropertyType.SvrEid)
			{
				obj = Helper.ConvertServerEIdFromExportToOursFormat(this.mapiPropBag.Logon, (byte[])obj);
			}
			if (ExTraceGlobals.SourceSendTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("Receive Property=[");
				property.AppendToString(stringBuilder);
				stringBuilder.Append("]]");
				ExTraceGlobals.SourceSendTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			if (property.Tag == PropTag.Folder.PackedNamedProps)
			{
				MDBEFCollection mdbefcollection = MDBEFCollection.CreateFrom((byte[])obj, this.context.Logon.Encoding);
				using (IEnumerator<AnnotatedPropertyValue> enumerator = mdbefcollection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AnnotatedPropertyValue annotatedPropertyValue = enumerator.Current;
						PropertyTag propertyTag = annotatedPropertyValue.PropertyTag;
						if (this.Session.TryResolveFromNamedProperty(annotatedPropertyValue.NamedProperty, ref propertyTag))
						{
							StorePropTag tag = LegacyHelper.ConvertFromLegacyPropTag(propertyTag, this.GetObjectTypeImp(), this.Context.Logon.MapiMailbox, true);
							object value = annotatedPropertyValue.PropertyValue.Value;
							RcaTypeHelpers.MassageIncomingPropertyValue(propertyTag, ref value);
							this.SetPropertyImp(new Property(tag, value));
						}
					}
					return;
				}
			}
			this.mapiPropBag.SetOneProp(this.Context.CurrentOperationContext, property.Tag, obj);
		}

		protected virtual void DeleteImp(StorePropTag propTag)
		{
			this.mapiPropBag.DeleteOneProp(this.Context.CurrentOperationContext, propTag);
		}

		public virtual Stream GetPropertyStreamImp(StorePropTag propTag)
		{
			FastTransferPropertyBag.MapiStreamWrapper mapiStreamWrapper = new FastTransferPropertyBag.MapiStreamWrapper(this, true);
			mapiStreamWrapper.Configure(propTag);
			if (ExTraceGlobals.SourceSendTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("Send Property Stream=[tag=[");
				propTag.AppendToString(stringBuilder);
				stringBuilder.Append("]]");
				ExTraceGlobals.SourceSendTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			return mapiStreamWrapper;
		}

		public virtual Stream SetPropertyStreamImp(StorePropTag propTag, long dataSize)
		{
			FastTransferPropertyBag.MapiStreamWrapper mapiStreamWrapper = new FastTransferPropertyBag.MapiStreamWrapper(this, false);
			mapiStreamWrapper.Configure(propTag, dataSize);
			if (ExTraceGlobals.SourceSendTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("Receive Property Stream=[tag=[");
				propTag.AppendToString(stringBuilder);
				stringBuilder.Append("] size=[");
				stringBuilder.Append(dataSize.ToString());
				stringBuilder.Append("]]");
				ExTraceGlobals.SourceSendTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			return mapiStreamWrapper;
		}

		protected virtual bool IncludeTag(StorePropTag propTag)
		{
			if (this.excludeProps)
			{
				return this.propList == null || !this.propList.Contains(propTag);
			}
			return this.propList != null && this.propList.Contains(propTag);
		}

		protected virtual bool IncludeToPackedNamedProperties(StorePropTag propTag)
		{
			return false;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferPropertyBag>(this);
		}

		protected override void InternalDispose(bool isCalledFromDispose)
		{
		}

		private Property ComputePackedNamedPropertiesProperty(List<StorePropTag> namedPropertiesList)
		{
			MDBEFCollection mdbefcollection = new MDBEFCollection();
			foreach (StorePropTag storePropTag in namedPropertiesList)
			{
				AnnotatedPropertyValue annotatedProperty = this.GetAnnotatedProperty(new PropertyTag(storePropTag.PropTag));
				mdbefcollection.AddAnnotatedProperty(annotatedProperty);
			}
			return new Property(PropTag.Folder.PackedNamedProps, mdbefcollection.Serialize(this.context.Logon.Encoding));
		}

		private readonly bool forUpload;

		private FastTransferContext context;

		private MapiPropBagBase mapiPropBag;

		private bool excludeProps;

		private HashSet<StorePropTag> propList;

		private List<Property> loadedProperties;

		private Property packedNamedProperties = Property.NotFoundError(PropTag.Folder.PackedNamedProps);

		internal class MapiStreamWrapper : Stream
		{
			public MapiStreamWrapper(FastTransferPropertyBag fastTransferPropBag, bool readOnly)
			{
				this.fastTransferPropBag = fastTransferPropBag;
				this.readOnly = readOnly;
			}

			public void Configure(StorePropTag propTag)
			{
				this.propTag = propTag;
				this.ResetMemoryBufferState();
				this.mapiStream = this.fastTransferPropBag.MapiPropBag.OpenStream(this.fastTransferPropBag.MapiPropBag.CurrentOperationContext, StreamFlags.AllowRead, propTag, this.fastTransferPropBag.MapiPropBag.Logon.Session.CodePage);
				this.streamFlushedToProperty = false;
			}

			public void Configure(StorePropTag propTag, long dataSize)
			{
				this.propTag = propTag;
				this.ResetMemoryBufferState();
				this.EnsureUnderlyingStorageIsAllocated(dataSize);
				this.streamFlushedToProperty = false;
			}

			public override bool CanRead
			{
				get
				{
					return true;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return true;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return !this.readOnly;
				}
			}

			public override void Flush()
			{
				if (this.mapiStream != null)
				{
					this.mapiStream.Commit(this.fastTransferPropBag.MapiPropBag.CurrentOperationContext);
				}
				else if (this.buffer != null)
				{
					BufferPool pool = TempStream.Pool;
					object value;
					if (this.propTag.PropType != PropertyType.Unicode)
					{
						if (this.length == this.buffer.Length && this.buffer.Length != pool.BufferSize)
						{
							value = this.buffer;
							this.buffer = null;
						}
						else
						{
							byte[] array = new byte[this.length];
							Buffer.BlockCopy(this.buffer, 0, array, 0, this.length);
							value = array;
						}
					}
					else if (this.propTag.ExternalType == PropertyType.String8)
					{
						value = CTSGlobals.AsciiEncoding.GetString(this.buffer, 0, this.length);
					}
					else
					{
						value = Encoding.Unicode.GetString(this.buffer, 0, this.length);
					}
					this.fastTransferPropBag.SetPropertyImp(new Property(this.propTag, value));
					if (this.buffer != null && this.buffer.Length == pool.BufferSize)
					{
						pool.Release(this.buffer);
					}
					this.ResetMemoryBufferState();
				}
				this.streamFlushedToProperty = true;
			}

			public override long Length
			{
				get
				{
					if (this.mapiStream != null)
					{
						return this.mapiStream.GetSize(this.CurrentOperationContext);
					}
					return (long)this.length;
				}
			}

			public override long Position
			{
				get
				{
					if (this.mapiStream != null)
					{
						return this.mapiStream.Seek(this.CurrentOperationContext, 0L, SeekOrigin.Current);
					}
					return (long)this.position;
				}
				set
				{
					this.EnsureUnderlyingStorageIsAllocated(value);
					if (this.mapiStream != null)
					{
						this.mapiStream.Seek(this.CurrentOperationContext, value, SeekOrigin.Begin);
						return;
					}
					this.position = (int)value;
				}
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				if (this.mapiStream != null)
				{
					return this.mapiStream.Read(this.CurrentOperationContext, buffer, offset, count);
				}
				if (offset + count > buffer.Length)
				{
					throw new ExExceptionStreamInvalidParameter((LID)37944U, "Read offset out or range.");
				}
				int num = Math.Min(count, this.length - this.position);
				Buffer.BlockCopy(this.buffer, this.position, buffer, 0, num);
				return num;
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				long num = 0L;
				if (this.mapiStream == null)
				{
					switch (origin)
					{
					case SeekOrigin.Begin:
						num = offset;
						break;
					case SeekOrigin.Current:
						num = (long)this.position + offset;
						break;
					case SeekOrigin.End:
						num = (long)this.position + offset;
						break;
					}
					if (num < 0L)
					{
						throw new ExExceptionStreamSeekError((LID)33848U, "Seek offset out of range");
					}
					this.EnsureUnderlyingStorageIsAllocated(num);
				}
				if (this.mapiStream != null)
				{
					return this.mapiStream.Seek(this.CurrentOperationContext, offset, origin);
				}
				this.position = (int)num;
				return num;
			}

			public override void SetLength(long value)
			{
				this.EnsureUnderlyingStorageIsAllocated(value);
				if (this.mapiStream != null)
				{
					this.mapiStream.SetSize(this.CurrentOperationContext, value);
					return;
				}
				this.length = (int)value;
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				if (this.mapiStream == null)
				{
					this.EnsureUnderlyingStorageIsAllocated((long)(this.position + count));
				}
				if (this.mapiStream != null)
				{
					this.mapiStream.Write(this.CurrentOperationContext, buffer, offset, count);
					return;
				}
				Buffer.BlockCopy(buffer, offset, this.buffer, this.position, count);
				this.position += count;
				if (this.position > this.length)
				{
					this.length = this.position;
				}
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					if (this.mapiStream != null)
					{
						this.mapiStream.Dispose();
						this.mapiStream = null;
						return;
					}
					if (this.buffer != null)
					{
						BufferPool pool = TempStream.Pool;
						if (this.buffer.Length == pool.BufferSize)
						{
							pool.Release(this.buffer);
						}
						this.ResetMemoryBufferState();
					}
				}
			}

			private void ResetMemoryBufferState()
			{
				this.buffer = null;
				this.position = 0;
				this.length = 0;
			}

			private void EnsureUnderlyingStorageIsAllocated(long dataSize)
			{
				if (this.mapiStream != null)
				{
					return;
				}
				BufferPool pool = TempStream.Pool;
				if (dataSize > (long)pool.BufferSize && this.fastTransferPropBag.MapiPropBag.MapiObjectType != MapiObjectType.Person && this.propTag != PropTag.Folder.PackedNamedProps)
				{
					this.mapiStream = this.fastTransferPropBag.MapiPropBag.OpenStream(this.fastTransferPropBag.MapiPropBag.CurrentOperationContext, StreamFlags.AllowCreate | StreamFlags.AllowRead | StreamFlags.AllowWrite, this.propTag, this.fastTransferPropBag.MapiPropBag.Logon.Session.CodePage);
					if (this.buffer != null)
					{
						this.mapiStream.Write(this.CurrentOperationContext, this.buffer, 0, this.length);
						this.mapiStream.Seek(this.CurrentOperationContext, (long)this.position, SeekOrigin.Begin);
					}
					this.buffer = null;
					this.length = 0;
					this.position = 0;
					return;
				}
				if (this.buffer == null || dataSize > (long)this.buffer.Length)
				{
					byte[] dst;
					if (dataSize <= (long)pool.BufferSize)
					{
						dst = pool.Acquire();
					}
					else
					{
						dst = new byte[(int)dataSize];
					}
					if (this.buffer != null)
					{
						Buffer.BlockCopy(this.buffer, 0, dst, 0, this.length);
						if (this.buffer.Length == pool.BufferSize)
						{
							pool.Release(this.buffer);
						}
					}
					this.buffer = dst;
				}
			}

			private MapiContext CurrentOperationContext
			{
				get
				{
					return this.fastTransferPropBag.Context.CurrentOperationContext;
				}
			}

			private FastTransferPropertyBag fastTransferPropBag;

			private StorePropTag propTag;

			private bool streamFlushedToProperty;

			private MapiStream mapiStream;

			private byte[] buffer;

			private int position;

			private int length;

			private bool readOnly;
		}
	}
}
