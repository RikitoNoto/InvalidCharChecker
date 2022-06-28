using InvalidCharChecker.FileExplorer;

namespace InvalidCharCheckerTest
{
    [TestClass]
    public class FileExplorerTest
    {
        private int m_callback_count = 0; // �R�[���o�b�N�֐��̌Ăяo���ꂽ��

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
        public void ��̈������󂯎�������ɃR�[���o�b�N�֐����Ăяo���Ȃ�����()
        {
            FileExplorer_c.Explore("", this.callbackSpy);
            Assert.AreEqual(0, this.m_callback_count);
        }

    }
}