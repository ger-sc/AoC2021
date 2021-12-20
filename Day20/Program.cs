using System.Drawing;
using System.Text;

var input = File.ReadAllLines("input.txt");
var enhanceString = input.First();
var mapDict = ReadMap(input.Skip(2).ToList());

var part1 = Part1(mapDict, enhanceString);
Console.Out.WriteLine(part1);
var part2 = Part2(mapDict, enhanceString);
Console.Out.WriteLine(part2);

int Part1(IReadOnlyDictionary<Point, bool> map, string enhance) {
  var dict = map;
  for (var i = 0; i < 2; i++) {
    var def = enhance[0] == '.' ? "0" : i % 2 == 0 ? "0" : "1";
    dict = Enhance(dict, enhance, def);
  }
  return dict.Count(x => x.Value);
}

int Part2(IReadOnlyDictionary<Point, bool> map, string enhance) {
  var dict = map;
  for (var i = 0; i < 50; i++) {
    var def = enhance[0] == '.' ? "0" : i % 2 == 0 ? "0" : "1";
    dict = Enhance(dict, enhance, def);
  }
  return dict.Count(x => x.Value);
}

Dictionary<Point, bool> Enhance(IReadOnlyDictionary<Point, bool> map, string enhance, string backGround) {
  var minX = map.Keys.Min(p => p.X) - 2;
  var maxX = map.Keys.Max(p => p.X) + 2;
  var minY = map.Keys.Min(p => p.Y) - 2;
  var maxY = map.Keys.Max(p => p.Y) + 2;
  var newMap = new Dictionary<Point, bool>();

  for (var y = minY; y <= maxY; y++) {
    for (var x = minX; x <= maxX; x++) {
      var point = new Point(x, y);
      var n = CalcEnhancedValue(map, point, backGround);
      newMap.Add(point, enhance[n] == '#');
    }
  }
  return newMap;
}

int CalcEnhancedValue(IReadOnlyDictionary<Point, bool> map, Point p, string backGround) {
  var sb = new StringBuilder();
  for (var y = p.Y - 1; y <= p.Y + 1; y++) {
    for (var x = p.X - 1; x <= p.X + 1; x++) {
      var np = new Point(x, y);
      if (map.ContainsKey(np)) {
        sb.Append(map[np] ? "1" : "0");
      }
      else {
        sb.Append(backGround);
      }
    }
  }
  return Convert.ToInt32(sb.ToString(), 2);
}

Dictionary<Point, bool> ReadMap(IReadOnlyList<string> lines) {
  var dict = new Dictionary<Point, bool>();
  for (var y = 0; y < lines.Count; y++) {
    for (var x = 0; x < lines[y].Length; x++) {
      dict.Add(new Point(x, y), lines[y][x] == '#');
    }
  }
  return dict;
}