using System;

namespace Microsoft.Exchange.Data
{
	internal class PredictedActionAndProbability
	{
		public PredictedActionAndProbability(PredictedMessageAction action, short probability)
		{
			this.Action = action;
			this.Probability = (int)probability;
		}

		public PredictedActionAndProbability(short rawActionAndProbability)
		{
			this.RawActionAndProbability = rawActionAndProbability;
		}

		public PredictedMessageAction Action { get; private set; }

		public int Probability
		{
			get
			{
				return this.probability;
			}
			private set
			{
				if (value > 100 || value < 0)
				{
					throw new ArgumentOutOfRangeException("probability");
				}
				this.probability = value;
			}
		}

		public bool Completed { get; set; }

		public short RawActionAndProbability
		{
			get
			{
				short num = (short)this.Action;
				num = (short)(num << 8);
				num += (short)this.Probability;
				if (this.Completed)
				{
					num |= short.MinValue;
				}
				return num;
			}
			set
			{
				this.Completed = ((value & short.MinValue) != 0);
				this.Action = (PredictedMessageAction)(value >> 8 & 127);
				this.Probability = (int)(value & 255);
			}
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as PredictedActionAndProbability);
		}

		public override int GetHashCode()
		{
			return this.Action.GetHashCode() + this.Probability.GetHashCode();
		}

		private bool Equals(PredictedActionAndProbability other)
		{
			return other != null && (object.ReferenceEquals(other, this) || (this.Action.Equals(other.Action) && this.Probability.Equals(other.Probability)));
		}

		private int probability;
	}
}
