using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Rtf
{
	internal class RtfToHtmlAdapter : IProducerConsumer, IDisposable
	{
		public RtfToHtmlAdapter(RtfParser parser, ConverterOutput output, RtfToHtml rtfToHtml, IProgressMonitor progressMonitor)
		{
			this.parser = parser;
			this.output = output;
			this.rtfToHtml = rtfToHtml;
			this.progressMonitor = progressMonitor;
		}

		void IDisposable.Dispose()
		{
			if (this.parser != null && this.parser is IDisposable)
			{
				((IDisposable)this.parser).Dispose();
			}
			if (this.output != null && this.output != null)
			{
				((IDisposable)this.output).Dispose();
			}
			this.parser = null;
			this.output = null;
			GC.SuppressFinalize(this);
		}

		public void Run()
		{
			if (this.consumerOrProducer != null)
			{
				this.consumerOrProducer.Run();
				return;
			}
			this.ParseAndWatch();
		}

		public bool Flush()
		{
			if (this.consumerOrProducer != null)
			{
				return this.consumerOrProducer.Flush();
			}
			this.Run();
			return false;
		}

		private void ParseAndWatch()
		{
			while (!this.parser.ParseRun())
			{
				if (this.parser.ParseBufferFull)
				{
					this.Restart(RtfEncapsulation.None);
					return;
				}
				if (!this.parser.ReadMoreData(false))
				{
					return;
				}
			}
			RtfRunKind runKind = this.parser.RunKind;
			if (runKind != RtfRunKind.Ignore)
			{
				if (runKind != RtfRunKind.Begin)
				{
					if (runKind != RtfRunKind.Keyword)
					{
						this.Restart(RtfEncapsulation.None);
					}
					else
					{
						if (this.countTokens++ > 10)
						{
							this.Restart(RtfEncapsulation.None);
							return;
						}
						if (this.parser.KeywordId == 292)
						{
							if (this.parser.KeywordValue >= 1)
							{
								this.Restart(RtfEncapsulation.Html);
								return;
							}
							this.Restart(RtfEncapsulation.None);
							return;
						}
						else if (this.parser.KeywordId == 329)
						{
							this.Restart(RtfEncapsulation.Text);
							return;
						}
					}
				}
				else if (this.countTokens++ != 0)
				{
					this.Restart(RtfEncapsulation.None);
					return;
				}
			}
		}

		private void Restart(RtfEncapsulation encapsulation)
		{
			this.parser.Restart();
			this.consumerOrProducer = this.rtfToHtml.CreateChain(encapsulation, this.parser, this.output, this.progressMonitor);
		}

		private IProducerConsumer consumerOrProducer;

		private RtfParser parser;

		private ConverterOutput output;

		private RtfToHtml rtfToHtml;

		private int countTokens;

		private IProgressMonitor progressMonitor;
	}
}
