using System;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ExchangeCertificate;

namespace Microsoft.Exchange.Servicelets.ExchangeCertificate
{
	internal class ExchangeCertificateServer2 : ExchangeCertificateRpcServer2
	{
		public static bool Start(out Exception e)
		{
			e = null;
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
			FileSystemAccessRule accessRule = new FileSystemAccessRule(securityIdentifier, FileSystemRights.Read, AccessControlType.Allow);
			FileSecurity fileSecurity = new FileSecurity();
			fileSecurity.SetOwner(securityIdentifier);
			fileSecurity.SetAccessRule(accessRule);
			bool result;
			try
			{
				ExchangeCertificateServer2.server = (ExchangeCertificateServer2)RpcServerBase.RegisterServer(typeof(ExchangeCertificateServer2), fileSecurity, 1, false);
				result = true;
			}
			catch (RpcException ex)
			{
				e = ex;
				result = false;
			}
			return result;
		}

		public static void Stop()
		{
			if (ExchangeCertificateServer2.server != null)
			{
				RpcServerBase.StopServer(ExchangeCertificateRpcServer2.RpcIntfHandle);
				ExchangeCertificateServer2.server = null;
			}
		}

		public override byte[] CreateCertificate2(int version, byte[] inputBlob)
		{
			return ExchangeCertificateServerHelper.CreateCertificate(ExchangeCertificateRpcVersion.Version2, inputBlob);
		}

		public override byte[] GetCertificate2(int version, byte[] inputBlob)
		{
			return ExchangeCertificateServerHelper.GetCertificate(ExchangeCertificateRpcVersion.Version2, inputBlob);
		}

		public override byte[] RemoveCertificate2(int version, byte[] inputBlob)
		{
			return ExchangeCertificateServerHelper.RemoveCertificate(ExchangeCertificateRpcVersion.Version2, inputBlob);
		}

		public override byte[] ExportCertificate2(int version, byte[] inputBlob, SecureString password)
		{
			return ExchangeCertificateServerHelper.ExportCertificate(ExchangeCertificateRpcVersion.Version2, inputBlob, password);
		}

		public override byte[] ImportCertificate2(int version, byte[] inputBlob, SecureString password)
		{
			return ExchangeCertificateServerHelper.ImportCertificate(ExchangeCertificateRpcVersion.Version2, inputBlob, password);
		}

		public override byte[] EnableCertificate2(int version, byte[] inputBlob)
		{
			return ExchangeCertificateServerHelper.EnableCertificate(ExchangeCertificateRpcVersion.Version2, inputBlob);
		}

		private static ExchangeCertificateServer2 server;
	}
}
