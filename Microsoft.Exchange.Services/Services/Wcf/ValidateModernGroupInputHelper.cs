using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Wcf
{
	internal static class ValidateModernGroupInputHelper
	{
		public static bool IsAliasValid(IRecipientSession recipientSession, OrganizationId organizationId, string alias, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate writeError, ExchangeErrorCategory errorLoggerCategory)
		{
			return !string.IsNullOrWhiteSpace(alias) && alias.Length <= 64 && ValidateModernGroupInputHelper.ValidAliasRegex.IsMatch(alias) && RecipientTaskHelper.IsAliasUnique(recipientSession, organizationId, null, alias, logHandler, writeError, errorLoggerCategory);
		}

		public static bool IsSmtpAddressUnique(IRecipientSession recipientSession, string alias, string domain)
		{
			ADRecipient adrecipient = recipientSession.FindByProxyAddress(ProxyAddress.Parse(alias + "@" + domain));
			return adrecipient == null;
		}

		public static bool IsNameUnique(IRecipientSession recipientSession, OrganizationId organizationId, string name, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate writeError, ExchangeErrorCategory errorLoggerCategory)
		{
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			ArgumentValidator.ThrowIfNull("writeError", writeError);
			ADScope scope = null;
			if (organizationId.OrganizationalUnit != null)
			{
				scope = new ADScope(organizationId.OrganizationalUnit, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, organizationId.OrganizationalUnit));
			}
			return RecipientTaskHelper.IsPropertyValueUnique(recipientSession, scope, null, new ADPropertyDefinition[]
			{
				ADObjectSchema.Name
			}, ADObjectSchema.Name, name, false, logHandler, writeError, errorLoggerCategory, false);
		}

		private const int MaximumAliasLength = 64;

		private static readonly Regex ValidAliasRegex = new Regex("^[A-Za-z0-9-_\\.]+$", RegexOptions.Compiled);
	}
}
