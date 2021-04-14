using System;

namespace Microsoft.Exchange.Rpc.ExchangeCertificate
{
	internal enum RpcOutput
	{
		ExchangeCertList,
		ExchangeCert,
		CertRequest = 100,
		ExportBase64 = 300,
		ExportFile,
		ExportBinary,
		ExportPKCS10,
		TaskErrorString = 1000,
		TaskErrorCategory,
		TaskWarningString,
		TaskConfirmation,
		TaskWarningList,
		TaskConfirmationList
	}
}
