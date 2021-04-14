using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Microsoft.Office.CompliancePolicy
{
	public sealed class GrayException : CompliancePolicyException
	{
		private GrayException(Exception innerException) : base(string.Format("error message: {0}; original exception call stack: {1}", innerException.Message, innerException.StackTrace), innerException)
		{
		}

		public static bool DefaultIsGrayExceptionDelegate(Exception e)
		{
			return e is AppDomainUnloadedException || e.GetType() == typeof(ArgumentException) || e is ArgumentNullException || e is ArgumentOutOfRangeException || e is ArithmeticException || e is ArrayTypeMismatchException || e is CannotUnloadAppDomainException || e is KeyNotFoundException || e is InvalidEnumArgumentException || e is WarningException || e is FormatException || e is IndexOutOfRangeException || e is InsufficientMemoryException || e is InvalidCastException || e is InvalidOperationException || e is EndOfStreamException || e is InvalidDataException || e is PathTooLongException || e is MulticastNotSupportedException || e is NotSupportedException || e is NullReferenceException || e is InvalidFilterCriteriaException || e is ExternalException || e is InvalidOleVariantTypeException || e is MarshalDirectiveException || e is SafeArrayRankMismatchException || e is SafeArrayTypeMismatchException || e is SerializationException || e is AuthenticationException || e is CryptographicException || e is PolicyException || e is IdentityNotMappedException || e is SecurityException || e is XmlSyntaxException || e is System.ServiceProcess.TimeoutException || e is DecoderFallbackException || e is EncoderFallbackException || e is System.TimeoutException || e is UnauthorizedAccessException || e is XmlSchemaException || e is XmlException || e is XPathException || e is XsltException || e is CommunicationException;
		}

		public static void DefaultReportWatsonDelegate(Exception ex)
		{
		}

		public static void Initialize(Func<Exception, bool> isGrayExceptionDelegate = null, Action<Exception> reportWatsonDelegate = null)
		{
			if (isGrayExceptionDelegate != null)
			{
				GrayException.isGrayExceptionDelegate = isGrayExceptionDelegate;
			}
			if (reportWatsonDelegate != null)
			{
				GrayException.reportWatsonDelegate = reportWatsonDelegate;
			}
		}

		public static void MapAndReportGrayExceptions(Action userCodeDelegate)
		{
			GrayException.MapAndReportGrayExceptions(userCodeDelegate, GrayException.isGrayExceptionDelegate, GrayException.reportWatsonDelegate);
		}

		public static void MapAndReportGrayExceptions(Action userCodeDelegate, Func<Exception, bool> isGrayExceptionDelegate, Action<Exception> reportWatsonDelegate)
		{
			ArgumentValidator.ThrowIfNull("userCodeDelegate", userCodeDelegate);
			ArgumentValidator.ThrowIfNull("isGrayExceptionDelegate", isGrayExceptionDelegate);
			ArgumentValidator.ThrowIfNull("reportWatsonDelegate", reportWatsonDelegate);
			try
			{
				userCodeDelegate();
			}
			catch (Exception ex)
			{
				if (isGrayExceptionDelegate(ex))
				{
					reportWatsonDelegate(ex);
					throw new GrayException(ex);
				}
				throw;
			}
		}

		private static Func<Exception, bool> isGrayExceptionDelegate = new Func<Exception, bool>(GrayException.DefaultIsGrayExceptionDelegate);

		private static Action<Exception> reportWatsonDelegate = new Action<Exception>(GrayException.DefaultReportWatsonDelegate);
	}
}
