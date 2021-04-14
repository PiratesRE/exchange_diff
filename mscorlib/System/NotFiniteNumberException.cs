using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class NotFiniteNumberException : ArithmeticException
	{
		public NotFiniteNumberException() : base(Environment.GetResourceString("Arg_NotFiniteNumberException"))
		{
			this._offendingNumber = 0.0;
			base.SetErrorCode(-2146233048);
		}

		public NotFiniteNumberException(double offendingNumber)
		{
			this._offendingNumber = offendingNumber;
			base.SetErrorCode(-2146233048);
		}

		public NotFiniteNumberException(string message) : base(message)
		{
			this._offendingNumber = 0.0;
			base.SetErrorCode(-2146233048);
		}

		public NotFiniteNumberException(string message, double offendingNumber) : base(message)
		{
			this._offendingNumber = offendingNumber;
			base.SetErrorCode(-2146233048);
		}

		public NotFiniteNumberException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233048);
		}

		public NotFiniteNumberException(string message, double offendingNumber, Exception innerException) : base(message, innerException)
		{
			this._offendingNumber = offendingNumber;
			base.SetErrorCode(-2146233048);
		}

		protected NotFiniteNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this._offendingNumber = (double)info.GetInt32("OffendingNumber");
		}

		public double OffendingNumber
		{
			get
			{
				return this._offendingNumber;
			}
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("OffendingNumber", this._offendingNumber, typeof(int));
		}

		private double _offendingNumber;
	}
}
