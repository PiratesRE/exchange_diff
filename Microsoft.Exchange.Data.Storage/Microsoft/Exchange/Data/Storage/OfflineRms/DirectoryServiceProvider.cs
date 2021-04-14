using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;
using Microsoft.RightsManagementServices.Core;
using Microsoft.RightsManagementServices.Provider;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DirectoryServiceProvider : IDirectoryServiceProvider
	{
		public DirectoryServiceProvider()
		{
		}

		public DirectoryServiceProvider(RmsClientManagerContext clientContext)
		{
			if (clientContext == null)
			{
				throw new ArgumentNullException("clientContext");
			}
			this.clientContext = clientContext;
		}

		public bool EvaluateIdentity(string userAddress, string[] targetAddresses, out string matchedAddress)
		{
			if (string.IsNullOrEmpty(userAddress))
			{
				throw new ArgumentNullException("userAddress");
			}
			if (targetAddresses == null)
			{
				throw new ArgumentNullException("targetAddresses");
			}
			matchedAddress = null;
			string recipientAddress = DirectoryServiceProvider.RmsIdentityAddressToSmtpAddress(userAddress);
			try
			{
				foreach (string text in targetAddresses)
				{
					if (!string.IsNullOrEmpty(text))
					{
						string groupAddress = DirectoryServiceProvider.RmsIdentityAddressToSmtpAddress(text);
						if (ServerManager.IsMemberOf(this.clientContext, recipientAddress, groupAddress))
						{
							matchedAddress = text;
							return true;
						}
					}
				}
			}
			catch (TransientException ex)
			{
				throw new DirectoryServiceProviderException(false, "Failed to look up " + userAddress, ex);
			}
			catch (ADOperationException ex2)
			{
				throw new DirectoryServiceProviderException(false, "Failed to look up " + userAddress, ex2);
			}
			return false;
		}

		public bool UserMatchesAnyoneIdentity(string userAddress)
		{
			if (string.IsNullOrEmpty(userAddress))
			{
				throw new ArgumentNullException("userAddress");
			}
			return DirectoryServiceProvider.IsUserMemberOfTenant(DirectoryServiceProvider.RmsIdentityAddressToSmtpAddress(userAddress), this.clientContext.OrgId);
		}

		private static bool IsUserMemberOfTenant(string userAddress, OrganizationId organizationId)
		{
			bool result;
			try
			{
				result = ServerManager.IsUserMemberOfTenant(userAddress, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId));
			}
			catch (NonUniqueRecipientException ex)
			{
				throw new DirectoryServiceProviderException(false, "User email address is not unique: " + userAddress, ex);
			}
			return result;
		}

		private static string RmsIdentityAddressToSmtpAddress(string rmsAddress)
		{
			rmsAddress = rmsAddress.Trim();
			if (rmsAddress.IndexOf("mail=", StringComparison.OrdinalIgnoreCase) == 0)
			{
				rmsAddress = rmsAddress.Substring("mail=".Length);
			}
			return rmsAddress.Trim();
		}

		private readonly RmsClientManagerContext clientContext;
	}
}
