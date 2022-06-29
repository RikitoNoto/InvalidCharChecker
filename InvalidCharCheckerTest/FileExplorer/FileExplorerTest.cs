using InvalidCharChecker.FileExplorer;

namespace InvalidCharCheckerTest
{
    [TestClass]
    public class FileExplorerTest
    {
        private int m_callback_count = 0; // �R�[���o�b�N�֐��̌Ăяo���ꂽ��
        private const string WORKING_DIRECTORY_PATH = "temp";
        private static string working_dir = FileExplorerTest.getRootWorkingDirName();
        private static bool is_initialize = FileExplorerTest.initializeWorkingDir();

        private static string getRootWorkingDirName([System.Runtime.CompilerServices.CallerFilePath]string path="")
        {
            string? dir = Path.GetDirectoryName(path);
            if(dir == null)
            {
                dir = "C:";
            }
            return dir;
        }

        private static bool initializeWorkingDir()
        {
            bool is_complete = false;
            try
            {
                Directory.SetCurrentDirectory(FileExplorerTest.working_dir);
                Directory.Delete(WORKING_DIRECTORY_PATH, true);
                Directory.CreateDirectory(WORKING_DIRECTORY_PATH);
                is_complete = true;
            }
            catch
            {

            }
            return is_complete;
        }

        private static string getWorkingDirName([System.Runtime.CompilerServices.CallerMemberName] string path = "")
        {
            return path;
        }

        [TestInitialize]
        public void setUp()
        {
            Directory.SetCurrentDirectory(Path.Combine(FileExplorerTest.working_dir, WORKING_DIRECTORY_PATH));
            this.m_callback_count = 0;
        }


        [TestCleanup]
        public void tearDown()
        {
            //FIXME: �e�X�g���Ƃɍ�ƃf�B���N�g���̍쐬�ƍ폜���s���ƁA��O����������B
            //       File.Create������ƃv���Z�X�������ł���悤�ŁA�e�X�g����v���Z�X���f�B���N�g���ɃA�N�Z�X�������邽��
            //       �f�B���N�g���̍폜���ł��Ȃ��B
            //Directory.Delete(WORKING_DIRECTORY_PATH,true);
        }

        private bool callbackSpy(string path)
        {
            this.m_callback_count++;
            return true;
        }


        [TestMethod, TestCategory("Explore")]
        public void ��̕�������󂯎�������ɃR�[���o�b�N�֐����Ăяo���Ȃ�����()
        {
            FileExplorer_c.Explore("", this.callbackSpy);
            Assert.AreEqual(0, this.m_callback_count);
        }


        [TestMethod, TestCategory("Explore")]
        public void ��̃p�X���󂯎�������ɃR�[���o�b�N�֐����Ăяo���Ȃ�����()
        {
            Directory.CreateDirectory(getWorkingDirName());
            FileExplorer_c.Explore(getWorkingDirName(), this.callbackSpy);
            Assert.AreEqual(0, this.m_callback_count);
        }


        [TestMethod, TestCategory("Explore")]
        public void �t�@�C��������݂���p�X���󂯎�������ɃR�[���o�b�N�֐���1��Ăяo������()
        {
            Directory.CreateDirectory(getWorkingDirName());
            File.Create(Path.Combine(getWorkingDirName(), "test1"));
            FileExplorer_c.Explore(getWorkingDirName(), this.callbackSpy);
            Assert.AreEqual(1, this.m_callback_count);
        }


        [TestMethod, TestCategory("Explore")]
        public void �t�@�C��������݂���p�X���󂯎�������ɃR�[���o�b�N�֐���2��Ăяo������()
        {
            Directory.CreateDirectory(getWorkingDirName());
            File.Create(Path.Combine(getWorkingDirName(), "test1"));
            File.Create(Path.Combine(getWorkingDirName(), "test2"));
            FileExplorer_c.Explore(getWorkingDirName(), this.callbackSpy);
            Assert.AreEqual(2, this.m_callback_count);
        }

    }
}