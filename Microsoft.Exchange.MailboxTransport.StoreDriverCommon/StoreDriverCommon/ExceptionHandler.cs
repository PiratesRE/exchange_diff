using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	internal class ExceptionHandler
	{
		public ExceptionHandler()
		{
			HandlerOverrideLoader.ApplyConfiguredOverrides(this, ConfigurationManager.AppSettings);
		}

		public bool Register(string name, Dictionary<Type, Handler> handlerDefinition)
		{
			return this.registeredHandlers.TryAdd(name, new ConcurrentDictionary<Type, Handler>(handlerDefinition));
		}

		public bool AddOrUpdateOverrideHandler(string name, Type exceptionType, Handler handler)
		{
			if (!this.overrideHandlers.ContainsKey(name) && !this.overrideHandlers.TryAdd(name, new ConcurrentDictionary<Type, Handler>()))
			{
				return false;
			}
			ConcurrentDictionary<Type, Handler> concurrentDictionary = this.overrideHandlers[name];
			concurrentDictionary.AddOrUpdate(exceptionType, handler, (Type key, Handler oldvalue) => handler);
			return true;
		}

		public bool RemoveOverrideHandler(string name, Type exceptionType)
		{
			bool result = false;
			if (this.overrideHandlers.ContainsKey(name))
			{
				Handler handler = null;
				result = this.overrideHandlers[name].TryRemove(exceptionType, out handler);
			}
			else
			{
				ExceptionHandler.Diag.TraceWarning<string>(0L, "RemoveOverrideHandler called, but no registered handler was found with the following name: {0}", name);
			}
			return result;
		}

		public MessageStatus Handle(string handlerName, Exception exception, IMessageConverter converter, TimeSpan fastRetryInterval, TimeSpan quarantineRetryInterval)
		{
			StorageExceptionHandler.LogException(converter, exception);
			exception.GetType();
			Exception exception2 = null;
			Exception exceptionAssociatedWithHandler = null;
			ConcurrentDictionary<Type, Handler> handlerMap = this.registeredHandlers[handlerName];
			ConcurrentDictionary<Type, Handler> overrideHandlerMap = this.overrideHandlers.ContainsKey(handlerName) ? this.overrideHandlers[handlerName] : null;
			Handler handler = ExceptionHandler.FindHandler(handlerMap, overrideHandlerMap, exception, out exception2, out exceptionAssociatedWithHandler);
			if (handler == null)
			{
				handler = ExceptionHandler.ThrowHandler;
			}
			return handler.CreateStatus(exception2, exceptionAssociatedWithHandler, converter, fastRetryInterval, quarantineRetryInterval);
		}

		private static Handler FindHandler(ConcurrentDictionary<Type, Handler> handlerMap, ConcurrentDictionary<Type, Handler> overrideHandlerMap, Exception exception, out Exception exceptionToProcess, out Exception exceptionAssociatedWithHandler)
		{
			exceptionToProcess = exception;
			exceptionAssociatedWithHandler = exception;
			if (!(exception is StoreDriverAgentRaisedException))
			{
				if (!ExceptionHandler.IsBaseExceptionType(exception))
				{
					Handler handlerForException = ExceptionHandler.GetHandlerForException(handlerMap, overrideHandlerMap, exception, false);
					if (handlerForException != null)
					{
						exceptionToProcess = exception;
						exceptionAssociatedWithHandler = exception;
						return handlerForException;
					}
				}
				if (exception.InnerException != null && !(exception is SmtpResponseException))
				{
					Handler handlerForException2 = ExceptionHandler.GetHandlerForException(handlerMap, overrideHandlerMap, exception.InnerException, true);
					if (handlerForException2 != null)
					{
						exceptionAssociatedWithHandler = exception.InnerException;
						exceptionToProcess = (handlerForException2.ProcessInnerException ? exception.InnerException : exception);
						return handlerForException2;
					}
				}
				return ExceptionHandler.GetHandlerForException(handlerMap, overrideHandlerMap, exception, true);
			}
			if (exception.InnerException != null)
			{
				exceptionToProcess = exception.InnerException;
				exceptionAssociatedWithHandler = exception.InnerException;
				return ExceptionHandler.GetHandlerForException(handlerMap, overrideHandlerMap, exception.InnerException, true);
			}
			return null;
		}

		private static Handler GetHandlerForException(ConcurrentDictionary<Type, Handler> handlerMap, ConcurrentDictionary<Type, Handler> overrideHandlerMap, Exception exception, bool traverseTypeHierarchy = true)
		{
			if (exception is SmtpResponseException)
			{
				return ExceptionHandler.CreateHandlerFromSmtpResponseException(exception as SmtpResponseException);
			}
			Type type = exception.GetType();
			while (overrideHandlerMap == null || !overrideHandlerMap.ContainsKey(type))
			{
				if (handlerMap.ContainsKey(type))
				{
					return handlerMap[type];
				}
				type = type.BaseType;
				if (!(type != typeof(object)) || !traverseTypeHierarchy)
				{
					return null;
				}
			}
			return overrideHandlerMap[type];
		}

		private static bool IsBaseExceptionType(Exception exception)
		{
			Type type = exception.GetType();
			return type == typeof(StoragePermanentException) || type == typeof(StorageTransientException) || type == typeof(MapiPermanentException) || type == typeof(MapiRetryableException);
		}

		private static Handler CreateHandlerFromSmtpResponseException(SmtpResponseException smtpResponseException)
		{
			Handler handler = new Handler
			{
				Action = smtpResponseException.Status.Action,
				Response = smtpResponseException.Status.Response,
				IncludeDiagnosticStatusText = false
			};
			if (smtpResponseException.Status.RetryInterval != null)
			{
				handler.SpecifiedRetryInterval = new TimeSpan?(smtpResponseException.Status.RetryInterval.Value);
			}
			return handler;
		}

		public static readonly Handler ThrowHandler = new Handler
		{
			Action = MessageAction.Throw
		};

		private static readonly Trace Diag = ExTraceGlobals.StoreDriverDeliveryTracer;

		private ConcurrentDictionary<string, ConcurrentDictionary<Type, Handler>> registeredHandlers = new ConcurrentDictionary<string, ConcurrentDictionary<Type, Handler>>();

		private ConcurrentDictionary<string, ConcurrentDictionary<Type, Handler>> overrideHandlers = new ConcurrentDictionary<string, ConcurrentDictionary<Type, Handler>>();
	}
}
