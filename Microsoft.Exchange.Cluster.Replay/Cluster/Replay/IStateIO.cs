using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Common.Cluster;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class IStateIO
	{
		public void UseSetBrokenForIOFailures(ISetBroken setBroken)
		{
			this.m_setBroken = setBroken;
		}

		public bool IOFailures
		{
			get
			{
				return this.m_fSetBrokenCalled;
			}
		}

		protected void IOFailure(Exception ex)
		{
			bool flag = false;
			lock (this)
			{
				if (!this.UseIOFailure)
				{
					throw ex;
				}
				flag = !this.m_fSetBrokenCalled;
				this.m_fSetBrokenCalled = true;
			}
			if (flag)
			{
				this.m_setBroken.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_SavedStateError, ex, new string[]
				{
					ex.ToString()
				});
			}
		}

		public void IOFailureTestHook(Exception e)
		{
			this.IOFailure(e);
		}

		protected bool UseIOFailure
		{
			get
			{
				return this.m_setBroken != null;
			}
		}

		public bool TryReadLong(string valueName, long defaultValue, out long value)
		{
			string text;
			bool result = this.TryReadFromStore(valueName, defaultValue.ToString(), out text);
			IStateIO.trace.TraceDebug<string, string>(0L, "ReadLong: {0}={1}", valueName, text);
			try
			{
				value = long.Parse(text);
			}
			catch (FormatException innerException)
			{
				this.IOFailure(new InvalidSavedStateException(innerException));
				value = defaultValue;
				return false;
			}
			catch (OverflowException innerException2)
			{
				this.IOFailure(new InvalidSavedStateException(innerException2));
				value = defaultValue;
				return false;
			}
			return result;
		}

		public bool TryReadBool(string valueName, bool defaultValue, out bool value)
		{
			string text;
			bool result = this.TryReadFromStore(valueName, defaultValue.ToString(), out text);
			IStateIO.trace.TraceDebug<string, string>((long)this.GetHashCode(), "ReadBool: {0}={1}", valueName, text);
			value = Cluster.StringIEquals(text, "true");
			return result;
		}

		public bool TryReadString(string valueName, string defaultValue, out string value)
		{
			bool result = this.TryReadFromStore(valueName, defaultValue, out value);
			IStateIO.trace.TraceDebug<string, string>((long)this.GetHashCode(), "ReadStr: {0}={1}", valueName, value);
			if (string.IsNullOrEmpty(value))
			{
				value = null;
			}
			return result;
		}

		public bool TryReadDateTime(string valueName, DateTime defaultValue, out DateTime value)
		{
			long fileTime;
			bool result = this.TryReadLong(valueName, defaultValue.ToFileTime(), out fileTime);
			value = DateTime.FromFileTimeUtc(fileTime);
			return result;
		}

		public bool TryReadEnum<T>(string valueName, T defaultValue, out T value)
		{
			string text;
			bool result = this.TryReadFromStore(valueName, defaultValue.ToString(), out text);
			IStateIO.trace.TraceDebug<string, string>((long)this.GetHashCode(), "ReadEnum: {0}={1}", valueName, text);
			try
			{
				value = (T)((object)Enum.Parse(typeof(T), text));
			}
			catch (ArgumentException innerException)
			{
				this.IOFailure(new InvalidSavedStateException(innerException));
				value = defaultValue;
				return false;
			}
			return result;
		}

		protected abstract bool TryReadFromStore(string valueName, string defaultValue, out string value);

		public bool TryReadFromStoreTestHook(string valueName, string defaultValue, out string value)
		{
			return this.TryReadFromStore(valueName, defaultValue, out value);
		}

		public bool TryReadValueNames(out string[] valueNames)
		{
			return this.TryReadValueNamesInternal(out valueNames);
		}

		public void WriteString(string valueName, string value, bool fCritical)
		{
			IStateIO.trace.TraceDebug<string, string>((long)this.GetHashCode(), "WriteStr: {0}={1}", valueName, value);
			this.TryWriteToStore(valueName, value ?? string.Empty, fCritical);
		}

		public void WriteLong(string valueName, long value, bool fCritical)
		{
			IStateIO.trace.TraceDebug<string, long>((long)this.GetHashCode(), "WriteLong: {0}={1}", valueName, value);
			this.TryWriteToStore(valueName, value.ToString(), fCritical);
		}

		public void WriteBool(string valueName, bool value, bool fCritical)
		{
			IStateIO.trace.TraceDebug<string, bool>((long)this.GetHashCode(), "WriteBool: {0}={1}", valueName, value);
			this.TryWriteToStore(valueName, value.ToString(), fCritical);
		}

		public void WriteDateTime(string valueName, DateTime value, bool fCritical)
		{
			IStateIO.trace.TraceDebug<string, DateTime>((long)this.GetHashCode(), "WriteDT: {0}={1}", valueName, value);
			this.WriteLong(valueName, value.ToFileTime(), fCritical);
		}

		public void WriteEnum<T>(string valueName, T value, bool fCritical)
		{
			IStateIO.trace.TraceDebug<string, T>((long)this.GetHashCode(), "WriteEnum: {0}={1}", valueName, value);
			this.TryWriteToStore(valueName, value.ToString(), fCritical);
		}

		protected abstract bool TryWriteToStore(string valueName, string value, bool fCritical);

		protected abstract bool TryReadValueNamesInternal(out string[] valueNames);

		public bool TryWriteToStoreTestHook(string valueName, string value, bool fCritical)
		{
			return this.TryWriteToStore(valueName, value, fCritical);
		}

		public abstract void DeleteState();

		private ISetBroken m_setBroken;

		private bool m_fSetBrokenCalled;

		private static Trace trace = ExTraceGlobals.StateTracer;
	}
}
