using System;
using System.IO;
using System.Linq;

var input = File.ReadAllText("input.txt").Split(",").Select(long.Parse).ToList();

var min = (int)input.Min();
var max = (int)input.Max();

var minSum = long.MaxValue;
for (var i = min; i <= max; i++) {
  var sum = input.Select(x => Math.Abs(i - x)).Sum();
  if (sum < minSum) minSum = sum;
}

Console.Out.WriteLine(minSum);

minSum = long.MaxValue;
for (var i = min; i <= max; i++) {
  var sum = input.Select(x => Enumerable.Range(1, (int)Math.Abs(i - x)).Sum()).Sum();
  if (sum < minSum) minSum = sum;
}

Console.Out.WriteLine(minSum);