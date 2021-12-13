using System.Drawing;

var input = File.ReadAllLines("input.txt");

var (dots, folds) = ReadInput(input);

dots = Fold(dots, folds.First());

Console.Out.WriteLine(dots.Count);

foreach (var fold in folds.Skip(1)) {
  dots = Fold(dots, fold);
}

PrintResult(dots);

void PrintResult(ICollection<Point> resultDots) {
  for (var y = 0; y <= resultDots.Max(d => d.Y); y++) {
    for (var x = 0; x <= resultDots.Max(d => d.X); x++) {
      var point = new Point(x, y);
      Console.Out.Write(resultDots.Contains(point) ? "#" : ".");
    }
    Console.Out.WriteLine();
  }
}

IList<Point> Fold(IList<Point> dotList, Point fold) {
  return fold.Y > -1 ? FoldY(dotList.ToList(), fold.Y) : FoldX(dotList.ToList(), fold.X);
}

IList<Point> FoldY(List<Point> dotList, int foldLine) {
  dotList.AddRange(dotList
    .Where(d => d.Y > foldLine)
    .ToList()
    .Select(dot => new Point(dot.X, foldLine - (dot.Y - foldLine)))
    .Where(newDot => !dotList.Contains(newDot)));
  return dotList.Where(d => d.Y < foldLine).ToList();
}

IList<Point> FoldX(List<Point> dotList, int foldLine) {
  dotList.AddRange(dotList
    .Where(d => d.X > foldLine)
    .ToList()
    .Select(dot => new Point(foldLine - (dot.X - foldLine), dot.Y))
    .Where(newDot => !dotList.Contains(newDot)));
  return dotList.Where(d => d.X < foldLine).ToList();
}

(IList<Point>, IList<Point>) ReadInput(string[] inputArray) {
  var d = new List<Point>();
  var f = new List<Point>();
  foreach (var line in inputArray) {
    if (string.IsNullOrWhiteSpace(line)) continue;
    if (line.StartsWith("fold")) {
      var value = int.Parse(line.Substring(13));
      f.Add(line.Contains("x") ? new Point(value, -1) : new Point(-1, value));
    }
    else {
      var s = line.Split(",");
      var p = new Point(int.Parse(s[0]), int.Parse(s[1]));
      d.Add(p);
    }
  }

  return (d, f);
}