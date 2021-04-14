using System;
using System.IO;
using System.Security.Principal;
using System.Text;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class WacFileRep
	{
		public WacFileRep(SecurityIdentifier logonSid, bool directFileAccessEnabled, bool externalServicesEnabled, bool wacOMEXEnabled, bool isEdit, bool isArchive) : this(logonSid, DateTime.UtcNow, directFileAccessEnabled, externalServicesEnabled, wacOMEXEnabled, isEdit, isArchive)
		{
		}

		private WacFileRep(SecurityIdentifier logonSid, DateTime creationTime, bool directFileAccessEnabled, bool externalServicesEnabled, bool wacOMEXEnabled, bool isEdit, bool isArchive)
		{
			this.LogonSid = logonSid;
			this.CreationTime = creationTime;
			this.DirectFileAccessEnabled = directFileAccessEnabled;
			this.WacExternalServicesEnabled = externalServicesEnabled;
			this.OMEXEnabled = wacOMEXEnabled;
			this.IsEdit = isEdit;
			this.IsArchive = isArchive;
		}

		public SecurityIdentifier LogonSid { get; private set; }

		public DateTime CreationTime { get; private set; }

		public bool DirectFileAccessEnabled { get; private set; }

		public bool OMEXEnabled { get; private set; }

		public bool WacExternalServicesEnabled { get; private set; }

		public bool IsEdit { get; private set; }

		public bool IsArchive { get; private set; }

		public static WacFileRep Parse(string fileRepAsString)
		{
			byte[] array = WacUtilities.FromBase64String(fileRepAsString);
			WacFileRep wacFileRep = null;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(array))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream, new UTF8Encoding()))
					{
						string text = binaryReader.ReadString();
						DateTime creationTime = new DateTime(binaryReader.ReadInt64());
						bool directFileAccessEnabled = binaryReader.ReadBoolean();
						bool externalServicesEnabled = binaryReader.ReadBoolean();
						bool wacOMEXEnabled = binaryReader.ReadBoolean();
						bool isEdit = binaryReader.ReadBoolean();
						bool isArchive = binaryReader.ReadBoolean();
						wacFileRep = new WacFileRep(new SecurityIdentifier(text), creationTime, directFileAccessEnabled, externalServicesEnabled, wacOMEXEnabled, isEdit, isArchive);
						if (!wacFileRep.LogonSid.IsAccountSid())
						{
							throw new OwaInvalidRequestException("WacFileRep contained an invalid SecurityIdentifier: " + text);
						}
					}
				}
			}
			catch (EndOfStreamException)
			{
				throw new OwaInvalidRequestException("Unable to parse WacRequest. (" + array.Length.ToString() + " bytes)");
			}
			return wacFileRep;
		}

		internal string Serialize()
		{
			byte[] inArray;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, new UTF8Encoding()))
				{
					binaryWriter.Write(this.LogonSid.Value);
					binaryWriter.Write(this.CreationTime.Ticks);
					binaryWriter.Write(this.DirectFileAccessEnabled);
					binaryWriter.Write(this.WacExternalServicesEnabled);
					binaryWriter.Write(this.OMEXEnabled);
					binaryWriter.Write(this.IsEdit);
					binaryWriter.Write(this.IsArchive);
				}
				inArray = memoryStream.ToArray();
			}
			return Convert.ToBase64String(inArray);
		}
	}
}
