using System;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.UM.ClientAccess
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[MessageInspectorBehavior]
	[ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
	internal class UMWebService : IUM12LegacyContract
	{
		private IBudget GetBudgetFromContext(ExchangePrincipal exchangePrincipal)
		{
			WindowsPrincipal windowsPrincipal = HttpContext.Current.User as WindowsPrincipal;
			IStandardBudget standardBudget;
			if (windowsPrincipal != null)
			{
				standardBudget = StandardBudget.Acquire((windowsPrincipal.Identity as WindowsIdentity).User, Global.BudgetType, exchangePrincipal.MailboxInfo.OrganizationId.ToADSessionSettings());
			}
			else
			{
				standardBudget = StandardBudget.AcquireFallback(HttpContext.Current.User.Identity.Name, Global.BudgetType);
			}
			try
			{
				string callerInfo = "UMWebService.GetBudgetFromContext";
				ResourceLoadDelayInfo.CheckResourceHealth(standardBudget, this.workloadSettings, this.resources);
				standardBudget.StartConnection(callerInfo);
				standardBudget.StartLocal(callerInfo, default(TimeSpan));
			}
			catch (Exception)
			{
				standardBudget.Dispose();
				throw;
			}
			return standardBudget;
		}

		public bool IsUMEnabled()
		{
			bool retVal = false;
			this.DoOperation(delegate(UMClientCommon umClient)
			{
				retVal = umClient.IsUMEnabled();
			});
			return retVal;
		}

		public UMProperties GetUMProperties()
		{
			UMProperties umProps = null;
			this.DoOperation(delegate(UMClientCommon umClient)
			{
				UMPropertiesEx umproperties = umClient.GetUMProperties();
				umProps = new UMProperties(umproperties);
			});
			return umProps;
		}

		public void SetOofStatus(bool status)
		{
			this.DoOperation(delegate(UMClientCommon umClient)
			{
				umClient.SetOofStatus(status);
			});
		}

		public void SetPlayOnPhoneDialString(string dialString)
		{
			this.DoOperation(delegate(UMClientCommon umClient)
			{
				umClient.SetPlayOnPhoneDialString(dialString);
			});
		}

		public void SetTelephoneAccessFolderEmail(string base64FolderId)
		{
			this.DoOperation(delegate(UMClientCommon umClient)
			{
				umClient.SetTelephoneAccessFolderEmail(base64FolderId);
			});
		}

		public void SetMissedCallNotificationEnabled(bool status)
		{
			this.DoOperation(delegate(UMClientCommon umClient)
			{
				umClient.SetMissedCallNotificationEnabled(status);
			});
		}

		public void ResetPIN()
		{
			this.DoOperation(delegate(UMClientCommon umClient)
			{
				umClient.ResetPIN();
			});
		}

		public string PlayOnPhone(string base64ObjectId, string dialString)
		{
			string callId = null;
			this.DoOperation(delegate(UMClientCommon umClient)
			{
				callId = umClient.PlayOnPhone(base64ObjectId, dialString);
			});
			return callId;
		}

		public UMCallInfo GetCallInfo(string callId)
		{
			UMCallInfo callInfo = null;
			this.DoOperation(delegate(UMClientCommon umClient)
			{
				UMCallInfoEx callInfo = umClient.GetCallInfo(callId);
				callInfo = new UMCallInfo(callInfo);
			});
			return callInfo;
		}

		public void Disconnect([XmlElement("CallId")] string callId)
		{
			this.DoOperation(delegate(UMClientCommon umClient)
			{
				umClient.Disconnect(callId);
			});
		}

		public string PlayOnPhoneGreeting([XmlElement("GreetingType")] UMGreetingType greetingType, [XmlElement("DialString")] string dialString)
		{
			string callId = null;
			this.DoOperation(delegate(UMClientCommon umClient)
			{
				callId = umClient.PlayOnPhoneGreeting(greetingType, dialString);
			});
			return callId;
		}

		private ExchangePrincipal GetExchangePrincipal()
		{
			ExchangePrincipal result = null;
			using (AuthZClientInfo callerClientInfo = CallContext.GetCallerClientInfo())
			{
				if (callerClientInfo == null || !ExchangePrincipalCache.TryGetFromCache(callerClientInfo.ClientSecurityContext.UserSid, callerClientInfo.GetADRecipientSessionContext(), out result))
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceWarning(0L, "[UMWebService::GetExchangePrincipal] Could not get exchange principal.");
					throw new InvalidPrincipalException();
				}
			}
			return result;
		}

		private void DoOperation(Action<UMClientCommon> operation)
		{
			try
			{
				ExchangePrincipal exchangePrincipal = this.GetExchangePrincipal();
				using (IBudget budgetFromContext = this.GetBudgetFromContext(exchangePrincipal))
				{
					try
					{
						using (UMClientCommon umclientCommon = new UMClientCommon(exchangePrincipal))
						{
							operation(umclientCommon);
						}
					}
					finally
					{
						ResourceLoadDelayInfo.EnforceDelay(budgetFromContext, this.workloadSettings, this.resources, TimeSpan.MaxValue, null);
					}
				}
			}
			catch (LocalizedException exception)
			{
				throw FaultExceptionUtilities.CreateUMFault(exception, FaultParty.Receiver);
			}
		}

		private WorkloadSettings workloadSettings = new WorkloadSettings(WorkloadType.Ews);

		private ResourceKey[] resources = new ResourceKey[]
		{
			ProcessorResourceKey.Local
		};

		internal abstract class Constants
		{
			internal const string WebServiceDescription = "Provides the server support for UM client features in OWA and Outlook";

			internal const string IsUMEnabledDescription = "Returns true if the user defined by the http context is um-enabled, false otherwise";

			internal const string GetUMPropertiesDescription = "Returns all UM properties";

			internal const string SetOofStatusDescription = "Sets the OofStatus property";

			internal const string SetPlayOnPhoneDialStringDescription = "Sets the PlayOnPhoneDialString property";

			internal const string SetTelephoneAccessFolderEmailDescription = "Sets the TelephoneAccessFolderEmail property";

			internal const string SetMissedCallNotificationEnabledDescription = "Sets the MissedCallNotificationEnabled property";

			internal const string SetTelephoneAccessNumbersDescription = "Sets the TelephoneAccessNumbers property";

			internal const string ResetPINDescription = "Changes the PIN (TUI Password) to a new random value";

			internal const string PlayOnPhoneDescription = "Makes an outbound call and plays a voice message over the telephone";

			internal const string GetCallInfoDescription = "Returns information about a call";

			internal const string DisconnectDescription = "Disconnects a call";

			internal const string PlayOnPhoneGreetingDescription = "Makes an outbound call and plays/records a greeting over the telephone";
		}
	}
}
