using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class ArgumentException : SystemException, ISerializable
	{
		[__DynamicallyInvokable]
		public ArgumentException() : base(Environment.GetResourceString("Arg_ArgumentException"))
		{
			base.SetErrorCode(-2147024809);
		}

		[__DynamicallyInvokable]
		public ArgumentException(string message) : base(message)
		{
			base.SetErrorCode(-2147024809);
		}

		[__DynamicallyInvokable]
		public ArgumentException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2147024809);
		}

		[__DynamicallyInvokable]
		public ArgumentException(string message, string paramName, Exception innerException) : base(message, innerException)
		{
			this.m_paramName = paramName;
			base.SetErrorCode(-2147024809);
		}

		[__DynamicallyInvokable]
		public ArgumentException(string message, string paramName) : base(message)
		{
			this.m_paramName = paramName;
			base.SetErrorCode(-2147024809);
		}

		protected ArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.m_paramName = info.GetString("ParamName");
		}

		[__DynamicallyInvokable]
		public override string Message
		{
			[__DynamicallyInvokable]
			get
			{
				string message = base.Message;
				if (!string.IsNullOrEmpty(this.m_paramName))
				{
					string resourceString = Environment.GetResourceString("Arg_ParamName_Name", new object[]
					{
						this.m_paramName
					});
					return message + Environment.NewLine + resourceString;
				}
				return message;
			}
		}

		[__DynamicallyInvokable]
		public virtual string ParamName
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_paramName;
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
			info.AddValue("ParamName", this.m_paramName, typeof(string));
		}

		private string m_paramName;
	}
}
