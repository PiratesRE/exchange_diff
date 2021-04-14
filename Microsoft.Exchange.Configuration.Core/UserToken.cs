using System;
using System.IO;
using System.IO.Compression;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Configuration.Core.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Configuration.Core
{
	public class UserToken
	{
		internal UserToken(AuthenticationType authenticationType, DelegatedPrincipal delegatedPrincipal, string windowsLiveId, string userName, SecurityIdentifier userSid, PartitionId partitionId, OrganizationId organization, string managedOrganization, bool appPasswordUsed, CommonAccessToken commonAccessToken)
		{
			ExTraceGlobals.UserTokenTracer.TraceDebug(0L, "Version:{0}; AuthenticationType:{1}; DelegatedPrincipal:{2} WindowsLiveId:{3}; UserName:{4}; UserSid:{5}; PartitionId:{6}; Organization:{7}; ManagedOrg:{8};AppPasswordUsed:{9}; CAT:{10}", new object[]
			{
				0,
				authenticationType,
				delegatedPrincipal,
				windowsLiveId,
				userName,
				userSid,
				partitionId,
				organization,
				managedOrganization,
				appPasswordUsed,
				commonAccessToken
			});
			this.Version = 0;
			this.AuthenticationType = authenticationType;
			this.DelegatedPrincipal = delegatedPrincipal;
			this.WindowsLiveId = windowsLiveId;
			this.UserName = userName;
			this.UserSid = userSid;
			this.PartitionId = partitionId;
			this.Organization = organization;
			this.ManagedOrganization = managedOrganization;
			this.AppPasswordUsed = appPasswordUsed;
			this.CommonAccessToken = commonAccessToken;
		}

		private UserToken(Stream stream)
		{
			this.Deserialize(stream);
		}

		internal ushort Version { get; private set; }

		internal AuthenticationType AuthenticationType { get; private set; }

		internal DelegatedPrincipal DelegatedPrincipal { get; private set; }

		internal string WindowsLiveId { get; private set; }

		internal string UserName { get; private set; }

		internal SecurityIdentifier UserSid { get; set; }

		internal PartitionId PartitionId { get; private set; }

		internal OrganizationId Organization { get; private set; }

		internal string ManagedOrganization { get; private set; }

		internal CommonAccessToken CommonAccessToken { get; set; }

		internal bool HasCommonAccessToken
		{
			get
			{
				return this.CommonAccessToken != null;
			}
		}

		internal bool AppPasswordUsed { get; private set; }

		internal ADRawEntry Recipient
		{
			get
			{
				return UserTokenStaticHelper.GetADRawEntry(this);
			}
		}

		public static UserToken Deserialize(string token)
		{
			ExTraceGlobals.UserTokenTracer.TraceDebug<string>(0L, "token={0}", token);
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			UserToken result;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(token)))
				{
					result = new UserToken(memoryStream);
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.UserTokenTracer.TraceError<Exception>(0L, "Exception from Deserialize: {0}", ex);
				throw new UserTokenException(ex.Message, ex);
			}
			return result;
		}

		public string Serialize()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					this.Serialize(binaryWriter);
				}
				string text = Convert.ToBase64String(memoryStream.ToArray());
				ExTraceGlobals.UserTokenTracer.TraceDebug<string>(0L, "SerializedString={0}", text);
				result = text;
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("Version:{0}; AuthenticationType:{1}; DelegatedPrincipal:{2} WindowsLiveId:{3}; UserName:{4}; UserSid:{5}; PartitionId:{6}; Organization:{7}; ManagedOrg:{8};AppPasswordUsed:{9}; CAT:{10}", new object[]
			{
				this.Version,
				this.AuthenticationType,
				this.DelegatedPrincipal,
				this.WindowsLiveId,
				this.UserName,
				this.UserSid,
				this.PartitionId,
				this.Organization,
				this.ManagedOrganization,
				this.AppPasswordUsed,
				this.CommonAccessToken
			});
		}

		private void Serialize(BinaryWriter writer)
		{
			writer.Write('V');
			writer.Write(this.Version);
			writer.Write('A');
			writer.Write(this.AuthenticationType.ToString());
			writer.Write('D');
			this.WriteNullableValue(writer, (this.DelegatedPrincipal != null) ? this.DelegatedPrincipal.Identity.Name : null);
			writer.Write('L');
			this.WriteNullableValue(writer, this.WindowsLiveId);
			writer.Write('N');
			this.WriteNullableValue(writer, this.UserName);
			writer.Write('U');
			this.WriteNullableValue(writer, (this.UserSid != null) ? this.UserSid.ToString() : null);
			writer.Write('P');
			this.WriteNullableValue(writer, (this.PartitionId != null) ? this.PartitionId.ToString() : null);
			writer.Write('O');
			string value = null;
			if (this.Organization != null)
			{
				value = Convert.ToBase64String(this.Organization.GetBytes(Encoding.UTF8));
			}
			this.WriteNullableValue(writer, value);
			writer.Write('M');
			this.WriteNullableValue(writer, this.ManagedOrganization);
			writer.Write('W');
			writer.Write(this.AppPasswordUsed);
			if (this.CommonAccessToken != null)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(this.CommonAccessToken.Serialize());
				using (GZipStream gzipStream = new GZipStream(writer.BaseStream, CompressionMode.Compress, true))
				{
					gzipStream.Write(bytes, 0, bytes.Length);
				}
			}
			writer.Flush();
		}

		private void WriteNullableValue(BinaryWriter writer, string value)
		{
			if (value == null)
			{
				writer.Write("0");
				return;
			}
			writer.Write(value.Replace("0", "\\0"));
		}

		private void Deserialize(Stream stream)
		{
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				this.ReadAndValidateFieldType(binaryReader, 'V', Strings.MissingVersion);
				this.Version = this.BinaryRead<ushort>(new Func<ushort>(binaryReader.ReadUInt16), Strings.MissingVersion);
				this.ReadAndValidateFieldType(binaryReader, 'A', Strings.MissingAuthenticationType);
				string value = this.BinaryRead<string>(new Func<string>(binaryReader.ReadString), Strings.MissingAuthenticationType);
				AuthenticationType authenticationType;
				if (!Enum.TryParse<AuthenticationType>(value, out authenticationType))
				{
					ExTraceGlobals.UserTokenTracer.TraceError<AuthenticationType>(0L, "Invalid authentication type {0}", authenticationType);
					throw new UserTokenException(Strings.InvalidDelegatedPrincipal(value));
				}
				this.AuthenticationType = authenticationType;
				this.ReadAndValidateFieldType(binaryReader, 'D', Strings.MissingDelegatedPrincipal);
				string text = this.ReadNullableString(binaryReader, Strings.MissingDelegatedPrincipal);
				DelegatedPrincipal delegatedPrincipal = null;
				if (text != null && !DelegatedPrincipal.TryParseDelegatedString(text, out delegatedPrincipal))
				{
					ExTraceGlobals.UserTokenTracer.TraceError<string>(0L, "Invalid delegated principal {0}", text);
					throw new UserTokenException(Strings.InvalidDelegatedPrincipal(text));
				}
				this.DelegatedPrincipal = delegatedPrincipal;
				this.ReadAndValidateFieldType(binaryReader, 'L', Strings.MissingWindowsLiveId);
				this.WindowsLiveId = this.ReadNullableString(binaryReader, Strings.MissingWindowsLiveId);
				this.ReadAndValidateFieldType(binaryReader, 'N', Strings.MissingUserName);
				this.UserName = this.ReadNullableString(binaryReader, Strings.MissingUserName);
				this.ReadAndValidateFieldType(binaryReader, 'U', Strings.MissingUserSid);
				string text2 = this.ReadNullableString(binaryReader, Strings.MissingUserSid);
				if (text2 != null)
				{
					try
					{
						this.UserSid = new SecurityIdentifier(text2);
					}
					catch (ArgumentException innerException)
					{
						ExTraceGlobals.UserTokenTracer.TraceError<string>(0L, "Invalid user sid {0}", text2);
						throw new UserTokenException(Strings.InvalidUserSid(text2), innerException);
					}
				}
				this.ReadAndValidateFieldType(binaryReader, 'P', Strings.MissingPartitionId);
				string text3 = this.ReadNullableString(binaryReader, Strings.MissingPartitionId);
				PartitionId partitionId = null;
				if (text3 != null && !PartitionId.TryParse(text3, out partitionId))
				{
					ExTraceGlobals.UserTokenTracer.TraceError<string>(0L, "Invalid partition id {0}", text3);
					throw new UserTokenException(Strings.InvalidPartitionId(text3));
				}
				this.PartitionId = partitionId;
				this.ReadAndValidateFieldType(binaryReader, 'O', Strings.MissingOrganization);
				string text4 = this.ReadNullableString(binaryReader, Strings.MissingOrganization);
				if (text4 != null)
				{
					byte[] bytes;
					try
					{
						bytes = Convert.FromBase64String(text4);
					}
					catch (FormatException innerException2)
					{
						ExTraceGlobals.UserTokenTracer.TraceError<string>(0L, "Invalid organization id {0}", text4);
						throw new UserTokenException(Strings.InvalidOrganization(text4), innerException2);
					}
					OrganizationId organization;
					if (!OrganizationId.TryCreateFromBytes(bytes, Encoding.UTF8, out organization))
					{
						ExTraceGlobals.UserTokenTracer.TraceError<string>(0L, "Invalid organization id {0}", text4);
						throw new UserTokenException(Strings.InvalidOrganization(text4));
					}
					this.Organization = organization;
				}
				this.ReadAndValidateFieldType(binaryReader, 'M', Strings.MissingManagedOrganization);
				this.ManagedOrganization = this.ReadNullableString(binaryReader, Strings.MissingManagedOrganization);
				this.ReadAndValidateFieldType(binaryReader, 'W', Strings.MissingAppPasswordUsed);
				this.AppPasswordUsed = this.BinaryRead<bool>(new Func<bool>(binaryReader.ReadBoolean), Strings.MissingAppPasswordUsed);
				int num = (int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position);
				if (num > 0)
				{
					byte[] array = binaryReader.ReadBytes(num);
					array = this.Decompress(array);
					this.CommonAccessToken = CommonAccessToken.Deserialize(Encoding.UTF8.GetString(array));
				}
				else
				{
					this.CommonAccessToken = null;
				}
			}
		}

		private void ReadAndValidateFieldType(BinaryReader reader, char fieldType, LocalizedString errorMessage)
		{
			char c = this.BinaryRead<char>(new Func<char>(reader.ReadChar), errorMessage);
			if (c != fieldType)
			{
				ExTraceGlobals.UserTokenTracer.TraceError<char>(0L, "Invalid field char {0}", c);
				throw new UserTokenException(errorMessage);
			}
		}

		private string ReadNullableString(BinaryReader reader, LocalizedString errorMessage)
		{
			string result;
			try
			{
				string text = reader.ReadString();
				if ("0".Equals(text))
				{
					result = null;
				}
				else
				{
					result = text.Replace("\\0", "0");
				}
			}
			catch (EndOfStreamException ex)
			{
				ExTraceGlobals.UserTokenTracer.TraceError<EndOfStreamException>(0L, "Unexpected end of stream. Exception: {0}", ex);
				throw new UserTokenException(errorMessage, ex);
			}
			return result;
		}

		private T BinaryRead<T>(Func<T> readMethod, LocalizedString errorMessage)
		{
			T result;
			try
			{
				result = readMethod();
			}
			catch (EndOfStreamException ex)
			{
				ExTraceGlobals.UserTokenTracer.TraceError<EndOfStreamException>(0L, "Unexpected end of stream. Exception: {0}", ex);
				throw new UserTokenException(errorMessage, ex);
			}
			return result;
		}

		private byte[] Decompress(byte[] compressedBytes)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (MemoryStream memoryStream2 = new MemoryStream(compressedBytes))
				{
					using (GZipStream gzipStream = new GZipStream(memoryStream2, CompressionMode.Decompress))
					{
						gzipStream.CopyTo(memoryStream);
					}
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		public const string KeyToStoreUserToken = "X-EX-UserToken";

		private const string NullString = "0";

		private const string TranslatedZeroString = "\\0";

		private const string StringFmt = "Version:{0}; AuthenticationType:{1}; DelegatedPrincipal:{2} WindowsLiveId:{3}; UserName:{4}; UserSid:{5}; PartitionId:{6}; Organization:{7}; ManagedOrg:{8};AppPasswordUsed:{9}; CAT:{10}";
	}
}
