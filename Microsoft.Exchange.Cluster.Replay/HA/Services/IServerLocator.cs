using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Microsoft.Exchange.HA.Services
{
	[ServiceContract(Name = "ServerLocator", Namespace = "http://www.outlook.com/highavailability/ServerLocator/v1/")]
	public interface IServerLocator
	{
		[OperationContract]
		ServiceVersion GetVersion();

		[OperationContract]
		[FaultContract(typeof(DatabaseServerInformationFault))]
		DatabaseServerInformation GetServerForDatabase(DatabaseServerInformation database);

		[FaultContract(typeof(DatabaseServerInformationFault))]
		[OperationContract]
		List<DatabaseServerInformation> GetActiveCopiesForDatabaseAvailabilityGroup();

		[FaultContract(typeof(DatabaseServerInformationFault))]
		[OperationContract]
		List<DatabaseServerInformation> GetActiveCopiesForDatabaseAvailabilityGroupExtended(GetActiveCopiesForDatabaseAvailabilityGroupParameters parameters);
	}
}
