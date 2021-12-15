using System.Diagnostics;
using System.Drawing;

var input = File.ReadAllLines("input.txt").ToList();

var inputMap = new Dictionary<Point, long>();
for (var y = 0; y < input.Count; y++) {
  for (var x = 0; x < input[y].Length; x++) {
    inputMap.Add(new Point(x, y), long.Parse(input[y][x].ToString()));
  }
}

var sw = Stopwatch.StartNew();
var result = AStar(inputMap);
sw.Stop();
Console.Out.WriteLine($"{result} in {sw.Elapsed.TotalSeconds} s");

var maxX = input.First().Length;
var maxY = input.Count;

var inputMap2 = new Dictionary<Point, long>();
for (var y = 0; y < input.Count * 5; y++) {
  for (var x = 0; x < input.First().Length * 5; x++) {
    var origY = y % maxY;
    var origX = x % maxX;
    var add = x / maxX + y / maxY;
    var origValue = long.Parse(input[origY][origX].ToString());
    var newValue = origValue;
    while (add > 0) {
      newValue++;
      if (newValue > 9) newValue = 1;
      add--;
    }
    inputMap2.Add(new Point(x, y), newValue);
  }
}
sw = Stopwatch.StartNew();
var result2 = AStar(inputMap2);
sw.Stop();
Console.Out.WriteLine($"{result2} in {sw.Elapsed.TotalSeconds} s");

long AStar(Dictionary<Point, long> map) {
  var openList = new Dictionary<Point, long>();
  var closedList = new List<Point>();

  var goal = new Point(map.Keys.Max(k => k.X), map.Keys.Max(k => k.Y));

  openList.Add(new Point(0,0), 0);

  do {
    var minPoint = openList.Values.Min();
    var currentNode = openList.First(x => x.Value == minPoint);
    openList.Remove(currentNode.Key);
    if (currentNode.Key == goal) {
      return currentNode.Value;
    }

    closedList.Add(currentNode.Key);
    ExpandNode(currentNode, openList, closedList, map);

  } while (openList.Any());

  return 0;
}

void ExpandNode(KeyValuePair<Point, long> node, Dictionary<Point, long> openList, List<Point> closedList, Dictionary<Point, long> map) {
  foreach (var successor in GetNeighbors(node.Key, map)) {
    if(closedList.Contains(successor)) continue;
    var costs = node.Value + map[successor];
    if(openList.ContainsKey(successor) && costs >= openList[successor]) continue;
    openList[successor] = costs;
  }
}

IEnumerable<Point> GetNeighbors(Point node, Dictionary<Point, long> map) {
  var maxPointX = map.Keys.Max(k => k.X);
  var maxPointY = map.Keys.Max(k => k.Y);
  if (node.X > 0) yield return new Point(node.X - 1, node.Y);
  if (node.X < maxPointX) yield return new Point(node.X + 1, node.Y);
  if (node.Y > 0) yield return new Point(node.X, node.Y - 1);
  if (node.Y < maxPointY) yield return new Point(node.X, node.Y + 1);
}