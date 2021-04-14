using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.AddressBook.Nspi;
using Microsoft.Exchange.AddressBook.Nspi.Client;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Mapi;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal class NspiContext : IDisposeTrackable, IDisposable
	{
		private static string Hex(PropTag tag)
		{
			return string.Format(NumberFormatInfo.InvariantInfo, "{0:X8}", new object[]
			{
				(int)tag
			});
		}

		internal NspiContext(ClientSecurityContext clientSecurityContext, string userDomain, string clientAddress, string serverAddress, string protocolSequence, Guid requestId = default(Guid))
		{
			this.disposeTracker = ((IDisposeTrackable)this).GetDisposeTracker();
			this.clientSecurityContext = clientSecurityContext;
			this.userDomain = userDomain;
			this.contextHandle = NspiContext.GetNextContextHandle();
			this.protocolSequence = protocolSequence;
			this.protocolLogSession = ProtocolLog.CreateSession(this.contextHandle, clientAddress, serverAddress, protocolSequence);
			if (requestId != Guid.Empty)
			{
				ActivityContextState activityContextState = new ActivityContextState(new Guid?(requestId), new ConcurrentDictionary<Enum, object>());
				ActivityContext.ClearThreadScope();
				this.scope = ActivityContext.Resume(activityContextState, null);
			}
		}

		internal int ContextHandle
		{
			get
			{
				return this.contextHandle;
			}
		}

		internal EphemeralIdTable EphemeralIdTable
		{
			get
			{
				return this.ephemeralIdTable;
			}
		}

		internal ADObjectId ConfigNamingContext
		{
			get
			{
				return NspiContext.configNamingContext;
			}
		}

		internal ADObjectId DomainNamingContext
		{
			get
			{
				return NspiContext.domainNamingContext;
			}
		}

		internal ClientSecurityContext ClientSecurityContext
		{
			get
			{
				return this.clientSecurityContext;
			}
		}

		internal string UserDomain
		{
			get
			{
				return this.userDomain;
			}
		}

		internal bool TraceUser
		{
			get
			{
				return this.traceUser;
			}
		}

		internal Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		internal string UserIdentity
		{
			get
			{
				return this.userIdentity ?? string.Empty;
			}
		}

		internal ProtocolLogSession ProtocolLogSession
		{
			get
			{
				return this.protocolLogSession;
			}
		}

		internal IStandardBudget Budget
		{
			get
			{
				if (this.budget == null)
				{
					throw new InvalidOperationException("Budget has not been acquired");
				}
				return this.budget;
			}
		}

		internal NspiPrincipal NspiPrincipal
		{
			get
			{
				return this.nspiPrincipal;
			}
		}

		internal bool IsAnonymous
		{
			get
			{
				return this.isAnonymous;
			}
			set
			{
				this.isAnonymous = value;
			}
		}

		internal bool IsUsingHttp
		{
			get
			{
				return this.protocolSequence.Equals("ncacn_http", StringComparison.OrdinalIgnoreCase) || this.protocolSequence.Equals("MapiHttp");
			}
		}

		public IActivityScope ActivityScope
		{
			get
			{
				if (this.scope != null && !this.scope.IsDisposed)
				{
					return this.scope;
				}
				return null;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		DisposeTracker IDisposeTrackable.GetDisposeTracker()
		{
			return DisposeTracker.Get<NspiContext>(this);
		}

		void IDisposeTrackable.SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal static int GetNextContextHandle()
		{
			int num = Interlocked.Increment(ref NspiContext.lastContextHandle);
			if (num == 0)
			{
				num = Interlocked.Increment(ref NspiContext.lastContextHandle);
			}
			return num;
		}

		internal bool TryAcquireBudget()
		{
			if (this.budget != null)
			{
				throw new InvalidOperationException("Budget already acquired");
			}
			Exception ex2;
			try
			{
				ADSessionSettings settings;
				if (!string.IsNullOrEmpty(this.userDomain))
				{
					settings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(this.userDomain);
				}
				else
				{
					settings = ADSessionSettings.FromRootOrgScopeSet();
				}
				this.budget = StandardBudget.Acquire(this.ClientSecurityContext.UserSid, BudgetType.Rca, settings);
				return true;
			}
			catch (DataValidationException ex)
			{
				ex2 = ex;
			}
			catch (ADTransientException ex3)
			{
				ex2 = ex3;
			}
			catch (ADOperationException ex4)
			{
				ex2 = ex4;
			}
			NspiContext.NspiTracer.TraceError<string>((long)this.contextHandle, "TryAcquireBudget exception: {0}", ex2.Message);
			return false;
		}

		internal NspiStatus Bind(NspiBindFlags flags, NspiState state)
		{
			if (this.clientSecurityContext == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, "Bind: clientSecurityContext == null");
				return NspiStatus.LogonFailed;
			}
			if (string.IsNullOrEmpty(this.userDomain))
			{
				this.protocolLogSession[ProtocolLog.Field.OperationSpecific] = "userDomain_is_null";
				bool isDatacenter = Configuration.IsDatacenter;
			}
			this.nspiPrincipal = NspiPrincipal.FromUserSid(this.ClientSecurityContext.UserSid, this.userDomain);
			this.userIdentity = this.nspiPrincipal.LegacyDistinguishedName;
			this.protocolLogSession[ProtocolLog.Field.ClientName] = this.userIdentity;
			if (this.nspiPrincipal.OrganizationId != null && this.nspiPrincipal.OrganizationId.OrganizationalUnit != null)
			{
				this.protocolLogSession[ProtocolLog.Field.OrganizationInfo] = this.nspiPrincipal.OrganizationId.OrganizationalUnit.ToCanonicalName();
			}
			this.traceUser = ExUserTracingAdaptor.Instance.IsTracingEnabledUser(this.nspiPrincipal.LegacyDistinguishedName);
			if (this.traceUser)
			{
				BaseTrace.CurrentThreadSettings.EnableTracing();
			}
			NspiContext.NspiTracer.TraceDebug<NspiBindFlags>((long)this.contextHandle, "NspiBindFlags: {0}", flags);
			if ((flags & NspiBindFlags.AnonymousLogin) != NspiBindFlags.None)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, "NspiBindFlags.AnonymousLogin not supported");
				return NspiStatus.NotSupported;
			}
			if (state.CodePage == Encoding.Unicode.CodePage)
			{
				NspiContext.NspiTracer.TraceError<int>((long)this.contextHandle, "Invalid code page: {0}", state.CodePage);
				return NspiStatus.InvalidCodePage;
			}
			NspiContext.NspiTracer.TraceDebug<SmtpAddress, SecurityIdentifier>((long)this.contextHandle, "Bind {0} {1}", this.nspiPrincipal.PrimarySmtpAddress, this.ClientSecurityContext.UserSid);
			NspiContext.NspiTracer.TraceDebug<string>((long)this.contextHandle, "LegacyDN {0}", this.nspiPrincipal.LegacyDistinguishedName);
			if (!this.nspiPrincipal.MAPIEnabled)
			{
				NspiContext.NspiTracer.TraceError<string>((long)this.contextHandle, "User is not MAPI enabled: {0}", this.userIdentity);
				return NspiStatus.AccessDenied;
			}
			this.InitializeNamingContext();
			this.sortLocale = state.SortLocale;
			return NspiStatus.Success;
		}

		internal void Unbind(bool calledFromRundown)
		{
			this.stopwatch.Stop();
			this.protocolLogSession[ProtocolLog.Field.OperationSpecific] = (int)this.stopwatch.ElapsedMilliseconds;
			if (calledFromRundown)
			{
				NspiContext.NspiTracer.TraceDebug((long)this.contextHandle, "Unbind: Disconnected");
				this.protocolLogSession[ProtocolLog.Field.Failures] = "Disconnected";
			}
		}

		internal NspiStatus GetHierarchyInfo(NspiGetHierarchyInfoFlags flags, NspiState state, ref uint version, out PropRowSet rowset)
		{
			rowset = null;
			if (state == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, NspiContext.nullStateMessage);
				return NspiStatus.InvalidParameter;
			}
			NspiContext.NspiTracer.TraceDebug<NspiGetHierarchyInfoFlags>((long)this.contextHandle, "NspiGetHierarchyFlags: {0}", flags);
			this.sortLocale = state.SortLocale;
			if ((flags & NspiGetHierarchyInfoFlags.OneOff) == NspiGetHierarchyInfoFlags.OneOff)
			{
				return this.GetOneOffTable(flags, state, ref version, out rowset);
			}
			return this.GetHierarchyTable(flags, state, ref version, out rowset);
		}

		internal NspiStatus GetMatches(NspiState nspiState, Restriction restriction, int requested, PropTag[] requestedProperties, out int[] mids, out PropRowSet rowset)
		{
			mids = NspiContext.emptyIntArray;
			rowset = null;
			if (nspiState == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, NspiContext.nullStateMessage);
				return NspiStatus.InvalidParameter;
			}
			NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "Rows requested: {0}", requested);
			CultureInfo cultureInfo = this.GetCultureInfo(nspiState.SortLocale);
			this.sortLocale = cultureInfo.LCID;
			if (nspiState.ContainerId == -1)
			{
				NspiContext.NspiTracer.TraceError<int>((long)this.contextHandle, "nspiStateContainerId is invalid: {0}", nspiState.ContainerId);
				return NspiStatus.InvalidBookmark;
			}
			NspiPropMapperFlags nspiPropMapperFlags = NspiPropMapperFlags.UseEphemeralId;
			if (nspiState.SortIndex == SortIndex.DisplayNameReadOnly || nspiState.SortIndex == SortIndex.DisplayNameWritable)
			{
				nspiPropMapperFlags |= NspiPropMapperFlags.IncludeDisplayName;
			}
			NspiPropMapper nspiPropMapper = new NspiPropMapper(this, requestedProperties, nspiState.CodePage, nspiPropMapperFlags);
			bool flag = false;
			ADObjectId adobjectId;
			QueryFilter queryFilter;
			if (restriction == null)
			{
				adobjectId = this.nspiPrincipal.GlobalAddressListFromAddressBookPolicy;
				Guid guid;
				EphemeralIdTable.NamingContext namingContext;
				if (!this.ephemeralIdTable.GetGuid(nspiState.CurrentRecord, out guid, out namingContext))
				{
					NspiContext.NspiTracer.TraceError<int>((long)this.contextHandle, "Failed to find guid for mid: {0}", nspiState.CurrentRecord);
					return NspiStatus.GeneralFailure;
				}
				if (nspiState.SortIndex == SortIndex.DisplayNameReadOnly || nspiState.SortIndex == SortIndex.DisplayNameWritable)
				{
					PropTag containerId = (PropTag)nspiState.ContainerId;
					this.protocolLogSession[ProtocolLog.Field.OperationSpecific] = NspiContext.Hex(containerId);
					int currentRecord = nspiState.CurrentRecord;
					PropertyDefinition propertyDefinitionForLinkedAttribute = NspiPropMapper.GetPropertyDefinitionForLinkedAttribute(containerId);
					if (propertyDefinitionForLinkedAttribute == null)
					{
						if (containerId.Id() == ((PropTag)2151157791U).Id())
						{
							return this.GetContactsOfPublicFolder(nspiState, requested, requestedProperties, guid, cultureInfo, out mids, out rowset);
						}
						rowset = new PropRowSet(0);
						return NspiStatus.Success;
					}
					else
					{
						if (containerId.Id() == ((PropTag)2148073485U).Id() && this.HideDLMembers(guid))
						{
							rowset = new PropRowSet(0);
							return NspiStatus.Success;
						}
						queryFilter = new ComparisonFilter(ComparisonOperator.Equal, propertyDefinitionForLinkedAttribute, new ADObjectId(guid));
						if (this.modCache != null && this.modCache.HasMods(currentRecord, containerId))
						{
							queryFilter = this.modCache.ApplyMods(queryFilter, currentRecord, containerId);
							flag = true;
						}
					}
				}
				else
				{
					queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Guid, guid);
				}
			}
			else
			{
				adobjectId = this.GetAddressListScope(nspiState.ContainerId);
				if (adobjectId == null)
				{
					NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "Failed to get address list scope. ContainerId: {0}", nspiState.ContainerId);
					return NspiStatus.InvalidBookmark;
				}
				QueryFilterBuilder queryFilterBuilder = new QueryFilterBuilder(nspiPropMapper.NonsubstitutionEncoding);
				if (nspiState.ContainerId == 0 && QueryFilterBuilder.IsAnrRestriction(restriction))
				{
					queryFilter = queryFilterBuilder.TranslateANR(restriction, this.nspiPrincipal.LegacyDistinguishedName, adobjectId);
					adobjectId = null;
				}
				else
				{
					queryFilter = queryFilterBuilder.TranslateRestriction(restriction);
				}
			}
			queryFilter = QueryFilterBuilder.RestrictToVisibleItems(queryFilter, this.nspiPrincipal.LegacyDistinguishedName);
			NspiContext.NspiTracer.TraceDebug<QueryFilter>((long)this.contextHandle, "QueryFilter={0}", queryFilter);
			IRecipientSession recipientSession = this.GetRecipientSession(adobjectId);
			ADPagedReader<ADRawEntry> adpagedReader;
			try
			{
				adpagedReader = recipientSession.FindPagedADRawEntry(null, QueryScope.SubTree, queryFilter, this.GetSortOrder(nspiState), 0, nspiPropMapper.PropertyDefinitions);
			}
			catch (ADFilterException ex)
			{
				if (flag)
				{
					NspiContext.NspiTracer.TraceDebug((long)this.contextHandle, "ADFilterException thrown: will retry query without cached mods");
					this.modCache.PurgeMods(nspiState.CurrentRecord, (PropTag)nspiState.ContainerId);
					throw new ADTransientException(ex.LocalizedString, ex);
				}
				throw;
			}
			List<ADRawEntry> list = new List<ADRawEntry>();
			foreach (ADRawEntry item in adpagedReader)
			{
				if (list.Count >= requested)
				{
					NspiContext.NspiTracer.TraceError<int>((long)this.contextHandle, "Result contains more rows than requested ({0})", requested);
					return NspiStatus.TableTooBig;
				}
				list.Add(item);
			}
			nspiPropMapper.ResolveLinks(list);
			mids = new int[list.Count];
			if (requestedProperties != null && requestedProperties.Length > 0)
			{
				rowset = new PropRowSet(Math.Min(list.Count, 50));
			}
			if (nspiState.SortIndex == SortIndex.DisplayNameReadOnly || nspiState.SortIndex == SortIndex.DisplayNameWritable)
			{
				list.Sort(new NspiContext.DisplayNameComparer(cultureInfo));
			}
			for (int i = 0; i < list.Count; i++)
			{
				ADRawEntry adrawEntry = list[i];
				mids[i] = this.ephemeralIdTable.CreateEphemeralId(adrawEntry.Id.ObjectGuid, EphemeralIdTable.GetNamingContext(adrawEntry.Id));
				NspiContext.NspiTracer.TraceDebug<int, ADObjectId, int>((long)this.contextHandle, "Row {0}: {1} (Mid:{2})", i, adrawEntry.Id, mids[i]);
				if (rowset != null && rowset.Rows.Count < 50)
				{
					rowset.Add(nspiPropMapper.GetProps(adrawEntry));
				}
			}
			return NspiStatus.Success;
		}

		private NspiStatus GetContactsOfPublicFolder(NspiState nspiState, int requested, PropTag[] requestedProperties, Guid guid, CultureInfo cultureInfo, out int[] mids, out PropRowSet rowset)
		{
			mids = NspiContext.emptyIntArray;
			rowset = null;
			NspiPropMapperFlags nspiPropMapperFlags = NspiPropMapperFlags.UseEphemeralId | NspiPropMapperFlags.IncludeDisplayName;
			Guid[] objectGuids = new Guid[]
			{
				guid
			};
			PropTag[] requestedProperties2 = new PropTag[]
			{
				(PropTag)2151157791U
			};
			NspiPropMapper nspiPropMapper = new NspiPropMapper(this, requestedProperties2, nspiState.CodePage, nspiPropMapperFlags);
			ADObjectId addressListScope = null;
			IDirectorySession recipientSession = this.GetRecipientSession(addressListScope);
			Result<ADRawEntry>[] array = recipientSession.FindByObjectGuids(objectGuids, nspiPropMapper.PropertyDefinitions);
			if (array.Length == 0)
			{
				NspiContext.NspiTracer.TraceError<string>((long)this.contextHandle, "the public folder {0} was not found", guid.ToString());
				return NspiStatus.GeneralFailure;
			}
			PropRow props = nspiPropMapper.GetProps(array[0].Data);
			if (props.Properties.Count == 0 || (props.Properties[0].IsError() && props.Properties[0].GetErrorValue() == -2147221233))
			{
				NspiContext.NspiTracer.TraceError<string>((long)this.contextHandle, "no contact of the public folder {0} was found", guid.ToString());
				rowset = new PropRowSet(0);
				return NspiStatus.Success;
			}
			string[] array2 = (string[])props.Properties[0].Value;
			NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "{0} contacts were found", array2.Length);
			nspiPropMapperFlags |= NspiPropMapperFlags.IncludeHiddenFromAddressListsEnabled;
			nspiPropMapper = new NspiPropMapper(this, requestedProperties, nspiState.CodePage, nspiPropMapperFlags);
			List<ADRawEntry> list = new List<ADRawEntry>();
			foreach (string distinguishedName in array2)
			{
				if (list.Count >= requested)
				{
					NspiContext.NspiTracer.TraceError<int>((long)this.contextHandle, "Result contains more rows than requested ({0})", requested);
					return NspiStatus.TableTooBig;
				}
				ADRawEntry adrawEntry = recipientSession.ReadADRawEntry(new ADObjectId(distinguishedName), nspiPropMapper.PropertyDefinitions);
				RecipientType recipientType = (RecipientType)adrawEntry[ADRecipientSchema.RecipientType];
				if (this.VisibleEntry(adrawEntry) && (recipientType == RecipientType.MailContact || recipientType == RecipientType.UserMailbox || recipientType == RecipientType.MailUser))
				{
					list.Add(adrawEntry);
				}
			}
			if (list.Count == 0)
			{
				NspiContext.NspiTracer.TraceDebug<string>((long)this.contextHandle, "No visible contact of the public folder {0} was found", guid.ToString());
				rowset = new PropRowSet(0);
				return NspiStatus.Success;
			}
			list.Sort(new NspiContext.DisplayNameComparer(cultureInfo));
			mids = new int[list.Count];
			if (requestedProperties != null && requestedProperties.Length > 0)
			{
				rowset = new PropRowSet(Math.Min(list.Count, 50));
			}
			for (int j = 0; j < list.Count; j++)
			{
				ADRawEntry adrawEntry2 = list[j];
				mids[j] = this.ephemeralIdTable.CreateEphemeralId(adrawEntry2.Id.ObjectGuid, EphemeralIdTable.NamingContext.Domain);
				NspiContext.NspiTracer.TraceDebug<int, ADObjectId, int>((long)this.contextHandle, "Row {0}: {1} (Mid:{2})", j, adrawEntry2.Id, mids[j]);
				if (rowset != null && rowset.Rows.Count < 50)
				{
					rowset.Add(nspiPropMapper.GetProps(adrawEntry2));
				}
			}
			return NspiStatus.Success;
		}

		internal NspiStatus QueryRows(NspiQueryRowsFlags flags, NspiState nspiState, int[] midTable, int count, PropTag[] requestedProperties, out PropRowSet rowset)
		{
			rowset = null;
			if (nspiState == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, NspiContext.nullStateMessage);
				return NspiStatus.InvalidParameter;
			}
			bool flag = midTable == null || midTable.Length == 0;
			NspiContext.NspiTracer.TraceDebug<NspiQueryRowsFlags, int, bool>((long)this.contextHandle, "NspiRetrievePropertyFlags: {0}, rows requested: {1}, browse: {2}", flags, count, flag);
			this.sortLocale = nspiState.SortLocale;
			if (nspiState.ContainerId == -1)
			{
				NspiContext.NspiTracer.TraceError<int>((long)this.contextHandle, "nspiStateContainerId is invalid: {0}", nspiState.ContainerId);
				return NspiStatus.InvalidBookmark;
			}
			if (requestedProperties == null || requestedProperties.Length == 0)
			{
				requestedProperties = NspiContext.defaultQueryRowsPropertiesAnsi;
			}
			NspiPropMapperFlags nspiPropMapperFlags = NspiPropMapperFlags.IncludeHiddenFromAddressListsEnabled;
			if ((flags & NspiQueryRowsFlags.EphemeralIds) == NspiQueryRowsFlags.EphemeralIds)
			{
				nspiPropMapperFlags |= NspiPropMapperFlags.UseEphemeralId;
			}
			NspiPropMapper nspiPropMapper = new NspiPropMapper(this, requestedProperties, nspiState.CodePage, nspiPropMapperFlags);
			if (flag)
			{
				return this.QueryRowsVlv(flags, nspiState, count, nspiPropMapper, out rowset);
			}
			int[] array;
			this.ConvertMidsToAddressBook(nspiState, midTable, out array);
			midTable = array;
			PropRow[] array2 = new PropRow[midTable.Length];
			rowset = new PropRowSet(midTable.Length);
			Guid[] array3;
			EphemeralIdTable.NamingContext[] array4;
			this.ephemeralIdTable.ConvertIdsToGuids(midTable, out array3, out array4);
			List<Guid> list = new List<Guid>(midTable.Length);
			List<int> list2 = new List<int>(midTable.Length);
			for (int i = 0; i < midTable.Length; i++)
			{
				if (array4[i] == EphemeralIdTable.NamingContext.Domain)
				{
					list.Add(array3[i]);
					list2.Add(i);
				}
			}
			if (list.Count > 0)
			{
				IRecipientSession recipientSession = this.GetRecipientSession(null);
				Result<ADRawEntry>[] array5 = recipientSession.FindByObjectGuids(list.ToArray(), nspiPropMapper.PropertyDefinitions);
				nspiPropMapper.ResolveLinks(array5);
				for (int j = 0; j < array5.Length; j++)
				{
					ADRawEntry data = array5[j].Data;
					if (data != null && (this.VisibleEntry(data) || !Datacenter.IsMultiTenancyEnabled()))
					{
						array2[list2[j]] = nspiPropMapper.GetProps(data);
					}
				}
			}
			foreach (PropRow propRow in array2)
			{
				rowset.Add(propRow ?? nspiPropMapper.GetErrorRow());
			}
			return NspiStatus.Success;
		}

		internal NspiStatus DNToEph(string[] legacyDNArray, out int[] mids)
		{
			mids = null;
			if (legacyDNArray == null || legacyDNArray.Length == 0)
			{
				mids = NspiContext.emptyIntArray;
				return NspiStatus.Success;
			}
			for (int i = 0; i < legacyDNArray.Length; i++)
			{
				legacyDNArray[i] = ExchangeRpcClientAccess.FixFakeRedirectLegacyDNIfNeeded(legacyDNArray[i]);
			}
			this.TryResolveLegacyDNs(legacyDNArray, out mids);
			return NspiStatus.Success;
		}

		internal NspiStatus GetProps(NspiGetPropsFlags flags, NspiState nspiState, PropTag[] requestedProperties, out PropRow row)
		{
			row = null;
			if (nspiState == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, NspiContext.nullStateMessage);
				return NspiStatus.InvalidParameter;
			}
			NspiContext.NspiTracer.TraceDebug<NspiGetPropsFlags>((long)this.contextHandle, "NspiRetrievePropertyFlags: {0}", flags);
			this.sortLocale = nspiState.SortLocale;
			IList<PropTag> list = requestedProperties;
			NspiPropMapperFlags nspiPropMapperFlags = NspiPropMapperFlags.IncludeHiddenFromAddressListsEnabled;
			if (list == null)
			{
				list = ((nspiState.CodePage == Encoding.Unicode.CodePage) ? NspiPropMapper.SupportedPropTagsUnicode : NspiPropMapper.SupportedPropTagsAnsi);
				nspiPropMapperFlags |= NspiPropMapperFlags.SkipMissingProperties;
			}
			if ((flags & NspiGetPropsFlags.SkipObjects) == NspiGetPropsFlags.SkipObjects)
			{
				nspiPropMapperFlags |= NspiPropMapperFlags.SkipObjects;
			}
			if ((flags & NspiGetPropsFlags.EphemeralIds) == NspiGetPropsFlags.EphemeralIds)
			{
				nspiPropMapperFlags |= NspiPropMapperFlags.UseEphemeralId;
			}
			NspiPropMapper nspiPropMapper = new NspiPropMapper(this, list, nspiState.CodePage, nspiPropMapperFlags);
			int[] array;
			this.ConvertMidsToAddressBook(nspiState, new int[]
			{
				nspiState.CurrentRecord
			}, out array);
			int num = array[0];
			Guid guid;
			EphemeralIdTable.NamingContext nc;
			if (!this.ephemeralIdTable.GetGuid(num, out guid, out nc))
			{
				row = nspiPropMapper.GetErrorRow();
				NspiContext.NspiTracer.TraceError<Guid>((long)this.contextHandle, "Failed to find guid for mid: {0}", guid);
				return NspiStatus.ErrorsReturned;
			}
			bool flag = false;
			Result<ADRawEntry>[] array3;
			if (this.personalizedServerCache != null && this.personalizedServerCache.ContainsKey(guid))
			{
				string[] array2 = new string[NspiContext.networkAddressPatterns.Length];
				for (int i = 0; i < NspiContext.networkAddressPatterns.Length; i++)
				{
					array2[i] = string.Format(CultureInfo.InvariantCulture, NspiContext.networkAddressPatterns[i], new object[]
					{
						this.personalizedServerCache[guid]
					});
				}
				ADPropertyDefinition adpropertyDefinition = NspiPropMapper.GetPropertyDefinition((PropTag)2171605022U) as ADPropertyDefinition;
				if (adpropertyDefinition == null)
				{
					NspiContext.NspiTracer.TraceDebug<int, Guid>((long)this.contextHandle, "Mid {0}: {1} **ERROR**. NspiPropMapper.GetPropertyDefinition(PropTag.AbNetworkAddress) does not return the ADPropertyDefinition object", num, guid);
					return NspiStatus.ErrorsReturned;
				}
				MultiValuedProperty<string> value = new MultiValuedProperty<string>(true, adpropertyDefinition, array2);
				ADPropertyBag adpropertyBag = new ADPropertyBag();
				adpropertyBag.SetField(adpropertyDefinition, value);
				adpropertyBag.SetField(ADObjectSchema.Id, Configuration.MicrosoftExchangeConfigurationRoot);
				array3 = new Result<ADRawEntry>[]
				{
					new Result<ADRawEntry>(new ADRawEntry(adpropertyBag), null)
				};
			}
			else
			{
				IDirectorySession adsessionFromNamingContext = this.GetADSessionFromNamingContext(nc);
				Guid[] objectGuids = new Guid[]
				{
					guid
				};
				array3 = adsessionFromNamingContext.FindByObjectGuids(objectGuids, nspiPropMapper.PropertyDefinitions);
			}
			nspiPropMapper.ResolveLinks(array3);
			if (array3[0].Data != null && (this.VisibleEntry(array3[0].Data) || !Datacenter.IsMultiTenancyEnabled()))
			{
				NspiContext.NspiTracer.TraceDebug<int, Guid>((long)this.contextHandle, "Mid {0}: {1}", num, guid);
				row = nspiPropMapper.GetProps(array3[0].Data);
				using (IEnumerator<PropValue> enumerator = row.Properties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PropValue propValue = enumerator.Current;
						if (propValue.IsError())
						{
							flag = true;
							break;
						}
					}
					goto IL_2F5;
				}
			}
			NspiContext.NspiTracer.TraceDebug<int, Guid>((long)this.contextHandle, "Mid {0}: {1} **ERROR**", num, guid);
			row = nspiPropMapper.GetErrorRow();
			flag = true;
			IL_2F5:
			NspiContext.NspiTracer.TraceDebug<int, bool>((long)this.contextHandle, "Total props returned: {0}, hasErrors: {1}", row.Properties.Count, flag);
			if (!flag)
			{
				return NspiStatus.Success;
			}
			return NspiStatus.ErrorsReturned;
		}

		internal NspiStatus ResolveNames(NspiState nspiState, PropTag[] requestedProperties, object[] inputNames, out int[] mids, out PropRowSet rowset)
		{
			mids = null;
			rowset = null;
			if (nspiState == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, NspiContext.nullStateMessage);
				return NspiStatus.InvalidParameter;
			}
			this.sortLocale = nspiState.SortLocale;
			if (nspiState.ContainerId == -1)
			{
				NspiContext.NspiTracer.TraceError<int>((long)this.contextHandle, "nspiStateContainerId is invalid: {0}", nspiState.ContainerId);
				return NspiStatus.InvalidBookmark;
			}
			ADObjectId addressListScope = this.GetAddressListScope(nspiState.ContainerId);
			if (addressListScope == null)
			{
				NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "Failed to get address list scope. ContainerId: {0}", nspiState.ContainerId);
				return NspiStatus.InvalidBookmark;
			}
			if (requestedProperties == null || requestedProperties.Length == 0)
			{
				requestedProperties = NspiContext.defaultResolveNamesPropertiesAnsi;
			}
			NspiPropMapper nspiPropMapper = new NspiPropMapper(this, requestedProperties, nspiState.CodePage);
			string[] array = inputNames as string[];
			if (array == null)
			{
				Encoding nonsubstitutionEncoding = nspiPropMapper.NonsubstitutionEncoding;
				array = new string[inputNames.Length];
				for (int i = 0; i < inputNames.Length; i++)
				{
					if (inputNames[i] != null)
					{
						array[i] = nonsubstitutionEncoding.GetString((byte[])inputNames[i]);
					}
				}
			}
			ADRawEntry[] array2 = new ADRawEntry[array.Length];
			mids = new int[array.Length];
			List<int> list = new List<int>(array.Length);
			List<string> list2 = new List<string>(array.Length);
			List<int> list3 = new List<int>(array.Length);
			List<string> list4 = new List<string>(array.Length);
			for (int j = 0; j < array.Length; j++)
			{
				NspiContext.NspiTracer.TraceDebug<int, string>((long)this.contextHandle, "Row {0}: ANR={1}", j, array[j] ?? "<null>");
				mids[j] = 0;
				if (string.IsNullOrEmpty(array[j]))
				{
					NspiContext.NspiTracer.TraceDebug((long)this.contextHandle, "Unresolved empty string");
				}
				else if (array[j][0] == '/')
				{
					list2.Add(LegacyDN.NormalizeDN(array[j]));
					list.Add(j);
				}
				else if (array[j].StartsWith("EX:/", StringComparison.OrdinalIgnoreCase))
				{
					list2.Add(LegacyDN.NormalizeDN(array[j].Substring(3)));
					list.Add(j);
				}
				else
				{
					list4.Add(array[j]);
					list3.Add(j);
				}
			}
			IRecipientSession recipientSession;
			if (list2.Count > 0)
			{
				if (!Datacenter.IsMultiTenancyEnabled())
				{
					recipientSession = this.GetRecipientSession(null);
				}
				else
				{
					recipientSession = this.GetRecipientSession(addressListScope);
				}
				Result<ADRawEntry>[] array3 = recipientSession.FindByExchangeLegacyDNs(list2.ToArray(), nspiPropMapper.PropertyDefinitions);
				for (int k = 0; k < array3.Length; k++)
				{
					if (array3[k].Data != null)
					{
						mids[list[k]] = 2;
						array2[list[k]] = array3[k].Data;
					}
					else
					{
						mids[list[k]] = 0;
					}
				}
			}
			recipientSession = this.GetRecipientSession(null);
			QueryFilterBuilder queryFilterBuilder = new QueryFilterBuilder(nspiPropMapper.NonsubstitutionEncoding);
			for (int l = 0; l < list4.Count; l++)
			{
				QueryFilter filter = queryFilterBuilder.TranslateANR(list4[l], this.nspiPrincipal.LegacyDistinguishedName, addressListScope);
				if (Datacenter.IsMultiTenancyEnabled())
				{
					filter = QueryFilterBuilder.RestrictToVisibleItems(filter, this.nspiPrincipal.LegacyDistinguishedName);
				}
				ADRawEntry[] array4 = recipientSession.Find(null, QueryScope.SubTree, filter, null, 2, nspiPropMapper.PropertyDefinitions);
				switch (array4.Length)
				{
				case 0:
					NspiContext.NspiTracer.TraceDebug((long)this.contextHandle, "Unresolved");
					mids[list3[l]] = 0;
					break;
				case 1:
					NspiContext.NspiTracer.TraceDebug((long)this.contextHandle, "Found");
					mids[list3[l]] = 2;
					array2[list3[l]] = array4[0];
					break;
				default:
					NspiContext.NspiTracer.TraceDebug((long)this.contextHandle, "Ambiguous");
					mids[list3[l]] = 1;
					break;
				}
			}
			nspiPropMapper.ResolveLinks(array2);
			rowset = new PropRowSet(array2.Length);
			foreach (ADRawEntry adrawEntry in array2)
			{
				if (adrawEntry != null)
				{
					rowset.Add(nspiPropMapper.GetProps(adrawEntry));
				}
			}
			return NspiStatus.Success;
		}

		internal NspiStatus GetTemplateInfo(NspiGetTemplateInfoFlags flags, uint type, string dn, uint codePage, uint localeID, out PropRow row)
		{
			row = null;
			NspiContext.NspiTracer.TraceDebug<NspiGetTemplateInfoFlags>((long)this.contextHandle, "NspiGetTemplateInfoFlags: {0}", flags);
			if ((ulong)codePage == (ulong)((long)Encoding.Unicode.CodePage))
			{
				NspiContext.NspiTracer.TraceError<uint>((long)this.contextHandle, "Invalid code page: {0}", codePage);
				return NspiStatus.InvalidCodePage;
			}
			NspiPropMapper nspiPropMapper = new NspiPropMapper(this, flags, (int)codePage);
			ADRawEntry adrawEntry = null;
			if (string.IsNullOrEmpty(dn))
			{
				NspiContext.NspiTracer.TraceDebug<uint, uint>((long)this.contextHandle, "Template locale={0:X}, type={1:X}", localeID, type);
				IConfigurationSession rootOrgSystemConfigurationSession = this.GetRootOrgSystemConfigurationSession();
				ADObjectId childId = rootOrgSystemConfigurationSession.GetOrgContainerId().GetChildId("Addressing").GetChildId("Display-Templates").GetChildId(localeID.ToString("X"));
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, type.ToString("X"));
				ADRawEntry[] array = rootOrgSystemConfigurationSession.Find(childId, QueryScope.OneLevel, filter, null, 2, nspiPropMapper.PropertyDefinitions);
				if (array.Length == 1)
				{
					adrawEntry = array[0];
				}
			}
			else
			{
				dn = LegacyDN.NormalizeDN(dn);
				EphemeralIdTable.NamingContext[] array2 = new EphemeralIdTable.NamingContext[Datacenter.IsMultiTenancyEnabled() ? 2 : 1];
				if (Datacenter.IsMultiTenancyEnabled())
				{
					if (OrganizationId.ForestWideOrgId.Equals(this.nspiPrincipal.OrganizationId))
					{
						array2[0] = EphemeralIdTable.NamingContext.Config;
						array2[1] = EphemeralIdTable.NamingContext.TenantConfig;
					}
					else
					{
						array2[0] = EphemeralIdTable.NamingContext.TenantConfig;
						array2[1] = EphemeralIdTable.NamingContext.Config;
					}
				}
				else
				{
					array2[0] = EphemeralIdTable.NamingContext.Config;
				}
				foreach (EphemeralIdTable.NamingContext nc in array2)
				{
					IDirectorySession adsessionFromNamingContext = this.GetADSessionFromNamingContext(nc);
					NspiContext.NspiTracer.TraceDebug<string>((long)this.contextHandle, "Template DN={0}", dn);
					string[] exchangeLegacyDNs = new string[]
					{
						dn
					};
					Result<ADRawEntry>[] array4 = adsessionFromNamingContext.FindByExchangeLegacyDNs(exchangeLegacyDNs, nspiPropMapper.PropertyDefinitions);
					if (array4.Length == 1 && array4[0].Data != null)
					{
						adrawEntry = array4[0].Data;
						break;
					}
				}
			}
			if (adrawEntry == null)
			{
				return NspiStatus.InvalidLocale;
			}
			row = nspiPropMapper.GetProps(adrawEntry);
			return NspiStatus.Success;
		}

		internal NspiStatus UpdateStat(NspiState nspiState, out int delta)
		{
			NspiContext.<>c__DisplayClass1 CS$<>8__locals1 = new NspiContext.<>c__DisplayClass1();
			CS$<>8__locals1.returnedDelta = 0;
			if (nspiState == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, NspiContext.nullStateMessage);
				delta = CS$<>8__locals1.returnedDelta;
				return NspiStatus.InvalidParameter;
			}
			if (nspiState.SortIndex == SortIndex.PhoneticDisplayName)
			{
				if (!Configuration.EnablePhoneticSort)
				{
					this.protocolLogSession[ProtocolLog.Field.OperationSpecific] = "SortIndex: " + SortIndex.PhoneticDisplayName + " PhoneticSort disabled";
					delta = CS$<>8__locals1.returnedDelta;
					return NspiStatus.NotSupported;
				}
				this.protocolLogSession[ProtocolLog.Field.OperationSpecific] = "SortIndex: " + SortIndex.PhoneticDisplayName + " PhoneticSort enabled";
			}
			NspiStatus nspiStatus;
			using (NspiConnection connection = NspiConnectionPool.GetConnection(this.nspiServer, this.nspiPrincipal.OrganizationId.PartitionId))
			{
				nspiStatus = this.ExecutePassThrough(nspiState, connection, () => connection.Client.UpdateStat(out CS$<>8__locals1.returnedDelta));
			}
			delta = CS$<>8__locals1.returnedDelta;
			if (nspiStatus == NspiStatus.InvalidBookmark && nspiState.ContainerId == 0)
			{
				this.ResetStatBlockToZeroes(nspiState);
				nspiStatus = NspiStatus.Success;
			}
			return nspiStatus;
		}

		internal NspiStatus UpdateStat(NspiState nspiState)
		{
			if (nspiState == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, NspiContext.nullStateMessage);
				return NspiStatus.InvalidParameter;
			}
			if (nspiState.SortIndex == SortIndex.PhoneticDisplayName)
			{
				if (!Configuration.EnablePhoneticSort)
				{
					this.protocolLogSession[ProtocolLog.Field.OperationSpecific] = "SortIndex: " + SortIndex.PhoneticDisplayName + " PhoneticSort disabled";
					return NspiStatus.NotSupported;
				}
				this.protocolLogSession[ProtocolLog.Field.OperationSpecific] = "SortIndex: " + SortIndex.PhoneticDisplayName + " PhoneticSort enabled";
			}
			NspiStatus nspiStatus;
			using (NspiConnection connection = NspiConnectionPool.GetConnection(this.nspiServer, this.nspiPrincipal.OrganizationId.PartitionId))
			{
				nspiStatus = this.ExecutePassThrough(nspiState, connection, () => connection.Client.UpdateStat());
			}
			if (nspiStatus == NspiStatus.InvalidBookmark && nspiState.ContainerId == 0)
			{
				this.ResetStatBlockToZeroes(nspiState);
				nspiStatus = NspiStatus.Success;
			}
			return nspiStatus;
		}

		internal NspiStatus CompareMids(NspiState nspiState, int mid1, int mid2, out int result)
		{
			NspiContext.<>c__DisplayClassb CS$<>8__locals1 = new NspiContext.<>c__DisplayClassb();
			result = 0;
			if (nspiState == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, NspiContext.nullStateMessage);
				return NspiStatus.InvalidParameter;
			}
			CS$<>8__locals1.returnedResult = 0;
			this.ConvertMidsToActiveDirectory(new int[]
			{
				mid1,
				mid2
			}, out CS$<>8__locals1.outputMids);
			NspiStatus result2;
			using (NspiConnection connection = NspiConnectionPool.GetConnection(this.nspiServer, this.nspiPrincipal.OrganizationId.PartitionId))
			{
				result2 = this.ExecutePassThrough(nspiState, connection, () => connection.Client.CompareMids(CS$<>8__locals1.outputMids[0], CS$<>8__locals1.outputMids[1], out CS$<>8__locals1.returnedResult));
			}
			result = CS$<>8__locals1.returnedResult;
			return result2;
		}

		internal NspiStatus SeekEntries(NspiState nspiState, PropValue propValue, int[] mids, PropTag[] propTags, out PropRowSet rowset)
		{
			NspiContext.<>c__DisplayClass11 CS$<>8__locals1 = new NspiContext.<>c__DisplayClass11();
			CS$<>8__locals1.propValue = propValue;
			CS$<>8__locals1.mids = mids;
			CS$<>8__locals1.propTags = propTags;
			rowset = null;
			CS$<>8__locals1.returnedRowset = null;
			if (nspiState == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, NspiContext.nullStateMessage);
				return NspiStatus.InvalidParameter;
			}
			if (this.nspiPrincipal.DirectorySearchRoot != null)
			{
				NspiContext.NspiTracer.TraceDebug((long)this.contextHandle, "Open Domain User has No GAL to Seek");
				rowset = new PropRowSet(0);
				this.ResetStatBlockToZeroes(nspiState);
				return NspiStatus.Success;
			}
			NspiStatus nspiStatus;
			using (NspiConnection connection = NspiConnectionPool.GetConnection(this.nspiServer, this.nspiPrincipal.OrganizationId.PartitionId))
			{
				nspiStatus = this.ExecutePassThrough(nspiState, connection, () => connection.Client.SeekEntries(CS$<>8__locals1.propValue, CS$<>8__locals1.mids, CS$<>8__locals1.propTags, out CS$<>8__locals1.returnedRowset));
			}
			NspiPropMapper nspiPropMapper = new NspiPropMapper(this, null, nspiState.CodePage);
			nspiPropMapper.RewritePassThruProperties(CS$<>8__locals1.returnedRowset);
			rowset = CS$<>8__locals1.returnedRowset;
			if (nspiStatus == NspiStatus.InvalidBookmark && nspiState.ContainerId == 0)
			{
				rowset = new PropRowSet(0);
			}
			return nspiStatus;
		}

		internal NspiStatus ResortRestriction(NspiState nspiState, int[] inputMids, out int[] outputMids)
		{
			NspiContext.<>c__DisplayClass17 CS$<>8__locals1 = new NspiContext.<>c__DisplayClass17();
			outputMids = null;
			if (nspiState == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, NspiContext.nullStateMessage);
				return NspiStatus.InvalidParameter;
			}
			if (inputMids == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, "Null InputMids");
				return NspiStatus.InvalidParameter;
			}
			CS$<>8__locals1.returnedMids = null;
			this.ConvertMidsToActiveDirectory(inputMids, out CS$<>8__locals1.inputADMids);
			NspiStatus result;
			using (NspiConnection connection = NspiConnectionPool.GetConnection(this.nspiServer, this.nspiPrincipal.OrganizationId.PartitionId))
			{
				result = this.ExecutePassThrough(nspiState, connection, () => connection.Client.ResortRestriction(CS$<>8__locals1.inputADMids, out CS$<>8__locals1.returnedMids));
			}
			this.ConvertMidsToAddressBook(nspiState, CS$<>8__locals1.returnedMids, out outputMids);
			return result;
		}

		internal NspiStatus GetPropList(NspiGetPropListFlags flags, int mid, uint codePage, out IList<PropTag> propTagList)
		{
			propTagList = null;
			NspiContext.NspiTracer.TraceDebug<NspiGetPropListFlags>((long)this.contextHandle, "NspiRetrievePropertyFlags: {0}", flags);
			NspiState nspiState = new NspiState();
			nspiState.CurrentRecord = mid;
			PropRow propRow = null;
			NspiStatus nspiStatus = this.GetProps((NspiGetPropsFlags)flags, nspiState, null, out propRow);
			if (nspiStatus == NspiStatus.Success || nspiStatus == NspiStatus.ErrorsReturned)
			{
				nspiStatus = NspiStatus.Success;
				propTagList = new PropTag[propRow.Properties.Count];
				for (int i = 0; i < propRow.Properties.Count; i++)
				{
					propTagList[i] = propRow.Properties[i].PropTag;
				}
			}
			return nspiStatus;
		}

		internal NspiStatus ModProps(NspiState nspiState, PropTag[] propertiesToDelete, PropRow row)
		{
			if (nspiState == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, NspiContext.nullStateMessage);
				return NspiStatus.InvalidParameter;
			}
			byte[][] array = NspiContext.emptyByteArrayArray;
			byte[][] array2 = NspiContext.emptyByteArrayArray;
			this.sortLocale = nspiState.SortLocale;
			if (propertiesToDelete == null)
			{
				NspiContext.NspiTracer.TraceError((long)this.contextHandle, "Properties to delete parameter is null");
				return NspiStatus.InvalidParameter;
			}
			Guid guid;
			EphemeralIdTable.NamingContext namingContext;
			if (!this.ephemeralIdTable.GetGuid(nspiState.CurrentRecord, out guid, out namingContext))
			{
				NspiContext.NspiTracer.TraceError<int>((long)this.contextHandle, "Failed to find guid for mid: {0}", nspiState.CurrentRecord);
				return NspiStatus.InvalidParameter;
			}
			ADObjectId adobjectId = new ADObjectId(guid);
			NspiContext.NspiTracer.TraceDebug<int, ADObjectId>((long)this.contextHandle, "target mid: {0} (ADObjectId: {1})", nspiState.CurrentRecord, adobjectId);
			foreach (PropTag propTag in propertiesToDelete)
			{
				NspiContext.NspiTracer.TraceDebug<PropTag>((long)this.contextHandle, "Delete: 0x{0:X}", propTag);
				if (propTag == PropTag.UserSMimeCertificate)
				{
					array = null;
				}
				else
				{
					if (propTag != (PropTag)2355761410U)
					{
						this.protocolLogSession[ProtocolLog.Field.Failures] = "Delete:" + NspiContext.Hex(propTag);
						NspiContext.NspiTracer.TraceError<PropTag>((long)this.contextHandle, "Unsupported propTag to delete: 0x{0:X}", propTag);
						return NspiStatus.AccessDenied;
					}
					array2 = null;
				}
			}
			if (row != null)
			{
				foreach (PropValue propValue in row.Properties)
				{
					NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "Set: 0x{0:X}", (int)propValue.PropTag);
					if (propValue.PropTag == PropTag.UserSMimeCertificate)
					{
						array = (propValue.Value as byte[][]);
						if (array == null)
						{
							this.protocolLogSession[ProtocolLog.Field.Failures] = "NullUserSMimeCertificate";
							NspiContext.NspiTracer.TraceError((long)this.contextHandle, "Input smimeCertificate value is null");
							return NspiStatus.AccessDenied;
						}
					}
					else
					{
						if (propValue.PropTag != (PropTag)2355761410U)
						{
							this.protocolLogSession[ProtocolLog.Field.Failures] = "Set:" + NspiContext.Hex(propValue.PropTag);
							NspiContext.NspiTracer.TraceError<PropTag>((long)this.contextHandle, "Unsupported proptag to set: 0x{0:X}", propValue.PropTag);
							return NspiStatus.AccessDenied;
						}
						array2 = (propValue.Value as byte[][]);
						if (array2 == null)
						{
							this.protocolLogSession[ProtocolLog.Field.Failures] = "NullCertificate";
							NspiContext.NspiTracer.TraceError((long)this.contextHandle, "Input certificate value is null");
							return NspiStatus.AccessDenied;
						}
					}
				}
			}
			if (array == NspiContext.emptyByteArrayArray && array2 == NspiContext.emptyByteArrayArray)
			{
				return NspiStatus.Success;
			}
			using (RunspaceProxy runspaceProxy = this.CreateRunspaceProxy())
			{
				if (runspaceProxy == null)
				{
					return NspiStatus.AccessDenied;
				}
				PSCommand pscommand = new PSCommand().AddCommand("set-mailbox");
				pscommand.AddParameter("identity", adobjectId);
				if (array != NspiContext.emptyByteArrayArray)
				{
					pscommand.AddParameter("usersmimecertificate", array);
				}
				if (array2 != NspiContext.emptyByteArrayArray)
				{
					pscommand.AddParameter("usercertificate", array2);
				}
				Collection<PSObject> collection;
				Collection<Exception> collection2;
				if (!this.RunPowerShellCommand(runspaceProxy, pscommand, out collection, out collection2))
				{
					this.protocolLogSession[ProtocolLog.Field.Failures] = "ExecuteErrors";
					if (collection2 != null)
					{
						foreach (Exception ex in collection2)
						{
							ProtocolLogSession protocolLogSession;
							(protocolLogSession = this.protocolLogSession)[ProtocolLog.Field.Failures] = protocolLogSession[ProtocolLog.Field.Failures] + ":" + ex.Message;
						}
					}
					return NspiStatus.AccessDenied;
				}
			}
			return NspiStatus.Success;
		}

		internal NspiStatus ModLinkAtt(NspiModLinkAttFlags flags, PropTag propTag, int mid, EntryId[] entryIds)
		{
			NspiContext.NspiTracer.TraceDebug<NspiModLinkAttFlags, int>((long)this.contextHandle, "NspiModLinkAttFlags: {0}, propTag: 0x{1:X8}", flags, (int)propTag);
			int num = propTag.Id();
			if (num != ((PropTag)2148073485U).Id() && num != ((PropTag)2148859917U).Id())
			{
				this.protocolLogSession[ProtocolLog.Field.Failures] = "Unsupported:" + NspiContext.Hex(propTag);
				NspiContext.NspiTracer.TraceError<PropTag>((long)this.contextHandle, "Unsupported proptag to write: 0x{0:X}", propTag);
				return NspiStatus.NotFound;
			}
			PropType propType = propTag.ValueType();
			if (propType != PropType.Object && propType != PropType.AnsiStringArray && propType != PropType.StringArray)
			{
				this.protocolLogSession[ProtocolLog.Field.Failures] = "Unsupported:" + NspiContext.Hex(propTag);
				NspiContext.NspiTracer.TraceError<PropTag>((long)this.contextHandle, "Unsupported proptag to write: 0x{0:X}", propTag);
				return NspiStatus.NotFound;
			}
			Guid guid;
			EphemeralIdTable.NamingContext namingContext;
			if (!this.ephemeralIdTable.GetGuid(mid, out guid, out namingContext))
			{
				NspiContext.NspiTracer.TraceError<int>((long)this.contextHandle, "Failed to find guid for mid: {0}", mid);
				return NspiStatus.InvalidParameter;
			}
			ADObjectId adobjectId = new ADObjectId(guid);
			NspiContext.NspiTracer.TraceDebug<int, ADObjectId>((long)this.contextHandle, "target mid: {0} (ADObjectId: {1})", mid, adobjectId);
			bool flag = (flags & NspiModLinkAttFlags.Delete) == NspiModLinkAttFlags.Delete;
			if (entryIds == null || entryIds.Length == 0)
			{
				return NspiStatus.Success;
			}
			ADObjectId[] array;
			if (!this.TryResolveEntryIds(entryIds, out array))
			{
				this.protocolLogSession[ProtocolLog.Field.Failures] = "NotFound";
				return NspiStatus.AccessDenied;
			}
			using (RunspaceProxy runspaceProxy = this.CreateRunspaceProxy())
			{
				if (runspaceProxy == null)
				{
					return NspiStatus.AccessDenied;
				}
				if (propTag.Id() == ((PropTag)2148073485U).Id())
				{
					foreach (ADObjectId value in array)
					{
						PSCommand pscommand = new PSCommand().AddCommand(flag ? "remove-distributiongroupmember" : "add-distributiongroupmember");
						pscommand.AddParameter("identity", adobjectId);
						pscommand.AddParameter("member", value);
						pscommand.AddParameter("confirm", false);
						Collection<PSObject> collection;
						Collection<Exception> collection2;
						if (!this.RunPowerShellCommand(runspaceProxy, pscommand, out collection, out collection2))
						{
							if (collection2 == null || collection2.Count == 0)
							{
								this.protocolLogSession[ProtocolLog.Field.Failures] = "ExecuteError";
								return NspiStatus.AccessDenied;
							}
							foreach (Exception ex in collection2)
							{
								if (!(ex is MemberAlreadyExistsException) && !(ex is MemberNotFoundException))
								{
									this.protocolLogSession[ProtocolLog.Field.Failures] = "ExecuteError:" + ex.Message;
									return NspiStatus.AccessDenied;
								}
							}
						}
					}
				}
				else
				{
					if (propTag.Id() != ((PropTag)2148859917U).Id())
					{
						this.protocolLogSession[ProtocolLog.Field.Failures] = "Unsupported:" + NspiContext.Hex(propTag);
						NspiContext.NspiTracer.TraceError<PropTag>((long)this.contextHandle, "Unsupported proptag to write: 0x{0:X}", propTag);
						return NspiStatus.NotFound;
					}
					PSCommand pscommand2 = new PSCommand().AddCommand("get-mailbox");
					pscommand2.AddParameter("identity", adobjectId);
					pscommand2.AddParameter("resultSize", 2);
					Collection<PSObject> collection3;
					Collection<Exception> collection4;
					if (!this.RunPowerShellCommand(runspaceProxy, pscommand2, out collection3, out collection4))
					{
						this.protocolLogSession[ProtocolLog.Field.Failures] = "GetMailbox";
						if (collection4 != null)
						{
							foreach (Exception ex2 in collection4)
							{
								ProtocolLogSession protocolLogSession;
								(protocolLogSession = this.protocolLogSession)[ProtocolLog.Field.Failures] = protocolLogSession[ProtocolLog.Field.Failures] + ":" + ex2.Message;
							}
						}
						return NspiStatus.AccessDenied;
					}
					if (collection3.Count != 1)
					{
						NspiContext.NspiTracer.TraceError<int, ADObjectId>((long)this.contextHandle, "Found {0} mailbox objects for id '{1}', expected 1.", collection3.Count, adobjectId);
						this.protocolLogSession[ProtocolLog.Field.Failures] = string.Format("BadResults:{0}:{1}", collection3.Count, adobjectId);
						return NspiStatus.AccessDenied;
					}
					Mailbox mailbox = collection3[0].BaseObject as Mailbox;
					if (mailbox == null)
					{
						NspiContext.NspiTracer.TraceError<ADObjectId>((long)this.contextHandle, "Target object '{0}' can't be cast to Mailbox", adobjectId);
						this.protocolLogSession[ProtocolLog.Field.Failures] = string.Format("NotMailbox:{0}", adobjectId);
						return NspiStatus.AccessDenied;
					}
					MultiValuedProperty<ADObjectId> grantSendOnBehalfTo = mailbox.GrantSendOnBehalfTo;
					foreach (ADObjectId item in array)
					{
						if (flag)
						{
							if (mailbox.GrantSendOnBehalfTo.Contains(item))
							{
								mailbox.GrantSendOnBehalfTo.Remove(item);
							}
						}
						else if (!mailbox.GrantSendOnBehalfTo.Contains(item))
						{
							mailbox.GrantSendOnBehalfTo.Add(item);
						}
					}
					pscommand2 = new PSCommand().AddCommand("set-mailbox");
					pscommand2.AddParameter("identity", adobjectId);
					pscommand2.AddParameter("grantSendOnBehalfTo", grantSendOnBehalfTo);
					if (!this.RunPowerShellCommand(runspaceProxy, pscommand2, out collection3, out collection4))
					{
						this.protocolLogSession[ProtocolLog.Field.Failures] = "SetMailbox";
						if (collection4 != null)
						{
							foreach (Exception ex3 in collection4)
							{
								ProtocolLogSession protocolLogSession2;
								(protocolLogSession2 = this.protocolLogSession)[ProtocolLog.Field.Failures] = protocolLogSession2[ProtocolLog.Field.Failures] + ":" + ex3.Message;
							}
						}
						return NspiStatus.AccessDenied;
					}
				}
				if (this.modCache == null)
				{
					this.modCache = new ModCache(this, Configuration.ModCacheExpiryTimeInSeconds);
				}
				List<int> list = new List<int>(array.Length);
				foreach (ADObjectId adobjectId2 in array)
				{
					list.Add(this.ephemeralIdTable.CreateEphemeralId(adobjectId2.ObjectGuid, EphemeralIdTable.GetNamingContext(adobjectId2)));
				}
				this.modCache.RecordMods(mid, propTag, list, flag);
			}
			return NspiStatus.Success;
		}

		internal NspiStatus QueryColumns(NspiQueryColumnsMapiFlags flags, out IList<PropTag> propTags)
		{
			if ((flags & NspiQueryColumnsMapiFlags.Unicode) == NspiQueryColumnsMapiFlags.Unicode)
			{
				propTags = NspiPropMapper.SupportedPropTagsUnicode;
			}
			else
			{
				propTags = NspiPropMapper.SupportedPropTagsAnsi;
			}
			return NspiStatus.Success;
		}

		internal void PurgeExpiredLogs()
		{
			if (this.modCache != null && this.modCache.PurgeExpiredLogs())
			{
				this.modCache = null;
			}
		}

		internal IDirectorySession GetADSessionFromNamingContext(EphemeralIdTable.NamingContext nc)
		{
			switch (nc)
			{
			case EphemeralIdTable.NamingContext.Domain:
				return this.GetRecipientSession(null);
			case EphemeralIdTable.NamingContext.Config:
				return this.GetRootOrgSystemConfigurationSession();
			case EphemeralIdTable.NamingContext.TenantDomain:
				return this.GetTenantSystemConfigurationSession(false);
			case EphemeralIdTable.NamingContext.TenantConfig:
				return this.GetTenantSystemConfigurationSession(true);
			default:
				throw new NotSupportedException("Unsupported NamingContext");
			}
		}

		internal ITenantConfigurationSession GetTenantSystemConfigurationSession()
		{
			return this.GetTenantSystemConfigurationSession(true);
		}

		internal ITenantConfigurationSession GetTenantSystemConfigurationSession(bool useConfigNC)
		{
			if (this.nspiPrincipal.ConfigurationUnit == null)
			{
				string value = this.nspiPrincipal.LegacyDistinguishedName + " ConfigurationUnit is null";
				this.protocolLogSession[ProtocolLog.Field.Failures] = value;
				return null;
			}
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromTenantCUName(this.nspiPrincipal.ConfigurationUnit.Parent.Name), 2316, "GetTenantSystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\DoMT\\src\\Service\\NspiContext.cs");
			tenantConfigurationSession.ServerTimeout = Configuration.ADTimeout;
			tenantConfigurationSession.SessionSettings.AccountingObject = this.Budget;
			tenantConfigurationSession.UseConfigNC = useConfigNC;
			return tenantConfigurationSession;
		}

		internal ITopologyConfigurationSession GetRootOrgSystemConfigurationSession()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 2332, "GetRootOrgSystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\DoMT\\src\\Service\\NspiContext.cs");
			topologyConfigurationSession.ServerTimeout = Configuration.ADTimeout;
			topologyConfigurationSession.SessionSettings.AccountingObject = this.Budget;
			return topologyConfigurationSession;
		}

		internal IRecipientSession GetRecipientSession(ADObjectId addressListScope)
		{
			ADSessionSettings sessionSettings;
			if (addressListScope == null)
			{
				sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.nspiPrincipal.OrganizationId);
			}
			else
			{
				sessionSettings = ADSessionSettings.FromOrganizationIdWithAddressListScopeServiceOnly(this.nspiPrincipal.OrganizationId, addressListScope);
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, this.nspiPrincipal.DirectorySearchRoot, this.sortLocale, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 2357, "GetRecipientSession", "f:\\15.00.1497\\sources\\dev\\DoMT\\src\\Service\\NspiContext.cs");
			tenantOrRootOrgRecipientSession.ServerTimeout = Configuration.ADTimeout;
			tenantOrRootOrgRecipientSession.SessionSettings.AccountingObject = this.Budget;
			if (this.nspiPrincipal.DirectorySearchRoot != null)
			{
				tenantOrRootOrgRecipientSession.EnforceContainerizedScoping = false;
			}
			return tenantOrRootOrgRecipientSession;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
				if (this.clientSecurityContext != null)
				{
					this.clientSecurityContext.Dispose();
					this.clientSecurityContext = null;
				}
				if (this.budget != null)
				{
					this.budget.Dispose();
					this.budget = null;
				}
				if (this.scope != null)
				{
					this.scope.End();
					this.scope = null;
				}
			}
		}

		private IConfigurationSession GetTenantLocalSystemConfigurationSession()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.nspiPrincipal.OrganizationId), 2425, "GetTenantLocalSystemConfigurationSession", "f:\\15.00.1497\\sources\\dev\\DoMT\\src\\Service\\NspiContext.cs");
			tenantOrTopologyConfigurationSession.ServerTimeout = Configuration.ADTimeout;
			tenantOrTopologyConfigurationSession.SessionSettings.AccountingObject = this.Budget;
			return tenantOrTopologyConfigurationSession;
		}

		private void InitializeNamingContext()
		{
			if (NspiContext.configNamingContext == null)
			{
				ITopologyConfigurationSession rootOrgSystemConfigurationSession = this.GetRootOrgSystemConfigurationSession();
				ConfigurationContainer configurationContainer = rootOrgSystemConfigurationSession.Read<ConfigurationContainer>(rootOrgSystemConfigurationSession.ConfigurationNamingContext);
				if (configurationContainer != null)
				{
					Guid guid = (Guid)configurationContainer[ADObjectSchema.Guid];
					if (guid != Guid.Empty)
					{
						NspiContext.configNamingContext = new ADObjectId(rootOrgSystemConfigurationSession.ConfigurationNamingContext.DistinguishedName, guid);
					}
				}
			}
		}

		private NspiStatus GetOneOffTable(NspiGetHierarchyInfoFlags flags, NspiState state, ref uint version, out PropRowSet rowset)
		{
			PropTag propTag = (state.CodePage == Encoding.Unicode.CodePage) ? PropTag.DisplayName : PropTag.DisplayNameAnsi;
			IEnumerable<AddressTemplate> addressTemplates = this.GetAddressTemplates(state.TemplateLocale);
			rowset = new PropRowSet();
			byte[] array = new byte[4];
			foreach (AddressTemplate addressTemplate in addressTemplates)
			{
				EntryId entryId = new EntryId(EntryId.DisplayType.AbAddressTemplate, LegacyDN.FormatTemplateGuid(addressTemplate.Id.ObjectGuid));
				ExBitConverter.Write(this.ephemeralIdTable.CreateEphemeralId(addressTemplate.Id.ObjectGuid, EphemeralIdTable.GetNamingContext(addressTemplate.Id)), array, 0);
				PropRow propRow = new PropRow(7);
				propRow.Add(new PropValue(propTag, addressTemplate.DisplayName));
				propRow.Add(new PropValue(PropTag.AddrTypeAnsi, addressTemplate.AddressType));
				propRow.Add(new PropValue(PropTag.DisplayType, EntryId.DisplayType.MailUser));
				propRow.Add(new PropValue(PropTag.Depth, 0));
				propRow.Add(new PropValue(PropTag.Selectable, true));
				propRow.Add(new PropValue(PropTag.InstanceKey, array));
				propRow.Add(new PropValue(PropTag.EntryId, entryId.ToByteArray()));
				rowset.Add(propRow);
			}
			return NspiStatus.Success;
		}

		private NspiStatus GetHierarchyTable(NspiGetHierarchyInfoFlags flags, NspiState state, ref uint version, out PropRowSet rowset)
		{
			PropTag propTag;
			NspiPropMapper nspiPropMapper;
			if ((flags & NspiGetHierarchyInfoFlags.Unicode) == NspiGetHierarchyInfoFlags.Unicode)
			{
				propTag = PropTag.DisplayName;
				nspiPropMapper = null;
			}
			else
			{
				propTag = PropTag.DisplayNameAnsi;
				nspiPropMapper = new NspiPropMapper(this, null, state.CodePage);
			}
			rowset = new PropRowSet(10);
			PropRow propRow = new PropRow(6);
			propRow.Add(new PropValue(PropTag.EntryId, EntryId.DefaultGalEntryId.ToByteArray()));
			propRow.Add(new PropValue(PropTag.ContainerFlags, ContainerFlags.Recipients | ContainerFlags.Unmodifiable));
			propRow.Add(new PropValue(PropTag.Depth, 0));
			propRow.Add(new PropValue((PropTag)4294770691U, 0));
			propRow.Add(new PropValue(propTag, null));
			propRow.Add(new PropValue((PropTag)4294639627U, false));
			rowset.Add(propRow);
			if (this.nspiPrincipal.DirectorySearchRoot == null)
			{
				IConfigurationSession tenantLocalSystemConfigurationSession = this.GetTenantLocalSystemConfigurationSession();
				Organization orgContainer = tenantLocalSystemConfigurationSession.GetOrgContainer();
				MultiValuedProperty<ADObjectId> resourceAddressLists = orgContainer.ResourceAddressLists;
				CultureInfo cultureInfo = this.GetCultureInfo(state.SortLocale);
				IList<AddressBookBase> allAddressListsByHierarchy = AddressBookBase.GetAllAddressListsByHierarchy(this.clientSecurityContext, tenantLocalSystemConfigurationSession, this.nspiPrincipal.AddressBookPolicy, cultureInfo);
				Dictionary<string, EntryId> dictionary = new Dictionary<string, EntryId>(allAddressListsByHierarchy.Count);
				HashSet<string> hashSet = new HashSet<string>();
				foreach (AddressBookBase addressBookBase in allAddressListsByHierarchy)
				{
					dictionary[addressBookBase.DistinguishedName] = new EntryId(EntryId.DisplayType.AbContainer, LegacyDN.FormatAddressListDN(addressBookBase.Id.ObjectGuid));
					hashSet.Add(addressBookBase.Id.Parent.DistinguishedName);
				}
				foreach (AddressBookBase addressBookBase2 in allAddressListsByHierarchy)
				{
					if (!addressBookBase2.IsModernGroupsAddressList)
					{
						ContainerFlags containerFlags = ContainerFlags.Recipients | ContainerFlags.Unmodifiable;
						if (hashSet.Contains(addressBookBase2.Id.DistinguishedName))
						{
							containerFlags |= ContainerFlags.Subcontainers;
						}
						foreach (ADObjectId adobjectId in resourceAddressLists)
						{
							if (adobjectId.Equals(addressBookBase2.Id))
							{
								containerFlags |= ContainerFlags.ConfRooms;
								break;
							}
						}
						int num = this.ephemeralIdTable.CreateEphemeralId(addressBookBase2.Id.ObjectGuid, EphemeralIdTable.GetNamingContext(addressBookBase2.Id));
						propRow = new PropRow(7);
						propRow.Add(new PropValue(PropTag.EntryId, dictionary[addressBookBase2.DistinguishedName].ToByteArray()));
						propRow.Add(new PropValue(PropTag.ContainerFlags, containerFlags));
						propRow.Add(new PropValue(PropTag.Depth, addressBookBase2.Depth));
						propRow.Add(new PropValue((PropTag)4294770691U, num));
						if (nspiPropMapper != null)
						{
							propRow.Add(new PropValue(propTag, nspiPropMapper.ConvertStringWithSubstitions(propTag, addressBookBase2.DisplayName)));
						}
						else
						{
							propRow.Add(new PropValue(propTag, addressBookBase2.DisplayName));
						}
						propRow.Add(new PropValue((PropTag)4294639627U, false));
						EntryId entryId;
						if (dictionary.TryGetValue(addressBookBase2.Id.Parent.DistinguishedName, out entryId))
						{
							propRow.Add(new PropValue((PropTag)4294705410U, entryId.ToByteArray()));
						}
						rowset.Add(propRow);
					}
				}
			}
			version = (uint)rowset.GetHashCode();
			return NspiStatus.Success;
		}

		private NspiStatus QueryRowsVlv(NspiQueryRowsFlags flags, NspiState nspiState, int count, NspiPropMapper propMapper, out PropRowSet rowset)
		{
			NspiContext.<>c__DisplayClass1d CS$<>8__locals1 = new NspiContext.<>c__DisplayClass1d();
			CS$<>8__locals1.flags = flags;
			CS$<>8__locals1.count = count;
			CS$<>8__locals1.propMapper = propMapper;
			CS$<>8__locals1.returnedRowset = null;
			this.protocolLogSession[ProtocolLog.Field.OperationSpecific] = "Browse";
			ADObjectId addressListScope = this.GetAddressListScope(nspiState.ContainerId);
			if (addressListScope == null)
			{
				NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "Failed to get address list scope. ContainerId: {0}", nspiState.ContainerId);
				rowset = null;
				return NspiStatus.InvalidBookmark;
			}
			if (addressListScope.Equals(NspiContext.NoGalObjectID) || this.nspiPrincipal.DirectorySearchRoot != null)
			{
				NspiContext.NspiTracer.TraceDebug((long)this.contextHandle, "User has No GAL to Browse");
				rowset = new PropRowSet(0);
				this.ResetStatBlockToZeroes(nspiState);
				return NspiStatus.Success;
			}
			NspiStatus result;
			using (NspiConnection connection = NspiConnectionPool.GetConnection(this.nspiServer, this.nspiPrincipal.OrganizationId.PartitionId))
			{
				result = this.ExecutePassThrough(nspiState, connection, () => connection.Client.QueryRows(CS$<>8__locals1.flags, null, CS$<>8__locals1.count, CS$<>8__locals1.propMapper.RequestedPropTags, out CS$<>8__locals1.returnedRowset));
			}
			CS$<>8__locals1.propMapper.RewritePassThruProperties(CS$<>8__locals1.returnedRowset);
			rowset = CS$<>8__locals1.returnedRowset;
			return result;
		}

		private void ResetStatBlockToZeroes(NspiState nspiState)
		{
			nspiState.TotalRecords = 0;
			nspiState.Position = 0;
		}

		private int GetAddressListActiveDirectoryMid(int containerId)
		{
			containerId = this.GetAddressListMid(containerId);
			if (containerId == -1)
			{
				return -1;
			}
			if (containerId == -2)
			{
				return -2;
			}
			int[] array;
			this.ConvertMidsToActiveDirectory(new int[]
			{
				containerId
			}, out array);
			return array[0];
		}

		private void ConvertMidsToActiveDirectory(int[] inputMids, out int[] outputMids)
		{
			NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "Mids to convert to active directory: {0}", inputMids.Length);
			outputMids = new int[inputMids.Length];
			List<string> list = new List<string>(inputMids.Length);
			List<int> list2 = new List<int>(inputMids.Length);
			for (int i = 0; i < inputMids.Length; i++)
			{
				if (this.ephemeralIdTable.IsAddressBookEphemeralId(inputMids[i]))
				{
					Guid arg;
					EphemeralIdTable.NamingContext namingContext;
					if (!this.ephemeralIdTable.TryGetMapping(inputMids[i], out outputMids[i]) && this.ephemeralIdTable.GetGuid(inputMids[i], out arg, out namingContext))
					{
						NspiContext.NspiTracer.TraceDebug<int, int, Guid>((long)this.contextHandle, "Mid {0}: {1} Will convert {2}", i, inputMids[i], arg);
						list.Add(LegacyDN.FormatLegacyDnFromGuid(Guid.Empty, arg));
						list2.Add(i);
					}
				}
				else
				{
					outputMids[i] = inputMids[i];
				}
			}
			NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "Mids left to convert: {0}", list.Count);
			if (list.Count == 0)
			{
				return;
			}
			using (NspiConnection connection = NspiConnectionPool.GetConnection(this.nspiServer, this.nspiPrincipal.OrganizationId.PartitionId))
			{
				int[] array;
				if (connection.Client.DnToEph(list.ToArray(), out array) == NspiStatus.Success)
				{
					for (int j = 0; j < list.Count; j++)
					{
						NspiContext.NspiTracer.TraceDebug<int, int, int>((long)this.contextHandle, "Mid {0}: {1} {2}", list2[j], inputMids[list2[j]], array[j]);
						outputMids[list2[j]] = array[j];
						this.ephemeralIdTable.AddIdMapping(inputMids[list2[j]], array[j]);
					}
					this.nspiServer = connection.Server;
					connection.ReturnToPool();
				}
			}
		}

		private void ConvertMidsToAddressBook(NspiState nspiState, int[] inputMids, out int[] outputMids)
		{
			NspiContext.<>c__DisplayClass23 CS$<>8__locals1 = new NspiContext.<>c__DisplayClass23();
			NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "Mids to convert to address book: {0}", inputMids.Length);
			outputMids = new int[inputMids.Length];
			CS$<>8__locals1.addressBookMids = new List<int>(inputMids.Length);
			List<int> list = new List<int>(inputMids.Length);
			for (int i = 0; i < inputMids.Length; i++)
			{
				if (EphemeralIdTable.IsActiveDirectoryEphemeralId(inputMids[i]))
				{
					if (!this.ephemeralIdTable.TryGetMapping(inputMids[i], out outputMids[i]))
					{
						NspiContext.NspiTracer.TraceDebug<int, int>((long)this.contextHandle, "Mid {0}: {1} (Will convert)", i, inputMids[i]);
						CS$<>8__locals1.addressBookMids.Add(inputMids[i]);
						list.Add(i);
					}
				}
				else
				{
					outputMids[i] = inputMids[i];
				}
			}
			NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "Mids left to convert: {0}", CS$<>8__locals1.addressBookMids.Count);
			if (CS$<>8__locals1.addressBookMids.Count == 0)
			{
				return;
			}
			CS$<>8__locals1.rowset = null;
			NspiStatus nspiStatus;
			using (NspiConnection connection = NspiConnectionPool.GetConnection(this.nspiServer, this.nspiPrincipal.OrganizationId.PartitionId))
			{
				nspiStatus = this.ExecutePassThrough(nspiState.Clone(), connection, () => connection.Client.QueryRows(NspiQueryRowsFlags.None, CS$<>8__locals1.addressBookMids.ToArray(), CS$<>8__locals1.addressBookMids.Count, NspiContext.objectGuidPropTags, out CS$<>8__locals1.rowset));
			}
			if (nspiStatus == NspiStatus.InvalidBookmark && nspiState.ContainerId == 0)
			{
				CS$<>8__locals1.rowset = new PropRowSet(0);
				nspiStatus = NspiStatus.Success;
			}
			if (nspiStatus == NspiStatus.Success)
			{
				for (int j = 0; j < CS$<>8__locals1.rowset.Rows.Count; j++)
				{
					if (CS$<>8__locals1.rowset.Rows[j].Properties[0].PropTag == (PropTag)2355953922U)
					{
						Guid guid = new Guid(CS$<>8__locals1.rowset.Rows[j].Properties[0].GetBytes());
						int num = this.ephemeralIdTable.CreateEphemeralId(guid, EphemeralIdTable.NamingContext.Domain);
						NspiContext.NspiTracer.TraceDebug<int, int, int>((long)this.contextHandle, "Mid {0}: {1} {2}", list[j], inputMids[list[j]], num);
						outputMids[list[j]] = num;
						this.ephemeralIdTable.AddIdMapping(inputMids[list[j]], num);
					}
					else
					{
						NspiContext.NspiTracer.TraceDebug<int, int>((long)this.contextHandle, "Mid {0}: {1} (NOT FOUND)", list[j], inputMids[list[j]]);
					}
				}
			}
		}

		private int GetAddressListMid(int addressListMid)
		{
			if (addressListMid == 0)
			{
				if (this.galEphemeralId == 0)
				{
					AddressBookBase globalAddressList = AddressBookBase.GetGlobalAddressList(this.clientSecurityContext, this.GetTenantLocalSystemConfigurationSession(), this.GetRecipientSession(null), this.nspiPrincipal.GlobalAddressListFromAddressBookPolicy);
					if (globalAddressList == null)
					{
						NspiContext.NspiTracer.TraceDebug<SecurityIdentifier>((long)this.contextHandle, "Failed to find a GAL for user {0}", this.clientSecurityContext.UserSid);
						this.galEphemeralId = -2;
					}
					else
					{
						this.galEphemeralId = this.ephemeralIdTable.CreateEphemeralId(globalAddressList.Guid, EphemeralIdTable.GetNamingContext(globalAddressList.Id));
						NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "Caching GAL MID: {0}", this.galEphemeralId);
					}
				}
				return this.galEphemeralId;
			}
			if (!this.ephemeralIdTable.IsAddressBookEphemeralId(addressListMid))
			{
				return -1;
			}
			if (this.addressListPermissionsCache == null)
			{
				this.addressListPermissionsCache = new Dictionary<int, bool>();
			}
			bool flag;
			if (this.addressListPermissionsCache.TryGetValue(addressListMid, out flag))
			{
				if (!flag)
				{
					return -1;
				}
				return addressListMid;
			}
			else
			{
				flag = false;
				Guid guid;
				EphemeralIdTable.NamingContext namingContext;
				if (this.ephemeralIdTable.GetGuid(addressListMid, out guid, out namingContext) && (namingContext == EphemeralIdTable.NamingContext.Config || namingContext == EphemeralIdTable.NamingContext.TenantConfig))
				{
					IConfigurationSession tenantLocalSystemConfigurationSession = this.GetTenantLocalSystemConfigurationSession();
					ADObjectId entryId = new ADObjectId(guid);
					AddressBookBase addressBookBase = tenantLocalSystemConfigurationSession.Read<AddressBookBase>(entryId);
					if (addressBookBase != null)
					{
						flag = addressBookBase.CanOpenAddressList(this.clientSecurityContext);
					}
				}
				this.addressListPermissionsCache.Add(addressListMid, flag);
				if (!flag)
				{
					return -1;
				}
				return addressListMid;
			}
		}

		private ADObjectId GetAddressListScope(int addressListMid)
		{
			addressListMid = this.GetAddressListMid(addressListMid);
			if (addressListMid == -1)
			{
				return null;
			}
			if (addressListMid == -2)
			{
				return NspiContext.NoGalObjectID;
			}
			Guid arg;
			EphemeralIdTable.NamingContext namingContext;
			if (!this.ephemeralIdTable.GetGuid(addressListMid, out arg, out namingContext))
			{
				NspiContext.NspiTracer.TraceError<int>((long)this.contextHandle, "Couldn't find guid for address list mid: {0}", addressListMid);
				return null;
			}
			NspiContext.NspiTracer.TraceDebug<int, Guid>((long)this.contextHandle, "Found guid: {1} for address list mid: {0}", addressListMid, arg);
			return new ADObjectId(arg);
		}

		private IEnumerable<AddressTemplate> GetAddressTemplates(int codepage)
		{
			ITopologyConfigurationSession rootOrgSystemConfigurationSession = this.GetRootOrgSystemConfigurationSession();
			ADObjectId descendantId = rootOrgSystemConfigurationSession.GetOrgContainerId().GetDescendantId(AddressTemplate.ContainerId);
			string unescapedCommonName = codepage.ToString("x", CultureInfo.InvariantCulture);
			ADObjectId childId = descendantId.GetChildId(unescapedCommonName);
			return rootOrgSystemConfigurationSession.FindPaged<AddressTemplate>(childId, QueryScope.OneLevel, null, null, 0);
		}

		private CultureInfo GetCultureInfo(int lcid)
		{
			if (lcid != 0)
			{
				try
				{
					return CultureInfo.GetCultureInfo(lcid);
				}
				catch (ArgumentException ex)
				{
					NspiContext.NspiTracer.TraceError((long)this.contextHandle, ex.Message);
				}
			}
			if (this.nspiPrincipal.PreferredCulture != null)
			{
				return this.nspiPrincipal.PreferredCulture;
			}
			return Thread.CurrentThread.CurrentCulture;
		}

		private SortBy GetSortOrder(NspiState state)
		{
			SortIndex sortIndex = state.SortIndex;
			if (sortIndex == SortIndex.DisplayName)
			{
				return NspiContext.SortByDisplayName;
			}
			if (sortIndex == SortIndex.PhoneticDisplayName)
			{
				return NspiContext.SortByPhoneticDisplayName;
			}
			switch (sortIndex)
			{
			case SortIndex.DisplayNameReadOnly:
			case SortIndex.DisplayNameWritable:
				return null;
			default:
				return null;
			}
		}

		private NspiStatus ExecutePassThrough(NspiState nspiState, NspiConnection connection, Func<NspiStatus> passThroughFunction)
		{
			if (connection == null)
			{
				return NspiStatus.GeneralFailure;
			}
			this.nspiServer = connection.Server;
			connection.Client.Stat.SortType = nspiState.SortType;
			connection.Client.Stat.CurrentRecord = nspiState.CurrentRecord;
			connection.Client.Stat.Delta = nspiState.Delta;
			connection.Client.Stat.Position = nspiState.Position;
			connection.Client.Stat.TotalRecords = nspiState.TotalRecords;
			connection.Client.Stat.CodePage = nspiState.CodePage;
			connection.Client.Stat.TemplateLocale = nspiState.TemplateLocale;
			connection.Client.Stat.SortLocale = nspiState.SortLocale;
			AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.NspiBrowseRequests.Increment();
			AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.NspiBrowseRequestsRate.Increment();
			AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.NspiBrowseRequestsTotal.Increment();
			Stopwatch stopwatch = Stopwatch.StartNew();
			NspiStatus nspiStatus;
			try
			{
				int addressListActiveDirectoryMid = this.GetAddressListActiveDirectoryMid(nspiState.ContainerId);
				if (addressListActiveDirectoryMid == -1)
				{
					NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "Failed to get AD MID for address list. ContainerId: {0}", nspiState.ContainerId);
					return NspiStatus.InvalidBookmark;
				}
				if (addressListActiveDirectoryMid == -2)
				{
					NspiContext.NspiTracer.TraceDebug((long)this.contextHandle, "User with No available GAL.");
					return NspiStatus.InvalidBookmark;
				}
				connection.Client.Stat.ContainerId = addressListActiveDirectoryMid;
				nspiStatus = passThroughFunction();
			}
			catch (RpcException ex)
			{
				NspiContext.NspiTracer.TraceError<string>((long)this.contextHandle, "RpcException performing pass-through: {0}", ex.Message);
				throw new ADTransientException(DirectoryStrings.ExceptionServerUnavailable(connection.Server), ex);
			}
			finally
			{
				stopwatch.Stop();
				int num = (int)stopwatch.ElapsedMilliseconds;
				if (AddressBookService.NspiRpcBrowseRequestsAverageLatency != null)
				{
					AddressBookService.NspiRpcBrowseRequestsAverageLatency.AddSample((long)num);
				}
				AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.NspiBrowseRequests.Decrement();
			}
			if (nspiStatus == NspiStatus.Success)
			{
				nspiState.CurrentRecord = connection.Client.Stat.CurrentRecord;
				nspiState.Delta = connection.Client.Stat.Delta;
				nspiState.Position = connection.Client.Stat.Position;
				nspiState.TotalRecords = connection.Client.Stat.TotalRecords;
			}
			connection.ReturnToPool();
			NspiContext.NspiTracer.TraceError<NspiStatus>((long)this.contextHandle, "ExecutePassThrough status: {0}", nspiStatus);
			return nspiStatus;
		}

		private RunspaceProxy CreateRunspaceProxy()
		{
			SidWithGroupsIdentity identity = new SidWithGroupsIdentity(this.ClientSecurityContext.UserSid.ToString(), string.Empty, this.ClientSecurityContext);
			RunspaceProxy runspaceProxy = null;
			RunspaceProxy result = null;
			try
			{
				ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = new ExchangeRunspaceConfiguration(identity);
				InitialSessionState initialSessionState = exchangeRunspaceConfiguration.CreateInitialSessionState();
				initialSessionState.LanguageMode = PSLanguageMode.FullLanguage;
				runspaceProxy = new RunspaceProxy(new RunspaceMediator(new RunspaceFactory(new BasicInitialSessionStateFactory(initialSessionState), new BasicPSHostFactory(typeof(RunspaceHost), true)), new EmptyRunspaceCache()));
				RunspaceServerSettings runspaceServerSettings = RunspaceServerSettings.CreateRunspaceServerSettings(false);
				runspaceServerSettings.ViewEntireForest = true;
				runspaceProxy.SetVariable(ExchangePropertyContainer.ADServerSettingsVarName, runspaceServerSettings);
				result = runspaceProxy;
				runspaceProxy = null;
			}
			catch (CmdletAccessDeniedException ex)
			{
				this.protocolLogSession[ProtocolLog.Field.Failures] = "RunspaceProxyDenied";
				NspiContext.NspiTracer.TraceError<CmdletAccessDeniedException, string>((long)this.contextHandle, "Failed to create RunspaceProxy: Exception: {0} StackTrace: {1}", ex, ex.StackTrace ?? "(null)");
			}
			finally
			{
				if (runspaceProxy != null)
				{
					runspaceProxy.Dispose();
					runspaceProxy = null;
				}
			}
			return result;
		}

		private bool RunPowerShellCommand(RunspaceProxy runspaceProxy, PSCommand command, out Collection<PSObject> results, out Collection<Exception> errors)
		{
			bool result = true;
			results = null;
			errors = null;
			NspiContext.NspiTracer.TraceDebug<string>((long)this.contextHandle, "RunPowerShellCommand: {0}", command.Commands[0].CommandText);
			PowerShellProxy powerShellProxy = new PowerShellProxy(runspaceProxy, command);
			try
			{
				results = powerShellProxy.Invoke<PSObject>();
			}
			catch (Exception ex)
			{
				if (errors == null)
				{
					errors = new Collection<Exception>();
				}
				errors.Add(ex);
				NspiContext.NspiTracer.TraceError<string>((long)this.contextHandle, "Exception: {0}", ex.Message);
				return false;
			}
			if (powerShellProxy.Errors != null && powerShellProxy.Errors.Count > 0)
			{
				for (int i = 0; i < powerShellProxy.Errors.Count; i++)
				{
					if (errors == null)
					{
						errors = new Collection<Exception>();
					}
					errors.Add(powerShellProxy.Errors[i].Exception);
					NspiContext.NspiTracer.TraceError<Exception>((long)this.contextHandle, "Error: {0}", powerShellProxy.Errors[i].Exception);
				}
				result = false;
			}
			if (powerShellProxy.Warnings != null && powerShellProxy.Warnings.Count > 0)
			{
				for (int j = 0; j < powerShellProxy.Warnings.Count; j++)
				{
					NspiContext.NspiTracer.TraceDebug<string>((long)this.contextHandle, "Warning: {0}", powerShellProxy.Warnings[j].Message ?? "(null)");
				}
			}
			return result;
		}

		private bool TryResolveEntryIds(IList<EntryId> entryIds, out ADObjectId[] adObjectIds)
		{
			bool flag = true;
			NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "TryResolveEntryIds (EntryId -> ADObjectId): entryIds.Count: {0}", entryIds.Count);
			if (entryIds.Count == 0)
			{
				adObjectIds = NspiContext.emptyADObjectIdArray;
				return true;
			}
			ADObjectId[] returnedObjectIds = new ADObjectId[entryIds.Count];
			List<string> list = new List<string>();
			List<int> offsets = new List<int>();
			for (int k = 0; k < entryIds.Count; k++)
			{
				Guid guid;
				EphemeralIdTable.NamingContext namingContext;
				if (entryIds[k].DistinguishedName != null)
				{
					list.Add(entryIds[k].DistinguishedName);
					offsets.Add(k);
				}
				else if (!this.ephemeralIdTable.GetGuid(entryIds[k].EphemeralId, out guid, out namingContext))
				{
					NspiContext.NspiTracer.TraceError<int>((long)this.contextHandle, "Failed to resolve mid: {0}", entryIds[k].EphemeralId);
					returnedObjectIds[k] = null;
					flag = false;
				}
				else
				{
					returnedObjectIds[k] = new ADObjectId(guid);
				}
			}
			if (list.Count > 0)
			{
				flag &= this.TryResolveLegacyDNs(list, delegate(int i, ADObjectId adObjectId, EphemeralIdTable.NamingContext nc)
				{
					returnedObjectIds[offsets[i]] = adObjectId;
				}, (int i) => returnedObjectIds[offsets[i]] != null);
			}
			adObjectIds = returnedObjectIds;
			if (NspiContext.NspiTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				for (int j = 0; j < entryIds.Count; j++)
				{
					NspiContext.NspiTracer.TraceDebug<int, string, string>((long)this.contextHandle, "EntryId {0}: {1} (ADObjectId: {2})", j, (entryIds[j] != null) ? entryIds[j].ToString() : "(null)", (adObjectIds[j] != null) ? adObjectIds[j].ToString() : "(null)");
				}
			}
			return flag;
		}

		private bool TryResolveLegacyDNs(IList<string> legacyDNs, out int[] mids)
		{
			NspiContext.NspiTracer.TraceDebug<int>((long)this.contextHandle, "TryResolveLegacyDNs (legacyDN -> MID): legacyDNs.Count: {0}", legacyDNs.Count);
			if (legacyDNs.Count == 0)
			{
				mids = NspiContext.emptyIntArray;
				return true;
			}
			int[] returnedMids = new int[legacyDNs.Count];
			bool result = this.TryResolveLegacyDNs(legacyDNs, delegate(int i, ADObjectId adObjectId, EphemeralIdTable.NamingContext nc)
			{
				if (this.personalizedServerCache != null && this.personalizedServerCache.ContainsKey(adObjectId.ObjectGuid))
				{
					returnedMids[i] = this.ephemeralIdTable.CreateEphemeralId(adObjectId.ObjectGuid, nc);
					return;
				}
				returnedMids[i] = this.ephemeralIdTable.CreateEphemeralId(adObjectId.ObjectGuid, EphemeralIdTable.GetNamingContext(adObjectId));
			}, (int i) => returnedMids[i] != 0);
			mids = returnedMids;
			if (NspiContext.NspiTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				for (int j = 0; j < legacyDNs.Count; j++)
				{
					NspiContext.NspiTracer.TraceDebug<int, string, int>((long)this.contextHandle, "LegacyDN {0}: {1} (mid: {2})", j, legacyDNs[j] ?? "(null)", mids[j]);
				}
			}
			return result;
		}

		private bool TryResolveLegacyDNs(IList<string> legacyDNs, Action<int, ADObjectId, EphemeralIdTable.NamingContext> onResolved, Func<int, bool> isResolved)
		{
			if (legacyDNs.Count == 0)
			{
				return true;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < legacyDNs.Count; i++)
			{
				string text = legacyDNs[i];
				if (string.IsNullOrEmpty(text))
				{
					num4++;
				}
				else if (text.IndexOf("/cn=recipients", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					num++;
				}
				else if (text.IndexOf("/cn=Configuration/cn=Servers", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					num2++;
				}
				else
				{
					num3++;
				}
			}
			if (num4 == legacyDNs.Count)
			{
				return true;
			}
			EphemeralIdTable.NamingContext[] array = new EphemeralIdTable.NamingContext[Datacenter.IsMultiTenancyEnabled() ? 4 : 2];
			if (Datacenter.IsMultiTenancyEnabled())
			{
				int num5 = (num > num3 + num2) ? 0 : 3;
				array[num5] = EphemeralIdTable.NamingContext.Domain;
				num5 = ((num > num3 + num2) ? 1 : 0);
				if (num2 > num3)
				{
					array[num5] = EphemeralIdTable.NamingContext.Config;
					array[num5 + 1] = EphemeralIdTable.NamingContext.TenantConfig;
					array[num5 + 2] = EphemeralIdTable.NamingContext.TenantDomain;
				}
				else if (OrganizationId.ForestWideOrgId.Equals(this.nspiPrincipal.OrganizationId))
				{
					array[num5] = EphemeralIdTable.NamingContext.Config;
					array[num5 + 1] = EphemeralIdTable.NamingContext.TenantConfig;
					array[num5 + 2] = EphemeralIdTable.NamingContext.TenantDomain;
				}
				else
				{
					array[num5] = EphemeralIdTable.NamingContext.TenantConfig;
					array[num5 + 1] = EphemeralIdTable.NamingContext.Config;
					array[num5 + 2] = EphemeralIdTable.NamingContext.TenantDomain;
				}
			}
			else if (num > num3 + num2)
			{
				array[0] = EphemeralIdTable.NamingContext.Domain;
				array[1] = EphemeralIdTable.NamingContext.Config;
			}
			else
			{
				array[0] = EphemeralIdTable.NamingContext.Config;
				array[1] = EphemeralIdTable.NamingContext.Domain;
			}
			int num6 = 0;
			foreach (EphemeralIdTable.NamingContext ncHint in array)
			{
				num6 += this.ResolveLegacyDNs(legacyDNs, ncHint, onResolved, isResolved);
				if (num4 + num6 == legacyDNs.Count)
				{
					break;
				}
			}
			return num4 + num6 == legacyDNs.Count || num4 + num6 == legacyDNs.Count;
		}

		private int ResolveLegacyDNs(IList<string> legacyDNs, EphemeralIdTable.NamingContext ncHint, Action<int, ADObjectId, EphemeralIdTable.NamingContext> onResolved, Func<int, bool> isResolved)
		{
			int num = 0;
			IDirectorySession adsessionFromNamingContext = this.GetADSessionFromNamingContext(ncHint);
			if (adsessionFromNamingContext == null)
			{
				return num;
			}
			List<string> list = new List<string>(legacyDNs.Count);
			List<int> list2 = new List<int>(legacyDNs.Count);
			for (int i = 0; i < legacyDNs.Count; i++)
			{
				if (!isResolved(i) && !string.IsNullOrEmpty(legacyDNs[i]))
				{
					string text = LegacyDN.NormalizeDN(legacyDNs[i]);
					if (text.IndexOf('@') >= 0 && text.IndexOf("/cn=Configuration/cn=Servers", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						bool flag = false;
						LegacyDN legacyDN;
						if (LegacyDN.TryParse(legacyDNs[i], out legacyDN))
						{
							string text2;
							string text3;
							LegacyDN parentLegacyDN = legacyDN.GetParentLegacyDN(out text2, out text3);
							int length;
							if (!string.IsNullOrEmpty(text3) && (length = text3.IndexOf('@')) >= 0)
							{
								string text4;
								parentLegacyDN.GetParentLegacyDN(out text2, out text4);
								if (!string.IsNullOrEmpty(text4) && string.Compare(text4, "Servers", StringComparison.OrdinalIgnoreCase) == 0 && !string.IsNullOrEmpty(text2) && string.Compare(text2, "cn", StringComparison.OrdinalIgnoreCase) == 0)
								{
									num++;
									string input = text3.Substring(0, length);
									Guid key;
									if (!Guid.TryParse(input, out key))
									{
										key = Guid.NewGuid();
									}
									if (this.personalizedServerCache == null)
									{
										this.personalizedServerCache = new Dictionary<Guid, string>();
									}
									if (!this.personalizedServerCache.ContainsKey(key))
									{
										this.personalizedServerCache.Add(key, text3);
									}
									onResolved(i, new ADObjectId(key), EphemeralIdTable.NamingContext.Config);
									flag = true;
								}
							}
						}
						if (!flag)
						{
							list.Add(text);
							list2.Add(i);
						}
					}
					else
					{
						list.Add(text);
						list2.Add(i);
					}
				}
			}
			if (list.Count > 0)
			{
				try
				{
					Result<ADRawEntry>[] array = adsessionFromNamingContext.FindByExchangeLegacyDNs(list.ToArray(), new PropertyDefinition[]
					{
						SharedPropertyDefinitions.LegacyExchangeDN,
						ADObjectSchema.Id,
						ADRecipientSchema.HiddenFromAddressListsEnabled,
						ADObjectSchema.ConfigurationUnit,
						ADRecipientSchema.RecipientTypeDetails
					});
					for (int j = 0; j < array.Length; j++)
					{
						ADRawEntry data = array[j].Data;
						if (data != null && (this.VisibleEntry(data) || !Datacenter.IsMultiTenancyEnabled()))
						{
							num++;
							onResolved(list2[j], (ADObjectId)array[j].Data[ADObjectSchema.Id], ncHint);
						}
					}
				}
				catch (ADFilterException ex)
				{
					NspiContext.NspiTracer.TraceError((long)this.contextHandle, ex.Message);
				}
			}
			return num;
		}

		private bool VisibleEntry(ADRawEntry entry)
		{
			ADObjectId adobjectId = (ADObjectId)entry[ADObjectSchema.Id];
			if (adobjectId.IsDescendantOf(Configuration.ConfigNamingContext) || adobjectId.IsDescendantOf(ADSession.GetConfigurationUnitsRootForLocalForest()))
			{
				if ((adobjectId.IsDescendantOf(Configuration.MicrosoftExchangeConfigurationRoot) || adobjectId.IsDescendantOf(ADSession.GetConfigurationUnitsRootForLocalForest())) && (entry[ADObjectSchema.ConfigurationUnit] == null || object.Equals(entry[ADObjectSchema.ConfigurationUnit], this.nspiPrincipal.ConfigurationUnit)))
				{
					return true;
				}
			}
			else
			{
				if (!(bool)entry[ADRecipientSchema.HiddenFromAddressListsEnabled])
				{
					return true;
				}
				string text = (string)entry[SharedPropertyDefinitions.LegacyExchangeDN];
				if (!string.IsNullOrEmpty(text) && text.Equals(this.nspiPrincipal.LegacyDistinguishedName, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)entry[ADRecipientSchema.RecipientTypeDetails];
				if (recipientTypeDetails == RecipientTypeDetails.DiscoveryMailbox)
				{
					return true;
				}
			}
			return false;
		}

		private bool HideDLMembers(Guid guid)
		{
			IRecipientSession recipientSession = this.GetRecipientSession(null);
			Guid[] objectGuids = new Guid[]
			{
				guid
			};
			Result<ADRawEntry>[] array = recipientSession.FindByObjectGuids(objectGuids, new PropertyDefinition[]
			{
				ADGroupSchema.HiddenGroupMembershipEnabled
			});
			if (array[0].Data != null && (bool)array[0].Data[ADGroupSchema.HiddenGroupMembershipEnabled])
			{
				NspiContext.NspiTracer.TraceDebug((long)this.contextHandle, "HiddenGroupMembershipEnabled");
				return true;
			}
			return false;
		}

		internal const int InvalidHandle = 0;

		internal const int MaxAddressLists = 2500;

		internal const int MaxGetMatchesRows = 50;

		private static readonly ADObjectId NoGalObjectID = new ADObjectId(EphemeralIdTable.InvalidGuid);

		private static readonly Microsoft.Exchange.Diagnostics.Trace NspiTracer = ExTraceGlobals.NspiTracer;

		private static readonly PropTag[] defaultResolveNamesPropertiesAnsi = new PropTag[]
		{
			PropTag.ObjectType,
			PropTag.EntryId,
			PropTag.SearchKey,
			PropTag.RecordKey,
			PropTag.AddrTypeAnsi,
			PropTag.EmailAddressAnsi,
			PropTag.DisplayType,
			PropTag.TemplateId,
			PropTag.TransmitableDisplayNameAnsi,
			PropTag.DisplayNameAnsi,
			PropTag.MappingSignature
		};

		private static readonly PropTag[] defaultQueryRowsPropertiesAnsi = new PropTag[]
		{
			(PropTag)4294770691U,
			PropTag.ObjectType,
			PropTag.DisplayType,
			PropTag.DisplayNameAnsi,
			PropTag.PrimaryTelephoneNumberAnsi,
			PropTag.DepartmentNameAnsi,
			PropTag.OfficeLocationAnsi
		};

		private static readonly SortBy SortByDisplayName = new SortBy(ADRecipientSchema.DisplayName, SortOrder.Ascending);

		private static readonly SortBy SortByPhoneticDisplayName = new SortBy(ADRecipientSchema.PhoneticDisplayName, SortOrder.Ascending);

		private static readonly ADObjectId domainNamingContext = new ADObjectId();

		private static readonly ADObjectId[] emptyADObjectIdArray = new ADObjectId[0];

		private static readonly byte[][] emptyByteArrayArray = new byte[0][];

		private static readonly int[] emptyIntArray = new int[0];

		private static readonly PropTag[] objectGuidPropTags = new PropTag[]
		{
			(PropTag)2355953922U
		};

		private static readonly string[] networkAddressPatterns = new string[]
		{
			"ncacn_http:{0}",
			"ncacn_ip_tcp:{0}"
		};

		private static readonly string nullStateMessage = "Null State";

		private static ADObjectId configNamingContext;

		private static int lastContextHandle;

		private readonly int contextHandle;

		private readonly Stopwatch stopwatch = Stopwatch.StartNew();

		private readonly Guid guid = Guid.NewGuid();

		private readonly ProtocolLogSession protocolLogSession;

		private readonly string protocolSequence;

		private readonly string userDomain;

		private ClientSecurityContext clientSecurityContext;

		private NspiPrincipal nspiPrincipal;

		private EphemeralIdTable ephemeralIdTable = new EphemeralIdTable();

		private bool traceUser;

		private int sortLocale;

		private DisposeTracker disposeTracker;

		private int galEphemeralId;

		private string nspiServer;

		private string userIdentity;

		private ModCache modCache;

		private IStandardBudget budget;

		private Dictionary<int, bool> addressListPermissionsCache;

		private bool isAnonymous;

		private Dictionary<Guid, string> personalizedServerCache;

		private ActivityScope scope;

		internal class DisplayNameComparer : IComparer<ADRawEntry>
		{
			internal DisplayNameComparer(CultureInfo cultureInfo)
			{
				this.cultureInfo = cultureInfo;
			}

			public int Compare(ADRawEntry x, ADRawEntry y)
			{
				return string.Compare((string)x[ADRecipientSchema.DisplayName], (string)y[ADRecipientSchema.DisplayName], true, this.cultureInfo);
			}

			private readonly CultureInfo cultureInfo;
		}
	}
}
