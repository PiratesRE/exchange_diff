using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class PrintRecipientWell : RecipientWell
	{
		public PrintRecipientWell(RecipientWell delegateRecipient)
		{
			this.delegateRecipient = delegateRecipient;
		}

		public override void Render(TextWriter writer, UserContext userContext, RecipientWellType type, RecipientWell.RenderFlags flags, string id, string content, string extraStyle)
		{
			RecipientWellNode.RenderFlags renderFlags = RecipientWellNode.RenderFlags.RenderCommas;
			if ((flags & RecipientWell.RenderFlags.ReadOnly) != RecipientWell.RenderFlags.None)
			{
				renderFlags |= RecipientWellNode.RenderFlags.ReadOnly;
			}
			if (content != null)
			{
				Utilities.HtmlEncode(content, writer);
				return;
			}
			this.RenderContents(writer, userContext, type, renderFlags, new RenderRecipientWellNode(PrintRecipientWellNode.Render));
		}

		internal override void RenderContents(TextWriter writer, UserContext userContext, RecipientWellType type, RecipientWellNode.RenderFlags flags, RenderRecipientWellNode wellNode)
		{
			this.delegateRecipient.RenderContents(writer, userContext, type, flags, wellNode);
		}

		public override void Render(TextWriter writer, UserContext userContext, RecipientWellType type, RecipientWell.RenderFlags flags)
		{
			this.Render(writer, userContext, type, flags, string.Empty, null, string.Empty);
		}

		public override void Render(TextWriter writer, UserContext userContext, RecipientWellType type, RecipientWell.RenderFlags flags, string id)
		{
			this.Render(writer, userContext, type, flags, id, null, string.Empty);
		}

		public override void Render(TextWriter writer, UserContext userContext, RecipientWellType type)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.Render(writer, userContext, type, RecipientWell.RenderFlags.None);
		}

		public override bool HasRecipients(RecipientWellType type)
		{
			return this.delegateRecipient.HasRecipients(type);
		}

		private RecipientWell delegateRecipient;
	}
}
