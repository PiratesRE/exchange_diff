using System;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[Serializable]
	public sealed class InvalidAppointmentException : SharingSynchronizationException
	{
		public InvalidAppointmentException() : base(Strings.InvalidAppointmentException)
		{
		}

		public InvalidAppointmentException(Exception innerException) : base(Strings.InvalidAppointmentException, innerException)
		{
		}
	}
}
