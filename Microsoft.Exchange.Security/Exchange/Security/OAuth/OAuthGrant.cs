using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Exchange.Security.OAuth
{
	public static class OAuthGrant
	{
		public static string[] ExtractKnownGrants(string scope)
		{
			if (string.IsNullOrEmpty(scope))
			{
				return new string[0];
			}
			return (from s in scope.Split(new char[]
			{
				' '
			}, StringSplitOptions.RemoveEmptyEntries)
			where OAuthGrant.knownGrants.Contains(s)
			select s).ToArray<string>();
		}

		public static string[] ExtractKnownGrantsFromRole(string role)
		{
			if (string.IsNullOrEmpty(role))
			{
				return new string[0];
			}
			return (from s in role.Split(new char[]
			{
				' '
			}, StringSplitOptions.RemoveEmptyEntries)
			where OAuthGrant.knownGrants.Contains(s) || string.Equals(s, Constants.ClaimValues.FullAccess, StringComparison.OrdinalIgnoreCase)
			select s).ToArray<string>();
		}

		public static string[] KnownGrants
		{
			get
			{
				return OAuthGrant.knownGrants;
			}
		}

		public const string UserImpersonation = "user_impersonation";

		public const string MailRead = "Mail.Read";

		public const string MailReadWrite = "Mail.Write";

		public const string MailSend = "Mail.Send";

		public const string CalendarsRead = "Calendars.Read";

		public const string CalendarsReadWrite = "Calendars.Write";

		public const string ContactsRead = "Contacts.Read";

		public const string ContactsReadWrite = "Contacts.Write";

		public const string EasAccessAsUserAll = "EAS.AccessAsUser.All";

		public const string EwsAccessAsUserAll = "EWS.AccessAsUser.All";

		private static readonly string[] knownGrants = (from fieldInfo in typeof(OAuthGrant).GetFields(BindingFlags.Static | BindingFlags.Public)
		select fieldInfo.GetValue(null)).Cast<string>().ToArray<string>();
	}
}
