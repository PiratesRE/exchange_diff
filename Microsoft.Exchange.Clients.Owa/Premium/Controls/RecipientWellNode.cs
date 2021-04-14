using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class RecipientWellNode
	{
		internal RecipientWellNode(string displayName, string smtpAddress, string routingAddress, string routingType, string alias, AddressOrigin addressOrigin, int recipientFlags, string sipUri, string mobilePhoneNumber)
		{
			this.displayName = displayName;
			this.smtpAddress = smtpAddress;
			this.routingAddress = routingAddress;
			this.routingType = routingType;
			this.alias = alias;
			this.addressOrigin = addressOrigin;
			this.recipientFlags = recipientFlags;
			this.sipUri = sipUri;
			this.mobilePhoneNumber = mobilePhoneNumber;
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

		public string Alias
		{
			get
			{
				return this.alias;
			}
			set
			{
				this.alias = value;
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

		public string SipUri
		{
			get
			{
				return this.sipUri;
			}
			set
			{
				this.sipUri = value;
			}
		}

		public string MobilePhoneNumber
		{
			get
			{
				return this.mobilePhoneNumber;
			}
			set
			{
				this.mobilePhoneNumber = value;
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

		internal static bool Render(TextWriter writer, UserContext userContext, string displayName, string smtpAddress, string routingAddress, string routingType, string alias, AddressOrigin addressOrigin, int recipientFlags, StoreObjectId storeObjectId, EmailAddressIndex emailAddressIndex, ADObjectId adObjectId, RecipientWellNode.RenderFlags flags, string sipUri, string mobilePhoneNumber)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (string.CompareOrdinal(routingType, "SMTP") == 0 && ImceaAddress.IsImceaAddress(routingAddress) && Utilities.TryDecodeImceaAddress(routingAddress, ref routingType, ref routingAddress))
			{
				smtpAddress = null;
			}
			bool flag = (flags & RecipientWellNode.RenderFlags.ReadOnly) != RecipientWellNode.RenderFlags.None;
			bool flag2 = false;
			string text = "rwRR";
			if (string.CompareOrdinal(routingType, "MAPIPDL") != 0 && (string.IsNullOrEmpty(routingAddress) || string.IsNullOrEmpty(routingType)))
			{
				if (string.IsNullOrEmpty(displayName))
				{
					ExTraceGlobals.MailDataTracer.TraceDebug(0L, "Found recipient without an email address or display name");
					return false;
				}
				text = "rwUR";
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
			if (flag)
			{
				text = (flag2 ? "rwURO" : "rwRRO");
			}
			if ((recipientFlags & 1) != 0)
			{
				text += " rwDL";
			}
			if ((flags & RecipientWellNode.RenderFlags.RenderCommas) != RecipientWellNode.RenderFlags.None)
			{
				writer.Write(userContext.DirectionMark);
				writer.Write("; ");
			}
			if ((flags & RecipientWellNode.RenderFlags.RenderSkinnyHtml) != RecipientWellNode.RenderFlags.None)
			{
				if (!flag)
				{
					writer.Write("<span>");
				}
				writer.Write("<span class=\"");
			}
			else if (flag)
			{
				writer.Write("<span id=\"spnR\" ");
				writer.Write(" ");
				Utilities.RenderScriptHandler(writer, "oncontextmenu", "onRwCm(event);");
				writer.Write(" ");
				Utilities.RenderScriptHandler(writer, "ondblclick", "onDblClkReadRcp(event);");
				writer.Write(" ");
				Utilities.RenderScriptHandler(writer, "onclick", "onClkRcp(event);");
				writer.Write(" class=\"");
			}
			else
			{
				writer.Write("<span tabindex=\"-1\" contenteditable=\"false\">");
				if (userContext.BrowserType == BrowserType.IE)
				{
					writer.Write("<span tabindex=\"-1\" contenteditable=\"true\" id=\"spnR\" ");
				}
				else
				{
					writer.Write("<span tabindex=\"-1\" contenteditable=\"false\" id=\"spnR\" ");
				}
				writer.Write(" ");
				Utilities.RenderScriptHandler(writer, "oncontextmenu", "onContextMenuSpnRw(event);");
				writer.Write(" ");
				Utilities.RenderScriptHandler(writer, "onclick", "onClkRcp(event);");
				writer.Write(" ");
				Utilities.RenderScriptHandler(writer, "onkeydown", "onKDRcp(event);");
				writer.Write(" ");
				Utilities.RenderScriptHandler(writer, "ondblclick", "onDblClkRcp(event);");
				writer.Write(" ondrop=\"return(false);\" class=\"");
			}
			writer.Write(text);
			writer.Write("\" _ao=\"{0}\" _rf=\"{1}\" _rt=\"", (int)addressOrigin, recipientFlags);
			if (routingType != null)
			{
				Utilities.SanitizeHtmlEncode(routingType, writer);
			}
			writer.Write("\" _em=\"");
			if (routingAddress != null)
			{
				Utilities.SanitizeHtmlEncode(routingAddress, writer);
			}
			if (storeObjectId != null)
			{
				writer.Write("\" _id=\"");
				Utilities.SanitizeHtmlEncode(storeObjectId.ToBase64String(), writer);
				writer.Write("\" _ei=\"");
				writer.Write((int)emailAddressIndex);
			}
			else if (adObjectId != null)
			{
				writer.Write("\" _id=\"");
				Utilities.SanitizeHtmlEncode(Convert.ToBase64String(adObjectId.ObjectGuid.ToByteArray()), writer);
			}
			writer.Write("\" title=\"");
			if (smtpAddress != null)
			{
				Utilities.SanitizeHtmlEncode(smtpAddress, writer);
			}
			if (!flag || addressOrigin == AddressOrigin.OneOff || (addressOrigin == AddressOrigin.Directory && !userContext.IsFeatureEnabled(Feature.GlobalAddressList)))
			{
				if (smtpAddress != null)
				{
					writer.Write("\" _sa=\"");
					Utilities.SanitizeHtmlEncode(smtpAddress, writer);
				}
				else if (routingType != null && routingAddress != null)
				{
					writer.Write("\" _imcea=\"");
					Utilities.SanitizeHtmlEncode(ImceaAddress.Encode(routingType, routingAddress, OwaConfigurationManager.Configuration.DefaultAcceptedDomain.DomainName.ToString()), writer);
				}
			}
			if (userContext.IsInstantMessageEnabled() && userContext.InstantMessagingType == InstantMessagingTypeOptions.Ocs && sipUri != null)
			{
				writer.Write("\" ");
				if (adObjectId == null)
				{
					writer.Write("_sipTrsp=1 ");
				}
				writer.Write("_uri=\"");
				Utilities.SanitizeHtmlEncode(sipUri, writer);
			}
			if (userContext.IsSmsEnabled && mobilePhoneNumber != null)
			{
				writer.Write("\" _mo=\"");
				Utilities.SanitizeHtmlEncode(mobilePhoneNumber, writer);
			}
			if ((smtpAddress != null || routingAddress != null || mobilePhoneNumber != null || Utilities.IsMapiPDL(routingType)) && (!flag || addressOrigin == AddressOrigin.OneOff || (addressOrigin == AddressOrigin.Store && (!userContext.IsFeatureEnabled(Feature.Contacts) || userContext.IsSmsEnabled)) || (addressOrigin == AddressOrigin.Directory && (!userContext.IsFeatureEnabled(Feature.GlobalAddressList) || userContext.IsSmsEnabled))) && displayName != null)
			{
				writer.Write("\" _dn=\"");
				Utilities.SanitizeHtmlEncode(displayName, writer);
			}
			if (!flag && alias != null)
			{
				writer.Write("\" _al=\"");
				Utilities.SanitizeHtmlEncode(alias, writer);
			}
			writer.Write("\">");
			if (userContext.IsInstantMessageEnabled() && flag && !string.IsNullOrEmpty(routingType) && ((string.CompareOrdinal(routingType, "EX") == 0 && (recipientFlags & 1) == 0) || string.CompareOrdinal(routingType, "SMTP") == 0))
			{
				RenderingUtilities.RenderPresenceJellyBean(writer, userContext, true, "onRwCmJb(event);", false, null);
			}
			if (displayName != null)
			{
				Utilities.SanitizeHtmlEncode(displayName, writer);
			}
			RecipientWellNode.RenderFormattedAddress(writer, userContext, displayName, smtpAddress, routingAddress, routingType);
			writer.Write("</span>");
			if (!flag)
			{
				writer.Write("</span>");
			}
			return true;
		}

		public void Render(TextWriter writer, UserContext userContext, RecipientWellNode.RenderFlags flags)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			RecipientWellNode.Render(writer, userContext, this.displayName, this.smtpAddress, this.routingAddress, this.routingType, this.alias, this.addressOrigin, this.recipientFlags, this.storeObjectId, this.emailAddressIndex, this.adObjectId, flags, this.sipUri, this.mobilePhoneNumber);
		}

		internal static void RenderFormattedAddress(TextWriter writer, UserContext userContext, string displayName, string smtpAddress, string routingAddress, string routingType)
		{
			string text = null;
			if (!string.IsNullOrEmpty(smtpAddress) && !string.IsNullOrEmpty(displayName) && string.CompareOrdinal(routingType, "SMTP") == 0)
			{
				if (displayName.IndexOf(smtpAddress, StringComparison.OrdinalIgnoreCase) == -1)
				{
					text = smtpAddress;
				}
			}
			else if (!string.IsNullOrEmpty(displayName) && Utilities.IsMobileRoutingType(routingType))
			{
				if (displayName.IndexOf(routingAddress, StringComparison.OrdinalIgnoreCase) == -1)
				{
					text = routingAddress;
				}
			}
			else if (!string.IsNullOrEmpty(displayName) && Utilities.IsCustomRoutingType(routingAddress, routingType))
			{
				string text2 = routingType + ": " + routingAddress;
				if (displayName.IndexOf(text2, StringComparison.OrdinalIgnoreCase) == -1)
				{
					text = text2;
				}
			}
			if (text != null)
			{
				writer.Write(" ");
				writer.Write(userContext.DirectionMark);
				writer.Write("[");
				writer.Write(Utilities.SanitizeHtmlEncode(text));
				writer.Write("]");
				writer.Write(userContext.DirectionMark);
			}
		}

		private ADObjectId adObjectId;

		private StoreObjectId storeObjectId;

		private string alias;

		private string displayName;

		private string smtpAddress;

		private string sipUri;

		private string mobilePhoneNumber;

		private string routingAddress;

		private string routingType;

		private AddressOrigin addressOrigin;

		private int recipientFlags;

		private EmailAddressIndex emailAddressIndex;

		[Flags]
		public enum RenderFlags
		{
			None = 0,
			RenderSkinnyHtml = 1,
			RenderCommas = 2,
			ReadOnly = 4
		}
	}
}
