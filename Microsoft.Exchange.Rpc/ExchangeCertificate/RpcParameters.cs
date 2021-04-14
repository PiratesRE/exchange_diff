using System;

namespace Microsoft.Exchange.Rpc.ExchangeCertificate
{
	internal enum RpcParameters
	{
		GetByThumbprint,
		GetByCertificate,
		GetByDomains,
		CreateExportable = 100,
		CreateSubjectName,
		CreateFriendlyName,
		CreateDomains,
		CreateIncAccepted,
		CreateIncAutoDisc,
		CreateIncFqdn,
		CreateIncNetBios,
		CreateKeySize,
		CreateCloneCert,
		CreateBinary,
		CreateRequest,
		CreateServices,
		CreateSubjectKeyIdentifier,
		CreateAllowConfirmation,
		CreateWhatIf,
		RequireSsl,
		RemoveByThumbprint = 200,
		ExportByThumbprint = 300,
		ExportBinary,
		ImportCert = 401,
		ImportDescription,
		ImportExportable,
		EnableByThumbprint = 500,
		EnableServices,
		EnableAllowConfirmation,
		EnableUpdateAD,
		EnableNetworkService
	}
}
