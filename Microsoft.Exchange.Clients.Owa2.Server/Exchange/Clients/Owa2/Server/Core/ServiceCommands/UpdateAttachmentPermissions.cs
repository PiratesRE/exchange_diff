using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class UpdateAttachmentPermissions : ServiceCommand<string>
	{
		public UpdateAttachmentPermissions(CallContext callContext, UpdateAttachmentPermissionsRequest permissionsRequest) : base(callContext)
		{
			UpdateAttachmentPermissions.ConvertAliasesToExternalDirectoryObjectIds(permissionsRequest.UserIds);
			this.permissionsRequest = permissionsRequest;
			OwsLogRegistry.Register(UpdateAttachmentPermissions.UpdateAttachmentPermissionsActionName, typeof(UpdateAttachmentPermissionsMetadata), new Type[0]);
		}

		protected override string InternalExecute()
		{
			Guid guid = Guid.NewGuid();
			UserContext userContext = UserContextManager.GetUserContext(base.CallContext.HttpContext, base.CallContext.EffectiveCaller, true);
			if (this.permissionsRequest.AttachmentDataProviderPermissions != null)
			{
				base.CallContext.ProtocolLog.Set(UpdateAttachmentPermissionsMetadata.NumberOfUserIDs, this.permissionsRequest.UserIds.Length);
				List<string> largeDLsList;
				this.permissionsRequest.UserIds = UpdateAttachmentPermissions.ExpandDLsAndGetAllUsers(userContext, this.permissionsRequest.UserIds, out largeDLsList, base.CallContext);
				List<string> resourceList;
				Dictionary<string, List<AttachmentPermissionAssignment>> dictionary = this.ProcessUpdateAttachmentPermissionsRequest(this.permissionsRequest, userContext, base.CallContext, out resourceList);
				base.CallContext.ProtocolLog.Set(UpdateAttachmentPermissionsMetadata.NumberOfAttachmentDataProviders, dictionary.Count);
				base.CallContext.ProtocolLog.Set(UpdateAttachmentPermissionsMetadata.NumberOfUsersToSetPermissions, this.permissionsRequest.UserIds.Length);
				foreach (KeyValuePair<string, List<AttachmentPermissionAssignment>> keyValuePair in dictionary)
				{
					AttachmentDataProvider provider = userContext.AttachmentDataProviderManager.GetProvider(base.CallContext, keyValuePair.Key);
					UpdateAttachmentPermissions.UpdateAttachmentPermissionsAsync(provider, this.permissionsRequest.UserIds, keyValuePair.Value.ToArray(), userContext, resourceList, largeDLsList);
				}
			}
			return guid.ToString();
		}

		internal static void UpdateAttachmentPermissionsAsync(AttachmentDataProvider attachmentDataProvider, string[] userIds, AttachmentPermissionAssignment[] permissionAssignments, UserContext userContext, List<string> resourceList, List<string> largeDLsList)
		{
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(async delegate()
				{
					UpdatePermissionsAsyncResult updatePermissionsAsyncResult = await attachmentDataProvider.UpdateDocumentPermissionsAsync(userIds, permissionAssignments, default(CancellationToken)).ConfigureAwait(false);
					if (updatePermissionsAsyncResult.ResultCode != AttachmentResultCode.Success)
					{
						UpdateAttachmentPermissions.CreateFailureMessages(userContext, userContext.ExchangePrincipal, null, null, resourceList, null);
					}
					else
					{
						UpdateAttachmentPermissions.ProcessUpdatePermissionsAsyncResult(userContext, updatePermissionsAsyncResult, resourceList, largeDLsList);
					}
				});
			}
			catch (GrayException ex)
			{
				ExTraceGlobals.AttachmentHandlingTracer.TraceError<string>(0L, "UpdateAttachmentPermissions.UpdateAttachmentPermissionsAsync Exception while trying to update permissions async : {0}", ex.StackTrace);
			}
		}

		private Dictionary<string, List<AttachmentPermissionAssignment>> ProcessUpdateAttachmentPermissionsRequest(UpdateAttachmentPermissionsRequest request, UserContext userContext, CallContext callContext, out List<string> resourceList)
		{
			Dictionary<string, List<AttachmentPermissionAssignment>> dictionary = new Dictionary<string, List<AttachmentPermissionAssignment>>();
			List<string> list = new List<string>();
			resourceList = new List<string>();
			AttachmentDataProvider[] providers = userContext.AttachmentDataProviderManager.GetProviders(callContext, null);
			foreach (AttachmentDataProviderPermissions attachmentDataProviderPermissions2 in request.AttachmentDataProviderPermissions)
			{
				if (attachmentDataProviderPermissions2.PermissionAssignments != null)
				{
					string attachmentDataProviderId = attachmentDataProviderPermissions2.AttachmentDataProviderId;
					foreach (AttachmentPermissionAssignment attachmentPermissionAssignment in attachmentDataProviderPermissions2.PermissionAssignments)
					{
						resourceList.Add(attachmentPermissionAssignment.ResourceLocation);
					}
					if (string.IsNullOrEmpty(attachmentDataProviderId))
					{
						if (providers != null)
						{
							foreach (AttachmentPermissionAssignment attachmentPermissionAssignment2 in attachmentDataProviderPermissions2.PermissionAssignments)
							{
								bool flag = false;
								AttachmentDataProvider[] array = providers;
								int l = 0;
								while (l < array.Length)
								{
									AttachmentDataProvider attachmentDataProvider = array[l];
									if (attachmentDataProvider.FileExists(attachmentPermissionAssignment2.ResourceLocation))
									{
										flag = true;
										if (dictionary.ContainsKey(attachmentDataProvider.Id))
										{
											dictionary[attachmentDataProvider.Id].Add(attachmentPermissionAssignment2);
											break;
										}
										dictionary.Add(attachmentDataProvider.Id, new List<AttachmentPermissionAssignment>());
										dictionary[attachmentDataProvider.Id].Add(attachmentPermissionAssignment2);
										break;
									}
									else
									{
										l++;
									}
								}
								if (!flag)
								{
									list.Add(attachmentPermissionAssignment2.ResourceLocation);
								}
							}
						}
					}
					else if (dictionary.ContainsKey(attachmentDataProviderId))
					{
						dictionary[attachmentDataProviderId].AddRange(attachmentDataProviderPermissions2.PermissionAssignments);
					}
					else
					{
						dictionary.Add(attachmentDataProviderId, new List<AttachmentPermissionAssignment>());
						dictionary[attachmentDataProviderId].AddRange(attachmentDataProviderPermissions2.PermissionAssignments);
					}
				}
			}
			if (list.Count > 0)
			{
				ExTraceGlobals.AttachmentHandlingTracer.TraceDebug<int>(0L, "[UpdateAttachmentPermissions : ProcessUpdateAttachmentPermissionsRequest] There were {0} number of resources not found on the existing data providers", list.Count);
				UpdateAttachmentPermissions.CreateFailureMessages(userContext, userContext.ExchangePrincipal, null, list, resourceList, null);
			}
			return dictionary;
		}

		private static void ProcessUpdatePermissionsAsyncResult(UserContext userContext, UpdatePermissionsAsyncResult result, List<string> resourceList, List<string> largeDLsList)
		{
			ExTraceGlobals.AttachmentHandlingTracer.TraceDebug(0L, "[UpdateAttachmentPermissions : ProcessUpdatePermissionsAsyncResult] Method start");
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					Dictionary<string, IList<IUserSharingResult>> dictionary = new Dictionary<string, IList<IUserSharingResult>>();
					foreach (string text in result.ResultsDictionary.Keys)
					{
						IEnumerable<IUserSharingResult> enumerable = result.ResultsDictionary[text];
						foreach (IUserSharingResult userSharingResult in enumerable)
						{
							if (!userSharingResult.Status)
							{
								if (!dictionary.ContainsKey(text))
								{
									dictionary.Add(text, new List<IUserSharingResult>());
								}
								dictionary[text].Add(userSharingResult);
							}
							if (!string.IsNullOrEmpty(userSharingResult.InvitationLink) && !string.IsNullOrEmpty(userSharingResult.User))
							{
								ExTraceGlobals.AttachmentHandlingTracer.TraceDebug<string>(0L, "[UpdateAttachmentPermissions : ProcessUpdatePermissionsAsyncResult] Send invitiation for user : {0}", userSharingResult.User);
								UpdateAttachmentPermissions.SendInvitationMessage(userContext, userContext.ExchangePrincipal, userSharingResult.User, HttpUtility.UrlDecode(text), userSharingResult.InvitationLink);
							}
						}
					}
					if (dictionary.Count > 0 || (largeDLsList != null && largeDLsList.Count > 0))
					{
						ExTraceGlobals.AttachmentHandlingTracer.TraceDebug<int>(0L, "[UpdateAttachmentPermissions : ProcessUpdatePermissionsAsyncResult] There were {0} number of failures when setting perms", dictionary.Count);
						UpdateAttachmentPermissions.CreateFailureMessages(userContext, userContext.ExchangePrincipal, dictionary, null, resourceList, largeDLsList);
					}
				});
			}
			catch (GrayException ex)
			{
				ExTraceGlobals.AttachmentHandlingTracer.TraceError<string>(0L, "[UpdateAttachmentPermissions : ProcessUpdatePermissionsAsyncResult] Exception happened when processing results : {0}", ex.StackTrace);
			}
		}

		private static string[] ExpandDLsAndGetAllUsers(UserContext userContext, string[] userIds, out List<string> largeDLsList, CallContext callContext)
		{
			List<string> list = new List<string>(userIds);
			largeDLsList = new List<string>();
			Stopwatch stopwatch = Stopwatch.StartNew();
			GetMailTipsResponseMessage mailTipsForUsers = UpdateAttachmentPermissions.GetMailTipsForUsers(userContext, userIds);
			stopwatch.Stop();
			callContext.ProtocolLog.Set(UpdateAttachmentPermissionsMetadata.GetMailTipsTime, stopwatch.ElapsedMilliseconds);
			if (mailTipsForUsers.ResponseMessages != null && mailTipsForUsers.ResponseMessages.Length > 0)
			{
				stopwatch = Stopwatch.StartNew();
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = int.MaxValue;
				int num5 = 0;
				ADRecipientExpansion expansion = new ADRecipientExpansion(userContext.ExchangePrincipal.MailboxInfo.OrganizationId);
				foreach (MailTipsResponseMessage mailTipsResponseMessage in mailTipsForUsers.ResponseMessages)
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(mailTipsResponseMessage.MailTips.OuterXml);
					XmlNode xmlNode = xmlDocument.SelectSingleNode("//*[local-name()=\"EmailAddress\"]");
					XmlNode xmlNode2 = xmlDocument.SelectSingleNode("//*[local-name()=\"TotalMemberCount\"]");
					if (xmlNode2 != null && !string.IsNullOrEmpty(xmlNode2.InnerText) && xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
					{
						int num6 = int.Parse(xmlNode2.InnerText);
						if (num6 != 1)
						{
							list.Remove(xmlNode.InnerText);
							num++;
							num3 += num6;
							if (num4 > num6)
							{
								num4 = num6;
							}
							if (num5 < num6)
							{
								num5 = num6;
							}
						}
						if (num6 > 1 && num6 <= 100)
						{
							UpdateAttachmentPermissions.AddDLMembersToResults(list, expansion, xmlNode.InnerText);
						}
						else if (num6 > 100)
						{
							largeDLsList.Add(xmlNode.InnerText);
							ExTraceGlobals.AttachmentHandlingTracer.TraceError<string, int>(0L, "[UpdateAttachmentPermissions : ExpandDLsAndGetAllUsers] size of DL {0} was {1}, not setting permissions.", xmlNode.InnerText, num6);
							num2++;
						}
					}
					else
					{
						ExTraceGlobals.AttachmentHandlingTracer.TraceError(0L, "[UpdateAttachmentPermissions : ExpandDLsAndGetAllUsers] Error parsing GetMailTips response.");
					}
				}
				stopwatch.Stop();
				callContext.ProtocolLog.Set(UpdateAttachmentPermissionsMetadata.DLExpandTime, stopwatch.ElapsedMilliseconds);
				callContext.ProtocolLog.Set(UpdateAttachmentPermissionsMetadata.NumberOfDLs, num);
				if (num > 0)
				{
					callContext.ProtocolLog.Set(UpdateAttachmentPermissionsMetadata.NumberOfLargeDLs, num2);
					callContext.ProtocolLog.Set(UpdateAttachmentPermissionsMetadata.NumberOfRecipientsInDLs, num3);
					callContext.ProtocolLog.Set(UpdateAttachmentPermissionsMetadata.NumberOfRecipientsInSmallestDL, num4);
					callContext.ProtocolLog.Set(UpdateAttachmentPermissionsMetadata.NumberOfRecipientsInLargestDL, num5);
				}
			}
			else
			{
				ExTraceGlobals.AttachmentHandlingTracer.TraceError(0L, "[UpdateAttachmentPermissions : ExpandDLsAndGetAllUsers] GetMailTips response was empty.");
			}
			return list.ToArray();
		}

		private static void AddDLMembersToResults(List<string> results, ADRecipientExpansion expansion, string dlSmtpAddress)
		{
			ADRawEntry adrawEntryFromSmtpAddress = UpdateAttachmentPermissions.GetADRawEntryFromSmtpAddress(dlSmtpAddress);
			if (adrawEntryFromSmtpAddress != null)
			{
				new List<ADRawEntry>();
				ADRecipientExpansion.HandleRecipientDelegate handleRecipient = delegate(ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
				{
					if (recipientExpansionType == ExpansionType.GroupMembership)
					{
						return ExpansionControl.Continue;
					}
					string item = recipient[ADRecipientSchema.PrimarySmtpAddress].ToString();
					if (!results.Contains(item))
					{
						results.Add(item);
					}
					return ExpansionControl.Skip;
				};
				expansion.Expand(adrawEntryFromSmtpAddress, handleRecipient, null);
				return;
			}
			ExTraceGlobals.AttachmentHandlingTracer.TraceError(0L, "[UpdateAttachmentPermissions : AddDLMembersToResults] Unable to get RawADEntry from SmtpAddress.");
		}

		private static GetMailTipsResponseMessage GetMailTipsForUsers(UserContext userContext, string[] userIds)
		{
			EmailAddressWrapper[] array = new EmailAddressWrapper[userIds.Length];
			for (int i = 0; i < userIds.Length; i++)
			{
				array[i] = new EmailAddressWrapper();
				array[i].EmailAddress = userIds[i];
			}
			return UpdateAttachmentPermissions.InvokeGetMailTipsSynchronous(userContext, new GetMailTipsRequest
			{
				Recipients = array,
				MailTipsRequested = MailTipTypes.TotalMemberCount
			});
		}

		private static GetMailTipsResponseMessage InvokeGetMailTipsSynchronous(UserContext userContext, GetMailTipsRequest request)
		{
			OwaServerTraceLogger.AppendToLog(new TraceLogEvent("UpdateAttachmentPermissions-GetMailTipsBegin", userContext, "InvokeGetMailTipsSynchronous", "Starting Synchronous call to GetMailTips"));
			GetMailTipsJsonRequest getMailTipsJsonRequest = new GetMailTipsJsonRequest();
			getMailTipsJsonRequest.Body = request;
			OWAService owaservice = new OWAService();
			IAsyncResult asyncResult = owaservice.BeginGetMailTips(getMailTipsJsonRequest, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			GetMailTipsResponseMessage body = owaservice.EndGetMailTips(asyncResult).Body;
			OwaServerTraceLogger.AppendToLog(new TraceLogEvent("UpdateAttachmentPermissions-GetMailTipsEnd", userContext, "InvokeGetMailTipsSynchronous", "Ending Synchronous call to GetMailTips"));
			return body;
		}

		private static ADRawEntry GetADRawEntryFromSmtpAddress(string smtpAddress)
		{
			IRecipientSession adrecipientSession = CallContext.Current.ADRecipientSessionContext.GetADRecipientSession();
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.PrimarySmtpAddress, smtpAddress);
			ADRawEntry[] array = adrecipientSession.FindRecipient(adrecipientSession.SearchRoot, QueryScope.SubTree, filter, null, 1, new PropertyDefinition[]
			{
				ADRecipientSchema.PrimarySmtpAddress,
				ADRecipientSchema.RecipientType
			});
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		private static void ConvertAliasesToExternalDirectoryObjectIds(string[] userIds)
		{
			IRecipientSession adrecipientSession = CallContext.Current.ADRecipientSessionContext.GetADRecipientSession();
			for (int i = 0; i < userIds.Length; i++)
			{
				if (!userIds[i].Contains("@"))
				{
					string text = UpdateAttachmentPermissions.ConvertBetweenADRecipientProperties(adrecipientSession, userIds[i], ADRecipientSchema.Alias, ADRecipientSchema.ExternalDirectoryObjectId);
					if (!string.IsNullOrEmpty(text))
					{
						userIds[i] = text;
					}
				}
			}
		}

		private static string ConvertBetweenADRecipientProperties(IRecipientSession session, string convertFromPropertyValue, ADPropertyDefinition convertFromProperty, ADPropertyDefinition convertToProperty)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, convertFromProperty, convertFromPropertyValue);
			ADRawEntry[] array = session.FindRecipient(session.SearchRoot, QueryScope.SubTree, filter, null, 1, new PropertyDefinition[]
			{
				convertToProperty
			});
			if (array == null || array.Length == 0 || array[0].propertyBag[convertToProperty] == null)
			{
				return null;
			}
			return array[0].propertyBag[convertToProperty].ToString();
		}

		private static void SendInvitationMessage(UserContext userContext, ExchangePrincipal exchangePrincipal, string user, string location, string invitationLink)
		{
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					if (string.IsNullOrEmpty(user))
					{
						throw new ArgumentNullException("user", "User name cannot be null when sending invite!");
					}
					if (string.IsNullOrEmpty(invitationLink))
					{
						throw new ArgumentNullException("invitationLink", "InvitationLink cannot be null when sending invite!");
					}
					if (string.IsNullOrEmpty(location))
					{
						throw new ArgumentNullException("location", "Document location cannot be null when sending invite!");
					}
					try
					{
						string fileNameFromLocation = UpdateAttachmentPermissions.GetFileNameFromLocation(location);
						userContext.LockAndReconnectMailboxSession();
						MessageItem messageItem = MessageItem.Create(userContext.MailboxSession, userContext.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox));
						messageItem.From = new Participant(exchangePrincipal);
						messageItem.Sender = messageItem.From;
						messageItem.Subject = string.Format(Strings.GuestSharingInvitationSubject, exchangePrincipal.MailboxInfo.DisplayName, fileNameFromLocation);
						messageItem.IsDraft = false;
						messageItem.IsRead = false;
						messageItem.Recipients.Add(new Participant(user, user, "SMTP"));
						string bodyContent = AttachmentMessageBodyGenerator.GenerateBodyForInvitation(fileNameFromLocation, invitationLink);
						UpdateAttachmentPermissions.SetItemBody(messageItem, BodyFormat.TextHtml, bodyContent);
						messageItem.SendWithoutSavingMessage();
					}
					catch (InvalidRecipientsException ex2)
					{
						ExTraceGlobals.AttachmentHandlingTracer.TraceError<string>(0L, "[UpdateAttachmentPermissions : SendInvitationMessage] Exception happened when trying to send the invitation message : {0}", ex2.StackTrace);
					}
					finally
					{
						if (userContext.MailboxSessionLockedByCurrentThread())
						{
							userContext.UnlockAndDisconnectMailboxSession();
						}
					}
				});
			}
			catch (GrayException ex)
			{
				ExTraceGlobals.AttachmentHandlingTracer.TraceError<string>(0L, "[UpdateAttachmentPermissions : SendInvitationMessage] Exception happened when trying to send the invitation message : {0}", ex.StackTrace);
			}
		}

		private static void CreateOneFailureMessage(UserContext userContext, ExchangePrincipal exchangePrincipal, string body, List<AttachmentFile> attachmentFiles)
		{
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					try
					{
						userContext.LockAndReconnectMailboxSession();
						MessageItem messageItem = MessageItem.Create(userContext.MailboxSession, userContext.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox));
						messageItem.From = new Participant("Microsoft Outlook", "updatepermissionstest@microsoft.com", "SMTP");
						messageItem.Sender = messageItem.From;
						messageItem.IsDraft = false;
						messageItem.IsRead = false;
						messageItem.Recipients.Add(new Participant(exchangePrincipal), RecipientItemType.To);
						UpdateAttachmentPermissions.SetItemBody(messageItem, BodyFormat.TextHtml, body);
						if (attachmentFiles.Count == 1)
						{
							messageItem.Subject = string.Format(Strings.OneAttachmentSharingFailureSubject, attachmentFiles[0].FileName);
						}
						else
						{
							messageItem.Subject = Strings.AttachmentSharingFailureSubject;
						}
						messageItem.Save(SaveMode.NoConflictResolutionForceSave);
						messageItem[ItemSchema.ReceivedTime] = ExDateTime.Now;
						messageItem.Save(SaveMode.ResolveConflicts);
					}
					finally
					{
						if (userContext.MailboxSessionLockedByCurrentThread())
						{
							userContext.UnlockAndDisconnectMailboxSession();
						}
					}
				});
			}
			catch (GrayException ex)
			{
				ExTraceGlobals.AttachmentHandlingTracer.TraceError<string>(0L, "[UpdateAttachmentPermissions : CreateMessage] Exception happened when trying to send message : {0}", ex.StackTrace);
			}
		}

		private static void SetItemBody(Item item, BodyFormat bodyFormat, string bodyContent)
		{
			BodyWriteConfiguration configuration = new BodyWriteConfiguration(bodyFormat);
			using (TextWriter textWriter = item.Body.OpenTextWriter(configuration))
			{
				textWriter.Write(bodyContent);
			}
		}

		private static void CreateFailureMessages(UserContext userContext, ExchangePrincipal exchangePrincipal, Dictionary<string, IList<IUserSharingResult>> failedResultsDictionary, List<string> noProviderResources, List<string> resourceList, List<string> largeDLsList)
		{
			string text = null;
			List<AttachmentFile> list;
			if (noProviderResources != null)
			{
				list = UpdateAttachmentPermissions.GetAttachmentFiles(noProviderResources);
				text = AttachmentMessageBodyGenerator.GenerateBodyForAttachmentNotFound(list);
				UpdateAttachmentPermissions.CreateOneFailureMessage(userContext, exchangePrincipal, text, list);
			}
			if (failedResultsDictionary != null && failedResultsDictionary.Count > 0)
			{
				list = new List<AttachmentFile>();
				List<string> list2 = new List<string>();
				IRecipientSession adrecipientSession = CallContext.Current.ADRecipientSessionContext.GetADRecipientSession();
				foreach (string text2 in failedResultsDictionary.Keys)
				{
					AttachmentFile item = new AttachmentFile(UpdateAttachmentPermissions.GetFileNameFromLocation(text2), HttpUtility.UrlDecode(text2));
					list.Add(item);
					foreach (IUserSharingResult userSharingResult in failedResultsDictionary[text2])
					{
						string text3 = userSharingResult.User;
						if (text3 != null && !text3.Contains("@"))
						{
							text3 = UpdateAttachmentPermissions.ConvertBetweenADRecipientProperties(adrecipientSession, text3, ADRecipientSchema.ExternalDirectoryObjectId, ADRecipientSchema.PrimarySmtpAddress);
						}
						if (text3 != null)
						{
							list2.Add(text3);
						}
					}
				}
				text = AttachmentMessageBodyGenerator.GenerateBodyForSetWrongPermission(list, list2);
				UpdateAttachmentPermissions.CreateOneFailureMessage(userContext, exchangePrincipal, text, list);
			}
			list = UpdateAttachmentPermissions.GetAttachmentFiles(resourceList);
			if (largeDLsList != null && largeDLsList.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string value in largeDLsList)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(Strings.DistributionListNameSeperator);
					}
					stringBuilder.Append(value);
				}
				text = AttachmentMessageBodyGenerator.GenerateBodyForSentToTooLargeDL(list, stringBuilder.ToString());
				UpdateAttachmentPermissions.CreateOneFailureMessage(userContext, exchangePrincipal, text, list);
				return;
			}
			if (text == null)
			{
				text = AttachmentMessageBodyGenerator.GenerateBodyForCatchAll(list);
				UpdateAttachmentPermissions.CreateOneFailureMessage(userContext, exchangePrincipal, text, list);
			}
		}

		private static List<AttachmentFile> GetAttachmentFiles(List<string> resourceList)
		{
			List<AttachmentFile> list = new List<AttachmentFile>();
			if (resourceList != null)
			{
				foreach (string text in resourceList)
				{
					AttachmentFile item = new AttachmentFile(UpdateAttachmentPermissions.GetFileNameFromLocation(text), HttpUtility.UrlDecode(text));
					list.Add(item);
				}
			}
			return list;
		}

		private static string GetFileNameFromLocation(string location)
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(location))
			{
				Uri uri;
				if (Uri.TryCreate(location, UriKind.Absolute, out uri))
				{
					text = Path.GetFileName(uri.AbsolutePath);
				}
				else
				{
					text = Path.GetFileName(location);
				}
				if (string.IsNullOrEmpty(text))
				{
					text = HttpUtility.UrlDecode(location);
				}
				else
				{
					text = HttpUtility.UrlDecode(text);
				}
			}
			return text;
		}

		public const int SmallDLSizeTreshold = 100;

		private const string MailTipsTotalMemberCountPropertyName = "TotalMemberCount";

		private const string MailTipsEmailAddressPropertyName = "EmailAddress";

		private static readonly string UpdateAttachmentPermissionsActionName = typeof(UpdateAttachmentPermissions).Name;

		private readonly UpdateAttachmentPermissionsRequest permissionsRequest;
	}
}
