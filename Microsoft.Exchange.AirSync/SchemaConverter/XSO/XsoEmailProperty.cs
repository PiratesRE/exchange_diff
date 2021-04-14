using System;
using System.Text;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	[Serializable]
	internal class XsoEmailProperty : XsoProperty, IStringProperty, IProperty
	{
		public XsoEmailProperty(EmailAddressIndex emailIndex) : base(null)
		{
			this.emailIndex = emailIndex;
		}

		public virtual string StringData
		{
			get
			{
				Contact contact = base.XsoItem as Contact;
				Participant participant = null;
				try
				{
					participant = contact.EmailAddresses[this.emailIndex];
				}
				catch (PropertyErrorException ex)
				{
					if (ex.PropertyErrors[0].PropertyErrorCode == PropertyErrorCode.NotEnoughMemory && this.emailIndex != EmailAddressIndex.None)
					{
						contact.Load(new PropertyDefinition[]
						{
							XsoEmailProperty.emailAddressesMapping[(int)this.emailIndex]
						});
						participant = contact.GetValueOrDefault<Participant>(XsoEmailProperty.emailAddressesMapping[(int)this.emailIndex]);
					}
				}
				if (participant == null || (string.IsNullOrEmpty(participant.EmailAddress) && string.IsNullOrEmpty(participant.DisplayName)))
				{
					return null;
				}
				string text = EmailAddressConverter.LookupEmailAddressString(participant, contact.Session.MailboxOwner);
				string fullEmailString = XsoEmailProperty.GetFullEmailString(participant.DisplayName, text);
				if (participant.RoutingType == "EX" && text == participant.EmailAddress)
				{
					text = SmtpProxyAddress.EncapsulateAddress("EX", participant.EmailAddress, SendMailBase.DefaultDomain);
					fullEmailString = XsoEmailProperty.GetFullEmailString(participant.EmailAddress, text);
				}
				return fullEmailString;
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			string stringData = ((IStringProperty)srcProperty).StringData;
			Contact contact = base.XsoItem as Contact;
			if (string.IsNullOrEmpty(stringData))
			{
				contact.EmailAddresses.Remove(this.emailIndex);
				return;
			}
			Participant participant = contact.EmailAddresses[this.emailIndex];
			string text = null;
			string text2 = null;
			if (participant != null && !string.IsNullOrEmpty(participant.EmailAddress))
			{
				text2 = EmailAddressConverter.LookupEmailAddressString(participant, contact.Session.MailboxOwner);
				text = XsoEmailProperty.GetFullEmailString(participant.DisplayName, text2);
				AirSyncDiagnostics.TraceDebug<string, string, string>(ExTraceGlobals.XsoTracer, this, "XSOEmailProperty convertedSmtpAddress :{0}, currentEmail:{1}, RoutingType:{2}", text2, text, participant.RoutingType);
				if (participant.RoutingType == "EX" && text2 == participant.EmailAddress)
				{
					text2 = SmtpProxyAddress.EncapsulateAddress("EX", participant.EmailAddress, SendMailBase.DefaultDomain);
					text = XsoEmailProperty.GetFullEmailString(participant.EmailAddress, text2);
				}
			}
			if (stringData != text && stringData != text2)
			{
				contact.EmailAddresses.Remove(this.emailIndex);
				contact.EmailAddresses.Add(this.emailIndex, EmailAddressConverter.CreateParticipant(stringData));
			}
		}

		protected override void InternalSetToDefault(IProperty srcProperty)
		{
			Contact contact = base.XsoItem as Contact;
			contact.EmailAddresses.Remove(this.emailIndex);
		}

		private static string GetFullEmailString(string displayName, string emailAddress)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			if (displayName.StartsWith("\"") && displayName.EndsWith("\""))
			{
				stringBuilder.Append(displayName);
				stringBuilder.Append(" <");
			}
			else
			{
				stringBuilder.Append('"');
				stringBuilder.Append(displayName);
				stringBuilder.Append("\" <");
			}
			stringBuilder.Append(emailAddress);
			stringBuilder.Append('>');
			AirSyncDiagnostics.TraceInfo<StringBuilder>(ExTraceGlobals.CommonTracer, null, "GetFullEmailString = {0}", stringBuilder);
			return stringBuilder.ToString();
		}

		private static readonly PropertyDefinition[] emailAddressesMapping = new PropertyDefinition[]
		{
			null,
			ContactSchema.Email1,
			ContactSchema.Email2,
			ContactSchema.Email3,
			ContactSchema.ContactBusinessFax,
			ContactSchema.ContactHomeFax,
			ContactSchema.ContactHomeFax
		};

		private EmailAddressIndex emailIndex;
	}
}
