using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupMailbox : ILocatableMailbox
	{
		public GroupMailbox(GroupMailboxLocator locator)
		{
			this.Locator = locator;
		}

		public IMailboxLocator Locator { get; private set; }

		public string Alias { get; set; }

		public string DisplayName { get; set; }

		public SmtpAddress SmtpAddress { get; set; }

		public string Description { get; set; }

		public ModernGroupObjectType Type { get; set; }

		public IList<ADObjectId> Owners { get; set; }

		public bool IsPinned { get; set; }

		public bool IsMember { get; set; }

		public string JoinedBy { get; set; }

		public ExDateTime JoinDate { get; set; }

		public ExDateTime PinDate { get; set; }

		public Uri SharePointUrl { get; set; }

		public string SharePointSiteUrl { get; set; }

		public string SharePointDocumentsUrl { get; set; }

		public bool RequireSenderAuthenticationEnabled { get; set; }

		public bool AutoSubscribeNewGroupMembers { get; set; }

		public CultureInfo Language { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("Locator={");
			stringBuilder.Append(this.Locator);
			stringBuilder.Append("}, Alias=");
			stringBuilder.Append(this.Alias);
			stringBuilder.Append(", DisplayName=");
			stringBuilder.Append(this.DisplayName);
			stringBuilder.Append(", SmtpAddress=");
			stringBuilder.Append(this.SmtpAddress);
			stringBuilder.Append(", Type=");
			stringBuilder.Append(this.Type);
			stringBuilder.Append(", IsMember=");
			stringBuilder.Append(this.IsMember);
			stringBuilder.Append(", JoinedBy=");
			stringBuilder.Append(this.JoinedBy);
			stringBuilder.Append(", JoinDate=");
			stringBuilder.Append(this.JoinDate);
			stringBuilder.Append(", IsPinned=");
			stringBuilder.Append(this.IsPinned);
			stringBuilder.Append(", PinDate=");
			stringBuilder.Append(this.PinDate);
			stringBuilder.Append(", SharePointUrl=");
			stringBuilder.Append(this.SharePointUrl);
			stringBuilder.Append(", SharePointSiteUrl=");
			stringBuilder.Append(this.SharePointSiteUrl);
			stringBuilder.Append(", SharePointDocumentsUrl=");
			stringBuilder.Append(this.SharePointDocumentsUrl);
			stringBuilder.Append(", RequireSenderAuthenticationEnabled=");
			stringBuilder.Append(this.RequireSenderAuthenticationEnabled);
			stringBuilder.Append(", AutoSubscribeNewGroupMembers=");
			stringBuilder.Append(this.AutoSubscribeNewGroupMembers);
			stringBuilder.Append(", Language=");
			stringBuilder.Append(this.Language);
			stringBuilder.Append(", Owners={");
			stringBuilder.Append(string.Join<ADObjectId>(",", this.Owners));
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}
	}
}
