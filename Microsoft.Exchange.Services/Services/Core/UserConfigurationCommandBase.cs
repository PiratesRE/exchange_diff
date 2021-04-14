using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class UserConfigurationCommandBase<RequestType, SingleItemType> : SingleStepServiceCommand<RequestType, SingleItemType> where RequestType : BaseRequest
	{
		internal UserConfigurationCommandBase(CallContext callContext, RequestType request) : base(callContext, request)
		{
		}

		protected static UserConfiguration Get(UserConfigurationCommandBase<RequestType, SingleItemType>.UserConfigurationName userConfigurationName)
		{
			return userConfigurationName.MailboxSession.UserConfigurationManager.GetFolderConfiguration(userConfigurationName.Name, UserConfigurationTypes.Stream | UserConfigurationTypes.XML | UserConfigurationTypes.Dictionary, userConfigurationName.FolderId);
		}

		protected static void ValidatePropertiesForUpdate(ServiceUserConfiguration serviceUserConfiguration)
		{
			if (serviceUserConfiguration.ItemId != null)
			{
				throw new ServiceInvalidOperationException((CoreResources.IDs)2503843052U);
			}
		}

		protected static void SetProperties(ServiceUserConfiguration serviceUserConfiguration, UserConfiguration userConfiguration)
		{
			UserConfigurationCommandBase<RequestType, SingleItemType>.SetDictionary(serviceUserConfiguration, userConfiguration);
			UserConfigurationCommandBase<RequestType, SingleItemType>.SetXmlStream(serviceUserConfiguration, userConfiguration);
			UserConfigurationCommandBase<RequestType, SingleItemType>.SetStream(serviceUserConfiguration, userConfiguration);
		}

		protected static void UpdateProperties(ServiceUserConfiguration serviceUserConfiguration, UserConfiguration userConfiguration)
		{
			UserConfigurationCommandBase<RequestType, SingleItemType>.UpdateDictionary(serviceUserConfiguration, userConfiguration);
			UserConfigurationCommandBase<RequestType, SingleItemType>.SetXmlStream(serviceUserConfiguration, userConfiguration);
			UserConfigurationCommandBase<RequestType, SingleItemType>.SetStream(serviceUserConfiguration, userConfiguration);
		}

		protected static ConfigurationDictionary GetDictionary(UserConfiguration userConfiguration)
		{
			if (!UserConfigurationCommandBase<RequestType, SingleItemType>.PropertyExists(UserConfigurationTypes.Dictionary, userConfiguration.DataTypes))
			{
				throw new ObjectSavePropertyErrorException((CoreResources.IDs)2214456911U, null);
			}
			return (ConfigurationDictionary)userConfiguration.GetDictionary();
		}

		protected static Stream GetXmlStream(UserConfiguration userConfiguration)
		{
			if (!UserConfigurationCommandBase<RequestType, SingleItemType>.PropertyExists(UserConfigurationTypes.XML, userConfiguration.DataTypes))
			{
				throw new ObjectSavePropertyErrorException((CoreResources.IDs)2419720676U, null);
			}
			return userConfiguration.GetXmlStream();
		}

		protected static Stream GetStream(UserConfiguration userConfiguration)
		{
			if (!UserConfigurationCommandBase<RequestType, SingleItemType>.PropertyExists(UserConfigurationTypes.Stream, userConfiguration.DataTypes))
			{
				throw new ObjectSavePropertyErrorException(CoreResources.IDs.ErrorUserConfigurationBinaryDataNotExist, null);
			}
			return userConfiguration.GetStream();
		}

		protected static bool PropertyExists(UserConfigurationTypes userConfigurationPropertyToCheck, UserConfigurationTypes userConfigurationProperties)
		{
			return (userConfigurationPropertyToCheck & userConfigurationProperties) != (UserConfigurationTypes)0;
		}

		protected UserConfigurationCommandBase<RequestType, SingleItemType>.UserConfigurationName GetUserConfigurationName(UserConfigurationNameType userConfigurationName)
		{
			IdAndSession idAndSession = base.IdConverter.ConvertTargetFolderIdToIdAndContentSession(userConfigurationName.BaseFolderId, true);
			if (idAndSession.Session is PublicFolderSession)
			{
				throw new InvalidValueForPropertyException((CoreResources.IDs)3014743008U, null);
			}
			return new UserConfigurationCommandBase<RequestType, SingleItemType>.UserConfigurationName(userConfigurationName.Name, idAndSession.Id, (MailboxSession)idAndSession.Session);
		}

		private static void SetDictionary(ServiceUserConfiguration serviceUserConfiguration, UserConfiguration userConfiguration)
		{
			if (serviceUserConfiguration.Dictionary == null)
			{
				return;
			}
			ConfigurationDictionary dictionary = UserConfigurationCommandBase<RequestType, SingleItemType>.GetDictionary(userConfiguration);
			dictionary.Clear();
			IDictionary dictionary2 = dictionary;
			foreach (UserConfigurationDictionaryEntry userConfigurationDictionaryEntry in serviceUserConfiguration.Dictionary)
			{
				object key = UserConfigurationCommandBase<RequestType, SingleItemType>.ConstructDictionaryKey(userConfigurationDictionaryEntry.DictionaryKey, dictionary2);
				object value = UserConfigurationCommandBase<RequestType, SingleItemType>.ConstructDictionaryValue(userConfigurationDictionaryEntry.DictionaryValue);
				try
				{
					dictionary2.Add(key, value);
				}
				catch (NotSupportedException innerException)
				{
					throw new ServiceInvalidOperationException((CoreResources.IDs)2517173182U, innerException);
				}
			}
		}

		private static void UpdateDictionary(ServiceUserConfiguration serviceUserConfiguration, UserConfiguration userConfiguration)
		{
			if (serviceUserConfiguration.Dictionary == null || serviceUserConfiguration.Dictionary.Length == 0)
			{
				return;
			}
			ConfigurationDictionary dictionary = UserConfigurationCommandBase<RequestType, SingleItemType>.GetDictionary(userConfiguration);
			IDictionary dictionary2 = dictionary;
			foreach (UserConfigurationDictionaryEntry userConfigurationDictionaryEntry in serviceUserConfiguration.Dictionary)
			{
				object key = UserConfigurationCommandBase<RequestType, SingleItemType>.ConstructDictionaryObject(userConfigurationDictionaryEntry.DictionaryKey);
				object value = UserConfigurationCommandBase<RequestType, SingleItemType>.ConstructDictionaryValue(userConfigurationDictionaryEntry.DictionaryValue);
				try
				{
					if (dictionary2.Contains(key))
					{
						dictionary2[key] = value;
					}
					else
					{
						dictionary2.Add(key, value);
					}
				}
				catch (NotSupportedException innerException)
				{
					throw new ServiceInvalidOperationException((CoreResources.IDs)2517173182U, innerException);
				}
			}
		}

		private static object ConstructDictionaryKey(UserConfigurationDictionaryObject userConfigurationDictionaryKey, IDictionary configurationIDictionary)
		{
			object obj = UserConfigurationCommandBase<RequestType, SingleItemType>.ConstructDictionaryObject(userConfigurationDictionaryKey);
			if (configurationIDictionary.Contains(obj))
			{
				string dictionaryKey;
				if (userConfigurationDictionaryKey.Value.Length > 1)
				{
					dictionaryKey = userConfigurationDictionaryKey.Value[0];
				}
				else
				{
					dictionaryKey = string.Join(" ", userConfigurationDictionaryKey.Value);
				}
				throw InvalidValueForPropertyException.CreateDuplicateDictionaryKeyError((CoreResources.IDs)2578390262U, dictionaryKey, null);
			}
			return obj;
		}

		private static object ConstructDictionaryValue(UserConfigurationDictionaryObject userConfigurationDictionaryValue)
		{
			object result;
			if (userConfigurationDictionaryValue == null)
			{
				result = null;
			}
			else
			{
				result = UserConfigurationCommandBase<RequestType, SingleItemType>.ConstructDictionaryObject(userConfigurationDictionaryValue);
			}
			return result;
		}

		private static object ConstructDictionaryObject(UserConfigurationDictionaryObject userConfigurationDictionaryObject)
		{
			object result = null;
			if (userConfigurationDictionaryObject.Value == null || userConfigurationDictionaryObject.Value.Length == 0)
			{
				throw new InvalidValueForPropertyException(CoreResources.IDs.ErrorInvalidValueForProperty);
			}
			if (userConfigurationDictionaryObject.Type != UserConfigurationDictionaryObjectType.StringArray && userConfigurationDictionaryObject.Value.Length > 1)
			{
				throw new InvalidValueForPropertyException(CoreResources.IDs.ErrorInvalidValueForPropertyStringArrayDictionaryKey);
			}
			try
			{
				switch (userConfigurationDictionaryObject.Type)
				{
				case UserConfigurationDictionaryObjectType.DateTime:
					result = ExDateTimeConverter.Parse(userConfigurationDictionaryObject.Value[0]);
					break;
				case UserConfigurationDictionaryObjectType.Boolean:
					result = BooleanConverter.Parse(userConfigurationDictionaryObject.Value[0]);
					break;
				case UserConfigurationDictionaryObjectType.Byte:
					result = ByteConverter.Parse(userConfigurationDictionaryObject.Value[0]);
					break;
				case UserConfigurationDictionaryObjectType.String:
					result = userConfigurationDictionaryObject.Value[0];
					break;
				case UserConfigurationDictionaryObjectType.Integer32:
					result = IntConverter.Parse(userConfigurationDictionaryObject.Value[0]);
					break;
				case UserConfigurationDictionaryObjectType.UnsignedInteger32:
					result = UIntConverter.Parse(userConfigurationDictionaryObject.Value[0]);
					break;
				case UserConfigurationDictionaryObjectType.Integer64:
					result = LongConverter.Parse(userConfigurationDictionaryObject.Value[0]);
					break;
				case UserConfigurationDictionaryObjectType.UnsignedInteger64:
					result = ULongConverter.Parse(userConfigurationDictionaryObject.Value[0]);
					break;
				case UserConfigurationDictionaryObjectType.StringArray:
					result = userConfigurationDictionaryObject.Value;
					break;
				case UserConfigurationDictionaryObjectType.ByteArray:
					result = Base64StringConverter.Parse(userConfigurationDictionaryObject.Value[0]);
					break;
				}
			}
			catch (FormatException exception)
			{
				UserConfigurationCommandBase<RequestType, SingleItemType>.ThrowInvalidValueException(userConfigurationDictionaryObject, exception);
			}
			catch (OverflowException exception2)
			{
				UserConfigurationCommandBase<RequestType, SingleItemType>.ThrowInvalidValueException(userConfigurationDictionaryObject, exception2);
			}
			catch (ArgumentNullException exception3)
			{
				UserConfigurationCommandBase<RequestType, SingleItemType>.ThrowInvalidValueException(userConfigurationDictionaryObject, exception3);
			}
			return result;
		}

		private static void ThrowInvalidValueException(UserConfigurationDictionaryObject userConfigurationDictionaryObject, Exception exception)
		{
			ExTraceGlobals.ExceptionTracer.TraceError<string, UserConfigurationDictionaryObjectType, Exception>(0L, "Can't construct dictionary object with value: {0} type: {1} exception: {2}", userConfigurationDictionaryObject.Value[0], userConfigurationDictionaryObject.Type, exception);
			throw InvalidValueForPropertyException.CreateConversionError(CoreResources.IDs.ErrorInvalidValueForPropertyKeyValueConversion, userConfigurationDictionaryObject.Value[0], userConfigurationDictionaryObject.Type.ToString(), exception);
		}

		private static void SetXmlStream(ServiceUserConfiguration serviceUserConfiguration, UserConfiguration userConfiguration)
		{
			if (serviceUserConfiguration.XmlData == null)
			{
				return;
			}
			using (Stream xmlStream = UserConfigurationCommandBase<RequestType, SingleItemType>.GetXmlStream(userConfiguration))
			{
				UserConfigurationCommandBase<RequestType, SingleItemType>.SetStreamPropertyFromBase64String(serviceUserConfiguration.XmlData, xmlStream, (CoreResources.IDs)2643780243U);
			}
		}

		private static void SetStream(ServiceUserConfiguration serviceUserConfiguration, UserConfiguration userConfiguration)
		{
			if (serviceUserConfiguration.BinaryData == null)
			{
				return;
			}
			using (Stream stream = UserConfigurationCommandBase<RequestType, SingleItemType>.GetStream(userConfiguration))
			{
				UserConfigurationCommandBase<RequestType, SingleItemType>.SetStreamPropertyFromBase64String(serviceUserConfiguration.BinaryData, stream, CoreResources.IDs.ErrorInvalidValueForPropertyBinaryData);
			}
		}

		private static void SetStreamPropertyFromBase64String(string propertyAsBase64String, Stream stream, Enum invalidPropertyValueError)
		{
			byte[] array;
			try
			{
				array = Base64StringConverter.Parse(propertyAsBase64String);
			}
			catch (FormatException ex)
			{
				ExTraceGlobals.ExceptionTracer.TraceError<string, FormatException>(0L, "Can't parse base64 string {0} Exception: {1}", propertyAsBase64String, ex);
				throw new InvalidValueForPropertyException(invalidPropertyValueError, ex);
			}
			stream.SetLength(0L);
			stream.Write(array, 0, array.Length);
		}

		protected class UserConfigurationName
		{
			internal UserConfigurationName(string name, StoreId folderId, MailboxSession mailboxSession)
			{
				if (!UserConfigurationManager.IsValidName(name))
				{
					throw new InvalidValueForPropertyException((CoreResources.IDs)2744667914U);
				}
				this.name = name;
				this.folderId = folderId;
				this.mailboxSession = mailboxSession;
			}

			internal string Name
			{
				get
				{
					return this.name;
				}
			}

			internal StoreId FolderId
			{
				get
				{
					return this.folderId;
				}
			}

			internal MailboxSession MailboxSession
			{
				get
				{
					return this.mailboxSession;
				}
			}

			private string name;

			private StoreId folderId;

			private MailboxSession mailboxSession;
		}
	}
}
