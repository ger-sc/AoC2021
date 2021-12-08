using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var input = File.ReadAllLines("input.txt").ToList();

var signals = ReadInput(input).ToList();

Console.Out.WriteLine(signals.Select(x => x.Outputs).SelectMany(x => x)
  .Count(x => x.Length is 2 or 4 or 3 or 7));

var part2 = Solve(signals);

Console.Out.WriteLine(part2);

int Solve(IEnumerable<Signal> inputSignals) {
  return inputSignals.Select(DetermineNumbers).Sum();
}

int DetermineNumbers(Signal signal) {
  var dict = signal.Inputs.Concat(signal.Outputs).Distinct().ToDictionary(s => s, GetPossibleNumbers);

  while (dict.Values.Any(x => x.Count > 1)) {

    var kb = Enumerable.Range(0, 10)
      .Select(n => dict.SingleOrDefault(x => x.Value.Contains(n) && x.Value.Count == 1).Key).ToArray();

    foreach (var key in dict.Keys.Where(x => x.Length == 5)) {
      if (IsKnown(kb, 1) && kb[1].ToCharArray().All(x => key.Contains(x))) {
        dict[key].Remove(2);
        dict[key].Remove(5);
      }

      if (IsKnown(kb, 6) && IsKnown(kb, 3) && kb[6].ToCharArray().Except(key.ToCharArray()).Count() == 2) {
        dict[key].Remove(5);
      }
      if (IsKnown(kb, 9) && IsKnown(kb, 3) && kb[9].ToCharArray().Except(key.ToCharArray()).Count() == 2) {
        dict[key].Remove(2);
      }
    }
    
    foreach (var key in dict.Keys.Where(x => x.Length == 6)) {
      if (IsKnown(kb, 1) && kb[1].ToCharArray().All(x => key.Contains(x))) {
        dict[key].Remove(6);
      }
      if (IsKnown(kb, 7) && !kb[7].ToCharArray().All(x => key.Contains(x))) {
        dict[key].Remove(9);
        dict[key].Remove(0);
      }
      if (IsKnown(kb, 4) && IsKnown(kb, 6) && !kb[4].ToCharArray().All(x => key.Contains(x))) {
        dict[key].Remove(9);
      }
    }

    foreach (var key in dict.Keys) {
      for (var i = 0; i <= 9; i++) {
        if(IsKnown(kb, i) && kb[i] != key) dict[key].Remove(i);
      }
    }
  }

  return dict[signal.Outputs[0]].Single() * 1000 +
         dict[signal.Outputs[1]].Single() * 100 +
         dict[signal.Outputs[2]].Single() * 10 +
         dict[signal.Outputs[3]].Single();
}

bool IsKnown(IReadOnlyList<string> known, int index) {
  return !string.IsNullOrWhiteSpace(known[index]);
}

IList<int> GetPossibleNumbers(string s) {
  return s.Length switch {
    2 => new List<int> { 1 },
    3 => new List<int> { 7 },
    4 => new List<int> { 4 },
    5 => new List<int> { 2, 3, 5 },
    6 => new List<int> { 6, 9, 0 },
    7 => new List<int> { 8 },
    _ => new List<int>()
  };
}

IEnumerable<Signal> ReadInput(IEnumerable<string> inputList) {
  return inputList.Select(x => x.Split("|"))
    .Select(x => new { Input = x[0].Trim(), OutPut = x[1].Trim() })
    .Select(x => new Signal(
      x.Input.Split(" ").Select(i => new string(i.OrderBy(c => c).ToArray())).ToList(),
      x.OutPut.Split(" ").Select(i => new string(i.OrderBy(c => c).ToArray())).ToList()));
}

internal class Signal {
  public readonly IList<string> Inputs;
  public readonly IList<string> Outputs;

  public Signal(IList<string> inputs, IList<string> outputs) {
    Inputs = inputs;
    Outputs = outputs;
  }
}