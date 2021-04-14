using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class OrganizationIdConverter
	{
		public OrganizationId GetOrganizationId(string externalDirectoryOrgId)
		{
			OrganizationId result = OrganizationId.ForestWideOrgId;
			if (!string.IsNullOrEmpty(externalDirectoryOrgId))
			{
				try
				{
					result = OrganizationId.FromExternalDirectoryOrganizationId(Guid.Parse(externalDirectoryOrgId));
				}
				catch (CannotResolveExternalDirectoryOrganizationIdException exception)
				{
					PushNotificationsCrimsonEvents.CannotResolveTenantId.LogPeriodic<string>(externalDirectoryOrgId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, exception.ToTraceString());
				}
			}
			return result;
		}

		public static readonly OrganizationIdConverter Default = new OrganizationIdConverter();
	}
}
