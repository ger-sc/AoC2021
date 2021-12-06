using System;
using System.IO;
using System.Linq;

const string inputFile = "input.txt";
var fish = File.ReadAllText(inputFile).Split(",").Select(int.Parse).ToList();

var day = 0;

while (day < 80) {
  var i = 0;
  var len = fish.Count;
  while (i < len) {
    fish[i]--;
    if (fish[i] < 0) {
      fish[i] = 6;
      fish.Add(8);
    }
    i++;
  }
  day++;
}

Console.Out.WriteLine(fish.Count);

fish = File.ReadAllText(inputFile).Split(",").Select(int.Parse).ToList();

var array = new long[] {0,0,0,0,0,0,0,0,0};
foreach (var f in fish) {
  array[f]++;
}
day = 0;
while (day < 256) {
  var newArray = new long[9];
  for (var i = 0; i <= 8; i++) {
    var index = (i + 1) % 9;
    newArray[i] = array[index];
    if (i == 6) {
      newArray[i] += array[0];
    }
  }
  array = newArray;
  day++;
}

Console.Out.WriteLine(array.Sum());