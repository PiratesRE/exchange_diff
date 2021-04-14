using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OAB;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OABManifest
	{
		public OABManifestAddressList[] AddressLists { get; set; }

		internal void Serialize(Stream stream)
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				CheckCharacters = false,
				OmitXmlDeclaration = false,
				Indent = true,
				CloseOutput = false
			};
			using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
			{
				this.Serialize(xmlWriter);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			for (int i = 0; i < this.AddressLists.Length; i++)
			{
				stringBuilder.Append("AddressList[");
				stringBuilder.Append(i.ToString());
				stringBuilder.Append("]=");
				stringBuilder.AppendLine(this.AddressLists[i].ToString());
			}
			return stringBuilder.ToString();
		}

		public OfflineAddressBookManifestVersion GetVersion()
		{
			if (this.AddressLists == null || this.AddressLists.Length == 0)
			{
				return null;
			}
			List<AddressListSequence> list = new List<AddressListSequence>(this.AddressLists.Length);
			foreach (OABManifestAddressList oabmanifestAddressList in this.AddressLists)
			{
				string id = oabmanifestAddressList.Id;
				uint? num = null;
				foreach (OABManifestFile oabmanifestFile in oabmanifestAddressList.Files)
				{
					if (oabmanifestFile.Type == OABDataFileType.Full)
					{
						num = new uint?(oabmanifestFile.Sequence);
						break;
					}
				}
				if (!string.IsNullOrEmpty(id) && num != null)
				{
					AddressListSequence item = new AddressListSequence
					{
						AddressListId = id,
						Sequence = num.Value
					};
					list.Add(item);
				}
			}
			return new OfflineAddressBookManifestVersion
			{
				AddressLists = list.ToArray()
			};
		}

		public static OABManifest LoadFromFile(string manifestFilePath)
		{
			OABManifest.Tracer.TraceFunction(0L, "OABManifest.LoadFromFile: start");
			OABManifest oabmanifest = null;
			TimeSpan timeout = TimeSpan.FromMilliseconds(100.0);
			try
			{
				bool flag = false;
				int num = 0;
				while (!flag && num < 3)
				{
					try
					{
						using (FileStream fileStream = new FileStream(manifestFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							oabmanifest = OABManifest.Deserialize(fileStream);
							OABManifest.Tracer.TraceDebug<string, OABManifest>(0L, "OABManifest.LoadFromFile: loaded OAB manifest from file {0}:\n\r{1}", manifestFilePath, oabmanifest);
							flag = true;
						}
					}
					catch (IOException arg)
					{
						OABManifest.Tracer.TraceError<string, IOException>(0L, "OABManifest.LoadFromFile: IOException opening file {0}: {1}", manifestFilePath, arg);
					}
					if (!flag)
					{
						Thread.Sleep(timeout);
					}
					num++;
				}
			}
			catch (InvalidDataException arg2)
			{
				OABManifest.Tracer.TraceError<string, InvalidDataException>(0L, "OABManifest.LoadFromFile: unable to load OAB manifest {0} due to exception: {1}", manifestFilePath, arg2);
			}
			OABManifest.Tracer.TraceFunction(0L, "OABManifest.LoadFromFile: end");
			return oabmanifest;
		}

		public static OABManifest LoadFromMailbox(string fileSetId, MailboxSession session)
		{
			OABManifest.Tracer.TraceFunction(0L, "OABManifest.LoadFromMailbox: start");
			OABManifest result = null;
			MailboxFileStore mailboxFileStore = new MailboxFileStore("OAB");
			FileSetItem current = mailboxFileStore.GetCurrent(fileSetId, session);
			using (Stream singleFile = mailboxFileStore.GetSingleFile(current, "oab.xml", session))
			{
				if (singleFile != null)
				{
					try
					{
						result = OABManifest.Deserialize(singleFile);
						goto IL_6B;
					}
					catch (InvalidDataException arg)
					{
						OABManifest.Tracer.TraceError<string, InvalidDataException>(0L, "OABManifest.LoadFromMailbox: unable to load OAB manifest from mailbox fileset {0} due to exception: {1}", fileSetId, arg);
						goto IL_6B;
					}
				}
				OABManifest.Tracer.TraceError<string>(0L, "OABManifest.LoadFromMailbox: unable to load OAB manifest from mailbox fileset {0} because the manifest attachment cannot be found", fileSetId);
				IL_6B:;
			}
			OABManifest.Tracer.TraceFunction(0L, "OABManifest.LoadFromMailbox: end");
			return result;
		}

		public static OABManifest Deserialize(Stream stream)
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				CheckCharacters = false,
				ConformanceLevel = ConformanceLevel.Document,
				IgnoreComments = true,
				IgnoreWhitespace = true,
				IgnoreProcessingInstructions = true,
				CloseInput = false
			};
			OABManifest result;
			using (XmlReader xmlReader = XmlReader.Create(stream, settings))
			{
				result = OABManifest.Deserialize(stream, xmlReader);
			}
			return result;
		}

		private void Serialize(XmlWriter writer)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("OAB");
			foreach (OABManifestAddressList oabmanifestAddressList in this.AddressLists)
			{
				oabmanifestAddressList.Serialize(writer);
			}
			writer.WriteEndElement();
		}

		private static OABManifest Deserialize(Stream stream, XmlReader reader)
		{
			long position = stream.Position;
			try
			{
				reader.MoveToContent();
				reader.ReadStartElement("OAB");
			}
			catch (XmlException arg)
			{
				throw new InvalidDataException(string.Format("Invalid element at position {0} due exception: {1}", position, arg));
			}
			List<OABManifestAddressList> list = new List<OABManifestAddressList>(1);
			while (reader.NodeType == XmlNodeType.Element)
			{
				OABManifestAddressList oabmanifestAddressList;
				try
				{
					oabmanifestAddressList = OABManifestAddressList.Deserialize(stream, reader);
				}
				catch (InvalidDataException arg2)
				{
					OABManifest.Tracer.TraceError<InvalidDataException>(0L, "Ignoring element due exception: {0}", arg2);
					continue;
				}
				OABManifest.Tracer.TraceDebug<OABManifestAddressList>(0L, "Parsed address list from stream: {0}", oabmanifestAddressList);
				list.Add(oabmanifestAddressList);
			}
			position = stream.Position;
			try
			{
				reader.ReadEndElement();
			}
			catch (XmlException arg3)
			{
				throw new InvalidDataException(string.Format("Invalid element at position {0} due exception: {1}", position, arg3));
			}
			OABManifest.Tracer.TraceDebug<int>(0L, "Parsed {0} address lists from stream", list.Count);
			return new OABManifest
			{
				AddressLists = list.ToArray()
			};
		}

		private const string OABFolderName = "OAB";

		private const string OABManifestFileName = "oab.xml";

		private static readonly Trace Tracer = ExTraceGlobals.DataTracer;
	}
}
