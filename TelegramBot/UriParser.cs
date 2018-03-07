using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBot
{
    public class UriHandler
    {
        public string Command { get; set; }
        private List<UriParameter> parameters { get; set; }

        public UriHandler(Command c) {
            Command = c.CommandName;
            parameters = new List<UriParameter>();
        }

        public UriHandler(string input)
        {
            parameters = new List<UriParameter>();
            if (string.IsNullOrEmpty(input))
            {
                return;
            }

            int index = 0;
            var inputArray = input.ToArray();
            while (index != input.Length && inputArray[index] != '?')
            {
                Command += inputArray[index];
                index++;
            }

            //If there are no arguments
            if (index == input.Length)
            {
                return;
            }

            //Skip question sign
            index++;

            while (index != input.Length)
            {
                UriParameter param = new UriParameter();
                while (index != input.Length && inputArray[index] != '=')
                {
                    param.name += inputArray[index];
                    index++;
                }

                if (inputArray[index] != '=' || index + 1 == input.Length)
                {
                    throw new ArgumentException("Invalid url given. Parameters should be seperated. At parameter: " +
                                                param.name);
                }

                //Skip equals sign
                index++;

                if (inputArray[index] == '"')
                {
                    param.escaped = true;
                    index++;
                    while (index != input.Length && inputArray[index] != '"')
                    {
                        param.value += inputArray[index];
                        index++;
                    }

                    if (index == input.Length && inputArray[index] != '"')
                    {
                        throw new ArgumentException("Unexpected end of parameter. At parameter: " +
                                                    param.name + " with value " + param.value);
                    }
                    index++;
                    if (index != input.Length)
                    {
                        if (inputArray[index] != '&')
                        {
                            throw new ArgumentException("Expected empersant, but got " + inputArray[index] + ". At parameter: " +
                                                        param.name + " with value " + param.value);
                        }
                    }

                }
                else
                {
                    while (index != input.Length && inputArray[index] != '&')
                    {
                        param.value += inputArray[index];
                        index++;
                    }
                }

                parameters.Add(param);


                if (index != input.Length && inputArray[index] == '&')
                {
                    //Skip empersant
                    index++;
                }
                

            }
        }

        public bool IsNotSet() {
            return string.IsNullOrEmpty(Command);
        }

        public void Set(string key, string value, bool escapeQuote = false)
        {
            value = escapeQuote ? Regex.Replace(value, @"(?<!\\)'", @"\'").Replace('"', '\'') : value;

            var param = parameters.FirstOrDefault(i => i.name == key);

            if (param != null)
            {
                param.value = value;
            }
            else
            {
                parameters.Add(new UriParameter(key, value, escapeQuote));
            }
        }

        public UriParameter Find(string key)
        {
            return parameters.FirstOrDefault(i => i.name == key);
        }

        public string GetParameterValue(string key)
        {
            return parameters.FirstOrDefault(i => i.name == key)?.value;
        }

        public string ToUrl()
        {
            return Command + "?" + parameters.Select(i => i.ToUrl()).Aggregate((i, j) => i + "&" + j);
        }

        public T GetParameterValue<T>(string key)
        {
            string find = Find(key)?.value;
            return find == null ? default(T) : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(find);
        }

        public void Set(string key, object value)
        {
            Set(key, Newtonsoft.Json.JsonConvert.SerializeObject(value), true);
        }

        public void Remove(string list)
        {
            parameters.RemoveAll(i => i.name == "list");
        }
    }

    public class UriParameter
    {
        public UriParameter() { }

        public UriParameter(string name, string value, bool escape = false)
        {
            this.name = name;
            this.escaped = escape;
            this.value = value;
        }

        public string name;

        public string value { get; set; }

        public bool escaped { get; set; }

        public string ToUrl()
        {
            return name + "=" + (escaped ? "\"" + value + "\"" : value);
        }
    }
}
