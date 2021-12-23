using System.Drawing;

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

var board = new Board(dict, 2);

var results = new HashSet<int>();
var history = new Dictionary<string, int>();
//Step(board, 0, 0, ref results, ref history);

//Console.Out.WriteLine(results.Min());

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

var board2 = new Board(dict2, 4);

var results2 = new HashSet<int>();
var history2 = new Dictionary<string, int>();
Step(board2, 0, 0, ref results2, ref history2);

Console.Out.WriteLine(results2.Min());

void Step(Board b, int energy, int level, ref HashSet<int> solutions, ref Dictionary<string, int> hist) {
  var boardString = b.ToString();

  if (energy > 42106) return;
  
  if (hist.ContainsKey(boardString)) {
    if (hist[boardString] < energy) {
      return;
    }
    hist[boardString] = energy;
  }
  else {
    hist[boardString] = energy;
  }
  
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

  foreach (var pod in b.Amphipods.Where(p => !IsInEndPosition(b, p.Key, p.Value) && CanMove(b, p.Key))) {
    if (IsRoom(pod.Key)) {
      var availableNewPoints = b.AvailablePoints().Where(p => !IsExit(p) && (IsCorrectRoom(p, pod.Value) || IsCorridor(p)));
      foreach (var newPoint in availableNewPoints.Where(np => CanMoveTo(b, pod.Key, np))) {
        var (newBoard, newEnergy) = b.Move(pod.Key, newPoint);
        Step(newBoard, energy + newEnergy, level + 1, ref solutions, ref hist);
      }
    }
    else {
      var availableNewPoint = b.AvailablePoints().Where(p => IsCorrectRoom(p, pod.Value)).ToList();
      if (availableNewPoint.Any()) {
        var newPoint = availableNewPoint.Single(x => x.Y == availableNewPoint.Max(p => p.Y));
        if (!CanMoveTo(b, pod.Key, newPoint) || !b.IsRoomAvailable(pod.Value)) continue;
        var (newBoard, newEnergy) = b.Move(pod.Key, newPoint);
        Step(newBoard, energy + newEnergy, level + 1, ref solutions, ref hist);
      }
    }
  }
}

bool CanMove(Board b, Point p) {
  return IsCorridor(p) || b.Amphipods.Where(a => a.Key.X == p.X).Min(a => a.Key.Y) == p.Y;
}

void PrintBoard(Board b) {
  for (var y = 0; y <= b.RoomSize; y++) {
    for (var x = 0; x <= 10; x++) {
      var point = new Point(x, y);
      if (b.Points.Contains(point)) {
        if (b.Amphipods.ContainsKey(point)) {
          Console.Out.Write(b.Amphipods[point]);
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

bool CanMoveTo(Board b, Point start, Point end) {
  var moves = new List<Point>();
  var y = start.Y;
  var x = start.X;
  var destX = end.X;
  var destY = end.Y;
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

  return moves.All(p => !b.Amphipods.ContainsKey(p));
}

bool IsInEndPosition(Board b, Point p, AmphiPodType type) {
  return type switch {
    AmphiPodType.A => IsRoomA(p) && b.Amphipods.Where(x => IsRoomA(x.Key) && x.Key.Y > p.Y).All(x => x.Value == AmphiPodType.A),
    AmphiPodType.B => IsRoomB(p) && b.Amphipods.Where(x => IsRoomB(x.Key) && x.Key.Y > p.Y).All(x => x.Value == AmphiPodType.B),
    AmphiPodType.C => IsRoomC(p) && b.Amphipods.Where(x => IsRoomC(x.Key) && x.Key.Y > p.Y).All(x => x.Value == AmphiPodType.C),
    AmphiPodType.D => IsRoomD(p) && b.Amphipods.Where(x => IsRoomD(x.Key) && x.Key.Y > p.Y).All(x => x.Value == AmphiPodType.D),
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
  public IDictionary<Point, AmphiPodType> Amphipods;
  public readonly int RoomSize;
  
  public IEnumerable<Point> AvailablePoints() {
    return Points.Except(Amphipods.Keys).ToList();
  }

  public bool IsFinished() {
    if (Amphipods.Keys.Any(p => p.Y == 0)) return false;
    return Amphipods.All(p => {
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
      AmphiPodType.A => Amphipods.Where(x => x.Key.X == 2).All(p => p.Value == AmphiPodType.A) &&
                        Amphipods.Count(x => x.Key.X == 2) < RoomSize,
      AmphiPodType.B => Amphipods.Where(x => x.Key.X == 4).All(p => p.Value == AmphiPodType.B) &&
                        Amphipods.Count(x => x.Key.X == 4) < RoomSize,
      AmphiPodType.C => Amphipods.Where(x => x.Key.X == 6).All(p => p.Value == AmphiPodType.C) &&
                        Amphipods.Count(x => x.Key.X == 6) < RoomSize,
      AmphiPodType.D => Amphipods.Where(x => x.Key.X == 8).All(p => p.Value == AmphiPodType.D) &&
                        Amphipods.Count(x => x.Key.X == 8) < RoomSize,
      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
  }

  public (Board, int) Move(Point pod, Point newPos) {
    var dict = new Dictionary<Point, AmphiPodType>();
    foreach (var (key, value) in Amphipods) {
      dict.Add(key, value);
    }

    var podType = dict[pod];
    var costs = CalcCosts(pod, podType, newPos);
    dict.Remove(pod);
    dict.Add(newPos, podType);
    return (new Board(dict, RoomSize), costs);
  }

  public override string ToString() {
    return string.Join(";", Amphipods.OrderBy(x => x.Key.Y).ThenBy(x => x.Key.X)
      .Select(x => $"{x.Value}_{x.Key.X}_{x.Key.Y}"));
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

  public Board(Dictionary<Point, AmphiPodType> pods, int roomSize) {
    RoomSize = roomSize;
    Points = Enumerable.Range(0, 11).Select(x => new Point(x, 0)).ToList();
    Points.AddRange(Enumerable.Range(1, 4).SelectMany(x => Enumerable.Range(1, RoomSize).Select(index => new Point(2 * x, index))));
    Amphipods = pods;
  }
}

enum AmphiPodType {
  A,
  B,
  C,
  D
}