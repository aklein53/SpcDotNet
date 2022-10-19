using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpcReader
{
	public class SPC
	{
        public SPCHeader Header { get; set; } = new SPCHeader();
		public float[] XData { get; set; } = new float[0];

		public SPC(Stream input)
		{
			byte[] content;

			using (var memoryStream = new MemoryStream())
			{
				input.CopyTo(memoryStream);
				content = memoryStream.ToArray();
			}

			this.Header = SPCHeader.ReadHeader(content[0..512]);
			this.XData = ReadXData(content);
		}

		private float[] ReadXData(byte[] content)
		{
			if ((this.Header.Ftflag & FtflgValues.TXYXYS) != FtflgValues.TXYXYS)
			{
				if ((this.Header.Ftflag & FtflgValues.TXVALS) == FtflgValues.TXVALS)
				{
					float[] returnValue = new float[this.Header.Fnpts];

					for (int i = 0; i < this.Header.Fnpts; i++)
					{
						
						returnValue[i] = BitConverter.ToSingle(content[512..], i * sizeof(float));
					}

					return returnValue;
				}
				else
				{
					return Linspace(this.Header.Ffirst, this.Header.Flast, this.Header.Fnpts);
				}
			}
			else
			{
				return new float[0];
			}
		}

		public static float[] Linspace(double startval, double endval, int points)
		{
			double interval = (endval - startval) / (points - 1);
			return (from val in Enumerable.Range(0, points)
					select (float)(startval + (val * interval))).ToArray();
		}
	}
}
