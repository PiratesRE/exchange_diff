using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class OriginalCallerContext
	{
		private OriginalCallerContext()
		{
		}

		public static OriginalCallerContext FromAuthZClientInfo(AuthZClientInfo authZClientInfo)
		{
			return new OriginalCallerContext
			{
				OrganizationId = authZClientInfo.GetADRecipientSessionContext().OrganizationId,
				Sid = authZClientInfo.ObjectSid,
				IdentifierString = authZClientInfo.ToCallerString()
			};
		}

		public static OriginalCallerContext Empty
		{
			get
			{
				return OriginalCallerContext.EmptyInstance;
			}
		}

		public SecurityIdentifier Sid { get; private set; }

		public string IdentifierString { get; private set; }

		public OrganizationId OrganizationId { get; private set; }

		private static readonly OriginalCallerContext EmptyInstance = new OriginalCallerContext();
	}
}
