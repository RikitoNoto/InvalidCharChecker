using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace InvalidCharChecker.FileExplorer
{
    public class FileExplorer_c
    {

        public static int Explore(string path, Func<string, bool> callback_func, string file_pattern="", string except_dir_pattern="")
        {
            if(path == "")
            {
                return 0;
            }
            Regex file_regex = new Regex(file_pattern);
            Regex except_dir_regex = new Regex(except_dir_pattern);
            string[] files = Directory.GetFiles(path);


            foreach(string file_path in files)
            {
                if(file_regex.IsMatch(Path.GetFileName(file_path)))
                {
                    callback_func(file_path);
                }
            }

            foreach (string dir_path in Directory.GetDirectories(path))
            {
                if(except_dir_pattern=="" || !except_dir_regex.IsMatch(Path.GetFileName(dir_path)))
                {
                    FileExplorer_c.Explore(dir_path, callback_func, file_pattern, except_dir_pattern);
                }
            }

            return 0;
        }

    }
}
