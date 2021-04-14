using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public abstract class SyncService : DisposeTrackableBase, IClientMessageInspector, IEndpointBehavior
	{
		public IDirectorySync SyncProxy
		{
			get
			{
				return this.SyncClient;
			}
		}

		public DirectorySyncClient SyncClient { get; private set; }

		public string RawResponse { get; private set; }

		protected SyncService()
		{
			this.InitializeSyncServiceProxy();
		}

		public DirectoryObjectsAndLinks GetMsoRawObject(SyncObjectId syncObjectId, string serviceInstanceId, bool includeBackLinks, bool includeForwardLinks, int linksResultSize, out bool? allLinksCollected)
		{
			DirectoryObjectIdentity[] array = new DirectoryObjectIdentity[]
			{
				syncObjectId.ToMsoIdentity()
			};
			DirectoryObject[] array2 = new DirectoryObject[1];
			DirectoryLink[] array3 = new DirectoryLink[0];
			DirectoryObjectError[] array4 = new DirectoryObjectError[0];
			DirectoryObjectsAndLinks directoryObjectsAndLinks = new DirectoryObjectsAndLinks
			{
				NextPageToken = null,
				More = true
			};
			byte[] msoSyncCookie = this.GetMsoSyncCookie(serviceInstanceId);
			GetDirectoryObjectsOptions? getDirectoryObjectsOptions = new GetDirectoryObjectsOptions?(GetDirectoryObjectsOptions.None);
			if (includeBackLinks)
			{
				getDirectoryObjectsOptions |= GetDirectoryObjectsOptions.IncludeBackLinks;
			}
			if (includeForwardLinks)
			{
				getDirectoryObjectsOptions |= GetDirectoryObjectsOptions.IncludeForwardLinks;
			}
			if (includeForwardLinks || includeBackLinks)
			{
				allLinksCollected = new bool?(true);
			}
			else
			{
				allLinksCollected = null;
			}
			while (directoryObjectsAndLinks.More)
			{
				GetDirectoryObjectsRequest request = new GetDirectoryObjectsRequest((directoryObjectsAndLinks.NextPageToken == null) ? msoSyncCookie : null, (directoryObjectsAndLinks.NextPageToken == null) ? array : null, (directoryObjectsAndLinks.NextPageToken == null) ? getDirectoryObjectsOptions : null, directoryObjectsAndLinks.NextPageToken);
				GetDirectoryObjectsResponse directoryObjects = this.SyncProxy.GetDirectoryObjects(request);
				if (directoryObjects.GetDirectoryObjectsResult.Objects != null && directoryObjects.GetDirectoryObjectsResult.Objects.Length != 0 && array2[0] == null)
				{
					directoryObjects.GetDirectoryObjectsResult.Objects.CopyTo(array2, 0);
				}
				if (allLinksCollected == true && directoryObjects.GetDirectoryObjectsResult.Links != null && directoryObjects.GetDirectoryObjectsResult.Links.Length != 0 && array3.Length <= linksResultSize)
				{
					if (array3.Length == linksResultSize)
					{
						allLinksCollected = new bool?(false);
					}
					else
					{
						int num = array3.Length;
						int num2 = array3.Length + directoryObjects.GetDirectoryObjectsResult.Links.Length;
						int num3 = Math.Min(linksResultSize, num2);
						if (num2 > linksResultSize)
						{
							allLinksCollected = new bool?(false);
						}
						Array.Resize<DirectoryLink>(ref array3, num3);
						Array.Copy(directoryObjects.GetDirectoryObjectsResult.Links, 0, array3, num, num3 - num);
					}
				}
				if (directoryObjects.GetDirectoryObjectsResult.Errors != null && directoryObjects.GetDirectoryObjectsResult.Errors.Length != 0)
				{
					Array.Resize<DirectoryObjectError>(ref array4, array4.Length + directoryObjects.GetDirectoryObjectsResult.Errors.Length);
					directoryObjects.GetDirectoryObjectsResult.Errors.CopyTo(array4, array4.Length - directoryObjects.GetDirectoryObjectsResult.Errors.Length);
				}
				directoryObjectsAndLinks.NextPageToken = directoryObjects.GetDirectoryObjectsResult.NextPageToken;
				directoryObjectsAndLinks.More = directoryObjects.GetDirectoryObjectsResult.More;
			}
			directoryObjectsAndLinks.Objects = ((array2 != null && array2[0] != null) ? array2 : new DirectoryObject[0]);
			directoryObjectsAndLinks.Links = array3;
			directoryObjectsAndLinks.Errors = array4;
			return directoryObjectsAndLinks;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SyncService>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.DisposeSyncServiceProxy();
			}
		}

		protected abstract DirectorySyncClient CreateService();

		private void InitializeSyncServiceProxy()
		{
			if (this.SyncClient == null)
			{
				this.SyncClient = this.CreateService();
				this.SyncClient.ChannelFactory.Endpoint.EndpointBehaviors.Add(this);
			}
		}

		private void DisposeSyncServiceProxy()
		{
			if (this.SyncClient != null)
			{
				IDisposable syncClient = this.SyncClient;
				if (syncClient != null)
				{
					syncClient.Dispose();
				}
				this.SyncClient = null;
			}
		}

		public byte[] GetNewCookieForAllObjectsTypes(string serviceInstanceId)
		{
			Type[] array = new Type[]
			{
				typeof(SyncUserSchema),
				typeof(SyncContactSchema),
				typeof(SyncGroupSchema),
				typeof(SyncForeignPrincipalSchema),
				typeof(SyncAccountSchema),
				typeof(SyncCompanySchema)
			};
			HashSet<string> hashSet = new HashSet<string>();
			HashSet<string> hashSet2 = new HashSet<string>();
			HashSet<string> hashSet3 = new HashSet<string>();
			HashSet<string> hashSet4 = new HashSet<string>();
			foreach (Type schemaType in array)
			{
				SyncObjectSchema syncObjectSchema = (SyncObjectSchema)ObjectSchema.GetInstance(schemaType);
				hashSet.Add(Enum.GetName(typeof(DirectoryObjectClass), syncObjectSchema.DirectoryObjectClass));
				ReadOnlyCollection<PropertyDefinition> allProperties = syncObjectSchema.AllProperties;
				foreach (PropertyDefinition propertyDefinition in allProperties)
				{
					SyncPropertyDefinition syncPropertyDefinition = propertyDefinition as SyncPropertyDefinition;
					if (syncPropertyDefinition != null && syncPropertyDefinition.IsForwardSync && !syncPropertyDefinition.IsNotInMsoDirectory && !string.IsNullOrEmpty(syncPropertyDefinition.MsoPropertyName))
					{
						if (syncPropertyDefinition.IsSyncLink)
						{
							hashSet3.Add(syncPropertyDefinition.MsoPropertyName);
						}
						else
						{
							hashSet2.Add(syncPropertyDefinition.MsoPropertyName);
							if (syncPropertyDefinition.IsAlwaysReturned)
							{
								hashSet4.Add(syncPropertyDefinition.MsoPropertyName);
							}
						}
					}
				}
			}
			NewCookie2Request request = new NewCookie2Request(0, serviceInstanceId, SyncOptions.None, hashSet.ToArray<string>(), hashSet2.ToArray<string>(), hashSet3.ToArray<string>(), hashSet4.ToArray<string>());
			NewCookie2Response newCookie2Response = this.SyncProxy.NewCookie2(request);
			return newCookie2Response.NewCookie2Result;
		}

		private byte[] GetMsoSyncCookie(string serviceInstanceId)
		{
			if (!this.msoSyncCookies.ContainsKey(serviceInstanceId))
			{
				this.msoSyncCookies.Add(serviceInstanceId, this.GetNewCookieForAllObjectsTypes(serviceInstanceId));
			}
			return this.msoSyncCookies[serviceInstanceId];
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			MessageBuffer messageBuffer = reply.CreateBufferedCopy(1048576);
			MemoryStream memoryStream = new MemoryStream();
			messageBuffer.WriteMessage(memoryStream);
			memoryStream.Position = 0L;
			StreamReader streamReader = new StreamReader(memoryStream);
			this.RawResponse = streamReader.ReadToEnd();
			streamReader.Close();
			memoryStream.Close();
			reply = messageBuffer.CreateMessage();
			messageBuffer.Close();
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			return null;
		}

		public void AddBindingParameters(System.ServiceModel.Description.ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.ClientMessageInspectors.Add(this);
		}

		public void ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
		}

		public void Validate(System.ServiceModel.Description.ServiceEndpoint endpoint)
		{
		}

		private const int SchemaRevision = 0;

		private readonly Dictionary<string, byte[]> msoSyncCookies = new Dictionary<string, byte[]>();
	}
}
