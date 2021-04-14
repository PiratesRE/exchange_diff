using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MobileTransport;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	internal static class TextMessagingHelper
	{
		public static void SaveTextMessagingAccount(TextMessagingAccount account, VersionedXmlDataProvider storeDataProvider, ADRecipient adRecipient, IRecipientSession adSession)
		{
			if (account == null)
			{
				throw new ArgumentNullException("account");
			}
			if (storeDataProvider == null)
			{
				throw new ArgumentNullException("storeDataProvider");
			}
			if (adRecipient == null)
			{
				throw new ArgumentNullException("adRecipient");
			}
			if (adSession == null)
			{
				throw new ArgumentNullException("adSession");
			}
			bool notificationPhoneNumberVerified = account.NotificationPhoneNumberVerified;
			bool dupIdentitiesExist = false;
			account.TextMessagingSettings.DeliveryPoints.Sort(delegate(DeliveryPoint x, DeliveryPoint y)
			{
				if (x.Identity == y.Identity)
				{
					dupIdentitiesExist = true;
				}
				return x.Identity.CompareTo(y.Identity);
			});
			if (dupIdentitiesExist)
			{
				int num = 0;
				while (account.TextMessagingSettings.DeliveryPoints.Count > num)
				{
					account.TextMessagingSettings.DeliveryPoints[num].Identity = (byte)num;
					num++;
				}
			}
			TextMessagingHelper.UpdateTextMessagingState(adRecipient.TextMessagingState, account.TextMessagingSettings.DeliveryPoints);
			storeDataProvider.Save(account);
			adSession.Save(adRecipient);
		}

		public static void UpdateAndSaveTextMessgaingStateOnAdUser(TextMessagingAccount account, ADRecipient adRecipient, IRecipientSession adSession)
		{
			if (account == null)
			{
				throw new ArgumentNullException("account");
			}
			if (adRecipient == null)
			{
				throw new ArgumentNullException("adRecipient");
			}
			if (adSession == null)
			{
				throw new ArgumentNullException("adSession");
			}
			if (TextMessagingHelper.UpdateTextMessagingState(adRecipient.TextMessagingState, account.TextMessagingSettings.DeliveryPoints))
			{
				adSession.Save(adRecipient);
			}
		}

		private static bool UpdateTextMessagingState(MultiValuedProperty<TextMessagingStateBase> states, List<DeliveryPoint> deliveryPoints)
		{
			if (states == null)
			{
				throw new ArgumentNullException("states");
			}
			if (deliveryPoints == null)
			{
				throw new ArgumentNullException("deliveryPoint");
			}
			List<TextMessagingStateBase> list = new List<TextMessagingStateBase>(states.Count);
			bool flag = states.Count >= deliveryPoints.Count;
			foreach (TextMessagingStateBase textMessagingStateBase in states)
			{
				TextMessagingDeliveryPointState deliveryPointState = textMessagingStateBase as TextMessagingDeliveryPointState;
				if (deliveryPointState != null && !deliveryPointState.Shared)
				{
					if (!deliveryPoints.Exists(delegate(DeliveryPoint dp)
					{
						if (dp.Identity != deliveryPointState.Identity || dp.Type != deliveryPointState.Type || !((dp.M2pMessagingPriority > -1) ? (dp.M2pMessagingPriority == (int)deliveryPointState.MachineToPersonMessagingPriority) : (!deliveryPointState.MachineToPersonMessagingEnabled)))
						{
							return false;
						}
						if (dp.P2pMessagingPriority <= -1)
						{
							return !deliveryPointState.PersonToPersonMessagingEnabled;
						}
						return dp.P2pMessagingPriority == (int)deliveryPointState.PersonToPersonMessagingPriority;
					}))
					{
						flag = false;
					}
					list.Add(deliveryPointState);
				}
			}
			if (flag)
			{
				return false;
			}
			try
			{
				foreach (TextMessagingStateBase item in list)
				{
					states.Remove(item);
				}
				foreach (DeliveryPoint deliveryPoint in deliveryPoints)
				{
					states.Add(new TextMessagingDeliveryPointState(false, deliveryPoint.Ready && -1 < deliveryPoint.P2pMessagingPriority, deliveryPoint.Ready && -1 < deliveryPoint.M2pMessagingPriority, deliveryPoint.Type, deliveryPoint.Identity, (byte)deliveryPoint.P2pMessagingPriority, (byte)deliveryPoint.M2pMessagingPriority));
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new ADTransientException(new LocalizedString(ex.Message), ex);
			}
			return true;
		}

		private static void SendMessageItemWithoutSave(MailboxSession mailboxSession, string messageClass, Participant from, Participant to, string subject, string body, bool htmlContent, Importance importance)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (string.IsNullOrEmpty(messageClass))
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (null == from)
			{
				throw new ArgumentNullException("from");
			}
			if (null == to)
			{
				throw new ArgumentNullException("to");
			}
			if (string.IsNullOrEmpty(body))
			{
				throw new ArgumentNullException("body");
			}
			StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Configuration);
			using (MessageItem messageItem = MessageItem.Create(mailboxSession, defaultFolderId))
			{
				messageItem.ClassName = messageClass;
				if (!string.IsNullOrEmpty(subject))
				{
					messageItem.Subject = subject;
				}
				using (TextWriter textWriter = messageItem.Body.OpenTextWriter(new BodyWriteConfiguration(htmlContent ? BodyFormat.TextHtml : BodyFormat.TextPlain, Charset.Unicode.Name)))
				{
					textWriter.Write(body);
				}
				messageItem.From = from;
				messageItem.Recipients.Add(to, RecipientItemType.To);
				messageItem.Importance = importance;
				messageItem.SendWithoutSavingMessage();
			}
		}

		public static void SendSystemTextMessage(MailboxSession mailboxSession, E164Number recipient, string content, bool alert)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (string.IsNullOrEmpty(content))
			{
				throw new ArgumentNullException("content");
			}
			TextMessagingHelper.SendMessageItemWithoutSave(mailboxSession, alert ? "IPM.Note.Mobile.SMS.Alert.Info" : "IPM.Note.Mobile.SMS.Undercurrent", new Participant(mailboxSession.MailboxOwner), new Participant(recipient.Number, recipient.Number, "MOBILE"), null, content, false, Importance.Normal);
		}

		public static CultureInfo GetSupportedUserCulture(ADUser adUser)
		{
			if (adUser == null)
			{
				throw new ArgumentNullException("adUser");
			}
			CultureInfo result = null;
			if (adUser.Languages != null)
			{
				foreach (CultureInfo cultureInfo in adUser.Languages)
				{
					if (ClientLanguageConstraint.IsSupportedCulture(cultureInfo))
					{
						result = cultureInfo;
						break;
					}
				}
			}
			return result;
		}

		private static Uri GetEcpServiceUrl(ExchangePrincipal principal, ClientAccessType type)
		{
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			if (principal.MailboxInfo.Location.ServerVersion >= Server.E15MinVersion && enabled)
			{
				return TextMessagingHelper.GetE15MultitenancyEcpServiceUrl(principal, type);
			}
			ServiceTopology serviceTopology = enabled ? ServiceTopology.GetCurrentLegacyServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\sms\\TextMessagingHelper.cs", "GetEcpServiceUrl", 371) : ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\sms\\TextMessagingHelper.cs", "GetEcpServiceUrl", 371);
			if (serviceTopology != null)
			{
				IList<EcpService> list = serviceTopology.FindAll<EcpService>(principal, type, "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\sms\\TextMessagingHelper.cs", "GetEcpServiceUrl", 374);
				if (list != null && 0 < list.Count)
				{
					return list[0].Url;
				}
			}
			return null;
		}

		private static Uri GetE15MultitenancyEcpServiceUrl(ExchangePrincipal exchangePrincipal, ClientAccessType type)
		{
			ExTraceGlobals.ApplicationlogicTracer.TraceDebug(0L, "Entering GetE15MultitenancyEcpServiceUrl");
			Uri uri = null;
			Exception ex = null;
			try
			{
				switch (type)
				{
				case ClientAccessType.Internal:
					uri = BackEndLocator.GetBackEndEcpUrl(exchangePrincipal.MailboxInfo);
					break;
				case ClientAccessType.External:
					uri = FrontEndLocator.GetFrontEndEcpUrl(exchangePrincipal);
					break;
				default:
					ExAssert.RetailAssert(false, "Invalid client access type {0}", new object[]
					{
						type
					});
					break;
				}
				ExTraceGlobals.ApplicationlogicTracer.TraceDebug<Uri>(0L, "ecpUri='{0}'", uri);
			}
			catch (ServerNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (ADTransientException ex3)
			{
				ex = ex3;
			}
			catch (DataSourceOperationException ex4)
			{
				ex = ex4;
			}
			catch (DataValidationException ex5)
			{
				ex = ex5;
			}
			finally
			{
				if (ex != null)
				{
					ExTraceGlobals.ApplicationlogicTracer.TraceError<Exception>(0L, "GetE15MultitenancyEcpServiceUrl throwing an exception: {0}", ex);
				}
			}
			return uri;
		}

		public static Uri GetEcpUrl(ExchangePrincipal principal)
		{
			return TextMessagingHelper.GetEcpServiceUrl(principal, ClientAccessType.External) ?? TextMessagingHelper.GetEcpServiceUrl(principal, ClientAccessType.Internal);
		}

		public static bool IsMachineToPersonTextingOnlyAccount(TextMessagingAccount account)
		{
			if (account == null)
			{
				throw new ArgumentNullException("account");
			}
			return !account.EasEnabled && account.NotificationPhoneNumberVerified && null != account.NotificationPhoneNumber && !string.IsNullOrEmpty(account.NotificationPhoneNumber.Number);
		}

		public static bool ShouldReturnMobile(MailboxSession mailboxSession)
		{
			if (mailboxSession == null)
			{
				return false;
			}
			if (!mailboxSession.SupportedRoutingTypes.Contains("MOBILE"))
			{
				return false;
			}
			try
			{
				using (VersionedXmlDataProvider versionedXmlDataProvider = new VersionedXmlDataProvider(mailboxSession))
				{
					TextMessagingAccount textMessagingAccount = (TextMessagingAccount)versionedXmlDataProvider.Read<TextMessagingAccount>(mailboxSession.MailboxOwner.ObjectId);
					return textMessagingAccount.EasEnabled;
				}
			}
			catch (AccessDeniedException arg)
			{
				ExTraceGlobals.ApplicationlogicTracer.TraceError<AccessDeniedException>((long)typeof(TextMessagingHelper).GetHashCode(), "ShouldReturnMobile throwing an exception: {0}", arg);
			}
			return false;
		}
	}
}
