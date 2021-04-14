using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class Sender
	{
		public Sender(IReadOnlyMailItem mailItem)
		{
			this.mailItem = mailItem;
		}

		public Microsoft.Exchange.Data.Directory.Recipient.RecipientType? RecipientType
		{
			get
			{
				return this.GetProperty<Microsoft.Exchange.Data.Directory.Recipient.RecipientType>("Microsoft.Exchange.Transport.DirectoryData.RecipientType");
			}
		}

		public ADObjectId ObjectId
		{
			get
			{
				return this.GetProperty<ADObjectId>("Microsoft.Exchange.Transport.DirectoryData.Sender.Id", null);
			}
		}

		public string DistinguishedName
		{
			get
			{
				return this.GetProperty<string>("Microsoft.Exchange.Transport.DirectoryData.Sender.DistinguishedName", null);
			}
		}

		public Unlimited<ByteQuantifiedSize> MaxSendSize
		{
			get
			{
				ulong bytesValue;
				if (this.mailItem.ExtendedProperties.TryGetValue<ulong>("Microsoft.Exchange.Transport.DirectoryData.Sender.MaxSendSize", out bytesValue))
				{
					ByteQuantifiedSize limitedValue = ByteQuantifiedSize.FromBytes(bytesValue);
					return new Unlimited<ByteQuantifiedSize>(limitedValue);
				}
				return Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			}
		}

		public string PrimarySmtpAddress
		{
			get
			{
				return this.mailItem.From.ToString();
			}
		}

		public Unlimited<int> RecipientLimits
		{
			get
			{
				int limitedValue;
				if (this.mailItem.ExtendedProperties.TryGetValue<int>("Microsoft.Exchange.Transport.DirectoryData.Sender.RecipientLimits", out limitedValue))
				{
					return new Unlimited<int>(limitedValue);
				}
				return Unlimited<int>.UnlimitedValue;
			}
		}

		public ExternalOofOptions UserExternalOofOptions
		{
			get
			{
				return (ExternalOofOptions)this.GetProperty<int>("Microsoft.Exchange.Transport.DirectoryData.Sender.ExternalOofOptions", 1);
			}
		}

		public string ExternalEmailAddress
		{
			get
			{
				return this.GetProperty<string>("Microsoft.Exchange.Transport.DirectoryData.ExternalEmailAddress", null);
			}
		}

		public Unlimited<int> EffectiveRecipientLimit
		{
			get
			{
				if (!this.RecipientLimits.IsUnlimited)
				{
					return this.RecipientLimits;
				}
				return this.mailItem.TransportSettings.MaxRecipientEnvelopeLimit;
			}
		}

		public bool AllowExternalOofs
		{
			get
			{
				return this.UserExternalOofOptions == ExternalOofOptions.External;
			}
		}

		public RoutingAddress? EmailAddress
		{
			get
			{
				if (!this.loadedEmailAddress)
				{
					EmailRecipient sender = this.mailItem.Message.Sender;
					if (sender == null)
					{
						ExTraceGlobals.ResolverTracer.TraceDebug((long)this.GetHashCode(), "EmailMessage can't find a (valid) P2 sender address");
					}
					else
					{
						ExTraceGlobals.ResolverTracer.TraceDebug<string>((long)this.GetHashCode(), "P2 sender from EmailMessage is {0}", sender.SmtpAddress);
						RoutingAddress value = new RoutingAddress(sender.SmtpAddress);
						if (!value.IsValid)
						{
							ExTraceGlobals.ResolverTracer.TraceDebug<string>((long)this.GetHashCode(), "P2 sender address \"{0}\" is invalid", sender.SmtpAddress);
						}
						else
						{
							this.emailAddress = new RoutingAddress?(value);
						}
					}
					this.loadedEmailAddress = true;
				}
				return this.emailAddress;
			}
		}

		public bool InternalOrgSender
		{
			get
			{
				Microsoft.Exchange.Data.Directory.Recipient.RecipientType valueOrDefault = this.RecipientType.GetValueOrDefault();
				Microsoft.Exchange.Data.Directory.Recipient.RecipientType? recipientType;
				if (recipientType != null)
				{
					switch (valueOrDefault)
					{
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Invalid:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Contact:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailContact:
						return false;
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.User:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUser:
					{
						string externalEmailAddress = this.ExternalEmailAddress;
						return externalEmailAddress == null;
					}
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Group:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalDistributionGroup:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUniversalSecurityGroup:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailNonUniversalGroup:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.DynamicDistributionGroup:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicFolder:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.PublicDatabase:
					case Microsoft.Exchange.Data.Directory.Recipient.RecipientType.SystemAttendantMailbox:
						return true;
					}
				}
				return false;
			}
		}

		internal ProxyAddress P1Address
		{
			get
			{
				return Sender.GetInnermostAddress(this.mailItem.From);
			}
		}

		internal ProxyAddress P2Address
		{
			get
			{
				if (this.EmailAddress != null)
				{
					return Sender.GetInnermostAddress(this.EmailAddress.Value);
				}
				return null;
			}
		}

		public static void Resolve(ADRawEntry p1Sender, ADRawEntry p2Sender, TransportMailItem mailItem)
		{
			bool flag;
			if (mailItem.ExtendedProperties.TryGetValue<bool>("Microsoft.Exchange.Transport.Sender.Resolved", out flag) && flag)
			{
				return;
			}
			Sender.ResolveReversePath(p1Sender, mailItem);
			Sender.ResolveP2Sender(p2Sender, mailItem);
			mailItem.ExtendedProperties.SetValue<bool>("Microsoft.Exchange.Transport.Sender.Resolved", true);
		}

		public RoutingAddress GetPurportedResponsibleAddress()
		{
			return Sender.GetPurportedResponsibleAddress(this.mailItem.RootPart.Headers);
		}

		internal static RoutingAddress GetPurportedResponsibleAddress(HeaderList headerList)
		{
			return Util.GetPurportedResponsibleAddress(headerList);
		}

		internal static ProxyAddress GetInnermostAddress(RoutingAddress outer)
		{
			return Sender.GetInnermostAddress(outer, Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomainName);
		}

		internal static ProxyAddress GetInnermostAddress(RoutingAddress outer, string firstOrgDefaultDomainName)
		{
			if (outer.Equals(RoutingAddress.NullReversePath))
			{
				ExTraceGlobals.ResolverTracer.TraceDebug(0L, "Null reverse path");
				return null;
			}
			ProxyAddress proxyAddress;
			if (Resolver.TryDeencapsulate(outer, firstOrgDefaultDomainName, out proxyAddress))
			{
				if (proxyAddress.Prefix == ProxyAddressPrefix.Smtp)
				{
					ExTraceGlobals.ResolverTracer.TraceDebug(0L, "address contains SMTP-SMTP encapsulation");
					proxyAddress = null;
				}
			}
			else
			{
				proxyAddress = new SmtpProxyAddress(outer.ToString(), false);
			}
			return proxyAddress;
		}

		private static void ResolveReversePath(ADRawEntry senderEntry, TransportMailItem mailItem)
		{
			ExTraceGlobals.ResolverTracer.TraceDebug<string>(0L, "resolving P1 reverse path {0}", mailItem.From.ToString());
			if (senderEntry == null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug(0L, "P1 reverse path not found in the directory");
				return;
			}
			RoutingAddress primarySmtpAddress = Resolver.GetPrimarySmtpAddress(senderEntry);
			if (RoutingAddress.Empty == primarySmtpAddress)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug(0L, "P1 reverse path doesn't have a primary SMTP address");
				return;
			}
			if (!primarySmtpAddress.IsValid)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<string>(0L, "P1 reverse path address is invalid: \"{0}\"", primarySmtpAddress.ToString());
				return;
			}
			ExTraceGlobals.ResolverTracer.TraceDebug<string>(0L, "P1 reverse path is now {0}", primarySmtpAddress.ToString());
			mailItem.From = primarySmtpAddress;
		}

		private static void ResolveP2Sender(ADRawEntry p2Sender, TransportMailItem mailItem)
		{
			ExTraceGlobals.ResolverTracer.TraceDebug(0L, "resolving P2 sender");
			if (p2Sender == null)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug(0L, "P2 sender not found in the directory");
				return;
			}
			SenderSchema.Set(p2Sender, mailItem);
		}

		private T GetProperty<T>(string name, T defaultValue)
		{
			return this.mailItem.ExtendedProperties.GetValue<T>(name, defaultValue);
		}

		private T? GetProperty<T>(string name) where T : struct
		{
			T value;
			if (!this.mailItem.ExtendedProperties.TryGetValue<T>(name, out value))
			{
				return null;
			}
			return new T?(value);
		}

		public const string Resolved = "Microsoft.Exchange.Transport.Sender.Resolved";

		public static readonly RoutingAddress NoPRA = Util.NoPRA;

		private IReadOnlyMailItem mailItem;

		private RoutingAddress? emailAddress = null;

		private bool loadedEmailAddress;
	}
}
