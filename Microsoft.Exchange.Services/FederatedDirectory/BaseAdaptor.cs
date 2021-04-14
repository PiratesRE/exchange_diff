using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Office.Server.Directory;

namespace Microsoft.Exchange.FederatedDirectory
{
	internal abstract class BaseAdaptor : IDirectoryAdapter
	{
		public ICollection<PropertyType> PropertyTypes { get; protected set; }

		public ICollection<ResourceType> ResourceTypes { get; protected set; }

		public ICollection<RelationType> RelationTypes { get; protected set; }

		public Guid AdapterId { get; protected set; }

		public string ServiceName { get; protected set; }

		public NameValueCollection Parameters { get; protected set; }

		public abstract void Initialize(NameValueCollection parameters);

		public abstract void RemoveDirectoryObject(DirectoryObjectAccessor directoryObjectAccessor);

		public abstract void CommitDirectoryObject(DirectoryObjectAccessor directoryObjectAccessor);

		public abstract void LoadDirectoryObjectData(DirectoryObjectAccessor directoryObjectAccessor, RequestSchema requestSchema, out IDirectoryObjectState state);

		public IDirectorySessionContext GetDirectorySessionContext()
		{
			return null;
		}

		public IDirectoryObjectState CreateState()
		{
			return new DirectoryObjectState
			{
				IsCommitted = false,
				Version = -1L
			};
		}

		public virtual void NotifyChanges(DirectoryObjectAccessor directoryObjectAccessor)
		{
		}

		public virtual bool DirectoryObjectExists(DirectorySession directorySession, string propertyName, object propertyValue, DirectoryObjectType directoryObjectType)
		{
			throw new NotImplementedException();
		}

		public virtual bool RelationExists(DirectorySession directorySession, string relationName, Guid parentObjectId, DirectoryObjectType parentObjectObjectType, Guid targetObjectId)
		{
			throw new NotImplementedException();
		}

		public virtual bool TryRelationExists(DirectorySession directorySession, string relationName, Guid parentObjectId, DirectoryObjectType parentObjectObjectType, Guid targetObjectId, out bool relationExists)
		{
			throw new NotImplementedException();
		}

		public void LoadDirectoryObjectDataBatch(IList<DirectoryObjectAccessor> directoryObjectAccessors, RequestSchema requestSchema, out IList<IDirectoryObjectState> states)
		{
			states = new IDirectoryObjectState[directoryObjectAccessors.Count];
			for (int i = 0; i < directoryObjectAccessors.Count; i++)
			{
				IDirectoryObjectState value;
				this.LoadDirectoryObjectData(directoryObjectAccessors[i], requestSchema, out value);
				states[i] = value;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append(base.GetType().Name);
			stringBuilder.Append(": ");
			BaseAdaptor.AppendToString<PropertyType>(stringBuilder, this.PropertyTypes, "PropertyTypes");
			stringBuilder.Append(";");
			BaseAdaptor.AppendToString<ResourceType>(stringBuilder, this.ResourceTypes, "ResourceTypes");
			stringBuilder.Append(";");
			BaseAdaptor.AppendToString<RelationType>(stringBuilder, this.RelationTypes, "RelationTypes");
			return stringBuilder.ToString();
		}

		private static void AppendToString<T>(StringBuilder value, ICollection<T> schemaTypes, string name) where T : SchemaType
		{
			value.Append(name);
			value.Append("={");
			bool flag = true;
			foreach (T t in schemaTypes)
			{
				SchemaType schemaType = t;
				if (flag)
				{
					flag = false;
				}
				else
				{
					value.Append(",");
				}
				value.Append(schemaType.Name);
			}
			value.Append("}");
		}

		protected static readonly Trace Tracer = ExTraceGlobals.FederatedDirectoryTracer;
	}
}
