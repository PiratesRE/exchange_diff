using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class ExceptionMapper
	{
		public ExceptionMapper(ICollection<ExceptionMappingBase> exceptionMappings)
		{
			foreach (ExceptionMappingBase exceptionMappingBase in exceptionMappings)
			{
				this.exceptionMappingDictionary[exceptionMappingBase.ExceptionType] = exceptionMappingBase;
			}
		}

		public ExceptionMapper(ExceptionMapper chainedExceptionMapper, ICollection<ExceptionMappingBase> exceptionMappings) : this(exceptionMappings)
		{
			this.chainedExceptionMapper = chainedExceptionMapper;
		}

		public ServiceError GetServiceError(LocalizedException exception)
		{
			return this.GetServiceError(exception, ExchangeVersion.Current);
		}

		public ServiceError GetServiceError(LocalizedException exception, ExchangeVersion currentExchangeVersion)
		{
			ExTraceGlobals.ExceptionTracer.TraceError<LocalizedException>((long)this.GetHashCode(), "ExceptionMapper.TryGetServiceError called for exception: {0}", exception);
			ServicePermanentException ex = exception as ServicePermanentException;
			if (ex != null)
			{
				return ex.CreateServiceError(currentExchangeVersion);
			}
			ExceptionMappingBase exceptionMapping = this.GetExceptionMapping(exception);
			return exceptionMapping.GetServiceError(exception, currentExchangeVersion);
		}

		internal ExceptionMappingBase GetExceptionMapping(LocalizedException exception)
		{
			ExTraceGlobals.ExceptionTracer.TraceError<LocalizedException>((long)this.GetHashCode(), "ExceptionMapper.GetExceptionMapping called for exception: {0}", exception);
			ExceptionMappingBase exceptionMappingBase = null;
			Type type = exception.GetType();
			while (exceptionMappingBase == null && !type.Equals(typeof(Exception)))
			{
				exceptionMappingBase = this.TryMapExceptionType(type);
				type = type.GetTypeInfo().BaseType;
			}
			if (exceptionMappingBase.TryInnerExceptionForExceptionMapping && exception.InnerException != null)
			{
				LocalizedException ex = exception.InnerException as LocalizedException;
				if (ex != null)
				{
					ExTraceGlobals.ExceptionTracer.TraceError((long)this.GetHashCode(), "ExceptionMapper.TryGetExceptionMapping trying InnerException.");
					ExceptionMappingBase exceptionMapping = this.GetExceptionMapping(ex);
					if (exceptionMapping != null && !exceptionMapping.IsUnmappedException)
					{
						exceptionMappingBase = exceptionMapping;
						ExTraceGlobals.ExceptionTracer.TraceError<LocalizedString, LocalizedException>((long)this.GetHashCode(), "Mapping '{0}' found for inner exception '{1}'.", exceptionMapping.GetLocalizedMessage(ex), ex);
					}
				}
			}
			return exceptionMappingBase;
		}

		private ExceptionMappingBase TryMapExceptionType(Type exceptionType)
		{
			ExceptionMappingBase result = null;
			if (!this.exceptionMappingDictionary.TryGetValue(exceptionType, out result) && this.chainedExceptionMapper != null)
			{
				result = this.chainedExceptionMapper.TryMapExceptionType(exceptionType);
			}
			return result;
		}

		private ExceptionMapper chainedExceptionMapper;

		private Dictionary<Type, ExceptionMappingBase> exceptionMappingDictionary = new Dictionary<Type, ExceptionMappingBase>();
	}
}
