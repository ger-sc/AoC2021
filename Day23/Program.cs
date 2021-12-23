using System.Drawing;
using System.Text;

/*var dict = new Dictionary<Point, AmphiPodType> {
  { new Point(2, 1), AmphiPodType.B },
  { new Point(2, 2), AmphiPodType.A },
  { new Point(4, 1), AmphiPodType.C },
  { new Point(4, 2), AmphiPodType.D },
  { new Point(6, 1), AmphiPodType.B },
  { new Point(6, 2), AmphiPodType.C },
  { new Point(8, 1), AmphiPodType.D },
  { new Point(8, 2), AmphiPodType.A }
};*/

var dict = new Dictionary<Point, AmphiPodType> {
  { new Point(2, 1), AmphiPodType.D },
  { new Point(2, 2), AmphiPodType.C },
  { new Point(4, 1), AmphiPodType.B },
  { new Point(4, 2), AmphiPodType.C },
  { new Point(6, 1), AmphiPodType.D },
  { new Point(6, 2), AmphiPodType.A },
  { new Point(8, 1), AmphiPodType.A },
  { new Point(8, 2), AmphiPodType.B }
};

var board = new Board(dict);

var results = new HashSet<int>();
var history = new HashSet<string>();
Step(board, 0, new Point(0, 0),0, ref results, ref history);

Console.Out.WriteLine(results.Min());

var dict2 = new Dictionary<Point, AmphiPodType> {
  { new Point(2, 1), AmphiPodType.D },
  { new Point(2, 2), AmphiPodType.D },
  { new Point(2, 3), AmphiPodType.D },
  { new Point(2, 4), AmphiPodType.C },
  { new Point(4, 1), AmphiPodType.B },
  { new Point(4, 2), AmphiPodType.C },
  { new Point(4, 3), AmphiPodType.B },
  { new Point(4, 4), AmphiPodType.C },
  { new Point(6, 1), AmphiPodType.D },
  { new Point(6, 2), AmphiPodType.B },
  { new Point(6, 3), AmphiPodType.A },
  { new Point(6, 4), AmphiPodType.A },
  { new Point(8, 1), AmphiPodType.A },
  { new Point(8, 2), AmphiPodType.A },
  { new Point(8, 3), AmphiPodType.C },
  { new Point(8, 4), AmphiPodType.B }
};


void Step(Board b, int energy, Point lastMove, int level, ref HashSet<int> solutions, ref HashSet<string> hist) {
  var boardString = b.ToString();

  if (hist.Contains(boardString)) return;
  
  if (level == 1) {
    PrintBoard(b);
    Console.Out.WriteLine();
  }

  if (solutions.Any() && solutions.Min() < energy) return;
  
  if (b.IsFinished()) {
    if (!solutions.Any() || energy < solutions.Min()) {
      Console.Out.WriteLine(energy);
    }
    solutions.Add(energy);
    return;
  }

  foreach (var pod in b.AmphiPods.Where(p => p.Key != lastMove && !IsInEndPosition(b, p.Key, p.Value)).Reverse()) {
    if (IsRoom(pod.Key)) {
      var availableNewPoints = b.AvailablePoints().Where(p => !IsExit(p) && (IsCorrectRoom(p, pod.Value) || IsCorridor(p)));
      foreach (var newPoint in availableNewPoints) {
        if (!CanMove(b, pod.Key, newPoint)) continue;
        var (newBoard, newEnergy) = b.Move(pod.Key, newPoint);
        Step(newBoard, energy + newEnergy, newPoint, level + 1, ref solutions, ref hist);
      }
    }
    else {
      var availableNewPoint = b.AvailablePoints().Where(p => IsCorrectRoom(p, pod.Value)).ToList();
      if (availableNewPoint.Any()) {
        var newPoint = availableNewPoint.Single(x => x.Y == availableNewPoint.Max(p => p.Y));
        if (!CanMove(b, pod.Key, newPoint) || !b.IsRoomAvailable(pod.Value)) continue;
        var (newBoard, newEnergy) = b.Move(pod.Key, newPoint);
        Step(newBoard, energy + newEnergy, newPoint, level + 1, ref solutions, ref hist);
      }
    }
  }
  hist.Add(boardString);
}

void PrintBoard(Board b) {
  for (var y = 0; y <= 2; y++) {
    for (var x = 0; x <= 10; x++) {
      var point = new Point(x, y);
      if (b.Points.Contains(point)) {
        if (b.AmphiPods.ContainsKey(point)) {
          Console.Out.Write(b.AmphiPods[point]);
        }
        else {
          Console.Out.Write(".");
        }
      }
      else {
        Console.Out.Write(" ");
      }
    }

    Console.Out.WriteLine();
  }
}

bool CanMove(Board b, Point pod, Point destination) {
  var moves = new List<Point>();
  var y = pod.Y;
  var x = pod.X;
  var destX = destination.X;
  var destY = destination.Y;
  while (y > 0) {
    y--;
    moves.Add(new Point(x, y));
  }

  while (x != destX) {
    x = x > destX ? x - 1 : x + 1;
    moves.Add(new Point(x, y));
  }

  while (y < destY) {
    y++;
    moves.Add(new Point(x, y));
  }

  return moves.All(p => !b.AmphiPods.ContainsKey(p));
}

bool IsInEndPosition(Board b, Point p, AmphiPodType type) {
  return type switch {
    AmphiPodType.A => IsRoomA(p) && p.Y == 2 || b.AmphiPods.ContainsKey(new Point(2, 2)) &&
      b.AmphiPods[new Point(2, 2)] == AmphiPodType.A && IsRoomA(p) && p.Y == 1,
    AmphiPodType.B => IsRoomB(p) && p.Y == 2 || b.AmphiPods.ContainsKey(new Point(4, 2)) &&
      b.AmphiPods[new Point(4, 2)] == AmphiPodType.B && IsRoomB(p) && p.Y == 1,
    AmphiPodType.C => IsRoomC(p) && p.Y == 2 || b.AmphiPods.ContainsKey(new Point(6, 2)) &&
      b.AmphiPods[new Point(6, 2)] == AmphiPodType.C && IsRoomC(p) && p.Y == 1,
    AmphiPodType.D => IsRoomD(p) && p.Y == 2 || b.AmphiPods.ContainsKey(new Point(8, 2)) &&
      b.AmphiPods[new Point(8, 2)] == AmphiPodType.D && IsRoomD(p) && p.Y == 1,
    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
  };
}

bool IsCorrectRoom(Point p, AmphiPodType type) {
  return type switch {
    AmphiPodType.A => IsRoomA(p),
    AmphiPodType.B => IsRoomB(p),
    AmphiPodType.C => IsRoomC(p),
    AmphiPodType.D => IsRoomD(p),
    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
  };
}

bool IsCorridor(Point p) {
  return p.Y == 0;
}

bool IsExit(Point p) {
  return p.Y == 0 && p.X is 2 or 4 or 6 or 8;
}

bool IsRoom(Point p) {
  return IsRoomA(p) || IsRoomB(p) || IsRoomC(p) || IsRoomD(p);
}

bool IsRoomA(Point p) {
  return p.Y > 0 && p.X == 2;
}

bool IsRoomB(Point p) {
  return p.Y > 0 && p.X == 4;
}

bool IsRoomC(Point p) {
  return p.Y > 0 && p.X == 6;
}

bool IsRoomD(Point p) {
  return p.Y > 0 && p.X == 8;
}

internal class Board {
  public readonly List<Point> Points;
  public Dictionary<Point, AmphiPodType> AmphiPods;

  public List<Point> AvailablePoints() {
    return Points.Except(AmphiPods.Keys).ToList();
  }

  public bool IsFinished() {
    if (AmphiPods.Keys.Any(p => p.Y == 0)) return false;
    return AmphiPods.All(p => {
      return p.Value switch {
        AmphiPodType.A => p.Key.X == 2,
        AmphiPodType.B => p.Key.X == 4,
        AmphiPodType.C => p.Key.X == 6,
        AmphiPodType.D => p.Key.X == 8,
        _ => throw new ArgumentOutOfRangeException()
      };
    });
  }

  public bool IsRoomAvailable(AmphiPodType type) {
    return type switch {
      AmphiPodType.A => AmphiPods.Where(x => x.Key.X == 2).All(p => p.Value == AmphiPodType.A) &&
                        AmphiPods.Count(x => x.Key.X == 2) < 2,
      AmphiPodType.B => AmphiPods.Where(x => x.Key.X == 4).All(p => p.Value == AmphiPodType.B) &&
                        AmphiPods.Count(x => x.Key.X == 4) < 2,
      AmphiPodType.C => AmphiPods.Where(x => x.Key.X == 6).All(p => p.Value == AmphiPodType.C) &&
                        AmphiPods.Count(x => x.Key.X == 6) < 2,
      AmphiPodType.D => AmphiPods.Where(x => x.Key.X == 8).All(p => p.Value == AmphiPodType.D) &&
                        AmphiPods.Count(x => x.Key.X == 8) < 2,
      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
  }

  public (Board, int) Move(Point pod, Point newPos) {
    var dict = new Dictionary<Point, AmphiPodType>();
    foreach (var (key, value) in AmphiPods) {
      dict.Add(key, value);
    }

    var podType = dict[pod];
    var costs = CalcCosts(pod, podType, newPos);
    dict.Remove(pod);
    dict.Add(newPos, podType);
    return (new Board(dict), costs);
  }

  public override string ToString() {
    return string.Join("", AmphiPods.OrderBy(x => x.Key.Y).ThenBy(x => x.Key.X)
      .Select(x => $"{x.Value}{x.Key.X}{x.Key.Y}"));
  }

  private static int CalcCosts(Point pod, AmphiPodType podType, Point newPos) {
    var length = pod.Y + Math.Abs(newPos.X - pod.X) + newPos.Y;
    return podType switch {
      AmphiPodType.A => length,
      AmphiPodType.B => length * 10,
      AmphiPodType.C => length * 100,
      AmphiPodType.D => length * 1000,
      _ => throw new ArgumentOutOfRangeException(nameof(podType), podType, null)
    };
  }

  public Board(Dictionary<Point, AmphiPodType> pods) {
    
    Points = Enumerable.Range(0, 11).Select(x => new Point(x, 0)).ToList();
    Points.AddRange(Enumerable.Range(1, 4).SelectMany(x => Enumerable.Range(1, 2).Select(index => new Point(2 * x, index))));
    AmphiPods = pods;
  }
}

enum AmphiPodType {
  A,
  B,
  C,
  D
}