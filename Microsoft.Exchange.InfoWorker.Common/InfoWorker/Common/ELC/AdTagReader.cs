using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.InfoWorker.Common.Search;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class AdTagReader : AdReader
	{
		internal static Dictionary<Guid, AdTagData> GetAllTags()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 49, "GetAllTags", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\ELC\\AdTagReader.cs");
			ExchangeConfigurationContainer exchangeConfigurationContainer = tenantOrTopologyConfigurationSession.GetExchangeConfigurationContainer();
			ADPagedReader<RetentionPolicyTag> allAdTags = tenantOrTopologyConfigurationSession.FindPaged<RetentionPolicyTag>(exchangeConfigurationContainer.Id, QueryScope.SubTree, null, null, 0);
			AdReader.Tracer.TraceDebug(0L, "Found Policy Tags in the AD.");
			List<RetentionPolicyTag> theGoodTags = AdTagReader.GetTheGoodTags(allAdTags);
			Dictionary<Guid, AdTagData> dictionary = new Dictionary<Guid, AdTagData>();
			foreach (RetentionPolicyTag retentionPolicyTag in theGoodTags)
			{
				AdTagData value = AdTagReader.FetchTagContentSettings(retentionPolicyTag);
				dictionary[retentionPolicyTag.RetentionId] = value;
			}
			return dictionary;
		}

		internal static List<RetentionPolicyTag> GetAllRetentionTags(IConfigurationSession session, OrganizationId organizationId)
		{
			ADPagedReader<RetentionPolicyTag> source = session.FindPaged<RetentionPolicyTag>(null, QueryScope.SubTree, null, null, 0);
			List<RetentionPolicyTag> list = source.ToList<RetentionPolicyTag>();
			string arg = (organizationId.ConfigurationUnit == null) ? "First Organization" : organizationId.ConfigurationUnit.ToString();
			AdReader.Tracer.TraceDebug<int, string>(0L, "Found {0} retention tags for {1} in AD.", list.Count, arg);
			return list;
		}

		internal static Dictionary<Guid, AdTagData> GetTagsInPolicy(MailboxSession session, ADUser aduser, Dictionary<Guid, AdTagData> allAdTags)
		{
			if (allAdTags == null || allAdTags.Count == 0 || aduser == null)
			{
				return null;
			}
			string arg = session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			ADObjectId adobjectId = aduser.RetentionPolicy;
			if (adobjectId == null && aduser.ShouldUseDefaultRetentionPolicy && aduser.OrganizationId != null && !OrganizationId.ForestWideOrgId.Equals(aduser.OrganizationId))
			{
				IConfigurationSession scopedSession;
				if (SharedConfiguration.IsDehydratedConfiguration(aduser.OrganizationId))
				{
					scopedSession = SharedConfiguration.CreateScopedToSharedConfigADSession(aduser.OrganizationId);
				}
				else
				{
					scopedSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.FullyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(aduser.OrganizationId), 149, "GetTagsInPolicy", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\ELC\\AdTagReader.cs");
				}
				IList<RetentionPolicy> defaultRetentionPolicy = SharedConfiguration.GetDefaultRetentionPolicy(scopedSession, aduser, null, 1);
				if (defaultRetentionPolicy != null && defaultRetentionPolicy.Count > 0)
				{
					adobjectId = defaultRetentionPolicy[0].Id;
				}
			}
			if (adobjectId == null)
			{
				AdReader.Tracer.TraceDebug<string>(0L, "Mailbox '{0}' does not have an ELC Mailbox policy.", arg);
				return null;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.FullyConsistent, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(adobjectId), 175, "GetTagsInPolicy", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\ELC\\AdTagReader.cs");
			RetentionPolicy retentionPolicy = tenantOrTopologyConfigurationSession.Read<RetentionPolicy>(adobjectId);
			if (retentionPolicy == null)
			{
				AdReader.Tracer.TraceDebug<string, ADObjectId>(0L, "Mailbox '{0}' no matching ELC Mailbox policy for Template '{1}'.", arg, adobjectId);
				return null;
			}
			MultiValuedProperty<ADObjectId> retentionPolicyTagLinks = retentionPolicy.RetentionPolicyTagLinks;
			Dictionary<Guid, AdTagData> dictionary = new Dictionary<Guid, AdTagData>();
			List<AdTagData> list = new List<AdTagData>();
			List<AdTagData> list2 = new List<AdTagData>();
			int num = 0;
			Dictionary<Guid, AdTagData> dictionary2 = new Dictionary<Guid, AdTagData>();
			foreach (Guid key in allAdTags.Keys)
			{
				AdTagData adTagData = allAdTags[key];
				dictionary2[adTagData.Tag.Guid] = adTagData;
			}
			foreach (ADObjectId adobjectId2 in retentionPolicyTagLinks)
			{
				if (!dictionary2.ContainsKey(adobjectId2.ObjectGuid))
				{
					AdReader.Tracer.TraceDebug<string, string>(0L, "Mailbox '{0}' has link '{1}' in policy to a tag that could not be found in the AD. Skipping user.", arg, adobjectId2.DistinguishedName);
					throw new IWTransientException(Strings.descTagNotInAD(adobjectId2.DistinguishedName));
				}
				AdTagData adTagData2 = dictionary2[adobjectId2.ObjectGuid];
				if (adTagData2.Tag.Type == ElcFolderType.All)
				{
					if (ElcMailboxHelper.IsArchiveTag(adTagData2, false))
					{
						AdReader.Tracer.TraceDebug<string, string>(0L, "Mailbox '{0}'. Tag {1} is a default archive tag.", arg, adTagData2.Tag.Name);
						list2.Add(adTagData2);
					}
					else if (adTagData2.Tag.IsPrimary)
					{
						AdReader.Tracer.TraceDebug<string, string>(0L, "Mailbox '{0}'. Tag {1} is the primary default retention tag.", arg, adTagData2.Tag.Name);
						list.Insert(0, adTagData2);
						num++;
					}
					else
					{
						AdReader.Tracer.TraceDebug<string, string>(0L, "Mailbox '{0}'. Tag {1} is a default retention tag.", arg, adTagData2.Tag.Name);
						list.Add(adTagData2);
					}
				}
				else
				{
					dictionary[adobjectId2.ObjectGuid] = dictionary2[adobjectId2.ObjectGuid];
				}
			}
			if (list2.Count > 0)
			{
				AdReader.Tracer.TraceDebug<string, string, int>(0L, "Mailbox '{0}' with policy '{1}' has {2} default MTA tags.", arg, retentionPolicy.Id.DistinguishedName, list2.Count);
				list2.Sort(delegate(AdTagData x, AdTagData y)
				{
					if (x.ContentSettings.Count != 1)
					{
						return 1;
					}
					if (y.ContentSettings.Count != 1)
					{
						return -1;
					}
					return x.ContentSettings.Single<KeyValuePair<Guid, ContentSetting>>().Value.MessageClass.Length - y.ContentSettings.Single<KeyValuePair<Guid, ContentSetting>>().Value.MessageClass.Length;
				});
				AdTagData firstArchiveTag = new AdTagData();
				firstArchiveTag.Tag = list2[0].Tag;
				firstArchiveTag.ContentSettings = new SortedDictionary<Guid, ContentSetting>();
				list2.SelectMany((AdTagData x) => x.ContentSettings.Values).ForEach(delegate(ContentSetting x)
				{
					firstArchiveTag.ContentSettings[x.Guid] = x;
				});
				dictionary[firstArchiveTag.Tag.Guid] = firstArchiveTag;
			}
			if (list.Count > 0)
			{
				if (num > 1)
				{
					AdReader.Tracer.TraceDebug<string, string>(0L, "Mailbox '{0}' has policy '{1}' with corrupted default tags (Primary default should be 1). Skipping user.", arg, retentionPolicy.Id.DistinguishedName);
					throw new IWPermanentException(Strings.descPrimaryDefaultCorrupted(retentionPolicy.Id.DistinguishedName, num));
				}
				if (num == 0 && list.Count > 1)
				{
					list.Sort(delegate(AdTagData x, AdTagData y)
					{
						if (x.ContentSettings.Count != 1)
						{
							return 1;
						}
						if (y.ContentSettings.Count != 1)
						{
							return -1;
						}
						return x.ContentSettings.Single<KeyValuePair<Guid, ContentSetting>>().Value.MessageClass.Length - y.ContentSettings.Single<KeyValuePair<Guid, ContentSetting>>().Value.MessageClass.Length;
					});
				}
				for (int i = 0; i < list.Count; i++)
				{
					Guid guid = list[i].Tag.Guid;
					dictionary[guid] = list[i];
				}
			}
			return dictionary;
		}

		internal static void LoadTagsInOrg(OrganizationId orgId, Dictionary<Guid, AdTagData> allTags)
		{
			IConfigurationSession configurationSession;
			if (SharedConfiguration.IsDehydratedConfiguration(orgId))
			{
				configurationSession = SharedConfiguration.CreateScopedToSharedConfigADSession(orgId);
			}
			else
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(orgId);
				configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, sessionSettings, 375, "LoadTagsInOrg", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\ELC\\AdTagReader.cs");
			}
			ADPagedReader<RetentionPolicyTag> allAdTags = configurationSession.FindPaged<RetentionPolicyTag>(null, QueryScope.SubTree, null, null, 0);
			AdReader.Tracer.TraceDebug(0L, "Found Policy Tags in the AD.");
			List<RetentionPolicyTag> theGoodTags = AdTagReader.GetTheGoodTags(allAdTags);
			foreach (RetentionPolicyTag retentionPolicyTag in theGoodTags)
			{
				AdTagData value = AdTagReader.FetchTagContentSettings(retentionPolicyTag);
				allTags[retentionPolicyTag.RetentionId] = value;
			}
		}

		internal static AdTagData FetchTagContentSettings(RetentionPolicyTag retentionPolicyTag)
		{
			AdTagData adTagData = new AdTagData();
			adTagData.Tag = new RetentionTag(retentionPolicyTag);
			List<ElcContentSettings> list = new List<ElcContentSettings>(retentionPolicyTag.GetELCContentSettings().ReadAllPages());
			foreach (ElcContentSettings elcContentSettings in list)
			{
				ValidationError[] array = elcContentSettings.ValidateRead();
				if (array != null && array.Length > 0)
				{
					ValidationError[] array2 = array;
					int num = 0;
					if (num < array2.Length)
					{
						ValidationError validationError = array2[num];
						AdReader.Tracer.TraceError<string, LocalizedString>(0L, "The ElcContentSettings '{0}' has a validation error: {1}", elcContentSettings.Name, validationError.Description);
						throw new DataValidationException(validationError);
					}
				}
			}
			adTagData.ContentSettings = new SortedDictionary<Guid, ContentSetting>();
			foreach (ElcContentSettings elcContentSettings2 in list)
			{
				adTagData.ContentSettings[elcContentSettings2.Guid] = new ContentSetting(elcContentSettings2);
			}
			return adTagData;
		}

		private static List<RetentionPolicyTag> GetTheGoodTags(ADPagedReader<RetentionPolicyTag> allAdTags)
		{
			List<RetentionPolicyTag> list = new List<RetentionPolicyTag>();
			foreach (RetentionPolicyTag retentionPolicyTag in allAdTags)
			{
				ValidationError[] array = retentionPolicyTag.ValidateRead();
				if (array != null && array.Length > 0)
				{
					foreach (ValidationError validationError in array)
					{
						AdReader.Tracer.TraceError<string>(0L, "The retentionPolicyTag '{0}' has a validation error.", retentionPolicyTag.Name);
						if (!(validationError is PropertyValidationError))
						{
							AdReader.Tracer.TraceError<Type, LocalizedString>(0L, "The error type is '{0}'.  Validation error: {1}", validationError.GetType(), validationError.Description);
							throw new DataValidationException(validationError);
						}
						PropertyDefinition propertyDefinition = ((PropertyValidationError)validationError).PropertyDefinition;
						if (propertyDefinition != RetentionPolicyTagSchema.Type)
						{
							AdReader.Tracer.TraceError<Type, string, LocalizedString>(0L, "The error type is '{0}'.  Property '{1}' has a property validation error: {2}", validationError.GetType(), propertyDefinition.Name, validationError.Description);
							throw new DataValidationException(validationError);
						}
						AdReader.Tracer.TraceError<string>(0L, "This tag is of unknown type. Skipping tag '{0}'", retentionPolicyTag.Name);
					}
				}
				else
				{
					list.Add(retentionPolicyTag);
				}
			}
			return list;
		}
	}
}
