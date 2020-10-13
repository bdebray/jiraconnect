using System.Collections.Generic;
using System.Dynamic;

namespace JiraWriter.Extension
{
    public static class DictionaryCsvExtentions
    {
        public static dynamic ToDynamic(this Dictionary<string, object> dictionary)
        {
            dynamic dictionaryObject = new ExpandoObject();

            foreach (var item in dictionary)
            {
                AddProperty(dictionaryObject, item.Key, item.Value);
            }

            return dictionaryObject;
        }

        private static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
            {
                expandoDict[propertyName] = propertyValue;
            }
            else
            {
                expandoDict.Add(propertyName, propertyValue);
            }
        }
    }
}
