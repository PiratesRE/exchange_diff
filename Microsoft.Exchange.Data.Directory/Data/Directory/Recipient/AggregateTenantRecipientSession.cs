using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class AggregateTenantRecipientSession : DirectorySessionBase, ITenantRecipientSession, IRecipientSession, IDirectorySession, IConfigDataProvider, IAggregateSession
	{
		internal AggregateTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(false, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
			base.DomainController = domainController;
			base.SearchRoot = searchRoot;
			base.Lcid = lcid;
			base.UseGlobalCatalog = base.ReadOnly;
			if (AggregationHelper.IsMailboxRole)
			{
				this.directoryBackendType = (DirectoryBackendType.MServ | DirectoryBackendType.Mbx);
				((IAggregateSession)this).MbxReadMode = MbxReadMode.OnlyIfLocatorDataAvailable;
				if (!readOnly)
				{
					this.backendWriteMode = BackendWriteMode.WriteToMServ;
				}
			}
			else
			{
				this.directoryBackendType = DirectoryBackendType.MServ;
				((IAggregateSession)this).MbxReadMode = MbxReadMode.NoMbxRead;
				this.backendWriteMode = BackendWriteMode.NoWrites;
			}
			if (!DatacenterRegistry.IsForefrontForOffice() || DatacenterRegistry.IsForefrontForOfficeDeployment())
			{
				this.directoryBackendType |= DirectoryBackendType.AD;
			}
		}

		internal AggregateTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope) : this(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
			base.ConfigScope = configScope;
		}

		private ApiNotSupportedException HandleUnsupportedApi(string toBeAddressedIn = null, [CallerMemberName] string memberName = "<unknown>")
		{
			LocalizedString localizedString = DirectoryStrings.ApiNotSupportedError(AggregateTenantRecipientSession.className, memberName);
			string stackTraceLine = base.GetStackTraceLine(4);
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_ApiNotSupported, stackTraceLine, new object[]
			{
				localizedString,
				stackTraceLine
			});
			return new ApiNotSupportedException(localizedString);
		}

		private ApiInputNotSupportedException HandleUnsupportedInput(object input = null, [CallerMemberName] string memberName = "<unknown>")
		{
			LocalizedString localizedString = DirectoryStrings.ApiDoesNotSupportInputFormatError(AggregateTenantRecipientSession.className, memberName, (input != null) ? input.ToString() : "<null>");
			string stackTraceLine = base.GetStackTraceLine(4);
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_ApiInputNotSupported, stackTraceLine, new object[]
			{
				localizedString,
				stackTraceLine
			});
			return new ApiInputNotSupportedException(localizedString);
		}

		private ADRawEntry LoadADRawEntryByPuid(ulong puid, IEnumerable<PropertyDefinition> properties)
		{
			return this.LoadADRawEntryByPuidOrMemberName(puid, SmtpAddress.Empty, properties);
		}

		private TResult LoadGenericRecipientByPuid<TResult>(ulong puid, IEnumerable<PropertyDefinition> properties = null) where TResult : ADObject, new()
		{
			return this.LoadGenericRecipientObjectByPuidOrMemberName<TResult>(puid, SmtpAddress.Empty, properties);
		}

		private ADRawEntry LoadADRawEntryByMemberName(SmtpAddress address, IEnumerable<PropertyDefinition> properties)
		{
			return this.LoadADRawEntryByPuidOrMemberName(0UL, address, properties);
		}

		private TResult LoadGenericRecipientByMemberName<TResult>(SmtpAddress address, IEnumerable<PropertyDefinition> properties = null) where TResult : ADObject, new()
		{
			return this.LoadGenericRecipientObjectByPuidOrMemberName<TResult>(0UL, address, properties);
		}

		private TResult LoadGenericRecipientObjectByPuidOrMemberName<TResult>(ulong puid, SmtpAddress address, IEnumerable<PropertyDefinition> properties) where TResult : ADObject, new()
		{
			return this.LoadEntryByPuidOrMemberName<TResult>(puid, address, properties, Activator.CreateInstance<TResult>());
		}

		private ADRawEntry LoadADRawEntryByPuidOrMemberName(ulong puid, SmtpAddress address, IEnumerable<PropertyDefinition> properties)
		{
			return this.LoadEntryByPuidOrMemberName<ADRawEntry>(puid, address, properties, null);
		}

		private TResult LoadEntryByPuidOrMemberName<TResult>(ulong puid, SmtpAddress address, IEnumerable<PropertyDefinition> props, ADObject dummyInstance) where TResult : ADRawEntry, new()
		{
			List<PropertyDefinition> list = (props != null) ? new List<PropertyDefinition>(props) : new List<PropertyDefinition>();
			if (dummyInstance != null)
			{
				list.AddRange(dummyInstance.Schema.AllProperties);
			}
			bool flag = dummyInstance == null;
			bool flag2 = dummyInstance is MiniObject;
			bool readOnly = this.readOnly || flag || flag2;
			List<ADPropertyDefinition> list2;
			List<MServPropertyDefinition> list3;
			List<MbxPropertyDefinition> list4;
			AggregationHelper.FilterPropertyDefinitionsByBackendSource(list, ((IAggregateSession)this).MbxReadMode, out list2, out list3, out list4);
			DirectoryBackendType directoryBackendType = DirectoryBackendType.None;
			if (list4.Count > 0 || puid == 0UL)
			{
				list3.Add(MServRecipientSchema.Database);
			}
			ADRawEntry adrawEntry = null;
			if (list3.Count > 0)
			{
				directoryBackendType |= DirectoryBackendType.MServ;
				if (!base.IsDirectoryBackendMServ)
				{
					throw new ArgumentException("Wrong lookup type - this directory session does not support MServ backend");
				}
				if (address != SmtpAddress.Empty)
				{
					adrawEntry = AggregationHelper.PerformMservLookupByMemberName(address, this.readOnly, list3);
				}
				else
				{
					if (puid == 0UL)
					{
						return default(TResult);
					}
					adrawEntry = AggregationHelper.PerformMservLookupByPuid(puid, this.readOnly, list3);
				}
				if (adrawEntry == null)
				{
					return default(TResult);
				}
			}
			ADObjectId adobjectId = (adrawEntry != null) ? adrawEntry.Id : ConsumerIdentityHelper.GetADObjectIdFromPuid(puid);
			Guid mdbGuid = Guid.Empty;
			ADObjectId adobjectId2 = (ADObjectId)adrawEntry[MServRecipientSchema.Database];
			bool flag3 = list4.Count > 0 && adobjectId2 != null && adobjectId2.PartitionFQDN != null && ((IAggregateSession)this).MbxReadMode != MbxReadMode.NoMbxRead;
			if (list4.Count > 0 && (adobjectId2 == null || adobjectId2.PartitionFQDN == null) && ((IAggregateSession)this).MbxReadMode == MbxReadMode.Always)
			{
				throw new NoLocatorInformationInMServException();
			}
			if (flag3)
			{
				if (new PartitionId(adobjectId2.PartitionFQDN) != PartitionId.LocalForest)
				{
					throw new NotLocalMaiboxException();
				}
				mdbGuid = adobjectId2.ObjectGuid;
			}
			ADRawEntry adResult = null;
			if (list2.Count > 0)
			{
				directoryBackendType |= DirectoryBackendType.AD;
				if (!base.IsDirectoryBackendAD)
				{
					throw new ArgumentException("Wrong lookup type - this directory session does not support AD backend");
				}
				adResult = AggregationHelper.PerformADLookup(adobjectId, list2);
			}
			ADRawEntry adrawEntry2 = null;
			if (flag3)
			{
				if (!base.IsDirectoryBackendMbx)
				{
					throw new ArgumentException("Wrong lookup type - this directory session does not support Mbx backend, is it running on FE role?");
				}
				try
				{
					adrawEntry2 = AggregationHelper.PerformMbxLookupByPuid(adobjectId, mdbGuid, this.readOnly, list4);
					directoryBackendType |= DirectoryBackendType.Mbx;
				}
				catch (ADDriverStoreAccessPermanentException ex)
				{
					if (this.mbxReadMode != MbxReadMode.OnlyIfLocatorDataAvailable || ex.InnerException == null || !(ex.InnerException is MapiExceptionUserInformationNotFound))
					{
						throw;
					}
				}
			}
			ADPropertyBag adpropertyBag = new ADPropertyBag(readOnly, list.Count<PropertyDefinition>());
			adpropertyBag.SetField(ADObjectSchema.Id, adrawEntry[ADObjectSchema.Id]);
			foreach (PropertyDefinition propertyDefinition in list)
			{
				this.CopyPropertyToResultingPropertyBag((ADPropertyDefinition)propertyDefinition, adpropertyBag, adrawEntry, adrawEntry2, adResult);
			}
			TResult result;
			if (flag)
			{
				result = (TResult)((object)base.CreateAndInitializeADRawEntry(adpropertyBag));
			}
			else
			{
				result = (TResult)((object)this.CreateAndInitializeObject<TResult>(adpropertyBag, dummyInstance));
			}
			if (!this.readOnly)
			{
				result.MservPropertyBag = ((adrawEntry != null) ? ((ADPropertyBag)adrawEntry.propertyBag) : new ADPropertyBag());
				result.MbxPropertyBag = ((adrawEntry2 != null) ? ((ADPropertyBag)adrawEntry2.propertyBag) : new ADPropertyBag());
			}
			result.SetId(adrawEntry.Id);
			result.OriginatingServer = TemplateTenantConfiguration.GetLocalTemplateTenant().OriginatingServer;
			result.DirectoryBackendType = directoryBackendType;
			result.WhenReadUTC = new DateTime?(DateTime.UtcNow);
			ValidationError[] array = result.ValidateRead();
			if (array.Length > 0)
			{
				foreach (ValidationError validationError in array)
				{
					PropertyValidationError propertyValidationError = validationError as PropertyValidationError;
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_VALIDATION_FAILED_ATTRIBUTE, puid.ToString(), new object[]
					{
						puid.ToString(),
						"<AggregateTenantRecipientSession>",
						(propertyValidationError != null) ? propertyValidationError.PropertyDefinition.Name : string.Empty,
						validationError.Description,
						(propertyValidationError != null) ? (propertyValidationError.InvalidData ?? string.Empty) : string.Empty
					});
				}
				if (base.ConsistencyMode == ConsistencyMode.IgnoreInvalid)
				{
					return default(TResult);
				}
			}
			result.ResetChangeTracking(true);
			return result;
		}

		protected override ADObject CreateAndInitializeObject<TResult>(ADPropertyBag propertyBag, ADRawEntry dummyInstance)
		{
			return ADObjectFactory.CreateAndInitializeRecipientObject<TResult>(propertyBag, dummyInstance, this);
		}

		private void CopyPropertyToResultingPropertyBag(ADPropertyDefinition adProp, ADPropertyBag resultingPropertyBag, ADRawEntry mservResult, ADRawEntry mbxResult, ADRawEntry adResult)
		{
			if (adProp.MbxPropertyDefinition != null && mbxResult != null && mbxResult.propertyBag.Contains(adProp.MbxPropertyDefinition))
			{
				resultingPropertyBag.SetField(adProp, mbxResult[adProp.MbxPropertyDefinition]);
			}
			else if (adProp.MServPropertyDefinition != null)
			{
				resultingPropertyBag.SetField(adProp, mservResult[adProp.MServPropertyDefinition]);
			}
			else if (!adProp.IsCalculated)
			{
				resultingPropertyBag.SetField(adProp, adResult[adProp]);
			}
			else
			{
				foreach (ProviderPropertyDefinition providerPropertyDefinition in adProp.SupportingProperties)
				{
					ADPropertyDefinition adProp2 = (ADPropertyDefinition)providerPropertyDefinition;
					this.CopyPropertyToResultingPropertyBag(adProp2, resultingPropertyBag, mservResult, mbxResult, adResult);
				}
			}
			if (adProp == ADRecipientSchema.EmailAddresses)
			{
				ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)resultingPropertyBag[adProp];
				proxyAddressCollection.CopyChangesOnly = true;
			}
		}

		private ulong GetPuidByLegacyExchangeDN(string legacyExchangeDN, bool throwIfError = true)
		{
			ulong result;
			if (!ConsumerIdentityHelper.TryGetPuidFromLegacyExchangeDN(legacyExchangeDN, out result) && throwIfError)
			{
				Exception ex = this.HandleUnsupportedInput(legacyExchangeDN, "GetPuidByLegacyExchangeDN");
				if (!ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("TolerateInvalidInputInAggregateSession"))
				{
					throw ex;
				}
			}
			return result;
		}

		private ulong GetPuidByADObjectId(ADObjectId adObjectId, bool throwIfError = true)
		{
			ulong result;
			if (!ConsumerIdentityHelper.TryGetPuidFromADObjectId(adObjectId, out result) && throwIfError)
			{
				Exception ex = this.HandleUnsupportedInput(adObjectId, "GetPuidByADObjectId");
				if (!ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("TolerateInvalidInputInAggregateSession"))
				{
					throw ex;
				}
			}
			return result;
		}

		private ulong GetPuidByGuid(Guid exchangeGuid, bool throwIfError = true)
		{
			ulong result;
			if (!ConsumerIdentityHelper.TryGetPuidFromGuid(exchangeGuid, out result) && throwIfError)
			{
				Exception ex = this.HandleUnsupportedInput(exchangeGuid, "GetPuidByGuid");
				if (!ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("TolerateInvalidInputInAggregateSession"))
				{
					throw ex;
				}
			}
			return result;
		}

		private ulong GetPuidBySecurityIdentifier(SecurityIdentifier sid, bool throwIfError = true)
		{
			ulong result;
			if (!ConsumerIdentityHelper.TryGetPuidFromSecurityIdentifier(sid, out result) && throwIfError)
			{
				Exception ex = this.HandleUnsupportedInput(sid, "GetPuidBySecurityIdentifier");
				if (!ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("TolerateInvalidInputInAggregateSession"))
				{
					throw ex;
				}
			}
			return result;
		}

		private ulong GetPuidByExternalDirectoryObjectId(string externalDirectoryObjectId, bool throwIfError = true)
		{
			ulong result;
			if (!ConsumerIdentityHelper.TryGetPuidByExternalDirectoryObjectId(externalDirectoryObjectId, out result) && throwIfError)
			{
				Exception ex = this.HandleUnsupportedInput(externalDirectoryObjectId, "GetPuidByExternalDirectoryObjectId");
				if (!ConfigBase<AdDriverConfigSchema>.GetConfig<bool>("TolerateInvalidInputInAggregateSession"))
				{
					throw ex;
				}
			}
			return result;
		}

		private SmtpAddress GetSmtpAddressByProxyAddress(ProxyAddress proxyAddress, bool throwIfError = true)
		{
			return new SmtpAddress(proxyAddress.ValueString);
		}

		private Result<TData>[] ReadMultiple<TKey, TIdentity, TData>(TKey[] keys, AggregateTenantRecipientSession.KeyConverter<TKey, TIdentity> keyConverter, AggregateTenantRecipientSession.RawDataRetriever<TIdentity, TData> dataRetriever, IEnumerable<PropertyDefinition> properties = null) where TData : class
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			if (keys.Rank > 1)
			{
				throw new ArgumentOutOfRangeException("keys", DirectoryStrings.ExArgumentOutOfRangeException("keys.Rank", keys.Rank));
			}
			if (keys.Length == 0)
			{
				return new Result<TData>[0];
			}
			if (keyConverter == null)
			{
				throw new ArgumentNullException("keyConverter");
			}
			if (dataRetriever == null)
			{
				throw new ArgumentException("dataRetriever cannot be null");
			}
			TIdentity[] array = (from k in keys
			select keyConverter(k, true)).ToArray<TIdentity>();
			Result<TData>[] array2 = new Result<TData>[keys.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				TData tdata = dataRetriever(array[i], properties);
				array2[i] = ((tdata != null) ? new Result<TData>(tdata, null) : new Result<TData>(default(TData), ProviderError.NotFound));
			}
			return array2;
		}

		private void CreateOrUpdateEntry(ADRecipient instanceToSave)
		{
			IList<PropertyDefinition> allProperties = ADRecipientProperties.Instance.AllProperties;
			bool flag = false;
			bool flag2 = false;
			ExTraceGlobals.ADSaveTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "AggregateTenantRecipientSession::CreateOrUpdateEntry - updating or creating entry {0}", instanceToSave.Id);
			foreach (PropertyDefinition propertyDefinition in allProperties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (!adpropertyDefinition.IsCalculated && (instanceToSave.propertyBag.IsChanged(adpropertyDefinition) || adpropertyDefinition.IsMultivalued) && (instanceToSave.ObjectState != ObjectState.New || !adpropertyDefinition.PersistDefaultValue || !adpropertyDefinition.DefaultValue.Equals(instanceToSave[adpropertyDefinition])))
				{
					object obj = null;
					instanceToSave.propertyBag.TryGetField(adpropertyDefinition, ref obj);
					if (!adpropertyDefinition.IsMultivalued || (obj != null && ((MultiValuedPropertyBase)obj).Changed))
					{
						ExTraceGlobals.ADSaveTracer.TraceDebug<string>((long)this.GetHashCode(), "AggregateTenantRecipientSession::Save - updating {0}", adpropertyDefinition.Name);
						if (adpropertyDefinition.MServPropertyDefinition != null)
						{
							if (this.BackendWriteMode == BackendWriteMode.WriteToMServ)
							{
								flag = true;
								if (flag2)
								{
									throw new ADOperationException(DirectoryStrings.MservAndMbxExclusive);
								}
								this.ApplyPropertyChangeToMserv(instanceToSave, adpropertyDefinition);
							}
							else if (adpropertyDefinition.MbxPropertyDefinition == null)
							{
								throw new ADOperationException(DirectoryStrings.NotInWriteToMServMode(adpropertyDefinition.Name));
							}
						}
						if (adpropertyDefinition.MbxPropertyDefinition != null)
						{
							if (this.BackendWriteMode == BackendWriteMode.WriteToMbx)
							{
								flag2 = true;
								if (flag)
								{
									throw new ADOperationException(DirectoryStrings.MservAndMbxExclusive);
								}
								this.ApplyPropertyChangeToMbx(instanceToSave, adpropertyDefinition);
							}
							else if (adpropertyDefinition.MServPropertyDefinition == null)
							{
								throw new ADOperationException(DirectoryStrings.NotInWriteToMbxMode(adpropertyDefinition.Name));
							}
						}
						if (adpropertyDefinition.MServPropertyDefinition == null && adpropertyDefinition.MbxPropertyDefinition == null && ((adpropertyDefinition.DefaultValue == null && instanceToSave[adpropertyDefinition] != null) || !adpropertyDefinition.DefaultValue.Equals(instanceToSave[adpropertyDefinition])) && adpropertyDefinition != ADRecipientSchema.UMDtmfMap)
						{
							throw new ADOperationException(DirectoryStrings.AggregatedSessionCannotMakeADChanges(adpropertyDefinition.Name));
						}
					}
				}
			}
			if (!flag)
			{
				if (flag2)
				{
					if (this.BackendWriteMode != BackendWriteMode.WriteToMbx)
					{
						throw new InvalidOperationException("AggregateTenantRecipientSession instance with BackendWriteMode != WriteToMbx cannot be used to change Mbx-backed properties");
					}
					ADObjectId adobjectId = (ADObjectId)instanceToSave[ADMailboxRecipientSchema.Database];
					Guid guid = (adobjectId != null) ? adobjectId.ObjectGuid : Guid.Empty;
					Guid guid2 = (instanceToSave[ADMailboxRecipientSchema.ExchangeGuid] != null) ? ((Guid)instanceToSave[ADMailboxRecipientSchema.ExchangeGuid]) : Guid.Empty;
					if (guid == Guid.Empty || guid2 == Guid.Empty)
					{
						throw new ADOperationException(DirectoryStrings.AggregatedSessionCannotMakeMbxChanges);
					}
					AggregationHelper.PerformMbxModification(guid, guid2, instanceToSave.MbxPropertyBag, !instanceToSave.DirectoryBackendType.HasFlag(DirectoryBackendType.Mbx));
				}
				return;
			}
			if (this.BackendWriteMode != BackendWriteMode.WriteToMServ)
			{
				throw new InvalidOperationException("AggregateTenantRecipientSession instance with BackendWriteMode != WriteToMServ cannot be used to change MServ-backed properties");
			}
			AggregationHelper.PerformMservModification(instanceToSave.MservPropertyBag);
		}

		private void ApplyPropertyChangeToMserv(ADRawEntry instanceToSave, ADPropertyDefinition property)
		{
			if (instanceToSave.MservPropertyBag == null)
			{
				instanceToSave.MservPropertyBag = new ADPropertyBag(false, 4);
				instanceToSave.MservPropertyBag[MServRecipientSchema.Id] = instanceToSave.Id;
			}
			if (!property.IsMultivalued)
			{
				instanceToSave.MservPropertyBag[property.MServPropertyDefinition] = instanceToSave.propertyBag[property];
				return;
			}
			MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)instanceToSave.propertyBag[property];
			if (multiValuedPropertyBase == null || multiValuedPropertyBase.WasCleared)
			{
				throw new MservOperationException(DirectoryStrings.NoResetOrAssignedMvp);
			}
			MultiValuedPropertyBase multiValuedPropertyBase2 = (MultiValuedPropertyBase)instanceToSave.MservPropertyBag[property.MServPropertyDefinition];
			foreach (object item in multiValuedPropertyBase.Removed)
			{
				multiValuedPropertyBase2.Remove(item);
			}
			foreach (object item2 in multiValuedPropertyBase.Added)
			{
				multiValuedPropertyBase2.Add(item2);
			}
		}

		private void ApplyPropertyChangeToMbx(ADRawEntry instanceToSave, ADPropertyDefinition property)
		{
			if (instanceToSave.MbxPropertyBag == null)
			{
				instanceToSave.MbxPropertyBag = new ADPropertyBag(false, 4);
			}
			if (!property.IsMultivalued)
			{
				instanceToSave.MbxPropertyBag[property.MbxPropertyDefinition] = instanceToSave.propertyBag[property];
				return;
			}
			MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)instanceToSave.propertyBag[property];
			MultiValuedPropertyBase multiValuedPropertyBase2 = (MultiValuedPropertyBase)instanceToSave.MbxPropertyBag[property.MbxPropertyDefinition];
			multiValuedPropertyBase2.Clear();
			foreach (object item in ((IEnumerable)multiValuedPropertyBase))
			{
				multiValuedPropertyBase2.Add(item);
			}
		}

		private void DeleteEntry(ADRecipient instanceToDelete)
		{
			ExTraceGlobals.ADSaveTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "AggregateTenantRecipientSession::DeleteEntry - updating or creating entry {0}", instanceToDelete.Id);
			throw new NotImplementedException("Delete");
		}

		MbxReadMode IAggregateSession.MbxReadMode
		{
			get
			{
				return this.mbxReadMode;
			}
			set
			{
				if (value == MbxReadMode.Always && !base.IsDirectoryBackendMbx)
				{
					throw new ArgumentException("Cannot enable Mbx reads if server role is not BE");
				}
				this.mbxReadMode = value;
			}
		}

		public BackendWriteMode BackendWriteMode
		{
			get
			{
				return this.backendWriteMode;
			}
			set
			{
				if (value == BackendWriteMode.WriteToMbx && !base.IsDirectoryBackendMbx)
				{
					throw new ArgumentException("Cannot enable Mbx writes if server role is not BE");
				}
				this.backendWriteMode = value;
			}
		}

		ADRawEntry IDirectorySession.ReadADRawEntry(ADObjectId adObjectId, IEnumerable<PropertyDefinition> properties)
		{
			ulong puidByADObjectId = this.GetPuidByADObjectId(adObjectId, true);
			return this.LoadADRawEntryByPuid(puidByADObjectId, properties);
		}

		Result<ADRawEntry>[] IDirectorySession.FindByADObjectIds(ADObjectId[] ids, params PropertyDefinition[] properties)
		{
			return this.ReadMultiple<ADObjectId, ulong, ADRawEntry>(ids, new AggregateTenantRecipientSession.KeyConverter<ADObjectId, ulong>(this.GetPuidByADObjectId), new AggregateTenantRecipientSession.RawDataRetriever<ulong, ADRawEntry>(this.LoadADRawEntryByPuid), properties);
		}

		Result<TData>[] IDirectorySession.FindByADObjectIds<TData>(ADObjectId[] ids)
		{
			return this.ReadMultiple<ADObjectId, ulong, TData>(ids, new AggregateTenantRecipientSession.KeyConverter<ADObjectId, ulong>(this.GetPuidByADObjectId), new AggregateTenantRecipientSession.RawDataRetriever<ulong, TData>(this.LoadGenericRecipientByPuid<TData>), null);
		}

		void IConfigDataProvider.Delete(IConfigurable instance)
		{
			((IRecipientSession)this).Delete((ADRecipient)instance);
		}

		IConfigurable IConfigDataProvider.Read<T>(ObjectId identity)
		{
			if (!typeof(ADRecipient).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(DirectoryStrings.ErrorWrongTypeParameter);
			}
			ADRecipient adrecipient = ((IRecipientSession)this).Read((ADObjectId)identity);
			if (!(adrecipient is T))
			{
				return null;
			}
			return adrecipient;
		}

		void IConfigDataProvider.Save(IConfigurable instance)
		{
			((IRecipientSession)this).Save((ADRecipient)instance);
		}

		ADObjectId IRecipientSession.SearchRoot
		{
			get
			{
				return this.searchRoot;
			}
		}

		void IRecipientSession.Delete(ADRecipient instanceToDelete)
		{
			throw this.HandleUnsupportedApi("OM:1298014", "Delete");
		}

		ADRawEntry IRecipientSession.FindADRawEntryBySid(SecurityIdentifier sid, IEnumerable<PropertyDefinition> properties)
		{
			ulong puidBySecurityIdentifier = this.GetPuidBySecurityIdentifier(sid, true);
			return this.LoadADRawEntryByPuid(puidBySecurityIdentifier, properties);
		}

		Result<ADRecipient>[] IRecipientSession.FindADRecipientsByLegacyExchangeDNs(string[] legacyExchangeDNs)
		{
			return this.ReadMultiple<string, ulong, ADRecipient>(legacyExchangeDNs, new AggregateTenantRecipientSession.KeyConverter<string, ulong>(this.GetPuidByLegacyExchangeDN), new AggregateTenantRecipientSession.RawDataRetriever<ulong, ADRecipient>(this.LoadGenericRecipientByPuid<ADRecipient>), null);
		}

		ADUser IRecipientSession.FindADUserByObjectId(ADObjectId adObjectId)
		{
			ulong puidByADObjectId = this.GetPuidByADObjectId(adObjectId, true);
			return this.LoadGenericRecipientByPuid<ADUser>(puidByADObjectId, null);
		}

		ADUser IRecipientSession.FindADUserByExternalDirectoryObjectId(string externalDirectoryObjectId)
		{
			ulong puidByExternalDirectoryObjectId = this.GetPuidByExternalDirectoryObjectId(externalDirectoryObjectId, true);
			return this.LoadGenericRecipientByPuid<ADUser>(puidByExternalDirectoryObjectId, null);
		}

		ADRawEntry IRecipientSession.FindByExchangeGuid(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			ulong puidByGuid = this.GetPuidByGuid(exchangeGuid, true);
			return this.LoadADRawEntryByPuid(puidByGuid, properties);
		}

		TData IRecipientSession.FindByExchangeGuid<TData>(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			ulong puidByGuid = this.GetPuidByGuid(exchangeGuid, true);
			return this.LoadGenericRecipientByPuid<TData>(puidByGuid, properties);
		}

		ADRecipient IRecipientSession.FindByExchangeGuid(Guid exchangeGuid)
		{
			ulong puidByGuid = this.GetPuidByGuid(exchangeGuid, true);
			return this.LoadGenericRecipientByPuid<ADRecipient>(puidByGuid, null);
		}

		ADRecipient IRecipientSession.FindByExchangeGuidIncludingAlternate(Guid exchangeGuid)
		{
			return ((IRecipientSession)this).FindByExchangeGuid(exchangeGuid);
		}

		ADRawEntry IRecipientSession.FindByExchangeGuidIncludingAlternate(Guid exchangeGuid, IEnumerable<PropertyDefinition> properties)
		{
			return ((IRecipientSession)this).FindByExchangeGuid(exchangeGuid, properties);
		}

		TData IRecipientSession.FindByExchangeGuidIncludingAlternate<TData>(Guid exchangeGuid)
		{
			ulong puidByGuid = this.GetPuidByGuid(exchangeGuid, true);
			return this.LoadGenericRecipientByPuid<TData>(puidByGuid, null);
		}

		ADRecipient IRecipientSession.FindByExchangeGuidIncludingArchive(Guid exchangeGuid)
		{
			return ((IRecipientSession)this).FindByExchangeGuid(exchangeGuid);
		}

		Result<ADRecipient>[] IRecipientSession.FindByExchangeGuidsIncludingArchive(Guid[] keys)
		{
			return this.ReadMultiple<Guid, ulong, ADRecipient>(keys, new AggregateTenantRecipientSession.KeyConverter<Guid, ulong>(this.GetPuidByGuid), new AggregateTenantRecipientSession.RawDataRetriever<ulong, ADRecipient>(this.LoadGenericRecipientByPuid<ADRecipient>), null);
		}

		ADRecipient IRecipientSession.FindByExchangeObjectId(Guid exchangeObjectId)
		{
			return ((IRecipientSession)this).FindByExchangeGuid(exchangeObjectId);
		}

		ADRecipient IRecipientSession.FindByLegacyExchangeDN(string legacyExchangeDN)
		{
			ulong puidByLegacyExchangeDN = this.GetPuidByLegacyExchangeDN(legacyExchangeDN, true);
			return this.LoadGenericRecipientByPuid<ADRecipient>(puidByLegacyExchangeDN, null);
		}

		Result<ADRawEntry>[] IRecipientSession.FindByLegacyExchangeDNs(string[] legacyExchangeDNs, params PropertyDefinition[] properties)
		{
			return this.ReadMultiple<string, ulong, ADRawEntry>(legacyExchangeDNs, new AggregateTenantRecipientSession.KeyConverter<string, ulong>(this.GetPuidByLegacyExchangeDN), new AggregateTenantRecipientSession.RawDataRetriever<ulong, ADRawEntry>(this.LoadADRawEntryByPuid), properties);
		}

		ADRecipient IRecipientSession.FindByObjectGuid(Guid guid)
		{
			return ((IRecipientSession)this).FindByExchangeGuid(guid);
		}

		ADRecipient IRecipientSession.FindByProxyAddress(ProxyAddress proxyAddress)
		{
			return this.LoadGenericRecipientByMemberName<ADRecipient>(new SmtpAddress(proxyAddress.ValueString), null);
		}

		ADRawEntry IRecipientSession.FindByProxyAddress(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties)
		{
			return this.LoadADRawEntryByMemberName(this.GetSmtpAddressByProxyAddress(proxyAddress, true), properties);
		}

		TData IRecipientSession.FindByProxyAddress<TData>(ProxyAddress proxyAddress)
		{
			return this.LoadGenericRecipientByMemberName<TData>(this.GetSmtpAddressByProxyAddress(proxyAddress, true), null);
		}

		Result<ADRawEntry>[] IRecipientSession.FindByProxyAddresses(ProxyAddress[] proxyAddresses, params PropertyDefinition[] properties)
		{
			return this.ReadMultiple<ProxyAddress, SmtpAddress, ADRawEntry>(proxyAddresses, new AggregateTenantRecipientSession.KeyConverter<ProxyAddress, SmtpAddress>(this.GetSmtpAddressByProxyAddress), new AggregateTenantRecipientSession.RawDataRetriever<SmtpAddress, ADRawEntry>(this.LoadADRawEntryByMemberName), properties);
		}

		Result<TData>[] IRecipientSession.FindByProxyAddresses<TData>(ProxyAddress[] proxyAddresses)
		{
			return this.ReadMultiple<ProxyAddress, SmtpAddress, TData>(proxyAddresses, new AggregateTenantRecipientSession.KeyConverter<ProxyAddress, SmtpAddress>(this.GetSmtpAddressByProxyAddress), new AggregateTenantRecipientSession.RawDataRetriever<SmtpAddress, TData>(this.LoadGenericRecipientByMemberName<TData>), null);
		}

		Result<ADRecipient>[] IRecipientSession.FindByProxyAddresses(ProxyAddress[] proxyAddresses)
		{
			return this.ReadMultiple<ProxyAddress, SmtpAddress, ADRecipient>(proxyAddresses, new AggregateTenantRecipientSession.KeyConverter<ProxyAddress, SmtpAddress>(this.GetSmtpAddressByProxyAddress), new AggregateTenantRecipientSession.RawDataRetriever<SmtpAddress, ADRecipient>(this.LoadGenericRecipientByMemberName<ADRecipient>), null);
		}

		ADRecipient IRecipientSession.FindBySid(SecurityIdentifier sid)
		{
			ulong puidBySecurityIdentifier = this.GetPuidBySecurityIdentifier(sid, true);
			return this.LoadGenericRecipientByPuid<ADRecipient>(puidBySecurityIdentifier, null);
		}

		TResult IRecipientSession.FindMiniRecipientByProxyAddress<TResult>(ProxyAddress proxyAddress, IEnumerable<PropertyDefinition> properties)
		{
			return this.LoadGenericRecipientByMemberName<TResult>(this.GetSmtpAddressByProxyAddress(proxyAddress, true), properties);
		}

		TResult IRecipientSession.FindMiniRecipientBySid<TResult>(SecurityIdentifier sid, IEnumerable<PropertyDefinition> properties)
		{
			ulong puidBySecurityIdentifier = this.GetPuidBySecurityIdentifier(sid, true);
			return this.LoadGenericRecipientByPuid<TResult>(puidBySecurityIdentifier, properties);
		}

		MiniRecipientWithTokenGroups IRecipientSession.ReadTokenGroupsGlobalAndUniversal(ADObjectId id)
		{
			throw this.HandleUnsupportedApi("OM:1054958", "ReadTokenGroupsGlobalAndUniversal");
		}

		List<string> IRecipientSession.GetTokenSids(ADRawEntry user, AssignmentMethod assignmentMethodFlags)
		{
			return ((IRecipientSession)this).GetTokenSids(user.Id, assignmentMethodFlags);
		}

		List<string> IRecipientSession.GetTokenSids(ADObjectId userId, AssignmentMethod assignmentMethodFlags)
		{
			return AggregateTenantRecipientSession.defaultTokenSids;
		}

		ADRecipient IRecipientSession.Read(ADObjectId adObjectId)
		{
			ulong puidByADObjectId = this.GetPuidByADObjectId(adObjectId, true);
			return this.LoadGenericRecipientByPuid<ADRecipient>(puidByADObjectId, null);
		}

		MiniRecipient IRecipientSession.ReadMiniRecipient(ADObjectId adObjectId, IEnumerable<PropertyDefinition> properties)
		{
			ulong puidByADObjectId = this.GetPuidByADObjectId(adObjectId, true);
			return this.LoadGenericRecipientByPuid<MiniRecipient>(puidByADObjectId, properties);
		}

		TMiniRecipient IRecipientSession.ReadMiniRecipient<TMiniRecipient>(ADObjectId adObjectId, IEnumerable<PropertyDefinition> properties)
		{
			ulong puidByADObjectId = this.GetPuidByADObjectId(adObjectId, true);
			return this.LoadGenericRecipientByPuid<TMiniRecipient>(puidByADObjectId, properties);
		}

		ADRawEntry IRecipientSession.FindUserBySid(SecurityIdentifier sid, IList<PropertyDefinition> properties)
		{
			ulong puidBySecurityIdentifier = this.GetPuidBySecurityIdentifier(sid, true);
			return this.LoadADRawEntryByPuid(puidBySecurityIdentifier, properties);
		}

		Result<ADRecipient>[] IRecipientSession.ReadMultiple(ADObjectId[] entryIds)
		{
			return this.ReadMultiple<ADObjectId, ulong, ADRecipient>(entryIds, new AggregateTenantRecipientSession.KeyConverter<ADObjectId, ulong>(this.GetPuidByADObjectId), new AggregateTenantRecipientSession.RawDataRetriever<ulong, ADRecipient>(this.LoadGenericRecipientByPuid<ADRecipient>), null);
		}

		Result<ADRawEntry>[] IRecipientSession.ReadMultiple(ADObjectId[] entryIds, params PropertyDefinition[] properties)
		{
			return this.ReadMultiple<ADObjectId, ulong, ADRawEntry>(entryIds, new AggregateTenantRecipientSession.KeyConverter<ADObjectId, ulong>(this.GetPuidByADObjectId), new AggregateTenantRecipientSession.RawDataRetriever<ulong, ADRawEntry>(this.LoadADRawEntryByPuid), properties);
		}

		Result<ADUser>[] IRecipientSession.ReadMultipleADUsers(ADObjectId[] userIds)
		{
			return this.ReadMultiple<ADObjectId, ulong, ADUser>(userIds, new AggregateTenantRecipientSession.KeyConverter<ADObjectId, ulong>(this.GetPuidByADObjectId), new AggregateTenantRecipientSession.RawDataRetriever<ulong, ADUser>(this.LoadGenericRecipientByPuid<ADUser>), null);
		}

		Result<ADRawEntry>[] IRecipientSession.ReadMultipleWithDeletedObjects(ADObjectId[] entryIds, params PropertyDefinition[] properties)
		{
			return ((IRecipientSession)this).ReadMultiple(entryIds, properties);
		}

		ADObjectId[] IRecipientSession.ResolveSidsToADObjectIds(string[] sids)
		{
			ulong[] source = (from k in sids
			select this.GetPuidBySecurityIdentifier(new SecurityIdentifier(k), true)).ToArray<ulong>();
			return (from k in source
			select ConsumerIdentityHelper.GetADObjectIdFromPuid(k)).ToArray<ADObjectId>();
		}

		void IRecipientSession.Save(ADRecipient instanceToSave)
		{
			((IRecipientSession)this).Save(instanceToSave, false);
		}

		void IRecipientSession.Save(ADRecipient instanceToSave, bool bypassValidation)
		{
			ExTraceGlobals.ADSaveTracer.TraceDebug((long)this.GetHashCode(), "AggregateTenantRecipientSession::Save - saving object of type {0} and ID {1}.", new object[]
			{
				(instanceToSave == null) ? "<null>" : instanceToSave.GetType().Name,
				(instanceToSave == null) ? "<null>" : instanceToSave.Id
			});
			if (instanceToSave == null)
			{
				throw new ArgumentNullException("instanceToSave");
			}
			if (instanceToSave.Id == null)
			{
				throw new ArgumentNullException("instanceToSave.Id");
			}
			if (this.readOnly)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionInvalidOperationOnReadOnlySession("Save()"));
			}
			if (instanceToSave.IsReadOnly)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionInvalidOperationOnReadOnlyObject("Save()"));
			}
			if (!bypassValidation)
			{
				ValidationError[] array = instanceToSave.Validate();
				if (array.Length > 0)
				{
					throw new DataValidationException(array[0]);
				}
			}
			if (this.BackendWriteMode == BackendWriteMode.NoWrites)
			{
				throw new ArgumentException("Cannot complete Save operation - BackendWriteMode is set to NoWrites");
			}
			if (instanceToSave.ObjectState == ObjectState.Deleted)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionObjectHasBeenDeleted);
			}
			if (instanceToSave.ObjectState == ObjectState.Unchanged && (instanceToSave.MbxPropertyBag == null || !instanceToSave.MbxPropertyBag.Changed))
			{
				return;
			}
			if (instanceToSave.ObjectState == ObjectState.New || instanceToSave.ObjectState == ObjectState.Changed)
			{
				this.CreateOrUpdateEntry(instanceToSave);
			}
		}

		public override DirectoryBackendType DirectoryBackendType
		{
			get
			{
				return this.directoryBackendType;
			}
		}

		Result<ADRawEntry>[] ITenantRecipientSession.FindByExternalDirectoryObjectIds(string[] externalDirectoryObjectIds, params PropertyDefinition[] properties)
		{
			return ((ITenantRecipientSession)this).FindByExternalDirectoryObjectIds(externalDirectoryObjectIds, false, properties);
		}

		Result<ADRawEntry>[] ITenantRecipientSession.FindByExternalDirectoryObjectIds(string[] externalDirectoryObjectIds, bool includeDeletedObjects, params PropertyDefinition[] properties)
		{
			return this.ReadMultiple<string, ulong, ADRawEntry>(externalDirectoryObjectIds, new AggregateTenantRecipientSession.KeyConverter<string, ulong>(this.GetPuidByExternalDirectoryObjectId), new AggregateTenantRecipientSession.RawDataRetriever<ulong, ADRawEntry>(this.LoadADRawEntryByPuid), properties);
		}

		ADRawEntry[] ITenantRecipientSession.FindByNetID(string netID, string organizationContext, params PropertyDefinition[] properties)
		{
			return ((ITenantRecipientSession)this).FindByNetID(netID, properties);
		}

		ADRawEntry[] ITenantRecipientSession.FindByNetID(string netID, params PropertyDefinition[] properties)
		{
			return new ADRawEntry[]
			{
				((ITenantRecipientSession)this).FindUniqueEntryByNetID(netID, properties)
			};
		}

		MiniRecipient ITenantRecipientSession.FindRecipientByNetID(NetID netId)
		{
			ulong puid = netId.ToUInt64();
			return this.LoadGenericRecipientByPuid<MiniRecipient>(puid, null);
		}

		ADRawEntry ITenantRecipientSession.FindUniqueEntryByNetID(string netID, string organizationContext, params PropertyDefinition[] properties)
		{
			return ((ITenantRecipientSession)this).FindUniqueEntryByNetID(netID, properties);
		}

		ADRawEntry ITenantRecipientSession.FindUniqueEntryByNetID(string netID, params PropertyDefinition[] properties)
		{
			return this.LoadADRawEntryByPuid(ConsumerIdentityHelper.ConvertPuidStringToPuidNumber(netID), properties);
		}

		ADRawEntry ITenantRecipientSession.FindByLiveIdMemberName(string liveIdMemberName, params PropertyDefinition[] properties)
		{
			return this.LoadADRawEntryByMemberName(new SmtpAddress(liveIdMemberName), properties);
		}

		void IDirectorySession.AnalyzeDirectoryError(PooledLdapConnection connection, DirectoryRequest request, DirectoryException de, int totalRetries, int retriesOnServer)
		{
			throw this.HandleUnsupportedApi(null, "AnalyzeDirectoryError");
		}

		QueryFilter IDirectorySession.ApplyDefaultFilters(QueryFilter filter, ADObjectId rootId, ADObject dummyInstance, bool applyImplicitFilter)
		{
			throw this.HandleUnsupportedApi(null, "ApplyDefaultFilters");
		}

		QueryFilter IDirectorySession.ApplyDefaultFilters(QueryFilter filter, ADScope scope, ADObject dummyInstance, bool applyImplicitFilter)
		{
			throw this.HandleUnsupportedApi(null, "ApplyDefaultFilters");
		}

		void IDirectorySession.CheckFilterForUnsafeIdentity(QueryFilter filter)
		{
			throw this.HandleUnsupportedApi(null, "CheckFilterForUnsafeIdentity");
		}

		void IDirectorySession.UnsafeExecuteModificationRequest(DirectoryRequest request, ADObjectId rootId)
		{
			throw this.HandleUnsupportedApi(null, "UnsafeExecuteModificationRequest");
		}

		ADRawEntry[] IDirectorySession.Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			throw this.HandleUnsupportedApi(null, "Find");
		}

		TResult[] IDirectorySession.Find<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			throw this.HandleUnsupportedApi(null, "Find");
		}

		ADRawEntry[] IDirectorySession.FindAllADRawEntriesByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, bool useAtomicFilter, IEnumerable<PropertyDefinition> properties)
		{
			throw this.HandleUnsupportedApi(null, "FindAllADRawEntriesByUsnRange");
		}

		Result<ADRawEntry>[] IDirectorySession.FindByCorrelationIds(Guid[] correlationIds, ADObjectId configUnit, params PropertyDefinition[] properties)
		{
			throw this.HandleUnsupportedApi(null, "FindByCorrelationIds");
		}

		Result<ADRawEntry>[] IDirectorySession.FindByExchangeLegacyDNs(string[] exchangeLegacyDNs, params PropertyDefinition[] properties)
		{
			throw this.HandleUnsupportedApi(null, "FindByExchangeLegacyDNs");
		}

		Result<ADRawEntry>[] IDirectorySession.FindByObjectGuids(Guid[] objectGuids, params PropertyDefinition[] properties)
		{
			throw this.HandleUnsupportedApi(null, "FindByObjectGuids");
		}

		ADRawEntry[] IDirectorySession.FindDeletedTenantSyncObjectByUsnRange(ADObjectId tenantOuRoot, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties)
		{
			throw this.HandleUnsupportedApi(null, "FindDeletedTenantSyncObjectByUsnRange");
		}

		ADPagedReader<TResult> IDirectorySession.FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			throw this.HandleUnsupportedApi(null, "FindPaged");
		}

		ADPagedReader<ADRawEntry> IDirectorySession.FindPagedADRawEntry(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			throw this.HandleUnsupportedApi(null, "FindPagedADRawEntry");
		}

		ADPagedReader<ADRawEntry> IDirectorySession.FindPagedADRawEntryWithDefaultFilters<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			throw this.HandleUnsupportedApi(null, "FindPagedADRawEntryWithDefaultFilters");
		}

		ADPagedReader<TResult> IDirectorySession.FindPagedDeletedObject<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			throw this.HandleUnsupportedApi(null, "FindPagedDeletedObject");
		}

		ADObjectId IDirectorySession.GetConfigurationNamingContext()
		{
			return TemplateTenantConfiguration.GetTempateTenantRecipientSession().GetConfigurationNamingContext();
		}

		ADObjectId IDirectorySession.GetConfigurationUnitsRoot()
		{
			throw this.HandleUnsupportedApi(null, "GetConfigurationUnitsRoot");
		}

		ADObjectId IDirectorySession.GetDomainNamingContext()
		{
			return TemplateTenantConfiguration.GetTempateTenantRecipientSession().GetDomainNamingContext();
		}

		ADObjectId IDirectorySession.GetHostedOrganizationsRoot()
		{
			throw this.HandleUnsupportedApi(null, "GetHostedOrganizationsRoot");
		}

		ADObjectId IDirectorySession.GetRootDomainNamingContext()
		{
			return TemplateTenantConfiguration.GetTempateTenantRecipientSession().GetRootDomainNamingContext();
		}

		ADObjectId IDirectorySession.GetSchemaNamingContext()
		{
			return TemplateTenantConfiguration.GetTempateTenantRecipientSession().GetSchemaNamingContext();
		}

		PooledLdapConnection IDirectorySession.GetReadConnection(string preferredServer, ref ADObjectId rootId)
		{
			throw this.HandleUnsupportedApi(null, "GetReadConnection");
		}

		PooledLdapConnection IDirectorySession.GetReadConnection(string preferredServer, string optionalBaseDN, ref ADObjectId rootId, ADRawEntry scopeDeteriminingObject)
		{
			throw this.HandleUnsupportedApi(null, "GetReadConnection");
		}

		ADScope IDirectorySession.GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject)
		{
			throw this.HandleUnsupportedApi(null, "GetReadScope");
		}

		ADScope IDirectorySession.GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject, bool isWellKnownGuidSearch, out ConfigScopes applicableScope)
		{
			throw this.HandleUnsupportedApi(null, "GetReadScope");
		}

		bool IDirectorySession.GetSchemaAndApplyFilter(ADRawEntry adRawEntry, ADScope scope, out ADObject dummyInstance, out string[] ldapAttributes, ref QueryFilter filter, ref IEnumerable<PropertyDefinition> properties)
		{
			throw this.HandleUnsupportedApi(null, "GetSchemaAndApplyFilter");
		}

		bool IDirectorySession.IsReadConnectionAvailable()
		{
			return true;
		}

		bool IDirectorySession.IsRootIdWithinScope<TData>(ADObjectId rootId)
		{
			return true;
		}

		bool IDirectorySession.IsTenantIdentity(ADObjectId id)
		{
			throw this.HandleUnsupportedApi(null, "IsTenantIdentity");
		}

		public override SecurityDescriptor ReadSecurityDescriptorBlob(ADObjectId id)
		{
			return TemplateTenantConfiguration.GetTemplateUserSecurityDescriptorBlob();
		}

		string[] IDirectorySession.ReplicateSingleObject(ADObject instanceToReplicate, ADObjectId[] sites)
		{
			throw this.HandleUnsupportedApi(null, "ReplicateSingleObject");
		}

		bool IDirectorySession.ReplicateSingleObjectToTargetDC(ADObject instanceToReplicate, string targetServerName)
		{
			throw this.HandleUnsupportedApi(null, "ReplicateSingleObjectToTargetDC");
		}

		TResult IDirectorySession.ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, ADObjectId containerId)
		{
			throw this.HandleUnsupportedApi(null, "ResolveWellKnownGuid");
		}

		TResult IDirectorySession.ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, string containerDN)
		{
			throw this.HandleUnsupportedApi(null, "ResolveWellKnownGuid");
		}

		TenantRelocationSyncObject IDirectorySession.RetrieveTenantRelocationSyncObject(ADObjectId adObjectId, IEnumerable<PropertyDefinition> properties)
		{
			throw this.HandleUnsupportedApi(null, "RetrieveTenantRelocationSyncObject");
		}

		ADOperationResultWithData<TResult>[] IDirectorySession.RunAgainstAllDCsInSite<TResult>(ADObjectId siteId, Func<TResult> methodToCall)
		{
			throw this.HandleUnsupportedApi(null, "RunAgainstAllDCsInSite");
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd)
		{
			throw this.HandleUnsupportedApi(null, "SaveSecurityDescriptor");
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd, bool modifyOwner)
		{
			throw this.HandleUnsupportedApi(null, "SaveSecurityDescriptor");
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd)
		{
			throw this.HandleUnsupportedApi(null, "SaveSecurityDescriptor");
		}

		void IDirectorySession.SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd, bool modifyOwner)
		{
			throw this.HandleUnsupportedApi(null, "SaveSecurityDescriptor");
		}

		bool IDirectorySession.TryVerifyIsWithinScopes(ADObject entry, bool isModification, out ADScopeException exception)
		{
			exception = null;
			return true;
		}

		void IDirectorySession.UpdateServerSettings(PooledLdapConnection connection)
		{
			throw this.HandleUnsupportedApi(null, "UpdateServerSettings");
		}

		void IDirectorySession.VerifyIsWithinScopes(ADObject entry, bool isModification)
		{
			throw this.HandleUnsupportedApi(null, "VerifyIsWithinScopes");
		}

		TResult[] IDirectorySession.ObjectsFromEntries<TResult>(SearchResultEntryCollection entries, string originatingServerName, IEnumerable<PropertyDefinition> properties, ADRawEntry dummyInstance)
		{
			throw this.HandleUnsupportedApi(null, "ObjectsFromEntries");
		}

		IConfigurable[] IConfigDataProvider.Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			throw this.HandleUnsupportedApi(null, "Find");
		}

		IEnumerable<T> IConfigDataProvider.FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize)
		{
			throw this.HandleUnsupportedApi(null, "FindPaged");
		}

		string IConfigDataProvider.Source
		{
			get
			{
				return DirectoryBackendType.MServ.ToString();
			}
		}

		ITableView IRecipientSession.Browse(ADObjectId addressListId, int rowCountSuggestion, params PropertyDefinition[] properties)
		{
			throw this.HandleUnsupportedApi(null, "Browse");
		}

		ADRecipient[] IRecipientSession.Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			throw this.HandleUnsupportedApi(null, "Find");
		}

		ADRawEntry[] IRecipientSession.FindADRawEntryByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryScope scope, QueryFilter additionalFilter)
		{
			throw this.HandleUnsupportedApi(null, "FindADRawEntryByUsnRange");
		}

		ADUser[] IRecipientSession.FindADUser(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults)
		{
			throw this.HandleUnsupportedApi(null, "FindADUser");
		}

		ADObject IRecipientSession.FindByAccountName<T>(string domainName, string accountName)
		{
			throw this.HandleUnsupportedApi(null, "FindByAccountName");
		}

		IEnumerable<T> IRecipientSession.FindByAccountName<T>(string domain, string account, ADObjectId rootId, QueryFilter searchFilter)
		{
			throw this.HandleUnsupportedApi(null, "FindByAccountName");
		}

		ADRecipient[] IRecipientSession.FindByANR(string anrMatch, int maxResults, SortBy sortBy)
		{
			throw this.HandleUnsupportedApi(null, "FindByANR");
		}

		ADRawEntry[] IRecipientSession.FindByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties)
		{
			throw this.HandleUnsupportedApi(null, "FindByANR");
		}

		ADRecipient IRecipientSession.FindByCertificate(X509Identifier identifier)
		{
			throw this.HandleUnsupportedApi(null, "FindByCertificate");
		}

		ADRawEntry[] IRecipientSession.FindByCertificate(X509Identifier identifier, params PropertyDefinition[] properties)
		{
			throw this.HandleUnsupportedApi(null, "FindByCertificate");
		}

		ADRawEntry[] IRecipientSession.FindDeletedADRawEntryByUsnRange(ADObjectId lastKnownParentId, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties, QueryFilter additionalFilter)
		{
			throw this.HandleUnsupportedApi(null, "FindDeletedADRawEntryByUsnRange");
		}

		MiniRecipient[] IRecipientSession.FindMiniRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			throw this.HandleUnsupportedApi(null, "FindMiniRecipient");
		}

		MiniRecipient[] IRecipientSession.FindMiniRecipientByANR(string anrMatch, int maxResults, SortBy sortBy, IEnumerable<PropertyDefinition> properties)
		{
			throw this.HandleUnsupportedApi(null, "FindMiniRecipientByANR");
		}

		ADRecipient[] IRecipientSession.FindNames(IDictionary<PropertyDefinition, object> dictionary, int limit)
		{
			throw this.HandleUnsupportedApi(null, "FindNames");
		}

		object[][] IRecipientSession.FindNamesView(IDictionary<PropertyDefinition, object> dictionary, int limit, params PropertyDefinition[] properties)
		{
			throw this.HandleUnsupportedApi(null, "FindNamesView");
		}

		Result<OWAMiniRecipient>[] IRecipientSession.FindOWAMiniRecipientByUserPrincipalName(string[] userPrincipalNames)
		{
			throw this.HandleUnsupportedApi(null, "FindOWAMiniRecipientByUserPrincipalName");
		}

		ADPagedReader<ADRecipient> IRecipientSession.FindPaged(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize)
		{
			throw this.HandleUnsupportedApi(null, "FindPaged");
		}

		ADPagedReader<TData> IRecipientSession.FindPagedMiniRecipient<TData>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			throw this.HandleUnsupportedApi(null, "FindPagedMiniRecipient");
		}

		ADRawEntry[] IRecipientSession.FindRecipient(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			throw this.HandleUnsupportedApi(null, "FindRecipient");
		}

		IEnumerable<ADGroup> IRecipientSession.FindRoleGroupsByForeignGroupSid(ADObjectId root, SecurityIdentifier sid)
		{
			throw this.HandleUnsupportedApi(null, "FindRoleGroupsByForeignGroupSid");
		}

		SecurityIdentifier IRecipientSession.GetWellKnownExchangeGroupSid(Guid wkguid)
		{
			throw this.HandleUnsupportedApi(null, "GetWellKnownExchangeGroupSid");
		}

		bool IRecipientSession.IsLegacyDNInUse(string legacyDN)
		{
			throw this.HandleUnsupportedApi(null, "IsLegacyDNInUse");
		}

		bool IRecipientSession.IsMemberOfGroupByWellKnownGuid(Guid wellKnownGuid, string containerDN, ADObjectId id)
		{
			throw this.HandleUnsupportedApi(null, "IsMemberOfGroupByWellKnownGuid");
		}

		bool IRecipientSession.IsRecipientInOrg(ProxyAddress proxyAddress)
		{
			throw this.HandleUnsupportedApi(null, "IsRecipientInOrg");
		}

		bool IRecipientSession.IsReducedRecipientSession()
		{
			return false;
		}

		bool IRecipientSession.IsThrottlingPolicyInUse(ADObjectId throttlingPolicyId)
		{
			throw this.HandleUnsupportedApi(null, "IsThrottlingPolicyInUse");
		}

		Result<ADGroup>[] IRecipientSession.ReadMultipleADGroups(ADObjectId[] entryIds)
		{
			throw this.HandleUnsupportedApi(null, "ReadMultipleADGroups");
		}

		void IRecipientSession.SetPassword(ADObject obj, SecureString password)
		{
			throw this.HandleUnsupportedApi(null, "SetPassword");
		}

		void IRecipientSession.SetPassword(ADObjectId id, SecureString password)
		{
			throw this.HandleUnsupportedApi(null, "SetPassword");
		}

		ADRawEntry ITenantRecipientSession.ChooseBetweenAmbiguousUsers(ADRawEntry[] entries)
		{
			throw this.HandleUnsupportedApi(null, "ChooseBetweenAmbiguousUsers");
		}

		ADObjectId ITenantRecipientSession.ChooseBetweenAmbiguousUsers(ADObjectId user1Id, ADObjectId user2Id)
		{
			throw this.HandleUnsupportedApi(null, "ChooseBetweenAmbiguousUsers");
		}

		Result<ADRawEntry>[] ITenantRecipientSession.ReadMultipleByLinkedPartnerId(LinkedPartnerGroupInformation[] entryIds, params PropertyDefinition[] properties)
		{
			throw this.HandleUnsupportedApi(null, "ReadMultipleByLinkedPartnerId");
		}

		private static string className = typeof(AggregateTenantRecipientSession).FullName;

		private static List<string> defaultTokenSids = new List<string>
		{
			"S-1-1-0",
			"S-1-5-11"
		};

		private MbxReadMode mbxReadMode;

		private BackendWriteMode backendWriteMode;

		private readonly DirectoryBackendType directoryBackendType;

		private delegate TIdentity KeyConverter<TKey, TIdentity>(TKey key, bool throwOnError);

		private delegate TData DataRetriever<TIdentity, TData>(TIdentity id);

		private delegate TData RawDataRetriever<TIdentity, TData>(TIdentity id, IEnumerable<PropertyDefinition> properties);
	}
}
