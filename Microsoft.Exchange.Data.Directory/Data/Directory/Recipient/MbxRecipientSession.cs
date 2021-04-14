using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal static class MbxRecipientSession
	{
		internal static IStoreUserInformationReader StoreUserInformationReader
		{
			set
			{
				MbxRecipientSession.storeUserInformationReader = value;
			}
		}

		private static ExRpcAdmin GetExRpcAdminInstance()
		{
			if (MbxRecipientSession.storeUserInformationReader != null)
			{
				throw new ArgumentException("ExRpcAdmin should not be used in a context where storeUserInformationReader is initialized.");
			}
			ExRpcAdmin result;
			try
			{
				result = ExRpcAdmin.Create("Client=ADDriver", LocalServerCache.LocalServer.Name, null, null, null);
			}
			catch (MapiPermanentException innerException)
			{
				throw new ADDriverStoreAccessPermanentException(innerException);
			}
			catch (MapiRetryableException innerException2)
			{
				throw new ADDriverStoreAccessTransientException(innerException2);
			}
			return result;
		}

		public static ADRawEntry FindADRawEntryByPuid(ulong puid, Guid mdbGuid, bool readOnly, IEnumerable<MbxPropertyDefinition> properties)
		{
			Guid exchangeGuidFromPuid = ConsumerIdentityHelper.GetExchangeGuidFromPuid(puid);
			ADObjectId adobjectIdFromPuid = ConsumerIdentityHelper.GetADObjectIdFromPuid(puid);
			List<ValidationError> errors = new List<ValidationError>();
			return MbxRecipientSession.ReadUserInformationRecord(adobjectIdFromPuid, mdbGuid, exchangeGuidFromPuid, readOnly, properties, errors);
		}

		private static PropTag[] PropTagsFromProperties(IEnumerable<MbxPropertyDefinition> properties)
		{
			Dictionary<MbxPropertyDefinition, PropTag> dictionary = new Dictionary<MbxPropertyDefinition, PropTag>();
			if (properties != null)
			{
				foreach (MbxPropertyDefinition mbxPropertyDefinition in properties)
				{
					if (mbxPropertyDefinition.IsCalculated)
					{
						using (ReadOnlyCollection<ProviderPropertyDefinition>.Enumerator enumerator2 = mbxPropertyDefinition.SupportingProperties.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								ProviderPropertyDefinition providerPropertyDefinition = enumerator2.Current;
								MbxPropertyDefinition mbxPropertyDefinition2 = (MbxPropertyDefinition)providerPropertyDefinition;
								dictionary[mbxPropertyDefinition2] = mbxPropertyDefinition2.PropTag;
							}
							continue;
						}
					}
					dictionary[mbxPropertyDefinition] = mbxPropertyDefinition.PropTag;
				}
			}
			return dictionary.Values.ToArray<PropTag>();
		}

		private static PropValue[] PropValuesFromADPropertyBag(ADPropertyBag properties)
		{
			List<PropValue> list = new List<PropValue>();
			foreach (object obj in properties.Keys)
			{
				SimpleProviderPropertyDefinition simpleProviderPropertyDefinition = (SimpleProviderPropertyDefinition)obj;
				MbxPropertyDefinition mbxPropertyDefinition = simpleProviderPropertyDefinition as MbxPropertyDefinition;
				if (properties.IsChanged(simpleProviderPropertyDefinition) && mbxPropertyDefinition != null && mbxPropertyDefinition.PropTag != PropTag.Null)
				{
					object value = SimpleStoreValueConverter.ConvertValueToStore(properties[mbxPropertyDefinition]);
					list.Add(new PropValue(mbxPropertyDefinition.PropTag, value));
				}
			}
			return list.ToArray();
		}

		public static ADRawEntry ReadUserInformationRecord(ADObjectId id, Guid mdbGuid, Guid mbxGuid, bool createReadOnly, IEnumerable<MbxPropertyDefinition> properties, List<ValidationError> errors)
		{
			PropTag[] array = MbxRecipientSession.PropTagsFromProperties(properties);
			PropValue[] array4;
			try
			{
				if (MbxRecipientSession.storeUserInformationReader != null)
				{
					uint[] array2 = array.Cast<uint>().ToArray<uint>();
					object[] array3 = MbxRecipientSession.storeUserInformationReader.ReadUserInformation(mdbGuid, mbxGuid, array2);
					if (array3.Length != array2.Length)
					{
						throw new ArgumentException("Number of returned property values doesn't match number of requested properties");
					}
					array4 = new PropValue[array2.Length];
					for (int i = 0; i < array4.Length; i++)
					{
						if (array3[i] != null)
						{
							array4[i] = new PropValue((PropTag)array2[i], array3[i]);
						}
						else
						{
							array4[i] = new PropValue(PropTagHelper.ConvertToError((PropTag)array2[i]), -2147221233);
						}
					}
				}
				else
				{
					using (ExRpcAdmin exRpcAdminInstance = MbxRecipientSession.GetExRpcAdminInstance())
					{
						array4 = exRpcAdminInstance.ReadUserInfo(mdbGuid, mbxGuid, 0U, array);
					}
				}
			}
			catch (MapiPermanentException innerException)
			{
				throw new ADDriverStoreAccessPermanentException(innerException);
			}
			catch (MapiRetryableException innerException2)
			{
				throw new ADDriverStoreAccessTransientException(innerException2);
			}
			return MbxRecipientSession.ADRawEntryFromPropValues(id, createReadOnly, array4, properties, errors);
		}

		private static ADRawEntry ADRawEntryFromPropValues(ADObjectId id, bool createReadOnly, PropValue[] propValues, IEnumerable<MbxPropertyDefinition> properties, List<ValidationError> errors)
		{
			string fqdn = LocalServerCache.LocalServer.Fqdn;
			ADPropertyBag adpropertyBag = new ADPropertyBag(createReadOnly, 16);
			adpropertyBag.SetField(ADObjectSchema.Id, id);
			foreach (PropValue propValue in propValues)
			{
				if (!propValue.IsError())
				{
					MbxPropertyDefinition mbxPropertyDefinition = ObjectSchema.GetInstance<MbxRecipientSchema>().FindPropertyDefinitionByPropTag(propValue.PropTag);
					PropertyValidationError propertyValidationError = mbxPropertyDefinition.ValidateValue(propValue.Value, true);
					if (propertyValidationError != null)
					{
						errors.Add(propertyValidationError);
					}
					adpropertyBag.SetField(mbxPropertyDefinition, SimpleStoreValueConverter.ConvertValueFromStore(mbxPropertyDefinition, propValue.Value));
				}
			}
			MbxRecipientSession.PopulateCustomizedCalculatedProperties(adpropertyBag, properties);
			ADRawEntry adrawEntry = new ADRawEntry(adpropertyBag);
			adrawEntry.OriginatingServer = fqdn;
			adrawEntry.WhenReadUTC = new DateTime?(DateTime.UtcNow);
			adrawEntry.ResetChangeTracking(true);
			return adrawEntry;
		}

		private static void PopulateCustomizedCalculatedProperties(ADPropertyBag propertyBag, IEnumerable<PropertyDefinition> propertiesToPopulate)
		{
			foreach (PropertyDefinition propertyDefinition in propertiesToPopulate)
			{
				ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)propertyDefinition;
				if (providerPropertyDefinition.IsCalculated)
				{
					ReadOnlyCollection<ProviderPropertyDefinition> supportingProperties = providerPropertyDefinition.SupportingProperties;
					if (supportingProperties.SingleOrDefault((ProviderPropertyDefinition supportingProperty) => !propertyBag.Contains(supportingProperty)) == null)
					{
						object obj = propertyBag[providerPropertyDefinition];
					}
				}
			}
		}

		public static void CreateUserInformationRecord(Guid mdbGuid, Guid mbxGuid, ADPropertyBag properties)
		{
			PropValue[] properties2 = MbxRecipientSession.PropValuesFromADPropertyBag(properties);
			try
			{
				using (ExRpcAdmin exRpcAdminInstance = MbxRecipientSession.GetExRpcAdminInstance())
				{
					exRpcAdminInstance.CreateUserInfo(mdbGuid, mbxGuid, 0U, properties2);
				}
			}
			catch (MapiExceptionUserInformationAlreadyExists mapiExceptionUserInformationAlreadyExists)
			{
				throw new ADObjectAlreadyExistsException(new LocalizedString(mapiExceptionUserInformationAlreadyExists.Message), mapiExceptionUserInformationAlreadyExists);
			}
			catch (MapiPermanentException innerException)
			{
				throw new ADDriverStoreAccessPermanentException(innerException);
			}
			catch (MapiRetryableException innerException2)
			{
				throw new ADDriverStoreAccessTransientException(innerException2);
			}
		}

		public static void UpdateUserInformationRecord(Guid mdbGuid, Guid mbxGuid, ADPropertyBag propertiesToUpdate, IEnumerable<MbxPropertyDefinition> propertiesToRemove)
		{
			PropValue[] properties = MbxRecipientSession.PropValuesFromADPropertyBag(propertiesToUpdate);
			PropTag[] deletePropTag = MbxRecipientSession.PropTagsFromProperties(propertiesToRemove);
			try
			{
				using (ExRpcAdmin exRpcAdminInstance = MbxRecipientSession.GetExRpcAdminInstance())
				{
					exRpcAdminInstance.UpdateUserInfo(mdbGuid, mbxGuid, 0U, properties, deletePropTag);
				}
			}
			catch (MapiPermanentException innerException)
			{
				throw new ADDriverStoreAccessPermanentException(innerException);
			}
			catch (MapiRetryableException innerException2)
			{
				throw new ADDriverStoreAccessTransientException(innerException2);
			}
		}

		public static void RemoveUserInformationRecord(Guid mdbGuid, Guid mbxGuid)
		{
			try
			{
				using (ExRpcAdmin exRpcAdminInstance = MbxRecipientSession.GetExRpcAdminInstance())
				{
					exRpcAdminInstance.DeleteUserInfo(mdbGuid, mbxGuid, 0U);
				}
			}
			catch (MapiExceptionNoSupport innerException)
			{
				throw new RemovalAPINotSupportedException(innerException);
			}
			catch (MapiPermanentException innerException2)
			{
				throw new ADDriverStoreAccessPermanentException(innerException2);
			}
			catch (MapiRetryableException innerException3)
			{
				throw new ADDriverStoreAccessTransientException(innerException3);
			}
		}

		private static IStoreUserInformationReader storeUserInformationReader;
	}
}
