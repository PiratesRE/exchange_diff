using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.SecureMail;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.Transport
{
	internal class OutboundExch50 : Exch50
	{
		public static byte[] GetExch50(IReadOnlyMailItem mailItem, List<MailRecipient> recipients, ITransportConfiguration transportConfiguration, IMailRouter mailRouter)
		{
			Exch50Writer exch50Writer = new Exch50Writer();
			exch50Writer.Add(OutboundExch50.GetMessageMdbef(mailItem, transportConfiguration));
			foreach (MailRecipient recipient in recipients)
			{
				exch50Writer.Add(OutboundExch50.GetRecipientMdbef(mailItem, recipient, mailRouter));
			}
			return exch50Writer.GetBytes();
		}

		private static MdbefPropertyCollection GetMessageMdbef(IReadOnlyMailItem mailItem, ITransportConfiguration transportConfiguration)
		{
			byte[] legacyXexch50Blob = mailItem.LegacyXexch50Blob;
			MdbefPropertyCollection mdbefPropertyCollection;
			if (legacyXexch50Blob != null)
			{
				mdbefPropertyCollection = MdbefPropertyCollection.Create(legacyXexch50Blob, 0, legacyXexch50Blob.Length);
			}
			else
			{
				mdbefPropertyCollection = new MdbefPropertyCollection();
			}
			int? sclValue = OutboundExch50.GetSclValue(mailItem.RootPart.Headers);
			if (sclValue != null)
			{
				mdbefPropertyCollection[1081475075U] = sclValue.Value;
			}
			int? senderIdStatus = OutboundExch50.GetSenderIdStatus(mailItem.RootPart.Headers);
			if (senderIdStatus != null)
			{
				mdbefPropertyCollection[1081671683U] = senderIdStatus.Value;
			}
			AutoResponseSuppress autoResponseSuppress = OutboundExch50.GetAutoResponseSuppress(mailItem.RootPart.Headers);
			if (autoResponseSuppress != (AutoResponseSuppress)0)
			{
				mdbefPropertyCollection[1071579139U] = (int)autoResponseSuppress;
			}
			try
			{
				mdbefPropertyCollection[1703966U] = mailItem.Message.MapiMessageClass;
			}
			catch (ExchangeDataException)
			{
			}
			catch (IOException e)
			{
				if (!ExceptionHelper.IsHandleableTransientCtsException(e))
				{
					throw;
				}
			}
			string contentIdentifier = OutboundExch50.GetContentIdentifier(mailItem, transportConfiguration);
			if (contentIdentifier != null)
			{
				mdbefPropertyCollection[524318U] = contentIdentifier;
			}
			ReadOnlyCollection<Guid> readOnlyCollection;
			if (!mailItem.ExtendedProperties.TryGetListValue<Guid>("Microsoft.Exchange.JournalRecipientList", out readOnlyCollection) || readOnlyCollection == null)
			{
				Guid[] xmsexchangeOrganizationJournalRecipientList = OutboundExch50.GetXMSExchangeOrganizationJournalRecipientList(mailItem.Message.MimeDocument.RootPart.Headers);
				if (xmsexchangeOrganizationJournalRecipientList != null && xmsexchangeOrganizationJournalRecipientList.Length != 0)
				{
					readOnlyCollection = new ReadOnlyCollection<Guid>(xmsexchangeOrganizationJournalRecipientList);
				}
			}
			if (readOnlyCollection != null && readOnlyCollection.Count != 0)
			{
				byte[] array = new byte[16 * readOnlyCollection.Count];
				for (int i = 0; i < readOnlyCollection.Count; i++)
				{
					byte[] sourceArray = readOnlyCollection[i].ToByteArray();
					Array.Copy(sourceArray, 0, array, i * 16, 16);
				}
				mdbefPropertyCollection[1080819970U] = array;
			}
			if (mailItem.Message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Alt-Recipient-Prohibited") != null)
			{
				mdbefPropertyCollection[2818059U] = true;
			}
			if (mailItem.Message.MimeDocument.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-DL-Expansion-Prohibited") != null)
			{
				mdbefPropertyCollection[1310731U] = true;
			}
			OutboundExch50.ProcessLongAddress(mailItem.From, mdbefPropertyCollection, MdbefPropTag.DmsUnencapSenderA);
			if (Components.IsBridgehead)
			{
				DateTime time;
				bool flag = Util.TryGetOrganizationalMessageArrivalTime(mailItem, out time);
				if (flag)
				{
					mdbefPropertyCollection[1080754434U] = OutboundExch50.GetMsgtrackingOrgGuid(time, transportConfiguration);
				}
				BifInfo messageBifinfo = OutboundExch50.GetMessageBifinfo(mailItem);
				if (messageBifinfo != null)
				{
					mdbefPropertyCollection[1080688898U] = messageBifinfo.ToByteArray();
					if (!string.IsNullOrEmpty(messageBifinfo.CharSet))
					{
						mdbefPropertyCollection[1493368862U] = messageBifinfo.CharSet;
					}
				}
				object obj;
				byte[] mmpInfo;
				if (mdbefPropertyCollection.TryGetValue(1080951042U, out obj))
				{
					mmpInfo = (byte[])obj;
				}
				else
				{
					mmpInfo = null;
				}
				mdbefPropertyCollection[1080951042U] = OutboundExch50.GetTransportMmpInfo(mailItem, mmpInfo);
				if (mdbefPropertyCollection.TryGetValue(1081606402U, out obj))
				{
					mmpInfo = (byte[])obj;
				}
				else
				{
					mmpInfo = null;
				}
				mdbefPropertyCollection[1081606402U] = OutboundExch50.GetXorgTransportMmpInfo(mailItem, mmpInfo);
			}
			return mdbefPropertyCollection;
		}

		private static MdbefPropertyCollection GetRecipientMdbef(IReadOnlyMailItem mailItem, MailRecipient recipient, IMailRouter mailRouter)
		{
			ReadOnlyCollection<byte> readOnlyCollection;
			MdbefPropertyCollection mdbefPropertyCollection;
			if (recipient.ExtendedProperties.TryGetListValue<byte>("Microsoft.Exchange.Legacy.PassThru", out readOnlyCollection))
			{
				byte[] array = new byte[readOnlyCollection.Count];
				readOnlyCollection.CopyTo(array, 0);
				mdbefPropertyCollection = MdbefPropertyCollection.Create(array, 0, readOnlyCollection.Count);
			}
			else
			{
				mdbefPropertyCollection = new MdbefPropertyCollection();
			}
			OutboundExch50.ProcessLongAddress(recipient.Email, mdbefPropertyCollection, MdbefPropTag.DmsUnencapRcptA);
			if (Components.IsBridgehead)
			{
				string serverFqdn;
				string str;
				if (recipient.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.Transport.RoutingOverride", out serverFqdn) && mailRouter.TryGetServerLegacyDNByFqdn(serverFqdn, out str))
				{
					string text = str + "/cn=Microsoft Public MDB";
					byte[] array2 = new byte[Encoding.ASCII.GetByteCount(text) + 1];
					Encoding.ASCII.GetBytes(text, 0, text.Length, array2, 0);
					array2[array2.Length - 1] = 0;
					mdbefPropertyCollection[1080885506U] = array2;
				}
				object obj;
				byte[] mmpInfo;
				if (mdbefPropertyCollection.TryGetValue(1080951042U, out obj))
				{
					mmpInfo = (byte[])obj;
				}
				else
				{
					mmpInfo = null;
				}
				mdbefPropertyCollection[1080951042U] = OutboundExch50.GetTransportMmpInfo(mailItem, recipient, mmpInfo);
				BifInfo bifInfo = new BifInfo();
				if (OutboundExch50.CopyConversionOptions(mailItem.RootPart.Headers, bifInfo))
				{
					if (bifInfo.SendTNEF != null)
					{
						mdbefPropertyCollection[977272843U] = bifInfo.SendTNEF.Value;
					}
					if (bifInfo.SendInternetEncoding != null)
					{
						mdbefPropertyCollection[980484099U] = (int)bifInfo.SendInternetEncoding.Value;
					}
				}
			}
			byte[] value;
			if (OrarGenerator.TryGetOrarBlob(recipient, out value))
			{
				mdbefPropertyCollection[201916674U] = value;
			}
			if (!string.IsNullOrEmpty(recipient.ORcpt))
			{
				byte[] array3 = RedirectionHistory.GenerateRedirectionHistoryFromOrcpt(recipient.ORcpt);
				if (array3 != null)
				{
					mdbefPropertyCollection[2883842U] = array3;
				}
			}
			return mdbefPropertyCollection;
		}

		private static bool CopyConversionOptions(HeaderList headers, BifInfo bifinfo)
		{
			try
			{
				Header header = headers.FindFirst("X-MS-Exchange-Organization-ContentConversionOptions");
				if (header != null)
				{
					bifinfo.CopyFromContentConversionOptionsString(header.Value);
					return bifinfo.HasContentConversionOptions;
				}
			}
			catch (ExchangeDataException)
			{
			}
			return false;
		}

		private static int? GetSclValue(HeaderList headers)
		{
			Header header = headers.FindFirst("X-MS-Exchange-Organization-SCL");
			if (header == null)
			{
				return null;
			}
			int num;
			try
			{
				num = int.Parse(header.Value);
			}
			catch (ExchangeDataException)
			{
				return null;
			}
			catch (FormatException)
			{
				return null;
			}
			catch (OverflowException)
			{
				return null;
			}
			num = ((num < -1) ? -1 : ((num > 9) ? 9 : num));
			return new int?(num);
		}

		private static int? GetSenderIdStatus(HeaderList headers)
		{
			Header header = headers.FindFirst("X-MS-Exchange-Organization-SenderIdResult");
			if (header == null)
			{
				return null;
			}
			int? result = null;
			try
			{
				SenderIdStatus value;
				if (EnumValidator<SenderIdStatus>.TryParse(header.Value, EnumParseOptions.IgnoreCase, out value))
				{
					result = new int?((int)value);
				}
			}
			catch (ExchangeDataException)
			{
			}
			return result;
		}

		private static AutoResponseSuppress GetAutoResponseSuppress(HeaderList headers)
		{
			AutoResponseSuppress autoResponseSuppress = (AutoResponseSuppress)0;
			AutoResponseSuppress autoResponseSuppress2 = (AutoResponseSuppress)0;
			Header[] array = headers.FindAll("X-Auto-Response-Suppress");
			foreach (Header header in array)
			{
				try
				{
					if (EnumValidator<AutoResponseSuppress>.TryParse(header.Value, EnumParseOptions.IgnoreCase, out autoResponseSuppress2))
					{
						autoResponseSuppress |= autoResponseSuppress2;
					}
				}
				catch (ExchangeDataException)
				{
				}
			}
			return autoResponseSuppress;
		}

		private static Guid[] GetXMSExchangeOrganizationJournalRecipientList(HeaderList headers)
		{
			Header header = headers.FindFirst("X-MS-Exchange-Organization-JournalRecipientList");
			if (header == null)
			{
				return null;
			}
			string[] array = header.Value.Split(new char[]
			{
				';'
			});
			List<Guid> list = new List<Guid>(array.Length);
			foreach (string g in array)
			{
				Guid item;
				if (GuidHelper.TryParseGuid(g, out item))
				{
					list.Add(item);
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list.ToArray();
		}

		private static string GetContentIdentifier(IReadOnlyMailItem mailItem, ITransportConfiguration transportConfiguration)
		{
			string explicitContentIdentifier = OutboundExch50.GetExplicitContentIdentifier(mailItem);
			if (!string.IsNullOrEmpty(explicitContentIdentifier))
			{
				return explicitContentIdentifier;
			}
			if (!Components.IsBridgehead)
			{
				return null;
			}
			string defaultDomainName = transportConfiguration.FirstOrgAcceptedDomainTable.DefaultDomainName;
			if (defaultDomainName == null)
			{
				return null;
			}
			IList<RoutingAddress> privilegedSenders = ResolverConfiguration.GetPrivilegedSenders(OrganizationId.ForestWideOrgId, defaultDomainName, mailItem.TransportSettings);
			bool flag = DeliveryRestriction.IsPrivilegedSender(new Sender(mailItem), MultilevelAuth.IsAuthenticated(mailItem), privilegedSenders);
			if (flag)
			{
				return "ExSysMessage";
			}
			return null;
		}

		private static string GetExplicitContentIdentifier(IReadOnlyMailItem mailItem)
		{
			string text;
			if (mailItem.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.ContentIdentifier", out text) && !string.IsNullOrEmpty(text))
			{
				return text;
			}
			return null;
		}

		private static byte[] GetMsgtrackingOrgGuid(DateTime time, ITransportConfiguration transportConfiguration)
		{
			DateTime dateTime = time.ToUniversalTime();
			byte[] array = new byte[32];
			Buffer.BlockCopy(transportConfiguration.TransportSettings.OrganizationGuid.ToByteArray(), 0, array, 0, 16);
			Buffer.BlockCopy(BitConverter.GetBytes((short)dateTime.Year), 0, array, 16, 2);
			Buffer.BlockCopy(BitConverter.GetBytes((short)dateTime.Month), 0, array, 18, 2);
			short value = 0;
			switch (dateTime.DayOfWeek)
			{
			case DayOfWeek.Sunday:
				value = 0;
				break;
			case DayOfWeek.Monday:
				value = 1;
				break;
			case DayOfWeek.Tuesday:
				value = 2;
				break;
			case DayOfWeek.Wednesday:
				value = 3;
				break;
			case DayOfWeek.Thursday:
				value = 4;
				break;
			case DayOfWeek.Friday:
				value = 5;
				break;
			case DayOfWeek.Saturday:
				value = 6;
				break;
			}
			Buffer.BlockCopy(BitConverter.GetBytes(value), 0, array, 20, 2);
			Buffer.BlockCopy(BitConverter.GetBytes((short)dateTime.Day), 0, array, 22, 2);
			Buffer.BlockCopy(BitConverter.GetBytes((short)dateTime.Hour), 0, array, 24, 2);
			Buffer.BlockCopy(BitConverter.GetBytes((short)dateTime.Minute), 0, array, 26, 2);
			Buffer.BlockCopy(BitConverter.GetBytes((short)dateTime.Second), 0, array, 28, 2);
			Buffer.BlockCopy(BitConverter.GetBytes((short)dateTime.Millisecond), 0, array, 30, 2);
			return array;
		}

		private static byte[] GetTransportMmpInfo(IReadOnlyMailItem mailItem, byte[] mmpInfo)
		{
			TransportPropertyWriter transportPropertyWriter = new TransportPropertyWriter();
			bool flag = false;
			if (mmpInfo != null)
			{
				TransportPropertyReader transportPropertyReader = new TransportPropertyReader(mmpInfo, 0, mmpInfo.Length);
				while (transportPropertyReader.ReadNextProperty())
				{
					transportPropertyWriter.Add(transportPropertyReader.Range, transportPropertyReader.Id, transportPropertyReader.Value);
					if (transportPropertyReader.Id == 21U)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				Header header = mailItem.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-MessageSource");
				if (header != null && "StoreDriver".Equals(header.Value, StringComparison.OrdinalIgnoreCase))
				{
					transportPropertyWriter.Add(Exch50.ExchangeMailMsgPropertyRange, 21U, TransportProperty.PutBool(true));
				}
			}
			Sender sender = new Sender(mailItem);
			if (!string.Equals(mailItem.From.ToString(), sender.PrimarySmtpAddress, StringComparison.OrdinalIgnoreCase))
			{
				string text = sender.DistinguishedName ?? string.Empty;
				if (!BifInfo.IsValidDN(text))
				{
					transportPropertyWriter.Add(Exch50.ExchangeMailMsgPropertyRange, 35U, TransportProperty.PutUTF8String(text));
				}
			}
			if (!sender.RecipientLimits.IsUnlimited)
			{
				transportPropertyWriter.Add(Exch50.ExchangeMailMsgPropertyRange, 32U, TransportProperty.PutDword(sender.RecipientLimits.Value));
			}
			if (transportPropertyWriter.Length == 0)
			{
				return null;
			}
			return transportPropertyWriter.GetBytes();
		}

		private static byte[] GetXorgTransportMmpInfo(IReadOnlyMailItem mailItem, byte[] mmpInfo)
		{
			TransportPropertyWriter transportPropertyWriter = new TransportPropertyWriter();
			if (mmpInfo != null)
			{
				TransportPropertyReader transportPropertyReader = new TransportPropertyReader(mmpInfo, 0, mmpInfo.Length);
				while (transportPropertyReader.ReadNextProperty())
				{
					transportPropertyWriter.Add(transportPropertyReader.Range, transportPropertyReader.Id, transportPropertyReader.Value);
				}
			}
			Sender sender = new Sender(mailItem);
			if (!sender.RecipientLimits.IsUnlimited)
			{
				transportPropertyWriter.Add(Exch50.ExchangeMailMsgPropertyRange, 32U, TransportProperty.PutDword(sender.RecipientLimits.Value));
			}
			if (transportPropertyWriter.Length == 0)
			{
				return null;
			}
			return transportPropertyWriter.GetBytes();
		}

		private static byte[] GetTransportMmpInfo(IReadOnlyMailItem mailItem, MailRecipient recipient, byte[] mmpInfo)
		{
			TransportPropertyWriter transportPropertyWriter = new TransportPropertyWriter();
			if (mmpInfo != null)
			{
				TransportPropertyReader transportPropertyReader = new TransportPropertyReader(mmpInfo, 0, mmpInfo.Length);
				while (transportPropertyReader.ReadNextProperty())
				{
					transportPropertyWriter.Add(transportPropertyReader.Range, transportPropertyReader.Id, transportPropertyReader.Value);
				}
			}
			ResolverMessage resolverMessage = new ResolverMessage(mailItem.Message, mailItem.MimeSize);
			if (resolverMessage.RedirectHandled)
			{
				transportPropertyWriter.Add(Exch50.ExchangeMailMsgPropertyRange, 1022U, TransportProperty.PutBool(true));
			}
			if (transportPropertyWriter.Length == 0)
			{
				return null;
			}
			return transportPropertyWriter.GetBytes();
		}

		private static BifInfo GetMessageBifinfo(IReadOnlyMailItem mailItem)
		{
			BifInfo bifInfo = new BifInfo();
			Sender sender = new Sender(mailItem);
			if (!string.Equals(mailItem.From.ToString(), sender.PrimarySmtpAddress, StringComparison.OrdinalIgnoreCase))
			{
				string text = sender.DistinguishedName ?? string.Empty;
				if (BifInfo.IsValidDN(text))
				{
					bifInfo.SenderDN = text;
				}
			}
			if (mailItem.Message.MapiMessageClass.Equals("IPM.Outlook.Recall"))
			{
				ResolverMessage resolverMessage = new ResolverMessage(mailItem.Message, mailItem.MimeSize);
				if (resolverMessage.SuppressRecallReport)
				{
					if (mailItem.From == RoutingAddress.NullReversePath)
					{
						bifInfo.SenderType = new BifSenderType?(BifSenderType.NullSender);
					}
					else
					{
						bifInfo.SenderType = new BifSenderType?(BifSenderType.DlOwner);
					}
				}
			}
			if (!OutboundExch50.CopyConversionOptions(mailItem.RootPart.Headers, bifInfo))
			{
				return null;
			}
			bifInfo.SuppressFlag = new uint?((uint)OutboundExch50.GetAutoResponseSuppress(mailItem.RootPart.Headers));
			return bifInfo;
		}

		private static void ProcessLongAddress(RoutingAddress address, MdbefPropertyCollection mdbef, MdbefPropTag propTag)
		{
			X400ProxyAddress x400ProxyAddress;
			if (Util.IsLongAddressForE2k3(address) && Util.TryDeencapsulateX400(address, out x400ProxyAddress))
			{
				mdbef[(uint)propTag] = x400ProxyAddress.ToString();
			}
		}
	}
}
