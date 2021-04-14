using System;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	public sealed class E4eProbeHelper
	{
		public static void EncryptProbeMail(Guid senderTenantId, string sender, string recipient, Stream plaintextProbeMessageStream, Stream encryptedProbeMessageStream)
		{
			OrganizationId organizationId;
			IrmProbeHelper.GetTenantOrgId(senderTenantId, out organizationId);
			IReadOnlyMailItem readOnlyMailItem = IrmProbeMailItem.CreateFromStream(plaintextProbeMessageStream);
			using (RmsEncryptor rmsEncryptor = new RmsEncryptor(organizationId, readOnlyMailItem, new Guid("81E24817-F117-4943-8959-60E1477E67B6"), null, new Guid?(senderTenantId)))
			{
				EmailMessage emailMessage;
				Exception source;
				if (rmsEncryptor.Encrypt(out emailMessage, out source))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						emailMessage.MimeDocument.WriteTo(memoryStream);
						string emailMessage2 = AntiXssEncoder.HtmlEncode(Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length), false);
						CultureInfo defaultCulture = CultureProcessor.Instance.DefaultCulture;
						MiniRecipient miniRecipient = E4eHelper.CreateMiniRecipient(sender, organizationId);
						E4eEncryptionHelper e4eEncryptionHelper = E4eHelper.GetE4eEncryptionHelper(miniRecipient);
						using (EmailMessage emailMessage3 = e4eEncryptionHelper.CreateE4eMessage(emailMessage, emailMessage2, organizationId, sender, sender, recipient, DateTimeOffset.Now.Date, defaultCulture, readOnlyMailItem.IsProbe))
						{
							Utils.StampXHeader(emailMessage3, "X-MS-Exchange-Organization-E4eMessageEncrypted", "true");
							emailMessage3.MimeDocument.WriteTo(encryptedProbeMessageStream);
						}
						goto IL_FE;
					}
				}
				ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(source);
				exceptionDispatchInfo.Throw();
				IL_FE:;
			}
		}
	}
}
