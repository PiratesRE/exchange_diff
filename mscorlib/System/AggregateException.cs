using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[DebuggerDisplay("Count = {InnerExceptionCount}")]
	[__DynamicallyInvokable]
	[Serializable]
	public class AggregateException : Exception
	{
		[__DynamicallyInvokable]
		public AggregateException() : base(Environment.GetResourceString("AggregateException_ctor_DefaultMessage"))
		{
			this.m_innerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
		}

		[__DynamicallyInvokable]
		public AggregateException(string message) : base(message)
		{
			this.m_innerExceptions = new ReadOnlyCollection<Exception>(new Exception[0]);
		}

		[__DynamicallyInvokable]
		public AggregateException(string message, Exception innerException) : base(message, innerException)
		{
			if (innerException == null)
			{
				throw new ArgumentNullException("innerException");
			}
			this.m_innerExceptions = new ReadOnlyCollection<Exception>(new Exception[]
			{
				innerException
			});
		}

		[__DynamicallyInvokable]
		public AggregateException(IEnumerable<Exception> innerExceptions) : this(Environment.GetResourceString("AggregateException_ctor_DefaultMessage"), innerExceptions)
		{
		}

		[__DynamicallyInvokable]
		public AggregateException(params Exception[] innerExceptions) : this(Environment.GetResourceString("AggregateException_ctor_DefaultMessage"), innerExceptions)
		{
		}

		[__DynamicallyInvokable]
		public AggregateException(string message, IEnumerable<Exception> innerExceptions) : this(message, (innerExceptions as IList<Exception>) ?? ((innerExceptions == null) ? null : new List<Exception>(innerExceptions)))
		{
		}

		[__DynamicallyInvokable]
		public AggregateException(string message, params Exception[] innerExceptions) : this(message, innerExceptions)
		{
		}

		private AggregateException(string message, IList<Exception> innerExceptions) : base(message, (innerExceptions != null && innerExceptions.Count > 0) ? innerExceptions[0] : null)
		{
			if (innerExceptions == null)
			{
				throw new ArgumentNullException("innerExceptions");
			}
			Exception[] array = new Exception[innerExceptions.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = innerExceptions[i];
				if (array[i] == null)
				{
					throw new ArgumentException(Environment.GetResourceString("AggregateException_ctor_InnerExceptionNull"));
				}
			}
			this.m_innerExceptions = new ReadOnlyCollection<Exception>(array);
		}

		internal AggregateException(IEnumerable<ExceptionDispatchInfo> innerExceptionInfos) : this(Environment.GetResourceString("AggregateException_ctor_DefaultMessage"), innerExceptionInfos)
		{
		}

		internal AggregateException(string message, IEnumerable<ExceptionDispatchInfo> innerExceptionInfos) : this(message, (innerExceptionInfos as IList<ExceptionDispatchInfo>) ?? ((innerExceptionInfos == null) ? null : new List<ExceptionDispatchInfo>(innerExceptionInfos)))
		{
		}

		private AggregateException(string message, IList<ExceptionDispatchInfo> innerExceptionInfos) : base(message, (innerExceptionInfos != null && innerExceptionInfos.Count > 0 && innerExceptionInfos[0] != null) ? innerExceptionInfos[0].SourceException : null)
		{
			if (innerExceptionInfos == null)
			{
				throw new ArgumentNullException("innerExceptionInfos");
			}
			Exception[] array = new Exception[innerExceptionInfos.Count];
			for (int i = 0; i < array.Length; i++)
			{
				ExceptionDispatchInfo exceptionDispatchInfo = innerExceptionInfos[i];
				if (exceptionDispatchInfo != null)
				{
					array[i] = exceptionDispatchInfo.SourceException;
				}
				if (array[i] == null)
				{
					throw new ArgumentException(Environment.GetResourceString("AggregateException_ctor_InnerExceptionNull"));
				}
			}
			this.m_innerExceptions = new ReadOnlyCollection<Exception>(array);
		}

		[SecurityCritical]
		protected AggregateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			Exception[] array = info.GetValue("InnerExceptions", typeof(Exception[])) as Exception[];
			if (array == null)
			{
				throw new SerializationException(Environment.GetResourceString("AggregateException_DeserializationFailure"));
			}
			this.m_innerExceptions = new ReadOnlyCollection<Exception>(array);
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			Exception[] array = new Exception[this.m_innerExceptions.Count];
			this.m_innerExceptions.CopyTo(array, 0);
			info.AddValue("InnerExceptions", array, typeof(Exception[]));
		}

		[__DynamicallyInvokable]
		public override Exception GetBaseException()
		{
			Exception ex = this;
			AggregateException ex2 = this;
			while (ex2 != null && ex2.InnerExceptions.Count == 1)
			{
				ex = ex.InnerException;
				ex2 = (ex as AggregateException);
			}
			return ex;
		}

		[__DynamicallyInvokable]
		public ReadOnlyCollection<Exception> InnerExceptions
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_innerExceptions;
			}
		}

		[__DynamicallyInvokable]
		public void Handle(Func<Exception, bool> predicate)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			List<Exception> list = null;
			for (int i = 0; i < this.m_innerExceptions.Count; i++)
			{
				if (!predicate(this.m_innerExceptions[i]))
				{
					if (list == null)
					{
						list = new List<Exception>();
					}
					list.Add(this.m_innerExceptions[i]);
				}
			}
			if (list != null)
			{
				throw new AggregateException(this.Message, list);
			}
		}

		[__DynamicallyInvokable]
		public AggregateException Flatten()
		{
			List<Exception> list = new List<Exception>();
			List<AggregateException> list2 = new List<AggregateException>();
			list2.Add(this);
			int num = 0;
			while (list2.Count > num)
			{
				IList<Exception> innerExceptions = list2[num++].InnerExceptions;
				for (int i = 0; i < innerExceptions.Count; i++)
				{
					Exception ex = innerExceptions[i];
					if (ex != null)
					{
						AggregateException ex2 = ex as AggregateException;
						if (ex2 != null)
						{
							list2.Add(ex2);
						}
						else
						{
							list.Add(ex);
						}
					}
				}
			}
			return new AggregateException(this.Message, list);
		}

		[__DynamicallyInvokable]
		public override string ToString()
		{
			string text = base.ToString();
			for (int i = 0; i < this.m_innerExceptions.Count; i++)
			{
				text = string.Format(CultureInfo.InvariantCulture, Environment.GetResourceString("AggregateException_ToString"), new object[]
				{
					text,
					Environment.NewLine,
					i,
					this.m_innerExceptions[i].ToString(),
					"<---",
					Environment.NewLine
				});
			}
			return text;
		}

		private int InnerExceptionCount
		{
			get
			{
				return this.InnerExceptions.Count;
			}
		}

		private ReadOnlyCollection<Exception> m_innerExceptions;
	}
}
