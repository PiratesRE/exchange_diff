using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class AssistantsRpcErrorCode
	{
		public static int GetHRFromException(Exception ex)
		{
			if (ex is MailboxOrDatabaseNotSpecifiedException)
			{
				return -2147220991;
			}
			if (ex is UnknownAssistantException)
			{
				return -2147220990;
			}
			if (ex is UnknownDatabaseException)
			{
				return -2147220989;
			}
			if (ex is TransientException)
			{
				return -2147220988;
			}
			return -2147220992;
		}

		public const int E_GENERIC = -2147220992;

		public const int E_MAILBOXORDATABASENOTSPECIFIED = -2147220991;

		public const int E_UNKNOWNASSISTANT = -2147220990;

		public const int E_UNKNOWNDATABASE = -2147220989;

		public const int E_TRANSIENT = -2147220988;
	}
}
