using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class GrammarGenerationMetadata
	{
		public GrammarGenerationMetadata(int metadataVersion, string tenantId, Guid runId, string serverName, string serverVersion, DateTime generationTimeUtc, List<GrammarFileMetadata> grammarFiles)
		{
			ValidateArgument.NotNullOrEmpty(tenantId, "tenantId");
			ValidateArgument.NotNullOrEmpty(serverName, "serverName");
			ValidateArgument.NotNullOrEmpty(serverVersion, "serverVersion");
			ValidateArgument.NotNull(grammarFiles, "grammarFiles");
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, "GrammarGenerationMetadata constructor - metadataVersion='{0}', tenantId='{1}', runId='{2}', serverName='{3}', serverVersion='{4}', generationTimeUtc='{5}'", new object[]
			{
				metadataVersion,
				tenantId,
				runId.ToString(),
				serverName,
				serverVersion,
				generationTimeUtc.ToString("u", DateTimeFormatInfo.InvariantInfo)
			});
			this.MetadataVersion = metadataVersion;
			this.TenantId = tenantId;
			this.RunId = runId;
			this.ServerName = serverName;
			this.ServerVersion = serverVersion;
			this.GenerationTimeUtc = generationTimeUtc;
			this.GrammarFiles = grammarFiles;
		}

		public int MetadataVersion { get; private set; }

		public string TenantId { get; private set; }

		public Guid RunId { get; private set; }

		public string ServerName { get; private set; }

		public string ServerVersion { get; private set; }

		public DateTime GenerationTimeUtc { get; private set; }

		public List<GrammarFileMetadata> GrammarFiles { get; private set; }

		public static void Serialize(GrammarGenerationMetadata metadata, string filePath)
		{
			ValidateArgument.NotNull(metadata, "metadata");
			ValidateArgument.NotNullOrEmpty(filePath, "filePath");
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarGenerationMetadata.Serialize - filePath='{0}'", new object[]
			{
				filePath
			});
			using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, Encoding.UTF8))
				{
					xmlTextWriter.WriteStartDocument();
					xmlTextWriter.WriteStartElement("GrammarGeneration");
					xmlTextWriter.WriteAttributeString("metadataVersion", metadata.MetadataVersion.ToString("d"));
					xmlTextWriter.WriteAttributeString("tenantId", metadata.TenantId);
					xmlTextWriter.WriteAttributeString("runId", metadata.RunId.ToString());
					xmlTextWriter.WriteAttributeString("serverName", metadata.ServerName);
					xmlTextWriter.WriteAttributeString("serverVersion", metadata.ServerVersion);
					xmlTextWriter.WriteAttributeString("generationTimeUtc", metadata.GenerationTimeUtc.ToString("u", DateTimeFormatInfo.InvariantInfo));
					xmlTextWriter.WriteStartElement("GrammarFiles");
					foreach (GrammarFileMetadata grammarFileMetadata in metadata.GrammarFiles)
					{
						xmlTextWriter.WriteStartElement("File");
						xmlTextWriter.WriteAttributeString("SHA", grammarFileMetadata.Hash);
						xmlTextWriter.WriteAttributeString("size", grammarFileMetadata.Size.ToString("d"));
						xmlTextWriter.WriteString(grammarFileMetadata.Path);
						xmlTextWriter.WriteEndElement();
					}
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndDocument();
				}
			}
		}

		public static GrammarGenerationMetadata Deserialize(string filePath)
		{
			ValidateArgument.NotNullOrEmpty(filePath, "filePath");
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, null, "GrammarGenerationMetadata.Deserialize - filePath='{0}'", new object[]
			{
				filePath
			});
			List<GrammarFileMetadata> list = new List<GrammarFileMetadata>();
			GrammarGenerationMetadata result;
			using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				using (XmlTextReader xmlTextReader = new XmlTextReader(stream))
				{
					xmlTextReader.ReadToFollowing("GrammarGeneration");
					xmlTextReader.MoveToAttribute("metadataVersion");
					string attribute = xmlTextReader.GetAttribute("metadataVersion");
					int metadataVersion = int.Parse(attribute);
					xmlTextReader.MoveToAttribute("tenantId");
					string attribute2 = xmlTextReader.GetAttribute("tenantId");
					xmlTextReader.MoveToAttribute("runId");
					string attribute3 = xmlTextReader.GetAttribute("runId");
					Guid runId = Guid.Parse(attribute3);
					xmlTextReader.MoveToAttribute("serverName");
					string attribute4 = xmlTextReader.GetAttribute("serverName");
					xmlTextReader.MoveToAttribute("serverVersion");
					string attribute5 = xmlTextReader.GetAttribute("serverVersion");
					xmlTextReader.MoveToAttribute("generationTimeUtc");
					string attribute6 = xmlTextReader.GetAttribute("generationTimeUtc");
					DateTime generationTimeUtc = DateTime.Parse(attribute6, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind);
					xmlTextReader.ReadToFollowing("GrammarFiles");
					while (xmlTextReader.ReadToFollowing("File"))
					{
						xmlTextReader.MoveToAttribute("SHA");
						string attribute7 = xmlTextReader.GetAttribute("SHA");
						xmlTextReader.MoveToAttribute("size");
						string attribute8 = xmlTextReader.GetAttribute("size");
						long size = long.Parse(attribute8);
						string path = xmlTextReader.ReadString();
						list.Add(new GrammarFileMetadata(path, attribute7, size));
					}
					result = new GrammarGenerationMetadata(metadataVersion, attribute2, runId, attribute4, attribute5, generationTimeUtc, list);
				}
			}
			return result;
		}

		private const string RootElement = "GrammarGeneration";

		private const string MetadataVersionAttribute = "metadataVersion";

		private const string TenantIdAttribute = "tenantId";

		private const string RunIdAttribute = "runId";

		private const string ServerNameAttribute = "serverName";

		private const string ServerVersionAttribute = "serverVersion";

		private const string GenerationTimeUtcAttribute = "generationTimeUtc";

		private const string GrammarFilesElement = "GrammarFiles";

		private const string GrammarFileElement = "File";

		private const string FileHashAttribute = "SHA";

		private const string FileSizeAttribute = "size";

		public const int CurrentMetadataVersion = 1;
	}
}
