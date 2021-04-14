using System;
using System.Collections;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PlayBackContext
	{
		internal PlayBackContext()
		{
			this.committed = new PlayBackContext.ResumeInfo();
			this.potential = new PlayBackContext.ResumeInfo();
		}

		internal ArrayList Prompts
		{
			get
			{
				return this.committed.Prompts;
			}
			set
			{
				this.committed.Prompts = value;
			}
		}

		internal TimeSpan Offset
		{
			get
			{
				return this.committed.Offset;
			}
			set
			{
				this.committed.Offset = value;
			}
		}

		internal string Id
		{
			get
			{
				return this.committed.Id;
			}
			set
			{
				this.committed.Id = value;
			}
		}

		internal int LastPrompt
		{
			get
			{
				return this.committed.LastPrompt;
			}
			set
			{
				this.committed.LastPrompt = value;
			}
		}

		internal void Reset()
		{
			this.committed.Reset();
		}

		internal void Commit()
		{
			this.committed = (PlayBackContext.ResumeInfo)this.potential.Clone();
		}

		internal void Update(ArrayList prompts)
		{
			this.potential.Prompts = prompts;
		}

		internal void Update(string id, int lastPrompt, TimeSpan offset)
		{
			this.potential.Id = id;
			this.potential.LastPrompt = lastPrompt;
			this.potential.Offset = offset;
		}

		private PlayBackContext.ResumeInfo committed;

		private PlayBackContext.ResumeInfo potential;

		internal class ResumeInfo : ICloneable
		{
			internal ResumeInfo()
			{
				this.Reset();
			}

			internal ArrayList Prompts
			{
				get
				{
					return this.prompts;
				}
				set
				{
					this.prompts = value;
				}
			}

			internal TimeSpan Offset
			{
				get
				{
					return this.offset;
				}
				set
				{
					this.offset = value;
				}
			}

			internal int LastPrompt
			{
				get
				{
					return this.lastPrompt;
				}
				set
				{
					this.lastPrompt = value;
				}
			}

			internal string Id
			{
				get
				{
					return this.id;
				}
				set
				{
					this.id = value;
				}
			}

			public object Clone()
			{
				return new PlayBackContext.ResumeInfo
				{
					id = this.id,
					lastPrompt = this.lastPrompt,
					offset = this.offset,
					prompts = this.prompts
				};
			}

			internal void Reset()
			{
				this.id = string.Empty;
				this.lastPrompt = 0;
				this.offset = TimeSpan.Zero;
				this.prompts = new ArrayList();
			}

			private string id;

			private int lastPrompt;

			private TimeSpan offset;

			private ArrayList prompts;
		}
	}
}
