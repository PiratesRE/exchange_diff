using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("ReadPost")]
	internal sealed class ReadPostEventHandler : ItemEventHandler
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(ReadPostEventHandler));
		}

		[OwaEvent("Save")]
		[OwaEventParameter("Subj", typeof(string))]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEventParameter("CK", typeof(string))]
		public void Save()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ReadPostEventHandler.Savepost");
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			string changeKey = (string)base.GetParameter("CK");
			using (PostItem item = Utilities.GetItem<PostItem>(base.UserContext, owaStoreObjectId, changeKey, false, new PropertyDefinition[0]))
			{
				ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "Saving post. ");
				object parameter = base.GetParameter("Subj");
				if (parameter != null)
				{
					try
					{
						item.Subject = (string)parameter;
					}
					catch (PropertyValidationException ex)
					{
						throw new OwaInvalidRequestException(ex.Message);
					}
				}
				Utilities.SaveItem(item, true);
				item.Load();
				this.Writer.Write("<div id=ck>");
				this.Writer.Write(item.Id.ChangeKeyAsBase64String());
				this.Writer.Write("</div>");
			}
		}

		public const string EventNamespace = "ReadPost";

		public const string Subject = "Subj";
	}
}
