using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Sharing;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public class MockClientContext : IClientContext, IDisposable
	{
		public ICredentials Credentials { get; set; }

		public bool FormDigestHandlingEnabled { get; set; }

		public string TraceCorrelationId { get; set; }

		public string Url { get; private set; }

		public IWeb Web
		{
			get
			{
				MockWeb result;
				if ((result = this.web) == null)
				{
					result = (this.web = new MockWeb(this));
				}
				return result;
			}
		}

		public event EventHandler<WebRequestEventArgs> ExecutingWebRequest;

		public MockClientContext(string url)
		{
			this.Url = url;
		}

		public void ExecuteQuery()
		{
			EventHandler<WebRequestEventArgs> executingWebRequest = this.ExecutingWebRequest;
			if (executingWebRequest != null)
			{
				executingWebRequest(this, null);
			}
			this.TraceCorrelationId = Guid.NewGuid().ToString();
			if (this.mockClientObjectsToLoad != null)
			{
				foreach (MockClientObject mockClientObject in this.mockClientObjectsToLoad)
				{
					mockClientObject.LoadMockData();
				}
				this.mockClientObjectsToLoad.Clear();
			}
		}

		public Uri WebUrlFromFolderUrlDirect(Uri folderFullUrl)
		{
			return new Uri(this.Url);
		}

		public Uri WebUrlFromPageUrlDirect(Uri fileUri)
		{
			return new Uri(this.Url);
		}

		public IList<IUserSharingResult> DocumentSharingManagerUpdateDocumentSharingInfo(string resourceAddress, IList<UserRoleAssignment> userRoleAssignments, bool validateExistingPermissions, bool additiveMode, bool sendServerManagedNotification, string customMessage, bool includeAnonymousLinksInNotification)
		{
			List<IUserSharingResult> list = new List<IUserSharingResult>();
			foreach (UserRoleAssignment userRoleAssignment in userRoleAssignments)
			{
				list.Add(new MockUserSharingResult
				{
					Status = true,
					User = userRoleAssignment.UserId
				});
			}
			return list;
		}

		public void Load(MockClientObject clientObject)
		{
			if (this.mockClientObjectsToLoad == null)
			{
				this.mockClientObjectsToLoad = new List<MockClientObject>();
			}
			this.mockClientObjectsToLoad.Add(clientObject);
		}

		public void Dispose()
		{
			if (this.disposeList != null)
			{
				foreach (IDisposable disposable in this.disposeList)
				{
					disposable.Dispose();
				}
				this.disposeList.Clear();
			}
		}

		public void AddToDisposeList(IDisposable toBeDisposed)
		{
			if (this.disposeList == null)
			{
				this.disposeList = new List<IDisposable>();
			}
			this.disposeList.Add(toBeDisposed);
		}

		private const string DefaultMockAttachmentDataProviderFilePath = "C:\\MockAttachmentDataProvider";

		internal static readonly string MockAttachmentDataProviderFilePath = new StringAppSettingsEntry("MockAttachmentDataProviderFilePath", "C:\\MockAttachmentDataProvider", ExTraceGlobals.AttachmentHandlingTracer).Value;

		private MockWeb web;

		private List<MockClientObject> mockClientObjectsToLoad;

		private List<IDisposable> disposeList;
	}
}
