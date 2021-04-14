using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Sharing;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public interface IClientContext : IDisposable
	{
		ICredentials Credentials { get; set; }

		bool FormDigestHandlingEnabled { get; set; }

		string TraceCorrelationId { get; set; }

		string Url { get; }

		IWeb Web { get; }

		event EventHandler<WebRequestEventArgs> ExecutingWebRequest;

		void ExecuteQuery();

		Uri WebUrlFromFolderUrlDirect(Uri folderFullUrl);

		Uri WebUrlFromPageUrlDirect(Uri fileUri);

		IList<IUserSharingResult> DocumentSharingManagerUpdateDocumentSharingInfo(string resourceAddress, IList<UserRoleAssignment> userRoleAssignments, bool validateExistingPermissions, bool additiveMode, bool sendServerManagedNotification, string customMessage, bool includeAnonymousLinksInNotification);
	}
}
