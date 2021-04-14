using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal static class ConsumerMailboxExtensions
	{
		public static PrimaryMailboxSourceType PrimaryMailboxSource(this ADUser user)
		{
			if (user[ADUserSchema.PrimaryMailboxSource] == null)
			{
				return PrimaryMailboxSourceType.None;
			}
			return (PrimaryMailboxSourceType)user[ADUserSchema.PrimaryMailboxSource];
		}

		public static string SatchmoDGroup(this ADUser user)
		{
			if (user[ADUserSchema.SatchmoDGroup] == null)
			{
				return null;
			}
			return (string)user[ADUserSchema.SatchmoDGroup];
		}

		public static IPAddress SatchmoClusterIp(this ADUser user)
		{
			if (user[ADUserSchema.SatchmoClusterIp] == null)
			{
				return null;
			}
			return (IPAddress)user[ADUserSchema.SatchmoClusterIp];
		}
	}
}
