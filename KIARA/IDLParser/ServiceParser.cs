using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KIARA
{
    internal class ServiceParser
    {
        internal static ServiceParser Instance = new ServiceParser();

        internal void startServiceParsing(string serviceDefinition)
        {
            Regex nameRegEx = new Regex("service ([A-Za-z0-9_]*)[{}._<,>; ]*");
            Match nameMatch = nameRegEx.Match(serviceDefinition);
            string name = nameMatch.Groups[1].Value;
            currentlyParsedService = new KiaraService(name);
        }

        private KtdType getKtdType(string typeDefinition)
        {
            KtdType paramType;
            if (typeDefinition.Contains("array<"))
                paramType = ArrayParser.Instance.ParseArray(typeDefinition);
            else if (typeDefinition.Contains("map<"))
                paramType = MapParser.Instance.ParseMap(typeDefinition);
            else
                paramType = KTD.Instance.GetKtdType(typeDefinition);

            return paramType;
        }
        }

        KiaraService currentlyParsedService;
    }
}
