using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class ApnsFeedbackFileBase : IApnsFeedbackFile, IApnsFeedbackProvider, IEquatable<IApnsFeedbackFile>
	{
		protected ApnsFeedbackFileBase(ApnsFeedbackFileId identifier, ApnsFeedbackFileIO fileIO, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("identifier", identifier);
			ArgumentValidator.ThrowIfNull("fileIO", fileIO);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.identifier = identifier;
			this.fileIO = fileIO;
			this.tracer = tracer;
		}

		public ApnsFeedbackFileId Identifier
		{
			get
			{
				return this.identifier;
			}
		}

		public ApnsFeedbackFileIO FileIO
		{
			get
			{
				return this.fileIO;
			}
		}

		public ITracer Tracer
		{
			get
			{
				return this.tracer;
			}
		}

		public virtual bool IsLoaded
		{
			get
			{
				throw new NotImplementedException("ApnsFeedbackFileBase.IsLoaded");
			}
		}

		public virtual void Load()
		{
			throw new NotImplementedException("ApnsFeedbackFileBase.Load");
		}

		public virtual bool HasExpired(TimeSpan expirationThreshold)
		{
			return this.Identifier.Date < ExDateTime.UtcNow.Subtract(expirationThreshold);
		}

		public virtual void Remove()
		{
			throw new NotImplementedException("ApnsFeedbackFileBase.Remove");
		}

		public virtual ApnsFeedbackValidationResult ValidateNotification(ApnsNotification notification)
		{
			throw new NotImplementedException("ApnsFeedbackFileBase.ValidateNotification");
		}

		public bool Equals(IApnsFeedbackFile other)
		{
			return other != null && this.Identifier.Equals(other.Identifier);
		}

		public override bool Equals(object obj)
		{
			IApnsFeedbackFile apnsFeedbackFile = obj as IApnsFeedbackFile;
			return apnsFeedbackFile != null && this.Equals(apnsFeedbackFile);
		}

		public override int GetHashCode()
		{
			return this.Identifier.ToString().GetHashCode();
		}

		public override string ToString()
		{
			return this.Identifier.ToString();
		}

		protected static List<T> FindFeedbackFiles<T>(string root, string searchPattern, ApnsFeedbackFileIO fileIO, Func<ApnsFeedbackFileId, T> constructor, ITracer tracer) where T : ApnsFeedbackFileBase
		{
			return ApnsFeedbackFileBase.FindFeedbackFiles<T>(root, searchPattern, SearchOption.TopDirectoryOnly, fileIO, constructor, tracer);
		}

		protected static List<T> FindFeedbackFiles<T>(string root, string searchPattern, SearchOption searchOption, ApnsFeedbackFileIO fileIO, Func<ApnsFeedbackFileId, T> constructor, ITracer tracer) where T : ApnsFeedbackFileBase
		{
			ArgumentValidator.ThrowIfNullOrEmpty("root", root);
			ArgumentValidator.ThrowIfNullOrEmpty("searchPattern", searchPattern);
			ArgumentValidator.ThrowIfNull("fileIO", fileIO);
			tracer.TraceDebug<string, string, SearchOption>(0L, "[FindFeedbackFiles] Searching for files under '{0}' with pattern '{1}' and option '{2}'.", root, searchPattern, searchOption);
			List<T> list = new List<T>();
			Exception ex = null;
			try
			{
				string[] files = fileIO.GetFiles(root, searchPattern, searchOption);
				foreach (string text in files)
				{
					tracer.TraceDebug<string>(0L, "[FindFeedbackFiles] File name found: {0}.", text);
					ApnsFeedbackFileId arg = ApnsFeedbackFileId.Parse(text);
					T item = constructor(arg);
					list.Add(item);
				}
			}
			catch (UnauthorizedAccessException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				throw new ApnsFeedbackException(Strings.ApnsFeedbackFileGetFilesError(searchPattern, root, ex.Message), ex);
			}
			return list;
		}

		private ApnsFeedbackFileId identifier;

		private ApnsFeedbackFileIO fileIO;

		private ITracer tracer;
	}
}
