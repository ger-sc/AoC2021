using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

const string file = "input.txt";
var input = File.ReadAllLines(file);
var width = input[0].Length;

var readings = File.ReadAllText(file)
  .ReplaceLineEndings(string.Empty)
  .Select((r, i) => new ReadingPosition(int.Parse(r.ToString()), i))
  .ToList();

var lowPoints = readings
  .Where((v, i) => GetNeighbors(readings, i, width).All(n => n.Reading > v.Reading))
  .ToList();

Console.Out.WriteLine(lowPoints.Select(x => x.Reading + 1).Sum());

var basins = lowPoints
  .Select(lowPoint => GetBasin(readings, lowPoint, width, new List<ReadingPosition>()))
  .Select(res => res.Select(x => x.Position).Distinct().Count())
  .ToList();

Console.Out.WriteLine(basins.OrderByDescending(x => x).Take(3).Aggregate((a, b) => a * b));

IEnumerable<ReadingPosition> GetNeighbors(IReadOnlyList<ReadingPosition> r, int i, int w) {
  var neighbors = new List<ReadingPosition>();
  if (i - 1 >= 0 && i % w != 0) neighbors.Add(r[i-1]);
  if (i + 1 < r.Count && (i + 1) % w != 0) neighbors.Add(r[i+1]);
  if (i - w >= 0) neighbors.Add(r[i-w]);
  if (i + w < r.Count) neighbors.Add(r[i+w]);
  return neighbors;
}

List<ReadingPosition> GetBasin(IReadOnlyList<ReadingPosition> read, ReadingPosition rp, int w, List<ReadingPosition> result) {
  result.Add(rp);
  var neighbors = GetNeighbors(read, rp.Position, w);
  var todo = neighbors.Where(x => x.Reading != 9 && !result.Contains(x)).ToList();
  foreach (var t in todo) {
    result.AddRange(GetBasin(read, t, w, result).Where(x => !result.Contains(x)));
  }
  return result;
}

internal class ReadingPosition {
  public readonly int Position;
  public readonly int Reading;

  public ReadingPosition(int reading, int position) {
    Position = position;
    Reading = reading;
  }
}