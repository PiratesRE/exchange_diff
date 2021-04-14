using System;
using System.Collections.Generic;
using Microsoft.Exchange.Hygiene.Data.Directory;

namespace Microsoft.Forefront.Reporting.Common
{
	internal class QueryPropertyField : QueryField
	{
		internal QueryPropertyField(QueryGroupField parent, int startPos, int endPos) : base(parent, startPos, endPos)
		{
		}

		internal override string Compile()
		{
			int num = base.QueryString.IndexOf(':', base.StartPosition, base.EndPosition - base.StartPosition);
			if (base.StartPosition >= num)
			{
				throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidProperty, string.Empty, base.StartPosition);
			}
			string text = base.QueryString.Substring(base.StartPosition, num - base.StartPosition).Trim();
			QueryProperty queryProperty;
			try
			{
				queryProperty = (QueryProperty)Enum.Parse(typeof(QueryProperty), text);
			}
			catch (FormatException innerException)
			{
				throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidProperty, text, base.StartPosition, innerException);
			}
			switch (base.Compiler.Type)
			{
			case OnDemandQueryType.MTSummary:
			case OnDemandQueryType.MTDetail:
				if (!QueryPropertyField.mtSupportedProperties.Contains(queryProperty))
				{
					throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.UnsupportedProperty, text, base.StartPosition);
				}
				break;
			case OnDemandQueryType.DLP:
			case OnDemandQueryType.Rule:
			case OnDemandQueryType.AntiSpam:
			case OnDemandQueryType.AntiVirus:
				if (!QueryPropertyField.ruleHitSupportedProperties.Contains(queryProperty))
				{
					throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.UnsupportedProperty, text, base.StartPosition);
				}
				break;
			default:
				throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidQueryType, base.Compiler.Type.ToString(), base.StartPosition);
			}
			string text2 = base.QueryString.Substring(num + 1, base.EndPosition - num - 1).Trim();
			if (!text2.StartsWith("\"") || !text2.EndsWith("\""))
			{
				throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.MissingQuote, queryProperty.ToString(), num);
			}
			text2 = text2.Trim(new char[]
			{
				'"'
			});
			string result;
			try
			{
				switch (queryProperty)
				{
				case QueryProperty.FromDate:
				{
					if (base.Parent.FromDate != null)
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.DuplicateField, queryProperty.ToString(), base.StartPosition);
					}
					DateTime value = new DateTime(DateTime.Parse(text2).Ticks, DateTimeKind.Utc);
					base.Parent.FromDate = new DateTime?(value);
					result = string.Format("originTimeTicks >= {0}", value.Ticks);
					break;
				}
				case QueryProperty.ToDate:
				{
					if (base.Parent.ToDate != null)
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.DuplicateField, queryProperty.ToString(), base.StartPosition);
					}
					if (base.Parent.FromDate == null)
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.MissingRequiredProperty, QueryProperty.FromDate.ToString(), base.StartPosition);
					}
					DateTime dateTime = new DateTime(DateTime.Parse(text2).Ticks, DateTimeKind.Utc);
					if (dateTime > DateTime.UtcNow)
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.ValueOutRange, queryProperty.ToString(), base.StartPosition);
					}
					if (dateTime.Date == dateTime)
					{
						dateTime = dateTime.AddDays(1.0);
					}
					if (dateTime <= base.Parent.FromDate.Value)
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidValue, queryProperty.ToString(), base.StartPosition);
					}
					base.Compiler.QueryTimeRange.Add(Tuple.Create<DateTime, DateTime>(base.Parent.FromDate.Value, dateTime));
					base.Parent.ToDate = new DateTime?(dateTime);
					result = string.Format("originTimeTicks <= {0}", dateTime.Ticks);
					break;
				}
				case QueryProperty.Direction:
				{
					Directionality directionality = (Directionality)Enum.Parse(typeof(Directionality), text2);
					result = string.Format("direction == {0}", (byte)directionality);
					break;
				}
				case QueryProperty.Sender:
				{
					string[] array = text2.Split(new char[]
					{
						'@'
					});
					if (array.Length != 2)
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidValue, queryProperty.ToString(), base.StartPosition);
					}
					string text3 = array[0];
					string text4 = array[1];
					if (text4.Contains("\""))
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidValue, queryProperty.ToString(), base.StartPosition);
					}
					base.HasOptionalCriterion = true;
					if (text3 == "*")
					{
						result = string.Format("OnDemandQueryUtil.SenderMatch(senderAddress, \"{0}\")", text4);
					}
					else
					{
						result = string.Format("OnDemandQueryUtil.SenderMatch(senderAddress, \"{0}\", \"{1}\")", text4, base.Compiler.GetPIIHash(text3));
					}
					break;
				}
				case QueryProperty.Recipient:
				{
					string[] array2 = text2.Split(new char[]
					{
						'@'
					});
					if (array2.Length != 2)
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidValue, queryProperty.ToString(), base.StartPosition);
					}
					string text5 = array2[0];
					string text6 = array2[1];
					if (text6.Contains("\""))
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidValue, queryProperty.ToString(), base.StartPosition);
					}
					base.HasOptionalCriterion = true;
					base.Parent.NeedToFillInMsgStatus = true;
					if (text5 == "*")
					{
						result = string.Format("OnDemandQueryUtil.RecipientMatch(msgStatus, \"{0}\"{1})", text6, "%MsgStatus%");
					}
					else
					{
						result = string.Format("OnDemandQueryUtil.RecipientMatch(msgStatus, \"{0}\", \"{1}\"{2})", text6, base.Compiler.GetPIIHash(text5), "%MsgStatus%");
					}
					break;
				}
				case QueryProperty.MsgID:
					if (text2.Contains("\""))
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidValue, queryProperty.ToString(), base.StartPosition);
					}
					if (text2 == "*")
					{
						result = string.Empty;
					}
					else
					{
						base.HasOptionalCriterion = true;
						result = string.Format("messageId.IndexOf(@\"{0}\", StringComparison.OrdinalIgnoreCase) >= 0", text2);
					}
					break;
				case QueryProperty.MsgStatus:
				{
					if (base.Parent.MsgStatus != null)
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.DuplicateField, queryProperty.ToString(), base.StartPosition);
					}
					StatusFlags statusFlags = (StatusFlags)Enum.Parse(typeof(StatusFlags), text2);
					StatusFlags statusFlags2 = statusFlags;
					if (statusFlags2 <= StatusFlags.Send)
					{
						if (statusFlags2 != StatusFlags.Expand && statusFlags2 != StatusFlags.Send)
						{
							goto IL_4C4;
						}
					}
					else if (statusFlags2 != StatusFlags.Deliver)
					{
						if (statusFlags2 != StatusFlags.Fail)
						{
							goto IL_4C4;
						}
						base.Parent.MsgStatus = new StatusFlags?(statusFlags);
						base.HasOptionalCriterion = true;
						base.Parent.NeedToFillInMsgStatus = true;
						result = null;
						break;
					}
					base.Parent.MsgStatus = new StatusFlags?(statusFlags);
					base.Parent.NeedToFillInMsgStatus = true;
					result = null;
					break;
					IL_4C4:
					throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.UnsupportedValue, queryProperty.ToString(), base.StartPosition);
				}
				case QueryProperty.SenderIP:
					base.HasOptionalCriterion = true;
					result = string.Format("(originalIP != null && originalIP.Contains(\"{0}\"))", base.Compiler.GetPIIHash(text2));
					break;
				case QueryProperty.RuleID:
					result = string.Format("tra_etr.IndexOf(\"ruleid={0}\", StringComparison.OrdinalIgnoreCase) >= 0", Guid.Parse(text2).ToString());
					break;
				case QueryProperty.RuleAction:
					if (!QueryPropertyField.supportedRuleActions.Contains(text2))
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.UnsupportedValue, queryProperty.ToString(), base.StartPosition);
					}
					result = string.Format("tra_etr.IndexOf(\"action={0}\", StringComparison.OrdinalIgnoreCase) >= 0", text2);
					break;
				case QueryProperty.DLPID:
					result = string.Format("tra_etr.IndexOf(\"dlpid={0}\", StringComparison.OrdinalIgnoreCase) >= 0", Guid.Parse(text2).ToString());
					break;
				case QueryProperty.DLPAction:
					if (!QueryPropertyField.supportedRuleActions.Contains(text2))
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.UnsupportedValue, queryProperty.ToString(), base.StartPosition);
					}
					result = string.Format("tra_etr.IndexOf(\"action={0}\", StringComparison.OrdinalIgnoreCase) >= 0", text2);
					break;
				case QueryProperty.DLPAuditSeverity:
					if (text2 != "1" && text2 != "2" && text2 != "3")
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.UnsupportedValue, queryProperty.ToString(), base.StartPosition);
					}
					result = string.Format("tra_etr.IndexOf(\"sev={0}\", StringComparison.OrdinalIgnoreCase) >= 0", text2);
					break;
				case QueryProperty.DLPSenderOverride:
					if (text2 != "or" && text2 != "fp")
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.UnsupportedValue, queryProperty.ToString(), base.StartPosition);
					}
					result = string.Format("tra_etr.IndexOf(\"sndoverride={0}\", StringComparison.OrdinalIgnoreCase) >= 0", text2);
					break;
				case QueryProperty.Subject:
					if (text2.Length < 2)
					{
						throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidValue, queryProperty.ToString(), base.StartPosition);
					}
					base.HasOptionalCriterion = true;
					result = string.Format("PiiUtil.MatchLSH(PiiUtil.ExtractLSH(messageSubject), \"{0}\", ST)", base.Compiler.GetLSH(text2));
					break;
				default:
					throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.UnsupportedProperty, queryProperty.ToString(), base.StartPosition);
				}
			}
			catch (FormatException innerException2)
			{
				throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidValue, queryProperty.ToString(), num, innerException2);
			}
			return result;
		}

		private static HashSet<QueryProperty> mtSupportedProperties = new HashSet<QueryProperty>
		{
			QueryProperty.FromDate,
			QueryProperty.ToDate,
			QueryProperty.Direction,
			QueryProperty.Sender,
			QueryProperty.Recipient,
			QueryProperty.MsgID,
			QueryProperty.MsgStatus,
			QueryProperty.SenderIP,
			QueryProperty.Subject
		};

		private static HashSet<QueryProperty> ruleHitSupportedProperties = new HashSet<QueryProperty>
		{
			QueryProperty.FromDate,
			QueryProperty.ToDate,
			QueryProperty.Direction,
			QueryProperty.Sender,
			QueryProperty.Recipient,
			QueryProperty.MsgID,
			QueryProperty.MsgStatus,
			QueryProperty.SenderIP,
			QueryProperty.RuleID,
			QueryProperty.RuleAction,
			QueryProperty.DLPID,
			QueryProperty.DLPAction,
			QueryProperty.DLPAuditSeverity,
			QueryProperty.DLPSenderOverride,
			QueryProperty.Subject
		};

		private static HashSet<string> supportedRuleActions = new HashSet<string>
		{
			"AddBccRecipient",
			"Quarantine",
			"RedirectMessage",
			"RejectMessage",
			"PrependSubject",
			"ApplyClassification",
			"ApplyHtmlDisclaimer",
			"RightsProtectMessage",
			"SetMessageHeader",
			"RemoveMessageHeader",
			"AddToRecipient",
			"AddCcRecipient",
			"AddManagerAsRecipient",
			"ModerateMessageByUser",
			"ModerateMessageByManager",
			"DeleteMessage",
			"StopRuleProcessing",
			"RequireTLS",
			"GenerateIncidentReport",
			"GenerateNotification",
			"NotifySender",
			"RouteMessageUsingConnector",
			"SetAuditSeverity"
		};
	}
}
