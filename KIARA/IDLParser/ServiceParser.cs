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

        internal void parseLineOfService(string line, int lineNumber)
        {
            if (line.Contains('{'))
                line = line.Split('{')[1].Trim();
            if (line.Length == 0)
                return;

            if (line.Contains('}'))
                finalizeServiceParsing(line);
            else
                parseServiceFunctionDefinition(line, lineNumber);
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
            string[] values = splitDeclarationInNameAndType(nameAndType);

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
            parameterDefinition = replaceMapKeyValueDelimiter(parameterDefinition);
            string[] parameters = parameterDefinition.Split(';');
            foreach (string param in parameters)
            {
                createParameterForServiceFunction(param.Trim(), functionDescription);
            }
        }

        private void createParameterForServiceFunction(string param, ServiceFunctionDescription functionDescription)
        {
            string[] values = splitDeclarationInNameAndType(param);

            KtdType paramType = getKtdType(values[0].Trim());
            string paramName = values[1].Trim();
            functionDescription.parameters.Add(paramName, paramType);
        }

        private string[] splitDeclarationInNameAndType(string declaration)
        {
            string[] values;
            if (declaration.Contains("array<") || declaration.Contains("map<"))
            {
                declaration = Regex.Replace(declaration, @"\s+", "");
                values = declaration.Split('>');
                values[0] = values[0] + '>';
            }
            else
            {
                values = declaration.Split(' ');
            }

            return values;
        }

        private string replaceMapKeyValueDelimiter(string paramDefinition)
        {
            bool inMapDef = false;
            int currentIndex = 0;
            StringBuilder replacedStringBuilder = new StringBuilder(paramDefinition);
            foreach (char c in paramDefinition)
            {
                if (c == '<')
                    inMapDef = true;
                if (c == '>')
                    inMapDef = false;

                if (c == ',' && !inMapDef)
                {
                    replacedStringBuilder.Remove(currentIndex, 1);
                    replacedStringBuilder.Insert(currentIndex, ';');
                }
                currentIndex++;
            }
            return replacedStringBuilder.ToString();
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

        private void finalizeServiceParsing(string lastLine)
        {
            ServiceRegistry.Instance.services.Add(currentlyParsedService.Name, currentlyParsedService);
            IDLParser.Instance.currentlyParsing = IDLParser.ParseMode.NONE;
            if (lastLine.Length > 1)
                IDLParser.Instance.parseLine(lastLine.Split('}')[1].Trim());
        }

        KiaraService currentlyParsedService;
    }
}
