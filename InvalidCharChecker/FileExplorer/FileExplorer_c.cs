using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace InvalidCharChecker.FileExplorer
{
    public class FileExplorer_c
    {

        /// <summary>
        ///     指定されたディレクトリを検索する。
        ///     ファイル内の内容を正規表現で検索し、マッチしたらコールバック関数を呼び出す
        /// </summary>
        /// <param name="path">検索するディレクトリのパス</param>
        /// <param name="callback_func">コールバック関数</param>
        /// <param name="content_pattern">ファイル内容の正規表現パターン</param>
        /// <param name="file_pattern">検索するファイルの正規表現パターン</param>
        /// <param name="except_dir_pattern">検索から除外するディレクトリの正規表現パターン</param>
        /// <returns>検索したファイル数(未実装)</returns>
        public static int Explore(string path, Func<string, bool> callback_func, string content_pattern, string file_pattern="", string except_dir_pattern="")
        {
            Regex content_regex = new Regex(content_pattern);   // ファイル内容の正規表現パターンを作成
            // exploreメソッドを呼び出し
            return FileExplorer_c.Explore(path, delegate (string _path) {
                    // コールバック関数をラップし、正規表現にマッチする場合のみ元のコールバック関数を呼び出し
                    if (content_regex.IsMatch(File.ReadAllText(_path)))
                    {
                        return callback_func(_path);
                    }
                    return true;
                }, file_pattern: file_pattern, except_dir_pattern: except_dir_pattern);
        }

        /// <summary>
        ///     指定されたディレクトリを検索する。
        ///     ファイルにヒットしたらコールバック関数を呼び出す
        /// </summary>
        /// <param name="path">検索するディレクトリのパス</param>
        /// <param name="content_pattern">ファイル内容の正規表現パターン</param>
        /// <param name="file_pattern">検索するファイルの正規表現パターン</param>
        /// <param name="except_dir_pattern">検索から除外するディレクトリの正規表現パターン</param>
        /// <returns>検索したファイル数(未実装)</returns>
        public static int Explore(string path, Func<string, bool> callback_func, string file_pattern = "", string except_dir_pattern = "")
        {
            // path が空なら処理終了
            if (path == "")
            {
                return 0;
            }

            Regex file_regex = new Regex(file_pattern);             // ファイル検索の正規表現パターン
            Regex except_dir_regex = new Regex(except_dir_pattern); // 検索から除外するディレクトリの正規表現パターン

            // 現在のディレクトリのファイル検索
            foreach (string file_path in Directory.GetFiles(path))
            {
                // ファイル名がファイル名の正規表現パターンに一致している場合
                if (file_regex.IsMatch(Path.GetFileName(file_path)))
                {
                    callback_func(file_path);   // ファイルパスを引数にコールバック関数を呼び出し。
                }
            }

            // 現在のディレクトリのサブディレクトリを検索
            foreach (string dir_path in Directory.GetDirectories(path))
            {
                // 正規表現パターンが未設定 もしくは 除外する正規表現パターンにマッチしない場合
                if (except_dir_pattern == "" || !except_dir_regex.IsMatch(Path.GetFileName(dir_path)))
                {
                    FileExplorer_c.Explore(dir_path, callback_func, file_pattern, except_dir_pattern);  // サブディレクトリのパスを引数に再帰呼び出し
                }
            }

            return 0;   //TODO: 検索にマッチしたファイル数を返す。
        }

    }
}
