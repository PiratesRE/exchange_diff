using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class StampGroupTaskHelper
	{
		internal static void CheckServerDoesNotBelongToDifferentStampGroup(Task.TaskErrorLoggingDelegate writeError, IConfigDataProvider dataSession, Server server, string stampGroupName)
		{
			ADObjectId databaseAvailabilityGroup = server.DatabaseAvailabilityGroup;
			if (databaseAvailabilityGroup != null)
			{
				StampGroup stampGroup = (StampGroup)dataSession.Read<StampGroup>(databaseAvailabilityGroup);
				if (stampGroup != null && stampGroup.Name != stampGroupName)
				{
					writeError(new DagTaskServerMailboxServerIsInDifferentDagException(server.Name, stampGroup.Name), ErrorCategory.InvalidArgument, null);
				}
			}
		}

		internal static StampGroup StampGroupIdParameterToStampGroup(StampGroupIdParameter stampGroupIdParam, IConfigDataProvider configSession)
		{
			IEnumerable<StampGroup> objects = stampGroupIdParam.GetObjects<StampGroup>(null, configSession);
			StampGroup result;
			using (IEnumerator<StampGroup> enumerator = objects.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new ManagementObjectNotFoundException(Strings.ErrorDagNotFound(stampGroupIdParam.ToString()));
				}
				StampGroup stampGroup = enumerator.Current;
				if (enumerator.MoveNext())
				{
					throw new ManagementObjectAmbiguousException(Strings.ErrorDagNotUnique(stampGroupIdParam.ToString()));
				}
				result = stampGroup;
			}
			return result;
		}

		internal static StampGroup ReadStampGroup(ADObjectId stampGroupId, IConfigDataProvider configSession)
		{
			StampGroup result = null;
			if (!ADObjectId.IsNullOrEmpty(stampGroupId))
			{
				result = (StampGroup)configSession.Read<StampGroup>(stampGroupId);
			}
			return result;
		}

		internal static ADObjectId FindServerAdObjectIdInStampGroup(StampGroup stampGroup, AmServerName serverName)
		{
			foreach (ADObjectId adobjectId in stampGroup.Servers)
			{
				if (SharedHelper.StringIEquals(adobjectId.Name, serverName.NetbiosName))
				{
					return adobjectId;
				}
			}
			return null;
		}
	}
}
