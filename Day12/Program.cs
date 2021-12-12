using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var input = File.ReadAllLines("input.txt");

var caveMap = new Dictionary<string, SortedSet<string>>();
foreach (var i in input) {
  var caves = i.Split("-");
  var cave1 = caves[0];
  var cave2 = caves[1];
  if (!caveMap.ContainsKey(cave1)) {
    caveMap[cave1] = new SortedSet<string> { cave2 };
  }
  else {
    caveMap[cave1].Add(cave2);
  }
  if (!caveMap.ContainsKey(cave2)) {
    caveMap[cave2] = new SortedSet<string> { cave1 };
  }
  else {
    caveMap[cave2].Add(cave1);
  }
}

var results = new List<List<string>>();
FindPaths(caveMap, new List<string> {"start"}, "start", IsPossible);
Console.Out.WriteLine(results.Count);

results = new List<List<string>>();
FindPaths(caveMap, new List<string> {"start"}, "start", IsPossible2);
Console.Out.WriteLine(results.Count);

void FindPaths(IReadOnlyDictionary<string, SortedSet<string>> map, List<string> path, string root, Func<string, List<string>, bool> IsPossible) {
  if (root == "end") {
    results.Add(path);
    return;
  }
  var adjacentCaves = map[root];
  foreach (var cave in adjacentCaves.Where(cave => IsPossible(cave, path))) {
    FindPaths(map, path.Concat(new []{ cave }).ToList(), cave, IsPossible);
  }
}

bool IsPossible(string cave, List<string> path) {
  if (cave.ToUpper() == cave) return true;
  return !path.Contains(cave);
}

bool IsPossible2(string cave, List<string> path) {
  if (cave.ToUpper() == cave) return true;
  if (cave == "start" && path.Contains(cave)) return false;
  var twice = path
    .Where(x => x.ToLower() == x)
    .GroupBy(x => x)
    .Where(c => c.Count() > 1)
    .Select(x => x.Key)
    .SingleOrDefault();
  if (twice != null && cave == twice) return false;
  return twice == null || cave == twice || !path.Contains(cave);
}
