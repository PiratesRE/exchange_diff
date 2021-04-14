using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ExPerfTrace
	{
		public static ExPerfTrace.ActivityFrame NewActivity()
		{
			return new ExPerfTrace.ActivityFrame(Guid.NewGuid());
		}

		public static ExPerfTrace.ActivityFrame RelatedActivity(Guid relatedActivityId)
		{
			return new ExPerfTrace.ActivityFrame(relatedActivityId);
		}

		[DllImport("ADVAPI32.DLL", SetLastError = true)]
		private static extern uint EventActivityIdControl([MarshalAs(UnmanagedType.U4)] ExPerfTrace.EVENT_ACTIVITY_CTRL controlCode, ref Guid activityId);

		private const string AdvApi32 = "ADVAPI32.DLL";

		public struct ActivityFrame : IDisposable
		{
			public ActivityFrame(Guid newActivityId)
			{
				this.originalActivityId = Guid.Empty;
				Guid empty = Guid.Empty;
				if (ExPerfTrace.EventActivityIdControl(ExPerfTrace.EVENT_ACTIVITY_CTRL.GET_ID, ref empty) == 0U)
				{
					this.originalActivityId = empty;
				}
				ExPerfTrace.EventActivityIdControl(ExPerfTrace.EVENT_ACTIVITY_CTRL.SET_ID, ref newActivityId);
			}

			public void Dispose()
			{
				ExPerfTrace.EventActivityIdControl(ExPerfTrace.EVENT_ACTIVITY_CTRL.SET_ID, ref this.originalActivityId);
			}

			private Guid originalActivityId;
		}

		private enum EVENT_ACTIVITY_CTRL : uint
		{
			GET_ID = 1U,
			SET_ID,
			CREATE_ID,
			GET_SET_ID,
			CREATE_SET_ID
		}
	}
}
