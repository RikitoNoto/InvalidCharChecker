using InvalidCharChecker.FileExplorer;

namespace InvalidCharCheckerTest
{
    [TestClass]
    public class FileExplorerTest
    {
        private int m_callback_count = 0; // コールバック関数の呼び出された回数

        [TestInitialize]
        public void setUp()
        {
            this.m_callback_count = 0;
        }


        [TestCleanup]
        public void tearDown()
        {

        }

        private bool callbackSpy(string path)
        {
            this.m_callback_count++;
            return true;
        }


        [TestMethod, TestCategory("Explore")]
        public void 空の引数を受け取った時にコールバック関数を呼び出さないこと()
        {
            FileExplorer_c.Explore("", this.callbackSpy);
            Assert.AreEqual(0, this.m_callback_count);
        }

    }
}