using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal class E4eEncryptionHelperV2 : E4eEncryptionHelper
	{
		internal new static E4eEncryptionHelperV2 Instance
		{
			get
			{
				if (E4eEncryptionHelperV2.instance == null)
				{
					E4eEncryptionHelperV2.instance = new E4eEncryptionHelperV2();
				}
				return E4eEncryptionHelperV2.instance;
			}
		}

		internal virtual int Version()
		{
			return 2;
		}

		internal override void AppendVersionInfo(StringBuilder html, string messageId)
		{
			html.Append(string.Format("<input type='hidden' name='{0}' value='{1}' />", "version", this.Version()));
			E4eLog.Instance.LogInfo(messageId, "[E][form].Version: {0}", new object[]
			{
				this.Version()
			});
		}

		internal override byte[] SignText(RSACryptoServiceProvider csp, byte[] data)
		{
			byte[] result;
			using (SHA256Cng sha256Cng = new SHA256Cng())
			{
				byte[] rgbHash = sha256Cng.ComputeHash(data);
				result = csp.SignHash(rgbHash, CryptoConfig.MapNameToOID("SHA256"));
			}
			return result;
		}

		private static E4eEncryptionHelperV2 instance;
	}
}
