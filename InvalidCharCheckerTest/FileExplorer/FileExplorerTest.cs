using InvalidCharChecker.FileExplorer;

namespace InvalidCharCheckerTest
{
    [TestClass]
    public class FileExplorerTest
    {
        private int m_callback_count = 0;                                   // コールバック関数の呼び出された回数
        private List<string> m_callback_file_names = new List<string>();    // コールバック関数で呼び出されるファイル名リスト

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

            // ディレクトリの作成
            Directory.CreateDirectory(tree.m_name);
            Directory.SetCurrentDirectory(tree.m_name);

            // ファイルの作成
            foreach(string file in tree.m_files)
            {
                File.Create(file);
            }

            // サブディレクトリの作成
            foreach(DIRECTORY_TREE child_tree in tree.m_dirs)
            {
                this.makeTree(child_tree);
            }

            Directory.SetCurrentDirectory(backup_dir);
        }

        private void checkExploreAndMakeTree(DIRECTORY_TREE tree, int expect_callback_count, string file_pattern = "", string except_dir_pattern = "")
        {
            checkExploreAndMakeTree(tree, expect_callback_count, new string[0], file_pattern, except_dir_pattern);
        }

        private void checkExploreAndMakeTree(DIRECTORY_TREE tree, int expect_callback_count, string[] file_names, string file_pattern = "", string except_dir_pattern="")
        {
            this.makeTree(tree);
            this.m_callback_file_names.AddRange(file_names);
            FileExplorer_c.Explore(tree.m_name, this.callbackSpy, file_pattern, except_dir_pattern);
            Assert.AreEqual(expect_callback_count, this.m_callback_count);
            Assert.AreEqual(0, this.m_callback_file_names.Count);
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
            //FIXME: テストごとに作業ディレクトリの作成と削除を行うと、例外が発生する。
            //       File.Createをするとプロセスが複数できるようで、テスト後もプロセスがディレクトリにアクセスし続けるため
            //       ディレクトリの削除ができない。
            //Directory.Delete(WORKING_DIRECTORY_PATH,true);
        }

        private bool callbackSpy(string path)
        {
            this.m_callback_count++;
            this.m_callback_file_names.Remove(path);
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
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName()), 0);
        }


        [TestMethod, TestCategory("Explore")]
        public void ファイルが一つ存在するパスを受け取った時にコールバック関数を1回呼び出すこと()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new string[] { "test1"}), 1);
        }

        [TestMethod, TestCategory("Explore")]
        public void コールバック関数の引数がパス名になっていること()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new string[] { "test1" }), 1, new string[] { Path.Combine(getWorkingDirName(), "test1") });
        }

        [TestMethod, TestCategory("Explore")]
        public void サブディレクトリからのコールバック関数の引数がパス名になっていること()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {
                                                new DIRECTORY_TREE("child", new string[]{"test1"})
                                            }), 1, new string[] { Path.Combine(getWorkingDirName(), "child", "test1") });
        }


        [TestMethod, TestCategory("Explore")]
        public void ファイルが二つ存在するパスを受け取った時にコールバック関数を2回呼び出すこと()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new string[] { "test1", "test2" }), 2);
        }


        [TestMethod, TestCategory("Explore")]
        public void 再帰的にディレクトリの検索を行っていること_1階層1ファイル()
        {
            this.checkExploreAndMakeTree(   new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {                // root
                                                new DIRECTORY_TREE("child1", new string[]{"test1"})                     // child1
                                            }), 1);
        }


        [TestMethod, TestCategory("Explore")]
        public void 再帰的にディレクトリの検索を行っていること_2階層1ファイルずつ()
        {
            this.checkExploreAndMakeTree(   new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {                  // root
                                                new DIRECTORY_TREE("child1", new string[]{"test1"}, new DIRECTORY_TREE[] {  // child1
                                                    new DIRECTORY_TREE("grandchild1", new string[]{"test2"})                // grand child1
                                                })
                                            }), 2);
        }

        [TestMethod, TestCategory("Explore")]
        public void ファイルパターンにヒットしないときにコールバック関数が呼び出されないこと()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {                  // root
                                                new DIRECTORY_TREE("child1", new string[]{"test1"}, new DIRECTORY_TREE[] {  // child1
                                                    new DIRECTORY_TREE("grandchild1", new string[]{"test2"})                // grand child1
                                                })
                                            }), 0, ".*\\.cpp");
        }

        [TestMethod, TestCategory("Explore")]
        public void ディレクトリパターンにヒットしてもファイルパターンにヒットしないときにコールバック関数が呼び出されないこと()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {                  // root
                                                new DIRECTORY_TREE("child1", new string[]{"test1"}, new DIRECTORY_TREE[] {  // child1
                                                    new DIRECTORY_TREE("grandchild1", new string[]{"test2"})                // grand child1
                                                })
                                            }), 0, "child");
        }

        [TestMethod, TestCategory("Explore")]
        public void ファイルパターンを渡して検索ができること()
        {
            this.checkExploreAndMakeTree(new DIRECTORY_TREE(getWorkingDirName(), new DIRECTORY_TREE[] {                  // root
                                                new DIRECTORY_TREE("child1", new string[]{"test1.cpp"}, new DIRECTORY_TREE[] {  // child1
                                                    new DIRECTORY_TREE("grandchild1", new string[]{"test2.c"})                // grand child1
                                                })
                                            }), 1, ".*\\.cpp");
        }

        [TestMethod, TestCategory("Explore")]
        public void 対象外ディレクトリパターンにヒットするときにコールバック関数が呼び出されないこと()
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
        public void 対象外ディレクトリパターンにヒットしないときにコールバック関数が呼び出されること()
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
    }
}