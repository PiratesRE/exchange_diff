using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport
{
	internal static class ExceptionHelper
	{
		public static bool HandleLeakedException
		{
			get
			{
				return Components.TransportAppConfig.WorkerProcess.HandleLeakedException;
			}
		}

		public static bool IsHandleableException(Exception e)
		{
			return ExceptionHelper.IsHandleablePermanentException(e) || ExceptionHelper.IsHandleableTransientException(e);
		}

		public static bool IsHandleableTransientCtsException(Exception e)
		{
			if (ExceptionHelper.IsHandleablePermanentException(e))
			{
				return true;
			}
			IOException ex = e as IOException;
			return ex != null && (ExceptionHelper.IsDiskFullException(ex) || ExceptionHelper.IsUnspecificErrorException(ex));
		}

		public static bool IsHandleablePermanentCtsException(Exception e)
		{
			return false;
		}

		public static bool IsHandleablePermanentException(Exception e)
		{
			return e is ExchangeDataException;
		}

		public static bool IsHandleableTransientException(Exception e)
		{
			if (e is DataSourceTransientException)
			{
				return true;
			}
			if (e is AddressBookTransientException)
			{
				return true;
			}
			if (e is ExchangeConfigurationException)
			{
				return true;
			}
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Transport.ADExceptionHandling.Enabled)
			{
				if (e is DataSourceOperationException)
				{
					return true;
				}
				if (e is DataValidationException)
				{
					return true;
				}
			}
			IOException ex = e as IOException;
			return ex != null && (ExceptionHelper.IsDiskFullException(ex) || ExceptionHelper.IsUnspecificErrorException(ex));
		}

		internal static bool IsDiskFullException(IOException exception)
		{
			return ExceptionHelper.IsSpecificIoException(exception, 2147942512U);
		}

		internal static bool IsUnspecificErrorException(IOException exception)
		{
			return ExceptionHelper.IsSpecificIoException(exception, 2147500037U);
		}

		internal static bool IsSpecificIoException(IOException exception, uint errorCode)
		{
			for (Exception ex = exception; ex != null; ex = ex.InnerException)
			{
				if (ex.GetType() == typeof(IOException))
				{
					uint hrforException = (uint)Marshal.GetHRForException(ex);
					if (hrforException == errorCode)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static void StopServiceOnFatalError(string reason)
		{
			Components.StopService(reason, false, false, false);
		}

		private const uint Win32ErrorDiskFull = 2147942512U;

		private const uint Win32UnspecifiedError = 2147500037U;
	}
}
