using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using System.Threading;
using Microsoft.Rtc.Collaboration;
using Microsoft.Rtc.Signaling;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	internal class LocalConnectivityTester : BaseUMconnectivityTester
	{
		internal LocalConnectivityTester()
		{
			base.DebugTrace("Inside LocalConnectivityTester()", new object[0]);
		}

		internal bool AcceptingCalls
		{
			get
			{
				return this.acceptingCalls != null && this.acceptingCalls.Equals("1");
			}
		}

		internal string TotalQueueLength
		{
			get
			{
				return this.totalQueueLength;
			}
		}

		internal override bool ExecuteTest(TestParameters testparams)
		{
			base.DebugTrace("Inside LocalConnectivityTester ExecuteTest()", new object[0]);
			return this.ExecuteSIPINFOTest(testparams);
		}

		protected static bool IsDiagnosticHeaderPresent(MessageReceivedEventArgs e)
		{
			foreach (SignalingHeader signalingHeader in e.RequestData.SignalingHeaders)
			{
				if (signalingHeader.Name.Equals("UMTUCFirstResponse"))
				{
					return true;
				}
			}
			return false;
		}

		protected override void HandleMessageReceived(MessageReceivedEventArgs e)
		{
			base.DebugTrace("Inside LocalConnectivityTester HandleMessageReceived", new object[0]);
			if (e.MessageType != 1 || !e.HasTextBody || !LocalConnectivityTester.IsDiagnosticHeaderPresent(e))
			{
				return;
			}
			this.InfoHandling(e);
		}

		protected virtual bool SendLocalLoopInfoMesg(string dpname)
		{
			base.DebugTrace("Inside LocalConnectivityTester SendLocalLoopInfoMesg()", new object[0]);
			return this.SendInfo("UM Operation Check", null);
		}

		protected bool SendInfo(string data, List<SignalingHeader> headers)
		{
			base.DebugTrace("Inside LocalConnectivityTester SendInfo()", new object[0]);
			bool result;
			lock (this)
			{
				try
				{
					if (base.IsCallGone())
					{
						base.Error = new TUC_RemoteEndDisconnected();
						result = false;
					}
					else
					{
						this.sipInfoEvent = new ManualResetEvent(false);
						ContentType contentType = new ContentType("text/plain");
						ContentDescription contentDescription = new ContentDescription(contentType, data);
						CallSendMessageRequestOptions callSendMessageRequestOptions = new CallSendMessageRequestOptions();
						if (headers != null)
						{
							CollectionExtensions.AddRange<SignalingHeader>(callSendMessageRequestOptions.Headers, headers);
						}
						base.AudioCall.EndSendMessage(base.AudioCall.BeginSendMessage(1, contentDescription, callSendMessageRequestOptions, null, null));
						base.DebugTrace("Inside LocalConnectivityTester SendInfo: sent SIPINFO", new object[0]);
						result = true;
					}
				}
				catch (Exception ex)
				{
					if (!(ex is RealTimeException) && !(ex is InvalidOperationException))
					{
						throw;
					}
					base.Error = new TUC_SendSequenceError(ex.Message, ex);
					base.ErrorTrace("Inside LocalConnectivityTester SendInfo, error ={0}", new object[]
					{
						ex.ToString()
					});
					result = false;
				}
			}
			return result;
		}

		protected void InfoHandling(MessageReceivedEventArgs e)
		{
			string textBody = e.TextBody;
			base.DebugTrace("Inside LocalConnectivityTester InfoHandling, text recvd = {0}", new object[]
			{
				textBody
			});
			string[] array = textBody.Split(new char[]
			{
				':'
			});
			if (array.Length != 4 || !array[0].Equals("OK"))
			{
				return;
			}
			base.CurrCalls = array[1];
			this.totalQueueLength = array[2];
			this.acceptingCalls = array[3];
			this.sipInfoEvent.Set();
		}

		protected bool ExecuteSIPINFOTest(TestParameters testparams)
		{
			base.DebugTrace("Inside LocalConnectivityTester ExecuteSIPINFOTest()", new object[0]);
			if (base.IsCallGone())
			{
				base.Error = new TUC_RemoteEndDisconnected();
				return false;
			}
			if (!this.SendLocalLoopInfoMesg(testparams.DpName))
			{
				return false;
			}
			int num = WaitHandle.WaitAny(new ManualResetEvent[]
			{
				base.CallEndedEvent,
				this.sipInfoEvent
			}, 20000, false);
			int num2 = num;
			switch (num2)
			{
			case 0:
				base.Error = new TUC_RemoteEndDisconnected();
				return false;
			case 1:
				return true;
			default:
				if (num2 == 258)
				{
					base.Error = new TUC_OperationTimedOutInTestUMConnectivityTask(Strings.WaitForDiagnosticResponse, 20.ToString(CultureInfo.InvariantCulture));
					return false;
				}
				base.Error = new TUC_OperationTimedOutInTestUMConnectivityTask(Strings.WaitForDiagnosticResponse, 20.ToString(CultureInfo.InvariantCulture));
				return false;
			}
		}

		private ManualResetEvent sipInfoEvent;

		private string totalQueueLength;

		private string acceptingCalls;
	}
}
