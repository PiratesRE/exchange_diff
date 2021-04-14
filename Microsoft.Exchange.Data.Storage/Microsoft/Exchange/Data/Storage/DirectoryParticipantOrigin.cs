using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DirectoryParticipantOrigin : ParticipantOrigin
	{
		public DirectoryParticipantOrigin()
		{
		}

		public DirectoryParticipantOrigin(ADRawEntry adEntry)
		{
			this.adEntry = adEntry;
		}

		public DirectoryParticipantOrigin(IExchangePrincipal principal)
		{
			this.principal = principal;
		}

		public DirectoryParticipantOrigin(IStorePropertyBag adContact)
		{
			this.adContact = adContact;
		}

		public ADRawEntry ADEntry
		{
			get
			{
				return this.adEntry;
			}
		}

		public IExchangePrincipal Principal
		{
			get
			{
				return this.principal;
			}
		}

		public IStorePropertyBag ADContact
		{
			get
			{
				return this.adContact;
			}
		}

		public override string ToString()
		{
			return "Directory";
		}

		internal override IEnumerable<PropValue> GetProperties()
		{
			if (this.adEntry != null)
			{
				return DirectoryParticipantOrigin.GetProperties(this.adEntry);
			}
			if (this.principal != null)
			{
				return DirectoryParticipantOrigin.GetProperties(this.principal);
			}
			return null;
		}

		internal override ParticipantValidationStatus Validate(Participant participant)
		{
			if (participant.EmailAddress == null || participant.RoutingType == null)
			{
				return ParticipantValidationStatus.AddressAndOriginMismatch;
			}
			return ParticipantValidationStatus.NoError;
		}

		private static IEnumerable<PropValue> GetProperties(ADRawEntry adEntry)
		{
			List<PropValue> list = new List<PropValue>();
			ConversionPropertyBag conversionPropertyBag = new ConversionPropertyBag(adEntry, StoreToDirectorySchemaConverter.Instance);
			foreach (StorePropertyDefinition storePropertyDefinition in DirectoryParticipantOrigin.propertiesToDonate)
			{
				object obj = conversionPropertyBag.TryGetProperty(storePropertyDefinition);
				if (!PropertyError.IsPropertyError(obj))
				{
					list.Add(new PropValue(storePropertyDefinition, obj));
				}
			}
			return list.ToArray();
		}

		private static IEnumerable<PropValue> GetProperties(IExchangePrincipal principal)
		{
			List<PropValue> list = new List<PropValue>();
			RemoteUserMailboxPrincipal remoteUserMailboxPrincipal = principal as RemoteUserMailboxPrincipal;
			if (remoteUserMailboxPrincipal != null)
			{
				list.Add(new PropValue(ParticipantSchema.SmtpAddress, remoteUserMailboxPrincipal.PrimarySmtpAddress.ToString()));
			}
			else
			{
				list.Add(new PropValue(ParticipantSchema.SmtpAddress, principal.MailboxInfo.PrimarySmtpAddress.ToString()));
			}
			return list;
		}

		private readonly ADRawEntry adEntry;

		private readonly IExchangePrincipal principal;

		private readonly IStorePropertyBag adContact;

		private static readonly StorePropertyDefinition[] propertiesToDonate = new StorePropertyDefinition[]
		{
			ParticipantSchema.LegacyExchangeDN,
			ParticipantSchema.SmtpAddress,
			ParticipantSchema.DisplayTypeEx
		};
	}
}
