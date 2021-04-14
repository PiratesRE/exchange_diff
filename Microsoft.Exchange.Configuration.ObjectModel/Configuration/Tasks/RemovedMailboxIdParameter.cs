using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class RemovedMailboxIdParameter : ADIdParameter
	{
		public RemovedMailboxIdParameter()
		{
		}

		public RemovedMailboxIdParameter(string identity) : base(identity)
		{
		}

		public RemovedMailboxIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public RemovedMailboxIdParameter(RemovedMailbox removedMailbox) : base(removedMailbox.Id)
		{
		}

		public RemovedMailboxIdParameter(PSObject removedMailbox) : base(RemovedMailboxIdParameter.GetRemovedMailboxName(removedMailbox))
		{
		}

		public RemovedMailboxIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public static RemovedMailboxIdParameter Parse(string identity)
		{
			return new RemovedMailboxIdParameter(identity);
		}

		private ITopologyConfigurationSession ConfigSession
		{
			get
			{
				if (this.configSession == null)
				{
					this.configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 112, "ConfigSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\IdentityParameter\\RecipientParameters\\RemovedMailboxIdParameter.cs");
				}
				return this.configSession;
			}
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			TaskLogger.LogEnter();
			if (!typeof(RemovedMailbox).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (base.InternalADObjectId == null && string.IsNullOrEmpty(base.RawIdentity))
			{
				throw new InvalidOperationException(Strings.ErrorOperationOnInvalidObject);
			}
			notFoundReason = null;
			string orgName = null;
			string text = base.RawIdentity;
			ADObjectId adobjectId = base.InternalADObjectId ?? RemovedMailboxIdParameter.GetObjectIdFromCanonicalName(base.RawIdentity);
			if (adobjectId == null && !string.IsNullOrEmpty(base.RawIdentity))
			{
				int num = base.RawIdentity.IndexOf('\\');
				if (num > 0 && base.RawIdentity.Length > num + 1)
				{
					orgName = base.RawIdentity.Substring(0, num);
					text = base.RawIdentity.Substring(num + 1);
				}
			}
			QueryFilter queryFilter;
			bool flag = this.TryGetOrganizationFilter(rootId, adobjectId, orgName, out queryFilter);
			ComparisonFilter databaseRetentionPeriodFilter = this.GetDatabaseRetentionPeriodFilter();
			QueryFilter basicFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				queryFilter,
				new ExistsFilter(RemovedMailboxSchema.PreviousDatabase),
				databaseRetentionPeriodFilter
			});
			EnumerableWrapper<T> enumerableWrapper = EnumerableWrapper<T>.Empty;
			if (adobjectId != null)
			{
				enumerableWrapper = this.SearchByObjectId<T>(adobjectId, session, optionalData, basicFilter);
			}
			if (flag && (enumerableWrapper == null || !enumerableWrapper.HasElements()) && !string.IsNullOrEmpty(text))
			{
				enumerableWrapper = this.SearchByObjectAttributes<T>(session, optionalData, text, basicFilter);
			}
			enumerableWrapper = EnumerableWrapper<T>.GetWrapper(enumerableWrapper);
			TaskLogger.LogExit();
			return enumerableWrapper;
		}

		internal override IEnumerable<T> PerformSearch<T>(QueryFilter filter, ADObjectId rootId, IDirectorySession session, bool deepSearch)
		{
			ADObjectId deletedObjectsContainer = ADSession.GetDeletedObjectsContainer(session.GetDomainNamingContext());
			ADPagedReader<RemovedMailbox> adpagedReader = session.FindPagedDeletedObject<RemovedMailbox>(deletedObjectsContainer, QueryScope.OneLevel, filter, null, 0);
			return (IEnumerable<T>)adpagedReader;
		}

		private static string GetRemovedMailboxName(PSObject removedMailbox)
		{
			if (removedMailbox != null && removedMailbox.Properties != null)
			{
				PSPropertyInfo pspropertyInfo = removedMailbox.Properties["Name"];
				if (pspropertyInfo != null)
				{
					return pspropertyInfo.Value as string;
				}
			}
			return "";
		}

		private static ADObjectId GetObjectIdFromCanonicalName(string canonicalName)
		{
			if (!string.IsNullOrEmpty(canonicalName))
			{
				string[] array = canonicalName.Split(new char[]
				{
					'/'
				});
				ADObjectId childId;
				if (array.Length == 3 && array[1].Equals("Deleted Objects", StringComparison.OrdinalIgnoreCase) && ADIdParameter.TryResolveCanonicalName(array[0], out childId))
				{
					childId = childId.GetChildId(array[1]);
					if (childId != null)
					{
						return childId.GetChildId(array[2]);
					}
				}
			}
			return null;
		}

		private bool TryGetOrganizationFilter(ADObjectId rootId, ADObjectId objectIdentity, string orgName, out QueryFilter orgFilter)
		{
			orgFilter = null;
			if (rootId != null)
			{
				orgFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, rootId);
			}
			else
			{
				if (objectIdentity != null)
				{
					return false;
				}
				if (!string.IsNullOrEmpty(orgName))
				{
					ExchangeConfigurationUnit configurationUnit = base.GetConfigurationUnit(orgName);
					if (configurationUnit == null)
					{
						return false;
					}
					orgFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, configurationUnit.OrganizationalUnitRoot);
				}
				else
				{
					orgFilter = new NotFilter(new ExistsFilter(ADObjectSchema.OrganizationalUnitRoot));
				}
			}
			return true;
		}

		private EnumerableWrapper<T> SearchByObjectId<T>(ADObjectId objectId, IDirectorySession session, OptionalIdentityData optionalData, QueryFilter basicFilter) where T : IConfigurable, new()
		{
			EnumerableWrapper<T> result = EnumerableWrapper<T>.Empty;
			try
			{
				IEnumerable<T> enumerable = base.PerformPrimarySearch<T>(QueryFilter.AndTogether(new QueryFilter[]
				{
					basicFilter,
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, objectId)
				}), null, session, false, optionalData);
				result = EnumerableWrapper<T>.GetWrapper(enumerable);
			}
			catch (ADReferralException)
			{
			}
			return result;
		}

		private EnumerableWrapper<T> SearchByObjectAttributes<T>(IDirectorySession session, OptionalIdentityData optionalData, string queryFilter, QueryFilter basicFilter) where T : IConfigurable, new()
		{
			List<QueryFilter> list = new List<QueryFilter>();
			QueryFilter queryFilter2 = base.CreateWildcardOrEqualFilter(RemovedMailboxSchema.Name, queryFilter);
			if (queryFilter2 != null)
			{
				list.Add(queryFilter2);
			}
			SmtpAddress smtpAddress = new SmtpAddress(queryFilter);
			if (smtpAddress.IsValidAddress)
			{
				QueryFilter item = new ComparisonFilter(ComparisonOperator.Equal, RemovedMailboxSchema.EmailAddresses, "SMTP:" + smtpAddress);
				list.Add(item);
			}
			QueryFilter queryFilter3 = base.CreateWildcardOrEqualFilter(RemovedMailboxSchema.WindowsLiveID, queryFilter);
			if (queryFilter3 != null)
			{
				list.Add(queryFilter3);
			}
			QueryFilter queryFilter4 = base.CreateWildcardOrEqualFilter(RemovedMailboxSchema.SamAccountName, queryFilter);
			if (queryFilter4 != null)
			{
				list.Add(queryFilter4);
			}
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				basicFilter,
				QueryFilter.OrTogether(list.ToArray())
			});
			IEnumerable<T> enumerable = base.PerformPrimarySearch<T>(filter, null, session, false, optionalData);
			return EnumerableWrapper<T>.GetWrapper(enumerable);
		}

		private ComparisonFilter GetDatabaseRetentionPeriodFilter()
		{
			ADObjectId databasesContainerId = this.ConfigSession.GetDatabasesContainerId();
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, MailboxDatabaseSchema.IsExcludedFromProvisioning, false),
				new ComparisonFilter(ComparisonOperator.Equal, MailboxDatabaseSchema.IsSuspendedFromProvisioning, false),
				new ComparisonFilter(ComparisonOperator.Equal, DatabaseSchema.Recovery, false)
			});
			MailboxDatabase[] array = this.ConfigSession.Find<MailboxDatabase>(databasesContainerId, QueryScope.OneLevel, filter, null, 1);
			EnhancedTimeSpan enhancedTimeSpan = (array.Length > 0) ? array[0].MailboxRetention : EnhancedTimeSpan.Zero;
			DateTime dateTime = DateTime.UtcNow.Subtract(enhancedTimeSpan);
			return new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.WhenChangedUTC, dateTime);
		}

		[NonSerialized]
		private ITopologyConfigurationSession configSession;
	}
}
