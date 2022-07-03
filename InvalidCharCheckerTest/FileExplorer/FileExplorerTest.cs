using InvalidCharChecker.FileExplorer;

namespace InvalidCharCheckerTest
{
    [TestClass]
    public class FileExplorerTest
    {
        private int m_callback_count = 0;                                   // �R�[���o�b�N�֐��̌Ăяo���ꂽ��
        private List<string> m_callback_file_names = new List<string>();    // �R�[���o�b�N�֐��ŌĂяo�����t�@�C�������X�g

        private const string WORKING_DIRECTORY_PATH = "temp";
        private static string working_dir = FileExplorerTest.getRootWorkingDirName();
        private static bool is_initialize = FileExplorerTest.initializeWorkingDir();

        /// <summary>
        ///     �f�B���N�g���\����\���\����
        /// </summary>
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

            public DIRECTORY_TREE(string name, string[] files, DIRECTORY_TREE[] dirs):this(name, files.ToDictionary<string, string>(name=>name), dirs)
            {
            }

            public DIRECTORY_TREE(string name, Dictionary<string, string> files) : this(name, files, new DIRECTORY_TREE[0])
            {
            }

            public DIRECTORY_TREE(string name, Dictionary<string, string> files, DIRECTORY_TREE[] dirs)
            {
                this.m_name = name;
                this.m_files = files;
                this.m_dirs = dirs;
            }

            public string m_name;                       // �f�B���N�g����
            public Dictionary<string, string> m_files;  // �f�B���N�g�����̃t�@�C��(key: �t�@�C�����Acontent: �t�@�C���̓��e)
            public DIRECTORY_TREE[] m_dirs;             // �T�u�f�B���N�g��
        }


        /// <summary>
        ///     �e�X�g���̍�Ɨp�f�B���N�g�����쐬����f�B���N�g�����擾����B
        /// </summary>
        private static string getRootWorkingDirName([System.Runtime.CompilerServices.CallerFilePath]string path="")
        {
            string? dir = Path.GetDirectoryName(path);
            if(dir == null)
            {
                dir = "C:";
            }
            return dir;
        }

        /// <summary>
        ///     �e�X�g���̍�Ɨp�f�B���N�g��������������B
        ///     �O��e�X�g�ō쐬�������e�̃N���A�����A�f�B���N�g����V�K�쐬����B
        /// </summary>
        /// <returns>�������̊������</returns>
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

        /// <returns>��ƃf�B���N�g���̃p�X</returns>
        private static string getWorkingDirName([System.Runtime.CompilerServices.CallerMemberName] string path = "")
        {
            return path;
        }

        /// <summary>
        ///     �����̃f�B���N�g���c���[�����ƂɁA���ۂ̃t�@�C���V�X�e����Ƀc���[���쐬����B
        /// </summary>
        /// <param name="tree">�쐬����f�B���N�g���c���[</param>
        private void makeTree(DIRECTORY_TREE tree)
        {
            string backup_dir = Directory.GetCurrentDirectory();    // ��Ɨp�Ɍ��݂̃f�B���N�g����ύX���邽�߁A�o�b�N�A�b�v������Ă���

            // �f�B���N�g���̍쐬
            Directory.CreateDirectory(tree.m_name);
            Directory.SetCurrentDirectory(tree.m_name); // �쐬�����f�B���N�g���ֈړ�

            // �t�@�C���̍쐬
            foreach(string file_name in tree.m_files.Keys)
            {
                File.WriteAllText(file_name, tree.m_files[file_name]);
            }

            // �T�u�f�B���N�g���̍쐬
            foreach(DIRECTORY_TREE child_tree in tree.m_dirs)
            {
                this.makeTree(child_tree);
            }

            Directory.SetCurrentDirectory(backup_dir);  // ���݂̃f�B���N�g���𕜋A
        }

        private void checkExploreAndMakeTree(DIRECTORY_TREE tree, int expect_callback_count, string file_pattern = "", string except_dir_pattern = "", string content_pattern="")
        {
            checkExploreAndMakeTree(tree, expect_callback_count, new string[0], file_pattern, except_dir_pattern, content_pattern);
        }

        /// <summary>
        ///     �f�B���N�g���c���[���쐬���A�������ꂽ�t�@�C��������v���Ă��邩���`�F�b�N����B
        /// </summary>
        private void checkExploreAndMakeTree(DIRECTORY_TREE tree, int expect_callback_count, string[] file_names, string file_pattern = "", string except_dir_pattern="", string content_pattern="")
        {
            this.makeTree(tree);    // �c���[�̍쐬
            this.m_callback_file_names.AddRange(file_names);    // �Ăяo�������҂���R�[���o�b�N�֐��̈������X�g���쐬
            if(content_pattern == "")
            {
                FileExplorer_c.Explore(tree.m_name, this.callbackSpy, file_pattern, except_dir_pattern);
            }
            else
            {
                FileExplorer_c.Explore(tree.m_name, this.callbackSpy, content_pattern, file_pattern, except_dir_pattern);
            }
            Assert.AreEqual(expect_callback_count, this.m_callback_count);  // ���҂����Ăяo���񐔂ɂȂ��Ă��邱��
            Assert.AreEqual(0, this.m_callback_file_names.Count);           // �������X�g�����ׂď�������Ă��邱��
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
            this.m_callback_file_names.Clear();
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
            this.m_callback_file_names.Remove(path);
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
        public void �R�[���o�b�N�֐��̈������p�X���ɂȂ��Ă��邱��()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new string[] { "test1" }), 1, new string[] { Path.Combine(getWorkingDirName(), "test1") });
        }

        [TestMethod, TestCategory("Explore")]
        public void �T�u�f�B���N�g������̃R�[���o�b�N�֐��̈������p�X���ɂȂ��Ă��邱��()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {
                                                new DIRECTORY_TREE("child", new string[]{"test1"})
                                            }), 1, new string[] { Path.Combine(getWorkingDirName(), "child", "test1") });
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

        [TestMethod, TestCategory("Explore")]
        public void �t�@�C���p�^�[���Ƀq�b�g���Ȃ��Ƃ��ɃR�[���o�b�N�֐����Ăяo����Ȃ�����()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {                  // root
                                                new DIRECTORY_TREE("child1", new string[]{"test1"}, new DIRECTORY_TREE[] {  // child1
                                                    new DIRECTORY_TREE("grandchild1", new string[]{"test2"})                // grand child1
                                                })
                                            }), 0, ".*\\.cpp");
        }

        [TestMethod, TestCategory("Explore")]
        public void �f�B���N�g���p�^�[���Ƀq�b�g���Ă��t�@�C���p�^�[���Ƀq�b�g���Ȃ��Ƃ��ɃR�[���o�b�N�֐����Ăяo����Ȃ�����()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {                  // root
                                                new DIRECTORY_TREE("child1", new string[]{"test1"}, new DIRECTORY_TREE[] {  // child1
                                                    new DIRECTORY_TREE("grandchild1", new string[]{"test2"})                // grand child1
                                                })
                                            }), 0, "child");
        }

        [TestMethod, TestCategory("Explore")]
        public void �t�@�C���p�^�[����n���Č������ł��邱��()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {                  // root
                                                new DIRECTORY_TREE("child1", new string[]{"test1.cpp"}, new DIRECTORY_TREE[] {  // child1
                                                    new DIRECTORY_TREE("grandchild1", new string[]{"test2.c"})                // grand child1
                                                })
                                            }), 1, ".*\\.cpp");
        }

        [TestMethod, TestCategory("Explore")]
        public void �ΏۊO�f�B���N�g���p�^�[���Ƀq�b�g����Ƃ��ɃR�[���o�b�N�֐����Ăяo����Ȃ�����()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {                     // root
                                                new DIRECTORY_TREE("child1", new string[]{"match"}, new DIRECTORY_TREE[] {  // child1
                                                    new DIRECTORY_TREE("grandchild11", new string[]{"match"}),              // grand child11
                                                    new DIRECTORY_TREE("grandchild12", new string[]{"match"})               // grand child12
                                                }),
                                                new DIRECTORY_TREE("child2", new string[]{"ignore"}, new DIRECTORY_TREE[] { // child2
                                                    new DIRECTORY_TREE("grandchild21", new string[]{"match"}),              // grand child21
                                                    new DIRECTORY_TREE("grandchild22", new string[]{"ignore"})              // grand child22
                                                })
                                            }), 0, except_dir_pattern: "child");
        }

        [TestMethod, TestCategory("Explore")]
        public void �ΏۊO�f�B���N�g���p�^�[���Ƀq�b�g���Ȃ��Ƃ��ɃR�[���o�b�N�֐����Ăяo����邱��()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {                     // root
                                                new DIRECTORY_TREE("child1", new string[]{"ignore"}, new DIRECTORY_TREE[] {  // child1
                                                    new DIRECTORY_TREE("grandchild11", new string[]{"ignore"}),              // grand child11
                                                    new DIRECTORY_TREE("grandchild12", new string[]{"ignore"})               // grand child12
                                                }),
                                                new DIRECTORY_TREE("child2", new string[]{"match"}, new DIRECTORY_TREE[] { // child2
                                                    new DIRECTORY_TREE("grandchild21", new string[]{"ignore"}),              // grand child21
                                                    new DIRECTORY_TREE("grandchild22", new string[]{"match"})              // grand child22
                                                })
                                            }), 2, except_dir_pattern: "child.*1");
        }

        [TestMethod, TestCategory("Explore")]
        public void ���K�\���p�^�[���Ƀq�b�g���Ȃ��ꍇ�J�E���g���Ȃ�����()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new Dictionary<string, string>() { {"file1", "content" } }), 0, content_pattern: "text");
        }

        [TestMethod, TestCategory("Explore")]
        public void ���K�\���p�^�[���Ƀq�b�g����ꍇ�J�E���g���邱��()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new Dictionary<string, string>() { { "file1", "content" } }), 1, content_pattern: "c.n");
        }
    }
}