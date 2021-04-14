using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Security.Compliance;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FileSystemPhotoMap
	{
		public FileSystemPhotoMap(string photosRootDirectoryFullPath, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("photosRootDirectoryFullPath", photosRootDirectoryFullPath);
			ArgumentValidator.ThrowIfNull("upstreamTracer", upstreamTracer);
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.photosRootDirectoryFullPath = photosRootDirectoryFullPath;
		}

		public string Map(string smtpAddress, UserPhotoSize size)
		{
			if (string.IsNullOrEmpty(smtpAddress))
			{
				throw new ArgumentNullException("smtpAddress");
			}
			SmtpAddress smtpAddress2 = FileSystemPhotoMap.ParseAndValidate(smtpAddress);
			string text = Path.Combine(this.photosRootDirectoryFullPath, FileSystemPhotoMap.GetEscapedSmtpDomainWithHash(smtpAddress2), FileSystemPhotoMap.MapUserPhotoSizeToFileSystemResolutionDirectory(size), FileSystemPhotoMap.GetEscapedSmtpLocalWithHash(smtpAddress2)) + ".jpg";
			this.tracer.TraceDebug<string, UserPhotoSize, string>((long)this.GetHashCode(), "File system photo map: mapped ({0}, {1}) to {2}", smtpAddress, size, text);
			return text;
		}

		private static string Escape(string part)
		{
			StringBuilder stringBuilder = new StringBuilder(part.Length);
			foreach (char c in part)
			{
				if (FileSystemPhotoMap.IsInvalidCharInPath(c))
				{
					stringBuilder.Append('_');
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		private static bool IsInvalidCharInPath(char c)
		{
			return FileSystemPhotoMap.InvalidCharsInPath.Contains(c);
		}

		private static string MapUserPhotoSizeToFileSystemResolutionDirectory(UserPhotoSize size)
		{
			switch (size)
			{
			case UserPhotoSize.HR48x48:
				return "HR48x48";
			case UserPhotoSize.HR64x64:
				return "HR64x64";
			case UserPhotoSize.HR96x96:
				return "HR96x96";
			case UserPhotoSize.HR120x120:
				return "HR120x120";
			case UserPhotoSize.HR240x240:
				return "HR240x240";
			case UserPhotoSize.HR360x360:
				return "HR360x360";
			case UserPhotoSize.HR432x432:
				return "HR432x432";
			case UserPhotoSize.HR504x504:
				return "HR504x504";
			case UserPhotoSize.HR648x648:
				return "HR648x648";
			default:
				return size.ToString();
			}
		}

		private static SmtpAddress ParseAndValidate(string smtpAddress)
		{
			SmtpAddress smtpAddress2 = new SmtpAddress(smtpAddress);
			if (!smtpAddress2.IsValidAddress || SmtpAddress.NullReversePath.Equals(smtpAddress2))
			{
				throw new CannotMapInvalidSmtpAddressToPhotoFileException(smtpAddress);
			}
			return smtpAddress2;
		}

		private static string NormalizeAndHash(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			string s2 = s.ToLowerInvariant();
			string result;
			using (MessageDigestForNonCryptographicPurposes messageDigestForNonCryptographicPurposes = new MessageDigestForNonCryptographicPurposes())
			{
				byte[] bytes = messageDigestForNonCryptographicPurposes.ComputeHash(Encoding.UTF8.GetBytes(s2));
				result = FileSystemPhotoMap.ConvertToHexadecimalSequence(bytes);
			}
			return result;
		}

		private static string ConvertToHexadecimalSequence(byte[] bytes)
		{
			if (bytes == null || bytes.Length == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(bytes.Length);
			foreach (byte b in bytes)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0:X2}", new object[]
				{
					b
				});
			}
			return stringBuilder.ToString();
		}

		private static string GetEscapedSmtpDomainWithHash(SmtpAddress smtpAddress)
		{
			return "_" + FileSystemPhotoMap.Escape(smtpAddress.Domain.Substring(0, Math.Min(30, smtpAddress.Domain.Length))) + "-" + FileSystemPhotoMap.NormalizeAndHash(smtpAddress.Domain);
		}

		private static string GetEscapedSmtpLocalWithHash(SmtpAddress smtpAddress)
		{
			return "_" + FileSystemPhotoMap.Escape(smtpAddress.Local.Substring(0, Math.Min(20, smtpAddress.Local.Length))) + "-" + FileSystemPhotoMap.NormalizeAndHash(smtpAddress.ToString());
		}

		private const string PhotoFileExtension = ".jpg";

		private const int SmtpDomainPrefixLength = 30;

		private const int SmtpLocalPrefixLength = 20;

		private const string SmtpLocalFilePrefix = "_";

		private const string SmtpDomainDirectoryPrefix = "_";

		private const string SmtpLocalAndHashSeparator = "-";

		private const string SmtpDomainAndHashSeparator = "-";

		private static readonly HashSet<char> InvalidCharsInPath = new HashSet<char>(Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()));

		private readonly ITracer tracer;

		private readonly string photosRootDirectoryFullPath;

		private static class ResolutionPartStrings
		{
			internal const string HR48x48 = "HR48x48";

			internal const string HR64x64 = "HR64x64";

			internal const string HR96x96 = "HR96x96";

			internal const string HR120x120 = "HR120x120";

			internal const string HR240x240 = "HR240x240";

			internal const string HR360x360 = "HR360x360";

			internal const string HR432x432 = "HR432x432";

			internal const string HR504x504 = "HR504x504";

			internal const string HR648x648 = "HR648x648";
		}
	}
}
