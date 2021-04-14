using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Rtc.Signaling;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	internal class ScenarioTestingHelperMethods
	{
		internal abstract class ServiceTestScenarios
		{
			internal abstract bool Execute(LocalTUILogonConnectivityTester umConn);
		}

		internal class SendDigits : ScenarioTestingHelperMethods.ServiceTestScenarios
		{
			internal SendDigits(string digits)
			{
				this.digitsToSend = digits;
			}

			internal override bool Execute(LocalTUILogonConnectivityTester umConn)
			{
				umConn.DebugTrace("Inside SendDigits Execute()", new object[0]);
				if (umConn.IsCallGone())
				{
					umConn.Error = new TUC_RemoteEndDisconnected();
					return false;
				}
				if (string.IsNullOrEmpty(this.digitsToSend))
				{
					return true;
				}
				bool result;
				try
				{
					foreach (char c in this.digitsToSend)
					{
						umConn.DebugTrace("Inside SendDigits , sending digit {0}", new object[]
						{
							c
						});
						if (!umConn.SendDTMFTone(c))
						{
							return false;
						}
						Thread.Sleep(500);
					}
					result = true;
				}
				catch (Exception ex)
				{
					if (!(ex is RealTimeException) && !(ex is InvalidOperationException))
					{
						throw;
					}
					umConn.Error = new TUC_SendSequenceError(ex.Message, ex);
					umConn.ErrorTrace("Inside SendDigits Execute, error ={0}", new object[]
					{
						ex
					});
					result = false;
				}
				return result;
			}

			private readonly string digitsToSend;
		}

		internal class WaitForStates : ScenarioTestingHelperMethods.ServiceTestScenarios
		{
			internal WaitForStates(double timeoutInsecs, LocalizedString stateName, params string[] list)
			{
				if (list != null)
				{
					this.state = new List<string>();
					for (int i = 0; i < list.Length; i++)
					{
						this.state.Add(list[i]);
					}
				}
				this.secsTimeout = timeoutInsecs;
				this.stateName = stateName;
			}

			internal WaitForStates(LocalizedString stateName, params string[] list) : this(60.0, stateName, list)
			{
			}

			internal WaitForStates(double timeoutInsecs, LocalizedString stateName, List<string> stateToSearch, LocalizedString errorMesg, List<string> stateToExclude)
			{
				this.state = stateToSearch;
				this.secsTimeout = timeoutInsecs;
				this.stateName = stateName;
				this.errorMesg = errorMesg;
				this.statesToExclude = stateToExclude;
				this.isPreemptable = (stateToExclude != null);
			}

			internal WaitForStates(LocalizedString stateName, List<string> stateToSearch, LocalizedString errorMesg, List<string> stateToExclude) : this(60.0, stateName, stateToSearch, errorMesg, stateToExclude)
			{
			}

			internal override bool Execute(LocalTUILogonConnectivityTester umConnection)
			{
				this.umConn = umConnection;
				this.umConn.DebugTrace("Inside WaitForState Execute()", new object[0]);
				if (this.umConn.IsCallGone())
				{
					this.umConn.Error = new TUC_RemoteEndDisconnected();
					return false;
				}
				if (this.state == null)
				{
					return true;
				}
				if (this.secsTimeout <= 0.0)
				{
					this.umConn.Error = new TUC_OperationTimedOutInTestUMConnectivityTask(this.stateName, this.secsTimeout.ToString(CultureInfo.InvariantCulture));
					return false;
				}
				ManualResetEvent[] waitHandles = new ManualResetEvent[]
				{
					this.umConn.CallEndedEvent,
					this.umConn.SIPMessageQueuedEvent
				};
				double num = this.secsTimeout * 1000.0;
				bool flag = false;
				bool flag2 = false;
				while (!flag && !flag2 && num > 0.0)
				{
					ExDateTime utcNow = ExDateTime.UtcNow;
					int num2 = WaitHandle.WaitAny(waitHandles, TimeSpan.FromMilliseconds(num), false);
					num -= ExDateTime.UtcNow.Subtract(utcNow).TotalMilliseconds;
					int num3 = num2;
					switch (num3)
					{
					case 0:
						this.umConn.Error = new TUC_RemoteEndDisconnected();
						flag = true;
						break;
					case 1:
						switch (this.GotMyExpectedMessage(this.state, this.isPreemptable, this.statesToExclude))
						{
						case ScenarioTestingHelperMethods.WaitForStates.MatchedState.Expected:
							this.umConn.DebugTrace("Inside WaitForState Got one of the expected states", new object[0]);
							flag2 = true;
							break;
						case ScenarioTestingHelperMethods.WaitForStates.MatchedState.Excluded:
							this.umConn.Error = new TUC_OperationFailed(this.errorMesg);
							flag = true;
							break;
						default:
							if (num <= 0.0)
							{
								this.umConn.Error = new TUC_OperationTimedOutInTestUMConnectivityTask(this.stateName, this.secsTimeout.ToString(CultureInfo.InvariantCulture));
								flag = true;
							}
							else
							{
								flag = false;
							}
							break;
						}
						break;
					default:
						if (num3 == 258)
						{
							this.umConn.Error = new TUC_OperationTimedOutInTestUMConnectivityTask(this.stateName, this.secsTimeout.ToString(CultureInfo.InvariantCulture));
							flag = true;
						}
						else
						{
							this.umConn.Error = new TUC_OperationTimedOutInTestUMConnectivityTask(this.stateName, this.secsTimeout.ToString(CultureInfo.InvariantCulture));
							flag = true;
						}
						break;
					}
				}
				return flag2;
			}

			private ScenarioTestingHelperMethods.WaitForStates.MatchedState GotMyExpectedMessage(List<string> state, bool matchInExclusionList, List<string> exclusionList)
			{
				this.umConn.DebugTrace("Inside WaitForState GotMyExpectedMessage()", new object[0]);
				ScenarioTestingHelperMethods.WaitForStates.MatchedState result = ScenarioTestingHelperMethods.WaitForStates.MatchedState.NotMatched;
				bool flag = false;
				lock (this.umConn.SIPMessageQueue.SyncRoot)
				{
					while (!flag && this.umConn.SIPMessageQueue.Count > 0)
					{
						string mesg = (string)this.umConn.SIPMessageQueue.Dequeue();
						flag = this.IsThisMesgWhatIWant(mesg, state);
						if (flag)
						{
							result = ScenarioTestingHelperMethods.WaitForStates.MatchedState.Expected;
						}
						else if (matchInExclusionList)
						{
							flag = this.IsThisMesgWhatIWant(mesg, exclusionList);
							result = (flag ? ScenarioTestingHelperMethods.WaitForStates.MatchedState.Excluded : ScenarioTestingHelperMethods.WaitForStates.MatchedState.NotMatched);
						}
					}
					if (this.umConn.SIPMessageQueue.Count == 0)
					{
						this.umConn.SIPMessageQueuedEvent.Reset();
					}
				}
				return result;
			}

			private bool IsThisMesgWhatIWant(string mesg, List<string> state)
			{
				this.umConn.DebugTrace("Inside WaitForState IsThisMesgWhatIWant() with mesg = {0}", new object[]
				{
					mesg
				});
				if (mesg == null)
				{
					return false;
				}
				mesg = mesg.Trim();
				foreach (string str in state)
				{
					string text = "Call-State: " + str;
					this.umConn.DebugTrace("Inside WaitForState Looking for a match of {0} in {1}", new object[]
					{
						text,
						mesg
					});
					if (mesg.Contains(text))
					{
						return true;
					}
				}
				return false;
			}

			private const string CallStatePrefix = "Call-State: ";

			private List<string> state;

			private List<string> statesToExclude;

			private readonly double secsTimeout;

			private LocalizedString stateName;

			private LocalTUILogonConnectivityTester umConn;

			private readonly bool isPreemptable;

			private LocalizedString errorMesg;

			private enum MatchedState
			{
				Expected,
				Excluded,
				NotMatched
			}
		}
	}
}
