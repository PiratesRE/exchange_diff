using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetUserConfiguration : UserConfigurationCommandBase<GetUserConfigurationRequest, ServiceUserConfiguration>
	{
		public GetUserConfiguration(CallContext callContext, GetUserConfigurationRequest request) : base(callContext, request)
		{
			this.userConfigurationName = base.Request.UserConfigurationName;
			this.userConfigurationProperties = base.Request.UserConfigurationProperties;
			ServiceCommandBase.ThrowIfNull(this.userConfigurationName, "userConfigurationName", "GetUserConfiguration:Execute");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			GetUserConfigurationResponse getUserConfigurationResponse = new GetUserConfigurationResponse();
			getUserConfigurationResponse.ProcessServiceResult(base.Result);
			return getUserConfigurationResponse;
		}

		internal override ServiceResult<ServiceUserConfiguration> Execute()
		{
			UserConfigurationCommandBase<GetUserConfigurationRequest, ServiceUserConfiguration>.UserConfigurationName userConfigurationName = base.GetUserConfigurationName(this.userConfigurationName);
			ServiceUserConfiguration value;
			using (UserConfiguration userConfiguration = UserConfigurationCommandBase<GetUserConfigurationRequest, ServiceUserConfiguration>.Get(userConfigurationName))
			{
				value = this.PrepareServiceUserConfiguration(userConfiguration, userConfigurationName.MailboxSession);
			}
			return new ServiceResult<ServiceUserConfiguration>(value);
		}

		private ServiceUserConfiguration PrepareServiceUserConfiguration(UserConfiguration userConfiguration, MailboxSession mailboxSession)
		{
			ServiceUserConfiguration serviceUserConfiguration = new ServiceUserConfiguration();
			serviceUserConfiguration.UserConfigurationName = this.userConfigurationName;
			if (this.IsPropertyRequested(UserConfigurationProperties.Id))
			{
				serviceUserConfiguration.ItemId = this.GetItemId(userConfiguration, mailboxSession);
			}
			if (this.IsPropertyRequested(UserConfigurationProperties.Dictionary) && UserConfigurationCommandBase<GetUserConfigurationRequest, ServiceUserConfiguration>.PropertyExists(UserConfigurationTypes.Dictionary, userConfiguration.DataTypes))
			{
				serviceUserConfiguration.Dictionary = this.GetDictionaryEntries(userConfiguration);
			}
			if (this.IsPropertyRequested(UserConfigurationProperties.BinaryData) && UserConfigurationCommandBase<GetUserConfigurationRequest, ServiceUserConfiguration>.PropertyExists(UserConfigurationTypes.Stream, userConfiguration.DataTypes))
			{
				serviceUserConfiguration.BinaryData = this.GetBinaryData(userConfiguration);
			}
			if (this.IsPropertyRequested(UserConfigurationProperties.XmlData) && UserConfigurationCommandBase<GetUserConfigurationRequest, ServiceUserConfiguration>.PropertyExists(UserConfigurationTypes.XML, userConfiguration.DataTypes))
			{
				serviceUserConfiguration.XmlData = this.GetXmlData(userConfiguration);
			}
			return serviceUserConfiguration;
		}

		private bool IsPropertyRequested(UserConfigurationProperties userConfigurationProperties)
		{
			return (userConfigurationProperties & this.userConfigurationProperties) == userConfigurationProperties;
		}

		private ItemId GetItemId(UserConfiguration userConfiguration, MailboxSession mailboxSession)
		{
			ServiceXml.CreateElement(new SafeXmlDocument(), "TemporaryContainerName", ServiceXml.DefaultNamespaceUri);
			ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(userConfiguration.VersionedId, new MailboxId(mailboxSession), null);
			return new ItemId
			{
				Id = concatenatedId.Id,
				ChangeKey = concatenatedId.ChangeKey
			};
		}

		private UserConfigurationDictionaryEntry[] GetDictionaryEntries(UserConfiguration userConfiguration)
		{
			UserConfigurationDictionaryEntry[] array = null;
			ConfigurationDictionary dictionary = UserConfigurationCommandBase<GetUserConfigurationRequest, ServiceUserConfiguration>.GetDictionary(userConfiguration);
			if (dictionary.Count > 0)
			{
				array = new UserConfigurationDictionaryEntry[dictionary.Count];
				int num = 0;
				foreach (object obj in dictionary)
				{
					DictionaryEntry storeDictionaryEntry = (DictionaryEntry)obj;
					array[num] = this.CreateUserConfigurationDictionaryEntry(storeDictionaryEntry);
					num++;
				}
			}
			return array;
		}

		private UserConfigurationDictionaryEntry CreateUserConfigurationDictionaryEntry(DictionaryEntry storeDictionaryEntry)
		{
			UserConfigurationDictionaryEntry userConfigurationDictionaryEntry = new UserConfigurationDictionaryEntry();
			userConfigurationDictionaryEntry.DictionaryKey = this.ConstructDictionaryObject(storeDictionaryEntry.Key);
			if (storeDictionaryEntry.Value != null)
			{
				userConfigurationDictionaryEntry.DictionaryValue = this.ConstructDictionaryObject(storeDictionaryEntry.Value);
			}
			return userConfigurationDictionaryEntry;
		}

		private UserConfigurationDictionaryObject ConstructDictionaryObject(object storeDictionaryObject)
		{
			UserConfigurationDictionaryObject userConfigurationDictionaryObject = new UserConfigurationDictionaryObject();
			string[] array = storeDictionaryObject as string[];
			if (array != null)
			{
				userConfigurationDictionaryObject.Type = UserConfigurationDictionaryObjectType.StringArray;
				userConfigurationDictionaryObject.Value = array;
			}
			else
			{
				userConfigurationDictionaryObject.Value = new string[1];
				byte[] array2 = storeDictionaryObject as byte[];
				if (array2 != null)
				{
					userConfigurationDictionaryObject.Type = UserConfigurationDictionaryObjectType.ByteArray;
					userConfigurationDictionaryObject.Value[0] = Base64StringConverter.ToString(array2);
				}
				else if (storeDictionaryObject is ExDateTime)
				{
					userConfigurationDictionaryObject.Type = UserConfigurationDictionaryObjectType.DateTime;
					userConfigurationDictionaryObject.Value[0] = ExDateTimeConverter.ToUtcXsdDateTime((ExDateTime)storeDictionaryObject);
				}
				else
				{
					TypeCode typeCode = Type.GetTypeCode(storeDictionaryObject.GetType());
					switch (typeCode)
					{
					case TypeCode.Boolean:
						userConfigurationDictionaryObject.Type = UserConfigurationDictionaryObjectType.Boolean;
						userConfigurationDictionaryObject.Value[0] = BooleanConverter.ToString((bool)storeDictionaryObject);
						break;
					case TypeCode.Char:
					case TypeCode.SByte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
						break;
					case TypeCode.Byte:
						userConfigurationDictionaryObject.Type = UserConfigurationDictionaryObjectType.Byte;
						userConfigurationDictionaryObject.Value[0] = ByteConverter.ToString((byte)storeDictionaryObject);
						break;
					case TypeCode.Int32:
						userConfigurationDictionaryObject.Type = UserConfigurationDictionaryObjectType.Integer32;
						userConfigurationDictionaryObject.Value[0] = IntConverter.ToString((int)storeDictionaryObject);
						break;
					case TypeCode.UInt32:
						userConfigurationDictionaryObject.Type = UserConfigurationDictionaryObjectType.UnsignedInteger32;
						userConfigurationDictionaryObject.Value[0] = UIntConverter.ToString((uint)storeDictionaryObject);
						break;
					case TypeCode.Int64:
						userConfigurationDictionaryObject.Type = UserConfigurationDictionaryObjectType.Integer64;
						userConfigurationDictionaryObject.Value[0] = LongConverter.ToString((long)storeDictionaryObject);
						break;
					case TypeCode.UInt64:
						userConfigurationDictionaryObject.Type = UserConfigurationDictionaryObjectType.UnsignedInteger64;
						userConfigurationDictionaryObject.Value[0] = ULongConverter.ToString((ulong)storeDictionaryObject);
						break;
					default:
						if (typeCode == TypeCode.String)
						{
							userConfigurationDictionaryObject.Type = UserConfigurationDictionaryObjectType.String;
							userConfigurationDictionaryObject.Value[0] = (string)storeDictionaryObject;
						}
						break;
					}
				}
			}
			return userConfigurationDictionaryObject;
		}

		private string GetBinaryData(UserConfiguration userConfiguration)
		{
			string streamPropertyAsBase64String;
			using (Stream stream = UserConfigurationCommandBase<GetUserConfigurationRequest, ServiceUserConfiguration>.GetStream(userConfiguration))
			{
				streamPropertyAsBase64String = this.GetStreamPropertyAsBase64String(stream);
			}
			return streamPropertyAsBase64String;
		}

		private string GetStreamPropertyAsBase64String(Stream stream)
		{
			string result = null;
			if (stream.Length > 0L)
			{
				byte[] array = new byte[stream.Length];
				int num = 0;
				using (BinaryReader binaryReader = new BinaryReader(stream))
				{
					int num3;
					do
					{
						int num2 = Math.Min(4096, array.Length - num);
						num3 = ((num2 > 0) ? binaryReader.Read(array, num, num2) : 0);
						num += num3;
					}
					while (num3 > 0);
				}
				result = Convert.ToBase64String(array, 0, num);
			}
			return result;
		}

		private string GetXmlData(UserConfiguration userConfiguration)
		{
			string streamPropertyAsBase64String;
			using (Stream xmlStream = UserConfigurationCommandBase<GetUserConfigurationRequest, ServiceUserConfiguration>.GetXmlStream(userConfiguration))
			{
				streamPropertyAsBase64String = this.GetStreamPropertyAsBase64String(xmlStream);
			}
			return streamPropertyAsBase64String;
		}

		private UserConfigurationNameType userConfigurationName;

		private UserConfigurationProperties userConfigurationProperties;
	}
}
