using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class ExternalUserResponseShape
	{
		protected abstract List<PropertyPath> PropertiesAllowedForReadAccess { get; }

		protected Permission Permissions { get; set; }

		protected static PropertyPath[] GetAllowedProperties(ResponseShape requestedShape, List<PropertyPath> allowedPropertyList)
		{
			if (allowedPropertyList == null)
			{
				return null;
			}
			if (requestedShape.BaseShape != ShapeEnum.IdOnly)
			{
				return allowedPropertyList.ToArray();
			}
			if (requestedShape.AdditionalProperties == null)
			{
				return null;
			}
			List<PropertyPath> list = new List<PropertyPath>();
			foreach (PropertyPath item in requestedShape.AdditionalProperties)
			{
				if (allowedPropertyList.Contains(item))
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		protected virtual PropertyPath[] GetPropertiesForCustomPermissions(ItemResponseShape requestedResponseShape)
		{
			return null;
		}

		public static ExternalUserResponseShape Create(StoreObjectId storeObjectId, ExternalUserIdAndSession externalUserIdAndSession)
		{
			StoreObjectType objectType = storeObjectId.ObjectType;
			switch (objectType)
			{
			case StoreObjectType.CalendarFolder:
				ExTraceGlobals.ExternalUserTracer.TraceDebug(0L, "ExternalUserResponseShape.Create: Defining response shape for calendar folder.");
				return new ExternalUserCalendarFolderResponseShape();
			case StoreObjectType.ContactsFolder:
				ExTraceGlobals.ExternalUserTracer.TraceDebug(0L, "ExternalUserResponseShape.Create: Defining response shape for contacts folder.");
				return new ExternalUserContactsFolderResponseShape();
			default:
				switch (objectType)
				{
				case StoreObjectType.CalendarItem:
				case StoreObjectType.CalendarItemOccurrence:
					ExTraceGlobals.ExternalUserTracer.TraceDebug(0L, "ExternalUserResponseShape.Create: Defining response shape for calendar item.");
					return new ExternalUserCalendarResponseShape(externalUserIdAndSession.PermissionGranted);
				case StoreObjectType.Contact:
					ExTraceGlobals.ExternalUserTracer.TraceDebug(0L, "ExternalUserResponseShape.Create: Defining response shape for contact item.");
					return new ExternalUserContactResponseShape(externalUserIdAndSession.PermissionGranted);
				default:
					ExTraceGlobals.ExternalUserTracer.TraceDebug(0L, "ExternalUserResponseShape.Create: Defining response shape for unknown item.");
					return new ExternalUserUnknownResponseShape();
				}
				break;
			}
		}

		public ItemResponseShape GetExternalResponseShape(ItemResponseShape requestedResponseShape)
		{
			ItemResponseShape itemResponseShape = new ItemResponseShape();
			itemResponseShape.BaseShape = ShapeEnum.IdOnly;
			if (requestedResponseShape.BaseShape == ShapeEnum.IdOnly && requestedResponseShape.AdditionalProperties == null)
			{
				return itemResponseShape;
			}
			itemResponseShape.BodyType = requestedResponseShape.BodyType;
			itemResponseShape.UniqueBodyType = requestedResponseShape.UniqueBodyType;
			itemResponseShape.NormalizedBodyType = requestedResponseShape.NormalizedBodyType;
			if (this.Permissions != null)
			{
				if (this.Permissions.CanReadItems)
				{
					ExTraceGlobals.ExternalUserTracer.TraceDebug<ExternalUserResponseShape>((long)this.GetHashCode(), "{0}: Overriding shape for Read permissions.", this);
					itemResponseShape.AdditionalProperties = ExternalUserResponseShape.GetAllowedProperties(requestedResponseShape, this.PropertiesAllowedForReadAccess);
				}
				else
				{
					if (this.Permissions.PermissionLevel != PermissionLevel.Custom)
					{
						return null;
					}
					ExTraceGlobals.ExternalUserTracer.TraceDebug<ExternalUserResponseShape>((long)this.GetHashCode(), "{0}: Overriding shape for Custom permissions.", this);
					itemResponseShape.AdditionalProperties = this.GetPropertiesForCustomPermissions(requestedResponseShape);
				}
			}
			return itemResponseShape;
		}

		public PropertyPath[] GetAllowedProperties(FolderResponseShape requestedResponseShape)
		{
			return ExternalUserResponseShape.GetAllowedProperties(requestedResponseShape, this.PropertiesAllowedForReadAccess);
		}
	}
}
