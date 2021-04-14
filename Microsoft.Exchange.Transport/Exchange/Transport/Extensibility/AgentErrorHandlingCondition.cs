using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Transport.Extensibility
{
	internal class AgentErrorHandlingCondition
	{
		public AgentErrorHandlingCondition(string contextId, Type exceptionType, int errorDeferCount = 0, string exceptionMessage = null)
		{
			if (contextId == null)
			{
				throw new ArgumentNullException("contextId");
			}
			if (exceptionType == null)
			{
				throw new ArgumentNullException("exceptionType");
			}
			this.ContextId = contextId;
			this.ExceptionTypeName = null;
			this.exceptionType = exceptionType;
			this.exceptionTypes = null;
			this.ExceptionMessage = exceptionMessage;
			this.ErrorDeferCount = errorDeferCount;
		}

		public AgentErrorHandlingCondition(string contextId, IEnumerable<Type> exceptionTypes, int errorDeferCount = 0, string exceptionMessage = null)
		{
			if (contextId == null)
			{
				throw new ArgumentNullException("contextId");
			}
			if (exceptionTypes == null || exceptionTypes.Contains(null))
			{
				throw new ArgumentNullException("exceptionTypes");
			}
			this.ContextId = contextId;
			this.ExceptionTypeName = null;
			this.exceptionType = null;
			this.exceptionTypes = exceptionTypes;
			this.ExceptionMessage = exceptionMessage;
			this.ErrorDeferCount = errorDeferCount;
		}

		public AgentErrorHandlingCondition(string contextId, string exceptionTypeName, int errorDeferCount = 0, string exceptionMessage = null)
		{
			if (contextId == null)
			{
				throw new ArgumentNullException("contextId");
			}
			if (exceptionTypeName == null)
			{
				throw new ArgumentNullException("exceptionTypeName");
			}
			this.ContextId = contextId;
			this.exceptionType = null;
			this.exceptionTypes = null;
			this.ExceptionTypeName = exceptionTypeName;
			this.ExceptionMessage = exceptionMessage;
			this.ErrorDeferCount = errorDeferCount;
		}

		public string ContextId { get; private set; }

		public string ExceptionTypeName { get; private set; }

		public Type ExceptionType
		{
			get
			{
				if (this.exceptionType == null && this.exceptionTypes == null && this.ExceptionTypeName != null)
				{
					this.exceptionType = ((this.ExceptionTypeName == string.Empty) ? typeof(Exception) : Type.GetType(this.ExceptionTypeName));
				}
				return this.exceptionType;
			}
		}

		public IEnumerable<Type> ExceptionTypes
		{
			get
			{
				if (this.exceptionTypes == null && this.exceptionType == null && this.ExceptionTypeName != null)
				{
					string[] source = this.ExceptionTypeName.Split(new char[]
					{
						';'
					});
					if (source.Any<string>())
					{
						Type[] source2 = (from t in source.Select(new Func<string, Type>(Type.GetType))
						where t != null
						select t).ToArray<Type>();
						if (source2.Any<Type>())
						{
							this.exceptionTypes = source2;
						}
					}
				}
				return this.exceptionTypes;
			}
		}

		public string ExceptionMessage { get; private set; }

		public int ErrorDeferCount { get; private set; }

		public bool IsMatch(string contextId, Exception exception, TransportMailItem mailItem)
		{
			if (contextId == null)
			{
				throw new ArgumentNullException("contextId");
			}
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			return (this.ContextId == "*" || this.ContextId == contextId) && ((this.ExceptionTypes != null && this.ExceptionTypes.Any((Type t) => exception.GetType() == t || exception.GetType().IsSubclassOf(t))) || (this.ExceptionType != null && (exception.GetType() == this.ExceptionType || exception.GetType().IsSubclassOf(this.ExceptionType)))) && (string.IsNullOrEmpty(this.ExceptionMessage) || exception.Message.IndexOf(this.ExceptionMessage, StringComparison.InvariantCultureIgnoreCase) >= 0) && (this.ErrorDeferCount == 0 || (mailItem != null && mailItem.ExtendedProperties.GetValue<int>("Microsoft.Exchange.Transport.AgentErrorDeferCount", 0) >= this.ErrorDeferCount));
		}

		private Type exceptionType;

		private IEnumerable<Type> exceptionTypes;
	}
}
