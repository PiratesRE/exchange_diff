using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.DirSync;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.ExchangeTopology;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationCache;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal abstract class ADDataSession : DirectorySessionBase, IDirectorySession
	{
		public override DirectoryBackendType DirectoryBackendType
		{
			get
			{
				return DirectoryBackendType.AD;
			}
		}

		protected ADDataSession(bool useConfigNC, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(useConfigNC, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\ADDriver\\Parameters"))
			{
				this.diag_enabled = ADDataSession.Diag_GetRegistryBool(registryKey, "LoggingEnabled", false);
			}
			this.LogCtorCallStack();
		}

		private static bool MatchesOpathFilter(ADRawEntry obj, QueryFilter filter)
		{
			return filter == null || OpathFilterEvaluator.FilterMatches(filter, obj);
		}

		private static QueryFilter QueryFilterFromCorrelationId(Guid guid)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.CorrelationIdRaw, guid);
		}

		private static QueryFilter QueryFilterFromObjectGuid(Guid guid)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guid);
		}

		private static void FindByObjectGuidsHashInserter<TResult>(Hashtable hash, TResult entry) where TResult : ADRawEntry
		{
			Result<TResult> result = new Result<TResult>(entry, null);
			Guid guid = (Guid)entry.propertyBag[ADObjectSchema.Guid];
			if (hash.ContainsKey(guid))
			{
				hash[guid] = new Result<TResult>(default(TResult), new ObjectValidationError(DirectoryStrings.ErrorNonUniqueExchangeGuid(guid.ToString()), entry.Id, string.Empty));
				return;
			}
			hash.Add(guid, result);
		}

		private static void FindByCorrelationIdsHashInserter<TResult>(Hashtable hash, TResult entry) where TResult : ADRawEntry
		{
			Result<TResult> result = new Result<TResult>(entry, null);
			Guid guid = (Guid)entry.propertyBag[ADObjectSchema.CorrelationIdRaw];
			if (Guid.Empty.Equals(guid) || hash.ContainsKey(guid))
			{
				hash[guid] = new Result<TResult>(default(TResult), new ObjectValidationError(DirectoryStrings.ErrorNonUniqueExchangeGuid(guid.ToString()), entry.Id, string.Empty));
				return;
			}
			hash.Add(guid, result);
		}

		private static Result<TResult> FindByCorrelationIdsHashLookup<TResult>(Hashtable hash, Guid key) where TResult : ADRawEntry
		{
			if (hash.ContainsKey(key))
			{
				return (Result<TResult>)hash[key];
			}
			return new Result<TResult>(default(TResult), ProviderError.NotFound);
		}

		private static Result<TResult> FindByObjectGuidsHashLookup<TResult>(Hashtable hash, Guid key) where TResult : ADRawEntry
		{
			if (hash.ContainsKey(key))
			{
				return (Result<TResult>)hash[key];
			}
			return new Result<TResult>(default(TResult), ProviderError.NotFound);
		}

		private static QueryFilter QueryFilterFromExchangeLegacyDN(string exchangeLegacyDn)
		{
			if (exchangeLegacyDn == null)
			{
				throw new ArgumentNullException("exchangeLegacyDn");
			}
			if (exchangeLegacyDn == string.Empty)
			{
				throw new ArgumentException("exchangeLegacyDn");
			}
			List<QueryFilter> list = new List<QueryFilter>(3);
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, SharedPropertyDefinitions.ExchangeLegacyDN, exchangeLegacyDn));
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, "x500:" + exchangeLegacyDn));
			Guid guid;
			if (LegacyDN.TryParseNspiDN(exchangeLegacyDn, out guid))
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guid));
			}
			return new OrFilter(list.ToArray());
		}

		private static Result<T> FindByExchangeLegacyDNsHashLookup<T>(Hashtable hash, string key) where T : ADRawEntry
		{
			if (!string.IsNullOrEmpty(key))
			{
				object obj = hash[key];
				if (obj != null)
				{
					return (Result<T>)obj;
				}
				obj = hash["x500:" + key];
				if (obj != null)
				{
					return (Result<T>)obj;
				}
				Guid guid;
				if (LegacyDN.TryParseNspiDN(key, out guid))
				{
					obj = hash["guid:" + guid.ToString()];
					if (obj != null)
					{
						return (Result<T>)obj;
					}
				}
			}
			return new Result<T>(default(T), ProviderError.NotFound);
		}

		private static void FindByExchangeLegacyDNsHashInserter<TResult>(Hashtable hash, TResult entry) where TResult : ADRawEntry
		{
			Result<TResult> result = new Result<TResult>(entry, null);
			string text = (string)entry.propertyBag[ServerSchema.ExchangeLegacyDN];
			if (hash.ContainsKey(text))
			{
				hash[text] = new Result<TResult>(default(TResult), new ObjectValidationError(DirectoryStrings.ErrorNonUniqueLegacyDN(text), entry.Id, string.Empty));
			}
			else
			{
				hash.Add(text, result);
			}
			foreach (ProxyAddress proxyAddress in ((ProxyAddressCollection)entry.propertyBag[ADRecipientSchema.EmailAddresses]))
			{
				if (proxyAddress.Prefix == ProxyAddressPrefix.X500)
				{
					string key = proxyAddress.ToString();
					if (hash.ContainsKey(key))
					{
						hash[key] = new Result<TResult>(default(TResult), new NonUniqueProxyAddressError(DirectoryStrings.ErrorNonUniqueProxy(proxyAddress.ToString()), entry.Id, string.Empty));
					}
					else
					{
						hash.Add(key, result);
					}
				}
			}
			string text2 = "guid:" + ((Guid)entry.propertyBag[ADObjectSchema.Guid]).ToString();
			if (hash.ContainsKey(text2))
			{
				hash[text2] = new Result<TResult>(default(TResult), new ObjectValidationError(DirectoryStrings.ErrorNonUniqueLegacyDN(text2), entry.Id, string.Empty));
				return;
			}
			hash.Add(text2, result);
		}

		private static QueryFilter QueryFilterFromADObjectId(ADObjectId id)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, id);
		}

		private static void FindByADObjectIdsHashInserter<TResult>(Hashtable hash, TResult entry) where TResult : ADRawEntry
		{
			Result<TResult> result = new Result<TResult>(entry, null);
			ADObjectId adobjectId = (ADObjectId)entry.propertyBag[ADObjectSchema.Id];
			string value = adobjectId.DistinguishedName.ToLower();
			if (!string.IsNullOrEmpty(value) && hash.ContainsKey(adobjectId.DistinguishedName))
			{
				hash[adobjectId.DistinguishedName] = new Result<TResult>(default(TResult), new ObjectValidationError(DirectoryStrings.ErrorNonUniqueDN(adobjectId.ToString()), entry.Id, string.Empty));
			}
			else
			{
				hash.Add(adobjectId.DistinguishedName, result);
			}
			if (!adobjectId.ObjectGuid.Equals(Guid.Empty) && hash.ContainsKey(adobjectId.ObjectGuid))
			{
				hash[adobjectId.ObjectGuid] = new Result<TResult>(default(TResult), new ObjectValidationError(DirectoryStrings.ErrorNonUniqueDN(adobjectId.ToString()), entry.Id, string.Empty));
				return;
			}
			hash.Add(adobjectId.ObjectGuid, result);
		}

		private static Result<TResult> FindByADObjectIdsHashLookup<TResult>(Hashtable hash, ADObjectId key) where TResult : ADRawEntry
		{
			string distinguishedName = key.DistinguishedName;
			if (!string.IsNullOrEmpty(distinguishedName) && hash.ContainsKey(distinguishedName))
			{
				return (Result<TResult>)hash[distinguishedName];
			}
			if (hash.ContainsKey(key.ObjectGuid))
			{
				return (Result<TResult>)hash[key.ObjectGuid];
			}
			return new Result<TResult>(default(TResult), ProviderError.NotFound);
		}

		private bool IsTenantRootContainer(ADObjectId id)
		{
			return id.Equals(this.GetConfigurationUnitsRoot()) || id.Equals(this.GetHostedOrganizationsRoot());
		}

		private void LogCtorCallStack()
		{
			try
			{
				if (this.diag_enabled)
				{
					if (this.sessionSettings.IsGlobal)
					{
						string text = string.Format("{0},{1}", this.GetHashCode(), base.GetType().Name);
						if (ADDataSession.diag_logWriter == null)
						{
							lock ("diag_logWriter")
							{
								if (ADDataSession.diag_logWriter == null)
								{
									using (Process currentProcess = Process.GetCurrentProcess())
									{
										ADDataSession.diag_logWriter = new StreamWriter(string.Format("{0}\\temp\\{1}-{2}-{3}-{4}.txt", new object[]
										{
											Environment.GetEnvironmentVariable("windir"),
											Environment.MachineName,
											currentProcess.MainModule.ModuleName,
											currentProcess.Id,
											Environment.TickCount
										}), true, Encoding.ASCII, 64);
									}
								}
							}
						}
						this.ctorLogString = text;
						this.ctorLogStack = base.GetStackTraceLine(4);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private string ShortDN(string orgId)
		{
			if (orgId.Contains("CN=First Organization"))
			{
				orgId = orgId.Substring(0, orgId.IndexOf("CN=First Organization")) + "<First Org>";
			}
			else if (orgId.Contains("CN=Configuration,DC="))
			{
				orgId = orgId.Substring(0, orgId.IndexOf("CN=Configuration,DC=")) + "<ConfigNC>";
			}
			else if (orgId.Contains(",DC="))
			{
				orgId = orgId.Substring(0, orgId.IndexOf(",DC=")) + ",<DomainNC>";
			}
			return orgId.Replace(',', ';');
		}

		private string GetOrgId()
		{
			string result = "<nullCurrentOrg>";
			if (this.sessionSettings.CurrentOrganizationId != null)
			{
				if (this.sessionSettings.CurrentOrganizationId == OrganizationId.ForestWideOrgId)
				{
					result = "<ForestWide>";
				}
				else
				{
					result = this.ShortDN(this.sessionSettings.CurrentOrganizationId.ConfigurationUnit.DistinguishedName);
				}
			}
			return result;
		}

		private void Dbg_LogObjectsFromEntries(ADObjectId id, ADRawEntry dummyInstance, ref bool dbg_loggedFirstOrg, ref bool dbg_loggedTenant, ref bool dbg_loggedUnknown)
		{
			try
			{
				if (this.diag_enabled && InternalDirectoryRootOrganizationCache.GetTenantCULocation(base.SessionSettings.GetAccountOrResourceForestFqdn()) != TenantCULocation.Undefined)
				{
					if (dummyInstance != null && this is IConfigurationSession && this.sessionSettings.IsGlobal && Globals.IsDatacenter)
					{
						if (id.IsDescendantOf(this.GetConfigurationUnitsRoot()))
						{
							if (dbg_loggedTenant)
							{
								return;
							}
							dbg_loggedTenant = true;
						}
						else if (id.IsDescendantOf(ADSystemConfigurationSession.GetRootOrgContainerId(base.SessionSettings.GetAccountOrResourceForestFqdn(), null, null)))
						{
							if (dbg_loggedFirstOrg)
							{
								return;
							}
							dbg_loggedFirstOrg = true;
						}
						else
						{
							if (dbg_loggedUnknown)
							{
								return;
							}
							dbg_loggedUnknown = true;
						}
						ObjectScopeAttribute objectScopeAttribute = (ObjectScopeAttribute)Attribute.GetCustomAttribute(dummyInstance.GetType(), typeof(ObjectScopeAttribute));
						bool flag = objectScopeAttribute != null && objectScopeAttribute.HasApplicableConfigScope(ConfigScopes.TenantSubTree);
						string text = dbg_loggedTenant ? "_TenantObj_" : (dbg_loggedFirstOrg ? "_FirstOrgObj_" : ("_UnknownObj_" + this.ShortDN(id.DistinguishedName)));
						string value = string.Format("{0},{1},{2},{3},{4},{5}", new object[]
						{
							this.ctorLogString,
							text,
							flag,
							dummyInstance.GetType().Name,
							base.GetStackTraceLine(4),
							this.ctorLogStack
						});
						lock (ADDataSession.diag_logWriter)
						{
							ADDataSession.diag_logWriter.WriteLine(value);
							ADDataSession.diag_logWriter.Flush();
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private DirectoryRequest PrepareAddRequest(ADObject instanceToSave, IEnumerable<PropertyDefinition> properties)
		{
			AddRequest addRequest = new AddRequest();
			addRequest.DistinguishedName = instanceToSave.Id.DistinguishedName;
			bool flag = SoftLinkMode.Disabled != this.sessionSettings.PartitionSoftLinkMode;
			ExTraceGlobals.ADSaveTracer.TraceDebug<string>((long)this.GetHashCode(), "ADSession::PrepareAddRequest - creating brand new entry {0}", addRequest.DistinguishedName);
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (adpropertyDefinition.LdapDisplayName != null && !adpropertyDefinition.IsCalculated && adpropertyDefinition != ADObjectSchema.RawName && adpropertyDefinition != ADObjectSchema.ObjectState && adpropertyDefinition != ADObjectSchema.OriginalPrimarySmtpAddress && adpropertyDefinition != ADObjectSchema.OriginalWindowsEmailAddress)
				{
					object obj = null;
					if (instanceToSave.propertyBag.TryGetField(adpropertyDefinition, ref obj) && obj != null)
					{
						if (adpropertyDefinition.IsMultivalued)
						{
							MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)instanceToSave.propertyBag[adpropertyDefinition];
							if (multiValuedPropertyBase == null || multiValuedPropertyBase.Count == 0)
							{
								continue;
							}
						}
						DirectoryAttribute directoryAttribute = new DirectoryAttribute();
						if (flag && adpropertyDefinition.IsSoftLinkAttribute)
						{
							directoryAttribute.Name = adpropertyDefinition.SoftLinkShadowProperty.LdapDisplayName;
						}
						else
						{
							directoryAttribute.Name = adpropertyDefinition.LdapDisplayName;
						}
						ExTraceGlobals.ADSaveDetailsTracer.TraceDebug<string>((long)this.GetHashCode(), "ADSession::PrepareAddRequest - adding {0}", adpropertyDefinition.LdapDisplayName);
						if (!adpropertyDefinition.IsMultivalued)
						{
							ADValueConvertor.ConvertAndAddValueToDirectoryAttribute(obj, adpropertyDefinition, directoryAttribute, flag);
							if (adpropertyDefinition.Type == typeof(ADObjectId) && adpropertyDefinition != ADObjectSchema.Id && (!flag || !adpropertyDefinition.IsSoftLinkAttribute))
							{
								num++;
							}
						}
						else
						{
							IEnumerable enumerable = (IEnumerable)obj;
							bool flag2 = adpropertyDefinition.Type == typeof(ADObjectId) && (!flag || !adpropertyDefinition.IsSoftLinkAttribute);
							foreach (object value in enumerable)
							{
								ADValueConvertor.ConvertAndAddValueToDirectoryAttribute(value, adpropertyDefinition, directoryAttribute, flag);
								if (flag2)
								{
									num++;
								}
							}
						}
						addRequest.Attributes.Add(directoryAttribute);
					}
				}
			}
			if (num > 10000)
			{
				throw new ADOperationException(DirectoryStrings.ExceptionDnLimitExceeded(num, 10000));
			}
			return addRequest;
		}

		private DirectoryRequest PrepareModifyRequest(ADRawEntry instanceToSave, IEnumerable<PropertyDefinition> properties, ADObjectId originalId)
		{
			ModifyRequest modifyRequest = new ModifyRequest();
			modifyRequest.DistinguishedName = originalId.ToDNString();
			ExTraceGlobals.ADSaveTracer.TraceDebug<string>((long)this.GetHashCode(), "ADSession::PrepareModifyRequest - updating existing entry {0}", modifyRequest.DistinguishedName);
			bool flag = SoftLinkMode.Disabled != this.sessionSettings.PartitionSoftLinkMode;
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (adpropertyDefinition.LdapDisplayName != null && !adpropertyDefinition.IsCalculated && adpropertyDefinition != ADObjectSchema.Name && adpropertyDefinition != ADObjectSchema.RawName && adpropertyDefinition != ADObjectSchema.Id && adpropertyDefinition != ADObjectSchema.ObjectState && adpropertyDefinition != ADObjectSchema.OriginalPrimarySmtpAddress && adpropertyDefinition != ADObjectSchema.OriginalWindowsEmailAddress && (instanceToSave.propertyBag.IsChanged(adpropertyDefinition) || adpropertyDefinition.IsMultivalued))
				{
					object obj = null;
					instanceToSave.propertyBag.TryGetField(adpropertyDefinition, ref obj);
					if (!adpropertyDefinition.IsMultivalued || (obj != null && ((MultiValuedPropertyBase)obj).Changed))
					{
						ExTraceGlobals.ADSaveTracer.TraceDebug<string>((long)this.GetHashCode(), "ADSession::Save - updating {0}", adpropertyDefinition.LdapDisplayName);
						DirectoryAttributeModification directoryAttributeModification = new DirectoryAttributeModification();
						if (flag && adpropertyDefinition.IsSoftLinkAttribute)
						{
							directoryAttributeModification.Name = adpropertyDefinition.SoftLinkShadowProperty.LdapDisplayName;
						}
						else
						{
							directoryAttributeModification.Name = adpropertyDefinition.LdapDisplayName;
						}
						directoryAttributeModification.Operation = DirectoryAttributeOperation.Replace;
						if (!adpropertyDefinition.IsMultivalued)
						{
							bool flag2 = adpropertyDefinition.Type == typeof(ADObjectId) && adpropertyDefinition != ADObjectSchema.Id && (!flag || !adpropertyDefinition.IsSoftLinkAttribute);
							if (obj != null)
							{
								ADValueConvertor.ConvertAndAddValueToDirectoryAttribute(obj, adpropertyDefinition, directoryAttributeModification, flag);
								if (flag2)
								{
									num++;
								}
							}
							modifyRequest.Modifications.Add(directoryAttributeModification);
						}
						else
						{
							MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)obj;
							if (multiValuedPropertyBase.Removed.Length > 0)
							{
								DirectoryAttributeModification directoryAttributeModification2 = new DirectoryAttributeModification();
								if (flag && adpropertyDefinition.IsSoftLinkAttribute)
								{
									directoryAttributeModification2.Name = adpropertyDefinition.SoftLinkShadowProperty.LdapDisplayName;
								}
								else
								{
									directoryAttributeModification2.Name = adpropertyDefinition.LdapDisplayName;
								}
								foreach (object value in multiValuedPropertyBase.Removed)
								{
									ADValueConvertor.ConvertAndAddValueToDirectoryAttribute(value, adpropertyDefinition, directoryAttributeModification2, flag);
								}
								directoryAttributeModification2.Operation = DirectoryAttributeOperation.Delete;
								modifyRequest.Modifications.Add(directoryAttributeModification2);
							}
							if (multiValuedPropertyBase.WasCleared || multiValuedPropertyBase.Added.Length > 0)
							{
								bool flag3 = adpropertyDefinition.Type == typeof(ADObjectId) && (!flag || !adpropertyDefinition.IsSoftLinkAttribute);
								foreach (object value2 in multiValuedPropertyBase.Added)
								{
									ADValueConvertor.ConvertAndAddValueToDirectoryAttribute(value2, adpropertyDefinition, directoryAttributeModification, flag);
									if (flag3)
									{
										num++;
									}
								}
								if (!multiValuedPropertyBase.WasCleared)
								{
									directoryAttributeModification.Operation = DirectoryAttributeOperation.Add;
								}
								modifyRequest.Modifications.Add(directoryAttributeModification);
							}
						}
					}
				}
			}
			if (num > 10000)
			{
				throw new ADOperationException(DirectoryStrings.ExceptionDnLimitExceeded(num, 10000));
			}
			if (modifyRequest.Modifications.Count != 0)
			{
				return modifyRequest;
			}
			return null;
		}

		public bool IsTenantIdentity(ADObjectId id)
		{
			if (ADSession.IsBoundToAdam || id.DomainId == null)
			{
				return false;
			}
			if (!Globals.IsMicrosoftHostedOnly || base.SessionSettings.PartitionId.ForestFQDN.EndsWith(id.GetPartitionId().ForestFQDN, StringComparison.OrdinalIgnoreCase))
			{
				return !id.Equals(this.GetConfigurationNamingContext()) && !id.Equals(this.GetDomainNamingContext()) && (ADSessionSettings.IsForefrontObject(id) || (id.IsDescendantOf(this.GetConfigurationUnitsRoot()) && !id.Equals(this.GetConfigurationUnitsRoot())) || (id.IsDescendantOf(this.GetHostedOrganizationsRoot()) && !id.Equals(this.GetHostedOrganizationsRoot())));
			}
			LocalizedString message = DirectoryStrings.ExceptionObjectPartitionDoesNotMatchSessionPartition(id.DistinguishedName, base.SessionSettings.PartitionId.ForestFQDN);
			if (TopologyProvider.IsAdminMode)
			{
				throw new ADExternalException(message);
			}
			throw new ADTransientException(message);
		}

		public override SecurityDescriptor ReadSecurityDescriptorBlob(ADObjectId id)
		{
			ADRawEntry adrawEntry = this.InternalRead<ADRawEntry>(id, new ADPropertyDefinition[]
			{
				ADObjectSchema.NTSecurityDescriptor
			});
			if (adrawEntry == null)
			{
				return null;
			}
			return (SecurityDescriptor)adrawEntry.propertyBag[ADObjectSchema.NTSecurityDescriptor];
		}

		public void SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd)
		{
			this.SaveSecurityDescriptor(id, sd, false);
		}

		public void SaveSecurityDescriptor(ADObjectId id, RawSecurityDescriptor sd, bool modifyOwner)
		{
			this.SaveSecurityDescriptor(new ADDataSession.UnfilterableObject(id), sd, modifyOwner);
		}

		public void SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd)
		{
			this.SaveSecurityDescriptor(obj, sd, false);
		}

		public void SaveSecurityDescriptor(ADObject obj, RawSecurityDescriptor sd, bool modifyOwner)
		{
			ADObjectId id = obj.Id;
			ADPropertyBag adpropertyBag = new ADPropertyBag(false, 2);
			adpropertyBag.SetField(ADObjectSchema.Id, id);
			adpropertyBag.SetField(ADObjectSchema.NTSecurityDescriptor, SecurityDescriptor.FromRawSecurityDescriptor(sd));
			ADRawEntry instanceToSave = new ADRawEntry(adpropertyBag);
			DirectoryRequest directoryRequest = this.PrepareModifyRequest(instanceToSave, new PropertyDefinition[]
			{
				ADObjectSchema.NTSecurityDescriptor
			}, id);
			SecurityMasks masks = modifyOwner ? (SecurityMasks.Owner | SecurityMasks.Dacl) : SecurityMasks.Dacl;
			if (sd.DiscretionaryAcl == null && modifyOwner)
			{
				masks = SecurityMasks.Owner;
			}
			directoryRequest.Controls.Add(new SecurityDescriptorFlagControl(masks));
			if (this.IsDeletedObjectId(id))
			{
				directoryRequest.Controls.Add(new ShowDeletedControl());
			}
			this.ExecuteModificationRequest(obj, directoryRequest, id);
		}

		public void SetPassword(ADObjectId id, SecureString password)
		{
			this.SetPassword(new ADDataSession.UnfilterableTenantObject(id), password);
		}

		public void SetPassword(ADObject obj, SecureString password)
		{
			byte[] array = null;
			try
			{
				array = ADSession.EncodePasswordForLdap(password);
				ADObjectId id = obj.Id;
				ADPropertyBag adpropertyBag = new ADPropertyBag(false, 2);
				adpropertyBag.SetField(ADObjectSchema.Id, id);
				adpropertyBag.SetField(ADUserSchema.UnicodePassword, array);
				ADRawEntry instanceToSave = new ADRawEntry(adpropertyBag);
				DirectoryRequest request = this.PrepareModifyRequest(instanceToSave, new PropertyDefinition[]
				{
					ADUserSchema.UnicodePassword
				}, id);
				this.ExecuteModificationRequest(obj, request, id);
			}
			finally
			{
				if (array != null)
				{
					Array.Clear(array, 0, array.Length);
				}
			}
		}

		public bool TryVerifyIsWithinScopes(ADObject entry, bool isModification, out ADScopeException exception)
		{
			return this.TryVerifyIsWithinScopes(entry, isModification, entry.OriginalId, false, out exception);
		}

		private bool TryVerifyIsWithinScopes(ADObject entry, bool isModification, ADObjectId originalId, bool emptyObjectSessionOnException, out ADScopeException exception)
		{
			ExTraceGlobals.ScopeVerificationTracer.TraceDebug((long)this.GetHashCode(), "ADSession::TryVerifyWithinScopes Entry '{0}', IsModification '{1}', OriginalId '{2}', EmptyObjectSessionOnException '{3}', ObjectState '{4}', UseConfigNC '{5}'", new object[]
			{
				entry.Id.ToDNString(),
				isModification,
				(originalId == null) ? "<null>" : originalId.ToDNString(),
				emptyObjectSessionOnException,
				entry.ObjectState,
				base.UseConfigNC
			});
			IList<ADScopeCollection> list = null;
			ADScopeCollection adscopeCollection = null;
			bool flag = false;
			ADObject adobject = entry;
			ADObjectId rootId = entry.Id;
			if (entry.ObjectState == ObjectState.Changed)
			{
				rootId = originalId;
			}
			ConfigScopes configScopes;
			ADScope readScope = this.GetReadScope(rootId, entry, out configScopes);
			if (base.UseConfigNC)
			{
				if (ConfigScopes.Server == configScopes)
				{
					IList<ADScopeCollection> configWriteScopes = base.SessionSettings.ScopeSet.GetConfigWriteScopes("Server");
					if (configWriteScopes != null)
					{
						list = configWriteScopes;
					}
					adscopeCollection = base.SessionSettings.ScopeSet.GetExclusiveConfigWriteScopes("Server");
					ExTraceGlobals.ScopeVerificationTracer.TraceDebug<string, bool, string>((long)this.GetHashCode(), "ADSession::TryVerifyWithinScopes ServerScopes are '{0}<null>', EntryIsServer '{1}', ExclusiveConfigScopes are '{2}<null>'", (configWriteScopes == null) ? string.Empty : "not ", entry is Server, (adscopeCollection == null) ? string.Empty : "not ");
					if (!(entry is Server))
					{
						flag = true;
						if (configWriteScopes != null || (adscopeCollection != null && adscopeCollection.Count > 0))
						{
							Server parentServer = ((ITopologyConfigurationSession)this).GetParentServer(entry.Id, originalId);
							if (parentServer == null)
							{
								ExTraceGlobals.ScopeVerificationTracer.TraceError<string>((long)this.GetHashCode(), "ADSession::TryVerifyWithinScopes Parent server object of '{0}' was not found.", entry.Id.ToDNString());
								exception = new ADScopeException(DirectoryStrings.ErrorNotInServerWriteScope(entry.Id.ToString()), null);
								return false;
							}
							adobject = parentServer;
						}
					}
				}
				else if (ConfigScopes.Database == configScopes)
				{
					list = (base.SessionSettings.ScopeSet.GetConfigWriteScopes("Database") ?? list);
					adscopeCollection = base.SessionSettings.ScopeSet.GetExclusiveConfigWriteScopes("Database");
				}
				IList<ADScopeCollection> list2;
				if ((list2 = list) == null)
				{
					list2 = (IList<ADScopeCollection>)new ADScopeCollection[]
					{
						new ADScopeCollection(new ADScope[]
						{
							base.SessionSettings.ScopeSet.ConfigWriteScope
						})
					};
				}
				list = list2;
			}
			else
			{
				list = this.sessionSettings.RecipientWriteScopes;
				adscopeCollection = this.sessionSettings.ExclusiveRecipientScopes;
			}
			bool flag2 = ADSession.TryVerifyIsWithinScopes(adobject, readScope, list, adscopeCollection, base.SessionSettings.ScopeSet.ValidationRules, emptyObjectSessionOnException, base.ConfigScope, out exception);
			if (flag2 && isModification && !(adobject is ADDataSession.UnfilterableObject))
			{
				ADObject adobject2 = (ADObject)adobject.GetOriginalObject();
				adobject2.m_Session = adobject.m_Session;
				flag2 = ADSession.TryVerifyIsWithinScopes(adobject2, readScope, list, adscopeCollection, null, false, out exception);
				adobject2.m_Session = null;
			}
			if (flag && !flag2)
			{
				exception = new ADScopeException(DirectoryStrings.ErrorNotInServerWriteScope(entry.Id.ToString()), null);
				return false;
			}
			return flag2;
		}

		public void VerifyIsWithinScopes(ADObject entry, bool isModification)
		{
			ADScopeException ex;
			if (!this.TryVerifyIsWithinScopes(entry, isModification, out ex))
			{
				throw ex;
			}
		}

		internal static bool IsUnfilterableObject(ADRawEntry obj)
		{
			return obj is ADDataSession.UnfilterableObject;
		}

		internal static bool IsWithinScope(ADRawEntry obj, ADScope scope)
		{
			bool flag;
			return ADDataSession.IsWithinScope(obj, scope, out flag);
		}

		internal static bool IsWithinScope(ADRawEntry obj, ADScope scope, out bool outOfScopeBecauseOfFilter)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (obj.Id == null)
			{
				throw new ArgumentException("obj.Id is null");
			}
			outOfScopeBecauseOfFilter = false;
			if (scope.Root != null && !obj.Id.IsDescendantOf(scope.Root))
			{
				ExTraceGlobals.ScopeVerificationTracer.TraceDebug<string, string, bool>(0L, "ADSession::IsWithinScope '{0}' failed the root check. ScopeRoot '{1}', IsDescendantOf '{2}'", obj.Id.ToDNString(), (scope.Root == null) ? "<null>" : scope.Root.ToDNString(), obj.Id.IsDescendantOf(scope.Root));
				return false;
			}
			if (scope.Filter != null && obj is ADDataSession.UnfilterableObject)
			{
				ExTraceGlobals.ScopeVerificationTracer.TraceDebug<string, QueryFilter>(0L, "ADSession::IsWithinScope '{0}' is Unfilterable but we have a filter. ScopeFilter '{1}'", obj.Id.ToDNString(), scope.Filter);
				outOfScopeBecauseOfFilter = true;
				return false;
			}
			bool flag = false;
			try
			{
				flag = ADDataSession.MatchesOpathFilter(obj, scope.Filter);
			}
			catch (FilterOnlyAttributesException ex)
			{
				ExTraceGlobals.ScopeVerificationTracer.TraceDebug<string>(0L, "ADSession::IsWithinScope FilterOnlyAttributesException: {0}", ex.Message);
				ADRawEntry adrawEntry;
				if (!Globals.IsMicrosoftHostedOnly && obj is ADPresentationObject)
				{
					adrawEntry = ((ADPresentationObject)obj).DataObject;
				}
				else
				{
					adrawEntry = obj;
				}
				IDirectorySession directorySession;
				if (!Globals.IsMicrosoftHostedOnly && adrawEntry is ADRecipient)
				{
					directorySession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(adrawEntry.Id), 1485, "IsWithinScope", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADDataSession.cs");
				}
				else
				{
					directorySession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(obj.Id), 1492, "IsWithinScope", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADDataSession.cs");
				}
				ADRawEntry[] array = directorySession.Find(obj.Id, QueryScope.Base, scope.Filter, null, 1, new PropertyDefinition[]
				{
					ADObjectSchema.Id
				});
				flag = (array.Length != 0);
			}
			ExTraceGlobals.ScopeVerificationTracer.TraceDebug(0L, "ADSession::IsWithinScope '{0}' is{1} within scope. ScopeRoot '{2}', ScopeFilter '{3}'", new object[]
			{
				obj.Id.ToDNString(),
				flag ? string.Empty : " NOT",
				(scope.Root == null) ? "<null>" : scope.Root.ToDNString(),
				(scope.Filter == null) ? "<null>" : scope.Filter.ToString()
			});
			return flag;
		}

		protected void Save(ADObject instanceToSave, IEnumerable<PropertyDefinition> properties)
		{
			this.Save(instanceToSave, properties, false);
		}

		protected void Save(ADObject instanceToSave, IEnumerable<PropertyDefinition> properties, bool bypassValidation)
		{
			ExTraceGlobals.ADSaveTracer.TraceDebug((long)this.GetHashCode(), "ADSession::Save - saving object of type {0} and ID {1}.", new object[]
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
			if (base.ServerSettings.WriteOriginatingChangeTimestamp)
			{
				this.ConditionallyWriteOriginatingChangeTimestamp(instanceToSave);
			}
			if (base.ServerSettings.WriteShadowProperties)
			{
				this.ConditionallyWriteShadowProperties(instanceToSave, properties);
			}
			bool flag = false;
			if (instanceToSave.ObjectState == ObjectState.New)
			{
				instanceToSave.m_Session = this;
				flag = true;
			}
			if (instanceToSave.m_Session == null)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionInvalidOperationOnObject("Save()"));
			}
			if (!instanceToSave.m_Session.SessionSettings.RootOrgId.Equals(base.SessionSettings.RootOrgId))
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionInvalidOperationOnInvalidSession("Save()"));
			}
			instanceToSave.InitializeSchema();
			if (!bypassValidation)
			{
				ValidationError[] array = instanceToSave.Validate();
				if (array.Length > 0)
				{
					if (flag)
					{
						instanceToSave.m_Session = null;
					}
					throw new DataValidationException(array[0]);
				}
			}
			DirectoryRequest directoryRequest = null;
			if (instanceToSave.ObjectState == ObjectState.New)
			{
				directoryRequest = this.PrepareAddRequest(instanceToSave, properties);
			}
			else
			{
				if (instanceToSave.ObjectState == ObjectState.Deleted)
				{
					throw new InvalidOperationException(DirectoryStrings.ExceptionObjectHasBeenDeleted);
				}
				if (instanceToSave.ObjectState == ObjectState.Changed)
				{
					directoryRequest = this.PrepareModifyRequest(instanceToSave, properties, instanceToSave.OriginalId);
				}
			}
			if (directoryRequest != null)
			{
				if (!string.IsNullOrEmpty(this.linkResolutionServer))
				{
					ExTraceGlobals.ADSaveTracer.TraceDebug<string>((long)this.GetHashCode(), "Using link resolution server {0}", this.linkResolutionServer);
					string accountOrResourceForestFqdn = base.SessionSettings.GetAccountOrResourceForestFqdn();
					if (this.linkResolutionServer.Contains(accountOrResourceForestFqdn))
					{
						directoryRequest.Controls.Add(new VerifyNameControl(this.linkResolutionServer));
					}
				}
				this.ExecuteModificationRequest(instanceToSave, directoryRequest, instanceToSave.OriginalId, flag);
			}
			if (instanceToSave.ObjectState != ObjectState.New)
			{
				ADObjectId originalId = instanceToSave.OriginalId;
				ADObjectId id = instanceToSave.Id;
				if (!originalId.DistinguishedName.Equals(id.DistinguishedName))
				{
					ModifyDNRequest modifyDNRequest = new ModifyDNRequest();
					if (!originalId.DomainId.Equals(id.DomainId))
					{
						PooledLdapConnection writableConnection = this.GetWritableConnection(null, id, instanceToSave);
						try
						{
							modifyDNRequest.Controls.Add(new CrossDomainMoveControl(writableConnection.ServerName));
							ExTraceGlobals.ADSaveTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "Adding control for cross-domain move: source DN={0}; target DN={1}; target DC={2}", originalId.DistinguishedName, id.DistinguishedName, writableConnection.ServerName);
						}
						finally
						{
							writableConnection.ReturnToPool();
						}
					}
					modifyDNRequest.NewName = id.Rdn.ToString();
					modifyDNRequest.DistinguishedName = originalId.ToDNString();
					modifyDNRequest.NewParentDistinguishedName = id.Parent.ToDNString();
					this.ExecuteModificationRequest(instanceToSave, modifyDNRequest, instanceToSave.OriginalId);
				}
			}
			else if (!string.IsNullOrEmpty(instanceToSave.DistinguishedName) && string.IsNullOrEmpty((string)instanceToSave[ADObjectSchema.RawName]))
			{
				instanceToSave[ADObjectSchema.RawName] = instanceToSave.Id.Rdn.UnescapedName;
			}
			ProvisioningCache.InvalidCacheEntry(instanceToSave);
			instanceToSave.ResetChangeTracking(true);
		}

		private bool ReplicateSingleObjectToTargetDC(ADObject instanceToReplicate, string targetServerName, ModifyRequest request)
		{
			PooledLdapConnection pooledLdapConnection = null;
			try
			{
				pooledLdapConnection = this.GetWritableConnection(targetServerName, instanceToReplicate.Id, instanceToReplicate);
				ExTraceGlobals.ADSaveTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ADSession::ReplicateSingleObjectToTargetDC using server=[{0}], sending request for DN=[{1}]", targetServerName, instanceToReplicate.Id.ToDNString());
				DirectoryResponse directoryResponse = pooledLdapConnection.SendRequest(request, LdapOperation.Write, this.clientSideSearchTimeout, base.ActivityScope, base.CallerInfo);
				if (directoryResponse.ResultCode == ResultCode.Success)
				{
					ExTraceGlobals.ADSaveTracer.TraceDebug<string>((long)this.GetHashCode(), "ADSession::ReplicateSingleObjectToTargetDC Success submiting the RSO request to server=[{0}]", targetServerName);
					return true;
				}
				ExTraceGlobals.ADSaveTracer.TraceDebug<string, ResultCode>((long)this.GetHashCode(), "ADSession::ReplicateSingleObjectToTargetDC failed submiting the RSO request to server=[{0}]. ResultCode=[{1}]", targetServerName, directoryResponse.ResultCode);
			}
			catch (SuitabilityException ex)
			{
				ExTraceGlobals.ADSaveTracer.TraceError<string, string>((long)this.GetHashCode(), "ADSession::ReplicateSingleObjectToTargetDC SuitabilityException trying to contact {0} - {1}", targetServerName, ex.Message);
			}
			catch (ADTransientException ex2)
			{
				ExTraceGlobals.ADSaveTracer.TraceError<string, string>((long)this.GetHashCode(), "ADSession::ReplicateSingleObjectToTargetDC ADTransientException trying to contact {0} - {1}", targetServerName, ex2.Message);
			}
			catch (ADExternalException ex3)
			{
				ExTraceGlobals.ADSaveTracer.TraceError<string, string>((long)this.GetHashCode(), "ADSession::ReplicateSingleObjectToTargetDC ADExternalException trying to contact {0} - {1}", targetServerName, ex3.Message);
			}
			catch (ADServerNotSuitableException ex4)
			{
				ExTraceGlobals.ADSaveTracer.TraceError<string, string>((long)this.GetHashCode(), "ADSession::ReplicateSingleObjectToTargetDC ADServerNotSuitableException trying to contact {0} - {1}", targetServerName, ex4.Message);
			}
			catch (DirectoryException ex5)
			{
				ADErrorRecord aderrorRecord = pooledLdapConnection.AnalyzeDirectoryError(ex5);
				ExTraceGlobals.ADSaveTracer.TraceError((long)this.GetHashCode(), "ADSession::ReplicateSingleObjectToTargetDC Caugth {0} trying to contact {1} - LdapError {2}(0x{3}), {4}, JetError {5}(0x{6})", new object[]
				{
					ex5.GetType(),
					targetServerName,
					(int)aderrorRecord.LdapError,
					((int)aderrorRecord.LdapError).ToString("X"),
					aderrorRecord.Message,
					aderrorRecord.JetError,
					aderrorRecord.JetError.ToString("X")
				});
			}
			finally
			{
				if (pooledLdapConnection != null)
				{
					pooledLdapConnection.ReturnToPool();
				}
			}
			return false;
		}

		private ModifyRequest PrepareRsoModifyRequest(ADObject instanceToReplicate)
		{
			ExchangeTopology rsoTopology = ExchangeTopology.RsoTopology;
			ModifyRequest modifyRequest = new ModifyRequest();
			DirectoryAttributeModification directoryAttributeModification = new DirectoryAttributeModification();
			directoryAttributeModification.Name = "replicateSingleObject";
			directoryAttributeModification.Operation = DirectoryAttributeOperation.Replace;
			ADServer adServer = rsoTopology.GetAdServer(instanceToReplicate.OriginatingServer);
			if (adServer == null)
			{
				return null;
			}
			directoryAttributeModification.Add(string.Format("CN=NTDS Settings,{0}:{1}", adServer.DistinguishedName, instanceToReplicate.DistinguishedName));
			modifyRequest.Modifications.Add(directoryAttributeModification);
			return modifyRequest;
		}

		public bool ReplicateSingleObjectToTargetDC(ADObject instanceToReplicate, string targetServerName)
		{
			ExTraceGlobals.ADSaveTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ADSession::ReplicateSingleObjectToTargetDC - saving object. ADObject_type=[{0}] ADObject_ID=[{1}]", (instanceToReplicate == null) ? "<null>" : instanceToReplicate.GetType().Name, (instanceToReplicate == null) ? "<null>" : instanceToReplicate.Id.ToString());
			if (string.IsNullOrEmpty(targetServerName))
			{
				throw new ArgumentNullException("targetServerName");
			}
			if (instanceToReplicate == null)
			{
				throw new ArgumentNullException("instanceToReplicate");
			}
			if (instanceToReplicate.Id == null)
			{
				throw new ArgumentNullException("instanceToReplicate.Id");
			}
			if (string.IsNullOrEmpty(instanceToReplicate.OriginatingServer))
			{
				throw new ArgumentNullException("instanceToReplicate.OriginatingServer");
			}
			ModifyRequest modifyRequest = this.PrepareRsoModifyRequest(instanceToReplicate);
			return modifyRequest != null && this.ReplicateSingleObjectToTargetDC(instanceToReplicate, targetServerName, modifyRequest);
		}

		public string[] ReplicateSingleObject(ADObject instanceToReplicate, ADObjectId[] sites)
		{
			ExTraceGlobals.ADSaveTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ADSession::ReplicateSingleObject - saving object. ADObject_type=[{0}] ADObject_ID=[{1}]", (instanceToReplicate == null) ? "<null>" : instanceToReplicate.GetType().Name, (instanceToReplicate == null) ? "<null>" : instanceToReplicate.Id.ToString());
			if (sites == null)
			{
				throw new ArgumentNullException("sites");
			}
			if (instanceToReplicate == null)
			{
				throw new ArgumentNullException("instanceToReplicate");
			}
			if (instanceToReplicate.Id == null)
			{
				throw new ArgumentNullException("instanceToReplicate.Id");
			}
			if (string.IsNullOrEmpty(instanceToReplicate.OriginatingServer))
			{
				throw new ArgumentNullException("instanceToReplicate.OriginatingServer");
			}
			string[] array = new string[sites.Length];
			ExchangeTopology rsoTopology = ExchangeTopology.RsoTopology;
			ModifyRequest modifyRequest = this.PrepareRsoModifyRequest(instanceToReplicate);
			if (modifyRequest != null)
			{
				for (int i = 0; i < sites.Length; i++)
				{
					ADObjectId adobjectId = sites[i];
					ReadOnlyCollection<ADServer> readOnlyCollection = rsoTopology.ADServerFromSite(adobjectId.DistinguishedName);
					foreach (ADServer adserver in readOnlyCollection)
					{
						if (this.ReplicateSingleObjectToTargetDC(instanceToReplicate, adserver.DnsHostName, modifyRequest))
						{
							array[i] = adserver.DnsHostName;
							ExTraceGlobals.ADSaveTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ADSession::ReplicateSingleObject - selected DC=[{0}] for site=[{1}].", array[i], adobjectId.Name);
							break;
						}
					}
				}
			}
			return array;
		}

		public ADOperationResultWithData<TResult>[] RunAgainstAllDCsInSite<TResult>(ADObjectId siteId, Func<TResult> methodToCall) where TResult : class
		{
			if (siteId == null)
			{
				throw new ArgumentNullException("siteId");
			}
			ExchangeTopology rsoTopology = ExchangeTopology.RsoTopology;
			ReadOnlyCollection<ADServer> readOnlyCollection = rsoTopology.ADServerFromSite(siteId.DistinguishedName);
			List<ADOperationResultWithData<TResult>> list = new List<ADOperationResultWithData<TResult>>(readOnlyCollection.Count);
			string domainController = base.DomainController;
			try
			{
				foreach (ADServer adserver in readOnlyCollection)
				{
					base.DomainController = adserver.DnsHostName;
					TResult result = default(TResult);
					ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						result = methodToCall();
					}, 0);
					list.Add(new ADOperationResultWithData<TResult>(adserver.DnsHostName, result, adoperationResult.ErrorCode, adoperationResult.Exception));
				}
			}
			finally
			{
				base.DomainController = domainController;
			}
			return list.ToArray();
		}

		protected void Delete(ADObject instanceToDelete)
		{
			this.Delete(instanceToDelete, false);
		}

		protected void Delete(ADObject instanceToDelete, bool enableTreeDelete)
		{
			ExTraceGlobals.ADDeleteTracer.TraceDebug<bool, string, object>((long)this.GetHashCode(), "ADSession::Delete - deleting (enableTreeDelete = {0}) object of type {1} and ID {2}.", enableTreeDelete, (instanceToDelete == null) ? "<null>" : instanceToDelete.GetType().Name, (instanceToDelete == null) ? "<null>" : instanceToDelete.Id);
			if (instanceToDelete == null)
			{
				throw new ArgumentNullException("instanceToDelete");
			}
			if (instanceToDelete.Id == null)
			{
				throw new ArgumentNullException("instanceToDelete.Id");
			}
			if (this.readOnly)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionInvalidOperationOnReadOnlySession("Delete()"));
			}
			if (instanceToDelete.m_Session != null && instanceToDelete.m_Session.SessionSettings.RootOrgId != base.SessionSettings.RootOrgId)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionInvalidOperationOnInvalidSession("Delete()"));
			}
			if (instanceToDelete.ObjectState == ObjectState.Deleted)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionObjectHasBeenDeleted);
			}
			DeleteRequest deleteRequest = new DeleteRequest(instanceToDelete.Id.ToDNString());
			if (enableTreeDelete)
			{
				deleteRequest.Controls.Add(new TreeDeleteControl());
			}
			this.ExecuteModificationRequest(instanceToDelete, deleteRequest, instanceToDelete.OriginalId);
			instanceToDelete.MarkAsDeleted();
			ProvisioningCache.InvalidCacheEntry(instanceToDelete);
		}

		public void UnsafeExecuteModificationRequest(DirectoryRequest request, ADObjectId rootId)
		{
			this.ExecuteModificationRequest(null, request, rootId, false, true);
		}

		private void ExecuteModificationRequest(ADObject entry, DirectoryRequest request, ADObjectId originalId)
		{
			this.ExecuteModificationRequest(entry, request, originalId, false);
		}

		private void ExecuteModificationRequest(ADObject entry, DirectoryRequest request, ADObjectId originalId, bool emptyObjectSessionOnException)
		{
			this.ExecuteModificationRequest(entry, request, originalId, emptyObjectSessionOnException, false);
		}

		private void ExecuteModificationRequest(ADObject entry, DirectoryRequest request, ADObjectId originalId, bool emptyObjectSessionOnException, bool isSync)
		{
			RetryManager retryManager = new RetryManager();
			string text = null;
			IDirectorySession session = null;
			bool isModification = request is ModifyRequest || request is ModifyDNRequest;
			if (!isSync)
			{
				ADScopeException ex;
				if (!this.sessionSettings.IsGlobal && !this.TryVerifyIsWithinScopes(entry, isModification, originalId, emptyObjectSessionOnException, out ex))
				{
					throw ex;
				}
				this.CheckIsObjectScopedToWritableTenant(entry.Id);
				if (entry.MaximumSupportedExchangeObjectVersion.IsOlderThan(entry.ExchangeVersion))
				{
					throw new DataValidationException(new PropertyValidationError(DataStrings.ErrorCannotSaveBecauseTooNew(entry.ExchangeVersion, entry.MaximumSupportedExchangeObjectVersion), ADObjectSchema.ExchangeVersion, entry.ExchangeVersion));
				}
				session = entry.m_Session;
			}
			for (;;)
			{
				PooledLdapConnection writableConnection = this.GetWritableConnection(text, originalId, entry);
				try
				{
					try
					{
						ExTraceGlobals.ADSaveTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "ADSession::ExecuteModificationRequest using {0} - Sending {1} request for {2}", writableConnection.ADServerInfo.FqdnPlusPort, request.GetType().Name, originalId.ToDNString());
						DirectoryResponse directoryResponse = writableConnection.SendRequest(request, LdapOperation.Write, this.clientSideSearchTimeout, base.ActivityScope, base.CallerInfo);
						if (directoryResponse.Referral == null || directoryResponse.Referral.Length <= 0)
						{
							if (entry != null)
							{
								entry.OriginatingServer = writableConnection.ServerName;
								entry.m_Session = session;
							}
							this.UpdateServerSettings(writableConnection);
							break;
						}
						string dnsSafeHost = directoryResponse.Referral[0].DnsSafeHost;
						text = TopologyProvider.GetInstance().GetServerFromDomainDN(NativeHelpers.DistinguishedNameFromCanonicalName(dnsSafeHost), base.NetworkCredential).Fqdn;
						ExTraceGlobals.ADSaveTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Got a referral to domain {0}, chose DC {1}", dnsSafeHost, text);
					}
					catch (DirectoryException de)
					{
						retryManager.Tried(writableConnection.ServerName);
						if (emptyObjectSessionOnException && entry != null)
						{
							entry.m_Session = null;
						}
						this.AnalyzeDirectoryError(writableConnection, request, de, retryManager.TotalRetries, retryManager[writableConnection.ServerName]);
					}
					continue;
				}
				finally
				{
					writableConnection.ReturnToPool();
				}
				break;
			}
		}

		private void RehomeAndThrowWrongTenantException(string orgDN, bool rehome, bool isArriving = false)
		{
			if (rehome && OrganizationId.ForestWideOrgId != base.SessionSettings.CurrentOrganizationId)
			{
				this.RehomeSession();
			}
			if (isArriving)
			{
				throw new TenantIsArrivingException(orgDN);
			}
			throw new TenantIsRelocatedException(orgDN);
		}

		private void CheckIsObjectScopedToWritableTenant(ADObjectId id)
		{
			if (!Globals.IsMicrosoftHostedOnly)
			{
				return;
			}
			if (!this.IsTenantIdentity(id))
			{
				return;
			}
			if (this.IsTenantRelocationAllowedInForest() && !base.SessionSettings.RetiredTenantModificationAllowed)
			{
				if (this.configScope != ConfigScopes.AllTenants)
				{
					if (OrganizationId.ForestWideOrgId.Equals(base.SessionSettings.CurrentOrganizationId))
					{
						return;
					}
				}
				try
				{
					TenantRelocationState tenantRelocationState;
					bool flag;
					if (TenantRelocationStateCache.TryGetTenantRelocationStateByObjectId(id, out tenantRelocationState, out flag))
					{
						if (flag && tenantRelocationState.SourceForestState == TenantRelocationStatus.Retired)
						{
							this.RehomeAndThrowWrongTenantException(id.DistinguishedName, false, false);
						}
						else
						{
							if (flag && tenantRelocationState.SourceForestState == TenantRelocationStatus.Lockdown && tenantRelocationState.SourceForestFQDN == base.SessionSettings.PartitionId.ForestFQDN)
							{
								throw new TenantIsLockedDownForRelocationException(id.DistinguishedName);
							}
							if (!flag && tenantRelocationState.TargetForestState == RelocationStatusDetailsDestination.Arriving)
							{
								throw new TenantIsArrivingException(id.DistinguishedName);
							}
						}
					}
				}
				catch (CannotResolveTenantNameException)
				{
				}
			}
		}

		private bool IsTenantRelocationAllowedInForest()
		{
			return ForestTenantRelocationsCache.IsTenantRelocationAllowed(base.SessionSettings.PartitionId.ForestFQDN);
		}

		private void ConditionallyWriteOriginatingChangeTimestamp(ADObject entry)
		{
			if (entry.ObjectState == ObjectState.Unchanged)
			{
				return;
			}
			IOriginatingChangeTimestamp originatingChangeTimestamp = entry as IOriginatingChangeTimestamp;
			if (originatingChangeTimestamp != null && entry.IsChanged(ADOrgPersonSchema.Manager))
			{
				originatingChangeTimestamp.LastExchangeChangedTime = new DateTime?((DateTime)ExDateTime.UtcNow);
			}
		}

		private void ConditionallyWriteShadowProperties(ADObject entry, IEnumerable<PropertyDefinition> properties)
		{
			if (entry.ObjectState == ObjectState.Unchanged)
			{
				return;
			}
			if (!(entry is ADUser))
			{
				return;
			}
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				object value = null;
				if (adpropertyDefinition.ShadowProperty != null && (entry.ObjectState == ObjectState.New || entry.IsChanged(adpropertyDefinition)) && entry.propertyBag.TryGetField(adpropertyDefinition, ref value))
				{
					entry.propertyBag[adpropertyDefinition.ShadowProperty] = value;
				}
			}
		}

		public TResult[] Find<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults) where TResult : ADObject, new()
		{
			return this.Find<TResult>(rootId, scope, filter, sortBy, maxResults, null, false);
		}

		protected TResult[] Find<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties, bool includeDeletedObjects = false) where TResult : IConfigurable, new()
		{
			return this.InternalFind<TResult>(rootId, null, null, scope, filter, sortBy, maxResults, properties, includeDeletedObjects);
		}

		public ADRawEntry[] Find(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			return this.Find<ADRawEntry>(rootId, scope, filter, sortBy, maxResults, properties, false);
		}

		public Result<ADRawEntry>[] FindByObjectGuids(Guid[] objectGuids, params PropertyDefinition[] properties)
		{
			if (properties == null)
			{
				properties = new PropertyDefinition[1];
			}
			else
			{
				Array.Resize<PropertyDefinition>(ref properties, properties.Length + 1);
			}
			properties[properties.Length - 1] = ADObjectSchema.Guid;
			return this.FindByObjectGuids<ADRawEntry>(objectGuids, properties);
		}

		public Result<ADRawEntry>[] FindByADObjectIds(ADObjectId[] ids, params PropertyDefinition[] properties)
		{
			if (properties == null)
			{
				properties = new PropertyDefinition[1];
			}
			else
			{
				Array.Resize<PropertyDefinition>(ref properties, properties.Length + 1);
			}
			properties[properties.Length - 1] = ADObjectSchema.Id;
			return this.FindByADObjectIds<ADRawEntry>(ids, properties);
		}

		public Result<ADRawEntry>[] FindByCorrelationIds(Guid[] correlationIds, ADObjectId configUnit, params PropertyDefinition[] properties)
		{
			if (properties == null)
			{
				properties = new PropertyDefinition[1];
			}
			else
			{
				Array.Resize<PropertyDefinition>(ref properties, properties.Length + 1);
			}
			properties[properties.Length - 1] = ADObjectSchema.CorrelationIdRaw;
			return this.ReadMultiple<Guid, ADRawEntry>(correlationIds, null, new Converter<Guid, QueryFilter>(ADDataSession.QueryFilterFromCorrelationId), new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ConfigurationUnit, configUnit), new ADDataSession.HashInserter<ADRawEntry>(ADDataSession.FindByCorrelationIdsHashInserter<ADRawEntry>), new ADDataSession.HashLookup<Guid, ADRawEntry>(ADDataSession.FindByCorrelationIdsHashLookup<ADRawEntry>), properties, true, true);
		}

		protected Result<TData>[] FindByObjectGuids<TData>(Guid[] objectGuids, IEnumerable<PropertyDefinition> properties) where TData : ADRawEntry, new()
		{
			if (objectGuids == null)
			{
				throw new ArgumentNullException("objectGuids");
			}
			if (objectGuids.Length == 0)
			{
				return new Result<TData>[0];
			}
			return this.ReadMultiple<Guid, TData>(objectGuids, new Converter<Guid, QueryFilter>(ADDataSession.QueryFilterFromObjectGuid), new ADDataSession.HashInserter<TData>(ADDataSession.FindByObjectGuidsHashInserter<TData>), new ADDataSession.HashLookup<Guid, TData>(ADDataSession.FindByObjectGuidsHashLookup<TData>), properties);
		}

		public Result<TData>[] FindByADObjectIds<TData>(ADObjectId[] ids) where TData : ADObject, new()
		{
			return this.FindByADObjectIds<TData>(ids, null);
		}

		protected Result<TData>[] FindByADObjectIds<TData>(ADObjectId[] ids, IEnumerable<PropertyDefinition> properties) where TData : ADRawEntry, new()
		{
			if (ids == null)
			{
				throw new ArgumentNullException("objectGuids");
			}
			if (ids.Length == 0)
			{
				return new Result<TData>[0];
			}
			return this.ReadMultiple<ADObjectId, TData>(ids, new Converter<ADObjectId, QueryFilter>(ADDataSession.QueryFilterFromADObjectId), new ADDataSession.HashInserter<TData>(ADDataSession.FindByADObjectIdsHashInserter<TData>), new ADDataSession.HashLookup<ADObjectId, TData>(ADDataSession.FindByADObjectIdsHashLookup<TData>), properties);
		}

		public Result<ADRawEntry>[] FindByExchangeLegacyDNs(string[] exchangeLegacyDNs, params PropertyDefinition[] properties)
		{
			if (exchangeLegacyDNs == null)
			{
				throw new ArgumentNullException("exchangeLegacyDNs");
			}
			if (exchangeLegacyDNs.Length == 0)
			{
				return new Result<ADRawEntry>[0];
			}
			if (properties == null)
			{
				properties = new PropertyDefinition[3];
			}
			else
			{
				Array.Resize<PropertyDefinition>(ref properties, properties.Length + 3);
			}
			properties[properties.Length - 1] = ADObjectSchema.Guid;
			properties[properties.Length - 2] = ADRecipientSchema.EmailAddresses;
			properties[properties.Length - 3] = SharedPropertyDefinitions.ExchangeLegacyDN;
			return this.FindByExchangeLegacyDNs<ADRawEntry>(exchangeLegacyDNs, properties);
		}

		protected Result<TData>[] FindByExchangeLegacyDNs<TData>(string[] exchangeLegacyDNs, IEnumerable<PropertyDefinition> properties) where TData : ADRawEntry, new()
		{
			if (exchangeLegacyDNs == null)
			{
				throw new ArgumentNullException("objectGuids");
			}
			if (exchangeLegacyDNs.Length == 0)
			{
				return new Result<TData>[0];
			}
			return this.ReadMultiple<string, TData>(exchangeLegacyDNs, new Converter<string, QueryFilter>(ADDataSession.QueryFilterFromExchangeLegacyDN), new ADDataSession.HashInserter<TData>(ADDataSession.FindByExchangeLegacyDNsHashInserter<TData>), new ADDataSession.HashLookup<string, TData>(ADDataSession.FindByExchangeLegacyDNsHashLookup<TData>), properties);
		}

		public ADObjectId GetConfigurationNamingContext()
		{
			return this.GetNamingContext(ADSession.ADNamingContext.Config);
		}

		public ADObjectId GetConfigurationUnitsRoot()
		{
			ADObjectId adobjectId = ADSession.IsTenantConfigInDomainNC(base.SessionSettings.GetAccountOrResourceForestFqdn()) ? this.GetRootDomainNamingContext() : ADSession.GetMicrosoftExchangeRoot(this.GetConfigurationNamingContext());
			return adobjectId.GetChildId("CN", ADObject.ConfigurationUnits);
		}

		public ADObjectId GetDomainNamingContext()
		{
			return this.GetNamingContext(ADSession.ADNamingContext.Domain);
		}

		public ADObjectId GetHostedOrganizationsRoot()
		{
			return ADSession.GetHostedOrganizationsRoot(base.SessionSettings.GetAccountOrResourceForestFqdn());
		}

		public ADObjectId GetRootDomainNamingContext()
		{
			return this.GetNamingContext(ADSession.ADNamingContext.RootDomain);
		}

		public ADObjectId GetSchemaNamingContext()
		{
			return this.GetNamingContext(ADSession.ADNamingContext.Schema);
		}

		public bool GetSchemaAndApplyFilter(ADRawEntry adRawEntry, ADScope scope, out ADObject dummyObject, out string[] ldapAttributes, ref QueryFilter filter, ref IEnumerable<PropertyDefinition> properties)
		{
			bool flag = adRawEntry is MiniObject;
			bool flag2 = adRawEntry is ReducedRecipient;
			dummyObject = (adRawEntry as ADObject);
			bool result = !flag && !flag2 && null == dummyObject;
			if (dummyObject != null && properties == null)
			{
				properties = dummyObject.Schema.AllProperties;
				ldapAttributes = dummyObject.Schema.LdapAttributes;
			}
			else
			{
				List<PropertyDefinition> list = new List<PropertyDefinition>(properties);
				if (flag)
				{
					list.AddRange(dummyObject.Schema.AllProperties);
				}
				HashSet<PropertyDefinition> hashSet = new HashSet<PropertyDefinition>(list.Count);
				foreach (PropertyDefinition propertyDefinition in list)
				{
					hashSet.TryAdd(propertyDefinition);
					ProviderPropertyDefinition providerPropertyDefinition = propertyDefinition as ProviderPropertyDefinition;
					if (providerPropertyDefinition != null && providerPropertyDefinition.IsCalculated)
					{
						foreach (ProviderPropertyDefinition item in providerPropertyDefinition.SupportingProperties)
						{
							hashSet.TryAdd(item);
						}
					}
				}
				hashSet.TryAdd(ADObjectSchema.ExchangeVersion);
				properties = hashSet;
				ldapAttributes = ADObjectSchema.ADPropertyCollectionToLdapAttributes(properties, this.GetHashCode());
			}
			if (base.ExclusiveLdapAttributes != null)
			{
				ldapAttributes = ldapAttributes.Except(base.ExclusiveLdapAttributes).ToArray<string>();
				ExTraceGlobals.ADFindTracer.TraceDebug<string>((long)this.GetHashCode(), "Following LDAP attributes are excluded: {0}", string.Join(", ", base.ExclusiveLdapAttributes));
			}
			filter = this.ApplyDefaultFilters(filter, scope, dummyObject, true);
			return result;
		}

		public void CheckFilterForUnsafeIdentity(QueryFilter filter)
		{
			if (!this.isRehomed)
			{
				return;
			}
			bool flag;
			string filter2 = LdapFilterBuilder.LdapFilterFromQueryFilter(filter, true, this.sessionSettings.PartitionSoftLinkMode, this.sessionSettings.IsTenantScoped, out flag);
			if (flag)
			{
				throw new UnsafeIdentityFilterNotAllowedException(filter2, base.SessionSettings.CurrentOrganizationId.ToString());
			}
		}

		private TResult[] InternalFind<TResult>(ADObjectId rootId, string optionalBaseDN, ADObjectId readId, QueryScope scope, QueryFilter filter, SortBy sortBy, int maxResults, IEnumerable<PropertyDefinition> properties, bool includeDeletedObjects) where TResult : IConfigurable, new()
		{
			if (!typeof(ADRawEntry).IsAssignableFrom(typeof(TResult)))
			{
				throw new InvalidOperationException(DirectoryStrings.ErrorMustBeADRawEntry);
			}
			TResult tresult = (default(TResult) == null) ? Activator.CreateInstance<TResult>() : default(TResult);
			this.CheckFilterForUnsafeIdentity(filter);
			bool flag = readId != null || scope == QueryScope.Base;
			ConfigScopes configScopes;
			ADScope readScope = this.GetReadScope(rootId, (ADRawEntry)((object)tresult), this.IsWellKnownGuidDN(optionalBaseDN), out configScopes);
			ADObject adobject;
			string[] attributeList;
			bool schemaAndApplyFilter = this.GetSchemaAndApplyFilter((ADRawEntry)((object)tresult), readScope, out adobject, out attributeList, ref filter, ref properties);
			this.UpdateFilterforSoftDeletedSearch((ADRawEntry)((object)tresult), ref filter);
			string ldapFilter = LdapFilterBuilder.LdapFilterFromQueryFilter(filter, false, this.sessionSettings.PartitionSoftLinkMode, base.SessionSettings.IsTenantScoped);
			if (this.sessionSettings.IncludeSoftDeletedObjects || this.sessionSettings.IncludeInactiveMailbox)
			{
				this.enforceContainerizedScoping = false;
			}
			SearchRequest searchRequest = new SearchRequest(null, ldapFilter, (SearchScope)scope, attributeList);
			ExtendedDNControl control = new ExtendedDNControl(ExtendedDNFlag.StandardString);
			searchRequest.Controls.Add(control);
			SortRequestControl sortRequestControl = null;
			ADPropertyDefinition adpropertyDefinition = null;
			if (sortBy != null)
			{
				adpropertyDefinition = (ADPropertyDefinition)sortBy.ColumnDefinition;
				sortRequestControl = new SortRequestControl(adpropertyDefinition.LdapDisplayName, LcidMapper.OidFromLcid(this.lcid), sortBy.SortOrder == SortOrder.Descending);
				sortRequestControl.IsCritical = true;
				searchRequest.Controls.Add(sortRequestControl);
				ExTraceGlobals.ADFindTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "ADSession::Find - Sort on {0}, {1} using rule {2})", sortRequestControl.SortKeys[0].AttributeName, sortRequestControl.SortKeys[0].ReverseOrder ? "reverse order (descending)" : "regular  order (ascending)", sortRequestControl.SortKeys[0].MatchingRule);
			}
			if (TopologyProvider.IsAdamTopology() && this.useGlobalCatalog)
			{
				searchRequest.Controls.Add(new SearchOptionsControl(System.DirectoryServices.Protocols.SearchOption.PhantomRoot));
			}
			if (schemaAndApplyFilter)
			{
				int num = 0;
				foreach (PropertyDefinition propertyDefinition in properties)
				{
					if (propertyDefinition == ADObjectSchema.NTSecurityDescriptor)
					{
						searchRequest.Controls.Add(new SecurityDescriptorFlagControl(SecurityMasks.Owner | SecurityMasks.Group | SecurityMasks.Dacl));
						break;
					}
					if (++num >= 2)
					{
						break;
					}
				}
			}
			bool flag2 = false;
			if (includeDeletedObjects)
			{
				flag2 = includeDeletedObjects;
				ExTraceGlobals.ADFindTracer.TraceDebug((long)this.GetHashCode(), "ADSession::Find adding ShowDeletedControl because includeDeletedObjects is TRUE.");
			}
			else if ((rootId != null && rootId.IsDescendantOf(ADDataSession.GetDomainDeletedObjectsContainer(this.GetConfigurationNamingContext()))) || (readId != null && readId.IsDescendantOf(ADDataSession.GetDomainDeletedObjectsContainer(this.GetConfigurationNamingContext()))))
			{
				flag2 = true;
				ExTraceGlobals.ADFindTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ADSession::Find adding ShowDeletedControl because rootId {0} or readId {1} are descendant of the Config Deleted Objects Container.", (rootId != null) ? rootId.ToDNString() : "<null>", (readId != null) ? readId.ToDNString() : "<null>");
			}
			else if ((rootId != null && rootId.DomainId != null && rootId.IsDescendantOf(ADDataSession.GetDomainDeletedObjectsContainer(rootId.DomainId))) || (readId != null && readId.DomainId != null && readId.IsDescendantOf(ADDataSession.GetDomainDeletedObjectsContainer(readId.DomainId))))
			{
				flag2 = true;
				ExTraceGlobals.ADFindTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ADSession::Find adding ShowDeletedControl because rootId {0} or readId {1} are descendant of the Domain Deleted Objects Container.", (rootId != null) ? rootId.ToDNString() : "<null>", (readId != null) ? readId.ToDNString() : "<null>");
			}
			if (flag2)
			{
				if (!base.SessionSettings.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId) && (base.ConfigScope == ConfigScopes.TenantLocal || base.ConfigScope == ConfigScopes.AllTenants) && !this.IsDeletedObjectId(rootId))
				{
					throw new InvalidOperationException("Deleted objects will not be found with a session scoped to TenantLocal or AllTenants, unless search is scoped to Deleted Objects container.");
				}
				searchRequest.Controls.Add(new ShowDeletedControl());
			}
			searchRequest.TimeLimit = ((base.ServerTimeout != null) ? base.ServerTimeout.Value : ADDataSession.OptimisticTimeout);
			if (maxResults > 0)
			{
				searchRequest.SizeLimit = maxResults;
			}
			SearchResponse searchResponse = null;
			string text = null;
			RetryManager retryManager = new RetryManager();
			for (;;)
			{
				PooledLdapConnection readConnection = this.GetReadConnection(null, optionalBaseDN, ref rootId, (ADRawEntry)((object)tresult));
				Guid serviceProviderRequestId = Microsoft.Exchange.Diagnostics.Trace.TraceCasStart(CasTraceEventType.ActiveDirectory);
				try
				{
					try
					{
						if (!flag && !(tresult is TenantRelocationSyncObject) && base.EnforceContainerizedScoping && searchRequest.Scope == SearchScope.Subtree)
						{
							ADObjectId domainId = rootId.DomainId;
							if (domainId != null)
							{
								ADObjectId childId = domainId.GetChildId("OU", "Microsoft Exchange Hosted Organizations");
								ADObjectId parent = rootId.Parent;
								if (childId != null && parent != null && ADObjectId.Equals(childId, parent))
								{
									searchRequest.Scope = SearchScope.OneLevel;
								}
							}
						}
						searchRequest.DistinguishedName = ((optionalBaseDN == null) ? rootId.ToDNString() : optionalBaseDN);
						ExTraceGlobals.ADFindTracer.TraceDebug((long)this.GetHashCode(), "ADSession::Find using {0} - LDAP search from {1}, scope {2}, filter {3}, sizeLimit {4}, timeout {5}, ShowDeletedControl {6}", new object[]
						{
							readConnection.ADServerInfo.FqdnPlusPort,
							searchRequest.DistinguishedName,
							(int)searchRequest.Scope,
							searchRequest.Filter,
							searchRequest.SizeLimit,
							searchRequest.TimeLimit,
							flag2 ? "added" : "not added"
						});
						text = readConnection.ServerName;
						searchResponse = (SearchResponse)readConnection.SendRequest(searchRequest, flag ? LdapOperation.Read : LdapOperation.Search, this.clientSideSearchTimeout, base.ActivityScope, base.CallerInfo);
						this.UpdateServerSettings(readConnection);
						break;
					}
					catch (DirectoryException ex)
					{
						if (readConnection.IsResultCode(ex, ResultCode.NoSuchObject))
						{
							ExTraceGlobals.ADFindTracer.TraceWarning<string, object>((long)this.GetHashCode(), "NoSuchObject caught when searching from {0} with filter {1}", searchRequest.DistinguishedName, searchRequest.Filter);
							if (scope == QueryScope.Base && this.useConfigNC)
							{
								ADProviderPerf.UpdateProcessCounter(Counter.ProcessRateNotFoundConfigReads, UpdateType.Update, 1U);
								Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_BASE_CONFIG_SEARCH_FAILED, rootId.ToString(), new object[]
								{
									rootId.ToDNString(),
									text
								});
							}
							return new TResult[0];
						}
						if (readConnection.IsResultCode(ex, ResultCode.SizeLimitExceeded))
						{
							ExTraceGlobals.ADFindTracer.TraceWarning<string, object, int>((long)this.GetHashCode(), "SizeLimitExceeded caught when searching from {0} with filter {1}, maxresults={2}", searchRequest.DistinguishedName, searchRequest.Filter, maxResults);
							searchResponse = (SearchResponse)((DirectoryOperationException)ex).Response;
							this.UpdateServerSettings(readConnection);
							if (base.LogSizeLimitExceededEvent || maxResults == 0)
							{
								Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_LDAP_SIZELIMIT_EXCEEDED, (string)searchRequest.Filter, new object[]
								{
									readConnection.ServerName,
									searchResponse.Entries.Count,
									searchRequest.DistinguishedName,
									searchRequest.Filter,
									(int)searchRequest.Scope
								});
							}
							if (maxResults > 0)
							{
								break;
							}
							retryManager.Tried(readConnection.ServerName);
							this.AnalyzeDirectoryError(readConnection, searchRequest, ex, retryManager.TotalRetries, retryManager[readConnection.ServerName]);
						}
						else if (readConnection.IsResultCode(ex, ResultCode.UnavailableCriticalExtension) && sortRequestControl != null && adpropertyDefinition != null)
						{
							ExTraceGlobals.ADFindTracer.TraceWarning<int, int>((long)this.GetHashCode(), "UnavailableCriticalExtension error caught when performing a sorted search using LCID 0x{0:X}. Falling back to US English 0x{1:X}", this.lcid, LcidMapper.DefaultLcid);
							this.lcid = LcidMapper.DefaultLcid;
							searchRequest.Controls.Remove(sortRequestControl);
							sortRequestControl = new SortRequestControl(adpropertyDefinition.LdapDisplayName, LcidMapper.OidFromLcid(this.lcid), sortBy.SortOrder == SortOrder.Descending);
							sortRequestControl.IsCritical = false;
							searchRequest.Controls.Add(sortRequestControl);
						}
						else if (readConnection.IsResultCode(ex, ResultCode.TimeLimitExceeded))
						{
							ExTraceGlobals.ADFindTracer.TraceWarning<string, object, TimeSpan>((long)this.GetHashCode(), "TimeLimitExceeded caught when searching from {0} with filter {1}, will use Pessimistic timeout of {2}", searchRequest.DistinguishedName, searchRequest.Filter, ADDataSession.PessimisticTimeout);
							searchRequest.TimeLimit = ADDataSession.PessimisticTimeout;
							retryManager.Tried(readConnection.ServerName);
							this.AnalyzeDirectoryError(readConnection, searchRequest, ex, retryManager.TotalRetries, retryManager[readConnection.ServerName]);
						}
						else
						{
							retryManager.Tried(readConnection.ServerName);
							this.AnalyzeDirectoryError(readConnection, searchRequest, ex, retryManager.TotalRetries, retryManager[readConnection.ServerName]);
						}
					}
					continue;
				}
				finally
				{
					bool isSnapshotInProgress = PerformanceContext.Current.IsSnapshotInProgress;
					bool flag3 = ETWTrace.ShouldTraceCasStop(serviceProviderRequestId);
					if (isSnapshotInProgress || flag3)
					{
						StringBuilder stringBuilder = new StringBuilder(128);
						if (flag)
						{
							stringBuilder.Append("readId : ");
							stringBuilder.Append(readId);
							stringBuilder.Append(", ");
						}
						stringBuilder.Append("scope: ").Append(searchRequest.Scope);
						stringBuilder.Append(", filter: ").Append(searchRequest.Filter);
						stringBuilder.Append(", limit: ").Append(searchRequest.SizeLimit);
						stringBuilder.Append(", timeout: ").Append(searchRequest.TimeLimit);
						if (isSnapshotInProgress)
						{
							PerformanceContext.Current.AppendToOperations(stringBuilder.ToString());
						}
						if (flag3)
						{
							Microsoft.Exchange.Diagnostics.Trace.TraceCasStop(CasTraceEventType.ActiveDirectory, serviceProviderRequestId, 0, 0, readConnection.ADServerInfo.FqdnPlusPort, searchRequest.DistinguishedName, flag ? "ADSession::Find:Read" : "ADSession::Find:Non-Read", stringBuilder, string.Empty);
						}
					}
					readConnection.ReturnToPool();
				}
				break;
			}
			SearchResultEntryCollection entries = searchResponse.Entries;
			if (entries.Count == 0)
			{
				if (flag && this.useConfigNC)
				{
					ADProviderPerf.UpdateProcessCounter(Counter.ProcessRateNotFoundConfigReads, UpdateType.Update, 1U);
					string text2 = (readId == null) ? searchRequest.DistinguishedName : readId.ToDNString();
					Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_BASE_CONFIG_SEARCH_FAILED, text2, new object[]
					{
						text2,
						text
					});
				}
				return new TResult[0];
			}
			return this.ObjectsFromEntries<TResult>(entries, text, properties, (ADRawEntry)((object)tresult));
		}

		public ADPagedReader<ADRawEntry> FindPagedADRawEntry(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties)
		{
			return this.FindPaged<ADRawEntry>(rootId, scope, filter, sortBy, pageSize, properties);
		}

		public virtual ADPagedReader<ADRawEntry> FindPagedADRawEntryWithDefaultFilters<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties) where TResult : ADObject, new()
		{
			ADRawEntry adrawEntry = (ADRawEntry)((object)Activator.CreateInstance<TResult>());
			ADObject adobject = adrawEntry as ADObject;
			if (adobject != null)
			{
				filter = this.ApplyDefaultFilters(filter, rootId, adobject, true);
			}
			return this.FindPaged<ADRawEntry>(rootId, scope, filter, sortBy, pageSize, properties);
		}

		protected ADRawEntry[] FindADRawEntryWithDefaultFilters<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int sizeLimit, IEnumerable<PropertyDefinition> properties) where TResult : ADObject, new()
		{
			ADRawEntry adrawEntry = (ADRawEntry)((object)Activator.CreateInstance<TResult>());
			ADObject adobject = adrawEntry as ADObject;
			if (adobject != null)
			{
				filter = this.ApplyDefaultFilters(filter, rootId, adobject, true);
			}
			return this.Find<ADRawEntry>(rootId, scope, filter, sortBy, sizeLimit, properties, false);
		}

		public ADPagedReader<TResult> FindPagedDeletedObject<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize) where TResult : DeletedObject, new()
		{
			ADPagedReader<TResult> adpagedReader = this.FindPaged<TResult>(rootId, scope, filter, sortBy, pageSize, null, true);
			adpagedReader.IncludeDeletedObjects = true;
			return adpagedReader;
		}

		public ADPagedReader<TResult> FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties) where TResult : IConfigurable, new()
		{
			return this.FindPaged<TResult>(rootId, scope, filter, sortBy, pageSize, properties, base.SessionSettings.SkipCheckVirtualIndex);
		}

		protected ADPagedReader<TResult> FindPaged<TResult>(ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties, bool skipCheckVirtualIndex) where TResult : IConfigurable, new()
		{
			if (!typeof(ADRawEntry).IsAssignableFrom(typeof(TResult)))
			{
				throw new InvalidOperationException(DirectoryStrings.ErrorMustBeADRawEntry);
			}
			return new ADPagedReader<TResult>(this, rootId, scope, filter, sortBy, pageSize, properties, skipCheckVirtualIndex);
		}

		public ADRawEntry[] FindAllADRawEntriesByUsnRange(ADObjectId root, long startUsn, long endUsn, int sizeLimit, bool useAtomicFilter, IEnumerable<PropertyDefinition> properties)
		{
			if (sizeLimit > ADDataSession.RangedValueDefaultPageSize)
			{
				throw new ArgumentOutOfRangeException("sizeLimit");
			}
			if (endUsn < startUsn)
			{
				throw new ArgumentOutOfRangeException("endUsn");
			}
			List<QueryFilter> list = new List<QueryFilter>(2);
			list.Add(new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADRecipientSchema.UsnChanged, startUsn));
			if (endUsn != 9223372036854775807L)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.LessThanOrEqual, ADRecipientSchema.UsnChanged, endUsn));
			}
			QueryFilter queryFilter = (list.Count == 1) ? list[0] : new AndFilter(list.ToArray());
			queryFilter.IsAtomic = useAtomicFilter;
			return this.Find<TenantRelocationSyncObject>(root, QueryScope.SubTree, queryFilter, ADDataSession.SortByUsn, sizeLimit, properties, false);
		}

		public ADRawEntry[] FindDeletedTenantSyncObjectByUsnRange(ADObjectId tenantOuRoot, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties)
		{
			if (sizeLimit > ADDataSession.RangedValueDefaultPageSize)
			{
				throw new ArgumentOutOfRangeException("sizeLimit");
			}
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADRecipientSchema.UsnChanged, startUsn),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, tenantOuRoot)
			});
			ADObjectId deletedObjectsContainer;
			if (base.UseConfigNC)
			{
				deletedObjectsContainer = ADSession.GetDeletedObjectsContainer(this.GetConfigurationNamingContext());
			}
			else
			{
				deletedObjectsContainer = ADSession.GetDeletedObjectsContainer(tenantOuRoot.DomainId);
			}
			return this.Find<TenantRelocationSyncObject>(deletedObjectsContainer, QueryScope.OneLevel, filter, ADDataSession.SortByUsn, sizeLimit, properties, true);
		}

		internal void RehomeSession()
		{
			if (OrganizationId.ForestWideOrgId == base.SessionSettings.CurrentOrganizationId)
			{
				return;
			}
			TenantRelocationState tenantRelocationState;
			bool flag;
			if (!TenantRelocationStateCache.TryGetTenantRelocationStateByObjectId(base.SessionSettings.CurrentOrganizationId.ConfigurationUnit, out tenantRelocationState, out flag))
			{
				return;
			}
			OrganizationId organizationId = null;
			if (flag && tenantRelocationState.SourceForestState == TenantRelocationStatus.Retired && tenantRelocationState.TargetOrganizationId != null)
			{
				organizationId = tenantRelocationState.TargetOrganizationId;
			}
			else if (!flag && tenantRelocationState.SourceForestState != TenantRelocationStatus.Retired && tenantRelocationState.OrganizationId != null)
			{
				organizationId = tenantRelocationState.OrganizationId;
			}
			if (organizationId != null)
			{
				this.sessionSettings = ADSessionSettings.RescopeToOrganization(this.sessionSettings, organizationId, false, false);
				base.DomainController = null;
				base.LinkResolutionServer = null;
				this.isRehomed = true;
			}
		}

		public TenantRelocationSyncObject RetrieveTenantRelocationSyncObject(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			TenantRelocationSyncObject[] array = this.Find<TenantRelocationSyncObject>(entryId, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, entryId), null, 2, properties, true);
			if (array.Length <= 0)
			{
				return null;
			}
			return array[0];
		}

		public ADRawEntry ReadADRawEntry(ADObjectId entryId, IEnumerable<PropertyDefinition> properties)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			return this.InternalRead<ADRawEntry>(entryId, properties);
		}

		protected TResult InternalRead<TResult>(ADObjectId entryId, IEnumerable<PropertyDefinition> properties) where TResult : IConfigurable, new()
		{
			if (entryId == null)
			{
				throw new ArgumentNullException("entryId");
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, entryId);
			ADObjectId adobjectId = null;
			if (!TopologyProvider.IsAdamTopology() && !base.UseGlobalCatalog && !base.UseConfigNC && string.IsNullOrEmpty(base.DomainController) && entryId != null && entryId.Rdn != null)
			{
				ADObjectId adobjectId2 = null;
				ADObjectId adobjectId3 = null;
				if (base.EnforceDefaultScope && base.SessionSettings.RecipientViewRoot != null)
				{
					adobjectId2 = base.SessionSettings.RecipientViewRoot;
				}
				if (base.EnforceDefaultScope && base.SessionSettings.RecipientReadScope != null && base.SessionSettings.RecipientReadScope.Root != null)
				{
					adobjectId3 = base.SessionSettings.RecipientReadScope.Root;
				}
				if (adobjectId2 != null && adobjectId3 != null)
				{
					if (adobjectId2.IsDescendantOf(adobjectId3))
					{
						adobjectId = adobjectId2;
					}
					else
					{
						adobjectId = adobjectId3;
					}
				}
				else if (adobjectId2 != null)
				{
					adobjectId = adobjectId2;
				}
				else if (adobjectId3 != null)
				{
					adobjectId = adobjectId3;
				}
				else if (this.IsTenantIdentity(entryId))
				{
					adobjectId = entryId.Parent;
				}
				else
				{
					adobjectId = entryId.DescendantDN(0);
				}
			}
			if (adobjectId == null && entryId.Name.Equals("Microsoft Exchange Autodiscover") && base.ConfigScope == ConfigScopes.RootOrg)
			{
				adobjectId = entryId;
			}
			if (adobjectId == null && entryId.Name.Equals(ADMicrosoftExchangeRecipient.DefaultName))
			{
				adobjectId = entryId;
			}
			TResult[] array = this.InternalFind<TResult>(adobjectId, null, entryId, QueryScope.SubTree, filter, null, 0, properties, false);
			if (array.Length <= 0)
			{
				return default(TResult);
			}
			return array[0];
		}

		private ValidationError ReadRangedAttribute(string distinguishedName, ADPropertyDefinition propertyDefinition, string preferredServerName, List<DirectoryAttribute> results, ADRawEntry dummyInstance)
		{
			SearchRequest searchRequest = new SearchRequest(distinguishedName, "(objectclass=*)", SearchScope.Base, new string[0]);
			ExtendedDNControl control = new ExtendedDNControl(ExtendedDNFlag.StandardString);
			searchRequest.Controls.Add(control);
			RetryManager retryManager = new RetryManager();
			for (;;)
			{
				int num = 0;
				int num2 = 0;
				bool flag = false;
				string text = null;
				results.Clear();
				ADObjectId adobjectId = ADObjectId.ParseExtendedDN(distinguishedName);
				PooledLdapConnection readConnection = this.GetReadConnection(preferredServerName, ref adobjectId, dummyInstance);
				Guid serviceProviderRequestId = Microsoft.Exchange.Diagnostics.Trace.TraceCasStart(CasTraceEventType.ActiveDirectory);
				bool flag2 = true;
				int num3 = ADDataSession.RangedValueDefaultPageSize;
				try
				{
					try
					{
						do
						{
							num2 = num + num3 - 1;
							text = ADSession.GetAttributeNameWithRange(propertyDefinition.LdapDisplayName, num.ToString(CultureInfo.InvariantCulture), (flag || flag2) ? "*" : num2.ToString(CultureInfo.InvariantCulture));
							searchRequest.Attributes.Clear();
							searchRequest.Attributes.Add(text);
							ExTraceGlobals.ADFindTracer.TraceDebug((long)this.GetHashCode(), "ADSession::ReadRangedAttribute using {0} - LDAP search from {1}, scope {2}, filter {3}, asking for {4}", new object[]
							{
								readConnection.ADServerInfo.FqdnPlusPort,
								searchRequest.DistinguishedName,
								(int)searchRequest.Scope,
								searchRequest.Filter,
								text
							});
							SearchResponse searchResponse = (SearchResponse)readConnection.SendRequest(searchRequest, LdapOperation.Read, this.clientSideSearchTimeout, base.ActivityScope, base.CallerInfo);
							SearchResultEntryCollection entries = searchResponse.Entries;
							SearchResultEntry searchResultEntry = entries[0];
							if (flag2)
							{
								foreach (object obj in searchResultEntry.Attributes.AttributeNames)
								{
									string text2 = obj.ToString();
									int num4 = text2.LastIndexOf('-');
									string text3 = text2.Substring(num4 + 1);
									if (text3.Equals("*", StringComparison.InvariantCulture))
									{
										text = ADSession.GetAttributeNameWithRange(propertyDefinition.LdapDisplayName, num.ToString(CultureInfo.InvariantCulture), text3);
										flag = true;
										break;
									}
									try
									{
										num3 = int.Parse(text3) + 1;
										num2 = num + num3 - 1;
										text = ADSession.GetAttributeNameWithRange(propertyDefinition.LdapDisplayName, num.ToString(CultureInfo.InvariantCulture), num2.ToString(CultureInfo.InvariantCulture));
										break;
									}
									catch (FormatException)
									{
										ExWatson.AddExtraData(string.Format("Trying to parse: {0}", text3));
										ExWatson.AddExtraData(string.Format("Property Definition Name: {0}", propertyDefinition.Name));
										ExWatson.AddExtraData(string.Format("Property Definition Type: {0}", propertyDefinition.Type));
										ExWatson.AddExtraData(string.Format("Propert Definition LDAP Display Name: {0}", propertyDefinition.LdapDisplayName));
										throw;
									}
								}
								flag2 = false;
							}
							DirectoryAttribute directoryAttribute = searchResultEntry.Attributes[text];
							if (directoryAttribute == null)
							{
								if (searchResultEntry.Attributes.Count == 0)
								{
									break;
								}
								using (IEnumerator enumerator2 = searchResultEntry.Attributes.Values.GetEnumerator())
								{
									if (enumerator2.MoveNext())
									{
										DirectoryAttribute directoryAttribute2 = (DirectoryAttribute)enumerator2.Current;
										directoryAttribute = directoryAttribute2;
									}
								}
								flag = true;
							}
							num = num2 + 1;
							results.Add(directoryAttribute);
						}
						while (!flag);
						this.UpdateServerSettings(readConnection);
						break;
					}
					catch (DirectoryException de)
					{
						if (readConnection.IsResultCode(de, ResultCode.NoSuchObject))
						{
							ExTraceGlobals.ADReadTracer.TraceDebug<string, string>((long)this.GetHashCode(), "NoSuchObject caught when reading ranges of {0} from {1}", propertyDefinition.LdapDisplayName, distinguishedName);
							return new ObjectValidationError(DirectoryStrings.ExceptionObjectHasBeenDeletedDuringCurrentOperation(distinguishedName), new ADObjectId(distinguishedName, Guid.Empty), readConnection.ServerName);
						}
						retryManager.Tried(readConnection.ServerName);
						this.AnalyzeDirectoryError(readConnection, searchRequest, de, retryManager.TotalRetries, retryManager[readConnection.ServerName]);
					}
					continue;
				}
				finally
				{
					bool isSnapshotInProgress = PerformanceContext.Current.IsSnapshotInProgress;
					bool flag3 = ETWTrace.ShouldTraceCasStop(serviceProviderRequestId);
					if (isSnapshotInProgress || flag3)
					{
						string text4 = string.Format(CultureInfo.InvariantCulture, "scope {0}, filter {1}", new object[]
						{
							searchRequest.Scope,
							searchRequest.Filter
						});
						if (isSnapshotInProgress)
						{
							PerformanceContext.Current.AppendToOperations(text4);
						}
						if (flag3)
						{
							Microsoft.Exchange.Diagnostics.Trace.TraceCasStop(CasTraceEventType.ActiveDirectory, serviceProviderRequestId, 0, 0, preferredServerName, distinguishedName, "ADSession::ReadRangedAttribute", text4, string.Empty);
						}
					}
					readConnection.ReturnToPool();
				}
				break;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_RANGED_READ, distinguishedName, new object[]
			{
				propertyDefinition.Name,
				distinguishedName,
				results.Count,
				preferredServerName
			});
			return null;
		}

		public QueryFilter ApplyDefaultFilters(QueryFilter filter, ADObjectId rootId, ADObject dummyObject, bool applyImplicitFilter)
		{
			ADScope readScope = this.GetReadScope(rootId, dummyObject);
			return this.ApplyDefaultFilters(filter, readScope, dummyObject, applyImplicitFilter);
		}

		public virtual QueryFilter ApplyDefaultFilters(QueryFilter filter, ADScope scope, ADObject dummyObject, bool applyImplicitFilter)
		{
			if (scope.Filter != null)
			{
				filter = ((filter == null) ? scope.Filter : new AndFilter(new QueryFilter[]
				{
					filter,
					scope.Filter
				}));
			}
			if (dummyObject == null)
			{
				filter = (filter ?? ADObject.ObjectClassExistsFilter);
			}
			else
			{
				QueryFilter versioningFilter = dummyObject.VersioningFilter;
				QueryFilter queryFilter = applyImplicitFilter ? dummyObject.ImplicitFilter : null;
				filter = QueryFilter.AndTogether(new QueryFilter[]
				{
					filter,
					queryFilter,
					versioningFilter
				});
			}
			return QueryFilter.SimplifyFilter(filter);
		}

		private void ReadBatch<TKey, TData>(TKey[] keys, ADObjectId rootId, Converter<TKey, QueryFilter> filterBuilder, QueryFilter commonFilter, ADDataSession.HashInserter<TData> hashInserter, ADDataSession.HashLookup<TKey, TData> hashLookup, IEnumerable<PropertyDefinition> properties, bool includeDeletedObjects, bool searchAllNcs, List<Result<TData>> results) where TData : ADRawEntry, new()
		{
			if (keys == null)
			{
				throw new ArgumentNullException("keys");
			}
			if (keys.Rank > 1)
			{
				throw new ArgumentException(DirectoryStrings.ExArgumentException("keys.Rank", keys.Rank.ToString()), "keys");
			}
			if (keys.Length > 20)
			{
				throw new ArgumentOutOfRangeException("keys");
			}
			List<QueryFilter> list = new List<QueryFilter>();
			foreach (QueryFilter queryFilter in Array.ConvertAll<TKey, QueryFilter>(keys, filterBuilder))
			{
				if (queryFilter.GetType() == typeof(OrFilter))
				{
					list.AddRange(((OrFilter)queryFilter).Filters);
				}
				else
				{
					list.Add(queryFilter);
				}
			}
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new OrFilter(list.ToArray()),
				commonFilter
			});
			ADPagedReader<TData> adpagedReader = this.FindPaged<TData>(rootId, QueryScope.SubTree, filter, null, 0, properties);
			adpagedReader.IncludeDeletedObjects = includeDeletedObjects;
			adpagedReader.SearchAllNcs = searchAllNcs;
			if (hashInserter != null)
			{
				Hashtable hashResults = new Hashtable(StringComparer.OrdinalIgnoreCase);
				foreach (TData obj in adpagedReader)
				{
					hashInserter(hashResults, obj);
				}
				results.AddRange(Array.ConvertAll<TKey, Result<TData>>(keys, (TKey key) => hashLookup(hashResults, key)));
				return;
			}
			foreach (TData data in adpagedReader)
			{
				results.Add(new Result<TData>(data, null));
			}
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		protected Result<TData>[] ReadMultiple<TKey, TData>(TKey[] keys, Converter<TKey, QueryFilter> filterBuilder, ADDataSession.HashInserter<TData> hashInserter, ADDataSession.HashLookup<TKey, TData> hashLookup, IEnumerable<PropertyDefinition> properties) where TData : ADRawEntry, new()
		{
			return this.ReadMultiple<TKey, TData>(keys, filterBuilder, hashInserter, hashLookup, properties, false);
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		protected Result<TData>[] ReadMultiple<TKey, TData>(TKey[] keys, Converter<TKey, QueryFilter> filterBuilder, ADDataSession.HashInserter<TData> hashInserter, ADDataSession.HashLookup<TKey, TData> hashLookup, IEnumerable<PropertyDefinition> properties, bool includeDeletedObjects) where TData : ADRawEntry, new()
		{
			return this.ReadMultiple<TKey, TData>(keys, null, filterBuilder, null, hashInserter, hashLookup, properties, includeDeletedObjects, false);
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		protected Result<TData>[] ReadMultiple<TKey, TData>(TKey[] keys, ADObjectId rootId, Converter<TKey, QueryFilter> filterBuilder, QueryFilter commonFilter, ADDataSession.HashInserter<TData> hashInserter, ADDataSession.HashLookup<TKey, TData> hashLookup, IEnumerable<PropertyDefinition> properties) where TData : ADRawEntry, new()
		{
			return this.ReadMultiple<TKey, TData>(keys, rootId, filterBuilder, commonFilter, hashInserter, hashLookup, properties, false, false);
		}

		[MethodImpl(MethodImplOptions.NoOptimization)]
		protected Result<TData>[] ReadMultiple<TKey, TData>(TKey[] keys, ADObjectId rootId, Converter<TKey, QueryFilter> filterBuilder, QueryFilter commonFilter, ADDataSession.HashInserter<TData> hashInserter, ADDataSession.HashLookup<TKey, TData> hashLookup, IEnumerable<PropertyDefinition> properties, bool includeDeletedObjects, bool searchAllNcs = false) where TData : ADRawEntry, new()
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
			for (int i = 0; i < keys.Length; i++)
			{
				if (keys[i] == null)
				{
					throw new ArgumentNullException("keys", DirectoryStrings.ExArgumentNullException(string.Format("keys[{0}]", i)));
				}
			}
			List<Result<TData>> list = new List<Result<TData>>(keys.Length);
			TKey[] array = keys;
			if (keys.Length > 20)
			{
				array = new TKey[20];
			}
			int num = 0;
			do
			{
				if (array != keys)
				{
					if (keys.Length < num + 20)
					{
						array = new TKey[keys.Length - num];
					}
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = keys[num + j];
					}
				}
				this.ReadBatch<TKey, TData>(array, rootId, filterBuilder, commonFilter, hashInserter, hashLookup, properties, includeDeletedObjects, searchAllNcs, list);
				num += array.Length;
			}
			while (num < keys.Length);
			return list.ToArray();
		}

		private ADObjectId GetNamingContext(ADSession.ADNamingContext adNamingContext)
		{
			string accountOrResourceForestFqdn = base.SessionSettings.GetAccountOrResourceForestFqdn();
			ExTraceGlobals.ADTopologyTracer.TraceDebug<string, string, bool>((long)this.GetHashCode(), "{0} - GetNamingContext. DomainController {1}. IsCredentialsSet {2}", accountOrResourceForestFqdn, base.DomainController ?? string.Empty, null != base.NetworkCredential);
			ADObjectId result = null;
			if (!string.IsNullOrEmpty(base.DomainController) && (base.NetworkCredential != null || (!Globals.IsDatacenter && !PartitionId.IsLocalForestPartition(accountOrResourceForestFqdn))))
			{
				switch (adNamingContext)
				{
				case ADSession.ADNamingContext.RootDomain:
					result = ADSession.GetRootDomainNamingContext(base.DomainController, base.NetworkCredential);
					break;
				case ADSession.ADNamingContext.Domain:
					result = ADSession.GetDomainNamingContext(base.DomainController, base.NetworkCredential);
					break;
				case (ADSession.ADNamingContext)3:
					break;
				case ADSession.ADNamingContext.Config:
					result = ADSession.GetConfigurationNamingContext(base.DomainController, base.NetworkCredential);
					break;
				default:
					if (adNamingContext == ADSession.ADNamingContext.Schema)
					{
						result = ADSession.GetSchemaNamingContext(base.DomainController, base.NetworkCredential);
					}
					break;
				}
				return result;
			}
			switch (adNamingContext)
			{
			case ADSession.ADNamingContext.RootDomain:
				result = ADSession.GetRootDomainNamingContext(accountOrResourceForestFqdn);
				break;
			case ADSession.ADNamingContext.Domain:
				result = ADSession.GetDomainNamingContext(accountOrResourceForestFqdn);
				break;
			case (ADSession.ADNamingContext)3:
				break;
			case ADSession.ADNamingContext.Config:
				result = ADSession.GetConfigurationNamingContext(accountOrResourceForestFqdn);
				break;
			default:
				if (adNamingContext == ADSession.ADNamingContext.Schema)
				{
					result = ADSession.GetSchemaNamingContext(accountOrResourceForestFqdn);
				}
				break;
			}
			return result;
		}

		private ADPropertyBag PropertyBagFromSearchResult(ADObjectId id, bool createReadOnly, SearchResultEntry result, IEnumerable<PropertyDefinition> properties, string originatingServerName, OrganizationId executingUserOrgId, List<ValidationError> errors, ADObjectSchema schema, ADRawEntry dummyInstance, bool isDirSyncResponse)
		{
			ADPropertyBag adpropertyBag = new ADPropertyBag(createReadOnly, (schema != null) ? schema.AllProperties.Count : 16);
			List<ADPropertyDefinition> list = new List<ADPropertyDefinition>();
			adpropertyBag.SetField(ADObjectSchema.Id, id);
			this.AddPropertyToPropertyBag(id, ADObjectSchema.ExchangeVersion, result.Attributes, adpropertyBag, list, executingUserOrgId, errors, isDirSyncResponse);
			ExchangeObjectVersion exchangeObjectVersion = (ExchangeObjectVersion)adpropertyBag[ADObjectSchema.ExchangeVersion];
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (!isDirSyncResponse && exchangeObjectVersion.IsOlderThan(adpropertyDefinition.VersionAdded))
				{
					ExTraceGlobals.ADReadDetailsTracer.TraceWarning<string, ADObjectId, ExchangeObjectVersion>(0L, "Skipping property {0} on {1} because object version is {2}.", adpropertyDefinition.Name, id, exchangeObjectVersion);
					if (!adpropertyDefinition.IsCalculated)
					{
						adpropertyBag.SetField(adpropertyDefinition, null);
					}
				}
				else
				{
					this.AddPropertyToPropertyBag(id, adpropertyDefinition, result.Attributes, adpropertyBag, list, executingUserOrgId, errors, isDirSyncResponse);
				}
			}
			if (schema != null)
			{
				foreach (PropertyDefinition propertyDefinition2 in schema.AllMandatoryProperties)
				{
					ADPropertyDefinition adpropertyDefinition2 = (ADPropertyDefinition)propertyDefinition2;
					object obj = null;
					if (!adpropertyBag.TryGetField(adpropertyDefinition2, ref obj) || obj == null)
					{
						PropertyValidationError item = adpropertyDefinition2.ValidateValue(null, true);
						errors.Add(item);
					}
				}
			}
			foreach (ADPropertyDefinition adpropertyDefinition3 in list)
			{
				List<DirectoryAttribute> list2 = new List<DirectoryAttribute>();
				if (!this.skipRangedAttributes)
				{
					ValidationError validationError = this.ReadRangedAttribute(id.DistinguishedName, adpropertyDefinition3, originatingServerName, list2, dummyInstance);
					if (validationError != null)
					{
						errors.Add(validationError);
						continue;
					}
				}
				list2 = this.FilterSoftDeletedObjectLinks(adpropertyDefinition3, list2);
				MultiValuedPropertyBase multiValuedPropertyBase;
				if (list2.Count == 0)
				{
					multiValuedPropertyBase = ADValueConvertor.CreateGenericMultiValuedProperty(adpropertyDefinition3, true, ADDirSyncHelper.EmptyList, ADDirSyncHelper.EmptyList, null);
				}
				else
				{
					multiValuedPropertyBase = ADValueConvertor.GetValueFromMultipleDirectoryAttributes(id, list2, adpropertyDefinition3, base.ReadOnly, errors);
				}
				multiValuedPropertyBase.IsCompletelyRead = !this.skipRangedAttributes;
				adpropertyBag.SetField(adpropertyDefinition3, multiValuedPropertyBase);
			}
			return adpropertyBag;
		}

		private void AddPropertyToPropertyBag(ADObjectId objectId, ADPropertyDefinition propertyDefinition, SearchResultAttributeCollection attributeCollection, PropertyBag propertyBag, List<ADPropertyDefinition> rangedProperties, OrganizationId executingUserOrgId, List<ValidationError> errors, bool isDirSyncResponse)
		{
			ADPropertyDefinition adpropertyDefinition = propertyDefinition;
			bool softLinkEnabled = false;
			if (propertyDefinition.IsSoftLinkAttribute)
			{
				switch (this.sessionSettings.PartitionSoftLinkMode)
				{
				case SoftLinkMode.Enabled:
					adpropertyDefinition = propertyDefinition.SoftLinkShadowProperty;
					softLinkEnabled = true;
					break;
				case SoftLinkMode.DualMatch:
				{
					DirectoryAttribute directoryAttribute;
					if (adpropertyDefinition.IsRanged)
					{
						IntRange intRange = null;
						directoryAttribute = RangedPropertyHelper.GetRangedPropertyValue(propertyDefinition.SoftLinkShadowProperty, attributeCollection, out intRange);
					}
					else
					{
						directoryAttribute = attributeCollection[propertyDefinition.SoftLinkShadowProperty.LdapDisplayName];
					}
					if (directoryAttribute != null)
					{
						adpropertyDefinition = propertyDefinition.SoftLinkShadowProperty;
						softLinkEnabled = true;
					}
					break;
				}
				}
			}
			if (propertyBag.Contains(propertyDefinition))
			{
				return;
			}
			if (propertyDefinition.IsCalculated)
			{
				foreach (ProviderPropertyDefinition providerPropertyDefinition in propertyDefinition.SupportingProperties)
				{
					ADPropertyDefinition propertyDefinition2 = (ADPropertyDefinition)providerPropertyDefinition;
					this.AddPropertyToPropertyBag(objectId, propertyDefinition2, attributeCollection, propertyBag, rangedProperties, executingUserOrgId, errors, isDirSyncResponse);
				}
				return;
			}
			if (string.IsNullOrEmpty(propertyDefinition.LdapDisplayName))
			{
				return;
			}
			IntRange valueRange = null;
			DirectoryAttribute directoryAttribute2;
			if (adpropertyDefinition.IsRanged)
			{
				directoryAttribute2 = RangedPropertyHelper.GetRangedPropertyValue(adpropertyDefinition, attributeCollection, out valueRange);
			}
			else
			{
				directoryAttribute2 = attributeCollection[adpropertyDefinition.LdapDisplayName];
			}
			object obj;
			if (isDirSyncResponse)
			{
				if (directoryAttribute2 == null)
				{
					if (ADDirSyncHelper.IsDirSyncLinkProperty(propertyDefinition))
					{
						int count = rangedProperties.Count;
						obj = ADDirSyncHelper.GetAddedRemovedLinks(propertyDefinition, attributeCollection, rangedProperties, errors);
						if (obj != null)
						{
							propertyBag.SetField(propertyDefinition, obj);
						}
					}
					return;
				}
				if (directoryAttribute2.Count == 0 && propertyDefinition.IsMultivalued)
				{
					obj = ADValueConvertor.CreateGenericMultiValuedProperty(propertyDefinition, true, ADDirSyncHelper.EmptyList, ADDirSyncHelper.EmptyList, null);
					propertyBag.SetField(propertyDefinition, obj);
					return;
				}
			}
			if (directoryAttribute2 == null || (!propertyDefinition.IsMultivalued && directoryAttribute2.Count == 0))
			{
				if (propertyDefinition.IsMandatory)
				{
					PropertyValidationError item = propertyDefinition.ValidateValue(null, true);
					errors.Add(item);
					return;
				}
				obj = null;
			}
			else
			{
				int count2 = directoryAttribute2.Count;
				directoryAttribute2 = this.FilterSoftDeletedObjectLinks(propertyDefinition, directoryAttribute2);
				if (directoryAttribute2.Count == 0 && (!propertyDefinition.IsMultivalued || count2 != 0))
				{
					obj = null;
				}
				else
				{
					obj = ADValueConvertor.GetValueFromDirectoryAttribute(objectId, directoryAttribute2, propertyDefinition, base.ReadOnly || propertyDefinition.IsReadOnly, rangedProperties, executingUserOrgId, errors, valueRange, softLinkEnabled);
				}
			}
			propertyBag.SetField(propertyDefinition, obj);
		}

		public TResult[] ObjectsFromEntries<TResult>(SearchResultEntryCollection entries, string originatingServerName, IEnumerable<PropertyDefinition> properties, ADRawEntry dummyInstance) where TResult : IConfigurable, new()
		{
			if (!typeof(ADRawEntry).IsAssignableFrom(typeof(TResult)))
			{
				throw new InvalidOperationException(DirectoryStrings.ErrorMustBeADRawEntry);
			}
			int tickCount = Environment.TickCount;
			bool flag = dummyInstance.GetType().Equals(typeof(ADRawEntry));
			bool flag2 = dummyInstance is MiniObject;
			bool flag3 = dummyInstance is RootDse;
			bool flag4 = dummyInstance is ADDirSyncResult;
			bool flag5 = dummyInstance is TenantRelocationSyncObject;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = !flag && !flag4 && this.consistencyMode == ConsistencyMode.FullyConsistent;
			bool flag10 = this.readOnly || flag || flag2 || flag4;
			bool flag11 = false;
			if (base.SessionSettings.TenantConsistencyMode != TenantConsistencyMode.IncludeRetiredTenants)
			{
				OrganizationId currentOrganizationId = base.SessionSettings.CurrentOrganizationId;
				if (this.configScope == ConfigScopes.TenantLocal && !OrganizationId.ForestWideOrgId.Equals(currentOrganizationId) && this.IsTenantRelocationAllowedInForest())
				{
					bool flag12;
					TenantRelocationState tenantRelocationState = TenantRelocationStateCache.GetTenantRelocationState(currentOrganizationId.OrganizationalUnit.Name, currentOrganizationId.PartitionId, out flag12, false);
					if (flag12 && tenantRelocationState.SourceForestState == TenantRelocationStatus.Retired)
					{
						this.RehomeAndThrowWrongTenantException(currentOrganizationId.OrganizationalUnit.DistinguishedName, true, false);
					}
					else if (!flag12 && tenantRelocationState.SourceForestState != TenantRelocationStatus.Retired)
					{
						this.RehomeAndThrowWrongTenantException(currentOrganizationId.OrganizationalUnit.DistinguishedName, true, true);
					}
				}
				else if (this.configScope == ConfigScopes.AllTenants && this.IsTenantRelocationAllowedInForest())
				{
					flag11 = true;
				}
			}
			bool flag13 = false;
			if (dummyInstance is ExchangeConfigurationUnit)
			{
				flag13 = true;
			}
			ArrayList arrayList = new ArrayList(entries.Count);
			for (int i = 0; i < entries.Count; i++)
			{
				ADObjectId adobjectId = null;
				if (!string.IsNullOrEmpty(entries[i].DistinguishedName))
				{
					ValidationError validationError = null;
					adobjectId = (ADObjectId)ADValueConvertor.ConvertFromADAndValidateSingleValue(null, entries[i].DistinguishedName, ADObjectSchema.Id, false, base.SessionSettings.ExecutingUserOrganizationId, out validationError);
					if (validationError != null)
					{
						adobjectId = null;
					}
					else if (!ADSession.IsBoundToAdam)
					{
						InternalDirectoryRootOrganizationCache.GetTenantCULocation(base.SessionSettings.GetAccountOrResourceForestFqdn());
					}
					this.Dbg_LogObjectsFromEntries(adobjectId, dummyInstance, ref flag6, ref flag7, ref flag8);
				}
				else if (flag3)
				{
					adobjectId = new ADObjectId(string.Empty);
				}
				if (adobjectId == null)
				{
					ADProviderPerf.UpdateProcessCounter(Counter.ProcessRateIgnoredValidationFailures, UpdateType.Update, 1U);
					Globals.LogEvent((this is ADRecipientObjectSession) ? DirectoryEventLogConstants.Tuple_DSC_EVENT_VALIDATION_FAILED_IGNORE_MODE_RECIPIENT : DirectoryEventLogConstants.Tuple_DSC_EVENT_VALIDATION_FAILED_IGNORE_MODE_CONFIG, entries[i].DistinguishedName ?? string.Empty, new object[]
					{
						entries[i].DistinguishedName ?? string.Empty,
						originatingServerName
					});
				}
				else if (-1 != adobjectId.DistinguishedName.IndexOf(",CN=LostAndFound,DC=", StringComparison.OrdinalIgnoreCase) || -1 != adobjectId.DistinguishedName.IndexOf(",CN=LostAndFoundConfig,CN=Configuration,DC=", StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.ADReadTracer.TraceWarning<string>((long)this.GetHashCode(), "{0} is under the LostAndFound or LostAndFoundConfig container and will be ignored", adobjectId.DistinguishedName);
				}
				else if (ADSession.ShouldFilterCNFObject(this.sessionSettings, adobjectId))
				{
					ExTraceGlobals.ADReadTracer.TraceWarning<string>((long)this.GetHashCode(), "{0} is Collision object and will be ignored", adobjectId.DistinguishedName);
				}
				else if (Globals.IsMicrosoftHostedOnly && (this.configScope == ConfigScopes.TenantSubTree || this.configScope == ConfigScopes.Global) && ADSession.ShouldFilterSoftDeleteObject(this.sessionSettings, adobjectId))
				{
					ExTraceGlobals.ADReadTracer.TraceWarning<string>((long)this.GetHashCode(), "{0} is under the Soft Deleted Objects container and will be ignored because IncludeSoftDeletedObjects is false", adobjectId.DistinguishedName);
				}
				else
				{
					List<ValidationError> list = new List<ValidationError>();
					ADObject adobject = dummyInstance as ADObject;
					ADPropertyBag adpropertyBag = this.PropertyBagFromSearchResult(adobjectId, flag10, entries[i], properties, originatingServerName, base.SessionSettings.ExecutingUserOrganizationId, list, (adobject != null) ? adobject.Schema : null, dummyInstance, flag4);
					if (flag11)
					{
						bool flag14 = false;
						bool flag15 = false;
						if (flag13)
						{
							RelocationStatusDetails r = (RelocationStatusDetails)((byte)adpropertyBag[TenantRelocationRequestSchema.RelocationStatusDetailsRaw]);
							if (TenantRelocationRequest.GetRelocationStatusFromStatusDetails(r) == TenantRelocationStatus.Retired)
							{
								flag14 = true;
							}
							else if ((string)adpropertyBag[TenantRelocationRequestSchema.RelocationSourceForestRaw] != string.Empty && TenantRelocationRequest.GetRelocationStatusFromStatusDetails(r) < TenantRelocationStatus.Active)
							{
								PartitionId partitionId = new PartitionId((string)adpropertyBag[TenantRelocationRequestSchema.RelocationSourceForestRaw]);
								string name = ((ADObjectId)adpropertyBag[ADObjectSchema.Id]).Parent.Name;
								RelocationStatusDetailsSource relocationStatusDetailsSource;
								if (TenantRelocationStateCache.TryLoadTenantRelocationStateSourceReplica(name, partitionId, out relocationStatusDetailsSource) && relocationStatusDetailsSource < RelocationStatusDetailsSource.RetiredUpdatedSourceForest)
								{
									flag15 = true;
								}
							}
						}
						else
						{
							flag14 = TenantRelocationStateCache.IsTenantRetired(adobjectId);
							flag15 = TenantRelocationStateCache.IsTenantArriving(adobjectId);
						}
						if (flag14 || flag15)
						{
							if (base.SessionSettings.TenantConsistencyMode == TenantConsistencyMode.IgnoreRetiredTenants)
							{
								ExTraceGlobals.ADReadTracer.TraceWarning<string, string>((long)this.GetHashCode(), "{0} belongs to {1} tenant and will be ignored because SessionSettings.TenantConsistencyMode is IgnoreRetiredTenants", adobjectId.DistinguishedName, flag14 ? "retired" : "arriving");
								goto IL_85A;
							}
							this.RehomeAndThrowWrongTenantException(adobjectId.DistinguishedName, true, flag15);
						}
					}
					ADRawEntry adrawEntry;
					if (flag)
					{
						adrawEntry = base.CreateAndInitializeADRawEntry(adpropertyBag);
					}
					else if (flag5)
					{
						adrawEntry = this.CreateAndInitializeTenantRelocationSyncObject(adpropertyBag, entries[i]);
					}
					else if (flag4)
					{
						adrawEntry = ADDirSyncHelper.CreateAndInitializeDirSyncResult(adpropertyBag, dummyInstance as ADDirSyncResult);
					}
					else
					{
						adrawEntry = this.CreateAndInitializeObject<TResult>(adpropertyBag, dummyInstance);
					}
					if (!flag && !flag4 && !flag5)
					{
						ADObject adobject2 = (ADObject)adrawEntry;
						if (adobject2.ExchangeVersion.Major > adobject2.MaximumSupportedExchangeObjectVersion.Major)
						{
							ExTraceGlobals.ADReadTracer.TraceWarning<ADObjectId, byte, byte>(0L, "{0} has major version {1} which is greater than current one ({2}) and will be ignored", adobject2.Id, adobject2.ExchangeVersion.Major, adobject2.MaximumSupportedExchangeObjectVersion.Major);
							goto IL_85A;
						}
						if (!flag10 && adobject2.MaximumSupportedExchangeObjectVersion.IsOlderThan(adobject2.ExchangeVersion))
						{
							ExTraceGlobals.ADReadDetailsTracer.TraceWarning<ADObjectId, ExchangeObjectVersion, ExchangeObjectVersion>(0L, "{0} has version {1} which is greater than current one ({2}) and will be readonly", adobject2.Id, adobject2.ExchangeVersion, adobject2.MaximumSupportedExchangeObjectVersion);
							adobject2.SetIsReadOnly(true);
						}
					}
					adrawEntry.OriginatingServer = originatingServerName;
					adrawEntry.WhenReadUTC = new DateTime?(DateTime.UtcNow);
					adrawEntry.IsCached = false;
					adrawEntry.DirectoryBackendType = DirectoryBackendType.AD;
					ExTraceGlobals.ValidationTracer.TraceDebug<int>((long)this.GetHashCode(), "Adding {0} instantiation error(s).", list.Count);
					adrawEntry.InstantiationErrors = list;
					ValidationError[] array = adrawEntry.ValidateRead();
					adrawEntry.ResetChangeTracking(true);
					if (array.Length > 0)
					{
						foreach (ValidationError validationError2 in array)
						{
							PropertyValidationError propertyValidationError = validationError2 as PropertyValidationError;
							ExTraceGlobals.ValidationTracer.TraceDebug((long)this.GetHashCode(), "Object '{0}' read from '{1}' failed validation. Attribute: '{2}'. Invalid data: '{3}'. Error message: '{4}'.", new object[]
							{
								adobjectId.DistinguishedName,
								originatingServerName,
								(propertyValidationError != null) ? propertyValidationError.PropertyDefinition.Name : "<null>",
								(propertyValidationError != null) ? (propertyValidationError.InvalidData ?? "<null>") : "<null>",
								validationError2.Description
							});
							Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_VALIDATION_FAILED_ATTRIBUTE, adobjectId.ToString(), new object[]
							{
								adobjectId.ToDNString(),
								originatingServerName,
								(propertyValidationError != null) ? propertyValidationError.PropertyDefinition.Name : string.Empty,
								validationError2.Description,
								(propertyValidationError != null) ? (propertyValidationError.InvalidData ?? string.Empty) : string.Empty
							});
						}
						if (flag9)
						{
							ADProviderPerf.UpdateProcessCounter(Counter.ProcessRateCriticalValidationFailures, UpdateType.Update, 1U);
							Globals.LogEvent((this is ADRecipientObjectSession) ? DirectoryEventLogConstants.Tuple_DSC_EVENT_VALIDATION_FAILED_FCO_MODE_RECIPIENT : DirectoryEventLogConstants.Tuple_DSC_EVENT_VALIDATION_FAILED_FCO_MODE_CONFIG, adobjectId.ToString(), new object[]
							{
								adobjectId.ToDNString(),
								originatingServerName
							});
							throw new DataValidationException(array[0]);
						}
						if (base.ConsistencyMode == ConsistencyMode.IgnoreInvalid)
						{
							ExTraceGlobals.ADReadTracer.TraceWarning<ADObjectId>(0L, "{0} is invalid and will be ignored", adrawEntry.Id);
							ADProviderPerf.UpdateProcessCounter(Counter.ProcessRateIgnoredValidationFailures, UpdateType.Update, 1U);
							Globals.LogEvent((this is ADRecipientObjectSession) ? DirectoryEventLogConstants.Tuple_DSC_EVENT_VALIDATION_FAILED_IGNORE_MODE_RECIPIENT : DirectoryEventLogConstants.Tuple_DSC_EVENT_VALIDATION_FAILED_IGNORE_MODE_CONFIG, adobjectId.ToString(), new object[]
							{
								adobjectId.ToDNString(),
								originatingServerName
							});
							goto IL_85A;
						}
						ADProviderPerf.UpdateProcessCounter(Counter.ProcessRateNonCriticalValidationFailures, UpdateType.Update, 1U);
						Globals.LogEvent((this is ADRecipientObjectSession) ? DirectoryEventLogConstants.Tuple_DSC_EVENT_VALIDATION_FAILED_PCO_MODE_RECIPIENT : DirectoryEventLogConstants.Tuple_DSC_EVENT_VALIDATION_FAILED_PCO_MODE_CONFIG, adobjectId.ToString(), new object[]
						{
							adobjectId.ToDNString(),
							originatingServerName
						});
					}
					ExTraceGlobals.ADReadTracer.TraceDebug<string>((long)this.GetHashCode(), "Got {0}", adobjectId.DistinguishedName);
					arrayList.Add(adrawEntry);
				}
				IL_85A:;
			}
			TResult[] array3 = new TResult[arrayList.Count];
			arrayList.CopyTo(array3);
			int num = Environment.TickCount - tickCount;
			if (num > ConnectionPoolManager.ObjectsFromEntriesThresholdMilliseconds)
			{
				ExTraceGlobals.ADPerformanceTracer.TraceDebug((long)this.GetHashCode(), "ADSession::ObjectsFromEntries took {0} milliseconds to process {1} {2} ({3}).", new object[]
				{
					num,
					entries.Count,
					(entries.Count == 1) ? "entry" : "entries",
					(array3.Length == 0) ? "no type" : array3[0].GetType().Name
				});
			}
			ActivityContext.AddOperation(ActivityOperationType.ADObjToExchObjLatency, originatingServerName, (float)num, 1);
			return array3;
		}

		private ADRawEntry CreateAndInitializeTenantRelocationSyncObject(PropertyBag propertyBag, SearchResultEntry entry)
		{
			DirectoryAttribute[] array = new DirectoryAttribute[entry.Attributes.Count];
			entry.Attributes.CopyTo(array, 0);
			TenantRelocationSyncObject tenantRelocationSyncObject = new TenantRelocationSyncObject((ADPropertyBag)propertyBag, array);
			tenantRelocationSyncObject.ResetChangeTracking(true);
			return tenantRelocationSyncObject;
		}

		private DirectoryAttribute FilterSoftDeletedObjectLinks(ADPropertyDefinition propertyDefinition, DirectoryAttribute propertyValue)
		{
			DirectoryAttribute directoryAttribute;
			if (Globals.IsMicrosoftHostedOnly && !propertyDefinition.IsBinary && !propertyDefinition.IsMandatory && !propertyDefinition.IsSoftLinkAttribute && propertyDefinition.Type.Equals(typeof(ADObjectId)) && !base.SessionSettings.IncludeSoftDeletedObjectLinks && !base.SessionSettings.IncludeSoftDeletedObjects)
			{
				directoryAttribute = new DirectoryAttribute();
				string[] array = (string[])propertyValue.GetValues(typeof(string));
				foreach (string text in array)
				{
					if (-1 == text.IndexOf(",OU=Soft Deleted Objects,", StringComparison.OrdinalIgnoreCase))
					{
						directoryAttribute.Add(text);
					}
				}
			}
			else
			{
				directoryAttribute = propertyValue;
			}
			return directoryAttribute;
		}

		private List<DirectoryAttribute> FilterSoftDeletedObjectLinks(ADPropertyDefinition propertyDefinition, List<DirectoryAttribute> propertyValueList)
		{
			List<DirectoryAttribute> list = new List<DirectoryAttribute>();
			foreach (DirectoryAttribute propertyValue in propertyValueList)
			{
				DirectoryAttribute directoryAttribute = this.FilterSoftDeletedObjectLinks(propertyDefinition, propertyValue);
				if (directoryAttribute.Count != 0)
				{
					list.Add(directoryAttribute);
				}
			}
			return list;
		}

		protected static ADObjectId GetDomainDeletedObjectsContainer(ADObjectId domainId)
		{
			if (domainId == null)
			{
				throw new ArgumentNullException("domainId");
			}
			return domainId.GetChildId("Deleted Objects");
		}

		internal static bool Diag_GetRegistryBool(RegistryKey regkey, string key, bool defaultValue)
		{
			int? num = null;
			if (regkey != null)
			{
				num = (regkey.GetValue(key) as int?);
			}
			if (num == null)
			{
				return defaultValue;
			}
			return Convert.ToBoolean(num.Value);
		}

		private void CheckTenantConfigObjectHierarchy(ADObjectId tenantObjectId)
		{
			if (tenantObjectId != null && ADSession.IsTenantConfigInDomainNC(base.SessionSettings.GetAccountOrResourceForestFqdn()) && tenantObjectId.IsDescendantOf(this.GetConfigurationNamingContext()))
			{
				throw new ArgumentException("tenantObjectId");
			}
		}

		public TResult ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, ADObjectId containerId) where TResult : ADObject, new()
		{
			return this.ResolveWellKnownGuid<TResult>(wellKnownGuid, containerId.DistinguishedName);
		}

		public TResult ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, string containerDN) where TResult : ADObject, new()
		{
			return this.ResolveWellKnownGuid<TResult>(wellKnownGuid, containerDN, null, null);
		}

		internal TResult ResolveWellKnownGuid<TResult>(Guid wellKnownGuid, string containerDN, QueryFilter restrictingFilter, IEnumerable<PropertyDefinition> props) where TResult : ADRawEntry, new()
		{
			string text = null;
			if (wellKnownGuid == WellKnownGuid.ConfigDeletedObjectsWkGuid)
			{
				if (!base.UseConfigNC)
				{
					throw new InvalidOperationException(DirectoryStrings.ExceptionWKGuidNeedsConfigSession(wellKnownGuid));
				}
			}
			else if (wellKnownGuid == WellKnownGuid.EmaWkGuid_E12 || wellKnownGuid == WellKnownGuid.EoaWkGuid_E12 || wellKnownGuid == WellKnownGuid.EraWkGuid_E12 || wellKnownGuid == WellKnownGuid.EpaWkGuid_E12 || wellKnownGuid == WellKnownGuid.EmaWkGuid || wellKnownGuid == WellKnownGuid.EoaWkGuid || wellKnownGuid == WellKnownGuid.EraWkGuid || wellKnownGuid == WellKnownGuid.EpaWkGuid || wellKnownGuid == WellKnownGuid.E3iWkGuid || wellKnownGuid == WellKnownGuid.EtsWkGuid || wellKnownGuid == WellKnownGuid.EwpWkGuid || wellKnownGuid == WellKnownGuid.ExSWkGuid || wellKnownGuid == WellKnownGuid.MaSWkGuid || wellKnownGuid == WellKnownGuid.EfomgWkGuid || wellKnownGuid == WellKnownGuid.EahoWkGuid || wellKnownGuid == WellKnownGuid.RgDiscoveryManagementWkGuid || wellKnownGuid == WellKnownGuid.RgHelpDeskWkGuid || wellKnownGuid == WellKnownGuid.RgRecordsManagementWkGuid || wellKnownGuid == WellKnownGuid.RgServerManagementWkGuid || wellKnownGuid == WellKnownGuid.RgUmManagementWkGuid || wellKnownGuid == WellKnownGuid.RgUmManagementWkGuid || wellKnownGuid == WellKnownGuid.RgDelegatedSetupWkGuid || wellKnownGuid == WellKnownGuid.RgHygieneManagementWkGuid || wellKnownGuid == WellKnownGuid.RgManagementForestOperatorWkGuid || wellKnownGuid == WellKnownGuid.RgManagementForestTier1SupportWkGuid || wellKnownGuid == WellKnownGuid.RgViewOnlyManagementForestOperatorWkGuid || wellKnownGuid == WellKnownGuid.RgManagementForestMonitoringWkGuid || wellKnownGuid == WellKnownGuid.RgDataCenterManagementWkGuid || wellKnownGuid == WellKnownGuid.RgViewOnlyLocalServerAccessWkGuid || wellKnownGuid == WellKnownGuid.RgDestructiveAccessWkGuid || wellKnownGuid == WellKnownGuid.RgElevatedPermissionsWkGuid || wellKnownGuid == WellKnownGuid.RgServiceAccountsWkGuid || wellKnownGuid == WellKnownGuid.RgOperationsWkGuid || wellKnownGuid == WellKnownGuid.RgViewOnlyWkGuid || wellKnownGuid == WellKnownGuid.RgComplianceManagementWkGuid || wellKnownGuid == WellKnownGuid.RgViewOnlyPIIWkGuid || wellKnownGuid == WellKnownGuid.RgCapacityDestructiveAccessWkGuid || wellKnownGuid == WellKnownGuid.RgCapacityServerAdminsWkGuid || wellKnownGuid == WellKnownGuid.RgCapacityFrontendServerAdminsWkGuid || wellKnownGuid == WellKnownGuid.RgCustomerChangeAccessWkGuid || wellKnownGuid == WellKnownGuid.RgCustomerDataAccessWkGuid || wellKnownGuid == WellKnownGuid.RgAccessToCustomerDataDCOnlyWkGuid || wellKnownGuid == WellKnownGuid.RgDatacenterOperationsDCOnlyWkGuid || wellKnownGuid == WellKnownGuid.RgCustomerDestructiveAccessWkGuid || wellKnownGuid == WellKnownGuid.RgCustomerPIIAccessWkGuid || wellKnownGuid == WellKnownGuid.RgAppLockerExemptionWkGuid || wellKnownGuid == WellKnownGuid.RgDedicatedSupportAccessWkGuid || wellKnownGuid == WellKnownGuid.RgECSAdminServerAccessWkGuid || wellKnownGuid == WellKnownGuid.RgECSPIIAccessServerAccessWkGuid || wellKnownGuid == WellKnownGuid.RgECSAdminWkGuid || wellKnownGuid == WellKnownGuid.RgECSPIIAccessWkGuid || wellKnownGuid == WellKnownGuid.RgManagementAdminAccessWkGuid || wellKnownGuid == WellKnownGuid.RgManagementCACoreAdminWkGuid || wellKnownGuid == WellKnownGuid.RgManagementChangeAccessWkGuid || wellKnownGuid == WellKnownGuid.RgManagementDestructiveAccessWkGuid || wellKnownGuid == WellKnownGuid.RgManagementServerAdminsWkGuid || wellKnownGuid == WellKnownGuid.RgNetworkingAdminAccessWkGuid || wellKnownGuid == WellKnownGuid.RgNetworkingChangeAccessWkGuid || wellKnownGuid == WellKnownGuid.RgCapacityDCAdminsWkGuid || wellKnownGuid == WellKnownGuid.RgCommunicationManagersWkGuid || wellKnownGuid == WellKnownGuid.RgMailboxManagementWkGuid || wellKnownGuid == WellKnownGuid.RgFfoAntiSpamAdminsWkGuid)
			{
				if (containerDN.IndexOf(",CN=ConfigurationUnits,") >= 0)
				{
					if (!base.UseGlobalCatalog && this.domainController == null)
					{
						throw new InvalidOperationException(DirectoryStrings.ExceptionWKGuidNeedsGCSession(wellKnownGuid));
					}
				}
				else
				{
					if (base.UseConfigNC || (!base.UseGlobalCatalog && this.domainController == null))
					{
						throw new InvalidOperationException(DirectoryStrings.ExceptionWKGuidNeedsGCSession(wellKnownGuid));
					}
					text = "CN=Microsoft Exchange,CN=Services," + containerDN;
				}
			}
			else if ((wellKnownGuid == WellKnownGuid.UsersWkGuid || wellKnownGuid == WellKnownGuid.SystemWkGuid || wellKnownGuid == WellKnownGuid.DomainControllersWkGuid) && base.UseConfigNC)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionWKGuidNeedsDomainSession(wellKnownGuid));
			}
			TResult[] array = this.FindByWellKnownGuid<TResult>(wellKnownGuid, text ?? containerDN, restrictingFilter, props);
			if ((array == null || array.Length == 0) && text != null)
			{
				array = this.FindByWellKnownGuid<TResult>(wellKnownGuid, containerDN, restrictingFilter, props);
			}
			if (array == null || array.Length == 0)
			{
				return default(TResult);
			}
			return array[0];
		}

		private TResult[] FindByWellKnownGuid<TResult>(Guid wellKnownGuid, string containerDN, QueryFilter restrictingFilter, IEnumerable<PropertyDefinition> props) where TResult : ADRawEntry, new()
		{
			string optionalBaseDN = ADSession.StringFromWkGuid(wellKnownGuid, containerDN);
			return this.InternalFind<TResult>(new ADObjectId(containerDN), optionalBaseDN, null, QueryScope.Base, restrictingFilter, null, 1, props, false);
		}

		public PooledLdapConnection GetReadConnection(string preferredServer, ref ADObjectId rootId)
		{
			return this.GetReadConnection(preferredServer, ref rootId, null);
		}

		internal PooledLdapConnection GetReadConnection(string preferredServer, ref ADObjectId rootId, ADRawEntry scopeDeteriminingObject)
		{
			return this.GetReadConnection(preferredServer, null, ref rootId, scopeDeteriminingObject);
		}

		public PooledLdapConnection GetReadConnection(string preferredServer, string optionalBaseDN, ref ADObjectId rootId, ADRawEntry scopeDeteriminingObject)
		{
			ConfigScopes configScopes;
			ADScope readScope = this.GetReadScope(rootId, scopeDeteriminingObject, this.IsWellKnownGuidDN(optionalBaseDN), out configScopes);
			return this.GetConnection(preferredServer, false, optionalBaseDN, ref rootId, readScope);
		}

		private bool IsWellKnownGuidDN(string baseDN)
		{
			return !string.IsNullOrEmpty(baseDN) && baseDN.StartsWith("<WKGUID=", StringComparison.Ordinal);
		}

		public bool IsReadConnectionAvailable()
		{
			try
			{
				ADObjectId adobjectId = null;
				PooledLdapConnection readConnection = this.GetReadConnection(null, ref adobjectId);
				readConnection.ReturnToPool();
				return true;
			}
			catch (ADTransientException)
			{
			}
			catch (SystemException)
			{
			}
			return false;
		}

		public void UpdateServerSettings(PooledLdapConnection connection)
		{
			base.ServerSettings.SetLastUsedDc(base.SessionSettings.GetAccountOrResourceForestFqdn(), connection.ServerName);
			if (string.IsNullOrEmpty(base.DomainController))
			{
				if (base.ServerSettings.IsUpdatableByADSession)
				{
					Fqdn fqdn = new Fqdn(connection.ServerName);
					switch (connection.Role)
					{
					case ADServerRole.GlobalCatalog:
						if (base.ServerSettings.PreferredGlobalCatalog(base.SessionSettings.GetAccountOrResourceForestFqdn()) == null)
						{
							base.ServerSettings.SetPreferredGlobalCatalog(base.SessionSettings.GetAccountOrResourceForestFqdn(), connection.ADServerInfo);
							ExTraceGlobals.ADTopologyTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Setting preferred Global Catalog '{0}' for partition '{1}'", connection.ServerName, base.SessionSettings.GetAccountOrResourceForestFqdn());
							goto IL_18C;
						}
						goto IL_18C;
					case ADServerRole.DomainController:
						base.ServerSettings.AddPreferredDC(connection.ADServerInfo);
						ExTraceGlobals.ADTopologyTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Setting Domain Controller '{0}' as preferred for domain '{1}'", connection.ServerName, connection.ADServerInfo.WritableNC);
						goto IL_18C;
					case ADServerRole.ConfigurationDomainController:
						if (!connection.IsNotify && base.ServerSettings.ConfigurationDomainController(base.SessionSettings.GetAccountOrResourceForestFqdn()) == null)
						{
							base.ServerSettings.SetConfigurationDomainController(base.SessionSettings.GetAccountOrResourceForestFqdn(), connection.ADServerInfo);
							ExTraceGlobals.ADTopologyTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Setting preferred Configuration Domain Controller '{0}' for partition '{1}'", connection.ServerName, base.SessionSettings.GetAccountOrResourceForestFqdn());
							goto IL_18C;
						}
						goto IL_18C;
					}
					throw new NotSupportedException("Unsupported server role: " + connection.Role);
					IL_18C:
					base.ServerSettings.MarkDcUp(fqdn);
					return;
				}
				if (!this.readOnly)
				{
					base.DomainController = connection.ServerName;
					this.stickyDC = true;
				}
			}
		}

		private void UpdateServerSettings(PooledLdapConnection connection, ADErrorRecord errorRecord)
		{
			base.ServerSettings.SetLastUsedDc(base.SessionSettings.GetAccountOrResourceForestFqdn(), connection.ServerName);
			if (string.IsNullOrEmpty(base.DomainController) && base.ServerSettings.IsUpdatableByADSession && errorRecord.IsDownError)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "Marking Domain Controller '{0}' as down in the ADServerSettings", connection.ServerName);
				base.ServerSettings.MarkDcDown(new Fqdn(connection.ServerName));
			}
		}

		private void UpdateServerSettingsAfterSuitabilityError(string fqdn)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			bool flag = false;
			if (string.IsNullOrEmpty(base.DomainController) && base.ServerSettings.IsUpdatableByADSession)
			{
				ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "Marking Domain Controller '{0}' as down in the ADServerSettings", fqdn);
				base.ServerSettings.MarkDcDown(new Fqdn(fqdn));
				flag = true;
			}
			string text = new StackTrace().ToString();
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_UpdateServerSettingsAfterSuitabilityError, text, new object[]
			{
				fqdn,
				base.DomainController ?? "<null>",
				flag,
				text
			});
		}

		[Conditional("DEBUG")]
		private void Dbg_VerifyIsObjectTypeValid<TResult>() where TResult : IConfigurable, new()
		{
			ADRecipientObjectSession adrecipientObjectSession = this as ADRecipientObjectSession;
		}

		private PooledLdapConnection GetWritableConnection(string preferredServer, ADObjectId identity, ADRawEntry scopeDeterimingObject)
		{
			ADObjectId rootId = identity;
			ADScope readScope = this.GetReadScope(rootId, scopeDeterimingObject);
			return this.GetConnection(preferredServer, true, null, ref rootId, readScope);
		}

		public ADScope GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject)
		{
			ConfigScopes configScopes;
			return this.GetReadScope(rootId, scopeDeterminingObject, out configScopes);
		}

		internal ADScope GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject, out ConfigScopes applicableScope)
		{
			return this.GetReadScope(rootId, scopeDeterminingObject, false, out applicableScope);
		}

		public virtual ADScope GetReadScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject, bool isWellKnownGuidSearch, out ConfigScopes applicableScope)
		{
			ADObjectId adobjectId = null;
			QueryFilter queryFilter = null;
			applicableScope = ConfigScopes.Global;
			bool flag = base.UseConfigNC;
			bool flag2 = false;
			bool flag3 = this.IsDeletedObjectId(rootId);
			if (!this.sessionSettings.IsGlobal)
			{
				applicableScope = this.configScope;
				if (rootId != null)
				{
					if (scopeDeterminingObject is RootDse)
					{
						flag = true;
					}
					flag = rootId.IsDescendantOf(this.GetConfigurationNamingContext());
					if (!flag)
					{
						try
						{
							if (ADSession.IsTenantConfigInDomainNC(base.SessionSettings.GetAccountOrResourceForestFqdn()) && rootId.IsDescendantOf(this.GetConfigurationUnitsRoot()))
							{
								flag = true;
							}
						}
						catch (OrgContainerNotFoundException ex)
						{
							if (!Globals.IsDatacenter)
							{
								throw ex;
							}
						}
					}
				}
				if (!flag3 && scopeDeterminingObject != null && scopeDeterminingObject is ADObject)
				{
					ObjectScopeAttribute objectScopeAttribute;
					flag2 = ((ADObject)scopeDeterminingObject).IsApplicableToTenant(out objectScopeAttribute);
					if (this.configScope == ConfigScopes.AllTenants && !flag2)
					{
						throw new InvalidOperationException("AllTenants and TenantSubtree scopes must be used to read only tenant objects");
					}
					if (objectScopeAttribute != null && objectScopeAttribute.ConfigScope != ConfigScopes.None)
					{
						applicableScope = objectScopeAttribute.ConfigScope;
					}
				}
			}
			else if (this is ADConfigurationSession && Globals.IsDatacenter && rootId == null && flag && scopeDeterminingObject != null && scopeDeterminingObject is ADObject)
			{
				ObjectScopeAttribute objectScopeAttribute2;
				flag2 = ((ADObject)scopeDeterminingObject).IsApplicableToTenant(out objectScopeAttribute2);
				if (objectScopeAttribute2 != null && objectScopeAttribute2.HasApplicableConfigScope(ConfigScopes.TenantSubTree))
				{
					applicableScope = ConfigScopes.AllTenants;
				}
			}
			this.VerifySessionScopeRestrictionsForRootOrgScope(rootId, scopeDeterminingObject, flag2);
			if (flag)
			{
				if (ConfigScopes.TenantLocal == applicableScope)
				{
					this.CheckTenantConfigObjectHierarchy(this.sessionSettings.ConfigReadScope.Root);
					adobjectId = (this.sessionSettings.ConfigReadScope.Root ?? this.sessionSettings.RootOrgId);
					queryFilter = this.sessionSettings.ConfigReadScope.Filter;
				}
				else if (ConfigScopes.RootOrg == applicableScope)
				{
					adobjectId = ((flag2 && !(scopeDeterminingObject is Organization)) ? this.sessionSettings.RootOrgId : rootId);
					if (rootId != null && rootId.Name.Equals("Microsoft Exchange Autodiscover"))
					{
						adobjectId = rootId;
					}
					if (isWellKnownGuidSearch)
					{
						adobjectId = null;
					}
					queryFilter = this.GetTenantLocalReadFilter(this.sessionSettings.ConfigReadScope.Filter);
				}
				else if (ConfigScopes.TenantSubTree == applicableScope)
				{
					this.CheckTenantConfigObjectHierarchy(this.sessionSettings.ConfigReadScope.Root);
					if (rootId != null)
					{
						adobjectId = rootId;
					}
					else
					{
						adobjectId = ((this.sessionSettings.ConfigReadScope.Root == null) ? null : this.sessionSettings.ConfigReadScope.Root.Parent);
					}
					queryFilter = this.sessionSettings.ConfigReadScope.Filter;
				}
				else if (ConfigScopes.AllTenants == applicableScope)
				{
					adobjectId = this.GetConfigurationUnitsRoot();
					queryFilter = this.sessionSettings.ConfigReadScope.Filter;
				}
				else if (ConfigScopes.Server != applicableScope && ConfigScopes.Database == applicableScope)
				{
				}
			}
			else
			{
				if (ConfigScopes.Server == applicableScope || ConfigScopes.Database == applicableScope)
				{
					throw new InvalidSessionOperationException(DirectoryStrings.ExceptionInvalidScopeOperation(applicableScope));
				}
				if (ConfigScopes.TenantLocal == applicableScope || ConfigScopes.RootOrg == applicableScope)
				{
					adobjectId = this.sessionSettings.RecipientReadScope.Root;
					queryFilter = this.GetTenantLocalReadFilter(this.sessionSettings.RecipientReadScope.Filter);
				}
				else if (ConfigScopes.TenantSubTree == applicableScope)
				{
					adobjectId = this.sessionSettings.RecipientReadScope.Root;
					queryFilter = this.sessionSettings.RecipientReadScope.Filter;
				}
				else if (ConfigScopes.AllTenants == applicableScope)
				{
					adobjectId = this.GetHostedOrganizationsRoot();
					queryFilter = this.sessionSettings.RecipientReadScope.Filter;
				}
			}
			if (flag3)
			{
				if (applicableScope == ConfigScopes.TenantLocal)
				{
					queryFilter = this.GetTenantLocalReadFilter(this.sessionSettings.RecipientReadScope.Filter);
					adobjectId = rootId;
				}
				else if (applicableScope == ConfigScopes.AllTenants)
				{
					queryFilter = new ExistsFilter(ADObjectSchema.ConfigurationUnit);
					adobjectId = rootId;
				}
			}
			ExTraceGlobals.ScopeVerificationTracer.TraceDebug((long)this.GetHashCode(), "ADSession::GetReadScope RootId '{0}', ScopeDeterminingObject '{1}', IsScopeInConfigNC '{2}', ApplicableScope '{3}', ScopeRoot '{4}', ScopeFilter '{5}'", new object[]
			{
				(rootId == null) ? "<null>" : rootId.ToDNString(),
				(scopeDeterminingObject == null) ? "<null>" : scopeDeterminingObject.GetType().Name,
				flag,
				applicableScope,
				(adobjectId == null) ? "<null>" : adobjectId.ToDNString(),
				(queryFilter == null) ? "<null>" : queryFilter.ToString()
			});
			return new ADScope(adobjectId, queryFilter);
		}

		private void VerifySessionScopeRestrictionsForRootOrgScope(ADObjectId rootId, ADRawEntry scopeDeterminingObject, bool isTenantObject)
		{
			if (this.configScope == ConfigScopes.RootOrg && rootId != null)
			{
				try
				{
					if (this.IsTenantIdentity(rootId))
					{
						throw new InvalidOperationException("RootOrg session with tenant root " + rootId.ToDNString());
					}
				}
				catch (OrgContainerNotFoundException ex)
				{
					if (!Globals.IsDatacenter)
					{
						throw ex;
					}
				}
			}
		}

		private ADScope EnforceRecipientViewRoot(ADScope scope)
		{
			ADObjectId root = scope.Root;
			ADObjectId recipientViewRoot = base.SessionSettings.RecipientViewRoot;
			if (base.EnforceDefaultScope && recipientViewRoot != null && (root == null || recipientViewRoot.IsDescendantOf(root)))
			{
				return new ADScope(recipientViewRoot, scope.Filter);
			}
			return scope;
		}

		private QueryFilter GetTenantLocalReadFilter(QueryFilter readScopeFilter)
		{
			QueryFilter queryFilter;
			if (this.sessionSettings.CurrentOrganizationId.ConfigurationUnit == null)
			{
				queryFilter = ADDataSession.rootOrgFilter;
			}
			else
			{
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ConfigurationUnit, this.sessionSettings.CurrentOrganizationId.ConfigurationUnit);
			}
			if (readScopeFilter != null)
			{
				return new AndFilter(new QueryFilter[]
				{
					readScopeFilter,
					queryFilter
				});
			}
			return queryFilter;
		}

		internal bool IsConfigScoped(ADScope scope)
		{
			if (!base.UseConfigNC || scope == null || scope.Root == null || scope.Root.IsDescendantOf(this.GetConfigurationNamingContext()))
			{
				return base.UseConfigNC;
			}
			string accountOrResourceForestFqdn = base.SessionSettings.GetAccountOrResourceForestFqdn();
			bool flag = scope.Root.IsDescendantOf(this.GetHostedOrganizationsRoot());
			bool flag2 = scope.Root.IsDescendantOf(this.GetConfigurationUnitsRoot());
			bool flag3 = scope.Root.IsDescendantOf(ADSession.GetDeletedObjectsContainer(this.GetDomainNamingContext())) || scope.Root.IsDescendantOf(ADSession.GetDeletedObjectsContainer(this.GetConfigurationNamingContext()));
			bool flag4 = ConfigScopes.TenantSubTree == base.ConfigScope || ConfigScopes.AllTenants == base.ConfigScope || ConfigScopes.TenantLocal == base.ConfigScope;
			bool flag5 = ADSession.IsTenantConfigInDomainNC(accountOrResourceForestFqdn);
			bool flag6 = this is IConfigurationSession;
			if (!flag && !flag2 && !flag3 && (!flag4 || !flag5 || !flag6))
			{
				throw new InvalidOperationException(string.Format("Scope.Root={0},ConfigScope={1},TenantInDomainNC={2},Session={3}, SessionPartition={4}", new object[]
				{
					scope.Root.DistinguishedName,
					base.ConfigScope,
					ADSession.IsTenantConfigInDomainNC(accountOrResourceForestFqdn),
					base.GetType().Name,
					accountOrResourceForestFqdn
				}));
			}
			return false;
		}

		[Conditional("DEBUG")]
		private static void CheckDcName(string dcName)
		{
			if (!string.IsNullOrEmpty(dcName))
			{
				return;
			}
			string text = ApplicationName.Current.Name.ToLower();
			if (text.Contains("frontend"))
			{
				return;
			}
			if (text.IndexOf("powershell") <= 0)
			{
				return;
			}
			StackTrace stackTrace = new StackTrace();
			foreach (StackFrame stackFrame in stackTrace.GetFrames())
			{
				MethodBase method = stackFrame.GetMethod();
				if (!(method.DeclaringType == null))
				{
					string fullName = method.DeclaringType.FullName;
					string a;
					if ((a = fullName) != null && (a == "Microsoft.Exchange.Configuration.Authorization.ExchangeAuthorizationPlugin" || a == "Microsoft.Exchange.Data.Directory.Budget"))
					{
						break;
					}
				}
			}
		}

		protected virtual PooledLdapConnection GetConnection(string preferredServer, bool isWriteOperation, string optionalBaseDN, ref ADObjectId rootId, ADScope scope)
		{
			bool flag = this.IsConfigScoped(scope);
			bool flag2 = base.UseGlobalCatalog && flag == base.UseConfigNC;
			PooledLdapConnection pooledLdapConnection = null;
			ADObjectId recipientViewRoot = base.SessionSettings.RecipientViewRoot;
			ADObjectId configurationNamingContext = this.GetConfigurationNamingContext();
			ADObjectId domain = null;
			if (scope == null)
			{
				scope = new ADScope(null, null);
			}
			string message = string.Empty;
			bool flag3 = ExTraceGlobals.GetConnectionTracer.IsTraceEnabled(TraceType.DebugTrace);
			if (flag3)
			{
				message = string.Format("Getting connection: preferred server={0}, session DC={1}, rootId={2}, Session UseConfigNC={3}, actual IsConfigScoped={4}, Session UseGlobalCatalog={5}, actual useGC={6}, IsWriteOperation={7}, Credentials {8}{9}{10}, DefaultScope={11}, EnforceDefaultScope={12}, SearchRoot={13}", new object[]
				{
					preferredServer ?? "<null>",
					this.domainController ?? "<null>",
					(rootId == null) ? "<null>" : rootId.ToDNString(),
					this.useConfigNC,
					flag,
					this.useGlobalCatalog,
					flag2,
					isWriteOperation,
					(this.networkCredential == null) ? " not present" : this.networkCredential.Domain,
					(this.networkCredential == null) ? string.Empty : "\\",
					(this.networkCredential == null) ? string.Empty : this.networkCredential.UserName,
					(recipientViewRoot == null) ? "<null>" : recipientViewRoot.ToDNString(),
					this.enforceDefaultScope,
					(base.SearchRoot == null) ? "<null>" : base.SearchRoot.ToDNString()
				});
			}
			ExTraceGlobals.GetConnectionTracer.TraceDebug((long)this.GetHashCode(), message);
			if (base.SearchRoot != null)
			{
				bool flag4 = false;
				if (rootId != null)
				{
					if (base.SearchRoot.ToDNString().StartsWith("OU=Soft Deleted Objects,") && base.SearchRoot.IsDescendantOf(rootId))
					{
						rootId = base.SearchRoot;
						flag4 = true;
					}
					else if (!rootId.IsDescendantOf(base.SearchRoot))
					{
						throw new ADOperationException(DirectoryStrings.ExceptionSearchRootNotChildOfSessionSearchRoot(rootId.ToCanonicalName(), base.SearchRoot.ToCanonicalName()));
					}
				}
				else
				{
					rootId = base.SearchRoot;
				}
				if (recipientViewRoot != null && !flag4)
				{
					throw new NotSupportedException(DirectoryStrings.ExceptionDefaultScopeAndSearchRoot);
				}
			}
			ADOperationException ex;
			if (rootId == null)
			{
				rootId = scope.Root;
			}
			else if (!this.IsValidReadScopeRoot(rootId, optionalBaseDN, scope, out ex))
			{
				throw ex;
			}
			string text;
			if (!string.IsNullOrEmpty(preferredServer))
			{
				text = preferredServer;
			}
			else if (!string.IsNullOrEmpty(this.domainController))
			{
				text = this.domainController;
			}
			else if (flag)
			{
				text = base.ServerSettings.ConfigurationDomainController(this.sessionSettings.GetAccountOrResourceForestFqdn());
			}
			else if (flag2 && !isWriteOperation)
			{
				text = base.ServerSettings.PreferredGlobalCatalog(this.sessionSettings.GetAccountOrResourceForestFqdn());
			}
			else
			{
				bool flag5 = true;
				if (rootId == null)
				{
					ADObjectId domainNamingContext = this.GetDomainNamingContext();
					bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.AD.EnableUseIsDescendantOfForRecipientViewRoot.Enabled;
					if (recipientViewRoot != null && (enabled ? recipientViewRoot.IsDescendantOf(domainNamingContext) : (recipientViewRoot.GetPartitionId() == domainNamingContext.GetPartitionId())))
					{
						rootId = recipientViewRoot;
					}
					else
					{
						rootId = domainNamingContext;
						flag5 = false;
					}
				}
				domain = rootId.DomainId;
				if (flag5)
				{
					scope = this.EnforceRecipientViewRoot(scope);
				}
				if (!this.IsValidReadScopeRoot(rootId, optionalBaseDN, scope, out ex))
				{
					throw ex;
				}
				text = this.sessionSettings.GetPreferredDC(domain);
			}
			string accountOrResourceForestFqdn = this.sessionSettings.GetAccountOrResourceForestFqdn();
			if (!this.readOnly)
			{
				bool isTenantScoped = base.SessionSettings.IsTenantScoped;
			}
			try
			{
				if (!string.IsNullOrEmpty(text))
				{
					if (!flag && flag2 && !isWriteOperation)
					{
						pooledLdapConnection = ConnectionPoolManager.GetConnection(ConnectionType.GlobalCatalog, (!Globals.IsDatacenter || this.networkCredential != null) ? TopologyProvider.LocalForestFqdn : accountOrResourceForestFqdn, this.networkCredential, text, TopologyProvider.GetInstance().DefaultGCPort);
						if (rootId == null)
						{
							rootId = new ADObjectId();
						}
					}
					else
					{
						pooledLdapConnection = ConnectionPoolManager.GetConnection(flag ? ConnectionType.ConfigurationDomainController : ConnectionType.DomainController, (!Globals.IsDatacenter || this.networkCredential != null) ? TopologyProvider.LocalForestFqdn : accountOrResourceForestFqdn, this.networkCredential, text, TopologyProvider.GetInstance().DefaultDCPort);
						if (rootId == null)
						{
							if (flag)
							{
								if (string.IsNullOrEmpty(pooledLdapConnection.ADServerInfo.ConfigNC))
								{
									rootId = configurationNamingContext;
								}
								else
								{
									rootId = ((base.SessionSettings.ConfigScopes == ConfigScopes.AllTenants && ADSession.IsTenantConfigInDomainNC(accountOrResourceForestFqdn)) ? ADObjectId.ParseExtendedDN(pooledLdapConnection.ADServerInfo.RootDomainNC) : ADObjectId.ParseExtendedDN(pooledLdapConnection.ADServerInfo.ConfigNC));
								}
							}
							else if (!flag2)
							{
								rootId = ADObjectId.ParseExtendedDN(pooledLdapConnection.ADServerInfo.WritableNC);
							}
							ExTraceGlobals.GetConnectionTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Replacing null rootId with {0} when talking to {1}", (rootId == null) ? "<null>" : rootId.ToString(), text);
						}
					}
				}
				else if (flag)
				{
					if (this.networkCredential != null)
					{
						throw new ADOperationException(DirectoryStrings.ExceptionCredentialsNotSupportedWithoutDC);
					}
					pooledLdapConnection = ConnectionPoolManager.GetConnection(ConnectionType.ConfigurationDomainController, accountOrResourceForestFqdn);
					if (rootId == null)
					{
						rootId = configurationNamingContext;
						ExTraceGlobals.GetConnectionTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "Setting rootId to {0} for local config read/write", rootId);
					}
				}
				else if (flag2 && !isWriteOperation)
				{
					if (this.networkCredential != null)
					{
						throw new ADOperationException(DirectoryStrings.ExceptionCredentialsNotSupportedWithoutDC);
					}
					pooledLdapConnection = ConnectionPoolManager.GetConnection(ConnectionType.GlobalCatalog, accountOrResourceForestFqdn);
					if (rootId == null)
					{
						rootId = new ADObjectId();
					}
				}
				else if (rootId.IsDescendantOf(configurationNamingContext))
				{
					if (this.networkCredential != null)
					{
						throw new ADOperationException(DirectoryStrings.ExceptionCredentialsNotSupportedWithoutDC);
					}
					pooledLdapConnection = ConnectionPoolManager.GetConnection(ConnectionType.ConfigurationDomainController, accountOrResourceForestFqdn);
				}
				else
				{
					if (this.networkCredential != null && flag2)
					{
						throw new ADOperationException(DirectoryStrings.ExceptionCredentialsNotSupportedWithoutDC);
					}
					pooledLdapConnection = ConnectionPoolManager.GetConnection(ConnectionType.DomainController, (!Globals.IsDatacenter || this.networkCredential != null) ? TopologyProvider.LocalForestFqdn : accountOrResourceForestFqdn, this.networkCredential, domain);
				}
			}
			catch (SuitabilityException ex2)
			{
				if (!string.IsNullOrEmpty(ex2.ServerFqdn))
				{
					this.UpdateServerSettingsAfterSuitabilityError(ex2.ServerFqdn);
					if (base.DomainController != null && this.stickyDC)
					{
						base.DomainController = null;
					}
				}
				throw;
			}
			catch (ADServerNotSuitableException ex3)
			{
				if (!string.IsNullOrEmpty(ex3.ServerFqdn))
				{
					this.UpdateServerSettingsAfterSuitabilityError(ex3.ServerFqdn);
				}
				throw;
			}
			if (!this.IsValidReadScopeRoot(rootId, optionalBaseDN, scope, out ex))
			{
				throw ex;
			}
			ExTraceGlobals.GetConnectionTracer.TraceDebug<string>((long)this.GetHashCode(), "Returning connection to {0}", pooledLdapConnection.ADServerInfo.FqdnPlusPort);
			return pooledLdapConnection;
		}

		public bool IsRootIdWithinScope<TObject>(ADObjectId rootId) where TObject : IConfigurable, new()
		{
			ADRawEntry scopeDeterminingObject = ((default(TObject) == null) ? Activator.CreateInstance<TObject>() : default(TObject)) as ADRawEntry;
			try
			{
				ADScope readScope = this.GetReadScope(rootId, scopeDeterminingObject);
				ADOperationException ex;
				if (!this.IsValidReadScopeRoot(rootId, readScope, out ex))
				{
					return false;
				}
			}
			catch (InvalidSessionOperationException)
			{
				return false;
			}
			return true;
		}

		private bool IsValidReadScopeRoot(ADObjectId rootId, ADScope scope, out ADOperationException error)
		{
			return this.IsValidReadScopeRoot(rootId, null, scope, out error);
		}

		private bool IsValidReadScopeRoot(ADObjectId rootId, string optionalBaseDN, ADScope scope, out ADOperationException error)
		{
			if (optionalBaseDN != null && base.ConfigScope == ConfigScopes.TenantLocal && OrganizationId.ForestWideOrgId.Equals(base.SessionSettings.CurrentOrganizationId) && this.IsWellKnownGuidDN(optionalBaseDN))
			{
				error = null;
				return true;
			}
			if (rootId != null && scope != null && scope.Root != null)
			{
				if (string.IsNullOrEmpty(rootId.DistinguishedName))
				{
					error = new ADOperationException(DirectoryStrings.ExceptionGuidSearchRootWithScope(rootId.ObjectGuid.ToString()));
					return false;
				}
				if (this.IsDeletedObjectId(rootId) && (base.ConfigScope == ConfigScopes.AllTenants || (base.ConfigScope == ConfigScopes.TenantLocal && !OrganizationId.ForestWideOrgId.Equals(base.SessionSettings.CurrentOrganizationId))))
				{
					error = null;
					return true;
				}
				if (!rootId.IsDescendantOf(scope.Root))
				{
					error = new ADOperationException(DirectoryStrings.ExceptionSearchRootNotWithinScope(rootId.ToCanonicalName() ?? "<null>", scope.Root.ToCanonicalName() ?? "<null>"));
					return false;
				}
				if (!ADObjectId.Equals(rootId.DomainId, scope.Root.DomainId))
				{
					error = new ADOperationException(DirectoryStrings.ExceptionSearchRootChildDomain(rootId.DomainId.ToCanonicalName() ?? "<null>", scope.Root.DomainId.ToCanonicalName() ?? "<null>"));
					return false;
				}
			}
			error = null;
			return true;
		}

		private bool IsDeletedObjectId(ADObjectId id)
		{
			return id != null && (id.IsDescendantOf(ADDataSession.GetDomainDeletedObjectsContainer(this.GetConfigurationNamingContext())) || (id.DomainId != null && id.IsDescendantOf(ADDataSession.GetDomainDeletedObjectsContainer(id.DomainId))));
		}

		private void LogFailedOperation(PooledLdapConnection connection, DirectoryRequest request, string message, int resultCode)
		{
			LocalizedString localizedString;
			string text;
			QueryScope queryScope;
			string text2;
			connection.GetLoggingDataFromDirectoryRequest(request, out localizedString, out text, out queryScope, out text2);
			if (!(localizedString == DirectoryStrings.LdapSearch))
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_WRITE_FAILED, null, new object[]
				{
					localizedString,
					connection.ServerName,
					resultCode,
					message,
					text
				});
				return;
			}
			if (resultCode == 3 || resultCode == 85)
			{
				double totalSeconds;
				if (base.ServerTimeout != null)
				{
					totalSeconds = ((SearchRequest)request).TimeLimit.TotalSeconds;
				}
				else if (this.clientSideSearchTimeout != null)
				{
					totalSeconds = this.clientSideSearchTimeout.Value.TotalSeconds;
				}
				else
				{
					totalSeconds = ConnectionPoolManager.ClientSideSearchTimeout.TotalSeconds;
				}
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_LDAP_TIMEOUT, text, new object[]
				{
					connection.ServerName,
					totalSeconds,
					text,
					text2,
					queryScope,
					message
				});
				return;
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_SYNC_FAILED, null, new object[]
			{
				connection.ServerName,
				resultCode,
				text,
				text2,
				queryScope,
				message
			});
		}

		public void AnalyzeDirectoryError(PooledLdapConnection connection, DirectoryRequest request, DirectoryException de, int totalRetries, int retriesOnServer)
		{
			SearchRequest searchRequest = request as SearchRequest;
			bool usingOptimisticTimeout = searchRequest != null && searchRequest.TimeLimit == ADDataSession.OptimisticTimeout;
			ADErrorRecord aderrorRecord = connection.AnalyzeDirectoryError(de, usingOptimisticTimeout);
			this.UpdateServerSettings(connection, aderrorRecord);
			if (aderrorRecord.HandlingType == HandlingType.Throw)
			{
				this.LogFailedOperation(connection, request, aderrorRecord.Message, (int)aderrorRecord.LdapError);
				if (aderrorRecord.LdapError == LdapError.AlreadyExists || aderrorRecord.LdapError == LdapError.AttributeOrValueExists)
				{
					if (request is AddRequest)
					{
						throw this.PrepareADOperation(new ADObjectAlreadyExistsException(DirectoryStrings.ExceptionADOperationFailedAlreadyExist(connection.ServerName, ((AddRequest)request).DistinguishedName), de), connection, request);
					}
					if (request is ModifyDNRequest)
					{
						throw this.PrepareADOperation(new ADObjectAlreadyExistsException(DirectoryStrings.ExceptionADOperationFailedAlreadyExist(connection.ServerName, string.Format("{0},{1}", ((ModifyDNRequest)request).NewName, ((ModifyDNRequest)request).NewParentDistinguishedName)), de), connection, request);
					}
					if (request is ModifyRequest)
					{
						throw this.PrepareADOperation(new ADObjectEntryAlreadyExistsException(DirectoryStrings.ExceptionADOperationFailedEntryAlreadyExist(connection.ServerName, ((ModifyRequest)request).DistinguishedName), de), connection, request);
					}
				}
				else if (aderrorRecord.LdapError == LdapError.NoSuchObject)
				{
					if (request is DeleteRequest)
					{
						throw this.PrepareADOperation(new ADNoSuchObjectException(DirectoryStrings.ExceptionADOperationFailedNoSuchObject(connection.ServerName, ((DeleteRequest)request).DistinguishedName), de), connection, request);
					}
					if (request is ModifyRequest)
					{
						throw this.PrepareADOperation(new ADNoSuchObjectException(DirectoryStrings.ExceptionADOperationFailedNoSuchObject(connection.ServerName, ((ModifyRequest)request).DistinguishedName), de), connection, request);
					}
				}
				else if (aderrorRecord.LdapError == LdapError.NotAllowedOnNonleaf)
				{
					if (request is DeleteRequest)
					{
						throw this.PrepareADOperation(new ADRemoveContainerException(DirectoryStrings.ExceptionADOperationFailedRemoveContainer(connection.ServerName, ((DeleteRequest)request).DistinguishedName), de), connection, request);
					}
				}
				else
				{
					if (aderrorRecord.LdapError == LdapError.ConstraintViolation)
					{
						throw this.PrepareADOperation(new ADConstraintViolationException(connection.ServerName, aderrorRecord.Message, de), connection, request);
					}
					if (aderrorRecord.LdapError == LdapError.SizelimitExceeded)
					{
						if (request is SearchRequest)
						{
							throw this.PrepareADOperation(new ADSizelimitExceededException(DirectoryStrings.ExceptionSizelimitExceeded(connection.ServerName), de), connection, request);
						}
					}
					else if (aderrorRecord.LdapError == LdapError.AdminLimitExceeded)
					{
						if (8397 == aderrorRecord.NativeError && request is DeleteRequest)
						{
							throw new ADTreeDeleteNotFinishedException(connection.ServerName, de);
						}
						if (8228 == aderrorRecord.NativeError)
						{
							throw new AdminLimitExceededException(de);
						}
					}
					else
					{
						if (aderrorRecord.LdapError == LdapError.InvalidCredentials)
						{
							throw aderrorRecord.InnerException;
						}
						if (aderrorRecord.LdapError == LdapError.UnwillingToPerform && aderrorRecord.NativeError == 1325)
						{
							throw this.PrepareADOperation(new ADInvalidPasswordException(DirectoryStrings.ExceptionADInvalidPassword(connection.ServerName), de), connection, request);
						}
						if (aderrorRecord.LdapError == LdapError.UnwillingToPerform && aderrorRecord.NativeError == 1377)
						{
							throw this.PrepareADOperation(new ADNotAMemberException(DirectoryStrings.ExceptionADOperationFailedNotAMember(connection.ServerName), de), connection, request);
						}
						if (aderrorRecord.LdapError == LdapError.UnavailableCritExtension && aderrorRecord.NativeError == 8431)
						{
							throw this.PrepareADOperation(new ADVlvSizeLimitExceededException(DirectoryStrings.ExceptionADVlvSizeLimitExceeded(connection.ServerName, aderrorRecord.Message), de), connection, request);
						}
					}
				}
				throw this.PrepareADOperation(new ADOperationException(DirectoryStrings.ExceptionADOperationFailed(connection.ServerName, aderrorRecord.Message), de), connection, request);
			}
			if (aderrorRecord.LdapError == LdapError.TimelimitExceeded && base.ServerTimeout != null)
			{
				throw this.PrepareADOperation(new ADTimelimitExceededException(DirectoryStrings.ExceptionTimelimitExceeded(connection.ServerName, base.ServerTimeout.Value), de), connection, request);
			}
			bool flag = false;
			if (aderrorRecord.HandlingType == HandlingType.Retry)
			{
				if (retriesOnServer <= this.GetMaxRetriesPerServer())
				{
					flag = true;
				}
			}
			else if (aderrorRecord.HandlingType == HandlingType.RetryOnce)
			{
				if (totalRetries <= 1)
				{
					flag = true;
				}
				else
				{
					this.LogFailedOperation(connection, request, aderrorRecord.Message, (int)aderrorRecord.LdapError);
					if (aderrorRecord.LdapError == LdapError.UnavailableCritExtension && request is SearchRequest)
					{
						throw new ADInvalidHandleCookieException(DirectoryStrings.ExceptionADInvalidHandleCookie(connection.ServerName, aderrorRecord.Message), de);
					}
					throw new ADPossibleOperationException(DirectoryStrings.ExceptionADRetryOnceOperationFailed(connection.ServerName, aderrorRecord.Message), de);
				}
			}
			if (!flag)
			{
				throw new ADTransientException(DirectoryStrings.ExceptionADUnavailable(connection.ServerName), de);
			}
			if (request is SearchRequest)
			{
				ExTraceGlobals.ADFindTracer.TraceWarning<int, string, int>((long)this.GetHashCode(), "Retry {0} on server {1}. Total retry {2}", retriesOnServer, connection.ServerName, totalRetries);
				return;
			}
			ExTraceGlobals.ADSaveTracer.TraceWarning<int, string, int>((long)this.GetHashCode(), "Retry {0} on server {1}. Total retry {2}", retriesOnServer, connection.ServerName, totalRetries);
		}

		private ADOperationException PrepareADOperation(ADOperationException adopEx, PooledLdapConnection connection, DirectoryRequest request)
		{
			adopEx.ADRequest = request;
			return adopEx;
		}

		private int GetMaxRetriesPerServer()
		{
			if (ADDataSession.maxRetriesPerServer < 1)
			{
				ADDataSession.maxRetriesPerServer = Globals.GetIntValueFromRegistry("MaxRetriesPerServer", 5, this.GetHashCode());
			}
			return ADDataSession.maxRetriesPerServer;
		}

		private bool BlockInvalidSessionsEnabled()
		{
			int intValueFromRegistry = Globals.GetIntValueFromRegistry("SOFTWARE\\Microsoft\\ExchangeLabs", "BlockInvalidSessions", 0, this.GetHashCode());
			return intValueFromRegistry == 1;
		}

		private void UpdateFilterforSoftDeletedSearch(ADRawEntry dummyRawEntry, ref QueryFilter filter)
		{
			if (!base.SessionSettings.IncludeSoftDeletedObjects && (dummyRawEntry is ADRecipient || dummyRawEntry is MiniRecipient || dummyRawEntry is ReducedRecipient))
			{
				QueryFilter queryFilter;
				if (this.sessionSettings.IncludeInactiveMailbox)
				{
					queryFilter = new OrFilter(new QueryFilter[]
					{
						new NotFilter(new ExistsFilter(ADRecipientSchema.RecipientSoftDeletedStatus)),
						new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientSoftDeletedStatus, RecipientSoftDeletedStatusFlags.None),
						new BitMaskAndFilter(ADRecipientSchema.RecipientSoftDeletedStatus, 8UL)
					});
				}
				else
				{
					queryFilter = new OrFilter(new QueryFilter[]
					{
						new NotFilter(new ExistsFilter(ADRecipientSchema.RecipientSoftDeletedStatus)),
						new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientSoftDeletedStatus, 0)
					});
				}
				if (filter != null)
				{
					filter = new AndFilter(new QueryFilter[]
					{
						filter,
						queryFilter
					});
					return;
				}
				filter = queryFilter;
			}
		}

		internal void UpdateFilterforInactiveMailboxSearch(ADRawEntry dummyRawEntry, ref QueryFilter filter)
		{
			if (!base.SessionSettings.IncludeSoftDeletedObjects && base.SessionSettings.IncludeInactiveMailbox && (dummyRawEntry is ADRecipient || dummyRawEntry is MiniRecipient || dummyRawEntry is ReducedRecipient))
			{
				QueryFilter queryFilter = new OrFilter(new QueryFilter[]
				{
					new NotFilter(new ExistsFilter(ADRecipientSchema.RecipientSoftDeletedStatus)),
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientSoftDeletedStatus, RecipientSoftDeletedStatusFlags.None),
					new BitMaskAndFilter(ADRecipientSchema.RecipientSoftDeletedStatus, 8UL)
				});
				if (filter != null)
				{
					filter = new AndFilter(new QueryFilter[]
					{
						filter,
						queryFilter
					});
					return;
				}
				filter = queryFilter;
			}
		}

		private const CasTraceEventType ActiveDirectoryTraceEventType = CasTraceEventType.ActiveDirectory;

		private const int DnLimitPerRequest = 10000;

		private const int DefaultMaxRetriesPerServer = 5;

		private const string MsExchangeADAccessRegistryPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess";

		private const string MaxRetriesPerServerValueName = "MaxRetriesPerServer";

		private const string RegistryParameters = "SYSTEM\\CurrentControlSet\\Services\\ADDriver\\Parameters";

		protected const int m_ReadMultipleMaxBatchSize = 20;

		private static readonly TimeSpan OptimisticTimeout = TimeSpan.FromSeconds(15.0);

		private static readonly TimeSpan PessimisticTimeout = TimeSpan.FromMinutes(2.0);

		private readonly bool diag_enabled;

		private static int maxRetriesPerServer = -1;

		private static Tuple<int, ExDateTime> tickCountReference = new Tuple<int, ExDateTime>(Environment.TickCount, ExDateTime.Now);

		[NonSerialized]
		private static StreamWriter diag_logWriter;

		private static QueryFilter allTenantsFilter = new ExistsFilter(ADObjectSchema.ConfigurationUnit);

		private static QueryFilter rootOrgFilter = new NotFilter(ADDataSession.allTenantsFilter);

		protected static readonly SortBy SortByUsn = new SortBy(ADRecipientSchema.UsnChanged, SortOrder.Ascending);

		protected static readonly int RangedValueDefaultPageSize = 1500;

		private string ctorLogString;

		private string ctorLogStack;

		protected delegate void HashInserter<TData>(Hashtable hash, TData obj);

		protected delegate Result<TData> HashLookup<TKey, TData>(Hashtable hash, TKey key);

		private class UnfilterableObject : ADObject
		{
			public UnfilterableObject(ADObjectId id)
			{
				if (id == null)
				{
					throw new ArgumentNullException("id");
				}
				base.SetId(id);
			}

			internal override ADObjectSchema Schema
			{
				get
				{
					throw new NotImplementedException("The method or operation is not implemented.");
				}
			}

			internal override string MostDerivedObjectClass
			{
				get
				{
					throw new NotImplementedException("The method or operation is not implemented.");
				}
			}
		}

		[ObjectScope(new ConfigScopes[]
		{
			ConfigScopes.TenantLocal,
			ConfigScopes.TenantSubTree
		})]
		private class UnfilterableTenantObject : ADDataSession.UnfilterableObject
		{
			public UnfilterableTenantObject(ADObjectId id) : base(id)
			{
			}
		}
	}
}
