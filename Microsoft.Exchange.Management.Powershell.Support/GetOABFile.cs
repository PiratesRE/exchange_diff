using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.OAB;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "OABFile")]
	public sealed class GetOABFile : Task
	{
		[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public string Path { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter Data { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter Metadata { get; set; }

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			using (FileStream fileStream = new FileStream(this.Path, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader reader = this.GetReader(fileStream))
				{
					OABFileHeader oabfileHeader = OABFileHeader.ReadFrom(reader);
					OABFileProperties oabfileProperties = OABFileProperties.ReadFrom(reader, "Properties");
					PropTag[] properties = Array.ConvertAll<OABPropertyDescriptor, PropTag>(oabfileProperties.HeaderProperties, (OABPropertyDescriptor propertyDescriptor) => propertyDescriptor.PropTag);
					OABFileRecord record = OABFileRecord.ReadFrom(reader, properties, "AddressListRecord");
					PropTag[] properties2 = Array.ConvertAll<OABPropertyDescriptor, PropTag>(oabfileProperties.DetailProperties, (OABPropertyDescriptor propertyDescriptor) => propertyDescriptor.PropTag);
					if (this.Metadata)
					{
						GetOABFile.FileMetadata fileMetadata = new GetOABFile.FileMetadata();
						fileMetadata.Version = oabfileHeader.Version;
						fileMetadata.RecordCount = oabfileHeader.RecordCount;
						fileMetadata.CRC = oabfileHeader.CRC;
						fileMetadata.AddressListProperties = Array.ConvertAll<OABPropertyDescriptor, GetOABFile.PropertyDescriptor>(oabfileProperties.HeaderProperties, (OABPropertyDescriptor propertyDescriptor) => new GetOABFile.PropertyDescriptor(propertyDescriptor));
						fileMetadata.RecordProperties = Array.ConvertAll<OABPropertyDescriptor, GetOABFile.PropertyDescriptor>(oabfileProperties.DetailProperties, (OABPropertyDescriptor propertyDescriptor) => new GetOABFile.PropertyDescriptor(propertyDescriptor));
						fileMetadata.AddressList = new GetOABFile.Record(record);
						GetOABFile.FileMetadata sendToPipeline = fileMetadata;
						base.WriteObject(sendToPipeline);
					}
					if (this.Data)
					{
						for (int i = 0; i < oabfileHeader.RecordCount; i++)
						{
							OABFileRecord record2 = OABFileRecord.ReadFrom(reader, properties2, "Record[" + i + "]");
							base.WriteObject(new GetOABFile.Record(record2));
						}
					}
				}
			}
			TaskLogger.LogExit();
		}

		private BinaryReader GetReader(Stream stream)
		{
			string a;
			if ((a = System.IO.Path.GetExtension(this.Path).ToLower()) != null)
			{
				if (a == ".lzx")
				{
					return new BinaryReader(new OABDecompressStream(stream));
				}
				if (a == ".flt")
				{
					return new BinaryReader(stream);
				}
			}
			throw new InvalidDataException();
		}

		public sealed class FileMetadata
		{
			public int Version { get; set; }

			public int RecordCount { get; set; }

			public uint CRC { get; set; }

			public GetOABFile.PropertyDescriptor[] AddressListProperties { get; set; }

			public GetOABFile.PropertyDescriptor[] RecordProperties { get; set; }

			public GetOABFile.Record AddressList { get; set; }
		}

		[Flags]
		public enum PropertyFlags
		{
			None = 0,
			ANR = 1,
			RDN = 2,
			Index = 4,
			Truncated = 8
		}

		public sealed class PropertyDescriptor
		{
			public GetOABFile.PropertyIdentity Id { get; private set; }

			public GetOABFile.PropertyFlags Flags { get; private set; }

			internal PropertyDescriptor(OABPropertyDescriptor propertyDescriptor)
			{
				this.Id = new GetOABFile.PropertyIdentity(propertyDescriptor.PropTag);
				this.Flags = (GetOABFile.PropertyFlags)propertyDescriptor.PropFlags;
			}

			public override string ToString()
			{
				return string.Concat(new object[]
				{
					"Id=",
					this.Id,
					",Flags=",
					this.Flags
				});
			}
		}

		public sealed class Record
		{
			public string Identity { get; private set; }

			public string DisplayName { get; private set; }

			public GetOABFile.PropertyValueCollection Properties { get; set; }

			internal Record(OABFileRecord record)
			{
				foreach (OABPropertyValue oabpropertyValue in record.PropertyValues)
				{
					if (oabpropertyValue != null)
					{
						if (oabpropertyValue.PropTag == PropTag.DisplayName)
						{
							this.DisplayName = (string)oabpropertyValue.Value;
						}
						if (oabpropertyValue.PropTag == (PropTag)2355953922U)
						{
							this.Identity = new Guid((byte[])oabpropertyValue.Value).ToString();
						}
					}
				}
				this.Properties = new GetOABFile.PropertyValueCollection(record.PropertyValues);
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder(1000);
				foreach (GetOABFile.PropertyIdentityValue propertyIdentityValue in this.Properties)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(propertyIdentityValue.ToString());
				}
				return stringBuilder.ToString();
			}
		}

		public sealed class PropertyValueCollection : List<GetOABFile.PropertyIdentityValue>
		{
			internal PropertyValueCollection(OABPropertyValue[] properties)
			{
				foreach (OABPropertyValue oabpropertyValue in properties)
				{
					if (oabpropertyValue != null)
					{
						base.Add(new GetOABFile.PropertyIdentityValue(oabpropertyValue));
					}
				}
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder(1000);
				foreach (GetOABFile.PropertyIdentityValue propertyIdentityValue in this)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(propertyIdentityValue.ToString());
				}
				return stringBuilder.ToString();
			}
		}

		public sealed class PropertyIdentityValue
		{
			public GetOABFile.PropertyIdentity Id { get; private set; }

			public GetOABFile.PropertyValue Value { get; private set; }

			internal PropertyIdentityValue(OABPropertyValue propertyValue)
			{
				this.Id = new GetOABFile.PropertyIdentity(propertyValue.PropTag);
				this.Value = new GetOABFile.PropertyValue(propertyValue.PropTag.ValueType(), propertyValue.Value);
			}

			public override string ToString()
			{
				return this.Id + "=" + this.Value;
			}
		}

		public sealed class PropertyIdentity
		{
			public uint Id { get; private set; }

			internal PropertyIdentity(PropTag propTag)
			{
				this.Id = (uint)propTag;
			}

			public override string ToString()
			{
				if (Enum.IsDefined(typeof(PropTag), this.Id))
				{
					PropTag id = (PropTag)this.Id;
					return id.ToString();
				}
				if (Enum.IsDefined(typeof(CustomPropTag), this.Id))
				{
					CustomPropTag id2 = (CustomPropTag)this.Id;
					return id2.ToString();
				}
				return this.Id.ToString("X8");
			}
		}

		public sealed class PropertyValue
		{
			public object Value { get; private set; }

			internal PropertyValue(PropType propType, object value)
			{
				this.propType = propType;
				this.Value = value;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				PropTypeHandler handler = PropTypeHandler.GetHandler(this.propType);
				handler.AppendText(stringBuilder, this.Value);
				return stringBuilder.ToString();
			}

			private PropType propType;
		}
	}
}
