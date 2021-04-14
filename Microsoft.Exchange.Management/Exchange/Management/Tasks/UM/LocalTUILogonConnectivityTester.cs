using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Rtc.Signaling;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	internal class LocalTUILogonConnectivityTester : LocalConnectivityTester
	{
		internal LocalTUILogonConnectivityTester()
		{
			base.DebugTrace("Inside LocalTUILogonConnectivityTester()", new object[0]);
			this.sipMessagesQueue = new Queue();
			this.sipMessageQueuedEvent = new ManualResetEvent(false);
		}

		internal ManualResetEvent SIPMessageQueuedEvent
		{
			get
			{
				return this.sipMessageQueuedEvent;
			}
		}

		internal Queue SIPMessageQueue
		{
			get
			{
				return this.sipMessagesQueue;
			}
		}

		internal override bool ExecuteTest(TestParameters testparams)
		{
			base.DebugTrace("Inside LocalTUILogonConnectivityTester ExecuteTest()", new object[0]);
			if (!base.ExecuteSIPINFOTest(testparams))
			{
				return false;
			}
			ScenarioTestingHelperMethods.ServiceTestScenarios[] tests = new ScenarioTestingHelperMethods.ServiceTestScenarios[]
			{
				new ScenarioTestingHelperMethods.WaitForStates(Strings.PilotNumberState, new string[]
				{
					"0102"
				}),
				new ScenarioTestingHelperMethods.SendDigits(testparams.Phone),
				new ScenarioTestingHelperMethods.WaitForStates(Strings.PINEnterState, new string[]
				{
					"0104"
				}),
				new ScenarioTestingHelperMethods.SendDigits(testparams.PIN + "#"),
				new ScenarioTestingHelperMethods.WaitForStates(Strings.SuccessfulLogonState, new List<string>(new string[]
				{
					"0170",
					"0600",
					"0MainMenuQA"
				}), Strings.LogonError, new List<string>(new string[]
				{
					"0140",
					"0141",
					"0142"
				}))
			};
			return this.ExecuteScenarioTests(tests);
		}

		protected override void HandleMessageReceived(MessageReceivedEventArgs e)
		{
			if (e.MessageType != 1 || !e.HasTextBody)
			{
				return;
			}
			if (LocalConnectivityTester.IsDiagnosticHeaderPresent(e))
			{
				base.InfoHandling(e);
				return;
			}
			this.HandleStateTransitionInfoMsgs(e);
		}

		protected override bool SendLocalLoopInfoMesg(string dpname)
		{
			base.DebugTrace("Inside LocalTUILogonConnectivityTester SendTUILocalLoopInfoMesg()", new object[0]);
			List<SignalingHeader> list = new List<SignalingHeader>();
			SignalingHeader item = new SignalingHeader("UMDialPlan", dpname);
			list.Add(item);
			return base.SendInfo("UM Operation Check", list);
		}

		private void HandleStateTransitionInfoMsgs(MessageReceivedEventArgs e)
		{
			base.DebugTrace("Inside LocalTUILogonConnectivityTester HandleStateTransitionInfoMsgs(), received ={0}", new object[]
			{
				e.TextBody
			});
			lock (this.sipMessagesQueue.SyncRoot)
			{
				base.DebugTrace("Inside LocalTUILogonConnectivityTester HandleStateTransitionInfoMsgs() enqeuing the mesg", new object[0]);
				if (this.sipMessagesQueue.Count == 0)
				{
					this.sipMessageQueuedEvent.Set();
				}
				this.sipMessagesQueue.Enqueue(e.TextBody);
			}
		}

		private bool ExecuteScenarioTests(ScenarioTestingHelperMethods.ServiceTestScenarios[] tests)
		{
			base.DebugTrace("Inside LocalTUILogonConnectivityTester ExecuteScenarioTests()", new object[0]);
			foreach (ScenarioTestingHelperMethods.ServiceTestScenarios serviceTestScenarios in tests)
			{
				if (!serviceTestScenarios.Execute(this))
				{
					return false;
				}
			}
			return true;
		}

		private Queue sipMessagesQueue;

		private ManualResetEvent sipMessageQueuedEvent;
	}
}
