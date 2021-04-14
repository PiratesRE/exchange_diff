using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class SharepointObject : IReadOnlyPropertyBag
	{
		internal SharepointObject(SharepointSiteId listId, SharepointSession session, XmlNode dataNode, Schema schema)
		{
			if (listId == null)
			{
				throw new ArgumentNullException();
			}
			if (session == null)
			{
				throw new ArgumentNullException();
			}
			if (dataNode == null)
			{
				throw new ArgumentNullException();
			}
			this.SharepointId = listId;
			this.Session = session;
			this.DataNode = dataNode.Clone();
			this.Schema = schema;
		}

		public static SharepointObject Read(SharepointSession session, ObjectId id)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			SharepointSiteId sharepointSiteId = id as SharepointSiteId;
			if (sharepointSiteId == null)
			{
				throw new ArgumentException("id");
			}
			if (sharepointSiteId is SharepointDocumentLibraryItemId)
			{
				return SharepointDocumentLibraryItem.Read(session, sharepointSiteId);
			}
			if (sharepointSiteId is SharepointListId)
			{
				return SharepointList.Read(session, sharepointSiteId);
			}
			throw new ObjectNotFoundException(sharepointSiteId, Strings.ExObjectNotFound(sharepointSiteId.ToString()));
		}

		public DocumentLibraryObjectId Id
		{
			get
			{
				return this.SharepointId;
			}
		}

		public abstract SharepointItemType ItemType { get; }

		public abstract string Title { get; }

		public virtual Uri Uri
		{
			get
			{
				return this.SharepointId.SiteUri;
			}
		}

		public object this[PropertyDefinition propDef]
		{
			get
			{
				object obj = this.TryGetProperty(propDef);
				if (obj is PropertyError)
				{
					throw PropertyErrorException.GetExceptionFromError((PropertyError)obj);
				}
				return obj;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		object[] IReadOnlyPropertyBag.GetProperties(ICollection<PropertyDefinition> propertyDefinitions)
		{
			if (propertyDefinitions == null)
			{
				return Array<object>.Empty;
			}
			object[] array = new object[propertyDefinitions.Count];
			int num = 0;
			foreach (PropertyDefinition propDef in propertyDefinitions)
			{
				array[num++] = this.TryGetProperty(propDef);
			}
			return array;
		}

		protected GType GetValueOrDefault<GType>(PropertyDefinition propDef)
		{
			object obj = this.TryGetProperty(propDef);
			DocumentLibraryPropertyDefinition documentLibraryPropertyDefinition = propDef as DocumentLibraryPropertyDefinition;
			if (obj is PropertyError)
			{
				obj = documentLibraryPropertyDefinition.DefaultValue;
			}
			return (GType)((object)obj);
		}

		public virtual object TryGetProperty(PropertyDefinition propDef)
		{
			DocumentLibraryPropertyDefinition documentLibraryPropertyDefinition = propDef as DocumentLibraryPropertyDefinition;
			if (documentLibraryPropertyDefinition != null && documentLibraryPropertyDefinition.PropertyId == DocumentLibraryPropertyId.Id)
			{
				return this.Id;
			}
			return SharepointHelpers.GetValuesFromCAMLView(this.Schema, this.DataNode, this.CultureInfo, new PropertyDefinition[]
			{
				propDef
			})[0];
		}

		internal readonly SharepointSiteId SharepointId;

		protected internal readonly SharepointSession Session;

		protected internal readonly XmlNode DataNode;

		protected internal readonly Schema Schema;

		protected internal CultureInfo CultureInfo;
	}
}
