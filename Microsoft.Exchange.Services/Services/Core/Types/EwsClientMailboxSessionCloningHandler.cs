using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal delegate MailboxSession EwsClientMailboxSessionCloningHandler(Guid mailboxGuid, CultureInfo userCulture, LogonType logonType, string userContextKey, ExchangePrincipal mailboxToAccess, IADOrgPerson person, ClientSecurityContext callerSecurityContext, GenericIdentity genericIdentity, bool unifiedLogon);
}
