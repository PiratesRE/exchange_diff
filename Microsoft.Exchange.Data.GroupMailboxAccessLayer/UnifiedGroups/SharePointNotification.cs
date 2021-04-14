using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Directory.Provider;

namespace Microsoft.Exchange.UnifiedGroups
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SharePointNotification : IDisposeTrackable, IDisposable
	{
		public SharePointNotification(SharePointNotification.NotificationType notificationType, string externalDirectoryObjectId, OrganizationId organizationId, ICredentials actAsUserCredentials, Guid activityId)
		{
			this.notificationType = notificationType;
			this.externalDirectoryObjectId = externalDirectoryObjectId;
			this.organizationId = organizationId;
			this.actAsUserCredentials = actAsUserCredentials;
			this.activityId = activityId;
			this.directoryObjectData = this.CreateDirectoryObjectData();
			this.disposeTracker = this.GetDisposeTracker();
		}

		private MemoryStream HelperStream
		{
			get
			{
				if (this.helperStream == null)
				{
					this.helperStream = new MemoryStream(4096);
				}
				return this.helperStream;
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SharePointNotification>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			if (this.helperStream != null)
			{
				this.helperStream.Dispose();
				this.helperStream = null;
			}
		}

		public void Execute()
		{
			ClientRuntimeContext tenantClientContext = this.GetTenantClientContext();
			if (tenantClientContext == null)
			{
				SharePointNotification.Tracer.TraceDebug((long)this.GetHashCode(), "No ClientContext, skipping call to SharePoint");
				return;
			}
			using (tenantClientContext)
			{
				SharePointDirectoryProvider sharePointDirectoryProvider = new SharePointDirectoryProvider(tenantClientContext);
				if (this.notificationType == SharePointNotification.NotificationType.Create)
				{
					this.directoryObjectData.Version = -1L;
					sharePointDirectoryProvider.CreateDirectoryObject(this.directoryObjectData);
				}
				else if (this.notificationType == SharePointNotification.NotificationType.Update)
				{
					sharePointDirectoryProvider.NotifyDataChanges(this.directoryObjectData);
				}
				else if (this.notificationType == SharePointNotification.NotificationType.Delete)
				{
					sharePointDirectoryProvider.DeleteDirectoryObject(this.directoryObjectData);
				}
				tenantClientContext.RequestTimeout = 180000;
				tenantClientContext.ExecuteQuery();
			}
		}

		public void AddOwners(params string[] objectIds)
		{
			this.AddRelations("Owners", objectIds);
		}

		public void RemoveOwners(params string[] objectIds)
		{
			this.RemoveRelations("Owners", objectIds);
		}

		public void AddMembers(params string[] objectIds)
		{
			this.AddRelations("Members", objectIds);
		}

		public void RemoveMembers(params string[] objectIds)
		{
			this.RemoveRelations("Members", objectIds);
		}

		public void SetAllowAccessTo(bool isPublic)
		{
			if (isPublic)
			{
				this.AddRelations("AllowAccessTo", new string[]
				{
					SharePointNotification.EveryoneGroupId.ToString()
				});
				return;
			}
			this.AddRelations("AllowAccessTo", null);
		}

		public void SetPropertyValue(string name, object value, bool delayLoad = false)
		{
			PropertyData propertyData = new PropertyData();
			propertyData.Name = name;
			propertyData.DelayLoad = delayLoad;
			propertyData.Value = this.SerializeObjectBinary(value);
			this.directoryObjectData.Properties[name] = propertyData;
		}

		public void SetResourceValue(string name, object value, bool delayLoad = false)
		{
			ResourceData resourceData = new ResourceData();
			resourceData.Name = name;
			resourceData.DelayLoad = delayLoad;
			resourceData.Value = this.SerializeObjectBinary(value);
			this.directoryObjectData.Resources[name] = resourceData;
		}

		private static Guid GetTenantContextId(OrganizationId organizationId)
		{
			if (organizationId == OrganizationId.ForestWideOrgId)
			{
				return Guid.Empty;
			}
			return new Guid(organizationId.ToExternalDirectoryOrganizationId());
		}

		private DirectoryObjectData CreateDirectoryObjectData()
		{
			DirectoryObjectData directoryObjectData = new DirectoryObjectData
			{
				TenantContextId = SharePointNotification.GetTenantContextId(this.organizationId),
				Id = new Guid(this.externalDirectoryObjectId),
				DirectoryObjectType = 2
			};
			directoryObjectData.RelationSets = new Dictionary<string, RelationSetData>();
			directoryObjectData.Properties = new Dictionary<string, PropertyData>();
			directoryObjectData.Resources = new Dictionary<string, ResourceData>();
			directoryObjectData.States = new Dictionary<string, StateData>();
			return directoryObjectData;
		}

		private RelationSetData CreateRelationSetData(string name)
		{
			return new RelationSetData
			{
				AddedRelations = new Dictionary<string, RelationData>(),
				Relations = new Dictionary<string, RelationData>(),
				RemovedRelations = new Dictionary<string, RelationData>(),
				Name = name,
				DelayLoad = true
			};
		}

		private ClientContext GetTenantClientContext()
		{
			Uri rootSiteUrl = SharePointUrl.GetRootSiteUrl(this.organizationId);
			if (rootSiteUrl == null)
			{
				return null;
			}
			ClientContext clientContext = new ClientContext(rootSiteUrl);
			clientContext.ExecutingWebRequest += delegate(object sender, WebRequestEventArgs request)
			{
				request.WebRequestExecutor.RequestHeaders.Add(HttpRequestHeader.Authorization, "Bearer");
				request.WebRequestExecutor.RequestHeaders.Add("SPResponseGuid", this.activityId.ToString());
				request.WebRequestExecutor.WebRequest.PreAuthenticate = true;
			};
			clientContext.Credentials = this.actAsUserCredentials;
			return clientContext;
		}

		private void AddRelations(string name, params string[] objectIds)
		{
			RelationSetData relationSetData = this.CreateRelationSetData(name);
			if (objectIds != null)
			{
				foreach (string text in objectIds)
				{
					Relation value = new Relation(SharePointNotification.RelationTypeIds[name], new Guid(text));
					RelationData relationData = new RelationData();
					relationData.Value = this.SerializeObjectXML<Relation>(value);
					relationSetData.AddedRelations[text] = relationData;
				}
			}
			this.directoryObjectData.RelationSets[name] = relationSetData;
		}

		private void RemoveRelations(string name, params string[] objectIds)
		{
			RelationSetData relationSetData = this.CreateRelationSetData(name);
			if (objectIds != null)
			{
				foreach (string text in objectIds)
				{
					Relation value = new Relation(SharePointNotification.RelationTypeIds[name], new Guid(text));
					RelationData relationData = new RelationData();
					relationData.Value = this.SerializeObjectXML<Relation>(value);
					relationSetData.RemovedRelations[text] = relationData;
				}
			}
			this.directoryObjectData.RelationSets[name] = relationSetData;
		}

		private byte[] SerializeObjectXML<T>(T value) where T : class
		{
			DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
			dataContractSerializer.WriteObject(this.HelperStream, value);
			byte[] array = new byte[this.HelperStream.Length];
			Array.Copy(this.HelperStream.GetBuffer(), array, array.Length);
			this.ResetHelperStream();
			return array;
		}

		private byte[] SerializeObjectBinary(object value)
		{
			if (value == null)
			{
				return null;
			}
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			binaryFormatter.Serialize(this.HelperStream, value);
			byte[] array = new byte[this.HelperStream.Length];
			Array.Copy(this.HelperStream.GetBuffer(), array, array.Length);
			this.ResetHelperStream();
			return array;
		}

		private void ResetHelperStream()
		{
			this.HelperStream.Seek(0L, SeekOrigin.Begin);
			this.HelperStream.SetLength(0L);
		}

		private const string OwnersRelationName = "Owners";

		private const string MembersRelationName = "Members";

		private const string AllowAccessToRelationName = "AllowAccessTo";

		private const string SPResponseGuidHeader = "SPResponseGuid";

		public static readonly Guid EveryoneGroupId = new Guid("C41554C4-1734-4462-9544-5E5542F2EB1C");

		internal static readonly Trace Tracer = ExTraceGlobals.ModernGroupsTracer;

		private static readonly Dictionary<string, Guid> RelationTypeIds = new Dictionary<string, Guid>
		{
			{
				"Owners",
				new Guid("cf58be5f-dd0f-4321-982e-72deaccca8a4")
			},
			{
				"Members",
				new Guid("3d420ade-03d2-493c-831b-adebde7b9702")
			},
			{
				"AllowAccessTo",
				new Guid("1E87A52F-0324-46F8-9170-F66EEFDEFC7D")
			}
		};

		private readonly string externalDirectoryObjectId;

		private readonly Guid activityId;

		private DirectoryObjectData directoryObjectData;

		private SharePointNotification.NotificationType notificationType;

		private OrganizationId organizationId;

		private ICredentials actAsUserCredentials;

		private MemoryStream helperStream;

		private DisposeTracker disposeTracker;

		public enum NotificationType
		{
			Create,
			Update,
			Delete
		}
	}
}
