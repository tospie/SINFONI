using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using KIARA.Exceptions;

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

        private void parseServiceFunctionDefinition(string line, int lineNumber)
        {
            if (!(line.Contains(';')
                && line.Contains('(')
                && line.Contains(')')))
                throw new IDLParseException(line, lineNumber);

            int indexOfOpenPar = line.IndexOf('(');
            int indexOfClosePar = line.IndexOf(')');

            string parameterDefinition = line.Substring(indexOfOpenPar + 1, indexOfClosePar - (indexOfOpenPar + 1));
            string nameAndType = line.Substring(0, indexOfOpenPar);

            ServiceFunctionDescription newServiceFunction = createTypedServiceFunction(nameAndType);
            parseParameters(parameterDefinition, newServiceFunction);
            currentlyParsedService.serviceFunctions.Add(newServiceFunction.Name, newServiceFunction);
        }

        private ServiceFunctionDescription createTypedServiceFunction(string nameAndType)
        {
            string[] values = nameAndType.Split(' ');
            KtdType returnType;
            if (values[0].Trim() == "void")
                returnType = new KtdType("void");
            else
                returnType = getKtdType(values[0].Trim());

            return new ServiceFunctionDescription(values[1], returnType);
        }

        private void parseParameters(string parameterDefinition, ServiceFunctionDescription functionDescription)
        {
            if (parameterDefinition.Length == 0)
                return;

            string[] parameters = parameterDefinition.Split(',');
            foreach (string param in parameters)
            {
                createParameterForServiceFunction(param.Trim(), functionDescription);
            }
        }

        private void createParameterForServiceFunction(string param, ServiceFunctionDescription functionDescription)
        {
            string[] values = param.Split(' ');
            KtdType paramType = getKtdType(values[0].Trim());
            string paramName = values[1].Trim();
            functionDescription.parameters.Add(paramName, paramType);
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
