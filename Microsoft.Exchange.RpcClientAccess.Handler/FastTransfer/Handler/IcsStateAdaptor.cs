using System;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal sealed class IcsStateAdaptor : BaseObject, IIcsState, IDisposable
	{
		public IcsStateAdaptor(IcsState icsState, ReferenceCount<CoreFolder> propertyMappingReference)
		{
			this.propertyMappingReference = propertyMappingReference;
			this.propertyMappingReference.AddRef();
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.propertyBag = IcsStateHelper.CreateInMemoryPropertyBag(propertyMappingReference.ReferencedObject);
				icsState.Checkpoint(this.propertyBag);
				foreach (PropertyTag propertyTag in FastTransferIcsState.AllIcsStateProperties)
				{
					AnnotatedPropertyValue annotatedProperty = this.propertyBag.GetAnnotatedProperty(propertyTag);
					if (!annotatedProperty.PropertyValue.IsError && annotatedProperty.PropertyValue.PropertyTag.PropertyType == PropertyType.Binary)
					{
						byte[] valueAssert = annotatedProperty.PropertyValue.GetValueAssert<byte[]>();
						if (valueAssert != null && valueAssert.Length > 0)
						{
							using (BufferReader bufferReader = Reader.CreateBufferReader(valueAssert))
							{
								try
								{
									IdSet.ParseWithReplGuids(bufferReader);
								}
								catch (BufferParseException)
								{
									int num = Math.Min(valueAssert.Length, 512);
									StringBuilder stringBuilder = new StringBuilder(num * 2);
									for (int j = valueAssert.Length - num; j < valueAssert.Length; j++)
									{
										stringBuilder.Append(valueAssert[j].ToString("X2"));
									}
									throw new RopExecutionException(string.Format("ICS state property {0} appears to be corrupt [{1}][{2}]", annotatedProperty.PropertyTag, valueAssert.Length, stringBuilder.ToString()), (ErrorCode)2147500037U);
								}
							}
						}
					}
				}
				disposeGuard.Success();
			}
		}

		IPropertyBag IIcsState.PropertyBag
		{
			get
			{
				base.CheckDisposed();
				return this.propertyBag;
			}
		}

		void IIcsState.Flush()
		{
			throw new NotSupportedException("We have no known scenarios for IcsState upload");
		}

		protected override void InternalDispose()
		{
			this.propertyMappingReference.Release();
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<IcsStateAdaptor>(this);
		}

		private readonly IPropertyBag propertyBag;

		private readonly ReferenceCount<CoreFolder> propertyMappingReference;
	}
}
