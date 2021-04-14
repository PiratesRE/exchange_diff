using System;
using System.Reflection;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	internal class Handler
	{
		public Handler()
		{
			this.RetryInterval = RetryInterval.None;
			this.Action = MessageAction.Retry;
			this.Category = Category.Transient;
			this.IncludeDiagnosticStatusText = true;
			this.ProcessInnerException = false;
			this.AppliesToAllRecipients = false;
		}

		public MessageAction Action { get; set; }

		public TimeSpan? SpecifiedRetryInterval { get; set; }

		public SmtpResponse Response { get; set; }

		public bool AppliesToAllRecipients { get; set; }

		public Category Category { get; set; }

		public RetryInterval RetryInterval { get; set; }

		public Func<Exception, string> PrimaryStatusTextCallback { get; set; }

		public string PrimaryStatusText { get; set; }

		public Func<Exception, string> EnhancedStatusCodeCallback { get; set; }

		public string EnhancedStatusCode { get; set; }

		public bool IncludeDiagnosticStatusText { get; set; }

		public bool ProcessInnerException { get; set; }

		public Action<Exception, IMessageConverter, MessageStatus> CustomizeStatusCallback { get; set; }

		public static Handler Parse(string serializedProperties)
		{
			Handler handler = new Handler();
			Type type = handler.GetType();
			string[] array = serializedProperties.Split(new char[]
			{
				';'
			});
			foreach (string text in array)
			{
				string[] array3 = text.Trim().Split(new char[]
				{
					'='
				});
				if (array3.Length != 2)
				{
					throw new HandlerParseException(string.Format("Unable to parse Handler properties. Error: {0}", array3));
				}
				string text2 = array3[0];
				PropertyInfo property = Handler.GetProperty(type, text2);
				if (property == null)
				{
					throw new HandlerParseException(string.Format("Unable to parse Handler properties. Error when retrieving property: {0}", text2));
				}
				if (!Handler.IsSerializationSupported(property.PropertyType))
				{
					throw new HandlerParseException(string.Format("Unable to parse Handler properties. Serialization of {0} properties not supported.", property.PropertyType));
				}
				object obj = array3[1];
				if (property.PropertyType.IsEnum)
				{
					obj = Handler.ParseEnumValue(array3[1], text2, property);
				}
				else
				{
					obj = Handler.ConvertValueToPropertyType(text2, property, obj);
				}
				Handler.SetPropertyValue(handler, text2, property, obj);
			}
			return handler;
		}

		public MessageStatus CreateStatus(Exception exception, Exception exceptionAssociatedWithHandler, IMessageConverter converter, TimeSpan fastRetryInterval, TimeSpan quarantineRetryInterval)
		{
			SmtpResponse exceptionSmtpResponse = StorageExceptionHandler.GetExceptionSmtpResponse(converter, exception, this.Category == Category.Permanent, this.GetEffectiveStatusCode(), this.GetEffectiveEnhancedStatusCode(exceptionAssociatedWithHandler), this.GetEffectivePrimaryStatusText(exceptionAssociatedWithHandler), this.IncludeDiagnosticStatusText);
			MessageStatus messageStatus = new MessageStatus(this.Action, exceptionSmtpResponse, exception, this.AppliesToAllRecipients);
			if (this.SpecifiedRetryInterval != null)
			{
				messageStatus.RetryInterval = new TimeSpan?(this.SpecifiedRetryInterval.Value);
			}
			else if (this.RetryInterval != RetryInterval.None)
			{
				if (this.RetryInterval == RetryInterval.QuarantinedRetry)
				{
					messageStatus.RetryInterval = new TimeSpan?(quarantineRetryInterval);
				}
				else
				{
					messageStatus.RetryInterval = new TimeSpan?(fastRetryInterval);
				}
			}
			if (this.CustomizeStatusCallback != null)
			{
				this.CustomizeStatusCallback(exceptionAssociatedWithHandler, converter, messageStatus);
			}
			return messageStatus;
		}

		private static void SetPropertyValue(Handler handler, string propName, PropertyInfo pi, object value)
		{
			try
			{
				pi.SetValue(handler, value, null);
			}
			catch (ArgumentException innerException)
			{
				throw new HandlerParseException(string.Format("Unable to set Handler property {0}:{1}.", propName, value), innerException);
			}
			catch (TargetException innerException2)
			{
				throw new HandlerParseException(string.Format("Unable to set Handler property {0}:{1}.", propName, value), innerException2);
			}
			catch (TargetParameterCountException innerException3)
			{
				throw new HandlerParseException(string.Format("Unable to set Handler property {0}:{1}.", propName, value), innerException3);
			}
			catch (MethodAccessException innerException4)
			{
				throw new HandlerParseException(string.Format("Unable to set Handler property {0}:{1}.", propName, value), innerException4);
			}
			catch (TargetInvocationException innerException5)
			{
				throw new HandlerParseException(string.Format("Unable to set Handler property {0}:{1}.", propName, value), innerException5);
			}
		}

		private static object ConvertValueToPropertyType(string propName, PropertyInfo pi, object serializedValue)
		{
			object obj = null;
			try
			{
				obj = Convert.ChangeType(serializedValue, pi.PropertyType);
			}
			catch (ArgumentNullException innerException)
			{
				throw new HandlerParseException(string.Format("Unable to parse Handler property {0}:{1}.", propName, obj), innerException);
			}
			catch (InvalidCastException innerException2)
			{
				throw new HandlerParseException(string.Format("Unable to parse Handler property {0}:{1}.", propName, obj), innerException2);
			}
			catch (FormatException innerException3)
			{
				throw new HandlerParseException(string.Format("Unable to parse Handler property {0}:{1}.", propName, obj), innerException3);
			}
			catch (OverflowException innerException4)
			{
				throw new HandlerParseException(string.Format("Unable to parse Handler property {0}:{1}.", propName, obj), innerException4);
			}
			return obj;
		}

		private static object ParseEnumValue(string serializedValue, string propName, PropertyInfo pi)
		{
			object obj = null;
			try
			{
				obj = Enum.Parse(pi.PropertyType, serializedValue);
			}
			catch (ArgumentNullException innerException)
			{
				throw new HandlerParseException(string.Format("Unable to parse Handler enum property {0}:{1}.", propName, obj), innerException);
			}
			catch (ArgumentException innerException2)
			{
				throw new HandlerParseException(string.Format("Unable to parse Handler enum property {0}:{1}.", propName, obj), innerException2);
			}
			catch (OverflowException innerException3)
			{
				throw new HandlerParseException(string.Format("Unable to parse Handler enum property {0}:{1}.", propName, obj), innerException3);
			}
			return obj;
		}

		private static PropertyInfo GetProperty(Type handlerType, string propName)
		{
			PropertyInfo property;
			try
			{
				property = handlerType.GetProperty(propName);
			}
			catch (ArgumentNullException innerException)
			{
				throw new HandlerParseException(string.Format("Unable to parse Handler properties. Error when retrieving property: {0}", propName), innerException);
			}
			catch (AmbiguousMatchException innerException2)
			{
				throw new HandlerParseException(string.Format("Unable to parse Handler properties. Error when retrieving property: {0}", propName), innerException2);
			}
			return property;
		}

		private static bool IsSerializationSupported(Type serializedType)
		{
			return serializedType != null && (serializedType.IsEnum || serializedType == typeof(string) || serializedType == typeof(bool));
		}

		private string GetEnhancedStatusCode(Exception e)
		{
			if (this.EnhancedStatusCodeCallback != null)
			{
				return this.EnhancedStatusCodeCallback(e);
			}
			return this.EnhancedStatusCode;
		}

		private string GetPrimaryStatusText(Exception e)
		{
			if (this.PrimaryStatusTextCallback != null)
			{
				return this.PrimaryStatusTextCallback(e);
			}
			return this.PrimaryStatusText;
		}

		private string GetEffectiveStatusCode()
		{
			if (!this.Response.Equals(SmtpResponse.Empty))
			{
				return this.Response.StatusCode;
			}
			return null;
		}

		private string GetEffectiveEnhancedStatusCode(Exception exception)
		{
			string enhancedStatusCode = this.GetEnhancedStatusCode(exception);
			if (!this.Response.Equals(SmtpResponse.Empty))
			{
				return this.Response.EnhancedStatusCode;
			}
			if (!string.IsNullOrEmpty(enhancedStatusCode))
			{
				return enhancedStatusCode;
			}
			return null;
		}

		private string GetEffectivePrimaryStatusText(Exception exception)
		{
			string primaryStatusText = this.GetPrimaryStatusText(exception);
			if (!this.Response.Equals(SmtpResponse.Empty))
			{
				return this.Response.StatusText[0];
			}
			if (!string.IsNullOrEmpty(primaryStatusText))
			{
				return primaryStatusText;
			}
			return null;
		}
	}
}
