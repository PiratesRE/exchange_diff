using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ActivityTransition : TransitionBase
	{
		internal ActivityTransition(FsmAction action, string refid, string tevent, bool heavy, bool playback, ActivityManagerConfig manager, ExpressionParser.Expression condition, string refInfo) : base(action, tevent, condition, heavy, playback, refInfo)
		{
			this.endpoint = manager.GetScopedConfig(refid);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Transition type={0}, tevent={1}, action={2}, endpoint={3}", new object[]
			{
				base.GetType().ToString(),
				base.Tevent,
				base.Action,
				this.endpoint.ToString()
			});
		}

		protected override void DoTransition(ActivityManager manager, BaseUMCallSession vo)
		{
			manager.ChangeActivity(this.endpoint.CreateActivity(manager), vo, base.RefInfo);
		}

		private ActivityConfig endpoint;
	}
}
