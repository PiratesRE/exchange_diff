using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal interface IActivationPreferenceSettable<T> : IComparable<T>
	{
		int ActualValue { get; set; }

		int DesiredValue { get; }

		bool InvalidHostServerAllowed { set; }

		bool Matches(T other);
	}
}
