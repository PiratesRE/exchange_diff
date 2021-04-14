using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IServerPicker<ManagedObjectType, ContextObjectType> where ManagedObjectType : class
	{
		ManagedObjectType PickNextServer(ContextObjectType context);

		ManagedObjectType PickNextServer(ContextObjectType context, out int totalServers);

		ManagedObjectType PickNextServer(ContextObjectType context, Guid tenantGuid, out int totalServers);

		void ServerUnavailable(ManagedObjectType server);
	}
}
