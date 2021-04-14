using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OscContactSourcesForContactParser : IOscContactSourcesForContactParser
	{
		private OscContactSourcesForContactParser()
		{
		}

		public OscNetworkProperties ReadOscContactSource(byte[] property)
		{
			if (property == null || property.Length == 0)
			{
				return null;
			}
			OscNetworkProperties result;
			using (MemoryStream memoryStream = new MemoryStream(property))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					try
					{
						short num = binaryReader.ReadInt16();
						if (num < 2)
						{
							throw new OscContactSourcesForContactParseException(string.Format("Header version not supported: {0}", num));
						}
						binaryReader.ReadInt16();
						short num2 = binaryReader.ReadInt16();
						if (num2 < 1)
						{
							throw new OscContactSourcesForContactParseException("No entries found in the property.");
						}
						short num3 = binaryReader.ReadInt16();
						if (num3 < 2)
						{
							throw new OscContactSourcesForContactParseException(string.Format("Entry version not supported: {0}", num3));
						}
						binaryReader.ReadInt16();
						byte[] array = binaryReader.ReadBytes(16);
						if (array == null || array.Length != 16)
						{
							throw new OscContactSourcesForContactParseException("Corrupted provider GUID.");
						}
						Guid providerGuid = new Guid(array);
						string networkId;
						if (!OscProviderRegistry.TryGetNameFromGuid(providerGuid, out networkId))
						{
							networkId = providerGuid.ToString("N");
						}
						short count = binaryReader.ReadInt16();
						binaryReader.ReadBytes((int)count);
						short count2 = binaryReader.ReadInt16();
						string @string = Encoding.Unicode.GetString(binaryReader.ReadBytes((int)count2));
						result = new OscNetworkProperties
						{
							NetworkId = networkId,
							NetworkUserId = @string
						};
					}
					catch (ArgumentOutOfRangeException ex)
					{
						throw new OscContactSourcesForContactParseException(ex.Message);
					}
					catch (EndOfStreamException ex2)
					{
						throw new OscContactSourcesForContactParseException(ex2.Message);
					}
				}
			}
			return result;
		}

		private const int GuidLength = 16;

		private const int HeaderVersion = 2;

		private const int EntryVersion = 2;

		public static readonly OscContactSourcesForContactParser Instance = new OscContactSourcesForContactParser();
	}
}
