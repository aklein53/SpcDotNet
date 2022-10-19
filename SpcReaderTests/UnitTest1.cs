namespace SpcReaderTests
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
			using (var stream = File.OpenRead("Test Content/1.SPC"))
			{
				SPC spc = new(stream);
			}
		}
	}
}