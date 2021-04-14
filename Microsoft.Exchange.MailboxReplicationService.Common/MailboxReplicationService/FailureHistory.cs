using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class FailureHistory : XMLSerializableBase
	{
		public FailureHistory()
		{
			this.Failures = new List<FailureHistory.MiniFailureRec>();
		}

		[XmlElement(ElementName = "F")]
		public List<FailureHistory.MiniFailureRec> Failures { get; set; }

		public void Add(Exception ex, bool isFatal, int maxHistoryLength)
		{
			if (ex == null || maxHistoryLength <= 0)
			{
				return;
			}
			while (this.Failures.Count >= maxHistoryLength)
			{
				this.Failures.RemoveAt(0);
			}
			this.Failures.Add(new FailureHistory.MiniFailureRec(ex, isFatal));
		}

		public sealed class MiniFailureRec : XMLSerializableBase
		{
			public MiniFailureRec()
			{
			}

			internal MiniFailureRec(Exception ex, bool isFatal)
			{
				this.Timestamp = DateTime.UtcNow;
				this.FailureType = CommonUtils.GetFailureType(ex);
				this.FailureSide = CommonUtils.GetExceptionSide(ex);
				this.ExceptionTypes = CommonUtils.ClassifyException(ex);
				this.IsFatal = isFatal;
				this.ExceptionCallStackHash = CommonUtils.ComputeCallStackHash(ex, 5);
			}

			[XmlAttribute(AttributeName = "Time")]
			public DateTime Timestamp { get; set; }

			[XmlAttribute(AttributeName = "Type")]
			public string FailureType { get; set; }

			[XmlIgnore]
			public ExceptionSide? FailureSide { get; private set; }

			[XmlAttribute(AttributeName = "Side")]
			public int FailureSideInt
			{
				get
				{
					ExceptionSide? failureSide = this.FailureSide;
					if (failureSide == null)
					{
						return 0;
					}
					return (int)failureSide.GetValueOrDefault();
				}
				set
				{
					if (value == 0)
					{
						this.FailureSide = null;
						return;
					}
					this.FailureSide = new ExceptionSide?((ExceptionSide)value);
				}
			}

			[XmlAttribute(AttributeName = "Fatal")]
			public bool IsFatal { get; set; }

			[XmlIgnore]
			public WellKnownException[] ExceptionTypes { get; private set; }

			[XmlAttribute(AttributeName = "ETypes")]
			public int[] ExceptionTypesInt
			{
				get
				{
					if (this.ExceptionTypes == null || this.ExceptionTypes.Length == 0)
					{
						return null;
					}
					int[] array = new int[this.ExceptionTypes.Length];
					for (int i = 0; i < this.ExceptionTypes.Length; i++)
					{
						array[i] = (int)this.ExceptionTypes[i];
					}
					return array;
				}
				set
				{
					if (value == null || value.Length == 0)
					{
						this.ExceptionTypes = null;
						return;
					}
					this.ExceptionTypes = new WellKnownException[value.Length];
					for (int i = 0; i < value.Length; i++)
					{
						this.ExceptionTypes[i] = (WellKnownException)value[i];
					}
				}
			}

			[XmlAttribute(AttributeName = "ExStackHash")]
			public string ExceptionCallStackHash { get; set; }
		}
	}
}
