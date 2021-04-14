using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class ADGenericPagedReader<TResult> : ADGenericReader, IEnumerable<TResult>, IEnumerable, IPageInformation where TResult : IConfigurable, new()
	{
		protected override ADRawEntry ScopeDeterminingObject
		{
			get
			{
				return this.dummyInstance;
			}
		}

		protected bool? RetrievedAllData
		{
			get
			{
				return this.retrievedAllData;
			}
			set
			{
				this.retrievedAllData = value;
			}
		}

		public int PagesReturned
		{
			get
			{
				return this.pagesReturned;
			}
		}

		public int PageSize
		{
			get
			{
				return this.pageSize;
			}
			set
			{
				this.pageSize = value;
			}
		}

		public int LastRetrievedCount
		{
			get
			{
				return this.lastRetrievedCount;
			}
		}

		public bool? MorePagesAvailable
		{
			get
			{
				bool? flag = this.retrievedAllData;
				if (flag == null)
				{
					return null;
				}
				return new bool?(!flag.GetValueOrDefault());
			}
		}

		public bool SkipNonUniqueResults
		{
			get
			{
				return this.skipNonUniqueResults;
			}
		}

		protected internal ADGenericPagedReader()
		{
		}

		protected internal ADGenericPagedReader(IDirectorySession session, ADObjectId rootId, QueryScope scope, QueryFilter filter, SortBy sortBy, int pageSize, IEnumerable<PropertyDefinition> properties, bool skipCheckVirtualIndex) : base(session, rootId, scope, sortBy)
		{
			if (!typeof(ADRawEntry).IsAssignableFrom(typeof(TResult)))
			{
				throw new InvalidOperationException(DirectoryStrings.ErrorMustBeADRawEntry);
			}
			if (pageSize < 0 || pageSize > 10000)
			{
				throw new ArgumentOutOfRangeException("pageSize", pageSize, string.Format("pageSize should be between 1 and {0} or 0 to use the default page size: {1}", 10000, ADGenericPagedReader<TResult>.DefaultPageSize));
			}
			this.dummyInstance = (ADRawEntry)((object)((default(TResult) == null) ? Activator.CreateInstance<TResult>() : default(TResult)));
			QueryFilter filter2 = filter;
			ConfigScopes configScopes;
			ADScope readScope = session.GetReadScope(rootId, this.dummyInstance, false, out configScopes);
			ADObject adobject;
			string[] ldapAttributes;
			base.Session.GetSchemaAndApplyFilter(this.dummyInstance, readScope, out adobject, out ldapAttributes, ref filter, ref properties);
			ADDataSession addataSession = base.Session as ADDataSession;
			if (addataSession != null)
			{
				addataSession.UpdateFilterforInactiveMailboxSearch(this.dummyInstance, ref filter);
			}
			base.LdapAttributes = ldapAttributes;
			this.pageSize = ((pageSize == 0) ? ADGenericPagedReader<TResult>.DefaultPageSize : pageSize);
			this.retrievedAllData = null;
			this.properties = properties;
			session.CheckFilterForUnsafeIdentity(filter2);
			base.LdapFilter = LdapFilterBuilder.LdapFilterFromQueryFilter(filter, skipCheckVirtualIndex, base.Session.SessionSettings.PartitionSoftLinkMode, base.Session.SessionSettings.IsTenantScoped);
			this.skipNonUniqueResults = (session is IConfigurationSession);
		}

		public TResult[] ReadAllPages()
		{
			if (this.retrievedAllData != null && this.retrievedAllData.Value)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionPagedReaderIsSingleUse);
			}
			if (this.pagesReturned > 0)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionPagedReaderReadAllAfterEnumerating);
			}
			List<TResult> list = new List<TResult>();
			foreach (TResult item in this)
			{
				list.Add(item);
			}
			return list.ToArray();
		}

		public IEnumerator<TResult> GetEnumerator()
		{
			if (base.IsEmptyReader)
			{
				this.RetrievedAllData = new bool?(true);
			}
			else
			{
				if (this.retrievedAllData != null && this.retrievedAllData.Value)
				{
					throw new InvalidOperationException(DirectoryStrings.ExceptionPagedReaderIsSingleUse);
				}
				while (this.retrievedAllData == null || !this.retrievedAllData.Value)
				{
					TResult[] results = this.GetNextPage();
					this.lastRetrievedCount = results.Length;
					foreach (TResult result in results)
					{
						yield return result;
					}
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		protected abstract SearchResultEntryCollection GetNextResultCollection();

		protected virtual TResult[] GetNextPage()
		{
			SearchResultEntryCollection nextResultCollection = this.GetNextResultCollection();
			if (nextResultCollection == null)
			{
				return (TResult[])new TResult[0];
			}
			TResult[] array = base.Session.ObjectsFromEntries<TResult>(nextResultCollection, base.PreferredServerName, this.properties, this.dummyInstance);
			TResult[] result = this.skipNonUniqueResults ? this.GetUniqueResults(array) : array;
			this.pagesReturned++;
			return result;
		}

		private TResult[] GetUniqueResults(TResult[] results)
		{
			TResult[] result;
			if (this.pagesReturned == 0)
			{
				if (this.retrievedAllData == null || !this.retrievedAllData.Value)
				{
					this.idHashSet = new HashSet<Guid>(this.pageSize);
					foreach (TResult tresult in results)
					{
						ADRawEntry adrawEntry = (ADRawEntry)((object)tresult);
						this.idHashSet.Add(adrawEntry.Id.ObjectGuid);
					}
				}
				result = results;
			}
			else
			{
				List<TResult> list = new List<TResult>(results.Length);
				foreach (TResult tresult2 in results)
				{
					ADRawEntry adrawEntry2 = (ADRawEntry)((object)tresult2);
					if (this.idHashSet.TryAdd(adrawEntry2.Id.ObjectGuid))
					{
						list.Add(tresult2);
					}
					else
					{
						ExTraceGlobals.ADReadTracer.TraceError<int, string>((long)this.GetHashCode(), "Warning: Removing non-unique object from result set on {0}th page: {1}", this.pagesReturned + 1, adrawEntry2.Id.DistinguishedName);
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		public const int MaximumPageSize = 10000;

		public static readonly int DefaultPageSize = 1000;

		private IEnumerable<PropertyDefinition> properties;

		private int pageSize;

		private bool? retrievedAllData;

		private ADRawEntry dummyInstance;

		private int lastRetrievedCount;

		private int pagesReturned;

		private HashSet<Guid> idHashSet;

		private bool skipNonUniqueResults;
	}
}
