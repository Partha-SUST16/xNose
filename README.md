# xNose: A Test Smell Detector for C#
This tool can detect test smells written with xUnit package in C# projects.
Currently, we support the following test smells:
1. Assertion Roulette Test Smell
2. Conditional Test Smell
3. Constructor Initialization Test Smell
4. Duplicate Assert Test Smell
5. Empty Test Smell
6. Eager Test Smell
7. Ignored Test Smell
8. Lack of Cohesion Smell
9. Magic Number Test Smell
10. Obscure In-Line Setup Test Smell
11. Redundant Assertion Test Smell
12. Redundant Print Smell
13. Sleepy Test Smell
14. Sensitive Equality Test Smell
15. Unknown Test Smell
16. Inappropriate Assertion

## Setup
To run this tool, you must download this repository in your local device.
Then follow the steps to run it with command line(CMD).
> ** Prerequisite: You have already installed C# environment in your pc and it's already associated with your command line. **

1. Go to the xNose directory
2. Open the terminal and type the following command

> ``dotnet run --project xNose.Core/xNose.Core.csproj   "<solution_path>"``

The `solution_path` is the path to your C# project solution file.

This will generate a `JSON` file of the given project to the root path of that project. This `JSON` file will have the info about test smells of the given project.

Otherwise, you can run the `xNose.sln` file with your Visual Studio. You have to provide the solution path of the desired project in the argument section.
 
## JSON File Format

```
[
    {
        "Class_Name": ***,
        "Message": <if Lack of Cohesion present for this class>,
        "Methods": [
            {
                "Name":***,
                "Body":***,
                "Smells": [
                    {
                        "Name": ***,
                        "Status": "Found"/"Not Found"
                    },
                    ...
                ]
            },
            ...
        ]
    },
    ...
]
```

To cite this paper:
```
@inproceedings{10.1145/3639478.3643116,
author = {Paul, Partha Protim and Akanda, Md Tonoy and Ullah, Mohammed Raihan and Mondal, Dipto and Chowdhury, Nazia Sultana and Tawsif, Fazle Mohammed},
title = {xNose: A Test Smell Detector for C#},
year = {2024},
isbn = {9798400705021},
publisher = {Association for Computing Machinery},
address = {New York, NY, USA},
url = {https://doi.org/10.1145/3639478.3643116},
doi = {10.1145/3639478.3643116},
abstract = {Test smells, similar to code smells, can negatively impact both the test code and the production code being tested. Despite extensive research on test smells in languages like Java, Scala, and Python, automated tools for detecting test smells in C# are lacking. This paper aims to bridge this gap by extending the study of test smells to C#, and developing a tool (xNose) to identify test smells in this language and analyze their distribution across projects. We identified 16 test smells from prior studies that were language-independent and had equivalent features in C# and evaluated xNose, achieving a precision score of 96.97\% and a recall score of 96.03\%. In addition, we conducted an empirical study to determine the prevalence of test smells in xUnit-based C# projects. This analysis sheds light on the frequency and distribution of test smells, deepening our understanding of their impact on C# projects and test suites. The development of xNose and our analysis of test smells in C# code aim to assist developers in maintaining code quality by addressing potential issues early in the development process.},
booktitle = {Proceedings of the 2024 IEEE/ACM 46th International Conference on Software Engineering: Companion Proceedings},
pages = {370–371},
numpages = {2},
keywords = {test smell, code smell, empirical studies, C#, abstract syntax tree (AST), rosalyn, static analysis},
location = {Lisbon, Portugal},
series = {ICSE-Companion '24}
}
```
