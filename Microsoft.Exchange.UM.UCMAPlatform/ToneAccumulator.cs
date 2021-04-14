using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore;

namespace Microsoft.Exchange.UM.UcmaPlatform
{
	internal class ToneAccumulator
	{
		public ToneAccumulator()
		{
			this.accumulator = new List<byte>(16);
		}

		public bool IsEmpty
		{
			get
			{
				bool result;
				lock (this.locker)
				{
					result = (0 == this.accumulator.Count);
				}
				return result;
			}
		}

		public void Add(byte b)
		{
			lock (this.locker)
			{
				this.accumulator.Add(b);
				CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "Accumulator count increased to = '{0}'", new object[]
				{
					this.accumulator.Count
				});
			}
		}

		public void Clear()
		{
			lock (this.locker)
			{
				this.accumulator.Clear();
				CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "Accumulator cleared.", new object[0]);
			}
		}

		public void RebufferDigits(byte[] dtmfDigits)
		{
			if (dtmfDigits != null)
			{
				lock (this.locker)
				{
					this.accumulator.InsertRange(0, dtmfDigits);
					CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "Rebuffered input, accumulator count increased to = '{0}'", new object[]
					{
						this.accumulator.Count
					});
				}
			}
		}

		public byte[] ConsumeAccumulatedDigits(int minDigits, int maxDigits, StopPatterns stopPatterns)
		{
			stopPatterns = (stopPatterns ?? StopPatterns.Empty);
			byte[] result;
			lock (this.locker)
			{
				int i = Math.Min(maxDigits, this.accumulator.Count);
				List<byte> list = null;
				int num = 0;
				int num2 = 0;
				while (i > 0)
				{
					list = this.accumulator.GetRange(0, i);
					ToneAccumulator.FindStopPatternMatches(list, stopPatterns, out num, out num2);
					if (num2 > 0 || (stopPatterns.ContainsAnyKey && StopPatterns.IsAnyKeyInput(new ReadOnlyCollection<byte>(list))))
					{
						break;
					}
					i--;
				}
				if (num2 == 0)
				{
					i = Math.Min(maxDigits, this.accumulator.Count);
					list = this.accumulator.GetRange(0, i);
				}
				this.accumulator.RemoveRange(0, i);
				CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "Accumulator removed '{0}' digits.  '{1}' remain.", new object[]
				{
					i,
					this.accumulator.Count
				});
				result = list.ToArray();
			}
			return result;
		}

		public InputState ComputeInputState(int minDigits, int maxDigits, StopPatterns stopPatterns, string stopTones)
		{
			InputState result;
			lock (this.locker)
			{
				InputState inputState;
				if (maxDigits == 0)
				{
					inputState = InputState.NotAllowed;
				}
				else if (this.accumulator.Count == 0)
				{
					inputState = InputState.NotStarted;
				}
				else if (this.accumulator.Count >= maxDigits)
				{
					inputState = InputState.StartedCompleteNotAmbiguous;
				}
				else if (this.EndsWithAStopTone(stopTones))
				{
					inputState = InputState.StartedCompleteNotAmbiguous;
				}
				else
				{
					inputState = this.ComputeStopPatternState(minDigits, stopPatterns);
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "Accumulator state: '{0}'", new object[]
				{
					inputState
				});
				result = inputState;
			}
			return result;
		}

		public bool ContainsBargeInPattern(StopPatterns patterns)
		{
			bool flag = patterns.IsBargeInPattern(new ReadOnlyCollection<byte>(this.accumulator));
			CallIdTracer.TraceDebug(ExTraceGlobals.UCMATracer, this, "Accumulator contains barge-in: '{0}'", new object[]
			{
				flag
			});
			return flag;
		}

		private static void FindStopPatternMatches(List<byte> digits, StopPatterns stopPatterns, out int partialMatches, out int completeMatches)
		{
			partialMatches = (completeMatches = 0);
			stopPatterns.CountMatches(new ReadOnlyCollection<byte>(digits), out partialMatches, out completeMatches);
		}

		private InputState ComputeStopPatternState(int min, StopPatterns stopPatterns)
		{
			foreach (string text in stopPatterns)
			{
				min = Math.Min(min, text.Length);
			}
			InputState result;
			if (this.accumulator.Count < min)
			{
				result = InputState.StartedNotComplete;
			}
			else
			{
				ExAssert.RetailAssert(this.accumulator.Count > 0, "no digits in accumulator!");
				int num;
				int num2;
				ToneAccumulator.FindStopPatternMatches(this.accumulator, stopPatterns, out num, out num2);
				if (num2 == 0)
				{
					if (num == 0)
					{
						result = InputState.StartedCompleteNotAmbiguous;
					}
					else
					{
						result = InputState.StartedNotComplete;
					}
				}
				else if (num > 1)
				{
					result = InputState.StartedCompleteAmbiguous;
				}
				else
				{
					result = InputState.StartedCompleteNotAmbiguous;
				}
			}
			return result;
		}

		private bool EndsWithAStopTone(string stopTones)
		{
			bool result = false;
			if (this.accumulator.Count > 0)
			{
				char value = Convert.ToChar(this.accumulator[this.accumulator.Count - 1]);
				result = (stopTones.IndexOf(value) != -1);
			}
			return result;
		}

		private List<byte> accumulator;

		private object locker = new object();
	}
}
