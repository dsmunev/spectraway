using System.IO;
using System.Linq;
using System.Text;

namespace SpectraWay.Extension
{
    public static class StringExt
    {
        static char[] _invalids;
        public static string ToFileName(this string fileNameCandidate, char? replacement = '_', bool fancy = true)
        {
            StringBuilder sb = new StringBuilder(fileNameCandidate.Length);
            var invalids = _invalids ?? (_invalids = Path.GetInvalidFileNameChars());
            bool changed = false;
            for (int i = 0; i < fileNameCandidate.Length; i++)
            {
                char c = fileNameCandidate[i];
                if (invalids.Contains(c))
                {
                    changed = true;
                    var repl = replacement ?? '\0';
                    if (fancy)
                    {
                        if (c == '"') repl = '”'; // U+201D right double quotation mark
                        else if (c == '\'') repl = '’'; // U+2019 right single quotation mark
                        else if (c == '/') repl = '⁄'; // U+2044 fraction slash
                    }
                    if (repl != '\0')
                        sb.Append(repl);
                }
                else
                    sb.Append(c);
            }
            if (sb.Length == 0)
                return "_";
            return changed ? sb.ToString() : fileNameCandidate;
        }
    }
}