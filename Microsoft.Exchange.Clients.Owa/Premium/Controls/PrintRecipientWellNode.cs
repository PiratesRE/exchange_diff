using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class PrintRecipientWellNode : RecipientWellNode
	{
		internal PrintRecipientWellNode(string displayName, string smtpAddress, string routingAddress, string routingType, string alias, AddressOrigin addressOrigin, int recipientFlags) : base(displayName, smtpAddress, routingAddress, routingType, alias, addressOrigin, recipientFlags, null, null)
		{
		}

		internal new static bool Render(TextWriter writer, UserContext userContext, string displayName, string smtpAddress, string routingAddress, string routingType, string alias, AddressOrigin addressOrigin, int recipientFlags, StoreObjectId storeObjectId, EmailAddressIndex emailAddressIndex, ADObjectId adObjectId, RecipientWellNode.RenderFlags flags, string sipUri, string mobilePhoneNumber)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (string.IsNullOrEmpty(routingAddress) || string.IsNullOrEmpty(routingType))
			{
				if (string.IsNullOrEmpty(displayName))
				{
					ExTraceGlobals.CoreTracer.TraceDebug(0L, "Found recipient without an email address or display name");
					return false;
				}
				routingAddress = null;
				smtpAddress = null;
				routingType = null;
			}
			else if (string.IsNullOrEmpty(displayName))
			{
				if (!string.IsNullOrEmpty(smtpAddress))
				{
					displayName = smtpAddress;
				}
				else if (!string.IsNullOrEmpty(routingAddress))
				{
					displayName = routingAddress;
				}
			}
			if ((flags & RecipientWellNode.RenderFlags.RenderCommas) != RecipientWellNode.RenderFlags.None)
			{
				writer.Write("; ");
			}
			if (displayName != null)
			{
				Utilities.HtmlEncode(displayName.Trim(new char[]
				{
					'\''
				}), writer);
			}
			RecipientWellNode.RenderFormattedAddress(writer, userContext, displayName, smtpAddress, routingAddress, routingType);
			return true;
		}
	}
}
