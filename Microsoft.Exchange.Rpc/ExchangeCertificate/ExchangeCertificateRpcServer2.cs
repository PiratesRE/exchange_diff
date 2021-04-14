using System;
using System.Security;

namespace Microsoft.Exchange.Rpc.ExchangeCertificate
{
	internal abstract class ExchangeCertificateRpcServer2 : RpcServerBase
	{
		public abstract byte[] GetCertificate2(int version, byte[] pInBytes);

		public abstract byte[] CreateCertificate2(int version, byte[] pInBytes);

		public abstract byte[] RemoveCertificate2(int version, byte[] pInBytes);

		public abstract byte[] ExportCertificate2(int version, byte[] pInBytes, SecureString password);

		public abstract byte[] ImportCertificate2(int version, byte[] pInBytes, SecureString password);

		public abstract byte[] EnableCertificate2(int version, byte[] pInBytes);

		public ExchangeCertificateRpcServer2()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IExchangeCertificate2_v1_0_s_ifspec;
	}
}
