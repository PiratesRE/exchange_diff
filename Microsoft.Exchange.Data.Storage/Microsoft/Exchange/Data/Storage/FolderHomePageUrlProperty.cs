using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class FolderHomePageUrlProperty : SmartPropertyDefinition
	{
		internal FolderHomePageUrlProperty() : base("FolderHomePageUrlProperty", typeof(string), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.FolderWebViewInfo, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			object value = propertyBag.GetValue(InternalSchema.FolderWebViewInfo);
			if (PropertyError.IsPropertyNotFound(value))
			{
				return value;
			}
			byte[] webViewInfo = PropertyBag.CheckPropertyValue<byte[]>(InternalSchema.FolderWebViewInfo, value);
			object result;
			try
			{
				result = FolderHomePageUrlProperty.GetUrlFromWebViewInfo(webViewInfo);
			}
			catch (CorruptDataException ex)
			{
				result = new PropertyError(this, PropertyErrorCode.CorruptedData, ex.Message);
			}
			return result;
		}

		protected override void InternalSetValue(PropertyBag.BasicPropertyStore propertyBag, object value)
		{
			string folderHomePageUrl = (string)value;
			byte[] propertyValue = FolderHomePageUrlProperty.CreateWebViewInformation(folderHomePageUrl);
			propertyBag.SetValueWithFixup(InternalSchema.FolderWebViewInfo, propertyValue);
		}

		protected override void InternalDeleteValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			propertyBag.Delete(InternalSchema.FolderWebViewInfo);
		}

		private static byte[] CreateWebViewInformation(string folderHomePageUrl)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.Unicode))
				{
					binaryWriter.Write(2U);
					binaryWriter.Write(1U);
					binaryWriter.Write(1U);
					for (int i = 0; i < 7; i++)
					{
						binaryWriter.Write(0U);
					}
					char[] array = folderHomePageUrl.ToCharArray();
					binaryWriter.Write((uint)(array.Length * 2 + 2));
					binaryWriter.Write(array);
					binaryWriter.Write('\0');
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		private static string GetUrlFromWebViewInfo(byte[] webViewInfo)
		{
			if (webViewInfo.Length == 0)
			{
				return string.Empty;
			}
			string result;
			try
			{
				string @string;
				using (MemoryStream memoryStream = new MemoryStream(webViewInfo))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						binaryReader.BaseStream.Seek(40L, SeekOrigin.Begin);
						int num = (int)(binaryReader.ReadUInt32() - 2U);
						if (num == 0)
						{
							return string.Empty;
						}
						if (num < 14)
						{
							throw new CorruptDataException(ServerStrings.ExCorruptFolderWebViewInfo);
						}
						byte[] array = binaryReader.ReadBytes(num);
						if (array.Length != num)
						{
							throw new CorruptDataException(ServerStrings.ExCorruptFolderWebViewInfo);
						}
						@string = Encoding.Unicode.GetString(array, 0, array.Length);
					}
				}
				result = @string;
			}
			catch (EndOfStreamException innerException)
			{
				throw new CorruptDataException(ServerStrings.ExCorruptFolderWebViewInfo, innerException);
			}
			return result;
		}

		private const uint WebViewVersion = 2U;

		private const uint WebViewType = 1U;

		private const uint WebViewFlagsShowByDefault = 1U;

		private const uint WebViewUnused = 0U;

		private const long WebViewUninterestingPortion = 40L;

		private const int MinURLLength = 7;
	}
}
