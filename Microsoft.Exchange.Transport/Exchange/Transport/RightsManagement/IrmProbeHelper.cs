using System;
using System.IO;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	public sealed class IrmProbeHelper
	{
		public static void EncryptProbeMail(Guid recipientTenantId, Stream plaintextProbeMessageStream, Stream encryptedProbeMessageStream)
		{
			OrganizationId orgId;
			IrmProbeHelper.GetTenantOrgId(recipientTenantId, out orgId);
			IReadOnlyMailItem mailItem = IrmProbeMailItem.CreateFromStream(plaintextProbeMessageStream);
			using (RmsEncryptor rmsEncryptor = new RmsEncryptor(orgId, mailItem, new Guid("CF5CF348-A8D7-40D5-91EF-A600B88A395D"), null, new Guid?(recipientTenantId)))
			{
				EmailMessage emailMessage;
				Exception ex;
				if (!rmsEncryptor.Encrypt(out emailMessage, out ex))
				{
					throw ex;
				}
				emailMessage.MimeDocument.WriteTo(encryptedProbeMessageStream);
			}
		}

		internal static void GetTenantOrgId(Guid tenantId, out OrganizationId organizationId)
		{
			ADOperationResult adoperationResult = MultiTenantTransport.TryGetOrganizationId(tenantId, out organizationId, null, null);
			if (!adoperationResult.Succeeded)
			{
				throw adoperationResult.Exception;
			}
		}
	}
}
