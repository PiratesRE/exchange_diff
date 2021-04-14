using System;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("IM")]
	[OwaEventSegmentation(Feature.InstantMessage)]
	internal sealed class InstantMessageEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(InstantMessageEventHandler));
		}

		[OwaEventParameter("cMsg", typeof(string))]
		[OwaEventParameter("sUris", typeof(string), true, false)]
		[OwaEventParameter("iType", typeof(int), false, true)]
		[OwaEventParameter("lDn", typeof(string), false, true)]
		[OwaEvent("SndNwChtMsg")]
		[OwaEventParameter("frmt", typeof(string))]
		public void SendNewChatMessage()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.SendNewChatMessage");
			this.ThowIfInvalidProvider("SendNewChatMessage");
			string[] array = (string[])base.GetParameter("sUris");
			string text = (string)base.GetParameter("lDn");
			string text2 = (array != null && array.Length > 0) ? array[0] : string.Empty;
			string text3 = null;
			bool flag = false;
			if (text != null)
			{
				text3 = this.GetSipUriFromLegacyDn(text, text2);
				this.ThrowIfSipInvalid(text3, false);
				if (text3 != text2)
				{
					array = new string[]
					{
						text3
					};
					flag = true;
				}
			}
			int num;
			if (array.Length < 1)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.SendNewChatMessage. Recipients are empty.");
				num = -1;
			}
			else
			{
				InstantMessageProvider.ProviderMessage message = default(InstantMessageProvider.ProviderMessage);
				message.Body = (string)base.GetParameter("cMsg");
				message.Format = (string)base.GetParameter("frmt");
				message.Recipients = array;
				int[] addressTypes = new int[1];
				message.AddressTypes = addressTypes;
				if (base.IsParameterSet("iType"))
				{
					message.AddressTypes[0] = (int)base.GetParameter("iType");
				}
				num = base.UserContext.InstantMessageManager.Provider.SendNewChatMessage(message);
			}
			this.Writer.WriteLine("{");
			if (flag)
			{
				this.Writer.Write("_sip : '");
				this.Writer.Write(text3);
				this.Writer.WriteLine("',");
			}
			this.Writer.Write("_cid : '");
			this.Writer.Write(num.ToString(CultureInfo.InvariantCulture));
			this.Writer.WriteLine("'");
			this.Writer.Write("}");
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.IMTotalInstantMessagesSent.Increment();
			}
		}

		[OwaEventParameter("cId", typeof(string))]
		[OwaEventParameter("sUris", typeof(string), true, false)]
		[OwaEventParameter("frmt", typeof(string))]
		[OwaEventParameter("cMsg", typeof(string))]
		[OwaEvent("SndChtMsg")]
		public void SendChatMessage()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.SendChatMessage");
			this.ThowIfInvalidProvider("SendChatMessage");
			string text = (string)base.GetParameter("cId");
			int chatSessionId;
			if (!int.TryParse(text, out chatSessionId))
			{
				throw new OwaInvalidRequestException("The chat ID format is not valid:" + text);
			}
			InstantMessageProvider.ProviderMessage message = default(InstantMessageProvider.ProviderMessage);
			message.Body = (string)base.GetParameter("cMsg");
			message.Format = (string)base.GetParameter("frmt");
			message.ChatSessionId = chatSessionId;
			message.Recipients = (string[])base.GetParameter("sUris");
			chatSessionId = base.UserContext.InstantMessageManager.Provider.SendMessage(message);
			this.Writer.WriteLine("{");
			this.Writer.Write("_cid : '");
			this.Writer.Write(chatSessionId.ToString(CultureInfo.InvariantCulture));
			this.Writer.WriteLine("'");
			this.Writer.Write("}");
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.IMTotalInstantMessagesSent.Increment();
			}
		}

		[OwaEventParameter("sbsUris", typeof(string))]
		[OwaEvent("SubChng")]
		[OwaEventParameter("unsbsUris", typeof(string))]
		public void PresenceSubscriptionChange()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.PresenceSubscriptionChange");
			this.ThowIfInvalidProvider("PresenceSubscriptionChange");
			string text = (string)base.GetParameter("sbsUris");
			if (!string.IsNullOrEmpty(text))
			{
				base.UserContext.InstantMessageManager.Provider.AddSubscription(text.Split(new char[]
				{
					','
				}));
			}
			string text2 = (string)base.GetParameter("unsbsUris");
			if (!string.IsNullOrEmpty(text2))
			{
				string[] array = text2.Split(new char[]
				{
					','
				});
				foreach (string text3 in array)
				{
					try
					{
						base.UserContext.InstantMessageManager.Provider.RemoveSubscription(text3);
					}
					catch (ArgumentException ex)
					{
						ExTraceGlobals.OehCallTracer.TraceError<string, string>((long)this.GetHashCode(), "InstantMessageEventHandler.PresenceSubscriptionChange. Unsubscribing SIP Uri: {0}. Exception Message: {1}", (text3 == null) ? string.Empty : text3, (ex.Message == null) ? string.Empty : ex.Message);
					}
				}
			}
		}

		[OwaEventParameter("qryUris", typeof(string))]
		[OwaEvent("QryPrsnc")]
		public void QueryPresence()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.QueryPresence");
			this.ThowIfInvalidProvider("QueryPresence");
			string text = (string)base.GetParameter("qryUris");
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = (from s in text.Split(new char[]
				{
					','
				})
				where s.Trim().Length > 0
				select s).ToArray<string>();
				if (array.Length != 0)
				{
					base.UserContext.InstantMessageManager.Provider.QueryPresence(array);
				}
			}
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.IMTotalPresenceQueries.Increment();
			}
		}

		[OwaEventParameter("dName", typeof(string), false, true)]
		[OwaEventParameter("bdyAddr", typeof(string))]
		[OwaEventParameter("msg", typeof(string), false, true)]
		[OwaEventParameter("lDn", typeof(string), false, true)]
		[OwaEventParameter("grpId", typeof(string), false, true)]
		[OwaEventParameter("grpNme", typeof(string), false, true)]
		[OwaEvent("AddBdy")]
		public void AddBuddy()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.AddBuddy");
			this.ThowIfInvalidProvider("AddBuddy");
			string text = (string)base.GetParameter("lDn");
			string text2 = (string)base.GetParameter("bdyAddr");
			string text3 = string.Empty;
			if (base.IsParameterSet("msg"))
			{
				text3 = (string)base.GetParameter("msg");
				if (text3.Length > 300)
				{
					text3 = text3.Substring(0, 300);
				}
			}
			if (text != null)
			{
				text2 = this.GetSipUriFromLegacyDn(text, text2);
				this.ThrowIfSipInvalid(text2, false);
			}
			try
			{
				InstantMessageBuddy instantMessageBuddy = InstantMessageBuddy.Create((string)base.GetParameter("ID"), text2, (string)base.GetParameter("dName"));
				instantMessageBuddy.RequestMessage = text3;
				InstantMessageGroup group = InstantMessageGroup.Create((string)base.GetParameter("grpId"), (string)base.GetParameter("grpNme"));
				base.UserContext.InstantMessageManager.Provider.AddBuddy(instantMessageBuddy, group);
				if (text != null)
				{
					this.Writer.Write(text2);
				}
			}
			catch (ArgumentException innerException)
			{
				throw new OwaInvalidRequestException("The SipUri is not valid. SipUri:" + text2, innerException);
			}
		}

		[OwaEventParameter("bdyAddr", typeof(string))]
		[OwaEventParameter("iType", typeof(int), false, true)]
		[OwaEventParameter("lDn", typeof(string), false, true)]
		[OwaEvent("RemFrmBdyLst")]
		[OwaEventParameter("sGid", typeof(string), false, true)]
		public void RemoveFromBuddyList()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.RemoveFromBuddyList");
			this.ThowIfInvalidProvider("RemoveFromBuddyList");
			string text = (string)base.GetParameter("lDn");
			string text2 = (string)base.GetParameter("bdyAddr");
			if (text != null)
			{
				text2 = this.GetSipUriFromLegacyDn(text, text2);
				this.ThrowIfSipInvalid(text2, false);
			}
			try
			{
				InstantMessageBuddy instantMessageBuddy = InstantMessageBuddy.Create((string)base.GetParameter("sGid"), text2, string.Empty);
				if (base.IsParameterSet("iType"))
				{
					instantMessageBuddy.AddressType = (int)base.GetParameter("iType");
				}
				base.UserContext.InstantMessageManager.Provider.RemoveBuddy(instantMessageBuddy);
				if (text != null)
				{
					this.Writer.Write(text2);
				}
			}
			catch (ArgumentException innerException)
			{
				throw new OwaInvalidRequestException("The SipUri is not valid. SipUri:" + text2, innerException);
			}
		}

		[OwaEvent("AcceptBdy")]
		[OwaEventParameter("iType", typeof(int))]
		[OwaEventParameter("grpNme", typeof(string), false, true)]
		[OwaEventParameter("bdyAddr", typeof(string))]
		[OwaEventParameter("grpId", typeof(string), false, true)]
		public void AcceptBuddy()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.AcceptBuddy");
			this.ThowIfInvalidProvider("AcceptBuddy");
			InstantMessageBuddy instantMessageBuddy = InstantMessageBuddy.Create(string.Empty, InstantMessageUtilities.ToSipFormat((string)base.GetParameter("bdyAddr")), string.Empty);
			InstantMessageGroup group = InstantMessageGroup.Create((string)base.GetParameter("grpId"), (string)base.GetParameter("grpNme"), InstantMessageGroupType.Standard);
			if (base.IsParameterSet("iType"))
			{
				instantMessageBuddy.AddressType = (int)base.GetParameter("iType");
			}
			base.UserContext.InstantMessageManager.Provider.AcceptBuddy(instantMessageBuddy, group);
		}

		[OwaEventParameter("iType", typeof(int))]
		[OwaEventParameter("bdyAddr", typeof(string))]
		[OwaEvent("DeclineBdy")]
		public void DeclineBuddy()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.DeclineBuddy");
			this.ThowIfInvalidProvider("DeclineBuddy");
			InstantMessageBuddy instantMessageBuddy = InstantMessageBuddy.Create(string.Empty, InstantMessageUtilities.ToSipFormat((string)base.GetParameter("bdyAddr")), string.Empty);
			if (base.IsParameterSet("iType"))
			{
				instantMessageBuddy.AddressType = (int)base.GetParameter("iType");
			}
			base.UserContext.InstantMessageManager.Provider.DeclineBuddy(instantMessageBuddy);
		}

		[OwaEventParameter("iType", typeof(int))]
		[OwaEventParameter("bdyAddr", typeof(string))]
		[OwaEvent("UnblockBuddy")]
		public void UnblockBuddy()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.UnblockBuddy");
			this.ThowIfInvalidProvider("UnblockBuddy");
			InstantMessageBuddy instantMessageBuddy = InstantMessageBuddy.Create(string.Empty, (string)base.GetParameter("bdyAddr"), string.Empty);
			if (base.IsParameterSet("iType"))
			{
				instantMessageBuddy.AddressType = (int)base.GetParameter("iType");
			}
			base.UserContext.InstantMessageManager.Provider.UnblockBuddy(instantMessageBuddy);
		}

		[OwaEvent("BlockBuddy")]
		[OwaEventParameter("bdyAddr", typeof(string))]
		[OwaEventParameter("iType", typeof(int))]
		public void BlockBuddy()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.BlockBuddy");
			this.ThowIfInvalidProvider("BlockBuddy");
			InstantMessageBuddy instantMessageBuddy = InstantMessageBuddy.Create(string.Empty, (string)base.GetParameter("bdyAddr"), string.Empty);
			if (base.IsParameterSet("iType"))
			{
				instantMessageBuddy.AddressType = (int)base.GetParameter("iType");
			}
			base.UserContext.InstantMessageManager.Provider.BlockBuddy(instantMessageBuddy);
		}

		[OwaEventParameter("sUri", typeof(string))]
		[OwaEvent("RemFrmPDL")]
		[OwaEventParameter("grpNme", typeof(string))]
		public void RemoveFromPersonalDistributionList()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.RemoveFromPersonalDistributionList");
		}

		[OwaEventParameter("sGid", typeof(string))]
		[OwaEventParameter("dName", typeof(string), false, true)]
		[OwaEventParameter("tag", typeof(bool), false, true)]
		[OwaEventParameter("grpIds", typeof(string), true, true)]
		[OwaEventParameter("bdyAddr", typeof(string))]
		[OwaEvent("RemFrmGrp")]
		[OwaEventParameter("grpId", typeof(string))]
		[OwaEventParameter("grpNme", typeof(string))]
		public void RemoveFromGroup()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.RemoveFromGroup");
			this.ThowIfInvalidProvider("RemoveFromGroup");
			InstantMessageGroup group = InstantMessageGroup.Create((string)base.GetParameter("grpId"), (string)base.GetParameter("grpNme"));
			InstantMessageBuddy instantMessageBuddy = InstantMessageBuddy.Create((string)base.GetParameter("sGid"), (string)base.GetParameter("bdyAddr"), (string)base.GetParameter("dName"));
			if (base.IsParameterSet("tag"))
			{
				instantMessageBuddy.Tagged = (bool)base.GetParameter("tag");
			}
			instantMessageBuddy.AddGroups((string[])base.GetParameter("grpIds"));
			base.UserContext.InstantMessageManager.Provider.RemoveFromGroup(group, instantMessageBuddy);
		}

		[OwaEventParameter("grpIds", typeof(string), true, true)]
		[OwaEventParameter("oldGrpNme", typeof(string))]
		[OwaEventParameter("tag", typeof(bool), false, true)]
		[OwaEventParameter("bdyAddr", typeof(string))]
		[OwaEventParameter("sGid", typeof(string))]
		[OwaEventParameter("oldGrpId", typeof(string))]
		[OwaEventParameter("newGrpId", typeof(string))]
		[OwaEventParameter("newGrpNme", typeof(string))]
		[OwaEventParameter("dName", typeof(string), false, true)]
		[OwaEvent("MvBdy")]
		public void MoveBuddy()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.MoveBuddy");
			this.ThowIfInvalidProvider("MoveBuddy");
			InstantMessageGroup oldGroup = InstantMessageGroup.Create((string)base.GetParameter("oldGrpId"), (string)base.GetParameter("oldGrpNme"));
			InstantMessageGroup newGroup = InstantMessageGroup.Create((string)base.GetParameter("newGrpId"), (string)base.GetParameter("newGrpNme"));
			InstantMessageBuddy instantMessageBuddy = InstantMessageBuddy.Create((string)base.GetParameter("sGid"), (string)base.GetParameter("bdyAddr"), (string)base.GetParameter("dName"));
			if (base.IsParameterSet("tag"))
			{
				instantMessageBuddy.Tagged = (bool)base.GetParameter("tag");
			}
			instantMessageBuddy.AddGroups((string[])base.GetParameter("grpIds"));
			base.UserContext.InstantMessageManager.Provider.MoveBuddy(oldGroup, newGroup, instantMessageBuddy);
		}

		[OwaEvent("CpBdy")]
		[OwaEventParameter("grpIds", typeof(string), true, true)]
		[OwaEventParameter("bdyAddr", typeof(string))]
		[OwaEventParameter("sGid", typeof(string))]
		[OwaEventParameter("grpId", typeof(string))]
		[OwaEventParameter("grpNme", typeof(string))]
		[OwaEventParameter("dName", typeof(string), false, true)]
		[OwaEventParameter("tag", typeof(bool), false, true)]
		public void CopyBuddy()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.CopyBuddy");
			this.ThowIfInvalidProvider("CopyBuddy");
			InstantMessageGroup group = InstantMessageGroup.Create((string)base.GetParameter("grpId"), (string)base.GetParameter("grpNme"));
			InstantMessageBuddy instantMessageBuddy = InstantMessageBuddy.Create((string)base.GetParameter("sGid"), (string)base.GetParameter("bdyAddr"), (string)base.GetParameter("dName"));
			if (base.IsParameterSet("tag"))
			{
				instantMessageBuddy.Tagged = (bool)base.GetParameter("tag");
			}
			instantMessageBuddy.AddGroups((string[])base.GetParameter("grpIds"));
			base.UserContext.InstantMessageManager.Provider.CopyBuddy(group, instantMessageBuddy);
		}

		[OwaEventParameter("sUri", typeof(string))]
		[OwaEvent("TgBdy")]
		public void TagBuddy()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.TagBuddy");
		}

		[OwaEventParameter("grpNme", typeof(string))]
		[OwaEvent("CrGrp")]
		public void CreateGroup()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.CreateGroup");
			this.ThowIfInvalidProvider("CreateGroup");
			string groupName = (string)base.GetParameter("grpNme");
			base.UserContext.InstantMessageManager.Provider.CreateGroup(groupName);
		}

		[OwaEventParameter("grpNme", typeof(string))]
		[OwaEvent("RemGrp")]
		[OwaEventParameter("grpId", typeof(string))]
		public void RemoveGroup()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.RemoveGroup");
			this.ThowIfInvalidProvider("RemoveGroup");
			InstantMessageGroup group = InstantMessageGroup.Create((string)base.GetParameter("grpId"), (string)base.GetParameter("grpNme"));
			base.UserContext.InstantMessageManager.Provider.RemoveGroup(group);
		}

		[OwaEvent("RenGrp")]
		[OwaEventParameter("grpId", typeof(string))]
		[OwaEventParameter("newGrpNme", typeof(string))]
		[OwaEventParameter("oldGrpNme", typeof(string))]
		public void RenameGroup()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.RenameGroup");
			this.ThowIfInvalidProvider("RenameGroup");
			InstantMessageGroup group = InstantMessageGroup.Create((string)base.GetParameter("grpId"), (string)base.GetParameter("oldGrpNme"));
			string newGroupName = (string)base.GetParameter("newGrpNme");
			base.UserContext.InstantMessageManager.Provider.RenameGroup(group, newGroupName);
		}

		[OwaEvent("GtBdyLst")]
		public void GetBuddyList()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.GetBuddyList");
			this.ThowIfInvalidProvider("GetBuddyList");
			base.UserContext.InstantMessageManager.Provider.GetBuddyList();
		}

		[OwaEventParameter("sUris", typeof(string))]
		[OwaEventParameter("cId", typeof(string))]
		[OwaEvent("InvSmOne")]
		public void InviteSomeone()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.InviteSomeone");
		}

		[OwaEventParameter("cId", typeof(string))]
		[OwaEvent("RmvSmOne")]
		[OwaEventParameter("sUris", typeof(string))]
		public void RemoveSomeone()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.RemoveSomeone");
		}

		[OwaEvent("EndChtSsn")]
		[OwaEventParameter("fDisc", typeof(bool), false, true)]
		[OwaEventParameter("cId", typeof(string))]
		public void EndChatSession()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.EndChatSession");
			this.ThowIfInvalidProvider("EndChatSession");
			bool disconnectSession = false;
			if (base.IsParameterSet("fDisc"))
			{
				disconnectSession = (bool)base.GetParameter("fDisc");
			}
			string text = (string)base.GetParameter("cId");
			int chatSessionId;
			if (int.TryParse(text, out chatSessionId))
			{
				base.UserContext.InstantMessageManager.Provider.EndChatSession(chatSessionId, disconnectSession);
				return;
			}
			throw new OwaInvalidRequestException("The chat ID format is not valid:" + text);
		}

		[OwaEvent("NtfyTpng")]
		[OwaEventParameter("cId", typeof(string))]
		public void NotifyTyping()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.NotifyTyping");
			this.ThowIfInvalidProvider("NotifyTyping");
			string text = (string)base.GetParameter("cId");
			int chatSessionId;
			if (int.TryParse(text, out chatSessionId))
			{
				base.UserContext.InstantMessageManager.Provider.NotifyTyping(chatSessionId, false);
				return;
			}
			throw new OwaInvalidRequestException("The chat ID format is not valid:" + text);
		}

		[OwaEvent("NtfyTpngCncl")]
		[OwaEventParameter("cId", typeof(string))]
		public void NotifyTypingCancelled()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.NotifyTypingCancelled");
			this.ThowIfInvalidProvider("NotifyTypingCancelled");
			string text = (string)base.GetParameter("cId");
			int chatSessionId;
			if (int.TryParse(text, out chatSessionId))
			{
				base.UserContext.InstantMessageManager.Provider.NotifyTyping(chatSessionId, true);
				return;
			}
			throw new OwaInvalidRequestException("The chat ID format is not valid:" + text);
		}

		[OwaEventParameter("expLvl", typeof(int))]
		[OwaEvent("ExpndDL")]
		[OwaEventParameter("alias", typeof(string))]
		public void ExpandDistributionList()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.ExpandDistributionList");
		}

		[OwaEvent("SgnIn")]
		[OwaEventParameter("mnlSI", typeof(bool), false, true)]
		public void SignIn()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.SignIn");
			if (base.UserContext.InstantMessageManager != null)
			{
				bool flag = false;
				if (base.IsParameterSet("mnlSI"))
				{
					flag = (bool)base.GetParameter("mnlSI");
				}
				if (flag)
				{
					base.UserContext.SaveSignedInToIMStatus();
				}
				base.UserContext.InstantMessageManager.StartProvider();
			}
		}

		[OwaEvent("SgnOut")]
		public void SignOut()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.SignOut");
			if (base.UserContext.InstantMessageManager != null)
			{
				base.UserContext.InstantMessageManager.SignOut();
				base.UserContext.SaveSignedOutOfIMStatus();
			}
		}

		[OwaEvent("ChngPrsnc")]
		[OwaEventParameter("prsnce", typeof(int), false, true)]
		public void ChangePresence()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.ChangePresence");
			this.ThowIfInvalidProvider("ChangePresence");
			if (base.IsParameterSet("prsnce"))
			{
				base.UserContext.InstantMessageManager.Provider.PublishSelfPresence((int)base.GetParameter("prsnce"));
				return;
			}
			base.UserContext.InstantMessageManager.Provider.PublishResetStatus();
		}

		[OwaEvent("AccptDeclIMMsg")]
		[OwaEventParameter("sUri", typeof(string))]
		[OwaEventParameter("accpt", typeof(int))]
		public void AcceptDeclineIMMessage()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.AcceptDeclineIMMessage");
		}

		[OwaEvent("IsSssnStrted")]
		public void IsSessionStarted()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.IsSessionStarted");
			bool value = base.UserContext.InstantMessageManager != null && base.UserContext.InstantMessageManager.Provider != null && base.UserContext.InstantMessageManager.Provider.IsSessionStarted;
			this.Writer.Write(value);
		}

		[OwaEvent("GetItemIMInfo")]
		[OwaEventParameter("gtNrmSub", typeof(int), false, true)]
		[OwaEventParameter("mId", typeof(OwaStoreObjectId))]
		public void GetItemIMInfo()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.GetItemIMInfo");
			base.ResponseContentType = OwaEventContentType.Javascript;
			string text = null;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			OwaStoreObjectId itemId = (OwaStoreObjectId)base.GetParameter("mId");
			bool getNormalizedSubject = false;
			if (base.IsParameterSet("gtNrmSub"))
			{
				getNormalizedSubject = ((int)base.GetParameter("gtNrmSub") == 1);
			}
			InstantMessageUtilities.GetItemIMInfo(itemId, getNormalizedSubject, base.UserContext, out text, out text2, out text3, out text4);
			this.Writer.WriteLine("{");
			this.Writer.Write("_dn : '");
			Utilities.JavascriptEncode((text == null) ? string.Empty : text, this.Writer);
			this.Writer.WriteLine("',");
			if (base.UserContext.InstantMessagingType == InstantMessagingTypeOptions.Ocs)
			{
				this.Writer.Write("_uri : '");
				Utilities.JavascriptEncode((text3 == null) ? string.Empty : text3, this.Writer);
				this.Writer.WriteLine("',");
			}
			this.Writer.Write("_sub : '");
			Utilities.JavascriptEncode((text4 == null) ? string.Empty : text4, this.Writer);
			this.Writer.WriteLine("'");
			this.Writer.Write("}");
		}

		[OwaEvent("ParticipateInConversation")]
		[OwaEventParameter("cId", typeof(string))]
		public void ParticipateInConversation()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.ParticipateInConversation");
			this.ThowIfInvalidProvider("ParticipateInConversation");
			string text = (string)base.GetParameter("cId");
			int conversationId;
			if (int.TryParse(text, out conversationId))
			{
				base.UserContext.InstantMessageManager.Provider.ParticipateInConversation(conversationId);
				return;
			}
			throw new OwaInvalidRequestException("The chat ID format is not valid:" + text);
		}

		private void ThrowIfSipInvalid(string sipUri, bool isOperationAllowedOnSelf)
		{
			if (sipUri == null)
			{
				throw new OwaUserNotIMEnabledException(LocalizedStrings.GetNonEncoded(-1159901588));
			}
			if (!isOperationAllowedOnSelf && sipUri == base.OwaContext.UserContext.SipUri)
			{
				throw new OwaIMOperationNotAllowedToSelf(LocalizedStrings.GetNonEncoded(1382663936));
			}
		}

		private void ThowIfInvalidProvider(string methodName)
		{
			if (base.UserContext.InstantMessageManager == null || base.UserContext.InstantMessageManager.Provider == null || !base.UserContext.InstantMessageManager.Provider.IsSessionStarted)
			{
				throw new OwaInstantMessageEventHandlerTransientException("The Instant Message Service is not started yet. Please try again later.");
			}
			if (base.UserContext.InstantMessagingType != InstantMessagingTypeOptions.Ocs)
			{
				throw new OwaInvalidOperationException(methodName + " was called with an instant messaging type that is not supported. Instant Messaging Type:" + base.UserContext.InstantMessagingType.ToString());
			}
		}

		private string GetSipUriFromLegacyDn(string legacyDn, string defaultSipUri)
		{
			IRecipientSession recipientSession = Utilities.CreateADRecipientSession(ConsistencyMode.IgnoreInvalid, base.OwaContext.UserContext);
			Result<ADRawEntry>[] array = recipientSession.FindByLegacyExchangeDNs(new string[]
			{
				legacyDn
			}, InstantMessageEventHandler.recipientQueryProperties);
			if (array == null || array.Length != 1)
			{
				return defaultSipUri;
			}
			ADRawEntry data = array[0].Data;
			if (data == null)
			{
				return defaultSipUri;
			}
			return InstantMessageUtilities.GetSipUri((ProxyAddressCollection)data[ADRecipientSchema.EmailAddresses]);
		}

		[OwaEventParameter("grpIds", typeof(string), true)]
		[OwaEvent("PersistExpand")]
		public void PersistExpandStatus()
		{
			ExTraceGlobals.OehCallTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageEventHandler.PersistExpandStatus");
			string[] groupIds = (string[])base.GetParameter("grpIds");
			InstantMessageExpandPersistence.SetExpandedGroups(base.UserContext, groupIds);
		}

		public const string EventNamespace = "IM";

		public const string MethodSendNewChatMessage = "SndNwChtMsg";

		public const string MethodSendChatMessage = "SndChtMsg";

		public const string MethodPresenceSubscriptionChange = "SubChng";

		public const string MethodQueryPresence = "QryPrsnc";

		public const string MethodAddBuddy = "AddBdy";

		public const string MethodRemoveFromBuddyList = "RemFrmBdyLst";

		public const string MethodAcceptBuddy = "AcceptBdy";

		public const string MethodDeclineBuddy = "DeclineBdy";

		public const string MethodBlockBuddy = "BlockBuddy";

		public const string MethodUnblockBuddy = "UnblockBuddy";

		public const string MethodRemoveFromPersonalDistributionList = "RemFrmPDL";

		public const string MethodRemoveFromGroup = "RemFrmGrp";

		public const string MethodMoveBuddy = "MvBdy";

		public const string MethodCopyBuddy = "CpBdy";

		public const string MethodTagBuddy = "TgBdy";

		public const string MethodCreateGroup = "CrGrp";

		public const string MethodRemoveGroup = "RemGrp";

		public const string MethodRenameGroup = "RenGrp";

		public const string MethodGetBuddyList = "GtBdyLst";

		public const string MethodInviteSomeone = "InvSmOne";

		public const string MethodEndChatSession = "EndChtSsn";

		public const string MethodRemoveSomeone = "RmvSmOne";

		public const string MethodNotifyTyping = "NtfyTpng";

		public const string MethodNotifyTypingCancelled = "NtfyTpngCncl";

		public const string MethodExpandDistributionList = "ExpndDL";

		public const string MethodSignIn = "SgnIn";

		public const string MethodSignOut = "SgnOut";

		public const string MethodChangePresence = "ChngPrsnc";

		public const string MethodAcceptDeclineIMMessage = "AccptDeclIMMsg";

		public const string MethodIsSessionStarted = "IsSssnStrted";

		public const string MethodGetItemIMInfo = "GetItemIMInfo";

		public const string MethodPersistExpand = "PersistExpand";

		public const string MethodParticipateInConversation = "ParticipateInConversation";

		public const string LegacyDN = "lDn";

		public const string SipUri = "sUri";

		public const string DisconnectSession = "fDisc";

		public const string ContactId = "sGid";

		public const string BuddyAddress = "bdyAddr";

		public const string DisplayName = "dName";

		public const string Message = "msg";

		public const string SipUris = "sUris";

		public const string BuddyAddressType = "iType";

		public const string SipUrisToQuery = "qryUris";

		public const string SipUrisToUnsubscribe = "unsbsUris";

		public const string SipUrisToSubscribe = "sbsUris";

		public const string GroupName = "grpNme";

		public const string GroupId = "grpId";

		public const string ChatSessionId = "cId";

		public const string ChatMessage = "cMsg";

		public const string Format = "frmt";

		public const string OldGroupName = "oldGrpNme";

		public const string OldGroupId = "oldGrpId";

		public const string NewGroupName = "newGrpNme";

		public const string NewGroupId = "newGrpId";

		public const string AliasOfDL = "alias";

		public const string LevelOfExpansion = "expLvl";

		public const string AcceptFlag = "accpt";

		public const string NewPresence = "prsnce";

		public const string MItemId = "mId";

		public const string GetNormalizedSubject = "gtNrmSub";

		public const string Password = "sp";

		public const string SavePasswordFlag = "fsp";

		public const string Tagged = "tag";

		public const string ContactGroups = "grpIds";

		public const string SignInManually = "mnlSI";

		private static PropertyDefinition[] recipientQueryProperties = new PropertyDefinition[]
		{
			ADRecipientSchema.PrimarySmtpAddress
		};
	}
}
