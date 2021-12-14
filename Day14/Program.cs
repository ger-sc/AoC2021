var input = File.ReadAllLines("input.txt").ToList();
var template = input[0];
var rules = ReadRules(input.Skip(2).ToList());

Console.Out.WriteLine(PerformSteps(template, rules, 10));
Console.Out.WriteLine(PerformSteps(template, rules, 40));

long PerformSteps(string polymer, Dictionary<string, string> rule, int steps) {
  var dict = polymer
    .Zip(polymer.Skip(1))
    .ToLookup(tuple => (tuple.First.ToString() + tuple.Second).ToString())
    .ToDictionary(x => x.Key, x => (long)x.Count());

  var counts = polymer
    .ToLookup(x => x)
    .ToDictionary(x => x.Key.ToString(), x => (long)x.Count());
  
  for (var i = 1; i <= steps; i++) {
    var newDict = new Dictionary<string, long>();
    foreach (var (key, value) in dict) {
      var middle = rule[key];
      AddToDict(newDict, key.First() + middle, value);
      AddToDict(newDict, middle + key.Last(), value);
      AddToDict(counts, middle, value);
    }
    dict = newDict;
  }

  var result = counts.Values.Max() - counts.Values.Min();
  return result;
}

void AddToDict(Dictionary<string, long> dict, string key, long value) {
  if (dict.ContainsKey(key)) {
    dict[key] += value;
  } else {
    dict[key] = value;
  }
}

Dictionary<string, string> ReadRules(IEnumerable<string> lines) {
  return lines.ToDictionary(l => l[..2], l => l[6..]);
}