using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using www.outlook.com.highavailability.ServerLocator.v1;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[ServiceContract(Namespace = "http://www.outlook.com/highavailability/ServerLocator/v1/", ConfigurationName = "ServerLocator")]
public interface ServerLocator
{
	[OperationContract(Action = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetVersion", ReplyAction = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetVersionResponse")]
	ServiceVersion GetVersion();

	[OperationContract(AsyncPattern = true, Action = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetVersion", ReplyAction = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetVersionResponse")]
	IAsyncResult BeginGetVersion(AsyncCallback callback, object asyncState);

	ServiceVersion EndGetVersion(IAsyncResult result);

	[OperationContract(Action = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetServerForDatabase", ReplyAction = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetServerForDatabaseResponse")]
	[FaultContract(typeof(DatabaseServerInformationFault), Action = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetServerForDatabaseDatabaseServerInformationFaultFault", Name = "DatabaseServerInformationFault")]
	DatabaseServerInformation GetServerForDatabase(DatabaseServerInformation database);

	[OperationContract(AsyncPattern = true, Action = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetServerForDatabase", ReplyAction = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetServerForDatabaseResponse")]
	IAsyncResult BeginGetServerForDatabase(DatabaseServerInformation database, AsyncCallback callback, object asyncState);

	DatabaseServerInformation EndGetServerForDatabase(IAsyncResult result);

	[FaultContract(typeof(DatabaseServerInformationFault), Action = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetActiveCopiesForDatabaseAvailabilityGroupDatabaseServerInformationFaultFault", Name = "DatabaseServerInformationFault")]
	[OperationContract(Action = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetActiveCopiesForDatabaseAvailabilityGroup", ReplyAction = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetActiveCopiesForDatabaseAvailabilityGroupResponse")]
	DatabaseServerInformation[] GetActiveCopiesForDatabaseAvailabilityGroup();

	[OperationContract(AsyncPattern = true, Action = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetActiveCopiesForDatabaseAvailabilityGroup", ReplyAction = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetActiveCopiesForDatabaseAvailabilityGroupResponse")]
	IAsyncResult BeginGetActiveCopiesForDatabaseAvailabilityGroup(AsyncCallback callback, object asyncState);

	DatabaseServerInformation[] EndGetActiveCopiesForDatabaseAvailabilityGroup(IAsyncResult result);

	[OperationContract(Action = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetActiveCopiesForDatabaseAvailabilityGroupExtended", ReplyAction = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetActiveCopiesForDatabaseAvailabilityGroupExtendedResponse")]
	[FaultContract(typeof(DatabaseServerInformationFault), Action = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetActiveCopiesForDatabaseAvailabilityGroupExtendedDatabaseServerInformationFaultFault", Name = "DatabaseServerInformationFault")]
	DatabaseServerInformation[] GetActiveCopiesForDatabaseAvailabilityGroupExtended(GetActiveCopiesForDatabaseAvailabilityGroupParameters parameters);

	[OperationContract(AsyncPattern = true, Action = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetActiveCopiesForDatabaseAvailabilityGroupExtended", ReplyAction = "http://www.outlook.com/highavailability/ServerLocator/v1/ServerLocator/GetActiveCopiesForDatabaseAvailabilityGroupExtendedResponse")]
	IAsyncResult BeginGetActiveCopiesForDatabaseAvailabilityGroupExtended(GetActiveCopiesForDatabaseAvailabilityGroupParameters parameters, AsyncCallback callback, object asyncState);

	DatabaseServerInformation[] EndGetActiveCopiesForDatabaseAvailabilityGroupExtended(IAsyncResult result);
}
