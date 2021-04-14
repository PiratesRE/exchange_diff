using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class PropTagHelper
	{
		public static PropTag PropTagFromIdAndType(int propId, PropType propType)
		{
			return (PropTag)(propId << 16 | (int)propType);
		}

		[Obsolete]
		public static int PropTagToId(PropTag propTag)
		{
			return propTag.Id();
		}

		[Obsolete]
		public static PropType PropTagToType(PropTag propTag)
		{
			return propTag.ValueType();
		}

		public static int Id(this PropTag propTag)
		{
			return (int)((propTag & (PropTag)4294901760U) >> 16);
		}

		public static PropType ValueType(this PropTag propTag)
		{
			return (PropType)(propTag & (PropTag)65535U);
		}

		public static PropTag ChangePropType(this PropTag propTag, PropType propType)
		{
			return PropTagHelper.PropTagFromIdAndType(propTag.Id(), propType);
		}

		public static bool IsMultiValued(this PropTag propTag)
		{
			return (propTag.ValueType() & PropType.MultiValueFlag) != PropType.Unspecified;
		}

		public static bool IsMultiInstance(this PropTag propTag)
		{
			return (propTag.ValueType() & PropType.MultiInstanceFlag) != PropType.Unspecified;
		}

		public static bool IsNamedProperty(this PropTag propTag)
		{
			return propTag.Id() >= 32768 && propTag.Id() <= 65534;
		}

		public static bool IsApplicationSpecific(this PropTag propTag)
		{
			return propTag.Id() >= 24576 && propTag.Id() <= 32767;
		}

		public static bool IsValid(this PropTag propTag)
		{
			return propTag.Id() <= 32767;
		}

		private static bool IsNotTransmittable(this PropTag propTag)
		{
			int num = propTag.Id();
			return (num >= 3584 && num <= 4095) || (num >= 24576 && num <= 26111) || (num >= 26112 && num <= 26623) || (num >= 31744 && num <= 32761) || num == PropTag.NativeBodyInfo.Id();
		}

		public static bool IsTransmittable(this PropTag propTag)
		{
			return !propTag.IsNotTransmittable();
		}

		internal static PropTag[] SPropTagArray(ICollection<PropTag> tags)
		{
			if (tags == null)
			{
				return null;
			}
			PropTag[] array = new PropTag[tags.Count + 1];
			tags.CopyTo(array, 1);
			array[0] = (PropTag)tags.Count;
			return array;
		}

		public static PropTag[] PropTagArray(byte[] blob)
		{
			if (blob == null)
			{
				return null;
			}
			if (blob.Length % 4 != 0)
			{
				throw new ArgumentException("blob", "Invalid blob size.");
			}
			int num = blob.Length / 4;
			PropTag[] array = new PropTag[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (PropTag)BitConverter.ToUInt32(blob, i * 4);
			}
			return array;
		}

		public static PropTag[] PropTagArray(IntPtr blob)
		{
			if (blob == IntPtr.Zero)
			{
				return null;
			}
			int num = 0;
			int num2 = Marshal.ReadInt32(blob, num);
			PropTag[] array = new PropTag[num2];
			for (int i = 0; i < num2; i++)
			{
				num += 4;
				array[i] = (PropTag)Marshal.ReadInt32(blob, num);
			}
			return array;
		}

		public static PropTag ConvertToError(PropTag prop)
		{
			return PropTagHelper.PropTagFromIdAndType(prop.Id(), PropType.Error);
		}

		public static int GetBytesToMarshal(ICollection<PropTag> props)
		{
			return (props.Count + 1) * 4;
		}

		public static void MarshalToNative(ICollection<PropTag> props, IntPtr blob)
		{
			int num = 0;
			Marshal.WriteInt32(blob, num, props.Count);
			foreach (PropTag val in props)
			{
				num += 4;
				Marshal.WriteInt32(blob, num, (int)val);
			}
		}

		public const int MultiValuedFlag = 4096;

		public const int MultiValuedInstanceFlag = 12288;

		private const int MinUserDefinedNamed = 32768;

		private const int MaxUserDefinedNamed = 65534;

		private const int MinApplicationSpecificPropertyTag = 26624;

		private const int MaxApplicationSpecificPropertyTag = 32767;

		private const int MinMapiNonTransmittable = 3584;

		private const int MaxMapiNonTransmittable = 4095;

		private const int MinUserDefinedNonTransmittable = 24576;

		private const int MaxUserDefinedNonTransmittable = 26111;

		private const int MinProviderDefinedNonTransmittable = 26112;

		private const int MaxProviderDefinedNonTransmittable = 26623;

		private const int MinMessageClassDefinedNonTransmittable = 31744;

		private const int MaxMessageClassDefinedNonTransmittable = 32761;
	}
}
