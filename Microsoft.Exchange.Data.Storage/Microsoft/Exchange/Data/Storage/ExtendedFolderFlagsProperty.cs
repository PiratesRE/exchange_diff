using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class ExtendedFolderFlagsProperty : SmartPropertyDefinition
	{
		internal ExtendedFolderFlagsProperty(ExtendedFolderFlagsProperty.FlagTag flag) : base("ExtendedFolderFlags", typeof(ExtendedFolderFlags), PropertyFlags.None, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.ExtendedFolderFlagsInternal, PropertyDependencyType.AllRead)
		})
		{
			this.flag = flag;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			ExtendedFolderFlagsProperty.ParsedFlags parsedFlags = ExtendedFolderFlagsProperty.DecodeFolderFlags(propertyBag.GetValue(InternalSchema.ExtendedFolderFlagsInternal)) as ExtendedFolderFlagsProperty.ParsedFlags;
			if (parsedFlags == null)
			{
				parsedFlags = new ExtendedFolderFlagsProperty.ParsedFlags();
			}
			parsedFlags[this.flag] = BitConverter.GetBytes((int)value);
			propertyBag.SetValueWithFixup(InternalSchema.ExtendedFolderFlagsInternal, ExtendedFolderFlagsProperty.EncodeFolderFlags(parsedFlags));
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object obj = ExtendedFolderFlagsProperty.DecodeFolderFlags(propertyBag.GetValue(InternalSchema.ExtendedFolderFlagsInternal));
			if (!(obj is ExtendedFolderFlagsProperty.ParsedFlags))
			{
				return obj;
			}
			ExtendedFolderFlagsProperty.ParsedFlags parsedFlags = (ExtendedFolderFlagsProperty.ParsedFlags)obj;
			if (parsedFlags.ContainsKey(this.flag))
			{
				return (ExtendedFolderFlags)BitConverter.ToInt32(parsedFlags[this.flag], 0);
			}
			return new PropertyError(this, PropertyErrorCode.NotFound);
		}

		internal static object DecodeFolderFlags(object extendedFolderFlagsInternalPropertyValue)
		{
			if (extendedFolderFlagsInternalPropertyValue is byte[])
			{
				ExtendedFolderFlagsProperty.ParsedFlags parsedFlags = new ExtendedFolderFlagsProperty.ParsedFlags();
				using (ParticipantEntryId.Reader reader = new ParticipantEntryId.Reader((byte[])extendedFolderFlagsInternalPropertyValue))
				{
					while (!reader.IsEnd)
					{
						byte key = reader.ReadByte();
						if (!reader.IsEnd)
						{
							byte b = reader.ReadByte();
							if (reader.BytesRemaining >= (int)b)
							{
								parsedFlags[(ExtendedFolderFlagsProperty.FlagTag)key] = reader.ReadExactBytes((int)b);
								continue;
							}
						}
						return new PropertyError(InternalSchema.ExtendedFolderFlags, PropertyErrorCode.CorruptedData);
					}
				}
				return parsedFlags;
			}
			return extendedFolderFlagsInternalPropertyValue;
		}

		internal static byte[] EncodeFolderFlags(ExtendedFolderFlagsProperty.ParsedFlags flags)
		{
			MemoryStream memoryStream = new MemoryStream();
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				foreach (KeyValuePair<ExtendedFolderFlagsProperty.FlagTag, byte[]> keyValuePair in flags)
				{
					binaryWriter.Write((byte)keyValuePair.Key);
					binaryWriter.Write((byte)keyValuePair.Value.Length);
					binaryWriter.Write(keyValuePair.Value);
				}
			}
			return memoryStream.ToArray();
		}

		private ExtendedFolderFlagsProperty.FlagTag flag;

		internal class ParsedFlags : SortedDictionary<ExtendedFolderFlagsProperty.FlagTag, byte[]>
		{
			internal ParsedFlags() : base(ExtendedFolderFlagsProperty.FlagTagComparer.Instance)
			{
			}
		}

		internal enum FlagTag : byte
		{
			Flags = 1,
			Clsid,
			ToDoVersion = 5
		}

		internal class FlagTagComparer : IComparer<ExtendedFolderFlagsProperty.FlagTag>
		{
			internal static ExtendedFolderFlagsProperty.FlagTagComparer Instance
			{
				get
				{
					return ExtendedFolderFlagsProperty.FlagTagComparer.instance;
				}
			}

			public int Compare(ExtendedFolderFlagsProperty.FlagTag x, ExtendedFolderFlagsProperty.FlagTag y)
			{
				return (int)(x - y);
			}

			private static readonly ExtendedFolderFlagsProperty.FlagTagComparer instance = new ExtendedFolderFlagsProperty.FlagTagComparer();
		}
	}
}
