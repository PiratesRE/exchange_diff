using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net.Sockets;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class UpdateAddressBookBase<TIdParameter> : SystemConfigurationObjectActionTask<TIdParameter, AddressBookBase> where TIdParameter : ADIdParameter, new()
	{
		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			this.DataObject = (AddressBookBase)base.PrepareDataObject();
			if (!base.HasErrors && this.DataObject.IsTopContainer)
			{
				TIdParameter identity = this.Identity;
				base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnContainer(identity.ToString())), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			TaskLogger.LogExit();
			return this.DataObject;
		}

		internal static void UpdateRecipients(AddressBookBase abb, string domainController, IRecipientSession globalCatalogSession, Task.TaskWarningLoggingDelegate writeWarning, WriteProgress writeProgress, Cmdlet cmdlet)
		{
			UpdateAddressBookBase<TIdParameter>.UpdateRecipients(abb, null, domainController, globalCatalogSession, writeWarning, writeProgress, cmdlet);
		}

		internal static void UpdateRecipients(AddressBookBase abb, ADObjectId[] removedAbbTreeIds, string domainController, IRecipientSession globalCatalogSession, Task.TaskWarningLoggingDelegate writeWarning, WriteProgress writeProgress, Cmdlet cmdlet)
		{
			if (abb == null)
			{
				throw new ArgumentNullException("abb");
			}
			if (Guid.Empty == abb.Guid)
			{
				throw new ArgumentNullException("abb.Guid");
			}
			if (writeWarning == null)
			{
				throw new ArgumentNullException("writeWarning");
			}
			if (writeProgress == null)
			{
				throw new ArgumentNullException("writeProgress");
			}
			int num = 0;
			try
			{
				if (cmdlet != null && cmdlet.Stopping)
				{
					return;
				}
				string domainControllerFqdn = null;
				if (!string.IsNullOrEmpty(domainController))
				{
					try
					{
						domainControllerFqdn = SystemConfigurationTasksHelper.GetConfigurationDomainControllerFqdn(domainController);
					}
					catch (SocketException ex)
					{
						writeWarning(Strings.ErrorResolveFqdnForDomainController(domainController, ex.Message));
						return;
					}
				}
				if (string.IsNullOrEmpty(abb.LdapRecipientFilter) || removedAbbTreeIds != null)
				{
					List<Guid> list = new List<Guid>();
					if (removedAbbTreeIds != null)
					{
						foreach (ADObjectId adobjectId in removedAbbTreeIds)
						{
							list.Add(adobjectId.ObjectGuid);
						}
						list.Sort();
					}
					else
					{
						list.Add(abb.Guid);
					}
					QueryFilter[] array = null;
					int num2 = LdapFilterBuilder.MaxCustomFilterTreeSize - 1;
					for (int j = 0; j < list.Count; j += num2)
					{
						int num3 = Math.Min(num2, list.Count - j);
						if (array == null || array.Length != num3)
						{
							array = new QueryFilter[num3];
						}
						for (int k = 0; k < num3; k++)
						{
							array[k] = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.AddressListMembership, new ADObjectId(list[k + j]));
						}
						QueryFilter queryFilter = new OrFilter(array);
						ADPagedReader<ADRecipient> adpagedReader = globalCatalogSession.FindPaged(null, QueryScope.SubTree, queryFilter, null, 0);
						using (IEnumerator<ADRecipient> enumerator = adpagedReader.GetEnumerator())
						{
							UpdateAddressBookBase<TIdParameter>.InternalUpdateRecipients(enumerator, null, queryFilter, list, domainControllerFqdn, globalCatalogSession, writeWarning, writeProgress, cmdlet, ref num);
						}
					}
				}
				else
				{
					IEnumerator<ADRecipient> enumerator2 = abb.FindUpdatingRecipientsPaged(globalCatalogSession, null).GetEnumerator();
					UpdateAddressBookBase<TIdParameter>.InternalUpdateRecipients(enumerator2, abb, null, null, domainControllerFqdn, globalCatalogSession, writeWarning, writeProgress, cmdlet, ref num);
				}
			}
			finally
			{
				if (cmdlet != null && cmdlet.Stopping)
				{
					ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_RecipientsUpdateForAddressBookCancelled, new string[]
					{
						string.Format("{0} (objectGUID=<{1}>)", abb.DistinguishedName ?? string.Empty, abb.Guid.ToString()),
						abb.LdapRecipientFilter,
						ADRecipientSchema.AddressListMembership.Name
					});
				}
			}
			if (num != 0)
			{
				writeProgress(Strings.ProgressActivityUpdateRecipient, Strings.ProgressStatusFinished, 100);
			}
		}

		private static void InternalUpdateRecipients(IEnumerator<ADRecipient> pagedReaderGC, AddressBookBase abb, QueryFilter removeFilter, List<Guid> abbObjectGuidList, string domainControllerFqdn, IRecipientSession globalCatalogSession, Task.TaskWarningLoggingDelegate writeWarning, WriteProgress writeProgress, Cmdlet cmdlet, ref int currentPercent)
		{
			if (cmdlet != null && cmdlet.Stopping)
			{
				return;
			}
			string domainControllerDomainName = null;
			if (!string.IsNullOrEmpty(domainControllerFqdn))
			{
				int num = domainControllerFqdn.IndexOf(".");
				if (0 <= num)
				{
					domainControllerDomainName = domainControllerFqdn.Substring(num + 1);
				}
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, globalCatalogSession.SessionSettings, 300, "InternalUpdateRecipients", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\AddressBook\\UpdateAddressBook.cs");
			tenantOrRootOrgRecipientSession.EnforceDefaultScope = false;
			tenantOrRootOrgRecipientSession.EnforceContainerizedScoping = false;
			string text = null;
			while (pagedReaderGC.MoveNext())
			{
				ADRecipient adrecipient = pagedReaderGC.Current;
				if (cmdlet != null && cmdlet.Stopping)
				{
					return;
				}
				ADRecipient adrecipient2 = adrecipient;
				try
				{
					SystemConfigurationTasksHelper.PrepareDomainControllerRecipientSessionForUpdate(tenantOrRootOrgRecipientSession, adrecipient.Id, domainControllerFqdn, domainControllerDomainName);
					tenantOrRootOrgRecipientSession.LinkResolutionServer = null;
					IConfigurable configurable = null;
					if (abb != null)
					{
						IEnumerator<ADRecipient> enumerator = abb.FindUpdatingRecipientsPaged(tenantOrRootOrgRecipientSession, adrecipient.Id).GetEnumerator();
						if (enumerator.MoveNext())
						{
							configurable = enumerator.Current;
						}
					}
					else
					{
						IConfigurable[] array = tenantOrRootOrgRecipientSession.Find(adrecipient.Id, QueryScope.Base, removeFilter, null, 1);
						if (array.Length > 0)
						{
							configurable = array[0];
						}
					}
					if (configurable != null)
					{
						adrecipient2 = (ADRecipient)configurable;
						if (!adrecipient2.IsValid || adrecipient2.IsReadOnly)
						{
							writeWarning(Strings.ErrorCannotUpdateInvalidRecipient(adrecipient2.Id.ToString()));
						}
						else
						{
							if (cmdlet != null && cmdlet.Stopping)
							{
								break;
							}
							bool flag = false;
							if (abb != null)
							{
								using (MultiValuedProperty<ADObjectId>.Enumerator enumerator2 = adrecipient2.AddressListMembership.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										ADObjectId adobjectId = enumerator2.Current;
										if (ADObjectId.Equals(adobjectId, abb.Id))
										{
											flag = true;
											adrecipient2.AddressListMembership.Remove(adobjectId);
											break;
										}
									}
									goto IL_1E5;
								}
							}
							int num2 = adrecipient2.AddressListMembership.Count - 1;
							while (0 <= num2)
							{
								if (abbObjectGuidList.BinarySearch(adrecipient2.AddressListMembership[num2].ObjectGuid) >= 0)
								{
									flag = true;
									adrecipient2.AddressListMembership.RemoveAt(num2);
								}
								num2--;
							}
							IL_1E5:
							if (!flag && !adrecipient2.HiddenFromAddressListsEnabled)
							{
								if (string.IsNullOrEmpty(text))
								{
									try
									{
										if (!string.IsNullOrEmpty(abb.OriginatingServer))
										{
											text = SystemConfigurationTasksHelper.GetConfigurationDomainControllerFqdn(abb.OriginatingServer);
										}
									}
									catch (SocketException ex)
									{
										writeWarning(Strings.ErrorResolveFqdnForDomainController(abb.OriginatingServer, ex.Message));
										continue;
									}
								}
								tenantOrRootOrgRecipientSession.LinkResolutionServer = text;
								adrecipient2.AddressListMembership.Add(abb.Id);
							}
							currentPercent = currentPercent++ % 99 + 1;
							writeProgress(Strings.ProgressActivityUpdateRecipient, Strings.ProgressStatusUpdateRecipient(adrecipient2.Id.ToString()), currentPercent);
							tenantOrRootOrgRecipientSession.Save(adrecipient2);
						}
					}
				}
				catch (DataSourceTransientException ex2)
				{
					writeWarning(Strings.ErrorUpdateRecipient(adrecipient2.Id.ToString(), ex2.Message));
					TaskLogger.Trace("Exception is raised while updating recipient '{0}': {1}", new object[]
					{
						adrecipient2.Id.ToString(),
						ex2.Message
					});
				}
				catch (DataSourceOperationException ex3)
				{
					writeWarning(Strings.ErrorUpdateRecipient(adrecipient2.Id.ToString(), ex3.Message));
					TaskLogger.Trace("Exception is raised while updating recipient '{0}': {1}", new object[]
					{
						adrecipient2.Id.ToString(),
						ex3.Message
					});
				}
				catch (DataValidationException ex4)
				{
					writeWarning(Strings.ErrorUpdateRecipient(adrecipient2.Id.ToString(), ex4.Message));
					TaskLogger.Trace("Exception is raised while updating recipient '{0}': {1}", new object[]
					{
						adrecipient2.Id.ToString(),
						ex4.Message
					});
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, this.DataObject.OrganizationId, base.ExecutingUserOrganizationId, false);
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.TenantGlobalCatalogSession.DomainController, true, ConsistencyMode.PartiallyConsistent, sessionSettings, 459, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\AddressBook\\UpdateAddressBook.cs");
				tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
				UpdateAddressBookBase<TIdParameter>.UpdateRecipients(this.DataObject, base.DomainController, tenantOrRootOrgRecipientSession, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new WriteProgress(base.WriteProgress), this);
			}
			catch (DataSourceTransientException ex)
			{
				TIdParameter identity = this.Identity;
				base.WriteError(new InvalidOperationException(Strings.ErrorReadMatchingRecipients(identity.ToString(), this.DataObject.LdapRecipientFilter, ex.Message), ex), ErrorCategory.InvalidOperation, this.DataObject.Id);
				TaskLogger.Trace("Exception is raised while reading recipients: {0}", new object[]
				{
					ex.ToString()
				});
			}
			catch (DataSourceOperationException ex2)
			{
				TIdParameter identity2 = this.Identity;
				base.WriteError(new InvalidOperationException(Strings.ErrorReadMatchingRecipients(identity2.ToString(), this.DataObject.LdapRecipientFilter, ex2.Message), ex2), ErrorCategory.InvalidOperation, this.DataObject.Id);
				TaskLogger.Trace("Exception is raised while reading recipients matching filter: {0}", new object[]
				{
					ex2.ToString()
				});
			}
			if (!this.DataObject.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2007))
			{
				this.DataObject[AddressBookBaseSchema.LastUpdatedRecipientFilter] = this.DataObject.RecipientFilter;
				this.DataObject[AddressBookBaseSchema.RecipientFilterApplied] = true;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}
	}
}
