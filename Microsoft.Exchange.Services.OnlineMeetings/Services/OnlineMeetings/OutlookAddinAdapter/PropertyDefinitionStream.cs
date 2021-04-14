using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	internal class PropertyDefinitionStream
	{
		internal PropertyDefinitionStream(byte[] propDefBlob)
		{
			if (propDefBlob != null)
			{
				using (MemoryStream memoryStream = new MemoryStream(propDefBlob))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						this.Read(binaryReader);
					}
				}
			}
		}

		internal List<FieldDefinitionStream> FieldDefinitions
		{
			get
			{
				return this.fieldDefinitions;
			}
		}

		internal void AddFieldDefinition(FieldDefinitionStream data)
		{
			this.fieldDefinitions.Add(data);
		}

		internal byte[] GetByteArray()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					this.Write(binaryWriter);
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		private void Read(BinaryReader reader)
		{
			PropertyDefinitionStreamVersion version = (PropertyDefinitionStreamVersion)reader.ReadUInt16();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				FieldDefinitionStream item = FieldDefinitionStream.Read(reader, version);
				this.fieldDefinitions.Add(item);
			}
		}

		private void Write(BinaryWriter writer)
		{
			writer.Write(259);
			writer.Write(this.fieldDefinitions.Count);
			foreach (FieldDefinitionStream fieldDefinitionStream in this.fieldDefinitions)
			{
				fieldDefinitionStream.Write(writer);
			}
		}

		private readonly List<FieldDefinitionStream> fieldDefinitions = new List<FieldDefinitionStream>();
	}
}
