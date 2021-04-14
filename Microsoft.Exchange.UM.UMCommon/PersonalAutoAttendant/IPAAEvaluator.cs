using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal interface IPAAEvaluator : IDisposeTrackable, IDisposable
	{
		IDataLoader UserDataLoader { get; set; }

		IPAAStore PAAStorage { get; set; }

		string CallId { get; set; }

		Breadcrumbs Crumbs { get; set; }

		bool EvaluationTimedOut { get; }

		bool SubscriberHasPAAConfigured { get; }

		bool GetEffectivePAA(out PersonalAutoAttendant personalAutoAttendant);
	}
}
