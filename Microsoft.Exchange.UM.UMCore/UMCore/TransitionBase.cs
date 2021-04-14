using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class TransitionBase
	{
		protected TransitionBase(FsmAction action, string tevent, ExpressionParser.Expression condition, bool heavy, bool playback, string refInfo)
		{
			this.action = action;
			this.tevent = tevent;
			this.condition = condition;
			this.heavy = heavy;
			this.isPlaybackTransition = playback;
			this.refInfo = refInfo;
		}

		internal ExpressionParser.Expression Condition
		{
			get
			{
				return this.condition;
			}
		}

		internal bool BargeIn
		{
			get
			{
				return !this.isPlaybackTransition;
			}
		}

		protected FsmAction Action
		{
			get
			{
				return this.action;
			}
			set
			{
				this.action = value;
			}
		}

		protected string Tevent
		{
			get
			{
				return this.tevent;
			}
			set
			{
				this.tevent = value;
			}
		}

		protected string RefInfo
		{
			get
			{
				return this.refInfo;
			}
			set
			{
				this.refInfo = value;
			}
		}

		protected bool IsPlaybackTransition
		{
			get
			{
				return this.isPlaybackTransition;
			}
			set
			{
				this.isPlaybackTransition = value;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Transition type={0}, tevent={1}, action={2}", new object[]
			{
				base.GetType().ToString(),
				this.tevent,
				this.action
			});
		}

		internal static TransitionBase Create(FsmAction action, string refid, string tevent, bool heavy, bool playback, ActivityManagerConfig manager, ExpressionParser.Expression condition, string refInfo)
		{
			Match match;
			if ((match = TransitionBase.outRegex.Match(refid)).Success)
			{
				string value = match.Groups["outId"].Value;
				return new OutTransition(action, tevent, value, heavy, manager, condition, refInfo);
			}
			if (TransitionBase.activityRegex.IsMatch(refid))
			{
				return new ActivityTransition(action, refid, tevent, heavy, playback, manager, condition, refInfo);
			}
			return null;
		}

		internal virtual void Execute(ActivityManager manager, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Transition.Execute entered with T={0}.", new object[]
			{
				this
			});
			TransitionBase autoEvent = null;
			bool flag = vo.IsDuringPlayback();
			bool flag2 = this.heavy;
			if (flag && !this.isPlaybackTransition)
			{
				vo.StopPlayback();
				return;
			}
			if (this.action != null)
			{
				if (!flag && flag2 && !vo.IsClosing)
				{
					HeavyBlockingOperation hbo = new HeavyBlockingOperation(manager, vo, this.action, this);
					manager.RunHeavyBlockingOperation(vo, hbo);
					return;
				}
				autoEvent = this.action.Execute(manager, vo);
			}
			this.ProcessAutoEvent(manager, vo, autoEvent);
		}

		internal void ProcessAutoEvent(ActivityManager manager, BaseUMCallSession vo, TransitionBase autoEvent)
		{
			if (autoEvent != null)
			{
				autoEvent.Execute(manager, vo);
				return;
			}
			if (!vo.IsDuringPlayback())
			{
				this.DoTransition(manager, vo);
			}
		}

		protected abstract void DoTransition(ActivityManager manager, BaseUMCallSession vo);

		private const string OutIdGroup = "outId";

		private static Regex activityRegex = new Regex("^(\\d{1,4}|[\\w]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static Regex outRegex = new Regex("^out-(?<outId>\\d{1,4}|[\\w]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static Regex nullRegex = new Regex("^null$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private bool heavy;

		private FsmAction action;

		private string tevent;

		private ExpressionParser.Expression condition;

		private string refInfo;

		private bool isPlaybackTransition;
	}
}
