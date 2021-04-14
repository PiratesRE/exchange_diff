using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OAB;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OABManifestAddressList
	{
		public string Id { get; set; }

		public string DN { get; set; }

		public string Name { get; set; }

		public OABManifestFile[] Files { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.Append("Id=");
			stringBuilder.Append(this.Id);
			stringBuilder.Append(", DN:");
			stringBuilder.Append(this.DN);
			stringBuilder.Append(", Name:");
			stringBuilder.Append(this.Name);
			stringBuilder.Append(", file count:");
			stringBuilder.Append(this.Files.Length.ToString());
			return stringBuilder.ToString();
		}

		internal static OABManifestAddressList Deserialize(Stream stream, XmlReader reader)
		{
			long position = stream.Position;
			if (!reader.IsStartElement("OAL"))
			{
				throw new InvalidDataException(string.Format("Invalid element at stream position {0} due non-expected element name: {1}", position, reader.Name));
			}
			string attribute;
			string attribute2;
			string attribute3;
			try
			{
				position = stream.Position;
				attribute = reader.GetAttribute("id");
				position = stream.Position;
				attribute2 = reader.GetAttribute("dn");
				position = stream.Position;
				attribute3 = reader.GetAttribute("name");
				position = stream.Position;
				reader.ReadStartElement();
			}
			catch (XmlException arg)
			{
				throw new InvalidDataException(string.Format("Invalid element at stream position {0} due exception: {1}", position, arg));
			}
			List<OABManifestFile> list = new List<OABManifestFile>(60);
			while (reader.NodeType == XmlNodeType.Element)
			{
				position = stream.Position;
				OABManifestFile oabmanifestFile = OABManifestFile.Deserialize(stream, reader);
				OABManifestAddressList.Tracer.TraceDebug<long, OABManifestFile>(0L, "Parsed file element from stream position {0}: {1}", position, oabmanifestFile);
				list.Add(oabmanifestFile);
			}
			position = stream.Position;
			try
			{
				reader.ReadEndElement();
			}
			catch (XmlException arg2)
			{
				throw new InvalidDataException(string.Format("Invalid element at stream position {0} due exception: {1}", position, arg2));
			}
			return new OABManifestAddressList
			{
				Id = attribute,
				DN = attribute2,
				Name = attribute3,
				Files = list.ToArray()
			};
		}

		internal void Serialize(XmlWriter writer)
		{
			writer.WriteStartElement("OAL");
			writer.WriteAttributeString("id", this.Id);
			writer.WriteAttributeString("dn", this.DN);
			writer.WriteAttributeString("name", this.Name);
			foreach (OABManifestFile oabmanifestFile in this.Files)
			{
				oabmanifestFile.Serialize(writer);
			}
			writer.WriteEndElement();
		}

		private static readonly Trace Tracer = ExTraceGlobals.DataTracer;
	}
}
