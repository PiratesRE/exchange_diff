using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class RecipientWellNode
	{
		internal RecipientWellNode(string displayName, string smtpAddress, string routingAddress, string routingType, AddressOrigin addressOrigin, int recipientFlags)
		{
			this.displayName = displayName;
			this.smtpAddress = smtpAddress;
			this.routingAddress = routingAddress;
			this.routingType = routingType;
			this.addressOrigin = addressOrigin;
			this.recipientFlags = recipientFlags;
		}

		internal RecipientWellNode(string displayName, string smtpAddress, AddressOrigin addressOrigin)
		{
			this.displayName = displayName;
			this.smtpAddress = smtpAddress;
			this.addressOrigin = addressOrigin;
		}

		internal RecipientWellNode(string displayName, string smtpAddress)
		{
			this.displayName = displayName;
			this.smtpAddress = smtpAddress;
			this.addressOrigin = AddressOrigin.Unknown;
		}

		internal AddressOrigin AddressOrigin
		{
			get
			{
				return this.addressOrigin;
			}
			set
			{
				this.addressOrigin = value;
			}
		}

		public int RecipientFlags
		{
			get
			{
				return this.recipientFlags;
			}
			set
			{
				this.recipientFlags = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		public string SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
			set
			{
				this.smtpAddress = value;
			}
		}

		public string RoutingAddress
		{
			get
			{
				return this.routingAddress;
			}
			set
			{
				this.routingAddress = value;
			}
		}

		public string RoutingType
		{
			get
			{
				return this.routingType;
			}
			set
			{
				this.routingType = value;
			}
		}

		internal StoreObjectId StoreObjectId
		{
			get
			{
				return this.storeObjectId;
			}
			set
			{
				this.storeObjectId = value;
			}
		}

		public ADObjectId ADObjectId
		{
			get
			{
				return this.adObjectId;
			}
			set
			{
				this.adObjectId = value;
			}
		}

		internal EmailAddressIndex EmailAddressIndex
		{
			get
			{
				return this.emailAddressIndex;
			}
			set
			{
				this.emailAddressIndex = value;
			}
		}

		internal static bool Render(UserContext userContext, TextWriter writer, string displayName, string smtpAddress, string routingAddress, string routingType, AddressOrigin addressOrigin, int recipientFlags, int readItemType, RecipientItemType recipientWell, ADObjectId adObjectId, StoreObjectId storeObjectId, RecipientWellNode.RenderFlags flags, string idString, bool isWebPartRequest)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			bool flag = (flags & RecipientWellNode.RenderFlags.ReadOnly) != RecipientWellNode.RenderFlags.None;
			bool flag2 = false;
			if (string.CompareOrdinal(routingType, "MAPIPDL") != 0 && (string.IsNullOrEmpty(routingAddress) || string.IsNullOrEmpty(routingType)))
			{
				if (string.IsNullOrEmpty(displayName))
				{
					ExTraceGlobals.MailDataTracer.TraceDebug(0L, "Found recipient without an email address or display name");
					return false;
				}
				flag2 = true;
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
			writer.Write("<span class=\"");
			if (flag2)
			{
				writer.Write("rwURO\">");
				if (displayName != null)
				{
					Utilities.HtmlEncode(displayName, writer);
				}
			}
			else
			{
				writer.Write("rwRO");
				if ((recipientFlags & 1) != 0)
				{
					writer.Write(" rwDL");
				}
				writer.Write("\">");
			}
			bool flag3 = false;
			if (!string.IsNullOrEmpty(idString))
			{
				if ((userContext.IsFeatureEnabled(Feature.Contacts) && storeObjectId != null) || adObjectId != null)
				{
					writer.Write("<a href=\"#\"");
					if (storeObjectId != null)
					{
						writer.Write(" id=\"");
						Utilities.HtmlEncode(storeObjectId.ToBase64String(), writer);
					}
					else if (adObjectId != null)
					{
						writer.Write(" id=\"");
						Utilities.HtmlEncode(Utilities.GetBase64StringFromADObjectId(adObjectId), writer);
					}
					writer.Write("\" onClick=\"return onClkRcpt(this,{0}", readItemType);
					if (!flag)
					{
						writer.Write(",{0}", (int)recipientWell);
					}
					writer.Write(");\">");
					flag3 = true;
				}
				else if (routingType == "SMTP" && !string.IsNullOrEmpty(smtpAddress) && flag && !isWebPartRequest)
				{
					writer.Write("<a href=\"");
					Utilities.HtmlEncode("?ae=Item&t=IPM.Note&a=New&to=", writer);
					Utilities.HtmlEncode(Utilities.UrlEncode(smtpAddress), writer);
					if (!string.IsNullOrEmpty(displayName))
					{
						Utilities.HtmlEncode("&nm=", writer);
						Utilities.HtmlEncode(Utilities.UrlEncode(displayName), writer);
					}
					writer.Write("\" class=\"emadr\">");
					flag3 = true;
				}
			}
			if (!flag2)
			{
				if (displayName != null)
				{
					Utilities.HtmlEncode(displayName, writer);
				}
				RecipientWellNode.RenderFormattedAddress(writer, displayName, smtpAddress, routingAddress, routingType);
			}
			if (flag3)
			{
				writer.Write("</a>");
			}
			if (!flag && !string.IsNullOrEmpty(idString))
			{
				writer.Write(" <span class=\"sq\">[<a href=\"#\" onClick=\"return onClkRmRcp('");
				writer.Write(idString);
				writer.Write("')\">");
				writer.Write(LocalizedStrings.GetHtmlEncoded(341226654));
				writer.Write("</a>]</span>");
			}
			writer.Write("</span>");
			return true;
		}

		internal static void RenderFormattedAddress(TextWriter writer, string displayName, string smtpAddress, string routingAddress, string routingType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(routingType) && !string.IsNullOrEmpty(smtpAddress) && string.CompareOrdinal(routingType, "SMTP") == 0 && displayName.IndexOf(smtpAddress, StringComparison.OrdinalIgnoreCase) == -1)
			{
				stringBuilder.Append(" [");
				stringBuilder.Append(Utilities.HtmlEncode(smtpAddress));
				stringBuilder.Append("]");
			}
			else if (!string.IsNullOrEmpty(displayName) && Utilities.IsCustomRoutingType(routingAddress, routingType))
			{
				string text = routingType + ": " + routingAddress;
				if (displayName.IndexOf(text, StringComparison.OrdinalIgnoreCase) == -1)
				{
					stringBuilder.Append(" [");
					stringBuilder.Append(Utilities.HtmlEncode(text));
					stringBuilder.Append("]");
				}
			}
			if (!string.IsNullOrEmpty(stringBuilder.ToString()))
			{
				writer.Write(stringBuilder);
			}
		}

		private ADObjectId adObjectId;

		private StoreObjectId storeObjectId;

		private string displayName;

		private string smtpAddress;

		private string routingAddress;

		private string routingType;

		private AddressOrigin addressOrigin;

		private int recipientFlags;

		private EmailAddressIndex emailAddressIndex;

		[Flags]
		public enum RenderFlags
		{
			None = 0,
			RenderCommas = 2,
			ReadOnly = 4
		}
	}
}
