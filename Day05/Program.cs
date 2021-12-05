using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

var input = File.ReadAllLines("input.txt");

var lines = GetLines(input).ToList();
var normalLines = lines.Where(IsNormal).ToList();

var result = DrawLines(normalLines);
var overlaps = result.Values.Count(x => x > 1);
Console.Out.WriteLine(overlaps);

var result2 = DrawLines(lines);
var overlaps2 = result2.Values.Count(x => x > 1);
Console.Out.WriteLine(overlaps2);


Dictionary<Point, int> DrawLines(IEnumerable<Line> list) {
  var dictionary = new Dictionary<Point, int>();
  foreach (var point in list.Select(GetPointsOnLine).SelectMany(points => points)) {
    if (dictionary.ContainsKey(point)) {
      dictionary[point]++;
    }
    else {
      dictionary[point] = 1;
    }
  }
  return dictionary;
}

IEnumerable<Point> GetPointsOnLine(Line l) {
  var x = l.X1;
  var y = l.Y1;
  yield return new Point(x, y);
  while (x != l.X2 || y != l.Y2) {
    if (x < l.X2) x++;
    if (x > l.X2) x--;
    if (y < l.Y2) y++;
    if (y > l.Y2) y--;
    yield return new Point(x, y);
  }
}

bool IsNormal(Line l) {
  return l.X1 == l.X2 || l.Y1 == l.Y2;
}

IEnumerable<Line> GetLines(IEnumerable<string> inputLines) {
  return inputLines
    .Select(x => x.Split(" -> "))
    .Select(x => new { Start = x[0].Split(","), End = x[1].Split(",") })
    .Select(x => new Line(int.Parse(x.Start[0]),
      int.Parse(x.Start[1]),
      int.Parse(x.End[0]),
      int.Parse(x.End[1])));
}

internal class Line {
  public readonly int X2;
  public readonly int Y1;
  public readonly int X1;
  public readonly int Y2;

  public Line(int x1, int y1, int x2, int y2) {
    X1 = x1;
    Y1 = y1;
    X2 = x2;
    Y2 = y2;
  }
}