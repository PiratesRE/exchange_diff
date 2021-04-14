using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class NdrOofHandler
	{
		private NdrOofHandler(MessageItem messageItem)
		{
			this.messageItem = messageItem;
		}

		public static bool TryCreate(MessageItem messageItem, out NdrOofHandler ndrOofHandler)
		{
			ndrOofHandler = null;
			if (!NdrOofHandler.IsNdrOrOof(messageItem))
			{
				return false;
			}
			ndrOofHandler = new NdrOofHandler(messageItem);
			return true;
		}

		public ApprovalEngine.ApprovalProcessResults Process()
		{
			MailboxSession session = (MailboxSession)this.messageItem.Session;
			string text;
			string approvalRequestMessageId;
			if (!this.TryGetInitiationAndApprovalRequestMessageId(out text, out approvalRequestMessageId))
			{
				NdrOofHandler.diag.TraceDebug((long)this.GetHashCode(), "Ignoring NDR/OOF. Cannot get initiation/approval request message id's");
				return ApprovalEngine.ApprovalProcessResults.NdrOofInvalid;
			}
			long num;
			StoreObjectId storeObjectId = NdrOofHandler.FindInitiationMessage(session, text, approvalRequestMessageId, out num);
			NdrOofHandler.diag.TraceDebug<long>((long)this.GetHashCode(), "Time searching for initiation = {0} ms", num);
			if (storeObjectId == null)
			{
				NdrOofHandler.diag.TraceDebug<string>((long)this.GetHashCode(), "Initiation message not found, ignoring NDR/OOF, messageId={0}", text);
				return new ApprovalEngine.ApprovalProcessResults(ApprovalEngine.ProcessResult.InitiationNotFoundForNdrOrOof, num);
			}
			if (!this.WriteToInitiationMessage(session, storeObjectId, text))
			{
				NdrOofHandler.diag.TraceDebug<string>((long)this.GetHashCode(), "Initiation message cannot be updated with NDR/OOF, messageId={0}", text);
				return new ApprovalEngine.ApprovalProcessResults(ApprovalEngine.ProcessResult.NdrOrOofUpdateSkipped, num);
			}
			return new ApprovalEngine.ApprovalProcessResults(ApprovalEngine.ProcessResult.NdrOrOofUpdated, num);
		}

		internal static string FormatNdrOofProperty(int totalApprovers, int totalNdred, int totalOofed)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0};{1};{2}", new object[]
			{
				totalApprovers,
				totalNdred,
				totalOofed
			});
		}

		private static StoreObjectId FindInitiationMessage(MailboxSession session, string initiationMessageId, string approvalRequestMessageId, out long messageSearchElapsedMilliseconds)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			IStorePropertyBag[] array = AllItemsFolderHelper.FindItemsFromInternetId(session, initiationMessageId, NdrOofHandler.PropertiesNeededFromInitiationForFinding);
			StoreObjectId result = null;
			if (array != null)
			{
				NdrOofHandler.diag.TraceDebug<int, string>(0L, "Found {0} initiation messages for this NDR/OOF. Init messageId={1}.", array.Length, initiationMessageId);
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i].TryGetProperty(MessageItemSchema.ApprovalRequestMessageId) as string;
					string value;
					string text2;
					if (!string.IsNullOrEmpty(text) && FindMessageUtils.TryParseMessageId(text, out value, out text2))
					{
						approvalRequestMessageId.StartsWith(value, StringComparison.Ordinal);
						result = ((VersionedId)array[i][ItemSchema.Id]).ObjectId;
						break;
					}
				}
			}
			stopwatch.Stop();
			messageSearchElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			return result;
		}

		private static bool TryParseNdrOofProperty(string ndrOofValue, out int totalApprovers, out int totalNdred, out int totalOofed)
		{
			string[] array = ndrOofValue.Split(NdrOofHandler.NdrOofPropertySeparator, 3, StringSplitOptions.RemoveEmptyEntries);
			totalApprovers = 0;
			totalNdred = 0;
			totalOofed = 0;
			if (array.Length != 3)
			{
				NdrOofHandler.diag.TraceDebug<string>(0L, "Unexpected number of NDR /OOF counts from property='{0}'", ndrOofValue);
				return false;
			}
			if (!int.TryParse(array[0], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out totalApprovers))
			{
				NdrOofHandler.diag.TraceDebug<string>(0L, "Parsing for total approvers failed for '{0}'", ndrOofValue);
				return false;
			}
			if (!int.TryParse(array[1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out totalNdred))
			{
				NdrOofHandler.diag.TraceDebug<string>(0L, "Parsing for total NDRed approvers failed for '{0}'", ndrOofValue);
				return false;
			}
			if (!int.TryParse(array[2], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out totalOofed))
			{
				NdrOofHandler.diag.TraceDebug<string>(0L, "Parsing for total OOFed approvers failed for '{0}'", ndrOofValue);
				return false;
			}
			return true;
		}

		private static bool IsNdrOrOof(MessageItem messageItem)
		{
			return messageItem != null && messageItem.Session != null && (ObjectClass.IsOfClass(messageItem.ClassName, "IPM.Note.Rules.OofTemplate.Microsoft") || ObjectClass.IsDsnNegative(messageItem.ClassName));
		}

		private bool WriteToInitiationMessage(MailboxSession session, StoreObjectId storeId, string initMessageId)
		{
			ConflictResolutionResult conflictResolutionResult = null;
			try
			{
				for (int i = 0; i < 5; i++)
				{
					using (MessageItem messageItem = MessageItem.Bind(session, storeId, NdrOofHandler.PropertiesNeededToUpdateInitiation))
					{
						messageItem.OpenAsReadWrite();
						string ndrOofValue = (string)messageItem[MessageItemSchema.ApprovalDecisionMakersNdred];
						ApprovalStatus? valueAsNullable = messageItem.GetValueAsNullable<ApprovalStatus>(MessageItemSchema.ApprovalStatus);
						if (valueAsNullable == null)
						{
							NdrOofHandler.diag.TraceDebug<string>((long)this.GetHashCode(), "Invalid approval status property in initation message {0}. Ignored", initMessageId);
							return false;
						}
						if ((valueAsNullable & (ApprovalStatus.Approved | ApprovalStatus.Rejected)) != (ApprovalStatus)0)
						{
							NdrOofHandler.diag.TraceDebug<string>((long)this.GetHashCode(), "There is already a decision for {0}, there is no need to handle NDR or OOF", initMessageId);
							return false;
						}
						int num;
						int num2;
						int num3;
						if (!NdrOofHandler.TryParseNdrOofProperty(ndrOofValue, out num, out num2, out num3))
						{
							NdrOofHandler.diag.TraceDebug<string>((long)this.GetHashCode(), "Invalid ndr/oof property in initation message {0}. Ignored", initMessageId);
							return false;
						}
						int num4;
						int num5;
						this.GetNdrOrOofCountInMessage(out num4, out num5);
						num2 += num4;
						num3 += num5;
						messageItem[MessageItemSchema.ApprovalDecisionMakersNdred] = NdrOofHandler.FormatNdrOofProperty(num, num2, num3);
						if (num3 + num2 == num)
						{
							if (num3 == 0)
							{
								messageItem[MessageItemSchema.ApprovalStatus] = (valueAsNullable.Value | ApprovalStatus.Ndred);
							}
							else
							{
								messageItem[MessageItemSchema.ApprovalStatus] = (valueAsNullable.Value | ApprovalStatus.Oofed);
							}
						}
						conflictResolutionResult = messageItem.Save(SaveMode.ResolveConflicts);
						if (SaveResult.IrresolvableConflict != conflictResolutionResult.SaveStatus)
						{
							NdrOofHandler.diag.TraceDebug<string>((long)this.GetHashCode(), "Saved initiation message '{0}' successfully.", initMessageId);
							return true;
						}
						NdrOofHandler.diag.TraceDebug<string, int, SaveResult>((long)this.GetHashCode(), "Saving message: '{0}' try {1}, resulted in an update conflict ({2}).", initMessageId, i, conflictResolutionResult.SaveStatus);
					}
				}
			}
			catch (ObjectNotFoundException arg)
			{
				NdrOofHandler.diag.TraceDebug<string, ObjectNotFoundException>((long)this.GetHashCode(), "Initiation Message '{0}' is no longer there {1}", initMessageId, arg);
				return false;
			}
			NdrOofHandler.diag.TraceDebug<string, SaveResult>((long)this.GetHashCode(), "Saving message failed after all retries. Init MessageId={0}, Save status={1}", initMessageId, conflictResolutionResult.SaveStatus);
			return false;
		}

		private void GetNdrOrOofCountInMessage(out int totalNdr, out int totalOof)
		{
			totalNdr = 0;
			totalOof = 0;
			if (ObjectClass.IsDsnNegative(this.messageItem.ClassName))
			{
				totalNdr = this.messageItem.Recipients.Count;
				return;
			}
			totalOof = 1;
		}

		private bool TryGetInitiationAndApprovalRequestMessageId(out string initiationMessageId, out string approvalRequestMessageId)
		{
			initiationMessageId = null;
			approvalRequestMessageId = null;
			string references = this.messageItem.References;
			if (string.IsNullOrEmpty(references))
			{
				NdrOofHandler.diag.TraceDebug((long)this.GetHashCode(), "No references in ndr/oof.");
				return false;
			}
			string text = references.TrimEnd(NdrOofHandler.ReferencesSeparator);
			int num = text.LastIndexOfAny(NdrOofHandler.ReferencesSeparator, text.Length - 1);
			if (num == -1)
			{
				NdrOofHandler.diag.TraceDebug<string>((long)this.GetHashCode(), "Cannot find approval request id in '{0}'", references);
				return false;
			}
			int num2 = text.LastIndexOfAny(NdrOofHandler.ReferencesSeparator, num - 1);
			if (num2 == -1)
			{
				NdrOofHandler.diag.TraceDebug<string>((long)this.GetHashCode(), "Cannot find initiation message id in '{0}'", references);
				return false;
			}
			approvalRequestMessageId = text.Substring(num + 1);
			initiationMessageId = text.Substring(num2 + 1, num - num2 - 1);
			if (string.IsNullOrEmpty(approvalRequestMessageId) || string.IsNullOrEmpty(initiationMessageId))
			{
				NdrOofHandler.diag.TraceDebug<string>((long)this.GetHashCode(), "Do not have approval request or initiation message id in references='{0}'.", references);
				return false;
			}
			return true;
		}

		private const string NdrOofPropertyFormat = "{0};{1};{2}";

		private const int ExpectedNumberOfValuesForNdrOofTracking = 3;

		private static readonly char[] NdrOofPropertySeparator = new char[]
		{
			';'
		};

		private static readonly Microsoft.Exchange.Diagnostics.Trace diag = ExTraceGlobals.ApprovalAgentTracer;

		private static readonly PropertyDefinition[] PropertiesNeededFromInitiationForFinding = new PropertyDefinition[]
		{
			ItemSchema.Id,
			MessageItemSchema.ApprovalRequestMessageId
		};

		private static readonly PropertyDefinition[] PropertiesNeededToUpdateInitiation = new PropertyDefinition[]
		{
			MessageItemSchema.ApprovalDecisionMakersNdred,
			MessageItemSchema.ApprovalStatus
		};

		private static readonly char[] ReferencesSeparator = new char[]
		{
			',',
			' '
		};

		private MessageItem messageItem;
	}
}
