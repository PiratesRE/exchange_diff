using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common
{
	public class GrayException : LocalizedException
	{
		private GrayException(Exception innerException) : base(LocalizedString.Empty, innerException)
		{
		}

		public static void MapAndReportGrayExceptions(GrayException.UserCodeDelegate tryCode)
		{
			GrayException.<>c__DisplayClass1 CS$<>8__locals1 = new GrayException.<>c__DisplayClass1();
			CS$<>8__locals1.tryCode = tryCode;
			ILUtil.DoTryFilterCatch(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<MapAndReportGrayExceptions>b__0)), new FilterDelegate(null, (UIntPtr)ldftn(ExceptionFilter)), new CatchDelegate(null, (UIntPtr)ldftn(ExceptionCatcher)));
		}

		public static void MapAndReportGrayExceptions(GrayException.UserCodeDelegate tryCode, Action<Exception> reportWatsonDelegate)
		{
			GrayException.<>c__DisplayClass5 CS$<>8__locals1 = new GrayException.<>c__DisplayClass5();
			CS$<>8__locals1.tryCode = tryCode;
			CS$<>8__locals1.reportWatsonDelegate = reportWatsonDelegate;
			ILUtil.DoTryFilterCatch(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<MapAndReportGrayExceptions>b__3)), new FilterDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<MapAndReportGrayExceptions>b__4)), new CatchDelegate(null, (UIntPtr)ldftn(ExceptionCatcher)));
		}

		public static void MapAndReportGrayExceptions(GrayException.UserCodeDelegate tryCode, GrayException.IsGrayExceptionDelegate isGrayExceptionDelegate)
		{
			GrayException.<>c__DisplayClass9 CS$<>8__locals1 = new GrayException.<>c__DisplayClass9();
			CS$<>8__locals1.tryCode = tryCode;
			CS$<>8__locals1.isGrayExceptionDelegate = isGrayExceptionDelegate;
			ILUtil.DoTryFilterCatch(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<MapAndReportGrayExceptions>b__7)), new FilterDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<MapAndReportGrayExceptions>b__8)), new CatchDelegate(null, (UIntPtr)ldftn(ExceptionCatcher)));
		}

		public static void MapAndReportGrayExceptions(GrayException.UserCodeDelegate tryCode, GrayException.ExceptionFilterDelegate exceptionFilter)
		{
			GrayException.<>c__DisplayClassd CS$<>8__locals1 = new GrayException.<>c__DisplayClassd();
			CS$<>8__locals1.tryCode = tryCode;
			CS$<>8__locals1.exceptionFilter = exceptionFilter;
			ILUtil.DoTryFilterCatch(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<MapAndReportGrayExceptions>b__b)), new FilterDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<MapAndReportGrayExceptions>b__c)), new CatchDelegate(null, (UIntPtr)ldftn(ExceptionCatcher)));
		}

		private static bool ShouldMapException(Exception e, GrayException.IsGrayExceptionDelegate del)
		{
			return !GrayException.CrashOnBug() && del(e);
		}

		public static bool ExceptionFilter(object exception)
		{
			return GrayException.ExceptionFilter(exception, new GrayException.IsGrayExceptionDelegate(GrayException.IsGrayException), new Action<Exception>(GrayException.ReportWatson));
		}

		private static bool ExceptionFilter(object exception, GrayException.IsGrayExceptionDelegate isGrayExceptionDelegate, Action<Exception> reportWatsonDelegate)
		{
			Exception ex = exception as Exception;
			if (ex == null)
			{
				return false;
			}
			if (!GrayException.ShouldMapException(ex, isGrayExceptionDelegate))
			{
				return false;
			}
			if (reportWatsonDelegate != null)
			{
				reportWatsonDelegate(ex);
			}
			return true;
		}

		private static void ExceptionCatcher(object exception)
		{
			throw new GrayException((Exception)exception);
		}

		public static bool IsSystemGrayException(Exception e)
		{
			return !(e is LocalizedException) && GrayException.IsGrayException(e);
		}

		public static bool IsGrayException(Exception e)
		{
			return e is LocalizedException || e is AppDomainUnloadedException || e.GetType() == typeof(ArgumentException) || e is ArgumentNullException || e is ArgumentOutOfRangeException || e is ArithmeticException || e is ArrayTypeMismatchException || e is CannotUnloadAppDomainException || e is KeyNotFoundException || e is InvalidEnumArgumentException || e is WarningException || e is FormatException || e is IndexOutOfRangeException || e is InsufficientMemoryException || e is InvalidCastException || e is InvalidOperationException || e is EndOfStreamException || e is InvalidDataException || e is PathTooLongException || e is MulticastNotSupportedException || e is SmtpException || e is NotSupportedException || e is NullReferenceException || e is InvalidFilterCriteriaException || e is ExternalException || e is InvalidOleVariantTypeException || e is MarshalDirectiveException || e is SafeArrayRankMismatchException || e is SafeArrayTypeMismatchException || e is SerializationException || e is AuthenticationException || e is CryptographicException || e is PolicyException || e is IdentityNotMappedException || e is SecurityException || e is XmlSyntaxException || e is System.ServiceProcess.TimeoutException || e is DecoderFallbackException || e is EncoderFallbackException || e is System.TimeoutException || e is UnauthorizedAccessException || e is XmlSchemaException || e is XmlException || e is XPathException || e is XsltException;
		}

		private static bool CrashOnBug()
		{
			string text = ConfigurationManager.AppSettings["CrashOnBug"];
			bool flag;
			return text != null && bool.TryParse(text, out flag) && flag;
		}

		private static void ReportWatson(Exception e)
		{
			if (!ExWatson.IsWatsonReportAlreadySent(e))
			{
				ExWatson.SendReport(e, ReportOptions.None, null);
				ExWatson.SetWatsonReportAlreadySent(e);
			}
		}

		public delegate void UserCodeDelegate();

		public delegate bool IsGrayExceptionDelegate(Exception e);

		public delegate bool ExceptionFilterDelegate(object exception);
	}
}
