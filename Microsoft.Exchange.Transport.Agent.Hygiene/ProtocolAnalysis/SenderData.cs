using System;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis
{
	[Serializable]
	internal abstract class SenderData
	{
		protected SenderData(DateTime tsCreate)
		{
			this.startTime = tsCreate;
			this.Rcpts = new int[2];
			this.Helo = new int[6];
			this.Callid = new int[6];
			this.ValidScl = new int[10];
			this.InvalidScl = new int[10];
			this.Length = new int[15];
			this.validUniqCnt = new UniqueCount();
			this.invalidUniqCnt = new UniqueCount();
		}

		public int UniqueValidRcptCount
		{
			get
			{
				return this.validUniqCnt.Count();
			}
		}

		public int UniqueInvalidRcptCount
		{
			get
			{
				return this.invalidUniqCnt.Count();
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this.startTime;
			}
		}

		public bool SenderBlocked
		{
			get
			{
				return this.Blocked;
			}
			set
			{
				this.Blocked = value;
			}
		}

		public void Merge(SenderData source)
		{
			this.NumMsgs += source.NumMsgs;
			int num = 0;
			while (num < this.Rcpts.Length && num < source.Rcpts.Length)
			{
				this.Rcpts[num] += source.Rcpts[num];
				num++;
			}
			num = 0;
			while (num < this.Helo.Length && num < source.Helo.Length)
			{
				this.Helo[num] += source.Helo[num];
				num++;
			}
			num = 0;
			while (num < this.Callid.Length && num < source.Callid.Length)
			{
				this.Callid[num] += source.Callid[num];
				num++;
			}
			num = 0;
			while (num < this.ValidScl.Length && num < source.ValidScl.Length)
			{
				this.ValidScl[num] += source.ValidScl[num];
				num++;
			}
			num = 0;
			while (num < this.InvalidScl.Length && num < source.InvalidScl.Length)
			{
				this.InvalidScl[num] += source.InvalidScl[num];
				num++;
			}
			num = 0;
			while (num < this.Length.Length && num < source.Length.Length)
			{
				this.Length[num] += source.Length[num];
				num++;
			}
			this.validUniqCnt.Merge(source.validUniqCnt);
			this.invalidUniqCnt.Merge(source.invalidUniqCnt);
		}

		public virtual void OnValidRecipient(string recipient)
		{
			this.Rcpts[0]++;
			this.validUniqCnt.AddItem(recipient);
		}

		public virtual void OnInvalidRecipient(string recipient)
		{
			this.Rcpts[1]++;
			this.invalidUniqCnt.AddItem(recipient);
		}

		public virtual void OnUnknownRecipient(string recipient)
		{
		}

		public virtual void OnEndOfData(int scl, long msgLength, CallerIdStatus status)
		{
			int num = (int)Math.Round(Math.Log((double)(msgLength + 1L)));
			if (num >= this.Length.Length)
			{
				num = this.Length.Length - 1;
			}
			this.NumMsgs++;
			this.Length[num]++;
			switch (status)
			{
			case CallerIdStatus.Valid:
				this.Callid[0]++;
				return;
			case CallerIdStatus.Invalid:
				this.Callid[1]++;
				return;
			case CallerIdStatus.Indeterminate:
				this.Callid[2]++;
				return;
			case CallerIdStatus.EpdError:
				this.Callid[3]++;
				return;
			case CallerIdStatus.Error:
				this.Callid[4]++;
				return;
			case CallerIdStatus.Null:
				this.Callid[5]++;
				return;
			default:
				return;
			}
		}

		public const int SclBuckets = 10;

		public const int LengthBuckets = 15;

		public const int HelloNullRdns = 0;

		public const int HelloEmpty = 1;

		public const int HelloMatchAll = 2;

		public const int HelloMatch2nd = 3;

		public const int HelloMatchLocal = 4;

		public const int HelloNoMatch = 5;

		public const int RcptValid = 0;

		public const int RcptInvalid = 1;

		public const int CallIdValid = 0;

		public const int CallIdInvalid = 1;

		public const int CallIdIndeterminate = 2;

		public const int CallIdEpdError = 3;

		public const int CallIdError = 4;

		public const int CallIdNull = 5;

		private DateTime startTime;

		internal bool Blocked;

		internal int NumMsgs;

		internal int[] Rcpts;

		internal int[] Helo;

		internal int[] Callid;

		internal int[] ValidScl;

		internal int[] InvalidScl;

		internal int[] Length;

		private UniqueCount validUniqCnt;

		private UniqueCount invalidUniqCnt;
	}
}
