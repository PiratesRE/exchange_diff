using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class ArgumentOutOfRangeException : ArgumentException, ISerializable
	{
		private static string RangeMessage
		{
			get
			{
				if (ArgumentOutOfRangeException._rangeMessage == null)
				{
					ArgumentOutOfRangeException._rangeMessage = Environment.GetResourceString("Arg_ArgumentOutOfRangeException");
				}
				return ArgumentOutOfRangeException._rangeMessage;
			}
		}

		[__DynamicallyInvokable]
		public ArgumentOutOfRangeException() : base(ArgumentOutOfRangeException.RangeMessage)
		{
			base.SetErrorCode(-2146233086);
		}

		[__DynamicallyInvokable]
		public ArgumentOutOfRangeException(string paramName) : base(ArgumentOutOfRangeException.RangeMessage, paramName)
		{
			base.SetErrorCode(-2146233086);
		}

		[__DynamicallyInvokable]
		public ArgumentOutOfRangeException(string paramName, string message) : base(message, paramName)
		{
			base.SetErrorCode(-2146233086);
		}

		[__DynamicallyInvokable]
		public ArgumentOutOfRangeException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233086);
		}

		[__DynamicallyInvokable]
		public ArgumentOutOfRangeException(string paramName, object actualValue, string message) : base(message, paramName)
		{
			this.m_actualValue = actualValue;
			base.SetErrorCode(-2146233086);
		}

		[__DynamicallyInvokable]
		public override string Message
		{
			[__DynamicallyInvokable]
			get
			{
				string message = base.Message;
				if (this.m_actualValue == null)
				{
					return message;
				}
				string resourceString = Environment.GetResourceString("ArgumentOutOfRange_ActualValue", new object[]
				{
					this.m_actualValue.ToString()
				});
				if (message == null)
				{
					return resourceString;
				}
				return message + Environment.NewLine + resourceString;
			}
		}

		[__DynamicallyInvokable]
		public virtual object ActualValue
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_actualValue;
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
			info.AddValue("ActualValue", this.m_actualValue, typeof(object));
		}

		protected ArgumentOutOfRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.m_actualValue = info.GetValue("ActualValue", typeof(object));
		}

		private static volatile string _rangeMessage;

		private object m_actualValue;
	}
}
