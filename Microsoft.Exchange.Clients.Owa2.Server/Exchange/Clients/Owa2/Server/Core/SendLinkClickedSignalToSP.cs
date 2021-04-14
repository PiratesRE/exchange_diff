using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.SharePointSignalStore;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SendLinkClickedSignalToSP : ServiceCommand<bool>
	{
		internal SendLinkClickedSignalToSP(CallContext callContext, SendLinkClickedSignalToSPRequest request) : base(callContext)
		{
			if (callContext == null)
			{
				throw new OwaInvalidRequestException("callContext parameter was null");
			}
			if (request == null)
			{
				throw new OwaInvalidRequestException("request parameter was null");
			}
			if (request.ID == null)
			{
				throw new OwaInvalidRequestException("request.ID parameter was null");
			}
			if (string.IsNullOrEmpty(request.Url))
			{
				throw new OwaInvalidRequestException("request.Url parameter was null");
			}
			this.callContext = callContext;
			this.itemid = request.ID;
			this.url = request.Url;
			this.title = request.Title;
			this.description = request.Description;
			this.imgurl = request.ImgURL;
			this.imgdimensions = request.ImgDimensions;
			this.linkStats = new LinkClickedSignalStats();
		}

		internal IRecipientSession ADRecipientSession
		{
			get
			{
				IRecipientSession result;
				if ((result = this.adRecipientSession) == null)
				{
					result = (this.adRecipientSession = this.callContext.ADRecipientSessionContext.GetGALScopedADRecipientSession(this.callContext.EffectiveCaller.ClientSecurityContext));
				}
				return result;
			}
			set
			{
				this.adRecipientSession = value;
			}
		}

		internal ILogger Logger
		{
			get
			{
				ILogger result;
				if ((result = this.logger) == null)
				{
					result = (this.logger = new TraceLogger(false));
				}
				return result;
			}
			set
			{
				this.logger = value;
			}
		}

		internal Func<MessageType> GetMailMessage
		{
			get
			{
				Func<MessageType> result;
				if ((result = this.getmailmessage) == null)
				{
					result = (this.getmailmessage = new Func<MessageType>(this.GetMessage));
				}
				return result;
			}
			set
			{
				this.getmailmessage = value;
			}
		}

		internal Func<string> GetSharePointUrl
		{
			get
			{
				Func<string> result;
				if ((result = this.getsharepointurl) == null)
				{
					result = (this.getsharepointurl = new Func<string>(this.GetSharePointUrlFromAAD));
				}
				return result;
			}
			set
			{
				this.getsharepointurl = value;
			}
		}

		internal Func<ICredentials> GetUserCredentials
		{
			get
			{
				Func<ICredentials> result;
				if ((result = this.getusercredentials) == null)
				{
					result = (this.getusercredentials = new Func<ICredentials>(this.GetCredentials));
				}
				return result;
			}
			set
			{
				this.getusercredentials = value;
			}
		}

		internal ItemResponseShape MakeItemResponseShape()
		{
			return new ItemResponseShape
			{
				AdditionalProperties = new PropertyPath[]
				{
					new PropertyUri(PropertyUriEnum.ToRecipients),
					new PropertyUri(PropertyUriEnum.CcRecipients),
					new PropertyUri(PropertyUriEnum.From)
				}
			};
		}

		internal GetItemRequest MakeGetItemRequest()
		{
			return new GetItemRequest
			{
				ItemShape = this.MakeItemResponseShape(),
				Ids = new BaseItemId[]
				{
					this.itemid
				}
			};
		}

		private MessageType GetMessage()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			GetItemRequest request = this.MakeGetItemRequest();
			GetItem getItem = new GetItem(base.CallContext, request);
			getItem.PreExecute();
			ServiceResult<ItemType[]> serviceResult = getItem.Execute();
			getItem.SetCurrentStepResult(serviceResult);
			getItem.PostExecute();
			this.Logger.LogInfo("GetMessage() took {0} seconds.", new object[]
			{
				stopwatch.Elapsed.TotalSeconds
			});
			return serviceResult.Value[0] as MessageType;
		}

		private ICredentials GetCredentials()
		{
			ADUser accessingADUser = this.callContext.AccessingADUser;
			return OAuthCredentials.GetOAuthCredentialsForAppActAsToken(accessingADUser.OrganizationId, accessingADUser, null);
		}

		private string GetSharePointUrlFromAAD()
		{
			ADUser accessingADUser = this.callContext.AccessingADUser;
			Uri rootSiteUrl = SharePointUrl.GetRootSiteUrl(accessingADUser.OrganizationId);
			if (rootSiteUrl != null)
			{
				string text = rootSiteUrl.ToString();
				this.Logger.LogInfo("Found SharePoint Url in AAD: {0}", new object[]
				{
					text
				});
				return text;
			}
			this.Logger.LogInfo("No SharePoint Url in AAD.", new object[0]);
			return "https://msft.spoppe.com";
		}

		internal ADRecipient GetADRecipient(EmailAddressWrapper recipient)
		{
			ADRecipient result = null;
			if (recipient.RoutingType.Equals("SMTP", StringComparison.OrdinalIgnoreCase))
			{
				Directory.TryFindRecipient(recipient.EmailAddress, this.ADRecipientSession, out result);
			}
			else if (recipient.RoutingType.Equals("EX", StringComparison.OrdinalIgnoreCase))
			{
				result = this.ADRecipientSession.FindByLegacyExchangeDN(recipient.EmailAddress);
			}
			return result;
		}

		internal bool IsOpenDL(ADGroup group)
		{
			return group != null && group.MemberJoinRestriction == MemberUpdateType.Open;
		}

		internal bool IsPublicGroup(ADUser user)
		{
			GroupMailbox groupMailbox = GroupMailbox.FromDataObject(user);
			return groupMailbox.ModernGroupType == ModernGroupObjectType.Public;
		}

		internal List<string> GetOpenDLs(EmailAddressWrapper[] recipients)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			int num = recipients.Length;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			List<string> list = new List<string>();
			foreach (EmailAddressWrapper emailAddressWrapper in recipients)
			{
				if (emailAddressWrapper.MailboxType == MailboxHelper.MailboxTypeType.PublicDL.ToString())
				{
					num2++;
					ADRecipient adrecipient = this.GetADRecipient(emailAddressWrapper);
					if (this.IsOpenDL(adrecipient as ADGroup))
					{
						num3++;
						list.Add(emailAddressWrapper.EmailAddress);
					}
				}
				else if (emailAddressWrapper.MailboxType == MailboxHelper.MailboxTypeType.GroupMailbox.ToString())
				{
					num4++;
					ADRecipient adrecipient2 = this.GetADRecipient(emailAddressWrapper);
					if (this.IsPublicGroup(adrecipient2 as ADUser))
					{
						num5++;
						list.Add(emailAddressWrapper.EmailAddress);
					}
				}
			}
			this.Logger.LogInfo("GetOpenDLs() took {0} seconds.", new object[]
			{
				stopwatch.Elapsed.TotalSeconds
			});
			this.Logger.LogInfo("Total recipients:{0} Total DLs:{1} Total open DLs:{2} Total groups:{3} Total open groups:{4}", new object[]
			{
				num,
				num2,
				num3,
				num4,
				num5
			});
			this.linkStats.nrRecipients = num;
			this.linkStats.nrDLs = num2;
			this.linkStats.nrOpenDLs = num3;
			this.linkStats.nrGroups = num4;
			this.linkStats.nrOpenGroups = num5;
			return list;
		}

		internal bool GetRecipients(out List<string> recipients, out string sender)
		{
			recipients = null;
			sender = null;
			MessageType messageType = this.GetMailMessage();
			if (messageType == null)
			{
				this.Logger.LogWarning("Unable to get mail item.", new object[0]);
				return false;
			}
			if (messageType.From == null)
			{
				this.Logger.LogInfo("No sender found in the mail, the From field was empty.", new object[0]);
				return false;
			}
			if ((messageType.ToRecipients == null || messageType.ToRecipients.Length == 0) && (messageType.CcRecipients == null || messageType.CcRecipients.Length == 0))
			{
				this.Logger.LogInfo("No To or CC recipients found in mail.", new object[0]);
				return false;
			}
			sender = messageType.From.Mailbox.EmailAddress;
			this.linkStats.userHash = LinkClickedSignalStats.GenerateObfuscatingHash(sender);
			List<EmailAddressWrapper> list = new List<EmailAddressWrapper>(messageType.ToRecipients ?? new EmailAddressWrapper[0]);
			list.AddRange(messageType.CcRecipients ?? new EmailAddressWrapper[0]);
			recipients = this.GetOpenDLs(list.ToArray());
			if (recipients.Count == 0)
			{
				this.Logger.LogInfo("No open public DLs or unified groups found in mail.", new object[0]);
				recipients = null;
				sender = null;
				return false;
			}
			return true;
		}

		internal bool FilterUrl(string clickedUrl, string spUrl)
		{
			return clickedUrl.StartsWith(spUrl, true, CultureInfo.InvariantCulture);
		}

		protected override bool InternalExecute()
		{
			bool result;
			try
			{
				this.linkStats.linkHash = LinkClickedSignalStats.GenerateObfuscatingHash(this.url);
				string text = this.GetSharePointUrl();
				if (text == null)
				{
					this.linkStats.isSPURLValid = false;
					this.Logger.LogWarning("No valid SharePoint Url found, aborting.", new object[0]);
					result = false;
				}
				else
				{
					text = text.TrimEnd(new char[]
					{
						'/'
					});
					if (this.FilterUrl(this.url, text))
					{
						this.linkStats.isInternalLink = true;
						this.Logger.LogInfo("The clicked Url is internal, skipping the signal.", new object[0]);
						result = true;
					}
					else
					{
						List<string> recipients = null;
						string sender = null;
						if (this.GetRecipients(out recipients, out sender))
						{
							SharePointSignalRestDataProvider sharePointSignalRestDataProvider = new SharePointSignalRestDataProvider();
							sharePointSignalRestDataProvider.AddAnalyticsSignalSource(new LinkClickedSignalSource(sender, this.url, this.title, this.description, this.imgurl, this.imgdimensions, recipients));
							this.linkStats.isValidSignal = true;
						}
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				this.Logger.LogWarning("Got exception of type: {0}", new object[]
				{
					ex.GetType().ToString()
				});
				throw;
			}
			finally
			{
				this.Logger.LogInfo(this.linkStats.GetLinkClickedSignalStatsLogString(), new object[0]);
				((TraceLogger)this.Logger).Close();
				this.logger = null;
			}
			return result;
		}

		private const string EdogSharePointUrl = "https://msft.spoppe.com";

		private readonly CallContext callContext;

		private readonly ItemId itemid;

		private readonly string url;

		private readonly string title;

		private readonly string description;

		private readonly string imgurl;

		private readonly string imgdimensions;

		private IRecipientSession adRecipientSession;

		private ILogger logger;

		private LinkClickedSignalStats linkStats;

		private Func<MessageType> getmailmessage;

		private Func<string> getsharepointurl;

		private Func<ICredentials> getusercredentials;
	}
}
