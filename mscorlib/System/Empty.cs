using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	internal sealed class Empty : ISerializable
	{
		private Empty()
		{
		}

		public override string ToString()
		{
			return string.Empty;
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			UnitySerializationHolder.GetUnitySerializationInfo(info, 1, null, null);
		}

		public static readonly Empty Value = new Empty();
	}
}
