using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class ComponentInfo
	{
		private ComponentInfo(Type type)
		{
			this.type = type;
			this.stateInfoMap = new Dictionary<uint, StateInfo>();
			this.signalInfoMap = new Dictionary<uint, SignalInfo>();
			this.transitionInfoMap = new Dictionary<KeyValuePair<uint, uint>, List<TransitionInfo>>();
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("ComponentInfo", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.CoreComponentRegistryTracer, (long)this.type.GetHashCode());
		}

		internal Type Type
		{
			get
			{
				return this.type;
			}
		}

		internal Dictionary<KeyValuePair<uint, uint>, List<TransitionInfo>> TransitionInfoMap
		{
			get
			{
				return this.transitionInfoMap;
			}
		}

		internal Dictionary<uint, StateInfo> StateInfoMap
		{
			get
			{
				return this.stateInfoMap;
			}
		}

		internal Dictionary<uint, SignalInfo> SignalInfoMap
		{
			get
			{
				return this.signalInfoMap;
			}
		}

		public override string ToString()
		{
			return this.type.ToString();
		}

		internal static ComponentInfo Create<T>() where T : StatefulComponent
		{
			return new ComponentInfo(typeof(T));
		}

		internal void RegisterState(Enum state)
		{
			StateInfo stateInfo = StateInfo.Create(state);
			if (this.stateInfoMap.ContainsKey(stateInfo.Value))
			{
				string text = string.Format("State {0} with key {1} has already been registered", state, stateInfo);
				this.diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, text, new object[0]);
				throw new ArgumentException(text);
			}
			this.diagnosticsSession.TraceDebug<StateInfo>("Registering state {0}", stateInfo);
			this.stateInfoMap.Add(stateInfo.Value, stateInfo);
		}

		internal void RegisterSignal(Enum signal, SignalPriority priority)
		{
			SignalInfo signalInfo = SignalInfo.Create(signal, priority);
			if (this.signalInfoMap.ContainsKey(signalInfo.Value))
			{
				string text = string.Format("State {0} with key {1} has already been registered", signalInfo.Value, signalInfo);
				this.diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "RegisterSignal", new object[]
				{
					text
				});
				throw new InvalidOperationException(text);
			}
			this.diagnosticsSession.TraceDebug<SignalInfo>("Registering signal {0}", signalInfo);
			this.signalInfoMap.Add(signalInfo.Value, signalInfo);
		}

		internal void RegisterTransition(uint sourceState, uint signal, uint targetState, ConditionMethod condition, ActionMethod action)
		{
			SignalInfo signalInfo = null;
			this.diagnosticsSession.TraceDebug("Registering transition for source: {0}, signal: {1}, target: {2}, condition: {3}, action: {4}", new object[]
			{
				sourceState,
				signal,
				targetState,
				condition,
				action
			});
			uint num = signal;
			if ((signal & 4026531840U) != 0U)
			{
				num &= 268435455U;
			}
			if (!this.signalInfoMap.TryGetValue(num, out signalInfo))
			{
				throw new ArgumentException(string.Format("Attempting to register a transition for a signal ({0}) that hasn't been registered", signal));
			}
			StateInfo stateInfo = null;
			if (sourceState != 4294967295U && !this.stateInfoMap.TryGetValue(sourceState, out stateInfo))
			{
				throw new ArgumentException(string.Format("Attempting to register a transition for a source state ({0}) that hasn't been registered", sourceState));
			}
			if (targetState == 4294967295U)
			{
				throw new ArgumentException(string.Format("Registering a target state with value 'Any' is disallowed", new object[0]));
			}
			StateInfo arg = null;
			if (!this.stateInfoMap.TryGetValue(targetState, out arg))
			{
				throw new ArgumentException(string.Format("Attempting to register a transition for a target state ({0}) that hasn't been registered", targetState));
			}
			KeyValuePair<uint, uint> key = new KeyValuePair<uint, uint>(sourceState, signal);
			bool flag = false;
			List<TransitionInfo> list;
			if (!this.transitionInfoMap.TryGetValue(key, out list))
			{
				list = new List<TransitionInfo>(1);
				flag = true;
			}
			TransitionInfo transitionInfo = TransitionInfo.Create(condition, action, targetState);
			this.diagnosticsSession.TraceDebug("Registered transition: [{0}, {1}] {2}, a:{3}, c:{4}", new object[]
			{
				stateInfo,
				signalInfo,
				transitionInfo,
				action,
				condition
			});
			if (this.transitionInfoMap.ContainsKey(key))
			{
				this.diagnosticsSession.TraceDebug<StateInfo, SignalInfo>("Transition for state {0} with signal {1} already exists, overwriting.", arg, signalInfo);
			}
			list.Add(transitionInfo);
			if (flag)
			{
				this.transitionInfoMap.Add(key, list);
			}
		}

		private readonly IDiagnosticsSession diagnosticsSession;

		private Type type;

		private Dictionary<KeyValuePair<uint, uint>, List<TransitionInfo>> transitionInfoMap;

		private Dictionary<uint, StateInfo> stateInfoMap;

		private Dictionary<uint, SignalInfo> signalInfoMap;
	}
}
