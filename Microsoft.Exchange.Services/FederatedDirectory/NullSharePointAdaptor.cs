using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Office.Server.Directory;
using Microsoft.Office.Server.Directory.Adapter;

namespace Microsoft.Exchange.FederatedDirectory
{
	internal sealed class NullSharePointAdaptor : BaseAdaptor
	{
		public override void Initialize(NameValueCollection parameters)
		{
			base.Parameters = parameters;
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection.Add("siteDomainSubstring", "spgrid.com");
			RemoteSharePointAdapter remoteSharePointAdapter = new RemoteSharePointAdapter();
			remoteSharePointAdapter.Initialize(nameValueCollection);
			base.AdapterId = remoteSharePointAdapter.AdapterId;
			base.ServiceName = remoteSharePointAdapter.ServiceName;
			base.PropertyTypes = remoteSharePointAdapter.PropertyTypes;
			base.ResourceTypes = remoteSharePointAdapter.ResourceTypes;
			base.RelationTypes = remoteSharePointAdapter.RelationTypes;
			BaseAdaptor.Tracer.TraceDebug<NullSharePointAdaptor>((long)this.GetHashCode(), "NullSharePointAdaptor.Initialize called: schema initialized with: {0}", this);
		}

		public override void RemoveDirectoryObject(DirectoryObjectAccessor directoryObjectAccessor)
		{
			BaseAdaptor.Tracer.TraceDebug((long)this.GetHashCode(), "NullSharePointAdaptor.RemoveDirectoryObject called.");
		}

		public override void CommitDirectoryObject(DirectoryObjectAccessor directoryObjectAccessor)
		{
			BaseAdaptor.Tracer.TraceDebug((long)this.GetHashCode(), "NullSharePointAdaptor.CommitDirectoryObject called.");
			DirectoryObjectState directoryObjectState = directoryObjectAccessor.GetState(base.ServiceName) as DirectoryObjectState;
			Group group = directoryObjectAccessor.DirectoryObject as Group;
			if (group != null)
			{
				string text = "http://sharepoint/?id=" + group.Id;
				directoryObjectAccessor.SetResource("SiteUrl", text, true);
			}
			directoryObjectState.Version += 1L;
			directoryObjectState.IsCommitted = true;
		}

		public override void LoadDirectoryObjectData(DirectoryObjectAccessor directoryObjectAccessor, RequestSchema requestSchema, out IDirectoryObjectState state)
		{
			BaseAdaptor.Tracer.TraceDebug((long)this.GetHashCode(), "NullSharePointAdaptor.LoadDirectoryObjectData called.");
			if (requestSchema.Resources.Any((string requestResource) => StringComparer.OrdinalIgnoreCase.Equals(requestResource, "SiteUrl")))
			{
				string text = "http://sharepoint/?id=" + directoryObjectAccessor.DirectoryObject.Id;
				directoryObjectAccessor.SetResource("SiteUrl", text, false);
				BaseAdaptor.Tracer.TraceDebug<string>((long)this.GetHashCode(), "NullSharePointAdaptor.CommitDirectoryObject(): SiteUrl={0}", text);
			}
			state = new DirectoryObjectState
			{
				Version = 1L
			};
		}
	}
}
