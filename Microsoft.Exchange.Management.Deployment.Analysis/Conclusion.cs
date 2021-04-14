using System;

namespace Microsoft.Exchange.Management.Deployment.Analysis
{
	public abstract class Conclusion
	{
		protected Conclusion()
		{
		}

		protected Conclusion(Result result)
		{
			this.name = result.Source.Name;
			this.value = (result.HasException ? null : result.ValueAsObject);
			this.valueType = result.Source.ValueType;
			this.exception = result.Exception;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.ThrowIfReadOnly();
				this.name = value;
			}
		}

		public object Value
		{
			get
			{
				if (this.HasException)
				{
					throw this.exception;
				}
				return this.value;
			}
			set
			{
				this.ThrowIfReadOnly();
				this.value = value;
			}
		}

		public object ValueOrNull
		{
			get
			{
				if (this.HasException)
				{
					return null;
				}
				return this.value;
			}
		}

		public Type ValueType
		{
			get
			{
				return this.valueType;
			}
			set
			{
				this.ThrowIfReadOnly();
				this.valueType = value;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
			set
			{
				this.ThrowIfReadOnly();
				this.exception = value;
			}
		}

		public bool HasException
		{
			get
			{
				return this.Exception != null;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		public virtual void MakeReadOnly()
		{
			this.isReadOnly = true;
		}

		protected void ThrowIfReadOnly()
		{
			if (this.isReadOnly)
			{
				throw new InvalidOperationException(Strings.CannotModifyReadOnlyProperty);
			}
		}

		private string name;

		private object value;

		private Type valueType;

		private Exception exception;

		private bool isReadOnly;
	}
}
