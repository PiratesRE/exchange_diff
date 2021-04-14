using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public interface ICountableObject
	{
		void IncrementObjectCounter(MapiObjectTrackingScope scope, MapiObjectTrackedType trackedType);

		void DecrementObjectCounter(MapiObjectTrackingScope scope);

		IMapiObjectCounter GetObjectCounter(MapiObjectTrackingScope scope);
	}
}
