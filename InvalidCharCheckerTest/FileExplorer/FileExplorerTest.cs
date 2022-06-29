using InvalidCharChecker.FileExplorer;

namespace InvalidCharCheckerTest
{
    [TestClass]
    public class FileExplorerTest
    {
        private int m_callback_count = 0; // コールバック関数の呼び出された回数
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
            //FIXME: テストごとに作業ディレクトリの作成と削除を行うと、例外が発生する。
            //       File.Createをするとプロセスが複数できるようで、テスト後もプロセスがディレクトリにアクセスし続けるため
            //       ディレクトリの削除ができない。
            //Directory.Delete(WORKING_DIRECTORY_PATH,true);
        }

        private bool callbackSpy(string path)
        {
            this.m_callback_count++;
            return true;
        }


        [TestMethod, TestCategory("Explore")]
        public void 空の文字列を受け取った時にコールバック関数を呼び出さないこと()
        {
            FileExplorer_c.Explore("", this.callbackSpy);
            Assert.AreEqual(0, this.m_callback_count);
        }


        [TestMethod, TestCategory("Explore")]
        public void 空のパスを受け取った時にコールバック関数を呼び出さないこと()
        {
            Directory.CreateDirectory(getWorkingDirName());
            FileExplorer_c.Explore(getWorkingDirName(), this.callbackSpy);
            Assert.AreEqual(0, this.m_callback_count);
        }


        [TestMethod, TestCategory("Explore")]
        public void ファイルが一つ存在するパスを受け取った時にコールバック関数を1回呼び出すこと()
        {
            Directory.CreateDirectory(getWorkingDirName());
            File.Create(Path.Combine(getWorkingDirName(), "test1"));
            FileExplorer_c.Explore(getWorkingDirName(), this.callbackSpy);
            Assert.AreEqual(1, this.m_callback_count);
        }


        [TestMethod, TestCategory("Explore")]
        public void ファイルが二つ存在するパスを受け取った時にコールバック関数を2回呼び出すこと()
        {
            Directory.CreateDirectory(getWorkingDirName());
            File.Create(Path.Combine(getWorkingDirName(), "test1"));
            File.Create(Path.Combine(getWorkingDirName(), "test2"));
            FileExplorer_c.Explore(getWorkingDirName(), this.callbackSpy);
            Assert.AreEqual(2, this.m_callback_count);
        }

    }
}