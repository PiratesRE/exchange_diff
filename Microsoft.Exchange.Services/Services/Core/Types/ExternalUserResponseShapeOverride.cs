using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class ExternalUserResponseShapeOverride
	{
		public static ItemResponseShape GetAllowedResponseShape(Type serviceCommandType, ExternalUserIdAndSession externalIdAndSession, ItemResponseShape requestedResponseShape)
		{
			if (externalIdAndSession == null || requestedResponseShape == null)
			{
				ExTraceGlobals.ExternalUserTracer.TraceError(0L, "ExternalUserResponseShapeOverride.GetAllowedResponseShape: ExternalUserIdAndSession or requested response shape is null.");
				return null;
			}
			ItemResponseShape itemResponseShape = null;
			if (serviceCommandType == typeof(SyncFolderItems))
			{
				if (requestedResponseShape.BaseShape != ShapeEnum.IdOnly || (requestedResponseShape.AdditionalProperties != null && requestedResponseShape.AdditionalProperties.GetLength(0) != 0))
				{
					itemResponseShape = null;
				}
				else
				{
					itemResponseShape = new ItemResponseShape();
					itemResponseShape.BaseShape = ShapeEnum.IdOnly;
				}
			}
			else if (serviceCommandType == typeof(GetItem))
			{
				itemResponseShape = ExternalUserResponseShapeOverride.GetReponseShapeForGetItem(externalIdAndSession, requestedResponseShape);
			}
			return itemResponseShape;
		}

		private static ItemResponseShape GetReponseShapeForGetItem(ExternalUserIdAndSession externalIdAndSession, ItemResponseShape requestedResponseShape)
		{
			StoreObjectId asStoreObjectId = externalIdAndSession.GetAsStoreObjectId();
			ExternalUserResponseShape externalUserResponseShape = ExternalUserResponseShape.Create(asStoreObjectId, externalIdAndSession);
			return externalUserResponseShape.GetExternalResponseShape(requestedResponseShape);
		}
	}
}
