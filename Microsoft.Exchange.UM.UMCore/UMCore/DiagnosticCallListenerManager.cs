using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DiagnosticCallListenerManager : ActivityManager
	{
		internal DiagnosticCallListenerManager(ActivityManager manager, DiagnosticCallListenerManager.ConfigClass config) : base(manager, config)
		{
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			this.umdialPlan = null;
			base.Manager.WriteVariable("diagnosticTUILogonCheck", false);
			base.Start(vo, refInfo);
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
		}

		internal override void OnMessageReceived(BaseUMCallSession vo, InfoMessage.MessageReceivedEventArgs e)
		{
			base.OnMessageReceived(vo, e);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "DiagnosticCallListenerManager in  OnMessageReceived.", new object[0]);
			if (!e.Message.ContentType.Equals(CommonConstants.ContentTypeTextPlain) || !Encoding.ASCII.GetString(e.Message.Body).Equals("UM Operation Check"))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "DiagnosticCallListenerManager disconnecting call for error in content type or body of SIP INFO.", new object[0]);
				vo.DisconnectCall();
			}
			string text;
			if (e.Message.Headers.TryGetValue("UMDialPlan", out text) && !string.IsNullOrEmpty(text))
			{
				this.umdialPlan = this.GetFirstOrgDialPlanFromName(text);
				if (this.umdialPlan != null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "DiagnosticCallListenerManager setting DiagnosticTUILogonCheck since found Dialplan info.", new object[0]);
					base.Manager.WriteVariable("diagnosticTUILogonCheck", true);
				}
			}
			InfoMessage infoMessage = new InfoMessage();
			infoMessage.ContentType = CommonConstants.ContentTypeTextPlain;
			infoMessage.Headers["UMTUCFirstResponse"] = "true";
			string text2;
			if (UmServiceGlobals.ArePerfCountersEnabled)
			{
				long rawValue = AvailabilityCounters.TotalQueuedMessages.RawValue;
				text2 = string.Concat(new string[]
				{
					"OK:",
					GeneralCounters.CurrentCalls.RawValue.ToString(CultureInfo.InvariantCulture),
					":",
					rawValue.ToString(CultureInfo.InvariantCulture),
					":",
					PipelineDispatcher.Instance.IsPipelineHealthy ? "1" : "0"
				});
			}
			else
			{
				text2 = "OK:0:0:1";
			}
			infoMessage.Body = Encoding.ASCII.GetBytes(text2);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "DiagnosticCallListenerManager sending resp = {0}.", new object[]
			{
				text2
			});
			vo.SendMessage(infoMessage);
		}

		internal override void OnMessageSent(BaseUMCallSession vo, EventArgs e)
		{
			base.OnMessageSent(vo, e);
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "DiagnosticCallListenerManager in OnMessageSent.", new object[0]);
			object obj = base.Manager.ReadVariable("diagnosticTUILogonCheck");
			bool flag = obj != null && (bool)obj;
			if (flag)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "DiagnosticCallListenerManager in OnMessageSent , diagnosticTUILogonCheck is set.", new object[0]);
				vo.CurrentCallContext.DialPlan = this.umdialPlan;
				vo.CurrentCallContext.CalleeInfo = null;
				vo.CurrentCallContext.CallerInfo = null;
				vo.CurrentCallContext.CallType = 1;
				vo.CurrentCallContext.CallerId = PhoneNumber.Empty;
				vo.CurrentCallContext.IsTUIDiagnosticCall = true;
			}
			vo.StopPlayback();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DiagnosticCallListenerManager>(this);
		}

		private UMDialPlan GetFirstOrgDialPlanFromName(string dialPlanName)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 202, "GetFirstOrgDialPlanFromName", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcore\\DiagnosticCallListenerManager.cs");
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, dialPlanName);
			ADObjectId descendantId = tenantOrTopologyConfigurationSession.GetOrgContainerId().GetDescendantId(new ADObjectId("CN=UM DialPlan Container", Guid.Empty));
			UMDialPlan[] array = tenantOrTopologyConfigurationSession.Find<UMDialPlan>(descendantId, QueryScope.SubTree, filter, null, 0);
			if (array != null && array.Length > 0)
			{
				return array[0];
			}
			return null;
		}

		private UMDialPlan umdialPlan;

		internal class ConfigClass : ActivityManagerConfig
		{
			internal ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing UMDiagnostic activity manager.", new object[0]);
				return new DiagnosticCallListenerManager(manager, this);
			}
		}
	}
}
