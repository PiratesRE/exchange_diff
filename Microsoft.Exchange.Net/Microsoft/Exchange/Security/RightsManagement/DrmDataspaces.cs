using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement.StructuredStorage;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal static class DrmDataspaces
	{
		public static string Read(IStorage rootStorage)
		{
			if (rootStorage == null)
			{
				throw new ArgumentNullException("rootStorage");
			}
			string result = string.Empty;
			StreamOverIStream streamOverIStream = new StreamOverIStream(null);
			BinaryReader binaryReader = new BinaryReader(streamOverIStream);
			IStorage storage = null;
			try
			{
				storage = DrmEmailUtils.EnsureStorage(rootStorage, "\u0006DataSpaces");
				IStream stream = null;
				try
				{
					stream = DrmEmailUtils.EnsureStream(storage, "Version");
					streamOverIStream.ReplaceIStream(stream);
					DrmDataspaces.VersionInfo versionInfo = new DrmDataspaces.VersionInfo("Microsoft.Container.DataSpaces");
					versionInfo.Read(binaryReader);
				}
				finally
				{
					if (stream != null)
					{
						Marshal.ReleaseComObject(stream);
						stream = null;
					}
				}
				IStream stream2 = null;
				try
				{
					stream2 = DrmEmailUtils.EnsureStream(storage, "DataSpaceMap");
					streamOverIStream.ReplaceIStream(stream2);
					DrmDataspaces.DataSpaceMapHeader dataSpaceMapHeader = new DrmDataspaces.DataSpaceMapHeader(1);
					dataSpaceMapHeader.Read(binaryReader);
					DrmDataspaces.DataSpaceMapEntry dataSpaceMapEntry = new DrmDataspaces.DataSpaceMapEntry(new string[][]
					{
						new string[]
						{
							"\tDRMContent",
							"\tDRMDataSpace"
						}
					});
					dataSpaceMapEntry.Read(binaryReader);
				}
				finally
				{
					if (stream2 != null)
					{
						Marshal.ReleaseComObject(stream2);
						stream2 = null;
					}
				}
				IStorage storage2 = null;
				try
				{
					storage2 = DrmEmailUtils.EnsureStorage(storage, "DataSpaceInfo");
					IStream stream3 = null;
					try
					{
						stream3 = DrmEmailUtils.EnsureStream(storage2, "\tDRMDataSpace");
						streamOverIStream.ReplaceIStream(stream3);
						DrmDataspaces.DataTransformHeader dataTransformHeader = new DrmDataspaces.DataTransformHeader(new string[]
						{
							"\tDRMTransform"
						});
						dataTransformHeader.Read(binaryReader);
					}
					finally
					{
						if (stream3 != null)
						{
							Marshal.ReleaseComObject(stream3);
							stream3 = null;
						}
					}
				}
				finally
				{
					if (storage2 != null)
					{
						Marshal.ReleaseComObject(storage2);
						storage2 = null;
					}
				}
				IStorage storage3 = null;
				try
				{
					storage3 = DrmEmailUtils.EnsureStorage(storage, "TransformInfo");
					IStorage storage4 = null;
					try
					{
						storage4 = DrmEmailUtils.EnsureStorage(storage3, "\tDRMTransform");
						IStream stream4 = null;
						try
						{
							stream4 = DrmEmailUtils.EnsureStream(storage4, "\u0006Primary");
							streamOverIStream.ReplaceIStream(stream4);
							DrmDataspaces.DataTransformInfo dataTransformInfo = new DrmDataspaces.DataTransformInfo("{C73DFACD-061F-43B0-8B64-0C620D2A8B50}");
							dataTransformInfo.Read(binaryReader);
							DrmDataspaces.VersionInfo versionInfo2 = new DrmDataspaces.VersionInfo("Microsoft.Metadata.DRMTransform");
							versionInfo2.Read(binaryReader);
							try
							{
								int num = binaryReader.ReadInt32();
								if (num != 4)
								{
									throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("WrongSizeBeforePublishLicense"));
								}
								result = DrmEmailUtils.ReadUTF8String(binaryReader);
							}
							catch (EndOfStreamException innerException)
							{
								throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("NotReadPublishLicense"), innerException);
							}
						}
						finally
						{
							if (stream4 != null)
							{
								Marshal.ReleaseComObject(stream4);
								stream4 = null;
							}
						}
					}
					finally
					{
						if (storage4 != null)
						{
							Marshal.ReleaseComObject(storage4);
							storage4 = null;
						}
					}
				}
				finally
				{
					if (storage3 != null)
					{
						Marshal.ReleaseComObject(storage3);
						storage3 = null;
					}
				}
			}
			finally
			{
				if (storage != null)
				{
					Marshal.ReleaseComObject(storage);
					storage = null;
				}
			}
			return result;
		}

		public static void Write(IStorage rootStorage, string publishLicense)
		{
			if (rootStorage == null)
			{
				throw new ArgumentNullException("rootStorage");
			}
			StreamOverIStream streamOverIStream = new StreamOverIStream(null);
			BufferedStream output = new BufferedStream(streamOverIStream);
			BinaryWriter binaryWriter = new BinaryWriter(output);
			IStorage storage = null;
			try
			{
				storage = rootStorage.CreateStorage("\u0006DataSpaces", 4114, 0, 0);
				IStream stream = null;
				try
				{
					stream = storage.CreateStream("Version", 4114, 0, 0);
					streamOverIStream.ReplaceIStream(stream);
					DrmDataspaces.VersionInfo versionInfo = new DrmDataspaces.VersionInfo("Microsoft.Container.DataSpaces");
					versionInfo.Write(binaryWriter);
					binaryWriter.Flush();
					stream.Commit(STGC.STGC_DEFAULT);
				}
				finally
				{
					if (stream != null)
					{
						Marshal.ReleaseComObject(stream);
						stream = null;
					}
				}
				IStream stream2 = null;
				try
				{
					stream2 = storage.CreateStream("DataSpaceMap", 4114, 0, 0);
					streamOverIStream.ReplaceIStream(stream2);
					DrmDataspaces.DataSpaceMapHeader dataSpaceMapHeader = new DrmDataspaces.DataSpaceMapHeader(1);
					dataSpaceMapHeader.Write(binaryWriter);
					DrmDataspaces.DataSpaceMapEntry dataSpaceMapEntry = new DrmDataspaces.DataSpaceMapEntry(new string[][]
					{
						new string[]
						{
							"\tDRMContent",
							"\tDRMDataSpace"
						}
					});
					dataSpaceMapEntry.Write(binaryWriter);
					binaryWriter.Flush();
					stream2.Commit(STGC.STGC_DEFAULT);
				}
				finally
				{
					if (stream2 != null)
					{
						Marshal.ReleaseComObject(stream2);
						stream2 = null;
					}
				}
				IStorage storage2 = null;
				try
				{
					storage2 = storage.CreateStorage("DataSpaceInfo", 4114, 0, 0);
					IStream stream3 = null;
					try
					{
						stream3 = storage2.CreateStream("\tDRMDataSpace", 4114, 0, 0);
						streamOverIStream.ReplaceIStream(stream3);
						DrmDataspaces.DataTransformHeader dataTransformHeader = new DrmDataspaces.DataTransformHeader(new string[]
						{
							"\tDRMTransform"
						});
						dataTransformHeader.Write(binaryWriter);
						binaryWriter.Flush();
						stream3.Commit(STGC.STGC_DEFAULT);
					}
					finally
					{
						if (stream3 != null)
						{
							Marshal.ReleaseComObject(stream3);
							stream3 = null;
						}
					}
				}
				finally
				{
					if (storage2 != null)
					{
						Marshal.ReleaseComObject(storage2);
						storage2 = null;
					}
				}
				IStorage storage3 = null;
				try
				{
					storage3 = storage.CreateStorage("TransformInfo", 4114, 0, 0);
					IStorage storage4 = null;
					try
					{
						storage4 = storage3.CreateStorage("\tDRMTransform", 4114, 0, 0);
						IStream stream4 = null;
						try
						{
							stream4 = storage4.CreateStream("\u0006Primary", 4114, 0, 0);
							streamOverIStream.ReplaceIStream(stream4);
							DrmDataspaces.DataTransformInfo dataTransformInfo = new DrmDataspaces.DataTransformInfo("{C73DFACD-061F-43B0-8B64-0C620D2A8B50}");
							dataTransformInfo.Write(binaryWriter);
							DrmDataspaces.VersionInfo versionInfo2 = new DrmDataspaces.VersionInfo("Microsoft.Metadata.DRMTransform");
							versionInfo2.Write(binaryWriter);
							binaryWriter.Write(4);
							DrmEmailUtils.WriteUTF8String(binaryWriter, publishLicense);
							binaryWriter.Flush();
							stream4.Commit(STGC.STGC_DEFAULT);
						}
						finally
						{
							if (stream4 != null)
							{
								Marshal.ReleaseComObject(stream4);
								stream4 = null;
							}
						}
					}
					finally
					{
						if (storage4 != null)
						{
							Marshal.ReleaseComObject(storage4);
							storage4 = null;
						}
					}
				}
				finally
				{
					if (storage3 != null)
					{
						Marshal.ReleaseComObject(storage3);
						storage3 = null;
					}
				}
			}
			finally
			{
				if (storage != null)
				{
					Marshal.ReleaseComObject(storage);
					storage = null;
				}
			}
		}

		private class VersionInfo
		{
			public VersionInfo(string featureName)
			{
				this.featureName = featureName;
			}

			public void Write(BinaryWriter writer)
			{
				DrmEmailUtils.WriteUnicodeString(writer, this.featureName);
				ushort value = 1;
				ushort value2 = 0;
				writer.Write(value);
				writer.Write(value2);
				writer.Write(value);
				writer.Write(value2);
				writer.Write(value);
				writer.Write(value2);
			}

			public void Read(BinaryReader reader)
			{
				try
				{
					string a = DrmEmailUtils.ReadUnicodeString(reader);
					if (a != this.featureName)
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("IncorrectFeatureName"));
					}
					ushort num = 1;
					ushort num2 = 0;
					ushort num3 = reader.ReadUInt16();
					ushort num4 = reader.ReadUInt16();
					if (num3 != num || num4 != num2)
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("IncorrectReaderVersion"));
					}
					num3 = reader.ReadUInt16();
					num4 = reader.ReadUInt16();
					num3 = reader.ReadUInt16();
					num4 = reader.ReadUInt16();
				}
				catch (EndOfStreamException innerException)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("NotReadVersionInfo"), innerException);
				}
			}

			private readonly string featureName;
		}

		private class DataSpaceMapHeader
		{
			public DataSpaceMapHeader(int entryCount)
			{
				this.entryCount = entryCount;
			}

			public void Write(BinaryWriter writer)
			{
				int value = 8;
				writer.Write(value);
				writer.Write(this.entryCount);
			}

			public void Read(BinaryReader reader)
			{
				int num = 8;
				try
				{
					int num2 = reader.ReadInt32();
					if (num2 != num)
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DataspaceMapHeaderWrongLength"));
					}
					reader.ReadInt32();
				}
				catch (EndOfStreamException innerException)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("NotReadDataspaceMapHeader"), innerException);
				}
			}

			private int entryCount;
		}

		private class DataSpaceMapEntry
		{
			public DataSpaceMapEntry(string[][] refComponents)
			{
				this.refComponents = refComponents;
				this.entrylength = 8;
				foreach (string[] array2 in this.refComponents)
				{
					this.entrylength += 4;
					this.entrylength += DrmEmailUtils.GetUnicodeStringLength(array2[0]);
					this.entrylength += DrmEmailUtils.GetUnicodeStringLength(array2[1]);
				}
			}

			public void Write(BinaryWriter writer)
			{
				writer.Write(this.entrylength);
				writer.Write(this.refComponents.Length);
				foreach (string[] array2 in this.refComponents)
				{
					writer.Write(0);
					DrmEmailUtils.WriteUnicodeString(writer, array2[0]);
					DrmEmailUtils.WriteUnicodeString(writer, array2[1]);
				}
			}

			public void Read(BinaryReader reader)
			{
				long position = reader.BaseStream.Position;
				try
				{
					int num = reader.ReadInt32();
					if (num != this.entrylength)
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DataspaceMapEntryWrongLength"));
					}
					int num2 = reader.ReadInt32();
					if (num2 != this.refComponents.Length)
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DataspaceMapEntryWrongRefComponentsCount"));
					}
					foreach (string[] array2 in this.refComponents)
					{
						int num3 = reader.ReadInt32();
						if (num3 != 0)
						{
							throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DataspaceMapEntryWrongRefComponentType"));
						}
						string a = DrmEmailUtils.ReadUnicodeString(reader);
						if (a != array2[0])
						{
							throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DataspaceMapEntryWrongRefComponentName1"));
						}
						string a2 = DrmEmailUtils.ReadUnicodeString(reader);
						if (a2 != array2[1])
						{
							throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DataspaceMapEntryWrongRefComponentName2"));
						}
					}
					if (reader.BaseStream.Position - position != (long)this.entrylength)
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DataspaceMapEntryLengthMismatch"));
					}
				}
				catch (EndOfStreamException innerException)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("NotReadDataspaceMapEntry"), innerException);
				}
			}

			private string[][] refComponents;

			private int entrylength;
		}

		private class DataTransformHeader
		{
			public DataTransformHeader(string[] transformNames)
			{
				this.transformNames = transformNames;
			}

			public void Write(BinaryWriter writer)
			{
				int value = 8;
				writer.Write(value);
				writer.Write(this.transformNames.Length);
				foreach (string value2 in this.transformNames)
				{
					DrmEmailUtils.WriteUnicodeString(writer, value2);
				}
			}

			public void Read(BinaryReader reader)
			{
				int num = 8;
				try
				{
					int num2 = reader.ReadInt32();
					if (num2 != num)
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DataTransformHeaderWrongLength"));
					}
					int num3 = reader.ReadInt32();
					if (num3 != this.transformNames.Length)
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DataTransformHeaderWrongNameCount"));
					}
					foreach (string b in this.transformNames)
					{
						string a = DrmEmailUtils.ReadUnicodeString(reader);
						if (a != b)
						{
							throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DataTransformHeaderWrongTransformName"));
						}
					}
				}
				catch (EndOfStreamException innerException)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("NotReadDataTransformHeader"), innerException);
				}
			}

			private string[] transformNames;
		}

		private class DataTransformInfo
		{
			public DataTransformInfo(string transformClassName)
			{
				this.transformClassName = transformClassName;
				this.entryLength = 8 + DrmEmailUtils.GetUnicodeStringLength(this.transformClassName);
			}

			public void Write(BinaryWriter writer)
			{
				writer.Write(this.entryLength);
				writer.Write(1);
				DrmEmailUtils.WriteUnicodeString(writer, this.transformClassName);
			}

			public void Read(BinaryReader reader)
			{
				try
				{
					int num = reader.ReadInt32();
					if (num != this.entryLength)
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DataTransformInfoWrongLength"));
					}
					int num2 = reader.ReadInt32();
					if (num2 != 1)
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DataTransformInfoWrongEntryType"));
					}
					string a = DrmEmailUtils.ReadUnicodeString(reader);
					if (a != this.transformClassName)
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("DataTransformInfoWrongName"));
					}
				}
				catch (EndOfStreamException innerException)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("NotReadDataTransformInfo"), innerException);
				}
			}

			private string transformClassName;

			private int entryLength;
		}
	}
}
