using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class ReplicaListProperty : SmartPropertyDefinition
	{
		internal ReplicaListProperty() : base("ReplicaListProperty", typeof(string[]), PropertyFlags.None, PropertyDefinitionConstraint.None, ReplicaListProperty.dependentProps)
		{
		}

		public static string[] GetStringArrayFromBytes(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			List<string> list = new List<string>();
			int num = 0;
			int num2 = 0;
			while (bytes.Length > num2)
			{
				if (bytes[num2] == 0)
				{
					list.Add(CTSGlobals.AsciiEncoding.GetString(bytes, num, num2 - num));
					num = 1 + num2;
				}
				num2++;
			}
			return list.ToArray();
		}

		public static byte[] GetBytesFromStringArray(string[] strings)
		{
			if (strings == null)
			{
				throw new ArgumentNullException("strings");
			}
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(1000))
			{
				foreach (string s in strings)
				{
					byte[] bytes = CTSGlobals.AsciiEncoding.GetBytes(s);
					memoryStream.Write(bytes, 0, bytes.Length);
					memoryStream.WriteByte(0);
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			byte[] array = propertyBag.GetValue(InternalSchema.ReplicaListBinary) as byte[];
			if (array == null)
			{
				return new PropertyError(this, PropertyErrorCode.NotFound);
			}
			return ReplicaListProperty.GetStringArrayFromBytes(array);
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			string[] array = value as string[];
			if (array != null)
			{
				propertyBag.SetValue(InternalSchema.ReplicaListBinary, ReplicaListProperty.GetBytesFromStringArray(array));
			}
		}

		private static PropertyDependency[] dependentProps = new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.ReplicaListBinary, PropertyDependencyType.NeedToReadForWrite)
		};
	}
}
