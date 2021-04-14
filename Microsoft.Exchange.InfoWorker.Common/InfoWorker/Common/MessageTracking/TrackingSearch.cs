using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Logging.Search;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class TrackingSearch
	{
		internal static LogSearchCursor GetCursorForMessageId(string server, ServerVersion version, string messageId)
		{
			return TrackingSearch.GetCursorForMessageId(server, version, "MSGTRK", messageId);
		}

		internal static LogSearchCursor GetCursorForMessageId(string server, ServerVersion version, string logFile, string messageId)
		{
			return TrackingSearch.GetCursor(server, version, logFile, messageId, null, CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime, DateTime.UtcNow.AddDays(1.0));
		}

		internal static LogSearchCursor GetCursorForMessageId(string server, ServerVersion version, string logFile, string messageId, DateTime startTime, DateTime endTime)
		{
			return TrackingSearch.GetCursor(server, version, logFile, messageId, null, startTime, endTime);
		}

		internal static LogSearchCursor GetCursor(string server, ServerVersion version, string logFile, string messageId, ProxyAddressCollection senderProxyAddresses, DateTime startTime, DateTime endTime)
		{
			LogQuery logQuery = new LogQuery();
			LogAndCondition logAndCondition = new LogAndCondition();
			if (!string.IsNullOrEmpty(messageId))
			{
				LogCondition fieldStringComparison = TrackingSearch.GetFieldStringComparison(MessageTrackingField.MessageId, CsvFieldCache.NormalizeMessageID(messageId));
				logAndCondition.Conditions.Add(fieldStringComparison);
			}
			if (senderProxyAddresses != null)
			{
				LogOrCondition logOrCondition = new LogOrCondition();
				foreach (ProxyAddress proxyAddress in senderProxyAddresses)
				{
					LogCondition fieldStringComparison2 = TrackingSearch.GetFieldStringComparison(MessageTrackingField.SenderAddress, proxyAddress.AddressString);
					logOrCondition.Conditions.Add(fieldStringComparison2);
				}
				logAndCondition.Conditions.Add(logOrCondition);
			}
			logQuery.Filter = logAndCondition;
			logQuery.Beginning = startTime;
			logQuery.End = endTime;
			return new LogSearchCursor(MessageTrackingSchema.MessageTrackingEvent, server, version, logFile, logQuery, null);
		}

		internal static LogCondition GetRecipientCondition(ProxyAddressCollection[] addresses)
		{
			LogOrCondition logOrCondition = new LogOrCondition();
			int num = 0;
			int num2 = 0;
			foreach (ProxyAddressCollection proxyAddressCollection in addresses)
			{
				foreach (ProxyAddress proxyAddress in proxyAddressCollection)
				{
					if (proxyAddress.Prefix == ProxyAddressPrefix.Smtp)
					{
						logOrCondition.Conditions.Add(TrackingSearch.GetSingleRecipientCondition(proxyAddress.AddressString, string.Concat(new object[]
						{
							"r",
							num,
							",",
							num2
						})));
						num2++;
					}
				}
				num++;
			}
			return logOrCondition;
		}

		private static LogCondition GetSingleRecipientCondition(string address, string variableName)
		{
			LogConditionField logConditionField = new LogConditionField();
			logConditionField.Name = MessageTrackingSchema.MessageTrackingEvent.Fields[12].Name;
			LogConditionVariable logConditionVariable = new LogConditionVariable();
			logConditionVariable.Name = variableName;
			LogConditionConstant logConditionConstant = new LogConditionConstant();
			logConditionConstant.Value = address;
			LogStringComparisonCondition logStringComparisonCondition = new LogStringComparisonCondition();
			logStringComparisonCondition.Left = logConditionVariable;
			logStringComparisonCondition.Right = logConditionConstant;
			logStringComparisonCondition.IgnoreCase = true;
			logStringComparisonCondition.Operator = LogComparisonOperator.Equals;
			return new LogForAnyCondition
			{
				Field = logConditionField,
				Variable = logConditionVariable,
				Condition = logStringComparisonCondition
			};
		}

		private static LogCondition GetFieldStringContainsComparison(MessageTrackingField field, string value)
		{
			LogConditionField logConditionField = new LogConditionField();
			logConditionField.Name = MessageTrackingSchema.MessageTrackingEvent.Fields[(int)field].Name;
			LogConditionConstant logConditionConstant = new LogConditionConstant();
			logConditionConstant.Value = value;
			return new LogStringContainsCondition
			{
				Left = logConditionField,
				Right = logConditionConstant,
				IgnoreCase = true
			};
		}

		internal static LogCondition GetFieldStringComparison(MessageTrackingField field, string value)
		{
			LogConditionField logConditionField = new LogConditionField();
			logConditionField.Name = MessageTrackingSchema.MessageTrackingEvent.Fields[(int)field].Name;
			LogConditionConstant logConditionConstant = new LogConditionConstant();
			logConditionConstant.Value = value;
			return new LogStringComparisonCondition
			{
				Left = logConditionField,
				Right = logConditionConstant,
				IgnoreCase = true,
				Operator = LogComparisonOperator.Equals
			};
		}

		internal static LogCondition GetFieldStringComparison(MessageTrackingField field, string[] values)
		{
			LogOrCondition logOrCondition = new LogOrCondition();
			foreach (string value in values)
			{
				LogCondition fieldStringComparison = TrackingSearch.GetFieldStringComparison(field, value);
				logOrCondition.Conditions.Add(fieldStringComparison);
			}
			return logOrCondition;
		}

		internal static string RemoveAngleBracketsIfNeeded(string value)
		{
			if (value[0] == '<' && value[value.Length - 1] == '>')
			{
				return value.Substring(1, value.Length - 2);
			}
			return value;
		}

		internal static List<ADRecipient> GetUsersGrantedSendOnBehalfPermission(ADRecipient mailbox, IRecipientSession galSession)
		{
			MultiValuedProperty<ADObjectId> grantSendOnBehalfTo = mailbox.GrantSendOnBehalfTo;
			List<ADRecipient> list = null;
			if (grantSendOnBehalfTo != null && grantSendOnBehalfTo.Count > 0)
			{
				list = new List<ADRecipient>(grantSendOnBehalfTo.Count);
				foreach (ADObjectId entryId in grantSendOnBehalfTo)
				{
					ADRecipient adrecipient = galSession.Read(entryId);
					if (adrecipient != null)
					{
						list.Add(adrecipient);
						TraceWrapper.SearchLibraryTracer.TraceDebug<string, string>(0, "Added {0} to list of users who have SendOnBehlf permission for {1}.", adrecipient.DisplayName, mailbox.DisplayName);
					}
				}
			}
			return list;
		}

		internal static List<ADRecipient> GetUsersGrantedSendAsPermission(ADRecipient mailbox, IRecipientSession galSession)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug<ADRecipient>(0, "Reading users who have SendAs rights for: {0}", mailbox);
			RawSecurityDescriptor rawSecurityDescriptor = galSession.ReadSecurityDescriptor(mailbox.Id);
			if (rawSecurityDescriptor == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug(0, "Null security-descriptor returned for mailbox", new object[0]);
				return null;
			}
			byte[] array = new byte[rawSecurityDescriptor.BinaryLength];
			rawSecurityDescriptor.GetBinaryForm(array, 0);
			ActiveDirectorySecurity activeDirectorySecurity = new ActiveDirectorySecurity();
			activeDirectorySecurity.SetSecurityDescriptorBinaryForm(array);
			AuthorizationRuleCollection accessRules = activeDirectorySecurity.GetAccessRules(true, false, typeof(SecurityIdentifier));
			if (accessRules == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug(0, "No rules on ACL for this mailbox", new object[0]);
				return null;
			}
			List<ADRecipient> list = null;
			foreach (object obj in accessRules)
			{
				ActiveDirectoryAccessRule activeDirectoryAccessRule = (ActiveDirectoryAccessRule)obj;
				if (activeDirectoryAccessRule.AccessControlType == AccessControlType.Allow && object.Equals(activeDirectoryAccessRule.ObjectType, WellKnownGuid.SendAsExtendedRightGuid))
				{
					IdentityReference identityReference = activeDirectoryAccessRule.IdentityReference;
					string value = identityReference.Value;
					try
					{
						ADRecipient adrecipient = galSession.FindBySid(new SecurityIdentifier(value));
						if (adrecipient == null)
						{
							TraceWrapper.SearchLibraryTracer.TraceError<string>(0, "User not found for SID: {0}", value);
						}
						else
						{
							if (list == null)
							{
								list = new List<ADRecipient>();
							}
							list.Add(adrecipient);
							TraceWrapper.SearchLibraryTracer.TraceDebug<string, string>(0, "Added {0} to list of users who have Send-As permission for {1}.", adrecipient.DisplayName, mailbox.DisplayName);
						}
					}
					catch (NonUniqueRecipientException arg)
					{
						TraceWrapper.SearchLibraryTracer.TraceError<string, string, NonUniqueRecipientException>(0, "Caught NonUniqueRecipientException when attempting to look up user with SID {0} while reading list of users granted Send-As permission to {1}: {2}", value, mailbox.Name, arg);
					}
				}
			}
			return list;
		}

		internal static readonly TimeSpan TimeoutInterval = new TimeSpan(0, 0, 60);
	}
}
