// See https://aka.ms/new-console-template for more information
using InvalidCharChecker.InvalidCharChecker;


string[]? invalid_files = null;
switch(args.Length)
{
    case 1:
        invalid_files = InvalidCharChecker_c.searchInvalidChar(args[0]);
        break;
    case 2:
        invalid_files = InvalidCharChecker_c.searchInvalidChar(args[0], args[1]);
        break;
    case 3:
        invalid_files = InvalidCharChecker_c.searchInvalidChar(args[0], args[1], args[2]);
        break;

}

if(invalid_files == null)
{
    return;
}

if(invalid_files.Length == 0)
{
    Console.WriteLine("不正文字は使用されていません。");
}
else
{
    Console.WriteLine("不正文字が使用されています。");

    foreach(string file_path in invalid_files)
    {
        Console.WriteLine(file_path);
    }
}