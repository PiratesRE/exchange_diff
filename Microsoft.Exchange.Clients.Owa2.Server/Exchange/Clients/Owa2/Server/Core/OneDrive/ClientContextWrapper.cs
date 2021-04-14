using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Sharing;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class ClientContextWrapper : IClientContext, IDisposable
	{
		public ClientContext BackingClientContext { get; private set; }

		public ICredentials Credentials
		{
			get
			{
				return this.BackingClientContext.Credentials;
			}
			set
			{
				this.BackingClientContext.Credentials = value;
			}
		}

		public bool FormDigestHandlingEnabled
		{
			get
			{
				return this.BackingClientContext.FormDigestHandlingEnabled;
			}
			set
			{
				this.BackingClientContext.FormDigestHandlingEnabled = value;
			}
		}

		public string TraceCorrelationId
		{
			get
			{
				return this.BackingClientContext.TraceCorrelationId;
			}
			set
			{
				this.BackingClientContext.TraceCorrelationId = value;
			}
		}

		public string Url
		{
			get
			{
				return this.BackingClientContext.Url;
			}
		}

		public IWeb Web
		{
			get
			{
				WebWrapper result;
				if ((result = this.web) == null)
				{
					result = (this.web = new WebWrapper(this.BackingClientContext.Web));
				}
				return result;
			}
		}

		public event EventHandler<WebRequestEventArgs> ExecutingWebRequest;

		public ClientContextWrapper(string url)
		{
			this.BackingClientContext = new ClientContext(url);
			this.BackingClientContext.ExecutingWebRequest += delegate(object sender, WebRequestEventArgs args)
			{
				EventHandler<WebRequestEventArgs> executingWebRequest = this.ExecutingWebRequest;
				if (executingWebRequest != null)
				{
					executingWebRequest(this, args);
				}
			};
		}

		public void Dispose()
		{
			this.BackingClientContext.Dispose();
		}

		public void ExecuteQuery()
		{
			this.BackingClientContext.ExecuteQuery();
		}

		public Uri WebUrlFromFolderUrlDirect(Uri folderFullUrl)
		{
			return Microsoft.SharePoint.Client.Web.WebUrlFromFolderUrlDirect(this.BackingClientContext, folderFullUrl);
		}

		public Uri WebUrlFromPageUrlDirect(Uri fileUri)
		{
			return Microsoft.SharePoint.Client.Web.WebUrlFromPageUrlDirect(this.BackingClientContext, fileUri);
		}

		public IList<IUserSharingResult> DocumentSharingManagerUpdateDocumentSharingInfo(string resourceAddress, IList<UserRoleAssignment> userRoleAssignments, bool validateExistingPermissions, bool additiveMode, bool sendServerManagedNotification, string customMessage, bool includeAnonymousLinksInNotification)
		{
			IList<UserSharingResult> list = DocumentSharingManager.UpdateDocumentSharingInfo(this.BackingClientContext, resourceAddress, userRoleAssignments, validateExistingPermissions, additiveMode, sendServerManagedNotification, customMessage, includeAnonymousLinksInNotification);
			List<IUserSharingResult> list2 = new List<IUserSharingResult>();
			foreach (UserSharingResult result in list)
			{
				list2.Add(new UserSharingResultWrapper(result));
			}
			return list2;
		}

		private WebWrapper web;
	}
}
