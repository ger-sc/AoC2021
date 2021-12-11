using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

var input = File.ReadAllLines("input.txt");
var map = new List<Octopus>();
var map2 = new List<Octopus>();
for (var row = 0; row < input.Length; row++) {
  for (var col = 0; col < input[row].Length; col++) {
    map.Add(new Octopus(col, row, int.Parse(input[row][col].ToString())));
    map2.Add(new Octopus(col, row, int.Parse(input[row][col].ToString())));
  }
}

for (var i = 1; i <= 100; i++) {
  Step(map);
}

Console.Out.WriteLine(map.Sum(o => o.Flashes));

var oldFlashCount = 0;
var flashes = 0;
var step = 0;
while (flashes != 100) {
  Step(map2);
  step++;
  var newFlashCount = map2.Sum(x => x.Flashes);
  flashes = newFlashCount - oldFlashCount;
  oldFlashCount = newFlashCount;
}

Console.Out.WriteLine(step);

void Step(IList<Octopus> octoMap) {
  foreach (var octo in octoMap) {
    octo.Erg += 1;
  }
  
  foreach (var octo in octoMap.Where(o => o.Erg == 10).ToList()) {
    IncreaseNeighbors(octo, octoMap);
  }

  foreach (var octo in octoMap.Where(o => o.Erg > 9).ToList()) {
    octo.Erg = 0;
    octo.Flashes++;
  }
}

void IncreaseNeighbors(Octopus octo, IList<Octopus> octoMap) {
  var nb = GetNeighbors(octo);
  foreach (var point in nb) {
    var found = octoMap.SingleOrDefault(o => o.Col == point.X && o.Row == point.Y);
    if (found == null) continue;
    found.Erg++;
    if (found.Erg == 10) {
      found.Erg++;
      IncreaseNeighbors(found, octoMap);
    }
  }
}

IEnumerable<Point> GetNeighbors(Octopus octo) {
  return new List<Point> {
    new (octo.Col - 1, octo.Row - 1),
    new (octo.Col, octo.Row - 1),
    new (octo.Col + 1, octo.Row - 1),
    new (octo.Col - 1, octo.Row),
    new (octo.Col + 1, octo.Row),
    new (octo.Col - 1, octo.Row + 1),
    new (octo.Col, octo.Row + 1),
    new (octo.Col + 1, octo.Row + 1)
  };
}

internal class Octopus {
  public readonly int Col;
  public readonly int Row;
  public int Erg { get; set; }
  public int Flashes;
  
  public Octopus(int col, int row, int erg) {
    Col = col;
    Row = row;
    Erg = erg;
    Flashes = 0;
  }
}