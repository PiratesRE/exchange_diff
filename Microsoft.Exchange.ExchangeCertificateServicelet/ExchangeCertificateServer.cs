using System;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ExchangeCertificate;

namespace Microsoft.Exchange.Servicelets.ExchangeCertificate
{
	internal class ExchangeCertificateServer : ExchangeCertificateRpcServer
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
				ExchangeCertificateServer.server = (ExchangeCertificateServer)RpcServerBase.RegisterServer(typeof(ExchangeCertificateServer), fileSecurity, 1, false);
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
			if (ExchangeCertificateServer.server != null)
			{
				RpcServerBase.StopServer(ExchangeCertificateRpcServer.RpcIntfHandle);
				ExchangeCertificateServer.server = null;
			}
		}

		public override byte[] CreateCertificate(int version, byte[] inputBlob)
		{
			return ExchangeCertificateServerHelper.CreateCertificate(ExchangeCertificateRpcVersion.Version1, inputBlob);
		}

		public override byte[] GetCertificate(int version, byte[] inputBlob)
		{
			return ExchangeCertificateServerHelper.GetCertificate(ExchangeCertificateRpcVersion.Version1, inputBlob);
		}

		public override byte[] RemoveCertificate(int version, byte[] inputBlob)
		{
			return ExchangeCertificateServerHelper.RemoveCertificate(ExchangeCertificateRpcVersion.Version1, inputBlob);
		}

		public override byte[] ExportCertificate(int version, byte[] inputBlob, SecureString password)
		{
			return ExchangeCertificateServerHelper.ExportCertificate(ExchangeCertificateRpcVersion.Version1, inputBlob, password);
		}

		public override byte[] ImportCertificate(int version, byte[] inputBlob, SecureString password)
		{
			return ExchangeCertificateServerHelper.ImportCertificate(ExchangeCertificateRpcVersion.Version1, inputBlob, password);
		}

		public override byte[] EnableCertificate(int version, byte[] inputBlob)
		{
			return ExchangeCertificateServerHelper.EnableCertificate(ExchangeCertificateRpcVersion.Version1, inputBlob);
		}

		internal const string RequestStoreName = "REQUEST";

		private static ExchangeCertificateServer server;
	}
}
