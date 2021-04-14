using System;
using System.Security;

namespace Microsoft.Exchange.Rpc.ExchangeCertificate
{
	internal abstract class ExchangeCertificateRpcServer : RpcServerBase
	{
		public abstract byte[] GetCertificate(int version, byte[] pInBytes);

		public abstract byte[] CreateCertificate(int version, byte[] pInBytes);

		public abstract byte[] RemoveCertificate(int version, byte[] pInBytes);

		public abstract byte[] ExportCertificate(int version, byte[] pInBytes, SecureString password);

		public abstract byte[] ImportCertificate(int version, byte[] pInBytes, SecureString password);

		public abstract byte[] EnableCertificate(int version, byte[] pInBytes);

		public ExchangeCertificateRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IExchangeCertificate_v1_0_s_ifspec;
	}
}
