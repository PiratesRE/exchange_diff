using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MemoryPropertyBag : IPropertyBag
	{
		public MemoryPropertyBag(ISession session) : this(session, 1048576)
		{
		}

		public MemoryPropertyBag(ISession session, int maxStreamLength)
		{
			Util.ThrowOnNullArgument(session, "session");
			this.session = session;
			this.maxStreamLength = maxStreamLength;
		}

		public virtual AnnotatedPropertyValue GetAnnotatedProperty(PropertyTag propertyTag)
		{
			PropertyValue property = this.GetProperty(propertyTag);
			return this.GetAnnotatedProperty(propertyTag, property);
		}

		public IEnumerable<AnnotatedPropertyValue> GetAnnotatedProperties()
		{
			return from propertyValue in this.properties.Values
			select this.GetAnnotatedProperty(propertyValue.PropertyTag, propertyValue);
		}

		public void SetProperty(PropertyValue propertyValue)
		{
			this.properties[propertyValue.PropertyTag.PropertyId] = propertyValue;
		}

		public void Delete(PropertyTag property)
		{
			this.properties.Remove(property.PropertyId);
		}

		public Stream GetPropertyStream(PropertyTag propertyTag)
		{
			PropertyValue property = this.GetProperty(propertyTag);
			return MemoryPropertyBag.WrapPropertyReadStream(property);
		}

		public Stream SetPropertyStream(PropertyTag property, long dataSizeEstimate)
		{
			return MemoryPropertyBag.WrapPropertyWriteStream(this, property, dataSizeEstimate, this.maxStreamLength);
		}

		public ISession Session
		{
			get
			{
				return this.session;
			}
		}

		internal static PropertyValue ConvertToRequestedType(PropertyValue propertyValue, PropertyType requestedPropertyType)
		{
			if (!propertyValue.IsError && propertyValue.PropertyTag.PropertyType != requestedPropertyType)
			{
				throw Feature.NotImplemented(0, string.Format("Property value conversion is not implemented yet: requested = {0}, stored = {1}", requestedPropertyType, propertyValue));
			}
			return propertyValue;
		}

		internal static Stream WrapPropertyReadStream(PropertyValue propertyValue)
		{
			FastTransferPropertyValue.CheckVariableSizePropertyType(propertyValue.PropertyTag.PropertyType);
			byte[] buffer;
			switch (propertyValue.PropertyTag.PropertyType)
			{
			case PropertyType.String8:
			case PropertyType.Unicode:
				buffer = Encoding.Unicode.GetBytes(propertyValue.GetValueAssert<string>());
				break;
			default:
				buffer = propertyValue.GetValueAssert<byte[]>();
				break;
			}
			return new MemoryStream(buffer, false);
		}

		internal static Stream WrapPropertyWriteStream(IPropertyBag propertyBag, PropertyTag propertyTag, long dataSizeEstimate)
		{
			return MemoryPropertyBag.WrapPropertyWriteStream(propertyBag, propertyTag, dataSizeEstimate, 1048576);
		}

		internal static Stream WrapPropertyWriteStream(IPropertyBag propertyBag, PropertyTag propertyTag, long dataSizeEstimate, int maxStreamLength)
		{
			return new MemoryPropertyBag.WriteMemoryStream(propertyBag, propertyTag, dataSizeEstimate, maxStreamLength);
		}

		private AnnotatedPropertyValue GetAnnotatedProperty(PropertyTag propertyTag, PropertyValue propertyValue)
		{
			NamedProperty namedProperty = null;
			if (propertyTag.IsNamedProperty)
			{
				this.Session.TryResolveToNamedProperty(propertyTag, out namedProperty);
			}
			return new AnnotatedPropertyValue(propertyTag, propertyValue, namedProperty);
		}

		private PropertyValue GetProperty(PropertyTag property)
		{
			PropertyValue propertyValue;
			if (this.properties.TryGetValue(property.PropertyId, out propertyValue))
			{
				return MemoryPropertyBag.ConvertToRequestedType(propertyValue, property.PropertyType);
			}
			return PropertyValue.Error(property.PropertyId, (ErrorCode)2147746063U);
		}

		private readonly Dictionary<PropertyId, PropertyValue> properties = new Dictionary<PropertyId, PropertyValue>(PropertyIdComparer.Instance);

		private readonly ISession session;

		private readonly int maxStreamLength;

		internal sealed class WriteMemoryStream : MemoryStream, IDisposeTrackable, IDisposable
		{
			public WriteMemoryStream(IPropertyBag propertyBag, PropertyTag propertyTag, long capacity) : this(propertyBag, propertyTag, capacity, 1048576)
			{
			}

			public WriteMemoryStream(IPropertyBag propertyBag, PropertyTag propertyTag, long capacity, int maxStreamLength) : base(MemoryPropertyBag.WriteMemoryStream.CheckLength(capacity, maxStreamLength))
			{
				FastTransferPropertyValue.CheckVariableSizePropertyType(propertyTag.PropertyType);
				this.propertyBag = propertyBag;
				this.propertyTag = propertyTag;
				this.maxStreamLength = maxStreamLength;
				this.disposeTracker = this.GetDisposeTracker();
				this.isDirty = true;
			}

			public override void Flush()
			{
				base.Flush();
				if (this.isDirty)
				{
					object value;
					if (this.propertyTag.PropertyType == PropertyType.Binary || this.propertyTag.PropertyType == PropertyType.Object || this.propertyTag.PropertyType == PropertyType.ServerId)
					{
						value = ((this.Length == (long)this.Capacity) ? this.GetBuffer() : this.ToArray());
					}
					else if (this.propertyTag.PropertyType == PropertyType.Unicode)
					{
						value = Encoding.Unicode.GetString(this.GetBuffer(), 0, (int)this.Length);
					}
					else
					{
						if (this.propertyTag.PropertyType != PropertyType.String8)
						{
							throw new InvalidOperationException(string.Format("Cannot stream properties of type {0}.", this.propertyTag.PropertyType));
						}
						Feature.Stubbed(54718, "String8 support in FastTransfer");
						value = CTSGlobals.AsciiEncoding.GetString(this.GetBuffer(), 0, (int)this.Length);
					}
					this.propertyBag.SetProperty(new PropertyValue(this.propertyTag, value));
					this.isDirty = false;
				}
			}

			public override void SetLength(long value)
			{
				MemoryPropertyBag.WriteMemoryStream.CheckLength(value, this.maxStreamLength);
				this.isDirty = true;
				base.SetLength(value);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				MemoryPropertyBag.WriteMemoryStream.CheckLength(this.Length + (long)count, this.maxStreamLength);
				this.isDirty = true;
				base.Write(buffer, offset, count);
			}

			public override void WriteByte(byte value)
			{
				MemoryPropertyBag.WriteMemoryStream.CheckLength(this.Length + 1L, this.maxStreamLength);
				this.isDirty = true;
				base.WriteByte(value);
			}

			protected sealed override void Dispose(bool disposing)
			{
				if (disposing && !this.isDisposed)
				{
					this.isDisposed = true;
					this.InternalDispose();
					GC.SuppressFinalize(this);
				}
				base.Dispose(disposing);
			}

			private static int CheckLength(long length, int maxStreamLength)
			{
				if (length > (long)maxStreamLength)
				{
					throw new RopExecutionException(string.Format("Memory property streams size limit exceeded. Size: '{0}', Limit: '{1}'", length, maxStreamLength), (ErrorCode)2147942414U);
				}
				return (int)length;
			}

			DisposeTracker IDisposeTrackable.GetDisposeTracker()
			{
				return this.GetDisposeTracker();
			}

			private DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<MemoryPropertyBag.WriteMemoryStream>(this);
			}

			void IDisposeTrackable.SuppressDisposeTracker()
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Suppress();
				}
			}

			private void InternalDispose()
			{
				Util.DisposeIfPresent(this.disposeTracker);
				this.Flush();
			}

			public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
			{
				MemoryPropertyBag.WriteMemoryStream.CheckLength(this.Length + (long)count, this.maxStreamLength);
				this.isDirty = true;
				return base.BeginWrite(buffer, offset, count, callback, state);
			}

			public override void EndWrite(IAsyncResult asyncResult)
			{
				MemoryPropertyBag.WriteMemoryStream.CheckLength(this.Length, this.maxStreamLength);
				this.isDirty = true;
				base.EndWrite(asyncResult);
			}

			private readonly IPropertyBag propertyBag;

			private readonly PropertyTag propertyTag;

			private readonly int maxStreamLength;

			private bool isDisposed;

			private DisposeTracker disposeTracker;

			private bool isDirty;
		}
	}
}
