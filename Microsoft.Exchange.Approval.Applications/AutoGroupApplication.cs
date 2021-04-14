using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Approval.Applications.Resources;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.Approval;

namespace Microsoft.Exchange.Approval.Applications
{
	internal class AutoGroupApplication : ApprovalApplication
	{
		internal AutoGroupApplication()
		{
		}

		internal static bool GetItemRecipients(MessageItem message, out string inetMsgId, out SmtpAddress approver, out SmtpAddress requester, out SmtpAddress[] moderators)
		{
			AutoGroupApplication.diag.TraceFunction(0L, "AutoGroupApplication.GetItemRecipients");
			inetMsgId = message.InternetMessageId;
			approver = SmtpAddress.Empty;
			requester = SmtpAddress.Empty;
			moderators = new SmtpAddress[0];
			string valueOrDefault = message.GetValueOrDefault<string>(MessageItemSchema.ApprovalDecisionMaker);
			string valueOrDefault2 = message.GetValueOrDefault<string>(MessageItemSchema.ApprovalAllowedDecisionMakers);
			string valueOrDefault3 = message.GetValueOrDefault<string>(MessageItemSchema.ApprovalRequestor);
			if (string.IsNullOrEmpty(valueOrDefault3))
			{
				AutoGroupApplication.diag.TraceError<string>(0L, "'{0}' requester is null or empty", inetMsgId);
				return false;
			}
			if (string.IsNullOrEmpty(valueOrDefault))
			{
				AutoGroupApplication.diag.TraceError<string>(0L, "'{0}' approver is null or empty", inetMsgId);
				return false;
			}
			approver = new SmtpAddress(valueOrDefault);
			requester = new SmtpAddress(valueOrDefault3);
			List<SmtpAddress> list = new List<SmtpAddress>();
			foreach (string text in valueOrDefault2.Split(new char[]
			{
				';'
			}))
			{
				SmtpAddress item = new SmtpAddress(text);
				list.Add(item);
				if (!item.IsValidAddress)
				{
					AutoGroupApplication.diag.TraceError<string, string>(0L, "'{0}' moderator list has invalid entry '{1}'", inetMsgId, text);
				}
			}
			moderators = list.ToArray();
			return approver.IsValidAddress && requester.IsValidAddress;
		}

		internal static SmtpAddress GetEmailAddressFromMailboxItem(MessageItem message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (message.Session == null || string.IsNullOrEmpty(message.Session.DisplayAddress) || !SmtpAddress.IsValidSmtpAddress(message.Session.DisplayAddress))
			{
				return SmtpAddress.Empty;
			}
			return new SmtpAddress(message.Session.DisplayAddress);
		}

		internal static string BuildApprovalData(string command, ADObjectId identity)
		{
			AutoGroupApplication.diag.TraceFunction<string, string>(0L, "AutoGroupApplication.BuildApprovalData '{0}' '{1}'", command, identity.ToString());
			if (command.IndexOf(',') != -1)
			{
				throw new ArgumentException("command");
			}
			if (identity.ObjectGuid == Guid.Empty)
			{
				throw new ArgumentException("identity");
			}
			return command + "," + identity.ObjectGuid.ToString();
		}

		internal static bool ParseApprovalData(string approvalData, out string command, out ADObjectId identity)
		{
			AutoGroupApplication.diag.TraceFunction<string>(0L, "AutoGroupApplication.ParseApprovalData '{0}'", approvalData);
			command = null;
			identity = null;
			int num = approvalData.IndexOf(',');
			if (num == -1)
			{
				return false;
			}
			command = approvalData.Substring(0, num);
			try
			{
				identity = new ADObjectId(new Guid(approvalData.Substring(num + 1)));
			}
			catch (FormatException)
			{
				return false;
			}
			catch (OverflowException)
			{
				return false;
			}
			return true;
		}

		internal override bool OnApprove(MessageItem message)
		{
			AutoGroupApplication.diag.TraceFunction((long)this.GetHashCode(), "AutoGroupApplication.OnApprove");
			message.Load(new PropertyDefinition[]
			{
				MessageItemSchema.ReceivedBy,
				MessageItemSchema.ApprovalDecisionMaker,
				MessageItemSchema.ApprovalRequestor,
				MessageItemSchema.ApprovalAllowedDecisionMakers,
				MessageItemSchema.ApprovalApplicationData
			});
			CultureInfo cultureInfo = null;
			CultureInfo messageCulture = null;
			string arg;
			SmtpAddress smtpAddress;
			SmtpAddress smtpAddress2;
			SmtpAddress[] array;
			if (!AutoGroupApplication.GetItemRecipients(message, out arg, out smtpAddress, out smtpAddress2, out array))
			{
				AutoGroupApplication.diag.TraceError<string, SmtpAddress, SmtpAddress>((long)this.GetHashCode(), "'{0}' message has invalid approver '{1}', requester '{2}', or empty moderators list.", arg, smtpAddress, smtpAddress2);
				return false;
			}
			SmtpAddress emailAddressFromMailboxItem = AutoGroupApplication.GetEmailAddressFromMailboxItem(message);
			if (emailAddressFromMailboxItem == SmtpAddress.Empty)
			{
				AutoGroupApplication.diag.TraceError<string>((long)this.GetHashCode(), "'{0}' cannot get arbitration mailbox SMTP address from message.", arg);
				return false;
			}
			string valueOrDefault = message.GetValueOrDefault<string>(MessageItemSchema.ApprovalApplicationData);
			string text;
			ADObjectId adobjectId;
			if (string.IsNullOrEmpty(valueOrDefault) || !AutoGroupApplication.ParseApprovalData(valueOrDefault, out text, out adobjectId))
			{
				AutoGroupApplication.diag.TraceError<string, string>((long)this.GetHashCode(), "'{0}' applicationData '{1}' cannot be parsed", arg, valueOrDefault);
				return false;
			}
			IRecipientSession recipientSession = ApprovalProcessor.CreateRecipientSessionFromSmtpAddress(smtpAddress);
			ADRawEntry adrawEntry = null;
			bool flag = true;
			try
			{
				adrawEntry = recipientSession.FindByProxyAddress(new SmtpProxyAddress(smtpAddress.ToString(), true), new PropertyDefinition[]
				{
					ADObjectSchema.Id,
					ADUserSchema.Languages
				});
			}
			catch (NonUniqueRecipientException)
			{
				flag = false;
			}
			ADRawEntry adrawEntry2 = null;
			bool flag2 = true;
			try
			{
				adrawEntry2 = recipientSession.FindByProxyAddress(new SmtpProxyAddress(smtpAddress2.ToString(), true), new PropertyDefinition[]
				{
					ADObjectSchema.Id,
					ADRecipientSchema.PrimarySmtpAddress,
					ADUserSchema.Languages
				});
			}
			catch (NonUniqueRecipientException)
			{
				flag2 = false;
			}
			MiniRecipient miniRecipient = recipientSession.ReadMiniRecipient(adobjectId, null);
			if (miniRecipient == null)
			{
				AutoGroupApplication.diag.TraceError<string, ADObjectId>((long)this.GetHashCode(), "'{0}' group '{1}' not found in AD.", arg, adobjectId);
				return false;
			}
			string group;
			if (string.IsNullOrEmpty(miniRecipient.DisplayName))
			{
				group = miniRecipient.Name;
			}
			else
			{
				group = miniRecipient.DisplayName;
			}
			if (adrawEntry == null)
			{
				if (flag)
				{
					AutoGroupApplication.diag.TraceError<string, SmtpAddress>((long)this.GetHashCode(), "'{0}' approver '{1}' not found in AD.", arg, smtpAddress);
				}
				else
				{
					AutoGroupApplication.diag.TraceError<string, SmtpAddress>((long)this.GetHashCode(), "'{0}' approver '{1}' not unique in AD.", arg, smtpAddress);
				}
				ApprovalProcessor.SendNotification(emailAddressFromMailboxItem, message.Id.ObjectId, new SmtpAddress[]
				{
					smtpAddress
				}, Strings.AutoGroupRequestFailedSubject(group), Strings.AutoGroupRequestFailedHeader(group), Strings.AutoGroupRequestFailedBodyBadApprover(group, ApprovalProcessor.GetDisplayNameFromSmtpAddress(smtpAddress2.ToString()), ApprovalProcessor.GetDisplayNameFromSmtpAddress(smtpAddress.ToString())), LocalizedString.Empty, null);
				return false;
			}
			MultiValuedProperty<CultureInfo> multiValuedProperty = (MultiValuedProperty<CultureInfo>)adrawEntry[ADUserSchema.Languages];
			if (multiValuedProperty.Count > 0)
			{
				cultureInfo = multiValuedProperty[0];
			}
			if (adrawEntry2 == null)
			{
				if (flag2)
				{
					AutoGroupApplication.diag.TraceError<string, SmtpAddress>((long)this.GetHashCode(), "'{0}' requester '{1}' not found in AD.", arg, smtpAddress2);
				}
				else
				{
					AutoGroupApplication.diag.TraceError<string, SmtpAddress>((long)this.GetHashCode(), "'{0}' requester '{1}' not unique in AD.", arg, smtpAddress2);
				}
				ApprovalProcessor.SendNotification(emailAddressFromMailboxItem, message.Id.ObjectId, new SmtpAddress[]
				{
					smtpAddress
				}, Strings.AutoGroupRequestFailedSubject(group), Strings.AutoGroupRequestFailedHeader(group), Strings.AutoGroupRequestFailedBodyBadRequester(group, ApprovalProcessor.GetDisplayNameFromSmtpAddress(smtpAddress2.ToString())), LocalizedString.Empty, cultureInfo);
				return false;
			}
			MultiValuedProperty<CultureInfo> multiValuedProperty2 = (MultiValuedProperty<CultureInfo>)adrawEntry2[ADUserSchema.Languages];
			if (multiValuedProperty2.Count > 0)
			{
				messageCulture = multiValuedProperty2[0];
			}
			bool flag3 = false;
			string a;
			if ((a = text) != null)
			{
				if (!(a == "Add-DistributionGroupMember"))
				{
					if (!(a == "Remove-DistributionGroupMember"))
					{
						goto IL_393;
					}
					flag3 = true;
				}
				PSCommand pscommand = new PSCommand().AddCommand(text);
				pscommand.AddParameter("Identity", adobjectId);
				pscommand.AddParameter("Member", adrawEntry2.Id);
				if (flag3)
				{
					pscommand.AddParameter("Confirm", new SwitchParameter(false));
				}
				AutoGroupApplication.diag.Information<string, string>((long)this.GetHashCode(), "'{0}' executing command '{1}'.", arg, pscommand.ToString());
				string text2;
				string text3;
				ApprovalApplication.ExecuteCommandsInRunspace(smtpAddress, pscommand, cultureInfo, out text2, out text3);
				if (!text2.Equals(string.Empty))
				{
					AutoGroupApplication.diag.TraceError<string, string, string>((long)this.GetHashCode(), "'{0}' command '{1}' failed with error {2}.", arg, pscommand.ToString(), text2);
					ApprovalProcessor.SendNotification(emailAddressFromMailboxItem, message.Id.ObjectId, new SmtpAddress[]
					{
						smtpAddress
					}, Strings.AutoGroupRequestFailedSubject(group), Strings.AutoGroupRequestFailedHeader(group), Strings.AutoGroupRequestFailedBodyTaskError(text2), LocalizedString.Empty, cultureInfo);
					return false;
				}
				AutoGroupApplication.diag.Information<string, string>((long)this.GetHashCode(), "'{0}' command '{1}' completed successfully.", arg, pscommand.ToString());
				ApprovalProcessor.SendApproveNotification(emailAddressFromMailboxItem, message.Id.ObjectId, new SmtpAddress[]
				{
					(SmtpAddress)adrawEntry2[ADRecipientSchema.PrimarySmtpAddress]
				}, Strings.AutoGroupRequestApprovedSubject(group), Strings.AutoGroupRequestApprovedHeader(ApprovalProcessor.GetDisplayNameFromSmtpAddress(smtpAddress.ToString()), group), Strings.AutoGroupRequestApprovedBody, LocalizedString.Empty, messageCulture);
				return true;
			}
			IL_393:
			AutoGroupApplication.diag.TraceError<string, string>((long)this.GetHashCode(), "'{0}' command '{1}' not recognized.", arg, text);
			return false;
		}

		internal override bool OnReject(MessageItem message)
		{
			AutoGroupApplication.diag.TraceFunction((long)this.GetHashCode(), "AutoGroupApplication.OnReject");
			message.Load(new PropertyDefinition[]
			{
				MessageItemSchema.ReceivedBy,
				MessageItemSchema.ApprovalDecisionMaker,
				MessageItemSchema.ApprovalRequestor,
				MessageItemSchema.ApprovalAllowedDecisionMakers,
				MessageItemSchema.ApprovalApplicationData
			});
			CultureInfo cultureInfo = null;
			string arg;
			SmtpAddress arg2;
			SmtpAddress smtpAddress;
			SmtpAddress[] array;
			if (!AutoGroupApplication.GetItemRecipients(message, out arg, out arg2, out smtpAddress, out array))
			{
				AutoGroupApplication.diag.TraceError<string, SmtpAddress, SmtpAddress>((long)this.GetHashCode(), "'{0}' message has invalid approver '{1}', requester '{2}', or empty moderators list.", arg, arg2, smtpAddress);
				return false;
			}
			SmtpAddress emailAddressFromMailboxItem = AutoGroupApplication.GetEmailAddressFromMailboxItem(message);
			if (emailAddressFromMailboxItem == SmtpAddress.Empty)
			{
				AutoGroupApplication.diag.TraceError<string>((long)this.GetHashCode(), "'{0}' cannot get arbitration mailbox SMTP address from message.", arg);
				return false;
			}
			string valueOrDefault = message.GetValueOrDefault<string>(MessageItemSchema.ApprovalApplicationData);
			string valueOrDefault2 = message.GetValueOrDefault<string>(MessageItemSchema.ApprovalAllowedDecisionMakers);
			string text;
			ADObjectId adobjectId;
			if (string.IsNullOrEmpty(valueOrDefault) || !AutoGroupApplication.ParseApprovalData(valueOrDefault, out text, out adobjectId))
			{
				AutoGroupApplication.diag.TraceError<string, string>((long)this.GetHashCode(), "'{0}' applicationData '{1}' cannot be parsed", arg, valueOrDefault);
				return false;
			}
			IRecipientSession recipientSession = ApprovalProcessor.CreateRecipientSessionFromSmtpAddress(smtpAddress);
			MiniRecipient miniRecipient = recipientSession.ReadMiniRecipient(adobjectId, null);
			if (miniRecipient == null)
			{
				AutoGroupApplication.diag.TraceError<string, ADObjectId>((long)this.GetHashCode(), "'{0}' group '{1}' not found in AD.", arg, adobjectId);
				return false;
			}
			string group;
			if (string.IsNullOrEmpty(miniRecipient.DisplayName))
			{
				group = miniRecipient.Name;
			}
			else
			{
				group = miniRecipient.DisplayName;
			}
			ADRawEntry adrawEntry = recipientSession.FindByProxyAddress(new SmtpProxyAddress(smtpAddress.ToString(), true), new PropertyDefinition[]
			{
				ADObjectSchema.Id,
				ADUserSchema.Languages
			});
			if (adrawEntry != null)
			{
				MultiValuedProperty<CultureInfo> multiValuedProperty = (MultiValuedProperty<CultureInfo>)adrawEntry[ADUserSchema.Languages];
				if (multiValuedProperty.Count > 0)
				{
					cultureInfo = multiValuedProperty[0];
				}
			}
			StringBuilder stringBuilder = new StringBuilder(" ");
			if (string.IsNullOrEmpty(valueOrDefault2))
			{
				AutoGroupApplication.diag.TraceError<string>(0L, "'{0}' moderator list is null or empty", arg);
				return false;
			}
			foreach (string address in valueOrDefault2.Split(new char[]
			{
				';'
			}))
			{
				stringBuilder.Append(ApprovalProcessor.GetDisplayNameFromSmtpAddress(address));
				stringBuilder.Append(Strings.Semicolon.ToString(cultureInfo));
			}
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
			ApprovalProcessor.SendRejectNotification(emailAddressFromMailboxItem, message.Id.ObjectId, new SmtpAddress[]
			{
				smtpAddress
			}, Strings.AutoGroupRequestRejectedSubject(group), Strings.AutoGroupRequestRejectedHeader(group), Strings.AutoGroupRequestRejectedBody(stringBuilder.ToString()), LocalizedString.Empty, cultureInfo);
			return true;
		}

		internal override bool OnExpire(MessageItem message, out bool sendUpdate)
		{
			AutoGroupApplication.diag.TraceFunction((long)this.GetHashCode(), "AutoGroupApplication.OnExpire");
			sendUpdate = true;
			message.Load(new PropertyDefinition[]
			{
				MessageItemSchema.ReceivedBy,
				MessageItemSchema.ApprovalDecisionMaker,
				MessageItemSchema.ApprovalRequestor,
				MessageItemSchema.ApprovalAllowedDecisionMakers,
				MessageItemSchema.ApprovalApplicationData
			});
			CultureInfo messageCulture = null;
			string arg;
			SmtpAddress arg2;
			SmtpAddress smtpAddress;
			SmtpAddress[] array;
			if (!AutoGroupApplication.GetItemRecipients(message, out arg, out arg2, out smtpAddress, out array))
			{
				AutoGroupApplication.diag.TraceError<string, SmtpAddress, SmtpAddress>((long)this.GetHashCode(), "'{0}' message has invalid approver '{1}', requester '{2}', or empty moderators list.", arg, arg2, smtpAddress);
				return false;
			}
			SmtpAddress emailAddressFromMailboxItem = AutoGroupApplication.GetEmailAddressFromMailboxItem(message);
			if (emailAddressFromMailboxItem == SmtpAddress.Empty)
			{
				AutoGroupApplication.diag.TraceError<string>((long)this.GetHashCode(), "'{0}' cannot get arbitration mailbox SMTP address from message.", arg);
				return false;
			}
			string valueOrDefault = message.GetValueOrDefault<string>(MessageItemSchema.ApprovalApplicationData);
			string text;
			ADObjectId adobjectId;
			if (string.IsNullOrEmpty(valueOrDefault) || !AutoGroupApplication.ParseApprovalData(valueOrDefault, out text, out adobjectId))
			{
				AutoGroupApplication.diag.TraceError<string, string>((long)this.GetHashCode(), "'{0}' applicationData '{1}' cannot be parsed", arg, valueOrDefault);
				return false;
			}
			IRecipientSession recipientSession = ApprovalProcessor.CreateRecipientSessionFromSmtpAddress(smtpAddress);
			MiniRecipient miniRecipient = recipientSession.ReadMiniRecipient(adobjectId, null);
			if (miniRecipient == null)
			{
				AutoGroupApplication.diag.TraceError<string, ADObjectId>((long)this.GetHashCode(), "'{0}' group '{1}' not found in AD.", arg, adobjectId);
				return false;
			}
			string group;
			if (string.IsNullOrEmpty(miniRecipient.DisplayName))
			{
				group = miniRecipient.Name;
			}
			else
			{
				group = miniRecipient.DisplayName;
			}
			ADRawEntry adrawEntry = recipientSession.FindByProxyAddress(new SmtpProxyAddress(smtpAddress.ToString(), true), new PropertyDefinition[]
			{
				ADObjectSchema.Id,
				ADUserSchema.Languages
			});
			if (adrawEntry != null)
			{
				MultiValuedProperty<CultureInfo> multiValuedProperty = (MultiValuedProperty<CultureInfo>)adrawEntry[ADUserSchema.Languages];
				if (multiValuedProperty.Count > 0)
				{
					messageCulture = multiValuedProperty[0];
				}
			}
			ApprovalProcessor.SendNotification(emailAddressFromMailboxItem, message.Id.ObjectId, new SmtpAddress[]
			{
				smtpAddress
			}, Strings.AutoGroupRequestExpiredSubject(group), Strings.AutoGroupRequestExpiredBody(group), LocalizedString.Empty, LocalizedString.Empty, messageCulture);
			return true;
		}

		internal const string JoinGroup = "Add-DistributionGroupMember";

		internal const string DepartGroup = "Remove-DistributionGroupMember";

		private static readonly Trace diag = ExTraceGlobals.AutoGroupTracer;
	}
}
