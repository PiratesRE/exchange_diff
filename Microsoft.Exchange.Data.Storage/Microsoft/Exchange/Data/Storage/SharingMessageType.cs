using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SharingMessageType
	{
		private SharingMessageType(string name, SharingFlavor sharingFlavor)
		{
			this.name = name;
			this.sharingFlavor = sharingFlavor;
		}

		public bool IsRequest
		{
			get
			{
				return this == SharingMessageType.Request || this == SharingMessageType.InvitationAndRequest;
			}
		}

		public bool IsResponseToRequest
		{
			get
			{
				return this == SharingMessageType.AcceptOfRequest || this == SharingMessageType.DenyOfRequest;
			}
		}

		public bool IsInvitationOrRequest
		{
			get
			{
				return this == SharingMessageType.Invitation || this == SharingMessageType.Request || this == SharingMessageType.InvitationAndRequest;
			}
		}

		public bool IsInvitationOrAcceptOfRequest
		{
			get
			{
				return this == SharingMessageType.Invitation || this == SharingMessageType.AcceptOfRequest || this == SharingMessageType.InvitationAndRequest;
			}
		}

		internal SharingFlavor SharingFlavor
		{
			get
			{
				return this.sharingFlavor;
			}
		}

		public static SharingMessageType GetSharingMessageType(SharingFlavor sharingFlavor)
		{
			EnumValidator.ThrowIfInvalid<SharingFlavor>(sharingFlavor, "sharingFlavor");
			if ((sharingFlavor & SharingFlavor.SharingMessageInvitation) == SharingFlavor.SharingMessageInvitation && (sharingFlavor & SharingFlavor.SharingMessageRequest) == SharingFlavor.SharingMessageRequest)
			{
				return SharingMessageType.InvitationAndRequest;
			}
			if ((sharingFlavor & SharingFlavor.SharingMessageAccept) == SharingFlavor.SharingMessageAccept)
			{
				return SharingMessageType.AcceptOfRequest;
			}
			if ((sharingFlavor & SharingFlavor.SharingMessageInvitation) == SharingFlavor.SharingMessageInvitation)
			{
				return SharingMessageType.Invitation;
			}
			if ((sharingFlavor & SharingFlavor.SharingMessageRequest) == SharingFlavor.SharingMessageRequest)
			{
				return SharingMessageType.Request;
			}
			if ((sharingFlavor & SharingFlavor.SharingMessageDeny) == SharingFlavor.SharingMessageDeny)
			{
				return SharingMessageType.DenyOfRequest;
			}
			return SharingMessageType.Unknown;
		}

		public override string ToString()
		{
			return this.name;
		}

		public static readonly SharingMessageType Invitation = new SharingMessageType("Invitation", SharingFlavor.SharingOut | SharingFlavor.SharingMessage | SharingFlavor.SharingMessageInvitation);

		public static readonly SharingMessageType Request = new SharingMessageType("Request", SharingFlavor.SharingMessage | SharingFlavor.SharingMessageRequest);

		public static readonly SharingMessageType InvitationAndRequest = new SharingMessageType("InvitationAndRequest", SharingFlavor.SharingOut | SharingFlavor.SharingMessage | SharingFlavor.SharingMessageInvitation | SharingFlavor.SharingMessageRequest);

		public static readonly SharingMessageType AcceptOfRequest = new SharingMessageType("AcceptOfRequest", SharingFlavor.SharingOut | SharingFlavor.SharingMessage | SharingFlavor.SharingMessageInvitation | SharingFlavor.SharingMessageResponse | SharingFlavor.SharingMessageAccept);

		public static readonly SharingMessageType DenyOfRequest = new SharingMessageType("DenyOfRequest", SharingFlavor.SharingMessage | SharingFlavor.SharingMessageResponse | SharingFlavor.SharingMessageDeny);

		public static readonly SharingMessageType Unknown = new SharingMessageType("Unknown", SharingFlavor.None);

		private SharingFlavor sharingFlavor;

		private string name;
	}
}
