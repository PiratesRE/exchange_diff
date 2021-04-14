using System;
using System.Collections;
using System.DirectoryServices.Protocols;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class ADGenericReader
	{
		protected bool UseNullRoot
		{
			get
			{
				return this.useNullRoot;
			}
			set
			{
				this.useNullRoot = value;
			}
		}

		protected bool IsEmptyReader
		{
			get
			{
				return this.isEmptyReader;
			}
		}

		protected virtual ADRawEntry ScopeDeterminingObject
		{
			get
			{
				return null;
			}
		}

		protected internal CustomExceptionHandler CustomExceptionHandler
		{
			set
			{
				this.customExceptionHandler = value;
			}
		}

		internal string PreferredServerName
		{
			get
			{
				return this.preferredServerName;
			}
		}

		public byte[] Cookie
		{
			get
			{
				return this.cookie;
			}
			set
			{
				this.cookie = value;
			}
		}

		public int Lcid
		{
			get
			{
				return this.lcid;
			}
		}

		public bool IncludeDeletedObjects
		{
			get
			{
				return this.includeDeletedObjects;
			}
			set
			{
				this.includeDeletedObjects = value;
				if (value)
				{
					if (!this.directoryControls.Contains(ADGenericReader.showDeletedControl))
					{
						this.directoryControls.Add(ADGenericReader.showDeletedControl);
						return;
					}
				}
				else if (ADGenericReader.showDeletedControl != null && this.directoryControls.Contains(ADGenericReader.showDeletedControl))
				{
					this.directoryControls.Remove(ADGenericReader.showDeletedControl);
				}
			}
		}

		public bool SearchAllNcs
		{
			get
			{
				return this.searchAllNcs;
			}
			set
			{
				this.searchAllNcs = value;
				if (value)
				{
					if (!this.directoryControls.Contains(ADGenericReader.searchOptionsControl))
					{
						this.directoryControls.Add(ADGenericReader.searchOptionsControl);
						return;
					}
				}
				else if (ADGenericReader.searchOptionsControl != null && this.directoryControls.Contains(ADGenericReader.searchOptionsControl))
				{
					this.directoryControls.Remove(ADGenericReader.searchOptionsControl);
				}
			}
		}

		protected IDirectorySession Session
		{
			get
			{
				return this.session;
			}
		}

		protected string[] LdapAttributes
		{
			get
			{
				return this.ldapAttributes;
			}
			set
			{
				this.ldapAttributes = value;
			}
		}

		internal string LdapFilter
		{
			get
			{
				return this.ldapFilter;
			}
			set
			{
				this.ldapFilter = value;
			}
		}

		protected ADObjectId RootId
		{
			get
			{
				return this.rootId;
			}
		}

		protected DirectoryControlCollection DirectoryControls
		{
			get
			{
				return this.directoryControls;
			}
		}

		protected virtual int SizeLimit
		{
			get
			{
				return 0;
			}
		}

		protected internal ADGenericReader()
		{
			this.isEmptyReader = true;
		}

		protected ADGenericReader(IDirectorySession session, ADObjectId rootId, QueryScope scope, SortBy sortBy)
		{
			this.session = session;
			this.rootId = rootId;
			this.scope = scope;
			this.lcid = session.Lcid;
			this.sortBy = sortBy;
			this.directoryControls = new DirectoryControlCollection();
			this.directoryControls.Add(new ExtendedDNControl(ExtendedDNFlag.StandardString));
			if (this.sortBy != null)
			{
				this.AddSortControl();
			}
		}

		protected SearchResultEntryCollection GetNextResultCollection(Type controlType, out DirectoryControl responseControl)
		{
			SearchRequest searchRequest = new SearchRequest(null, this.ldapFilter, (SearchScope)this.scope, this.ldapAttributes);
			searchRequest.Controls.AddRange(this.directoryControls);
			searchRequest.SizeLimit = this.SizeLimit;
			if (this.session.ServerTimeout != null)
			{
				searchRequest.TimeLimit = this.session.ServerTimeout.Value;
			}
			SearchResponse searchResponse = null;
			responseControl = null;
			RetryManager retryManager = new RetryManager();
			ADObjectId adobjectId = this.rootId;
			bool flag = !this.session.SessionSettings.IncludeSoftDeletedObjects && !this.session.SessionSettings.IncludeInactiveMailbox && this.session.EnforceContainerizedScoping;
			for (;;)
			{
				PooledLdapConnection readConnection = this.session.GetReadConnection(this.preferredServerName, null, ref adobjectId, this.ScopeDeterminingObject);
				Guid serviceProviderRequestId = Guid.Empty;
				try
				{
					try
					{
						if (this.useNullRoot)
						{
							searchRequest.DistinguishedName = null;
						}
						else
						{
							searchRequest.DistinguishedName = adobjectId.ToDNString();
							if (flag && searchRequest.Scope == SearchScope.Subtree)
							{
								ADObjectId domainId = adobjectId.DomainId;
								if (domainId != null)
								{
									ADObjectId childId = domainId.GetChildId("OU", "Microsoft Exchange Hosted Organizations");
									ADObjectId parent = adobjectId.Parent;
									if (childId != null && parent != null && ADObjectId.Equals(childId, parent))
									{
										searchRequest.Scope = SearchScope.OneLevel;
									}
								}
							}
						}
						if (TopologyProvider.IsAdamTopology() && string.IsNullOrEmpty(searchRequest.DistinguishedName))
						{
							searchRequest.Controls.Add(new SearchOptionsControl(SearchOption.PhantomRoot));
						}
						ExTraceGlobals.ADFindTracer.TraceDebug((long)this.GetHashCode(), "ADGenericReader::GetNextResultCollection({0}) using {1} - LDAP search from {2}, scope {3}, filter {4}", new object[]
						{
							controlType.Name,
							readConnection.ADServerInfo.FqdnPlusPort,
							searchRequest.DistinguishedName,
							(int)searchRequest.Scope,
							searchRequest.Filter
						});
						serviceProviderRequestId = Trace.TraceCasStart(CasTraceEventType.ActiveDirectory);
						searchResponse = (SearchResponse)readConnection.SendRequest(searchRequest, LdapOperation.Search, null, this.session.ActivityScope, this.session.CallerInfo);
						this.preferredServerName = readConnection.ServerName;
						this.session.UpdateServerSettings(readConnection);
						break;
					}
					catch (DirectoryException de)
					{
						if (this.customExceptionHandler != null)
						{
							this.customExceptionHandler(de);
						}
						if (readConnection.IsResultCode(de, ResultCode.NoSuchObject))
						{
							ExTraceGlobals.ADFindTracer.TraceWarning<string, object>((long)this.GetHashCode(), "NoSuchObject caught when searching from {0} with filter {1}", searchRequest.DistinguishedName, searchRequest.Filter);
							return null;
						}
						if (readConnection.IsResultCode(de, ResultCode.VirtualListViewError) && this.lcid != LcidMapper.DefaultLcid)
						{
							ExTraceGlobals.ADFindTracer.TraceWarning<int, int>((long)this.GetHashCode(), "VirtualListView error caught when performing a VLV lookup using LCID 0x{0:X}. Falling back to US English 0x{1:X}", this.lcid, LcidMapper.DefaultLcid);
							this.RefreshSortControlWithDefaultLCID(searchRequest);
						}
						else
						{
							retryManager.Tried(readConnection.ServerName);
							this.session.AnalyzeDirectoryError(readConnection, searchRequest, de, retryManager.TotalRetries, retryManager[readConnection.ServerName]);
						}
					}
					continue;
				}
				finally
				{
					bool isSnapshotInProgress = PerformanceContext.Current.IsSnapshotInProgress;
					bool flag2 = ETWTrace.ShouldTraceCasStop(serviceProviderRequestId);
					if (isSnapshotInProgress || flag2)
					{
						string text = string.Format(CultureInfo.InvariantCulture, "scope: {0}, filter: {1}", new object[]
						{
							searchRequest.Scope,
							searchRequest.Filter
						});
						if (isSnapshotInProgress)
						{
							PerformanceContext.Current.AppendToOperations(text);
						}
						if (flag2)
						{
							Trace.TraceCasStop(CasTraceEventType.ActiveDirectory, serviceProviderRequestId, 0, 0, readConnection.ADServerInfo.FqdnPlusPort, searchRequest.DistinguishedName, "ADGenericReader::GetNextResultCollection", text, string.Empty);
						}
					}
					readConnection.ReturnToPool();
				}
				break;
			}
			responseControl = this.FindControlInCollection(searchResponse.Controls, controlType);
			return searchResponse.Entries;
		}

		private DirectoryControl FindControlInCollection(IEnumerable controls, Type controlType)
		{
			foreach (object obj in controls)
			{
				DirectoryControl directoryControl = (DirectoryControl)obj;
				if (directoryControl.GetType().Equals(controlType))
				{
					return directoryControl;
				}
			}
			return null;
		}

		private void AddSortControl()
		{
			ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)this.sortBy.ColumnDefinition;
			SortRequestControl sortRequestControl = new SortRequestControl(adpropertyDefinition.LdapDisplayName, LcidMapper.OidFromLcid(this.lcid), this.sortBy.SortOrder == SortOrder.Descending);
			sortRequestControl.IsCritical = false;
			ExTraceGlobals.ADFindTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "ADGenericReader::AddSortControl - Sort on {0}, {1} using rule {2})", sortRequestControl.SortKeys[0].AttributeName, sortRequestControl.SortKeys[0].ReverseOrder ? "reverse order (descending)" : "regular  order (ascending)", sortRequestControl.SortKeys[0].MatchingRule);
			this.DirectoryControls.Add(sortRequestControl);
		}

		private void RefreshSortControlWithDefaultLCID(DirectoryRequest request)
		{
			DirectoryControl value = this.FindControlInCollection(this.DirectoryControls, typeof(SortRequestControl));
			this.DirectoryControls.Remove(value);
			this.lcid = LcidMapper.DefaultLcid;
			this.AddSortControl();
			request.Controls.Clear();
			request.Controls.AddRange(this.DirectoryControls);
		}

		private const CasTraceEventType ActiveDirectoryTraceEventType = CasTraceEventType.ActiveDirectory;

		private const int UnlimitedSizeLimit = 0;

		private static readonly ShowDeletedControl showDeletedControl = new ShowDeletedControl();

		private static readonly SearchOptionsControl searchOptionsControl = new SearchOptionsControl(SearchOption.PhantomRoot);

		private string preferredServerName;

		private ADObjectId rootId;

		private QueryScope scope;

		private byte[] cookie;

		private IDirectorySession session;

		private SortBy sortBy;

		private int lcid;

		private string[] ldapAttributes;

		private string ldapFilter;

		private DirectoryControlCollection directoryControls;

		private CustomExceptionHandler customExceptionHandler;

		private bool useNullRoot;

		private bool includeDeletedObjects;

		private bool searchAllNcs;

		private bool isEmptyReader;
	}
}
