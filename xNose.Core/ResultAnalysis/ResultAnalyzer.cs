using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using xNose.Core.Reporters;

namespace xNose.Core.ResultAnalysis
{
    public class TestSmellDistribution
    {
        public string TestSmellName { get; set; }
        public double ClassDistribution { get; set; }
        public double ProjectDistribution { get; set; }
    }

    public class TestSmellAnalyzer
    {
        public void AnalyzeTestSmells(List<string> jsonFileLocations)
        {
            List<TestSmellDistribution> distributions = new List<TestSmellDistribution>();
            int totalClasses = 0,
                totalProjects = 0,
                totalTestCases = 0,
                emptyRepos = 0,
                maxClassCount = 0,
                maxTestCaseCount = 0;
            Dictionary<string, int[]> affectedSmellsCount = new();
            List<string> distinctTestSmells = null;
            foreach (string fileLocation in jsonFileLocations)
            {
                try
                {
                    string jsonContent = File.ReadAllText(fileLocation);
                    var classReporters = JsonConvert.DeserializeObject<List<ClassReporter>>(jsonContent);
                    if (classReporters == null || classReporters.Count == 0)
                    {
                        emptyRepos++;
                        continue;
                    }

                    maxClassCount = Math.Max(maxClassCount, classReporters.Count);
                    totalClasses += classReporters.Count;
                    totalProjects += classReporters.Select(c => c.ProjectName).Distinct().Count();
                    var testCases = classReporters.Sum(c => c.Methods.Count);
                    maxTestCaseCount = Math.Max(maxTestCaseCount, testCases);
                    totalTestCases += testCases;
                    distinctTestSmells ??= GetDistinctTestSmells(classReporters);
                    foreach (var testSmell in distinctTestSmells)
                    {
                        int classCount = classReporters.Count(c =>
                            c.Methods.Any(m => m.Smells.Any(s => s.Name == testSmell && s.Status == "Found")));
                        int projectCount = classReporters.Select(c => c.ProjectName).Distinct()
                            .Count(p => classReporters.Any(c =>
                                c.ProjectName == p && c.Methods.Any(m =>
                                    m.Smells.Any(s => s.Name == testSmell && s.Status == "Found"))));

                        if (!affectedSmellsCount.ContainsKey(testSmell))
                        {
                            affectedSmellsCount[testSmell] = new int[2];
                        }

                        affectedSmellsCount[testSmell][0] += projectCount;
                        affectedSmellsCount[testSmell][1] += classCount;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            foreach (var testSmell in distinctTestSmells)
            {
                int classCount = affectedSmellsCount[testSmell][1];
                int projectCount = affectedSmellsCount[testSmell][0];
                double classDistribution = (double)classCount * 100 / totalClasses;
                double projectDistribution = (double)projectCount * 100 / totalProjects;
                TestSmellDistribution distribution = new TestSmellDistribution
                {
                    TestSmellName = testSmell,
                    ClassDistribution = classDistribution,
                    ProjectDistribution = projectDistribution
                };
                distributions.Add(distribution);
            }

            Console.WriteLine($"*******************************************");
            Console.WriteLine($"{JsonConvert.SerializeObject(distributions)}");
            Console.WriteLine($"*******************************************");
            Console.WriteLine($"*******************************************");
            Console.WriteLine(
                $"TotalProjects: {totalProjects}, TotalTestSuits: {totalClasses},TotalTestCase: {totalTestCases}");
            Console.WriteLine($"MaxTestClass: {maxClassCount}, MaxTestCase: {maxTestCaseCount}");
        }
        public async Task CopyResults(List<string> jsonFileLocations)
        {
            foreach (string fileLocation in jsonFileLocations)
            {
                if (File.Exists(fileLocation)==false)
                {
                    Console.WriteLine($"{fileLocation} not found");
                    continue;
                }
                string jsonContent = File.ReadAllText(fileLocation);
                string[] spiltted = fileLocation.Split('\\');
                var classReporters = JsonConvert.DeserializeObject<List<ClassReporter>>(jsonContent);
                var location =
                    $"E:\\workstation\\xNose\\results\\{spiltted[spiltted.Length-1]}";
                using var createStream = File.Create(location);
                var options = new JsonSerializerOptions { WriteIndented = true };
                await System.Text.Json.JsonSerializer.SerializeAsync(createStream, classReporters, options);
                await createStream.DisposeAsync();
            }
        }
        public void AnalyzeTestUniqueSmells(List<string> jsonFileLocations)
        {
            int totalClasses = 0;
            Dictionary<int, int> affectedSmellsCount = new();
            List<string> distinctTestSmells = null;
            foreach (string fileLocation in jsonFileLocations)
            {
                try
                {
                    string jsonContent = File.ReadAllText(fileLocation);
                    var classReporters = JsonConvert.DeserializeObject<List<ClassReporter>>(jsonContent);
                    totalClasses += classReporters.Count;

                    foreach (var classReporter in classReporters)
                    {
                        HashSet<string> testSmellNames = new();
                        foreach (var methodReporter in classReporter.Methods)
                        {
                            var smellNames = methodReporter.Smells.Where(item => item.Status == "Found").Select(item=>item.Name);
                            foreach (var VARIABLE in smellNames)
                            {
                                testSmellNames.Add(VARIABLE);
                            }
                        }

                        if (!affectedSmellsCount.ContainsKey(testSmellNames.Count))
                        {
                            affectedSmellsCount.Add(testSmellNames.Count, 1);
                        }
                        else
                            affectedSmellsCount[testSmellNames.Count]++;
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                
            }
            foreach (KeyValuePair<int, int> pair in affectedSmellsCount)
            {
                Console.WriteLine($"Unique test smell count: {pair.Key} ---------------> Distribution: {(pair.Value * 100.0 / totalClasses)}");
            }
        }


        /*public List<TestSmellDistribution> CalculateDistribution(JsonFileReporter report)
        {
            List<TestSmellDistribution> distributions = new List<TestSmellDistribution>();

            // Calculate the count of test classes and projects
            int totalClasses = report.Classes.Count;
            int totalProjects = report.Classes.Select(c => c.ProjectName).Distinct().Count();

            // Iterate through each test smell
            foreach (var testSmell in GetDistinctTestSmells(report))
            {
                // Count test classes and projects with the current test smell
                int classCount = report.Classes.Count(c => c.Methods.Any(m => m.Smells.Any(s => s.Name == testSmell)));
                int projectCount = report.Classes.Select(c => c.ProjectName).Distinct()
                    .Count(p => report.Classes.Any(c => c.ProjectName == p && c.Methods.Any(m => m.Smells.Any(s => s.Name == testSmell))));

                // Calculate the distributions
                double classDistribution = (double)classCount * 100 / totalClasses;
                double projectDistribution = (double)projectCount * 100 / totalProjects;

                // Create and add the distribution object
                TestSmellDistribution distribution = new TestSmellDistribution
                {
                    TestSmellName = testSmell,
                    ClassDistribution = classDistribution,
                    ProjectDistribution = projectDistribution
                };
                distributions.Add(distribution);
            }

            return distributions;
        }*/

        private List<string> GetDistinctTestSmells(List<ClassReporter> report)
        {
            List<string> testSmells = new List<string>();

            foreach (var testClass in report)
            {
                foreach (var method in testClass.Methods)
                {
                    foreach (var smell in method.Smells)
                    {
                        if (!testSmells.Contains(smell.Name))
                        {
                            testSmells.Add(smell.Name);
                        }
                    }
                }
            }

            return testSmells;
        }
    }
}