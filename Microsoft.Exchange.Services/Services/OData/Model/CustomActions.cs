using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Expressions;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal static class CustomActions
	{
		internal static void RegisterAction(EdmModel model, EdmEntityType entityType, EdmEntityType returnType, string actionName, IDictionary<string, IEdmTypeReference> parameters)
		{
			ArgumentValidator.ThrowIfNull("model", model);
			ArgumentValidator.ThrowIfNull("entityType", entityType);
			ArgumentValidator.ThrowIfNullOrEmpty("actionName", actionName);
			EdmAction edmAction = new EdmAction(entityType.Namespace, actionName, (returnType == null) ? null : new EdmEntityTypeReference(returnType, true), true, new EdmPathExpression("bindingParameter"));
			EdmOperationParameter edmOperationParameter = new EdmOperationParameter(edmAction, "bindingParameter", new EdmEntityTypeReference(entityType, true));
			edmAction.AddParameter(edmOperationParameter);
			if (parameters != null)
			{
				foreach (KeyValuePair<string, IEdmTypeReference> keyValuePair in parameters)
				{
					EdmOperationParameter edmOperationParameter2 = new EdmOperationParameter(edmAction, keyValuePair.Key, keyValuePair.Value);
					edmAction.AddParameter(edmOperationParameter2);
				}
			}
			model.AddElement(edmAction);
		}

		public const string Copy = "Copy";

		public const string Move = "Move";

		public const string TentativelyAccept = "TentativelyAccept";

		public const string Accept = "Accept";

		public const string Decline = "Decline";

		public const string CreateReply = "CreateReply";

		public const string CreateReplyAll = "CreateReplyAll";

		public const string CreateForward = "CreateForward";

		public const string Reply = "Reply";

		public const string ReplyAll = "ReplyAll";

		public const string Forward = "Forward";

		public const string Send = "Send";

		internal static class Parameters
		{
			public const string DestinationId = "DestinationId";

			public const string Comment = "Comment";

			public const string ToRecipients = "ToRecipients";
		}
	}
}
