using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class ClientAccessArrayTaskHelper
	{
		internal ClientAccessArrayTaskHelper(Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskErrorLoggingDelegate writeError)
		{
			this.writeVerbose = writeVerbose;
			this.writeError = writeError;
		}

		internal ADSite GetADSite(AdSiteIdParameter siteId, ITopologyConfigurationSession session, DataAccessHelper.GetDataObjectDelegate getDataObject)
		{
			this.writeVerbose(TaskVerboseStringHelper.GetFindByIdParameterVerboseString(siteId, session, typeof(ADSite), session.GetConfigurationNamingContext().GetChildId("Sites")));
			return (ADSite)getDataObject(siteId, session, null, null, new LocalizedString?(Strings.ErrorSiteNotFound(siteId.ToString())), new LocalizedString?(Strings.ErrorSiteNotUnique(siteId.ToString())));
		}

		internal void VerifyArrayUniqueness(IConfigDataProvider session, ClientAccessArray array)
		{
			this.CheckUnique(session, array.Id, new Func<IConfigDataProvider, QueryFilter, bool>(this.ServerOrArrayExists), ADObjectSchema.Name, array.Name, Strings.ErrorCasArrayOrServerAlreadyExists(array.Name));
		}

		internal bool ObjectExists(IConfigDataProvider session, ADObjectId thisObjectId, Func<IConfigDataProvider, QueryFilter, bool> objectExists, PropertyDefinition keyProperty, object keyValue)
		{
			QueryFilter arg = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, keyProperty, keyValue),
				(thisObjectId != null) ? new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, thisObjectId) : null
			});
			return objectExists(session, arg);
		}

		internal bool ServerOrArrayExists(IConfigDataProvider session, QueryFilter filter)
		{
			return this.ObjectExists<Server>(session, filter) || this.ObjectExists<ClientAccessArray>(session, filter);
		}

		internal QueryFilter GetSiteFilter(ADObjectId siteId)
		{
			if (siteId == null)
			{
				return null;
			}
			return new ComparisonFilter(ComparisonOperator.Equal, ServerSchema.ServerSite, siteId);
		}

		private void CheckUnique(IConfigDataProvider session, ADObjectId thisObjectId, Func<IConfigDataProvider, QueryFilter, bool> objectExists, PropertyDefinition keyProperty, object keyValue, LocalizedString errorMessage)
		{
			if (this.ObjectExists(session, thisObjectId, objectExists, keyProperty, keyValue))
			{
				this.writeError(new InvalidOperationException(errorMessage), ErrorCategory.InvalidOperation, keyValue);
			}
		}

		internal bool ObjectExists<T>(IConfigDataProvider session, QueryFilter filter) where T : ADConfigurationObject, new()
		{
			this.writeVerbose(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(session, typeof(T), filter, null, true));
			return session.Find<T>(filter, null, true, null).Length > 0;
		}

		internal string FindRpcClientAccessArrayOrServer(ITopologyConfigurationSession session, ADObjectId localServerId)
		{
			ADSite localSite = session.GetLocalSite();
			if (localSite == null)
			{
				return null;
			}
			ClientAccessArray[] array = session.Find<ClientAccessArray>(null, QueryScope.SubTree, QueryFilter.AndTogether(new QueryFilter[]
			{
				this.GetSiteFilter(localSite.Id),
				ClientAccessArray.PriorTo15ExchangeObjectVersionFilter
			}), null, 2);
			if (array.Length > 0)
			{
				return array[0].ExchangeLegacyDN;
			}
			List<string> list = new List<string>();
			foreach (KeyValuePair<Server, ExchangeRpcClientAccess> keyValuePair in ExchangeRpcClientAccess.GetServersWithRpcClientAccessEnabled(ExchangeRpcClientAccess.GetAllPossibleServers(session, localSite.Id), ExchangeRpcClientAccess.GetAll(session)))
			{
				if ((keyValuePair.Value.Responsibility & RpcClientAccessResponsibility.Mailboxes) != RpcClientAccessResponsibility.None)
				{
					if (localServerId.Equals(keyValuePair.Key.Id))
					{
						return keyValuePair.Key.ExchangeLegacyDN;
					}
					list.Add(keyValuePair.Key.ExchangeLegacyDN);
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			return NewRpcClientAccess.SelectServerWithEqualProbability(list);
		}

		private readonly Task.TaskErrorLoggingDelegate writeError;

		private readonly Task.TaskVerboseLoggingDelegate writeVerbose;
	}
}
