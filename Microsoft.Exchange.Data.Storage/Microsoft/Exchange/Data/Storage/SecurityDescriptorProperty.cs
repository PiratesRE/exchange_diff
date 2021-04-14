using System;
using System.Security.AccessControl;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SecurityDescriptorProperty : ReadonlySmartProperty
	{
		internal SecurityDescriptorProperty(NativeStorePropertyDefinition propertyDefinition) : base(propertyDefinition)
		{
			this.propertyDefinition = propertyDefinition;
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			byte[] array = propertyBag.GetValue(this.propertyDefinition) as byte[];
			if (array == null)
			{
				SecurityDescriptorProperty.Tracer.TraceError((long)this.GetHashCode(), "SecurityDescriptorProperty::InternalTryGetValue. Security descriptor is missing.");
				return null;
			}
			if (array.Length <= 8)
			{
				SecurityDescriptorProperty.Tracer.TraceError<int>((long)this.GetHashCode(), "SecurityDescriptorProperty::InternalTryGetValue. Security descriptor has invalid length: {0}.", array.Length);
				return null;
			}
			int num = (int)BitConverter.ToUInt16(array, 2);
			if (num != 3)
			{
				SecurityDescriptorProperty.Tracer.TraceError<int, ArrayTracer<byte>>((long)this.GetHashCode(), "SecurityDescriptorProperty::InternalTryGetValue. Security descriptor has unknown version: {0}. Bytes={1}.", num, new ArrayTracer<byte>(array));
				return null;
			}
			int num2 = (int)BitConverter.ToUInt16(array, 0);
			if (num2 < 8)
			{
				SecurityDescriptorProperty.Tracer.TraceError<int, ArrayTracer<byte>>((long)this.GetHashCode(), "SecurityDescriptorProperty::InternalTryGetValue. Security descriptor offset is less than 8 bytes: {0}. Bytes={1}.", num2, new ArrayTracer<byte>(array));
				return null;
			}
			object result;
			try
			{
				result = new RawSecurityDescriptor(array, num2);
			}
			catch (ArgumentException arg)
			{
				SecurityDescriptorProperty.Tracer.TraceError<ArrayTracer<byte>, ArgumentException>((long)this.GetHashCode(), "SecurityDescriptorProperty::InternalTryGetValue. Byte array for the RawSecurityDescritor is not valid. Bytes={0}. Exception = {1}.", new ArrayTracer<byte>(array), arg);
				result = null;
			}
			return result;
		}

		private const int MinimumStoreSecurityDescriptorSize = 8;

		private NativeStorePropertyDefinition propertyDefinition;

		private static readonly Trace Tracer = ExTraceGlobals.StorageTracer;
	}
}
