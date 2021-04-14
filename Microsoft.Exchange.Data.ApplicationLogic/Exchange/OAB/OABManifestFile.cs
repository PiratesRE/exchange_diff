using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OABManifestFile
	{
		public OABDataFileType Type { get; set; }

		public uint Sequence { get; set; }

		public string Version { get; set; }

		public uint CompressedSize { get; set; }

		public uint UncompressedSize { get; set; }

		public string Hash { get; set; }

		public int? Langid { get; set; }

		public string FileName { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.Append("Type=");
			stringBuilder.Append(this.Type.ToString());
			stringBuilder.Append(", Sequence:");
			stringBuilder.Append(this.Sequence.ToString());
			stringBuilder.Append(", Version:");
			stringBuilder.Append(this.Version);
			stringBuilder.Append(", CompressedSize:");
			stringBuilder.Append(this.CompressedSize.ToString());
			stringBuilder.Append(", UncompressedSize:");
			stringBuilder.Append(this.UncompressedSize.ToString());
			stringBuilder.Append(", Hash:");
			stringBuilder.Append(this.Hash);
			if (this.Langid != null)
			{
				stringBuilder.Append(", Langid:");
				stringBuilder.Append(this.Langid.Value.ToString());
			}
			stringBuilder.Append(", FileName:");
			stringBuilder.Append(this.FileName);
			return stringBuilder.ToString();
		}

		internal static OABManifestFile Deserialize(Stream stream, XmlReader reader)
		{
			long position = stream.Position;
			string name;
			string attribute;
			string attribute2;
			string attribute3;
			string attribute4;
			string attribute5;
			string attribute6;
			string attribute7;
			string text;
			try
			{
				name = reader.Name;
				attribute = reader.GetAttribute("seq");
				attribute2 = reader.GetAttribute("ver");
				attribute3 = reader.GetAttribute("size");
				attribute4 = reader.GetAttribute("uncompressedsize");
				attribute5 = reader.GetAttribute("SHA");
				attribute6 = reader.GetAttribute("langid");
				attribute7 = reader.GetAttribute("type");
				reader.ReadStartElement();
				text = reader.ReadString().Trim();
				reader.ReadEndElement();
			}
			catch (XmlException arg)
			{
				throw new InvalidDataException(string.Format("Invalid element at stream position {0} due exception: {1}", position, arg));
			}
			string a;
			if ((a = name) != null)
			{
				OABDataFileType type;
				if (!(a == "Full"))
				{
					if (!(a == "Diff"))
					{
						if (!(a == "Template"))
						{
							goto IL_11F;
						}
						string a2;
						if ((a2 = attribute7) != null)
						{
							if (a2 == "windows")
							{
								type = OABDataFileType.TemplateWin;
								goto IL_136;
							}
							if (a2 == "mac")
							{
								type = OABDataFileType.TemplateMac;
								goto IL_136;
							}
						}
						throw new InvalidDataException(string.Format("Invalid element at stream position {0} because 'type' attribute has unexpected value: {1}", position, attribute7));
					}
					else
					{
						type = OABDataFileType.Diff;
					}
				}
				else
				{
					type = OABDataFileType.Full;
				}
				IL_136:
				uint sequence;
				if (!uint.TryParse(attribute, out sequence))
				{
					throw new InvalidDataException(string.Format("Ignoring element at stream position {0} because 'seq' attribute has unexpected value: {1}", position, attribute));
				}
				if (string.IsNullOrWhiteSpace(attribute2))
				{
					throw new InvalidDataException(string.Format("Ignoring element at stream position {0} because 'ver' attribute has empty value: {1}", position, attribute2));
				}
				uint compressedSize;
				if (!uint.TryParse(attribute3, out compressedSize))
				{
					throw new InvalidDataException(string.Format("Ignoring element at stream position {0} because 'size' attribute has unexpected value: {1}", position, attribute3));
				}
				uint uncompressedSize;
				if (!uint.TryParse(attribute4, out uncompressedSize))
				{
					throw new InvalidDataException(string.Format("Ignoring element at stream position {0} 'uncompressedsize' attribute has unexpected value: {1}", position, attribute4));
				}
				int? langid = null;
				if (!string.IsNullOrWhiteSpace(attribute6))
				{
					int value;
					if (!int.TryParse(attribute6, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value))
					{
						throw new InvalidDataException(string.Format("Ignoring element at stream position {0} because 'langid' attribute has unexpected value: {1}", position, attribute6));
					}
					langid = new int?(value);
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					throw new InvalidDataException(string.Format("Ignoring element at stream position {0} because element has empty value: {1}", position, text));
				}
				return new OABManifestFile
				{
					Type = type,
					Sequence = sequence,
					Version = attribute2,
					CompressedSize = compressedSize,
					UncompressedSize = uncompressedSize,
					Hash = attribute5,
					Langid = langid,
					FileName = text
				};
			}
			IL_11F:
			throw new InvalidDataException(string.Format("Ignoring element at stream position {0} because it is an unknown element: {1}", position, name));
		}

		internal void Serialize(XmlWriter writer)
		{
			writer.WriteStartElement(OABManifestFile.GetFileType(this.Type));
			writer.WriteAttributeString("seq", this.Sequence.ToString());
			writer.WriteAttributeString("ver", this.Version);
			writer.WriteAttributeString("size", this.CompressedSize.ToString());
			writer.WriteAttributeString("uncompressedsize", this.UncompressedSize.ToString());
			writer.WriteAttributeString("SHA", this.Hash);
			if (this.Langid != null)
			{
				writer.WriteAttributeString("langid", this.Langid.Value.ToString("x4"));
			}
			string templateType = OABManifestFile.GetTemplateType(this.Type);
			if (templateType != null)
			{
				writer.WriteAttributeString("type", templateType);
			}
			writer.WriteString(this.FileName);
			writer.WriteEndElement();
		}

		private static string GetFileType(OABDataFileType fileType)
		{
			switch (fileType)
			{
			case OABDataFileType.Full:
				return "Full";
			case OABDataFileType.Diff:
				return "Diff";
			case OABDataFileType.TemplateMac:
			case OABDataFileType.TemplateWin:
				return "Template";
			default:
				throw new ArgumentException("fileType");
			}
		}

		private static string GetTemplateType(OABDataFileType fileType)
		{
			switch (fileType)
			{
			case OABDataFileType.Full:
			case OABDataFileType.Diff:
				return null;
			case OABDataFileType.TemplateMac:
				return "mac";
			case OABDataFileType.TemplateWin:
				return "windows";
			default:
				throw new ArgumentException("fileType");
			}
		}
	}
}
