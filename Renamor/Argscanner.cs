using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renamor
{
    public class ArgScanner
    {
        Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

        public ArgScanner(string [] args)
        {
            string last;
            List<string> lastList;

            void NextVal(string s) 
            {

                last = s.ToUpper();
                lastList = new List<string>();

                values[last] = lastList;
            };

            NextVal("");


            foreach (string s in args)
            {
                if (s.StartsWith("/"))
                {
                    NextVal(s.Substring(1));
                }
                else
                {
                    lastList.Add(s);
                }
            }


        }

        public bool HasOption(string option)
        {
            String check = option.ToUpper();
            return values.ContainsKey(check);

        }

        public List<string> GetParameters(string option)
        {
            List<string> parameters = null;

            String check = option.ToUpper();

            values.TryGetValue(check, out parameters);

            return parameters;

        }

        public List<string> this [string option]
        {
            get
            {
                return GetParameters(option);
            }
        }

        public string FirstOrDefault(String option, String def)
        {
            String check = option.ToUpper();

            if (HasOption(check) &&  GetParameters(check).Count() > 0)
            {
                return GetParameters(check)[0];
            }
            return def;
        }
    }
}
