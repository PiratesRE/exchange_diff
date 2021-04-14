using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("MessageAnnotation")]
	internal sealed class MessageAnnotationEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(MessageAnnotationEventHandler));
		}

		[OwaEvent("GetMessageAnnotationInternals")]
		[OwaEventParameter("sId", typeof(string))]
		public void GetMessageAnnotationInternals()
		{
			string idString = (string)base.GetParameter("sId");
			string divErrorId = "divFPErr";
			this.Writer.Write("<div id=\"divFPTrR\">");
			Infobar infobar = new Infobar(divErrorId, "infobar");
			infobar.Render(this.Writer);
			MessageAnnotationHost.RenderMessageAnnotationDivStart(this.Writer, "msgnotediv");
			string messageNoteText = string.Empty;
			PropertyDefinition[] prefetchProperties = new PropertyDefinition[]
			{
				MessageItemSchema.MessageAnnotation
			};
			using (MessageItem item = Utilities.GetItem<MessageItem>(base.UserContext, idString, prefetchProperties))
			{
				object obj = item.TryGetProperty(MessageItemSchema.MessageAnnotation);
				if (obj is string)
				{
					messageNoteText = (obj as string);
				}
			}
			MessageAnnotationHost.RenderMessageAnnotationControl(this.Writer, "msgnotectrl", messageNoteText);
			MessageAnnotationHost.RenderMessageAnnotationDivEnd(this.Writer);
			this.Writer.Write("</div>");
		}

		[OwaEventParameter("sId", typeof(string))]
		[OwaEvent("SaveMessageAnnotation")]
		[OwaEventParameter("svMsgAnnotation", typeof(string))]
		public void SaveMessageAnnotation()
		{
			PropertyDefinition[] propertyDefinitions = new PropertyDefinition[]
			{
				MessageItemSchema.MessageAnnotation
			};
			string idString = (string)base.GetParameter("sId");
			string text = (string)base.GetParameter("svMsgAnnotation");
			using (MessageItem item = Utilities.GetItem<MessageItem>(base.UserContext, idString, new PropertyDefinition[]
			{
				MessageItemSchema.MessageAnnotation
			}))
			{
				item.OpenAsReadWrite();
				item.SetProperties(propertyDefinitions, new object[]
				{
					text
				});
				item.Save(SaveMode.NoConflictResolutionForceSave);
			}
		}

		public const string EventNamespace = "MessageAnnotation";

		public const string MethodGetMessageAnnotationInternals = "GetMessageAnnotationInternals";

		public const string MethodSaveMessageAnnotation = "SaveMessageAnnotation";

		public const string StoreObjectId = "sId";

		public const string MessageAnnotation = "svMsgAnnotation";
	}
}
