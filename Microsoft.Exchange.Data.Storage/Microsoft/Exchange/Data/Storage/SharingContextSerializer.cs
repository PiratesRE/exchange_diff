using System;
using System.Collections.Generic;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SharingContextSerializer
	{
		internal SharingContextSerializer(SharingContext context)
		{
			Util.ThrowOnNullArgument(context, "context");
			this.context = context;
		}

		private static SharingMessageType CalculateSharingMessageType(SharingMessage sharingMessage)
		{
			if (sharingMessage.Invitation != null && sharingMessage.Request != null)
			{
				return SharingMessageType.InvitationAndRequest;
			}
			if (sharingMessage.Invitation != null)
			{
				return SharingMessageType.Invitation;
			}
			if (sharingMessage.Request != null)
			{
				return SharingMessageType.Request;
			}
			if (sharingMessage.AcceptOfRequest != null)
			{
				return SharingMessageType.AcceptOfRequest;
			}
			if (sharingMessage.DenyOfRequest != null)
			{
				return SharingMessageType.DenyOfRequest;
			}
			return SharingMessageType.Unknown;
		}

		internal bool ReadFromMetadataXml(MessageItem messageItem)
		{
			SharingMessage sharingMessage = SharingMessageAttachment.GetSharingMessage(messageItem);
			if (sharingMessage == null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: The sharing_metadata.xml is invalid", messageItem.Session.UserLegacyDN);
				return false;
			}
			if (sharingMessage.Validate().Result != ValidationResult.Success)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: The sharing_metadata.xml is invalid", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("SharingMetadata");
			}
			SharingDataType sharingDataType = SharingDataType.FromExternalName(sharingMessage.DataType);
			if (sharingDataType == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: Unknown sharing data type: {1}", messageItem.Session.UserLegacyDN, sharingMessage.DataType);
				throw new InvalidSharingMessageException("DataType");
			}
			this.context.FolderClass = sharingDataType.ContainerClass;
			if (!SmtpAddress.IsValidSmtpAddress(sharingMessage.Initiator.SmtpAddress))
			{
				ExTraceGlobals.SharingTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: Initiator's smtp address is invalid: {1}", messageItem.Session.UserLegacyDN, sharingMessage.Initiator.SmtpAddress);
				throw new InvalidSharingMessageException("InitiatorSmtpAddress");
			}
			this.context.InitiatorName = sharingMessage.Initiator.Name;
			this.context.InitiatorSmtpAddress = sharingMessage.Initiator.SmtpAddress;
			this.context.InitiatorEntryId = HexConverter.HexStringToByteArray(sharingMessage.Initiator.EntryId);
			this.context.AvailableSharingProviders.Clear();
			if (sharingMessage.Invitation != null)
			{
				this.ReadActionFromMetadataXml(sharingMessage.Invitation);
			}
			else if (sharingMessage.AcceptOfRequest != null)
			{
				this.ReadActionFromMetadataXml(sharingMessage.AcceptOfRequest);
			}
			else if (sharingMessage.Request != null)
			{
				this.ReadActionFromMetadataXml(sharingMessage.Request);
			}
			else if (sharingMessage.DenyOfRequest != null)
			{
				this.ReadActionFromMetadataXml(sharingMessage.DenyOfRequest);
			}
			if (this.context.AvailableSharingProviders.Count == 0)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: No known sharing provider is found in message.", messageItem.Session.UserLegacyDN);
				throw new NotSupportedSharingMessageException();
			}
			if (this.context.IsPrimary)
			{
				this.context.FolderName = sharingDataType.DisplayName.ToString(messageItem.Session.InternalPreferedCulture);
			}
			this.context.SetDefaultCapabilities();
			SharingMessageType sharingMessageType = SharingContextSerializer.CalculateSharingMessageType(sharingMessage);
			if (sharingMessageType == SharingMessageType.Unknown)
			{
				ExTraceGlobals.SharingTracer.TraceError<string>((long)this.GetHashCode(), "{0}: SharingMessageType is unknown", messageItem.Session.UserLegacyDN);
				throw new InvalidSharingMessageException("SharingMessageType");
			}
			this.context.SharingMessageType = sharingMessageType;
			return true;
		}

		private void ReadActionFromMetadataXml(SharingMessageAction action)
		{
			if (action != null)
			{
				this.context.IsPrimary = string.IsNullOrEmpty(action.Title);
				if (!this.context.IsPrimary)
				{
					this.context.FolderName = action.Title;
				}
				foreach (SharingMessageProvider sharingMessageProvider in action.Providers)
				{
					string[] recipients = sharingMessageProvider.TargetRecipients.Split(new char[]
					{
						';'
					});
					SharingProvider sharingProvider = SharingProvider.FromExternalName(sharingMessageProvider.Type);
					if (sharingProvider != null)
					{
						sharingProvider.ParseSharingMessageProvider(this.context, sharingMessageProvider);
						this.context.AvailableSharingProviders.Add(sharingProvider, new CheckRecipientsResults(ValidRecipient.ConvertFromStringArray(recipients)));
					}
				}
			}
		}

		internal void SaveIntoMetadataXml(MessageItem messageItem)
		{
			SharingMessage sharingMessage = this.CreateSharingMessage();
			SharingMessageAttachment.SetSharingMessage(messageItem, sharingMessage);
		}

		private SharingMessage CreateSharingMessage()
		{
			if (this.context.SharingMessageType == SharingMessageType.Invitation)
			{
				return this.CreateInvitationMessage();
			}
			if (this.context.SharingMessageType == SharingMessageType.Request)
			{
				return this.CreateRequestMessage();
			}
			if (this.context.SharingMessageType == SharingMessageType.InvitationAndRequest)
			{
				return this.CreateInvitationAndRequestMessage();
			}
			if (this.context.SharingMessageType == SharingMessageType.AcceptOfRequest)
			{
				return this.CreateAcceptOfRequestMessage();
			}
			if (this.context.SharingMessageType == SharingMessageType.DenyOfRequest)
			{
				return this.CreateDenyOfRequestMessage();
			}
			throw new NotSupportedException("SharingMessageType is not supported");
		}

		private SharingMessage CreateRequestMessage()
		{
			return new SharingMessage
			{
				DataType = this.context.DataType.ExternalName,
				Initiator = this.CreateInitiator(),
				Request = this.CreateRequestAction()
			};
		}

		private SharingMessage CreateInvitationMessage()
		{
			return new SharingMessage
			{
				DataType = this.context.DataType.ExternalName,
				Initiator = this.CreateInitiator(),
				Invitation = this.CreateInvitationAction()
			};
		}

		private SharingMessage CreateInvitationAndRequestMessage()
		{
			return new SharingMessage
			{
				DataType = this.context.DataType.ExternalName,
				Initiator = this.CreateInitiator(),
				Invitation = this.CreateInvitationAction(),
				Request = this.CreateRequestAction()
			};
		}

		private SharingMessage CreateAcceptOfRequestMessage()
		{
			return new SharingMessage
			{
				DataType = this.context.DataType.ExternalName,
				Initiator = this.CreateInitiator(),
				AcceptOfRequest = this.CreateInvitationAction()
			};
		}

		private SharingMessage CreateDenyOfRequestMessage()
		{
			return new SharingMessage
			{
				DataType = this.context.DataType.ExternalName,
				Initiator = this.CreateInitiator(),
				DenyOfRequest = this.CreateRequestAction()
			};
		}

		private SharingMessageAction CreateInvitationAction()
		{
			List<SharingMessageProvider> list = new List<SharingMessageProvider>(this.context.AvailableSharingProviders.Count);
			foreach (KeyValuePair<SharingProvider, CheckRecipientsResults> keyValuePair in this.context.AvailableSharingProviders)
			{
				SharingProvider key = keyValuePair.Key;
				CheckRecipientsResults value = keyValuePair.Value;
				if (value != null && value.ValidRecipients != null && value.ValidRecipients.Length > 0)
				{
					SharingMessageProvider sharingMessageProvider = key.CreateSharingMessageProvider(this.context);
					sharingMessageProvider.TargetRecipients = value.TargetRecipients;
					list.Add(sharingMessageProvider);
				}
			}
			return new SharingMessageAction
			{
				Title = (this.context.IsPrimary ? null : this.context.FolderName),
				Providers = list.ToArray()
			};
		}

		private SharingMessageAction CreateRequestAction()
		{
			List<SharingMessageProvider> list = new List<SharingMessageProvider>(this.context.AvailableSharingProviders.Count);
			foreach (KeyValuePair<SharingProvider, CheckRecipientsResults> keyValuePair in this.context.AvailableSharingProviders)
			{
				SharingProvider key = keyValuePair.Key;
				CheckRecipientsResults value = keyValuePair.Value;
				if (key != SharingProvider.SharingProviderPublish && value != null && value.ValidRecipients != null && value.ValidRecipients.Length > 0)
				{
					SharingMessageProvider sharingMessageProvider = key.CreateSharingMessageProvider();
					sharingMessageProvider.TargetRecipients = value.TargetRecipients;
					list.Add(sharingMessageProvider);
				}
			}
			return new SharingMessageAction
			{
				Providers = list.ToArray()
			};
		}

		private SharingMessageInitiator CreateInitiator()
		{
			return new SharingMessageInitiator
			{
				Name = this.context.InitiatorName,
				SmtpAddress = this.context.InitiatorSmtpAddress,
				EntryId = HexConverter.ByteArrayToHexString(this.context.InitiatorEntryId)
			};
		}

		private SharingContext context;
	}
}
