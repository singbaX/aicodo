using System;
using System.Collections.Generic;
using System.Text;

namespace AiCodo
{
    public static class FormatString
    {
        public static string BindData(this string Format,Dictionary<string, object> data)
        {
            var sb = new StringBuilder();
            var index = 0;
            var endIndex = -1;
            var chr = ' ';
            for (int i = 0; i < Format.Length; i++)
            {
                chr = Format[i];
                if (chr == '{')
                {
                    if (i < Format.Length - 1)
                    {
                        if (Format[i + 1] == '{')
                        {
                            sb.Append('{');
                            i++;
                            continue;
                        }
                        index = i;
                        endIndex = Format.IndexOf('}', index + 1);
                        if (endIndex > index)
                        {
                            var name = Format.Substring(index + 1, endIndex - index - 1).Trim();
                            if (data.TryGetValue(name, out object v))
                            {
                                sb.Append($"{v}");
                            }
                            i = endIndex;
                        }
                        else
                        {
                            sb.Append(Format.Substring(index));
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    sb.Append(chr);
                }
            }

            return sb.ToString();
        }
    }

}
