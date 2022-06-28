using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvalidCharChecker.FileExplorer
{
    public class FileExplorer_c
    {
        public static int Explore(string path, Func<string, bool> callback_func)
        {
            if(path == "")
            {
                return 0;
            }

            string[] files = Directory.GetFiles(path);

            foreach(string file in files)
            {
                callback_func(file);
            }

            return 0;
        }

    }
}
