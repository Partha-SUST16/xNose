using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using xNose.Core.Reporters;
namespace xNose.Core.ResultAnalysis
{
    public static class ResultAnalysis
    {
        private const string LackOfCohesion = "LackOfCohesionTest";
        private static Dictionary<string, int> totalSmellCount = new Dictionary<string, int>();

        public static async Task AnalysisResult(List<string> filePaths)
        {
            //List<string> filePaths = new List<string>()
            //  {
            //rootPath + "abp_test_smell_reports.json",
            //rootPath + "c4sharp_test_smell_reports.json",
            //rootPath + "eshoponweb_test_smell_reports.json",
            //rootPath + "greendonut_test_smell_reports.json",
            //rootPath + "hotchocolate.caching_test_smell_reports.json",
            //rootPath + "hotchocolate.core_test_smell_reports.json",
            //rootPath + "nlog_test_smell_reports.json",
            //rootPath + "ocelot_test_smell_reports.json",
            //rootPath + "refit_test_smell_reports.json",
            //rootPath + "skoruba.identityserver4.admin_test_smell_reports.json",
            //rootPath + "scrutor_test_smell_reports.json"
            // };
            totalSmellCount["TestClass"] = 0;
            totalSmellCount["TestMethod"] = 0;
            foreach (string filePath in filePaths)
            {
                if (File.Exists(filePath))
                {
                    string data = await File.ReadAllTextAsync(filePath);
                    List<ClassReporter> obj = JsonConvert.DeserializeObject<List<ClassReporter>>(data);
                    var projectName = filePath.Split("\\")[filePath.Split("\\").Length - 1];
                    DetailsAnalysis(obj, projectName);

                }
                else
                {
                    Console.WriteLine($"{filePath} not found");
                }
            }
            Console.WriteLine(ToDebugString(totalSmellCount));
        }
        private static string ToDebugString<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
        }
        private static void DetailsAnalysis(List<ClassReporter> obj, string projectName)
        {
            Dictionary<string, Dictionary<string, int>> counter = new Dictionary<string, Dictionary<string, int>>();
            foreach (ClassReporter classReporter in obj)
            {
                if (!counter.ContainsKey(classReporter.Name))
                {
                    counter[classReporter.Name] = new Dictionary<string, int>();
                }
                if (!counter[classReporter.Name].ContainsKey(LackOfCohesion))
                {
                    counter[classReporter.Name][LackOfCohesion] = (classReporter.Message == null ? 0 : 1);
                }
                foreach (MethodReporter methodReporter in classReporter.Methods)
                {
                    foreach (var smell in methodReporter.Smells)
                    {
                        if (!counter[classReporter.Name].ContainsKey(smell.Name))
                        {
                            counter[classReporter.Name][smell.Name] = 0;
                        }
                        counter[classReporter.Name][smell.Name] += (smell.Status == "Found" ? 1 : 0);
                    }
                }
                totalSmellCount["TestClass"]++;
                totalSmellCount["TestMethod"] += classReporter.Methods.Count;
            }
            Console.WriteLine($"\n\n**********************");
            Dictionary<string, int> totalCount = new Dictionary<string, int>();
            foreach (var (className, smellCounter) in counter)
            {
                foreach (var (smellName, totalFound) in smellCounter)
                {
                    if (!totalCount.ContainsKey(smellName))
                    {
                        totalCount[smellName] = 0;
                    }
                    totalCount[smellName] += totalFound;
                    if (!totalSmellCount.ContainsKey(smellName))
                    {
                        totalSmellCount[smellName] = 0;
                    }
                    totalSmellCount[smellName] += totalCount[smellName];
                }
            }
            Console.WriteLine($"Project Name: {projectName}\n{ToDebugString(totalCount)}");
        }
    }


}


