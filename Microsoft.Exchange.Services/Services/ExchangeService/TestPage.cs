using System;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security.AntiXss;
using System.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.ExchangeService
{
	public class TestPage : IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		public void ProcessRequest(HttpContext httpContext)
		{
			try
			{
				using (TestPage.NullMessage nullMessage = new TestPage.NullMessage())
				{
					using (CallContext callContext = this.CreateCallContext(httpContext, nullMessage))
					{
						using (IExchangeService exchangeService = ExchangeServiceFactory.Default.CreateForEws(callContext))
						{
							this.Dispatch(httpContext, exchangeService);
						}
					}
				}
			}
			catch (Exception ex)
			{
				httpContext.Response.Write(AntiXssEncoder.HtmlEncode(ex.ToString(), false));
			}
		}

		private void Dispatch(HttpContext httpContext, IExchangeService exchangeService)
		{
			string text = string.Format("{0}", httpContext.Request.QueryString["cmd"]);
			string key;
			switch (key = text.ToUpper())
			{
			case "ADDAGGREGATEDACCOUNT":
				this.CallAddAggregatedAccount(httpContext, exchangeService);
				goto IL_1A5;
			case "GETAGGREGATEDACCOUNT":
				this.CallGetAggregatedAccount(httpContext, exchangeService);
				goto IL_1A5;
			case "REMOVEAGGREGATEDACCOUNT":
				this.CallRemoveAggregatedAccount(httpContext, exchangeService);
				goto IL_1A5;
			case "SETAGGREGATEDACCOUNT":
				this.CallSetAggregatedAccount(httpContext, exchangeService);
				goto IL_1A5;
			case "ISOFFICE365DOMAIN":
				this.CallIsOffice365Domain(httpContext, exchangeService);
				goto IL_1A5;
			case "CREATEUNIFIEDMAILBOX":
				this.CallCreateUnifiedMailbox(httpContext, exchangeService);
				goto IL_1A5;
			case "FINDCONVERSATIONFORUNIFIEDMAILBOX":
				this.CallFindConversationForUnifiedMailbox(httpContext, exchangeService, " ");
				goto IL_1A5;
			case "GETFOLDER":
				this.CallGetFolder(httpContext, exchangeService);
				goto IL_1A5;
			case "GETMODERNGROUP":
				this.CallGetModernGroup(httpContext, exchangeService);
				goto IL_1A5;
			case "FINDFOLDER":
				this.CallFindFolder(httpContext, exchangeService);
				goto IL_1A5;
			case "FINDFLOWCONVERSATIONS":
				this.CallFindConversationForUnifiedMailbox(httpContext, exchangeService, "</br>");
				goto IL_1A5;
			}
			httpContext.Response.Output.WriteLine("Unknown command!");
			IL_1A5:
			httpContext.ApplicationInstance.CompleteRequest();
		}

		private void CallAddAggregatedAccount(HttpContext httpContext, IExchangeService exchangeService)
		{
			this.IsQueryStringParameterSet(httpContext, "email");
			this.IsQueryStringParameterSet(httpContext, "password");
			AddAggregatedAccountRequest request = new AddAggregatedAccountRequest
			{
				EmailAddress = httpContext.Request.QueryString["email"],
				Password = httpContext.Request.QueryString["password"]
			};
			this.AssignIfQueryStringParameterSet(httpContext, "incomingserver", delegate(string val)
			{
				request.IncomingServer = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "incomingport", delegate(string val)
			{
				request.IncomingPort = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "outgoingserver", delegate(string val)
			{
				request.OutgoingServer = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "outgoingport", delegate(string val)
			{
				request.OutgoingPort = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "security", delegate(string val)
			{
				request.Security = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "authentication", delegate(string val)
			{
				request.Authentication = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "interval", delegate(string val)
			{
				request.IncrementalSyncInterval = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "incomingprotocol", delegate(string val)
			{
				request.IncomingProtocol = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "outgoingprotocol", delegate(string val)
			{
				request.OutgoingProtocol = val;
			});
			AddAggregatedAccountResponse addAggregatedAccountResponse = exchangeService.AddAggregatedAccount(request, null);
			httpContext.Response.Output.WriteLine("<b>Response Code:</b>" + addAggregatedAccountResponse.ResponseCode.ToString() + "</br>");
			if (addAggregatedAccountResponse.Account == null)
			{
				httpContext.Response.Output.WriteLine("<b>Account:</b>NULL</br>");
				return;
			}
			httpContext.Response.Output.WriteLine("<b>Email address:</b>" + (addAggregatedAccountResponse.Account.EmailAddress ?? "NULL") + "</br>");
			httpContext.Response.Output.WriteLine("<b>User name:</b>" + (addAggregatedAccountResponse.Account.UserName ?? "NULL") + "</br>");
			httpContext.Response.Output.WriteLine("<b>Connection Settings:</b></br>");
			if (addAggregatedAccountResponse.Account.ConnectionSettings != null)
			{
				httpContext.Response.Output.WriteLine(addAggregatedAccountResponse.Account.ConnectionSettings.ToMultiLineString("</br>"));
				return;
			}
			httpContext.Response.Output.WriteLine("NULL</br>");
		}

		private void CallIsOffice365Domain(HttpContext httpContext, IExchangeService exchangeService)
		{
			this.IsQueryStringParameterSet(httpContext, "email");
			IsOffice365DomainRequest request = new IsOffice365DomainRequest
			{
				EmailAddress = httpContext.Request.QueryString["email"]
			};
			IsOffice365DomainResponse isOffice365DomainResponse = exchangeService.IsOffice365Domain(request, null);
			httpContext.Response.Output.WriteLine(isOffice365DomainResponse.IsOffice365Domain);
		}

		private void CallGetAggregatedAccount(HttpContext httpContext, IExchangeService exchangeService)
		{
			GetAggregatedAccountRequest request = new GetAggregatedAccountRequest();
			GetAggregatedAccountResponse aggregatedAccount = exchangeService.GetAggregatedAccount(request, null);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (AggregatedAccountType aggregatedAccount2 in aggregatedAccount.AggregatedAccounts)
			{
				stringBuilder.Append(this.ConvertAggregatedAccountTypeToString(aggregatedAccount2));
			}
			httpContext.Response.Output.WriteLine(stringBuilder);
		}

		private void CallRemoveAggregatedAccount(HttpContext httpContext, IExchangeService exchangeService)
		{
			this.IsQueryStringParameterSet(httpContext, "email");
			RemoveAggregatedAccountRequest request = new RemoveAggregatedAccountRequest
			{
				EmailAddress = httpContext.Request.QueryString["email"]
			};
			RemoveAggregatedAccountResponse removeAggregatedAccountResponse = exchangeService.RemoveAggregatedAccount(request, null);
			httpContext.Response.Output.WriteLine(removeAggregatedAccountResponse.ToString());
		}

		private void CallSetAggregatedAccount(HttpContext httpContext, IExchangeService exchangeService)
		{
			this.IsQueryStringParameterSet(httpContext, "email");
			SetAggregatedAccountRequest request = new SetAggregatedAccountRequest();
			this.AssignIfQueryStringParameterSet(httpContext, "authentication", delegate(string val)
			{
				request.Authentication = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "email", delegate(string val)
			{
				request.EmailAddress = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "port", delegate(string val)
			{
				request.IncomingPort = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "server", delegate(string val)
			{
				request.IncomingServer = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "interval", delegate(string val)
			{
				request.IncrementalSyncInterval = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "password", delegate(string val)
			{
				request.Password = val;
			});
			this.AssignIfQueryStringParameterSet(httpContext, "security", delegate(string val)
			{
				request.Security = val;
			});
			SetAggregatedAccountResponse setAggregatedAccountResponse = exchangeService.SetAggregatedAccount(request, null);
			httpContext.Response.Output.WriteLine(setAggregatedAccountResponse.ToString());
		}

		private void CallCreateUnifiedMailbox(HttpContext httpContext, IExchangeService exchangeService)
		{
			CreateUnifiedMailboxRequest request = new CreateUnifiedMailboxRequest();
			CreateUnifiedMailboxResponse createUnifiedMailboxResponse = exchangeService.CreateUnifiedMailbox(request, null);
			httpContext.Response.Output.WriteLine(createUnifiedMailboxResponse.UserPrincipalName);
		}

		private void CallGetFolder(HttpContext httpContext, IExchangeService exchangeService)
		{
			GetFolderResponse folder = exchangeService.GetFolder(new GetFolderRequest
			{
				Ids = new DistinguishedFolderId[]
				{
					new DistinguishedFolderId
					{
						Id = DistinguishedFolderIdName.inbox
					}
				},
				FolderShape = new FolderResponseShape(ShapeEnum.AllProperties, null)
			}, null);
			FolderInfoResponseMessage folderInfoResponseMessage = folder.ResponseMessages.Items[0] as FolderInfoResponseMessage;
			foreach (FolderType folderType in folderInfoResponseMessage.Folders)
			{
				httpContext.Response.Output.WriteLine(string.Format("Name = {0}", folderType.DisplayName));
				httpContext.Response.Output.WriteLine(string.Format("Id = {0}", folderType.FolderId.Id));
				httpContext.Response.Output.WriteLine(string.Format("ChangeKey = {0}", folderType.FolderId.ChangeKey));
			}
		}

		private void CallFindConversationForUnifiedMailbox(HttpContext httpContext, IExchangeService exchangeService, string delimiter = " ")
		{
			this.InternalCallFindConversationForUnifiedMailbox(httpContext, exchangeService, delegate(FindConversationRequest request)
			{
				new DistinguishedFolderId();
				this.AssignIfQueryStringParameterSet(httpContext, "folderId", delegate(string val)
				{
					request.ParentFolderId = new TargetFolderId
					{
						BaseFolderId = new DistinguishedFolderId
						{
							Id = (DistinguishedFolderIdName)Enum.Parse(typeof(DistinguishedFolderIdName), val)
						}
					};
				});
				this.AssignIfQueryStringParameterSet(httpContext, "mailboxguids", delegate(string val)
				{
					request.MailboxGuids = (from guid in val.Split(new char[]
					{
						','
					})
					select Guid.Parse(guid)).ToArray<Guid>();
				});
			}, delimiter);
		}

		private void InternalCallFindConversationForUnifiedMailbox(HttpContext httpContext, IExchangeService exchangeService, Action<FindConversationRequest> initializeRequest, string delimiter)
		{
			FindConversationRequest findConversationRequest = new FindConversationRequest();
			PropertyPath[] additionalProperties = new PropertyPath[]
			{
				new PropertyUri(PropertyUriEnum.Topic)
			};
			findConversationRequest.ConversationShape = new ConversationResponseShape(ShapeEnum.IdOnly, additionalProperties);
			initializeRequest(findConversationRequest);
			FindConversationResponseMessage findConversationResponseMessage = exchangeService.FindConversation(findConversationRequest, null);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ConversationType conversationType in findConversationResponseMessage.Conversations)
			{
				stringBuilder.Append(string.Format("{0}{1}", conversationType.ConversationTopic, delimiter));
			}
			httpContext.Response.Output.WriteLine(stringBuilder.ToString());
		}

		private void CallGetModernGroup(HttpContext httpContext, IExchangeService exchangeService)
		{
			GetModernGroupResponse modernGroup = exchangeService.GetModernGroup(new GetModernGroupRequest
			{
				SmtpAddress = CallContext.Current.AccessingADUser.PrimarySmtpAddress.ToString(),
				ResultSet = (ModernGroupRequestResultSet.General | ModernGroupRequestResultSet.Members)
			}, null);
			httpContext.Response.Output.WriteLine(modernGroup.MembersInfo.Count);
		}

		private void CallFindFolder(HttpContext httpContext, IExchangeService exchangeService)
		{
			FindFolderRequest request = new FindFolderRequest();
			request.Traversal = FolderQueryTraversal.Deep;
			if (!string.IsNullOrEmpty(httpContext.Request.QueryString["parentfolder"]))
			{
				DistinguishedFolderId distinguishedFolderId = new DistinguishedFolderId
				{
					Id = DistinguishedFolderIdName.msgfolderroot
				};
				request.ParentFolderIds = new BaseFolderId[]
				{
					distinguishedFolderId
				};
			}
			else
			{
				request.ParentFolderIds = null;
			}
			PropertyPath[] additionalProperties = new PropertyPath[]
			{
				new PropertyUri(PropertyUriEnum.FolderDisplayName)
			};
			request.FolderShape = new FolderResponseShape(ShapeEnum.IdOnly, additionalProperties);
			this.AssignIfQueryStringParameterSet(httpContext, "mailboxguid", delegate(string val)
			{
				request.MailboxGuid = new Guid(val);
			});
			FindFolderResponse findFolderResponse = exchangeService.FindFolder(request, null);
			FindFolderResponseMessage findFolderResponseMessage = findFolderResponse.ResponseMessages.Items[0] as FindFolderResponseMessage;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (BaseFolderType baseFolderType in findFolderResponseMessage.RootFolder.Folders)
			{
				stringBuilder.Append(string.Format("{0} ", baseFolderType.DisplayName));
			}
			httpContext.Response.Output.WriteLine(stringBuilder.ToString());
		}

		private CallContext CreateCallContext(HttpContext httpContext, TestPage.NullMessage message)
		{
			JsonMessageHeaderProcessor headerProcessor = new JsonMessageHeaderProcessor();
			message.Properties.Add("WebMethodEntry", WebMethodEntry.JsonWebMethodEntry);
			MSAIdentity msaidentity = httpContext.User.Identity as MSAIdentity;
			CallContext callContext;
			if (msaidentity == null)
			{
				callContext = CallContext.CreateFromRequest(headerProcessor, message);
			}
			else
			{
				TestPage.InitIfNeeded(httpContext);
				BudgetKey key = new StringBudgetKey(msaidentity.MemberName, false, BudgetType.Ews);
				callContext = CallContext.CreateForExchangeService(httpContext, TestPage.appWideStoreSessionCache, TestPage.acceptedDomainCache, TestPage.userWorkloadManager, EwsBudget.Acquire(key), Thread.CurrentThread.CurrentCulture);
			}
			httpContext.Items["CallContext"] = callContext;
			return callContext;
		}

		private void IsQueryStringParameterSet(HttpContext httpContext, string parameter)
		{
			if (string.IsNullOrEmpty(httpContext.Request.QueryString[parameter]))
			{
				httpContext.Response.Output.WriteLine(string.Format("Error: {0} parameter is not set.", parameter));
				throw new Exception(string.Format("Error: {0} parameter is not set.", parameter));
			}
		}

		private void AssignIfQueryStringParameterSet(HttpContext httpContext, string parameter, Action<string> assignValue)
		{
			string text = httpContext.Request.QueryString[parameter];
			if (!string.IsNullOrEmpty(text))
			{
				assignValue(text);
			}
		}

		private string ConvertAggregatedAccountTypeToString(AggregatedAccountType aggregatedAccount)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}:{1}", aggregatedAccount.GetType().Name, "</br>");
			stringBuilder.AppendFormat("Email address={0},{1}", aggregatedAccount.EmailAddress ?? "[NULL]", "</br>");
			stringBuilder.AppendFormat("User name={0},{1}", aggregatedAccount.UserName ?? "[NULL]", "</br>");
			if (aggregatedAccount.ConnectionSettings == null)
			{
				stringBuilder.Append("Connection settings=[NULL].");
			}
			else
			{
				stringBuilder.Append(aggregatedAccount.ConnectionSettings.ToMultiLineString("</br>"));
			}
			return stringBuilder.ToString();
		}

		private static void InitIfNeeded(HttpContext httpContext)
		{
			if (!TestPage.initialized)
			{
				lock (TestPage.staticLock)
				{
					if (!TestPage.initialized)
					{
						HttpApplicationState application = httpContext.Application;
						TestPage.appWideStoreSessionCache = (application["WS_APPWideMailboxCacheKey"] as AppWideStoreSessionCache);
						TestPage.acceptedDomainCache = (application["WS_AcceptedDomainCacheKey"] as AcceptedDomainCache);
						TestPage.userWorkloadManager = (application["WS_WorkloadManagerKey"] as UserWorkloadManager);
						TestPage.initialized = true;
					}
				}
			}
		}

		private const string LINE_SEPARATOR = "</br>";

		private static object staticLock = new object();

		private static bool initialized = false;

		private static AppWideStoreSessionCache appWideStoreSessionCache;

		private static AcceptedDomainCache acceptedDomainCache;

		private static UserWorkloadManager userWorkloadManager;

		private class NullMessage : Message
		{
			public override bool IsEmpty
			{
				get
				{
					return true;
				}
			}

			public override MessageHeaders Headers
			{
				get
				{
					return this.headers;
				}
			}

			public override MessageProperties Properties
			{
				get
				{
					return this.properties;
				}
			}

			public override MessageVersion Version
			{
				get
				{
					return this.headers.MessageVersion;
				}
			}

			protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
			{
			}

			protected override void OnBodyToString(XmlDictionaryWriter writer)
			{
			}

			private readonly MessageHeaders headers = new MessageHeaders(MessageVersion.None);

			private readonly MessageProperties properties = new MessageProperties();
		}
	}
}
