using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xNose.Core.Reporters;

namespace xNose.Core.ResultAnalysis
{
    public class TestSmellAnalyzer
    {
        private List<ClassReporter> classes;

        public void AnalyzeTestSmells(List<string> jsonFileLocations)
        {
            LoadDataFromJson(jsonFileLocations);
            CalculateTestSuiteSmelliness();
            CalculateTestSmellDistribution();
            CalculateTestSmellCorrelation();
        }

        private void LoadDataFromJson(List<string> jsonFileLocations)
        {
            classes = new List<ClassReporter>();

            foreach (var fileLocation in jsonFileLocations)
            {
                try
                {
                    string jsonContent = File.ReadAllText(fileLocation);
                    var classReporters = JsonConvert.DeserializeObject<List<ClassReporter>>(jsonContent);
                    classes.AddRange(classReporters);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading JSON file '{fileLocation}': {ex.Message}");
                }
            }
        }

        private void CalculateTestSuiteSmelliness()
        {
            var testSuiteSmelliness = new Dictionary<string, int>();
            testSuiteSmelliness["Lack of cohesion"] = 0;
            foreach (var cls in classes)
            {
                if (string.IsNullOrWhiteSpace(cls.Message) == false)
                {
                    testSuiteSmelliness["Lack of cohesion"]++;
                }
                foreach (var method in cls.Methods)
                {
                    int smellCount = method.Smells.Count;
                    foreach (var smell in method.Smells)
                    {
                        if (!testSuiteSmelliness.ContainsKey(smell.Name))
                        {
                            testSuiteSmelliness[smell.Name] = 0;
                        }
                        if (smell.Status == "Found")
                        {
                            testSuiteSmelliness[smell.Name]++;
                        }
                    }
                }
            }

            Console.WriteLine("\nTest Suite Smelliness:");
            foreach (var smellCount in testSuiteSmelliness.Keys)
            {
                double percentage = (double)testSuiteSmelliness[smellCount] / classes.Sum(c => c.Methods.Count) * 100;
                Console.WriteLine($"{smellCount} test smells: {percentage}%");
            }
        }

        private void CalculateTestSmellDistribution()
        {
            var testSmellDistribution = new Dictionary<string, double>();
            testSmellDistribution["Lack of cohesion"] = 0;
            foreach (var cls in classes)
            {
                if (string.IsNullOrWhiteSpace(cls.Message) == false)
                {
                    testSmellDistribution["Lack of cohesion"]++;
                }
                foreach (var method in cls.Methods)
                {
                    foreach (var smell in method.Smells)
                    {
                        if (!testSmellDistribution.ContainsKey(smell.Name))
                        {
                            testSmellDistribution[smell.Name] = 0;
                        }

                        if (smell.Status == "Found")
                        {
                            testSmellDistribution[smell.Name]++;
                        }
                    }
                }
            }

            Console.WriteLine("\nTest Smell Distribution:");
            foreach (var smellName in testSmellDistribution.Keys)
            {
                double percentage = (double)testSmellDistribution[smellName] / classes.Sum(c => c.Methods.Sum(m => m.Smells.Where(smel => smel.Status == "Found").ToList().Count)) * 100;
                Console.WriteLine($"{smellName}: {percentage}%");
            }
        }

        private void CalculateTestSmellCorrelation()
        {
            var smellCorrelations = new Dictionary<string, Dictionary<string, double>>();

            foreach (var cls in classes)
            {
                foreach (var method in cls.Methods)
                {
                    var smells = method.Smells.Select(s => s.Name).ToList();
                    foreach (var smell in method.Smells)
                    {
                        if (!smellCorrelations.ContainsKey(smell.Name))
                        {
                            smellCorrelations[smell.Name] = new Dictionary<string, double>();
                        }
                        foreach (var smell1 in method.Smells)
                        {
                            if (smell.Name != smell1.Name)
                            {
                                if (!smellCorrelations[smell.Name].ContainsKey(smell1.Name))
                                {
                                    smellCorrelations[smell.Name][smell1.Name] = 0;
                                }
                                if (smell1.Status == "Found")
                                {
                                    smellCorrelations[smell.Name][smell1.Name]++;
                                }
                            }
                        }
                    }
                   
                }
            }

            Console.WriteLine("\nTest Smell Correlation:");
            foreach (var smell1 in smellCorrelations.Keys)
            {
                foreach (var smell2 in smellCorrelations[smell1].Keys)
                {
                    double percentage = (double)smellCorrelations[smell1][smell2] / smellCorrelations[smell1].Values.Sum() * 100;
                    Console.WriteLine($"{smell1} -> {smell2}: {percentage}%");
                }
            }
        }
    }

}
