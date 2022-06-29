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
        private struct DIRECTORY_TREE
        {
            public DIRECTORY_TREE(string name) : this(name, new string[0], new DIRECTORY_TREE[0])
            {
            }

            public DIRECTORY_TREE(string name, string[] files) : this(name, files, new DIRECTORY_TREE[0])
            {                
            }

            public DIRECTORY_TREE(string name, DIRECTORY_TREE[] dirs) : this(name, new string[0], dirs)
            {
            }

            public DIRECTORY_TREE(string name, string[] files, DIRECTORY_TREE[] dirs)
            {
                this.m_name = name;
                this.m_files = files;
                this.m_dirs = dirs;
            }
            public string m_name;
            public string[] m_files;
            public DIRECTORY_TREE[] m_dirs;
        }

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
                if(Directory.Exists(WORKING_DIRECTORY_PATH))
                {
                    Directory.Delete(WORKING_DIRECTORY_PATH, true);
                }
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

        private void makeTree(DIRECTORY_TREE tree)
        {
            string backup_dir = Directory.GetCurrentDirectory();

            // �f�B���N�g���̍쐬
            Directory.CreateDirectory(tree.m_name);
            Directory.SetCurrentDirectory(tree.m_name);

            // �t�@�C���̍쐬
            foreach(string file in tree.m_files)
            {
                File.Create(file);
            }

            // �T�u�f�B���N�g���̍쐬
            foreach(DIRECTORY_TREE child_tree in tree.m_dirs)
            {
                this.makeTree(child_tree);
            }

            Directory.SetCurrentDirectory(backup_dir);
        }

        private void checkExploreAndMakeTree(DIRECTORY_TREE tree, int expect_callback_count)
        {
            this.makeTree(tree);
            FileExplorer_c.Explore(tree.m_name, this.callbackSpy);
            Assert.AreEqual(expect_callback_count, this.m_callback_count);
        }

        [TestInitialize]
        public void setUp()
        {
            if((!is_initialize) && (!initializeWorkingDir()))
            {
                Assert.Fail();
            }
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
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName()), 0);
        }


        [TestMethod, TestCategory("Explore")]
        public void �t�@�C��������݂���p�X���󂯎�������ɃR�[���o�b�N�֐���1��Ăяo������()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new string[] { "test1"}), 1);
        }


        [TestMethod, TestCategory("Explore")]
        public void �t�@�C��������݂���p�X���󂯎�������ɃR�[���o�b�N�֐���2��Ăяo������()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new string[] { "test1", "test2" }), 2);
        }


        [TestMethod, TestCategory("Explore")]
        public void �ċA�I�Ƀf�B���N�g���̌������s���Ă��邱��_1�K�w1�t�@�C��()
        {
            this.checkExploreAndMakeTree(   new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {                // root
                                                new DIRECTORY_TREE("child1", new string[]{"test1"})                     // child1
                                            }), 1);
        }


        [TestMethod, TestCategory("Explore")]
        public void �ċA�I�Ƀf�B���N�g���̌������s���Ă��邱��_2�K�w1�t�@�C������()
        {
            this.checkExploreAndMakeTree(   new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {                  // root
                                                new DIRECTORY_TREE("child1", new string[]{"test1"}, new DIRECTORY_TREE[] {  // child1
                                                    new DIRECTORY_TREE("grandchild1", new string[]{"test2"})                // grand child1
                                                })
                                            }), 2);
        }

    }
}