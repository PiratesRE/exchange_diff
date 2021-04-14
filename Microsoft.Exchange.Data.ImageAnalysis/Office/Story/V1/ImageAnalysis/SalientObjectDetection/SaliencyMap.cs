using System;
using System.Collections.Generic;
using Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner;

namespace Microsoft.Office.Story.V1.ImageAnalysis.SalientObjectDetection
{
	internal class SaliencyMap
	{
		public SaliencyMap(ArgbImage image, int patchSize, float scale) : this(new LabTiledImage(image, patchSize, scale))
		{
		}

		public SaliencyMap(LabTiledImage tiledImage)
		{
			if (tiledImage == null)
			{
				throw new ArgumentNullException("tiledImage");
			}
			this.TiledImage = tiledImage;
			this.Initialize();
			this.BuildNeighboringGraph();
			this.BoundaryAnalysis();
			this.InitScores();
			this.UpdateScores();
			this.NormalizeScores();
			this.CreateSaliencyMap();
		}

		public LabTiledImage TiledImage { get; private set; }

		private void Initialize()
		{
			this.patchNumberX = this.TiledImage.WidthInTiles;
			this.patchNumberY = this.TiledImage.HeightInTiles;
			this.patchesCount = this.TiledImage.AreaInTiles;
			int num = (this.patchNumberX + this.patchNumberY) * 2 - 4;
			this.scores = new float[this.patchesCount];
			this.neighborPatchIds = new int[this.patchesCount, 4];
			this.neighborPatchDists = new float[this.patchesCount, 4];
			this.boundaryIds = new int[num];
			this.boundaryBgScores = new float[num];
		}

		private void BuildNeighboringGraph()
		{
			for (int i = 0; i < this.patchesCount; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					this.neighborPatchIds[i, j] = -1;
					this.neighborPatchDists[i, j] = float.MaxValue;
				}
			}
			int num = 0;
			for (int k = 0; k < this.patchNumberY; k++)
			{
				for (int l = 0; l < this.patchNumberX; l++)
				{
					if (l + 1 < this.patchNumberX)
					{
						int num2 = num + 1;
						float num3 = this.TiledImage[l, k].ComputeLabDistance(this.TiledImage[l + 1, k]);
						this.neighborPatchIds[num, 2] = num2;
						this.neighborPatchIds[num2, 1] = num;
						this.neighborPatchDists[num, 2] = num3;
						this.neighborPatchDists[num2, 1] = num3;
					}
					if (k + 1 < this.patchNumberY)
					{
						int num4 = num + this.patchNumberX;
						float num5 = this.TiledImage[l, k].ComputeLabDistance(this.TiledImage[l, k + 1]);
						this.neighborPatchIds[num, 3] = num4;
						this.neighborPatchIds[num4, 0] = num;
						this.neighborPatchDists[num, 3] = num5;
						this.neighborPatchDists[num4, 0] = num5;
					}
					num++;
				}
			}
			float num6 = 0f;
			int num7 = 0;
			for (int m = 0; m < this.patchesCount; m++)
			{
				float num8 = float.MaxValue;
				for (int n = 0; n < 4; n++)
				{
					num8 = Math.Min(num8, this.neighborPatchDists[m, n]);
				}
				if (num8 < 3.40282347E+38f)
				{
					num6 += num8;
					num7++;
				}
			}
			this.neighborDistThre = num6 / (float)num7;
			for (int num9 = 0; num9 < this.patchesCount; num9++)
			{
				for (int num10 = 0; num10 < 4; num10++)
				{
					if (this.neighborPatchDists[num9, num10] < 3.40282347E+38f)
					{
						this.neighborPatchDists[num9, num10] = Math.Max(0f, this.neighborPatchDists[num9, num10] - this.neighborDistThre);
					}
				}
			}
		}

		private void BoundaryAnalysis()
		{
			int num = 0;
			for (int i = 0; i < this.patchNumberX; i++)
			{
				this.boundaryIds[num] = i;
				num++;
			}
			for (int j = 1; j < this.patchNumberY; j++)
			{
				this.boundaryIds[num] = j * this.patchNumberX - 1;
				num++;
			}
			for (int k = this.patchNumberX - 2; k >= 0; k--)
			{
				this.boundaryIds[num] = (this.patchNumberY - 1) * this.patchNumberX + k;
				num++;
			}
			for (int l = this.patchNumberY - 2; l >= 1; l--)
			{
				this.boundaryIds[num] = l * this.patchNumberX;
				num++;
			}
			float num2 = 1f / (float)Math.Min(this.patchNumberX, this.patchNumberY);
			this.boundaryPatchNumber = (this.patchNumberX + this.patchNumberY) * 2 - 4;
			float[] array = new float[this.boundaryPatchNumber];
			float num3 = float.MaxValue;
			float num4 = 0f;
			for (int m = 0; m < this.boundaryPatchNumber; m++)
			{
				int tile = this.boundaryIds[m];
				for (int n = 0; n < this.boundaryPatchNumber; n++)
				{
					if (n == m)
					{
						array[n] = float.MaxValue;
					}
					else
					{
						int tile2 = this.boundaryIds[n];
						float num5 = this.TiledImage[tile].ComputeLabDistance(this.TiledImage[tile2]);
						int num6 = Math.Abs(m - n);
						num6 = Math.Min(num6, this.boundaryPatchNumber - num6);
						array[n] = num5 / (1f + 3f * (float)num6 * num2);
					}
				}
				Array.Sort<float>(array, Comparer<float>.Default);
				float num7 = 0f;
				for (int num8 = 0; num8 < 10; num8++)
				{
					num7 += array[num8];
				}
				num7 /= 10f;
				num4 = Math.Max(num4, num7);
				num3 = Math.Min(num3, num7);
				this.boundaryBgScores[m] = num7;
			}
			if (num4 - num3 > 1f)
			{
				float num9 = 1f / (num4 - num3);
				for (int num10 = 0; num10 < this.boundaryPatchNumber; num10++)
				{
					float num11 = (this.boundaryBgScores[num10] - num3) * num9;
					if (num11 >= 0.8f)
					{
						this.boundaryBgScores[num10] = float.MaxValue;
					}
					else if ((double)num11 < 0.5)
					{
						this.boundaryBgScores[num10] = 0f;
					}
				}
				return;
			}
			for (int num12 = 0; num12 < this.boundaryBgScores.Length; num12++)
			{
				this.boundaryBgScores[num12] = 0f;
			}
		}

		private void InitScores()
		{
			for (int i = 0; i < this.patchesCount; i++)
			{
				this.scores[i] = float.MaxValue;
			}
			for (int j = 0; j < this.boundaryPatchNumber; j++)
			{
				int num = this.boundaryIds[j];
				this.scores[num] = this.boundaryBgScores[j];
			}
		}

		private void UpdateScores()
		{
			bool[] array = new bool[this.patchesCount];
			Queue<int> queue = new Queue<int>();
			for (int i = 0; i < this.patchesCount; i++)
			{
				array[i] = false;
				if (this.scores[i] < 3.40282347E+38f)
				{
					queue.Enqueue(i);
					array[i] = true;
				}
			}
			while (queue.Count > 0)
			{
				int num = queue.Dequeue();
				array[num] = false;
				for (int j = 0; j < 4; j++)
				{
					int num2 = this.neighborPatchIds[num, j];
					if (num2 >= 0)
					{
						float num3 = this.scores[num] + this.neighborPatchDists[num, j];
						if (this.scores[num2] > num3)
						{
							this.scores[num2] = num3;
							if (!array[num2])
							{
								queue.Enqueue(num2);
								array[num2] = true;
							}
						}
					}
				}
			}
		}

		private void NormalizeScores()
		{
			float[] array = new float[this.patchesCount];
			this.scores.CopyTo(array, 0);
			int num = (int)((float)this.patchesCount * 0.02f);
			Array.Sort<float>(array, new ReverseComparer<float>(null));
			float num2 = array[num - 1];
			for (int i = 0; i < this.patchesCount; i++)
			{
				this.scores[i] = Math.Min(num2, this.scores[i]) / num2;
			}
		}

		private void CreateSaliencyMap()
		{
			for (int i = 0; i < this.scores.Length; i++)
			{
				this.TiledImage[i].Saliency = this.scores[i];
			}
		}

		private const float OutOfRange = 3.40282347E+38f;

		private const int Neighborsize = 4;

		private int patchNumberX;

		private int patchNumberY;

		private int patchesCount;

		private int boundaryPatchNumber;

		private float[] scores;

		private int[,] neighborPatchIds;

		private float[,] neighborPatchDists;

		private float neighborDistThre;

		private int[] boundaryIds;

		private float[] boundaryBgScores;
	}
}
