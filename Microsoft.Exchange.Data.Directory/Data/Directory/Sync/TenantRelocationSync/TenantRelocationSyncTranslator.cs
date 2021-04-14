using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class TenantRelocationSyncTranslator
	{
		public TenantRelocationSyncMappingDictionary Mappings { get; private set; }

		public TenantRelocationSyncData SyncConfigData { get; private set; }

		internal static IList<string> UndefinedAttributesToCopy
		{
			get
			{
				if (TenantRelocationSyncTranslator.undefinedAttributesToCopy == null)
				{
					TenantRelocationSyncTranslator.undefinedAttributesToCopy = new List<string>
					{
						"msExchTargetServerAdmins",
						"msExchTargetServerViewOnlyAdmins",
						"msExchTargetServerPartnerAdmins",
						"msExchTargetServerPartnerViewOnlyAdmins",
						"msExchOWASetPhotoURL",
						"msExchMaxSignupAddressesPerUser",
						"msExchDistributionListCountQuota",
						"msExchUMThrottlingPolicyState",
						"msExchUMFaxId",
						"msExchHostedContentFilterConfigLink",
						"msExchMalwareFilterConfigLink",
						"employeeID",
						"employeeType",
						"msExchFedLocalRecipientAddress",
						"msExchSignupAddresses",
						"msExchDeviceHealth",
						"msExchHygieneConfigurationLink",
						"msExchOfflineOrgIdHomeRealmRecord"
					};
				}
				return TenantRelocationSyncTranslator.undefinedAttributesToCopy;
			}
		}

		internal static IList<string> MandatoryDnSyntaxAttributes
		{
			get
			{
				if (TenantRelocationSyncTranslator.mandatoryDnSyntaxAttributes == null)
				{
					TenantRelocationSyncTranslator.mandatoryDnSyntaxAttributes = new List<string>
					{
						"offlineABServer",
						"offLineABContainers"
					};
				}
				return TenantRelocationSyncTranslator.mandatoryDnSyntaxAttributes;
			}
		}

		internal static IList<string> AttributesToSkip
		{
			get
			{
				if (TenantRelocationSyncTranslator.attributesToSkip == null)
				{
					TenantRelocationSyncTranslator.attributesToSkip = new List<string>
					{
						IADSecurityPrincipalSchema.Sid.LdapDisplayName,
						IADSecurityPrincipalSchema.SidHistory.LdapDisplayName,
						ADObjectSchema.ExchangeObjectIdRaw.LdapDisplayName,
						ADObjectSchema.CorrelationIdRaw.LdapDisplayName,
						ADObjectSchema.WhenCreatedRaw.LdapDisplayName,
						IADSecurityPrincipalSchema.SamAccountName.LdapDisplayName,
						ADUserSchema.PrimaryGroupId.LdapDisplayName,
						ADUserSchema.PasswordLastSetRaw.LdapDisplayName,
						ADUserSchema.UnicodePassword.LdapDisplayName,
						OrganizationSchema.SupportedSharedConfigurations.LdapDisplayName,
						ExchangeConfigurationUnitSchema.OrganizationStatus.LdapDisplayName
					};
				}
				return TenantRelocationSyncTranslator.attributesToSkip;
			}
		}

		public TenantRelocationSyncTranslator(TenantRelocationSyncData syncConfigData, ITopologyConfigurationSession sourceSession, ITopologyConfigurationSession targetSession)
		{
			this.SyncConfigData = syncConfigData;
			this.sourceSystemConfigurationSession = sourceSession;
			this.targetPartitionSession = targetSession;
			this.Mappings = new TenantRelocationSyncMappingDictionary();
		}

		private static ADObjectId ReplaceRoot(ADObjectId value, ADObjectId oldRoot, ADObjectId newRoot)
		{
			string distinguishedName = value.DistinguishedName;
			string distinguishedName2 = oldRoot.DistinguishedName;
			string distinguishedName3 = newRoot.DistinguishedName;
			if (!value.IsDescendantOf(oldRoot))
			{
				return null;
			}
			int num = distinguishedName.LastIndexOf(distinguishedName2, StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				return null;
			}
			if (num > 0)
			{
				return new ADObjectId(distinguishedName.Substring(0, num) + distinguishedName3);
			}
			return new ADObjectId(distinguishedName3);
		}

		internal ADObjectId MapDistinguishedName(ADObjectId value)
		{
			bool flag;
			bool flag2;
			return this.MapDistinguishedName(value, out flag, out flag2);
		}

		internal ADObjectId MapDistinguishedName(ADObjectId value, out bool isSyntactical, out bool isTenantSpecific)
		{
			isTenantSpecific = false;
			isSyntactical = false;
			if (this.SyncConfigData.Source.IsUnderTenantScope(value))
			{
				isTenantSpecific = true;
				DistinguishedNameMapItem distinguishedNameMapItem = this.Mappings.LookupBySourceDn(value.DistinguishedName);
				if (distinguishedNameMapItem != null && distinguishedNameMapItem.TargetDN != null)
				{
					return distinguishedNameMapItem.TargetDN;
				}
			}
			isSyntactical = true;
			return this.SyntacticallyMapDistinguishedName(value);
		}

		internal bool IsSpecialObject(ADObjectId value)
		{
			return value.Equals(this.SyncConfigData.ResourcePartitionConfigNc.GetChildId("Services").GetChildId("Microsoft Exchange"));
		}

		public ADObjectId SyntacticallyMapDistinguishedName(ADObjectId value)
		{
			if (value.IsDescendantOf(this.SyncConfigData.Source.TenantConfigurationUnitRoot))
			{
				return TenantRelocationSyncTranslator.ReplaceRoot(value, this.SyncConfigData.Source.TenantConfigurationUnitRoot, this.SyncConfigData.Target.TenantConfigurationUnitRoot);
			}
			if (value.IsDescendantOf(this.SyncConfigData.Source.TenantOrganizationUnit))
			{
				return TenantRelocationSyncTranslator.ReplaceRoot(value, this.SyncConfigData.Source.TenantOrganizationUnit, this.SyncConfigData.Target.TenantOrganizationUnit);
			}
			if (this.IsSpecialObject(value))
			{
				return TenantRelocationSyncTranslator.ReplaceRoot(value, this.SyncConfigData.Source.PartitionRoot, this.SyncConfigData.Target.PartitionRoot);
			}
			if (value.IsDescendantOf(this.SyncConfigData.ResourcePartitionConfigNc) && !value.IsDescendantOf(this.SyncConfigData.ResourcePartitionConfigNc.GetChildId("Schema")))
			{
				return value;
			}
			if (value.IsDescendantOf(this.SyncConfigData.Source.PartitionRoot))
			{
				return TenantRelocationSyncTranslator.ReplaceRoot(value, this.SyncConfigData.Source.PartitionRoot, this.SyncConfigData.Target.PartitionRoot);
			}
			throw new InvalidOperationException("no mapping is found");
		}

		internal ADRawEntry FindByCorrelationIdWrapper(Guid correlationId, params PropertyDefinition[] properties)
		{
			Guid[] correlationIds = new Guid[]
			{
				correlationId
			};
			Result<ADRawEntry>[] array = this.targetPartitionSession.FindByCorrelationIds(correlationIds, this.SyncConfigData.Target.TenantConfigurationUnit, TenantRelocationSyncCoordinator.identityProperties);
			if (array.Length != 1 || array[0].Error != null)
			{
				return null;
			}
			return array[0].Data;
		}

		internal Result<ADRawEntry>[] FindByCorrelationIdsWrapper(Guid[] correlationIds, params PropertyDefinition[] properties)
		{
			return this.targetPartitionSession.FindByCorrelationIds(correlationIds, this.SyncConfigData.Target.TenantConfigurationUnit, TenantRelocationSyncCoordinator.identityProperties);
		}

		internal void ResolveSourceDistinguishedNamesIntoCache(ADObjectId[] distinguishedNames)
		{
			List<ADObjectId> list = new List<ADObjectId>();
			List<ADObjectId> list2 = new List<ADObjectId>();
			foreach (ADObjectId adobjectId in distinguishedNames)
			{
				if (!string.IsNullOrEmpty(adobjectId.DistinguishedName) && !adobjectId.IsDescendantOf(this.SyncConfigData.Source.PartitionRoot))
				{
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "ResolveSourceDistinguishedNamesIntoCache: foreign DN is skipped: {0}", adobjectId);
				}
				else
				{
					DistinguishedNameMapItem distinguishedNameMapItem = this.Mappings.LookupBySourceADObjectId(adobjectId);
					if (distinguishedNameMapItem != null && distinguishedNameMapItem.TargetDN != null && !string.IsNullOrEmpty(distinguishedNameMapItem.TargetDN.DistinguishedName))
					{
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<ADObjectId, Guid, string>((long)this.GetHashCode(), "ResolveSourceDistinguishedNamesIntoCache: DN found in cache, skipped: {0}, correlation Id: {1}, target DN:{2}", adobjectId, distinguishedNameMapItem.CorrelationId, distinguishedNameMapItem.TargetDN.DistinguishedName);
					}
					else if (string.IsNullOrEmpty(adobjectId.DistinguishedName))
					{
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "ResolveSourceDistinguishedNamesIntoCache: Found DN with only GUID {0}", adobjectId);
						if (this.SyncConfigData.Source.IsConfigurationUnitUnderConfigNC)
						{
							list2.Add(adobjectId);
						}
						list.Add(adobjectId);
					}
					else if (adobjectId.IsDescendantOf(this.SyncConfigData.Source.PartitionConfigNcRoot))
					{
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "ResolveSourceDistinguishedNamesIntoCache: Found DN belonging to source config NC: {0}", adobjectId);
						list2.Add(adobjectId);
					}
					else
					{
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "ResolveSourceDistinguishedNamesIntoCache: Found DN belonging to source domain NC: {0}", adobjectId);
						list.Add(adobjectId);
					}
				}
			}
			List<Guid> list3 = new List<Guid>();
			if (list.Count > 0)
			{
				this.sourceSystemConfigurationSession.UseConfigNC = false;
				Result<ADRawEntry>[] array = this.sourceSystemConfigurationSession.FindByADObjectIds(list.ToArray(), null);
				foreach (Result<ADRawEntry> result in array)
				{
					if (result.Data != null)
					{
						Guid guid = (Guid)result.Data[ADObjectSchema.CorrelationId];
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "ResolveSourceDistinguishedNamesIntoCache: FindByADObjectIds found dn in domain NC: {0}, correlation ID={1}", result.Data.Id.DistinguishedName, guid);
						list3.Add(guid);
						this.Mappings.Insert(result.Data.Id, null, guid);
					}
				}
			}
			if (list2.Count > 0)
			{
				this.sourceSystemConfigurationSession.UseConfigNC = true;
				Result<ADRawEntry>[] array = this.sourceSystemConfigurationSession.FindByADObjectIds(list2.ToArray(), null);
				this.sourceSystemConfigurationSession.UseConfigNC = false;
				foreach (Result<ADRawEntry> result2 in array)
				{
					if (result2.Data != null)
					{
						Guid guid2 = (Guid)result2.Data[ADObjectSchema.CorrelationId];
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "ResolveSourceDistinguishedNamesIntoCache: FindByADObjectIds found dn in config NC: {0}, correlation ID={1}", result2.Data.Id.DistinguishedName, guid2);
						list3.Add(guid2);
						this.Mappings.Insert(result2.Data.Id, null, guid2);
					}
				}
			}
			Result<ADRawEntry>[] array4 = this.FindByCorrelationIdsWrapper(list3.ToArray(), TenantRelocationSyncCoordinator.identityProperties);
			foreach (Result<ADRawEntry> result3 in array4)
			{
				if (result3.Error != null)
				{
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "ResolveSourceDistinguishedNamesIntoCache: couldn't find object with a correlation ID, error = {0}", result3.Error.ToString());
				}
				else
				{
					Guid guid3 = (Guid)result3.Data[ADObjectSchema.CorrelationId];
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "ResolveSourceDistinguishedNamesIntoCache: FindByCorrelationIds found objects in target forest: {0}, correlation ID={1}", result3.Data.Id.DistinguishedName, guid3);
					DistinguishedNameMapItem distinguishedNameMapItem2 = this.Mappings.LookupByCorrelationGuid(guid3);
					this.Mappings.Remove(distinguishedNameMapItem2);
					this.Mappings.Insert(distinguishedNameMapItem2.SourceDN, result3.Data.Id, guid3);
				}
			}
		}

		internal bool IsSkippedProperty(ADPropertyDefinition prop)
		{
			if (prop == null || prop.IsBackLink || (prop.Flags & ADPropertyDefinitionFlags.Calculated) != ADPropertyDefinitionFlags.None || string.IsNullOrEmpty(prop.LdapDisplayName))
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "TenantRelocationSyncTranslator: attribute {0} is skipped due to basic check", (prop != null) ? prop.LdapDisplayName : "<null>");
				return true;
			}
			if (TenantRelocationSyncTranslator.AttributesToSkip.Contains(prop.LdapDisplayName, StringComparer.InvariantCultureIgnoreCase) || TenantRelocationRequestSchema.RelocationSpecificProperties.Contains(prop))
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "TenantRelocationSyncTranslator: attribute {0} is skipped due to special attribute", (prop != null) ? prop.LdapDisplayName : "<null>");
				return true;
			}
			return false;
		}

		private void ConvertAndAddDirectoryAttribute(ADPropertyDefinition prop, object value, DirectoryAttributeOperation op, DirectoryAttributeModificationCollection modCollection)
		{
			DirectoryAttributeModification directoryAttributeModification = new DirectoryAttributeModification();
			directoryAttributeModification.Name = prop.LdapDisplayName;
			directoryAttributeModification.Operation = op;
			if (prop.IsMultivalued)
			{
				object[] array = (object[])value;
				foreach (object value2 in array)
				{
					ADValueConvertor.ConvertAndAddValueToDirectoryAttribute(value2, prop, directoryAttributeModification, false);
				}
			}
			else
			{
				ADValueConvertor.ConvertAndAddValueToDirectoryAttribute(value, prop, directoryAttributeModification, false);
			}
			modCollection.Add(directoryAttributeModification);
		}

		private bool MapDnValues(ADObjectId sourceObjectId, ADPropertyDefinition prop, DirectoryAttribute da, DirectoryAttribute targetDirectoryAttribute)
		{
			bool flag = this.SyncConfigData.IsSourceSoftLinkEnabled && prop == ADRecipientSchema.HomeMTA && prop.LdapDisplayName.Equals(da.Name, StringComparison.OrdinalIgnoreCase);
			bool flag2 = this.SyncConfigData.IsSourceSoftLinkEnabled && prop.IsSoftLinkAttribute && prop.SoftLinkShadowProperty.LdapDisplayName.Equals(da.Name, StringComparison.OrdinalIgnoreCase) && !flag;
			bool flag3 = this.SyncConfigData.IsTargetSoftLinkEnabled && prop.IsSoftLinkAttribute && !flag;
			if (flag && !this.SyncConfigData.IsTargetSoftLinkEnabled)
			{
				return false;
			}
			if (flag3)
			{
				targetDirectoryAttribute.Name = prop.SoftLinkShadowProperty.LdapDisplayName;
			}
			else
			{
				targetDirectoryAttribute.Name = prop.LdapDisplayName;
			}
			object[] values = da.GetValues(flag2 ? typeof(byte[]) : typeof(string));
			foreach (object obj in values)
			{
				if (!prop.Type.Equals(typeof(ADObjectId)))
				{
					bool result;
					if (this.IsDnStringSyntax(prop))
					{
						ADObjectIdWithString adobjectIdWithString = ADObjectIdWithString.ParseDNStringSyntax((string)obj, null);
						if (!adobjectIdWithString.ObjectIdValue.IsDeleted)
						{
							ADObjectIdWithString adobjectIdWithString2 = new ADObjectIdWithString(adobjectIdWithString.StringValue, this.MapDistinguishedName(adobjectIdWithString.ObjectIdValue));
							ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "MapDnValues: attribute {0} with value {1} is added", prop.LdapDisplayName, adobjectIdWithString2.ToString());
							targetDirectoryAttribute.Add(adobjectIdWithString2.ToString());
							goto IL_313;
						}
						result = false;
					}
					else
					{
						if (!this.IsDnBinarySyntax(prop))
						{
							goto IL_313;
						}
						DNWithBinary dnwithBinary = DNWithBinary.Parse((string)obj);
						ADObjectId adobjectId = new ADObjectId(dnwithBinary.DistinguishedName);
						if (!adobjectId.IsDeleted)
						{
							ADObjectId adobjectId2 = this.MapDistinguishedName(adobjectId);
							DNWithBinary dnwithBinary2 = new DNWithBinary(adobjectId2.DistinguishedName, dnwithBinary.Binary);
							ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "MapDnValues: attribute {0} with value {1} is added", prop.LdapDisplayName, dnwithBinary2.ToString());
							targetDirectoryAttribute.Add(dnwithBinary2.ToString());
							goto IL_313;
						}
						result = false;
					}
					return result;
				}
				if (flag2 && flag3)
				{
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "MapDnValues: attribute {0} is soft link on both source and target, parsing and re-encoding", da.Name);
					ADObjectId adobjectId3 = ADObjectId.FromSoftLinkValue((byte[])obj, sourceObjectId, null);
					targetDirectoryAttribute.Add(adobjectId3.ToSoftLinkValue());
				}
				else
				{
					ADObjectId adobjectId4;
					if (flag2)
					{
						adobjectId4 = ADObjectId.FromSoftLinkValue((byte[])obj, sourceObjectId, null);
						adobjectId4 = ADObjectIdResolutionHelper.ResolveDN(adobjectId4);
					}
					else
					{
						adobjectId4 = ADObjectId.ParseExtendedDN((string)obj);
					}
					if (!string.IsNullOrEmpty(adobjectId4.DistinguishedName) && !adobjectId4.IsDeleted)
					{
						ADObjectId adobjectId5 = this.MapDistinguishedName(adobjectId4);
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "MapDnValues: attribute {0} with value {1} is added", prop.LdapDisplayName, adobjectId5.ToDNString());
						if (flag3)
						{
							if (!adobjectId5.IsDescendantOf(this.SyncConfigData.ResourcePartitionConfigNc))
							{
								throw new InvalidOperationException("soft link value not pointing to resource config NC");
							}
							adobjectId5 = ADObjectIdResolutionHelper.ResolveDN(adobjectId5);
							targetDirectoryAttribute.Add(adobjectId5.ToSoftLinkValue());
						}
						else
						{
							targetDirectoryAttribute.Add(adobjectId5.ToDNString());
						}
					}
				}
				IL_313:;
			}
			return true;
		}

		public ADObjectId ResolveSourceDistinguishedName(ADObjectId dn, bool force)
		{
			ADObjectId[] dns = new ADObjectId[]
			{
				dn
			};
			if (force)
			{
				DistinguishedNameMapItem distinguishedNameMapItem = this.Mappings.LookupBySourceADObjectId(dn);
				if (distinguishedNameMapItem != null)
				{
					this.Mappings.Remove(distinguishedNameMapItem);
				}
			}
			DistinguishedNameMapItem[] array;
			ADObjectId[] array2;
			ADObjectId[] array3;
			this.ResolveSourceDistinguishedNames(dns, out array, out array2, out array3);
			if (array.Length == 1)
			{
				return array[0].TargetDN;
			}
			return null;
		}

		public void ResolveSourceDistinguishedNames(ADObjectId[] dns, out DistinguishedNameMapItem[] resolvedDNs, out ADObjectId[] unresolvedDNs, out ADObjectId[] nonTenantSpecifiedDNs)
		{
			List<DistinguishedNameMapItem> list = new List<DistinguishedNameMapItem>();
			List<ADObjectId> list2 = new List<ADObjectId>();
			List<ADObjectId> list3 = new List<ADObjectId>();
			foreach (ADObjectId adobjectId in dns)
			{
				if (!string.IsNullOrEmpty(adobjectId.DistinguishedName) && !this.SyncConfigData.Source.IsUnderTenantScope(adobjectId))
				{
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "ResolveSourceDistinguishedNames found non tenant specific DN: {0}", adobjectId.DistinguishedName);
					list3.Add(adobjectId);
				}
				else
				{
					DistinguishedNameMapItem distinguishedNameMapItem = this.Mappings.LookupBySourceADObjectId(adobjectId);
					if (distinguishedNameMapItem == null || distinguishedNameMapItem.TargetDN == null)
					{
						list2.Add(adobjectId);
					}
					else
					{
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "ResolveSourceDistinguishedNames found the DN {0} in the mapping cache", adobjectId.DistinguishedName);
						list.Add(distinguishedNameMapItem);
					}
				}
			}
			if (list2.Count > 0)
			{
				this.ResolveSourceDistinguishedNamesIntoCache(list2.ToArray());
				List<ADObjectId> list4 = new List<ADObjectId>();
				foreach (ADObjectId adobjectId2 in list2)
				{
					DistinguishedNameMapItem distinguishedNameMapItem2 = this.Mappings.LookupBySourceADObjectId(adobjectId2);
					if (distinguishedNameMapItem2 == null || distinguishedNameMapItem2.TargetDN == null)
					{
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "ResolveSourceDistinguishedNames cannot find the mapping for DN {0} after resolving against DC", adobjectId2.DistinguishedName);
						list4.Add(adobjectId2);
					}
					else
					{
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "ResolveSourceDistinguishedNames found the DN {0} in the mapping cache", adobjectId2.DistinguishedName);
						list.Add(distinguishedNameMapItem2);
					}
					list2 = list4;
				}
			}
			resolvedDNs = list.ToArray();
			unresolvedDNs = list2.ToArray();
			nonTenantSpecifiedDNs = list3.ToArray();
		}

		private bool IsApplicableChange(long localChangedUsn, ADPropertyDefinition prop, bool isDeleted, RequestType requestType, long uSN)
		{
			if (this.IsSkippedProperty(prop))
			{
				return false;
			}
			if (isDeleted && requestType == RequestType.AddPlaceHolder)
			{
				return false;
			}
			switch (requestType)
			{
			case RequestType.AddPlaceHolder:
				if (this.IsDnSyntaxAttribute(prop) && prop != ADObjectSchema.ObjectCategory && !TenantRelocationSyncTranslator.MandatoryDnSyntaxAttributes.Contains(prop.LdapDisplayName, StringComparer.InvariantCultureIgnoreCase))
				{
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "IsApplicableChange: attribute {0} is skipped on place holder object", (prop != null) ? prop.LdapDisplayName : "<null>");
					return false;
				}
				break;
			case RequestType.ModifyFullObject:
			case RequestType.ModifyIncremental:
				if (prop == ADObjectSchema.RawName || prop == ADObjectSchema.ObjectCategory || prop == ADObjectSchema.ObjectClass || prop.LdapDisplayName.Equals("systemFlags", StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "IsApplicableChange: attribute {0} is skipped because it is system-only and should not be updated", (prop != null) ? prop.LdapDisplayName : "<null>");
					return false;
				}
				break;
			}
			return true;
		}

		internal bool IsDnBinarySyntax(ADPropertyDefinition prop)
		{
			ADSchemaAttributeObject adschemaAttributeObjectByLdapDisplayName = ADSchemaDataProvider.Instance.GetADSchemaAttributeObjectByLdapDisplayName(prop.LdapDisplayName);
			return adschemaAttributeObjectByLdapDisplayName.DataSyntax == DataSyntax.DNBinary;
		}

		internal bool IsDnStringSyntax(ADPropertyDefinition prop)
		{
			ADSchemaAttributeObject adschemaAttributeObjectByLdapDisplayName = ADSchemaDataProvider.Instance.GetADSchemaAttributeObjectByLdapDisplayName(prop.LdapDisplayName);
			return adschemaAttributeObjectByLdapDisplayName.DataSyntax == DataSyntax.DNString;
		}

		internal bool IsRegularDnSyntax(ADPropertyDefinition prop)
		{
			ADSchemaAttributeObject adschemaAttributeObjectByLdapDisplayName = ADSchemaDataProvider.Instance.GetADSchemaAttributeObjectByLdapDisplayName(prop.LdapDisplayName);
			return adschemaAttributeObjectByLdapDisplayName.DataSyntax == DataSyntax.DSDN || adschemaAttributeObjectByLdapDisplayName.DataSyntax == DataSyntax.ORName;
		}

		internal bool IsDnSyntaxAttribute(ADPropertyDefinition prop)
		{
			return this.IsRegularDnSyntax(prop) || this.IsDnStringSyntax(prop) || this.IsDnBinarySyntax(prop);
		}

		private bool GenerateModifyRequestDiretoryAttributeDelegate(ADObjectId sourceObjectId, RequestType requestType, ADPropertyDefinition prop, DirectoryAttribute da, object result, UpdateData mData)
		{
			ModifyRequest modifyRequest = (ModifyRequest)result;
			DirectoryAttributeModification directoryAttributeModification = new DirectoryAttributeModification();
			directoryAttributeModification.Operation = DirectoryAttributeOperation.Replace;
			directoryAttributeModification.Name = da.Name;
			if (CustomPropertyHandlers.Instance.Handlers.ContainsKey(prop.LdapDisplayName))
			{
				CustomPropertyHandlers.Instance.Handlers[prop.LdapDisplayName].UpdateModifyRequestForTarget(this, da, ref directoryAttributeModification);
				modifyRequest.Modifications.Add(directoryAttributeModification);
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "GenerateModifyRequestDiretoryAttributeDelegate: Added custom attribute: {0}", prop.LdapDisplayName);
				return true;
			}
			if (prop == ADRecipientSchema.ConfigurationXMLRaw)
			{
				mData.SourceUserConfigXMLStatus = ((da.Count == 0) ? UpdateData.SourceStatus.Removed : UpdateData.SourceStatus.Updated);
				return false;
			}
			if (this.IsDnSyntaxAttribute(prop))
			{
				if (prop == OfflineAddressBookSchema.Server)
				{
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "GenerateModifyRequestDiretoryAttributeDelegate: ignore attribute: {0}", prop.LdapDisplayName);
					return false;
				}
				bool flag = this.MapDnValues(sourceObjectId, prop, da, directoryAttributeModification);
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, int, bool>((long)this.GetHashCode(), "GenerateModifyRequestDiretoryAttributeDelegate: add mapped DN values: attribute {0}, value count:{1}, hasValues:{2}", prop.LdapDisplayName, directoryAttributeModification.Count, flag);
				if (!flag)
				{
					return false;
				}
			}
			else
			{
				object[] array = new object[da.Count];
				da.CopyTo(array, 0);
				foreach (object obj in array)
				{
					if (obj is string)
					{
						directoryAttributeModification.Add((string)obj);
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "GenerateModifyRequestDiretoryAttributeDelegate: add string value: attribute {0}, value:{1}", prop.LdapDisplayName, obj.ToString());
					}
					else
					{
						directoryAttributeModification.Add((byte[])obj);
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "GenerateModifyRequestDiretoryAttributeDelegate: add binary value: attribute {0}", prop.LdapDisplayName);
					}
				}
			}
			if (directoryAttributeModification.Count == 0)
			{
				if (!prop.IsSoftLinkAttribute)
				{
					DirectoryAttributeModification directoryAttributeModification2 = new DirectoryAttributeModification();
					directoryAttributeModification2.Name = da.Name;
					directoryAttributeModification2.Operation = DirectoryAttributeOperation.Replace;
					object dummyValue = this.GetDummyValue(prop);
					if (dummyValue is string)
					{
						directoryAttributeModification2.Add((string)dummyValue);
					}
					else
					{
						directoryAttributeModification2.Add((byte[])dummyValue);
					}
					modifyRequest.Modifications.Add(directoryAttributeModification2);
				}
				directoryAttributeModification.Operation = DirectoryAttributeOperation.Delete;
			}
			modifyRequest.Modifications.Add(directoryAttributeModification);
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, DirectoryAttributeOperation, int>((long)this.GetHashCode(), "GenerateModifyRequestDiretoryAttributeDelegate: add item: attribute {0}, operation {1}, value count {2} ", directoryAttributeModification.Name, directoryAttributeModification.Operation, directoryAttributeModification.Count);
			return true;
		}

		private bool GenerateModifyRequestLinkMetaDataHandler(RequestType requestType, ADPropertyDefinition prop, LinkMetadata lmd, object result, UpdateData mData)
		{
			ModifyRequest modifyRequest = (ModifyRequest)result;
			DirectoryAttributeModification directoryAttributeModification = new DirectoryAttributeModification();
			directoryAttributeModification.Name = lmd.AttributeName;
			ADObjectId value = new ADObjectId(lmd.TargetDistinguishedName);
			ADObjectId adobjectId = this.MapDistinguishedName(value);
			object obj;
			if (prop.Type.Equals(typeof(ADObjectId)))
			{
				if (prop.IsSoftLinkAttribute && !this.SyncConfigData.IsSourceSoftLinkEnabled && this.SyncConfigData.IsTargetSoftLinkEnabled)
				{
					adobjectId = ADObjectIdResolutionHelper.ResolveSoftLink(adobjectId);
					directoryAttributeModification.Name = prop.SoftLinkShadowProperty.LdapDisplayName;
					obj = adobjectId.ToSoftLinkValue();
					directoryAttributeModification.Add((byte[])obj);
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "GenerateModifyRequestLinkMetaDataHandler: add soft link: attribute {0}, value:{1}", directoryAttributeModification.Name, adobjectId.DistinguishedName);
				}
				else
				{
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "GenerateModifyRequestLinkMetaDataHandler: add: attribute {0}, value:{1}", prop.LdapDisplayName, adobjectId.DistinguishedName);
					directoryAttributeModification.Add(adobjectId.DistinguishedName);
					obj = adobjectId.DistinguishedName;
				}
			}
			else if (this.IsDnBinarySyntax(prop))
			{
				DNWithBinary dnwithBinary = new DNWithBinary(adobjectId.DistinguishedName, lmd.Data);
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "GenerateModifyRequestLinkMetaDataHandler: add: attribute {0}, value:{1}", prop.LdapDisplayName, dnwithBinary.ToString());
				obj = dnwithBinary.ToString();
				directoryAttributeModification.Add((string)obj);
			}
			else
			{
				if (!this.IsDnStringSyntax(prop))
				{
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "GenerateModifyRequestLinkMetaDataHandler: skip: attribute {0}, value:{1}", prop.LdapDisplayName, adobjectId.DistinguishedName);
					return false;
				}
				UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
				string @string = unicodeEncoding.GetString(lmd.Data);
				ADObjectIdWithString adobjectIdWithString = new ADObjectIdWithString(@string, adobjectId);
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "GenerateModifyRequestLinkMetaDataHandler: add: attribute {0}, value:{1}", prop.LdapDisplayName, adobjectIdWithString.ToString());
				obj = adobjectIdWithString.ToString();
				directoryAttributeModification.Add((string)obj);
			}
			if (lmd.IsDeleted)
			{
				DirectoryAttributeModification directoryAttributeModification2 = new DirectoryAttributeModification();
				directoryAttributeModification2.Operation = DirectoryAttributeOperation.Add;
				directoryAttributeModification2.Name = directoryAttributeModification.Name;
				if (obj is string)
				{
					directoryAttributeModification2.Add((string)obj);
				}
				else
				{
					directoryAttributeModification2.Add((byte[])obj);
				}
				modifyRequest.Modifications.Add(directoryAttributeModification2);
				directoryAttributeModification.Operation = DirectoryAttributeOperation.Delete;
			}
			else
			{
				ADSchemaAttributeObject adschemaAttributeObjectByLdapDisplayName = ADSchemaDataProvider.Instance.GetADSchemaAttributeObjectByLdapDisplayName(prop.LdapDisplayName);
				directoryAttributeModification.Operation = (adschemaAttributeObjectByLdapDisplayName.IsSingleValued ? DirectoryAttributeOperation.Replace : DirectoryAttributeOperation.Add);
			}
			modifyRequest.Modifications.Add(directoryAttributeModification);
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, DirectoryAttributeOperation>((long)this.GetHashCode(), "GenerateModifyRequestLinkMetaDataHandler: add item: attribute {0}, op:{1}", directoryAttributeModification.Name, directoryAttributeModification.Operation);
			return true;
		}

		private DirectoryAttribute GetDirectoryAttributeByName(DirectoryAttribute[] attributeList, string name)
		{
			foreach (DirectoryAttribute directoryAttribute in attributeList)
			{
				if (directoryAttribute.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return directoryAttribute;
				}
			}
			return null;
		}

		private ADPropertyDefinition HandleUndefinedAttributesToCopy(string attributeName)
		{
			if (TenantRelocationSyncTranslator.UndefinedAttributesToCopy.Contains(attributeName, StringComparer.InvariantCultureIgnoreCase))
			{
				ADSchemaAttributeObject adschemaAttributeObjectByLdapDisplayName = ADSchemaDataProvider.Instance.GetADSchemaAttributeObjectByLdapDisplayName(attributeName);
				object obj;
				Type type = TenantRelocationSyncTranslator.DataSyntaxToType(adschemaAttributeObjectByLdapDisplayName.DataSyntax, out obj);
				ADPropertyDefinitionFlags adpropertyDefinitionFlags = (type == typeof(byte[])) ? ADPropertyDefinitionFlags.Binary : ADPropertyDefinitionFlags.None;
				if (obj != null)
				{
					adpropertyDefinitionFlags |= ADPropertyDefinitionFlags.PersistDefaultValue;
				}
				return new ADPropertyDefinition(Guid.NewGuid().ToString() + attributeName, ExchangeObjectVersion.Current, type, attributeName, adpropertyDefinitionFlags, obj, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
			}
			return null;
		}

		private void HandleUnknownAttributes(string attributeName, ADRawEntry obj)
		{
			ADSchemaAttributeObject adschemaAttributeObjectByLdapDisplayName = ADSchemaDataProvider.Instance.GetADSchemaAttributeObjectByLdapDisplayName(attributeName);
			if (adschemaAttributeObjectByLdapDisplayName != null && (adschemaAttributeObjectByLdapDisplayName.SystemFlags & SystemFlagsEnum.Category1) == SystemFlagsEnum.Category1)
			{
				return;
			}
			if (TenantRelocationSyncTranslator.AttributesToSkip.Contains(attributeName))
			{
				return;
			}
			throw new TenantRelocationException(RelocationError.ADUnkownSchemaAttribute, DirectoryStrings.ExceptionUnknownDirectoryAttribute(attributeName, obj.Id.DistinguishedName));
		}

		private bool LoopThroughAllAttributes(TenantRelocationSyncObject obj, long uSN, object retObject, TenantRelocationSyncTranslator.DirectoryAttributeHandler attributehandler, TenantRelocationSyncTranslator.LinkMetadataHandler linkHandler, UpdateData mData)
		{
			RequestType requestType = mData.RequestType;
			bool result = false;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, RequestType, long>((long)this.GetHashCode(), "LoopThroughAllAttributes entered: dn={0}, requestType={1}, uSN={2}", obj.Id.DistinguishedName, requestType, uSN);
			mData.SourceUserConfigXML = obj.ConfigurationXMLRaw;
			MultiValuedProperty<AttributeMetadata> multiValuedProperty = (MultiValuedProperty<AttributeMetadata>)obj[ADRecipientSchema.AttributeMetadata];
			if (obj.RawLdapSearchResult != null && multiValuedProperty != null)
			{
				foreach (AttributeMetadata attributeMetadata in multiValuedProperty)
				{
					if (attributeMetadata.LocalUpdateSequenceNumber < uSN)
					{
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "LoopThroughAllAttributes: attribute {0} is skipped on incremental update because it is been applied before", attributeMetadata.AttributeName);
					}
					else
					{
						DirectoryAttribute directoryAttribute = this.GetDirectoryAttributeByName(obj.RawLdapSearchResult, attributeMetadata.AttributeName);
						if (directoryAttribute == null)
						{
							directoryAttribute = new DirectoryAttribute(attributeMetadata.AttributeName, new object[0]);
						}
						ADPropertyDefinition adpropertyDefinition = null;
						bool arg;
						bool flag;
						TenantRelocationSyncUnionSchema.Instance.TryGetPropertyDefinitionByLdapDisplayName(attributeMetadata.AttributeName, out adpropertyDefinition, out arg, out flag);
						if (attributeMetadata.AttributeName.Equals(ADObjectSchema.NTSecurityDescriptor.LdapDisplayName, StringComparison.InvariantCultureIgnoreCase))
						{
							mData.IsNtSecurityDescriptorChanged = true;
						}
						else
						{
							if (adpropertyDefinition == null)
							{
								adpropertyDefinition = this.HandleUndefinedAttributesToCopy(attributeMetadata.AttributeName);
							}
							if (adpropertyDefinition == null)
							{
								if (directoryAttribute.Count != 0 || uSN != 0L)
								{
									this.HandleUnknownAttributes(attributeMetadata.AttributeName, obj);
								}
								ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "LoopThroughAllAttributes: cannot find attribute {0} in propery definiton, skipped", attributeMetadata.AttributeName);
							}
							else
							{
								ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, bool, bool>((long)this.GetHashCode(), "LoopThroughAllAttributes: found attribute {0} in propery definiton, isSoftLink:{1}, isShadow:{2}", attributeMetadata.AttributeName, arg, flag);
								if (!this.IsApplicableChange(attributeMetadata.LocalUpdateSequenceNumber, adpropertyDefinition, directoryAttribute.Count == 0, requestType, uSN))
								{
									ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "LoopThroughAllAttributes: attribute {0} is skipped", attributeMetadata.AttributeName);
								}
								else
								{
									if (flag)
									{
										ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "LoopThroughAllAttributes: shadow attribute found {0}, its metadata captured", attributeMetadata.AttributeName);
										mData.ShadowMetadata.Add(new PropertyMetaData(attributeMetadata.AttributeName, attributeMetadata.LastWriteTime));
									}
									if (attributehandler(obj, requestType, adpropertyDefinition, directoryAttribute, retObject, mData))
									{
										result = true;
									}
								}
							}
						}
					}
				}
			}
			if (obj.LinkValueMetadata != null)
			{
				foreach (LinkMetadata linkMetadata in obj.LinkValueMetadata)
				{
					ADPropertyDefinition adpropertyDefinition2 = null;
					bool flag2;
					bool flag3;
					TenantRelocationSyncUnionSchema.Instance.TryGetPropertyDefinitionByLdapDisplayName(linkMetadata.AttributeName, out adpropertyDefinition2, out flag2, out flag3);
					if (adpropertyDefinition2 == null)
					{
						adpropertyDefinition2 = this.HandleUndefinedAttributesToCopy(linkMetadata.AttributeName);
					}
					if (adpropertyDefinition2 == null)
					{
						this.HandleUnknownAttributes(linkMetadata.AttributeName, obj);
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "LoopThroughAllAttributes: cannot find attribute {0} in propery definiton, skipped", linkMetadata.AttributeName);
					}
					else if (!this.IsApplicableChange(linkMetadata.LocalUpdateSequenceNumber, adpropertyDefinition2, linkMetadata.IsDeleted, requestType, uSN))
					{
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "LoopThroughAllAttributes: {0} is skipped", linkMetadata.AttributeName);
					}
					else
					{
						if (flag3)
						{
							ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "LoopThroughAllAttributes: shadow attribute found {0}(link), its metadata captured", linkMetadata.AttributeName);
							mData.ShadowMetadata.Add(new PropertyMetaData(linkMetadata.AttributeName, linkMetadata.LastWriteTime));
						}
						if (linkHandler(obj, requestType, adpropertyDefinition2, linkMetadata, retObject, mData))
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		internal ModifyRequest GenerateModifyRequest(TenantRelocationSyncObject obj, RequestType requestType, long uSN, out ADObjectId rootId, out UpdateData mData)
		{
			ModifyRequest modifyRequest = new ModifyRequest();
			rootId = this.MapDistinguishedName(obj.Id);
			modifyRequest.DistinguishedName = rootId.DistinguishedName;
			mData = new UpdateData(requestType);
			bool flag = false;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug((long)this.GetHashCode(), "GenerateModifyRequest entered: dn={0}, requestType={1}, uSN={2}, mapped DN={3}", new object[]
			{
				obj.Id.DistinguishedName,
				requestType,
				uSN,
				rootId.DistinguishedName
			});
			TenantRelocationSyncTranslator.DirectoryAttributeHandler attributehandler = (TenantRelocationSyncObject syncObj, RequestType r, ADPropertyDefinition prop, DirectoryAttribute da, object result, UpdateData mData1) => this.GenerateModifyRequestDiretoryAttributeDelegate(syncObj.Id, r, prop, da, result, mData1);
			TenantRelocationSyncTranslator.LinkMetadataHandler linkHandler = (TenantRelocationSyncObject syncObj, RequestType rt, ADPropertyDefinition prop, LinkMetadata lmd, object result, UpdateData mData2) => this.GenerateModifyRequestLinkMetaDataHandler(rt, prop, lmd, result, mData2);
			flag |= this.LoopThroughAllAttributes(obj, uSN, modifyRequest, attributehandler, linkHandler, mData);
			ExTraceGlobals.TenantRelocationTracer.TraceDebug((long)this.GetHashCode(), "GenerateModifyRequest finished: dn={0}, requestType={1}, uSN={2}, change returned: {3}", new object[]
			{
				obj.Id.DistinguishedName,
				requestType,
				uSN,
				flag
			});
			ICustomObjectHandler customObjectHandler;
			if (CustomObjectHandlers.Instance.TryGetValue(obj, out customObjectHandler))
			{
				flag |= customObjectHandler.HandleObject(obj, modifyRequest, mData, this.SyncConfigData, this.targetPartitionSession);
			}
			if (!flag)
			{
				return null;
			}
			return modifyRequest;
		}

		private bool MapDnValuesForAddRequest(ADObjectId sourceObjectId, ADPropertyDefinition prop, DirectoryAttribute da, AddRequest addRequest)
		{
			DirectoryAttribute directoryAttribute = new DirectoryAttribute();
			if (prop == OfflineAddressBookSchema.Server || prop == OfflineAddressBookSchema.AddressLists)
			{
				directoryAttribute.Name = prop.LdapDisplayName;
				ADObjectId adobjectId = new ADObjectId(addRequest.DistinguishedName);
				directoryAttribute.Add(adobjectId.Parent.DistinguishedName);
				addRequest.Attributes.Add(directoryAttribute);
				return true;
			}
			if (this.MapDnValues(sourceObjectId, prop, da, directoryAttribute) && directoryAttribute.Count > 0)
			{
				addRequest.Attributes.Add(directoryAttribute);
			}
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "GenerateAddRequestDirectoryAttributeDelegate added a DN syntax attribute: {0}", prop.LdapDisplayName);
			return true;
		}

		private bool GenerateAddRequestDirectoryAttributeDelegate(TenantRelocationSyncObject sourceObj, ADPropertyDefinition prop, DirectoryAttribute da, object result)
		{
			AddRequest addRequest = (AddRequest)result;
			if (da.Count == 0)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "GenerateAddRequestDirectoryAttributeDelegate skips attribute {0}, with no value", da.Name);
				return false;
			}
			if (CustomPropertyHandlers.Instance.Handlers.ContainsKey(prop.LdapDisplayName))
			{
				return false;
			}
			if (this.IsDnSyntaxAttribute(prop))
			{
				this.MapDnValuesForAddRequest(sourceObj.Id, prop, da, addRequest);
			}
			else
			{
				if (prop == ADUserSchema.UserAccountControl)
				{
					string[] array = (string[])da.GetValues(typeof(string));
					UserAccountControlFlags userAccountControlFlags = (UserAccountControlFlags)int.Parse(array[0]);
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<int, string, UserAccountControlFlags>((long)this.GetHashCode(), "GenerateAddRequestDirectoryAttributeDelegate {0}, {1}, {2}", array.Length, array[0], userAccountControlFlags);
					if ((userAccountControlFlags & UserAccountControlFlags.NormalAccount) != UserAccountControlFlags.None && (userAccountControlFlags & UserAccountControlFlags.PasswordNotRequired) == UserAccountControlFlags.None)
					{
						string text = (string)sourceObj[ADRecipientSchema.DisplayName];
						text = (string.IsNullOrEmpty(text) ? "a" : text);
						string randomPassword = PasswordHelper.GetRandomPassword(text, "a", 128);
						byte[] value;
						using (SecureString secureString = randomPassword.ConvertToSecureString())
						{
							value = ADSession.EncodePasswordForLdap(secureString);
						}
						DirectoryAttribute directoryAttribute = new DirectoryAttribute();
						directoryAttribute.Name = ADUserSchema.UnicodePassword.LdapDisplayName;
						directoryAttribute.Add(value);
						addRequest.Attributes.Add(directoryAttribute);
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "GenerateAddRequestDirectoryAttributeDelegate added a password: {0}, {1}, displayName:{2}", directoryAttribute.Name, randomPassword, text);
					}
				}
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "GenerateAddRequestDirectoryAttributeDelegate added a non-DN syntax attribute: {0}", prop.LdapDisplayName);
				addRequest.Attributes.Add(da);
			}
			return true;
		}

		private bool GenerateAddRequestLinkMetaDataHandler(ADPropertyDefinition prop, LinkMetadata lmd, object result)
		{
			AddRequest addRequest = (AddRequest)result;
			ADObjectId value = new ADObjectId(lmd.TargetDistinguishedName);
			ADObjectId adobjectId = this.MapDistinguishedName(value);
			if (this.IsRegularDnSyntax(prop))
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "GenerateAddRequestLinkMetaDataHandler added a DN syntax link attribute: {0}", prop.LdapDisplayName);
				DirectoryAttribute attribute = new DirectoryAttribute(lmd.AttributeName, adobjectId.DistinguishedName);
				addRequest.Attributes.Add(attribute);
				return true;
			}
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "GenerateAddRequestLinkMetaDataHandler skipped a DN syntax link attribute: {0}, code need to be updated", prop.LdapDisplayName);
			throw new NotImplementedException("Mandatory linked DN-binary/string attributes are not handled");
		}

		public AddRequest GenerateAddRequest(TenantRelocationSyncObject obj, RequestType requestType, out ADObjectId rootId)
		{
			AddRequest addRequest = new AddRequest();
			rootId = this.MapDistinguishedName(obj.Id);
			addRequest.DistinguishedName = rootId.DistinguishedName;
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, RequestType, string>((long)this.GetHashCode(), "GenerateAddRequest entered: dn={0}, requestType={1}, mapped DN={2}", obj.Id.DistinguishedName, requestType, rootId.DistinguishedName);
			DirectoryAttribute directoryAttribute = new DirectoryAttribute();
			directoryAttribute.Name = ADObjectSchema.CorrelationIdRaw.LdapDisplayName;
			ADValueConvertor.ConvertAndAddValueToDirectoryAttribute(obj.Guid, ADObjectSchema.CorrelationIdRaw, directoryAttribute, false);
			addRequest.Attributes.Add(directoryAttribute);
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<Guid>((long)this.GetHashCode(), "GenerateAddRequest: CorrelationIdRaw added: {0}", obj.Guid);
			DirectoryAttribute directoryAttribute2 = new DirectoryAttribute();
			directoryAttribute2.Name = ADObjectSchema.ExchangeObjectIdRaw.LdapDisplayName;
			ADValueConvertor.ConvertAndAddValueToDirectoryAttribute(obj.ExchangeObjectId, ADObjectSchema.ExchangeObjectIdRaw, directoryAttribute2, false);
			addRequest.Attributes.Add(directoryAttribute2);
			this.AppendOrgIdsToAddRequest(addRequest);
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<Guid>((long)this.GetHashCode(), "GenerateAddRequest: ExchangeObjectIdRaw added: {0}", obj.ExchangeObjectId);
			TenantRelocationSyncTranslator.DirectoryAttributeHandler attributehandler = (TenantRelocationSyncObject syncObj, RequestType rt, ADPropertyDefinition prop, DirectoryAttribute da, object result, UpdateData mData1) => this.GenerateAddRequestDirectoryAttributeDelegate(syncObj, prop, da, result);
			TenantRelocationSyncTranslator.LinkMetadataHandler linkHandler = (TenantRelocationSyncObject syncObj, RequestType rt2, ADPropertyDefinition prop, LinkMetadata lmd, object result, UpdateData mData2) => this.GenerateAddRequestLinkMetaDataHandler(prop, lmd, result);
			UpdateData mData = new UpdateData(requestType);
			this.LoopThroughAllAttributes(obj, 0L, addRequest, attributehandler, linkHandler, mData);
			return addRequest;
		}

		internal void AppendOrgIdsToAddRequest(AddRequest addRequest)
		{
			DirectoryAttribute directoryAttribute = new DirectoryAttribute();
			directoryAttribute.Name = ADObjectSchema.ConfigurationUnit.LdapDisplayName;
			ADValueConvertor.ConvertAndAddValueToDirectoryAttribute(this.SyncConfigData.Target.TenantConfigurationUnit, ADObjectSchema.ConfigurationUnit, directoryAttribute, false);
			addRequest.Attributes.Add(directoryAttribute);
			DirectoryAttribute directoryAttribute2 = new DirectoryAttribute();
			directoryAttribute2.Name = ADObjectSchema.OrganizationalUnitRoot.LdapDisplayName;
			ADValueConvertor.ConvertAndAddValueToDirectoryAttribute(this.SyncConfigData.Target.TenantOrganizationUnit, ADObjectSchema.OrganizationalUnitRoot, directoryAttribute2, false);
			addRequest.Attributes.Add(directoryAttribute2);
		}

		internal void AppendOrgIdsToModifyRequest(ModifyRequest modRequest)
		{
			DirectoryAttributeModification directoryAttributeModification = new DirectoryAttributeModification();
			directoryAttributeModification.Name = ADObjectSchema.ConfigurationUnit.LdapDisplayName;
			ADValueConvertor.ConvertAndAddValueToDirectoryAttribute(this.SyncConfigData.Target.TenantConfigurationUnit, ADObjectSchema.ConfigurationUnit, directoryAttributeModification, false);
			directoryAttributeModification.Operation = DirectoryAttributeOperation.Replace;
			modRequest.Modifications.Add(directoryAttributeModification);
			DirectoryAttributeModification directoryAttributeModification2 = new DirectoryAttributeModification();
			directoryAttributeModification2.Name = ADObjectSchema.OrganizationalUnitRoot.LdapDisplayName;
			ADValueConvertor.ConvertAndAddValueToDirectoryAttribute(this.SyncConfigData.Target.TenantOrganizationUnit, ADObjectSchema.OrganizationalUnitRoot, directoryAttributeModification2, false);
			directoryAttributeModification2.Operation = DirectoryAttributeOperation.Replace;
			modRequest.Modifications.Add(directoryAttributeModification2);
		}

		private bool GetDnReferencesDirectoryAttributeDelegate(ADPropertyDefinition prop, DirectoryAttribute da, object result)
		{
			List<ADObjectId> list = (List<ADObjectId>)result;
			if (CustomPropertyHandlers.Instance.Handlers.ContainsKey(prop.LdapDisplayName))
			{
				list.AddRange(CustomPropertyHandlers.Instance.Handlers[prop.LdapDisplayName].EnumerateObjectDependenciesInSource(this, da));
				return true;
			}
			if (this.SyncConfigData.IsSourceSoftLinkEnabled && prop.IsSoftLinkAttribute && prop.SoftLinkShadowProperty.LdapDisplayName.Equals(da.Name))
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "GetDnReferencesDirectoryAttributeDelegate skips soft link DN, as they are not tenant specific: {0}", prop.LdapDisplayName);
				return false;
			}
			if (da.Count == 0)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "GetDnReferencesDirectoryAttributeDelegate skips attribute {0}, with no value", da.Name);
				return false;
			}
			object[] values = da.GetValues(typeof(string));
			object[] array = values;
			int i = 0;
			while (i < array.Length)
			{
				object obj = array[i];
				ADObjectId adobjectId;
				if (prop.Type.Equals(typeof(ADObjectId)))
				{
					adobjectId = ADObjectId.ParseExtendedDN((string)obj);
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "GetDnReferencesDirectoryAttributeDelegate gets a DN: {0}: {1}", prop.LdapDisplayName, adobjectId.DistinguishedName);
					goto IL_1A5;
				}
				if (this.IsDnStringSyntax(prop))
				{
					adobjectId = ADObjectIdWithString.ParseDNStringSyntax((string)obj, null).ObjectIdValue;
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "GetDnReferencesDirectoryAttributeDelegate gets a DN-String: {0}: {1}", prop.LdapDisplayName, adobjectId.DistinguishedName);
					goto IL_1A5;
				}
				if (this.IsDnBinarySyntax(prop))
				{
					DNWithBinary dnwithBinary = DNWithBinary.Parse((string)obj);
					adobjectId = new ADObjectId(dnwithBinary.DistinguishedName);
					ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, string>((long)this.GetHashCode(), "GetDnReferencesDirectoryAttributeDelegate gets a DN-Binary: {0}: {1}", prop.LdapDisplayName, adobjectId.DistinguishedName);
					goto IL_1A5;
				}
				IL_1D0:
				i++;
				continue;
				IL_1A5:
				if (this.SyncConfigData.Source.IsUnderTenantScope(adobjectId) && !adobjectId.IsDeleted && !list.Contains(adobjectId))
				{
					list.Add(adobjectId);
					goto IL_1D0;
				}
				goto IL_1D0;
			}
			return true;
		}

		private bool GetDnReferencesLinkMetaDataHandler(ADPropertyDefinition prop, LinkMetadata lmd, object result)
		{
			List<ADObjectId> list = (List<ADObjectId>)result;
			ADObjectId adobjectId = new ADObjectId(lmd.TargetDistinguishedName);
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, ADObjectId>((long)this.GetHashCode(), "GetDnReferencesLinkMetaDataHandler gets a DN: {0}: {1}", prop.LdapDisplayName, adobjectId);
			if (this.SyncConfigData.Source.IsUnderTenantScope(adobjectId) && !list.Contains(adobjectId))
			{
				list.Add(adobjectId);
			}
			return true;
		}

		internal ADObjectId[] GetAllDnReferences(TenantRelocationSyncObject obj, RequestType requestType, long uSN)
		{
			List<ADObjectId> list = new List<ADObjectId>();
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, RequestType, long>((long)this.GetHashCode(), "GenerateAddRequest entered: object={0}, requestType={1}, uSN={2}", obj.Id.DistinguishedName, requestType, uSN);
			TenantRelocationSyncTranslator.DirectoryAttributeHandler attributehandler = (TenantRelocationSyncObject syncObj, RequestType rt, ADPropertyDefinition prop, DirectoryAttribute da, object result, UpdateData mData1) => this.GetDnReferencesDirectoryAttributeDelegate(prop, da, result);
			TenantRelocationSyncTranslator.LinkMetadataHandler linkHandler = (TenantRelocationSyncObject syncObj, RequestType rt2, ADPropertyDefinition prop, LinkMetadata lmd, object result, UpdateData mData2) => this.GetDnReferencesLinkMetaDataHandler(prop, lmd, result);
			UpdateData mData = new UpdateData(requestType);
			this.LoopThroughAllAttributes(obj, 0L, list, attributehandler, linkHandler, mData);
			ExTraceGlobals.TenantRelocationTracer.TraceDebug((long)this.GetHashCode(), "GetAllDnReferences finished: object={0}, requestType={1}, uSN={2}, number of DNs={3}", new object[]
			{
				obj.Id.DistinguishedName,
				requestType,
				uSN,
				list.Count
			});
			return list.ToArray();
		}

		private static Type DataSyntaxToType(DataSyntax syntax, out object defaultValue)
		{
			switch (syntax)
			{
			case DataSyntax.Boolean:
				defaultValue = false;
				return typeof(bool);
			case DataSyntax.Integer:
			case DataSyntax.Enumeration:
				defaultValue = 0;
				return typeof(int);
			case DataSyntax.Octet:
				defaultValue = null;
				return typeof(byte[]);
			case DataSyntax.Numeric:
			case DataSyntax.Printable:
			case DataSyntax.Teletex:
			case DataSyntax.IA5:
			case DataSyntax.CaseSensitive:
			case DataSyntax.Unicode:
				defaultValue = string.Empty;
				return typeof(string);
			case DataSyntax.UTCTime:
			case DataSyntax.GeneralizedTime:
				defaultValue = default(DateTime);
				return typeof(DateTime);
			case DataSyntax.LargeInteger:
				defaultValue = 0L;
				return typeof(long);
			case DataSyntax.DNBinary:
				defaultValue = null;
				return typeof(DNWithBinary);
			case DataSyntax.DNString:
				defaultValue = null;
				return typeof(ADObjectIdWithString);
			case DataSyntax.DSDN:
			case DataSyntax.ORName:
				defaultValue = null;
				return typeof(ADObjectId);
			}
			defaultValue = null;
			return typeof(byte[]);
		}

		private object GetDummyValue(ADPropertyDefinition prop)
		{
			ADSchemaAttributeObject adschemaAttributeObjectByLdapDisplayName = ADSchemaDataProvider.Instance.GetADSchemaAttributeObjectByLdapDisplayName(prop.LdapDisplayName);
			int num = adschemaAttributeObjectByLdapDisplayName.RangeLower ?? 0;
			switch (adschemaAttributeObjectByLdapDisplayName.DataSyntax)
			{
			case DataSyntax.Boolean:
				return "TRUE";
			case DataSyntax.Integer:
			case DataSyntax.Enumeration:
			case DataSyntax.LargeInteger:
				return num.ToString();
			case DataSyntax.Sid:
			{
				SecurityIdentifier securityIdentifier = new SecurityIdentifier("BA");
				byte[] array = new byte[securityIdentifier.BinaryLength];
				securityIdentifier.GetBinaryForm(array, 0);
				return array;
			}
			case DataSyntax.Octet:
			{
				int num2 = (num > 0) ? num : 1;
				byte[] array2 = new byte[num2];
				for (int i = 0; i < num2; i++)
				{
					array2[i] = 1;
				}
				return array2;
			}
			case DataSyntax.Numeric:
			case DataSyntax.Printable:
			case DataSyntax.Teletex:
			case DataSyntax.IA5:
			case DataSyntax.CaseSensitive:
			case DataSyntax.Unicode:
				return new string('1', (num > 0) ? num : 1);
			case DataSyntax.UTCTime:
			case DataSyntax.GeneralizedTime:
				return "20120625235917.0Z";
			case DataSyntax.NTSecDesc:
			{
				RawSecurityDescriptor rawSecurityDescriptor = new RawSecurityDescriptor("O:PSG:PSD:(A;CI;CCRC;;;PS)");
				byte[] array3 = new byte[rawSecurityDescriptor.BinaryLength];
				rawSecurityDescriptor.GetBinaryForm(array3, 0);
				return array3;
			}
			case DataSyntax.DNBinary:
				return "B:32:006D6B1E74714B4E8DE10DF23ACB1C42:" + this.SyncConfigData.Target.TenantConfigurationUnit.DistinguishedName;
			case DataSyntax.DNString:
				return "S:5:dummy:" + this.SyncConfigData.Target.TenantConfigurationUnit.DistinguishedName;
			case DataSyntax.DSDN:
			case DataSyntax.ORName:
				return this.SyncConfigData.Target.TenantConfigurationUnit.DistinguishedName;
			}
			return null;
		}

		private ITopologyConfigurationSession sourceSystemConfigurationSession;

		private ITopologyConfigurationSession targetPartitionSession;

		private static IList<string> attributesToSkip;

		private static IList<string> undefinedAttributesToCopy;

		private static IList<string> mandatoryDnSyntaxAttributes;

		internal delegate bool DirectoryAttributeHandler(TenantRelocationSyncObject obj, RequestType requestType, ADPropertyDefinition prop, DirectoryAttribute da, object result, UpdateData mData);

		internal delegate bool LinkMetadataHandler(TenantRelocationSyncObject obj, RequestType requestType, ADPropertyDefinition prop, LinkMetadata lmd, object result, UpdateData mData);
	}
}
