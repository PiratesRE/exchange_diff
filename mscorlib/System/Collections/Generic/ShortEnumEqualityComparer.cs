﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
	[Serializable]
	internal sealed class ShortEnumEqualityComparer<T> : EnumEqualityComparer<T>, ISerializable where T : struct
	{
		public ShortEnumEqualityComparer()
		{
		}

		public ShortEnumEqualityComparer(SerializationInfo information, StreamingContext context)
		{
		}

		public override int GetHashCode(T obj)
		{
			int num = JitHelpers.UnsafeEnumCast<T>(obj);
			return ((short)num).GetHashCode();
		}
	}
}
