using System;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UserMailbox : ILocatableMailbox
	{
		public UserMailbox(UserMailboxLocator locator)
		{
			this.Locator = locator;
		}

		public IMailboxLocator Locator { get; private set; }

		public string Alias { get; set; }

		public string DisplayName { get; set; }

		public SmtpAddress SmtpAddress { get; set; }

		public string Title { get; set; }

		public string ImAddress { get; set; }

		public bool IsMember { get; set; }

		public bool IsOwner { get; set; }

		public bool ShouldEscalate { get; set; }

		public bool IsAutoSubscribed { get; set; }

		public bool IsPin { get; set; }

		public ExDateTime JoinDate { get; set; }

		public ExDateTime LastVisitedDate { get; set; }

		public ADObjectId ADObjectId { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("Identification={");
			stringBuilder.Append(this.Locator);
			stringBuilder.Append("}, Alias=");
			stringBuilder.Append(this.Alias);
			stringBuilder.Append(", DisplayName=");
			stringBuilder.Append(this.DisplayName);
			stringBuilder.Append(", SmtpAddress=");
			stringBuilder.Append(this.SmtpAddress);
			stringBuilder.Append(", Title=");
			stringBuilder.Append(this.Title);
			stringBuilder.Append(", ImAddress=");
			stringBuilder.Append(this.ImAddress);
			stringBuilder.Append(", IsMember=");
			stringBuilder.Append(this.IsMember);
			stringBuilder.Append(", IsOwner=");
			stringBuilder.Append(this.IsOwner);
			stringBuilder.Append(", ShouldEscalate=");
			stringBuilder.Append(this.ShouldEscalate);
			stringBuilder.Append(", IsAutoSubscribed=");
			stringBuilder.Append(this.IsAutoSubscribed);
			stringBuilder.Append(", IsPin=");
			stringBuilder.Append(this.IsPin);
			stringBuilder.Append(", JoinDate=");
			stringBuilder.Append(this.JoinDate);
			stringBuilder.Append(", LastVisitedDate=");
			stringBuilder.Append(this.LastVisitedDate);
			stringBuilder.Append(", ADObjectId=");
			stringBuilder.Append(this.ADObjectId);
			return stringBuilder.ToString();
		}
	}
}
