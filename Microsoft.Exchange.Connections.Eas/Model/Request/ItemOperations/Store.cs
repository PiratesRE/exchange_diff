using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.ItemOperations
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public static class Store
	{
		public static string Mailbox
		{
			get
			{
				return "Mailbox";
			}
		}

		public static string DocumentLibrary
		{
			get
			{
				return "Document Library";
			}
		}
	}
}
