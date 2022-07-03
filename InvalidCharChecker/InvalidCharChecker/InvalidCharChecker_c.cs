using InvalidCharChecker.FileExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvalidCharChecker.InvalidCharChecker
{
    public class InvalidCharChecker_c
    {
        public const string INVALID_PATTERN = "[表][\n\r]";


        static public string[] searchInvalidChar(string path, string file_pattern="", string except_dir_pattern="")
        {
            List<string> result = new List<string>();
            FileExplorer_c.Explore(path, delegate (string path)
                {
                    result.Add(path);
                    return true;
                }, INVALID_PATTERN, file_pattern, except_dir_pattern);

            return result.ToArray();
        }
    }
}
