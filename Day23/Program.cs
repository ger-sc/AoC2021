using System.Drawing;

var dict = new Dictionary<Point, PodType> {
  { new Point(2, 1), PodType.D },
  { new Point(2, 2), PodType.C },
  { new Point(4, 1), PodType.B },
  { new Point(4, 2), PodType.C },
  { new Point(6, 1), PodType.D },
  { new Point(6, 2), PodType.A },
  { new Point(8, 1), PodType.A },
  { new Point(8, 2), PodType.B }
};

var board = new Board(dict, 2);

var results = new SortedSet<int> { int.MaxValue };
var history = new Dictionary<string, int>();

Step(board, 0, new Point(0, 0), GetPoints(2), ref results, ref history);

Console.Out.WriteLine("Part1: "+results.Min());

var dict2 = new Dictionary<Point, PodType> {
  { new Point(2, 1), PodType.D },
  { new Point(2, 2), PodType.D },
  { new Point(2, 3), PodType.D },
  { new Point(2, 4), PodType.C },
  { new Point(4, 1), PodType.B },
  { new Point(4, 2), PodType.C },
  { new Point(4, 3), PodType.B },
  { new Point(4, 4), PodType.C },
  { new Point(6, 1), PodType.D },
  { new Point(6, 2), PodType.B },
  { new Point(6, 3), PodType.A },
  { new Point(6, 4), PodType.A },
  { new Point(8, 1), PodType.A },
  { new Point(8, 2), PodType.A },
  { new Point(8, 3), PodType.C },
  { new Point(8, 4), PodType.B }
};

var board2 = new Board(dict2, 4);

var results2 = new SortedSet<int> { int.MaxValue };
var history2 = new Dictionary<string, int>();
Step(board2, 0, new Point(0, 0), GetPoints(4), ref results2, ref history2);

Console.Out.WriteLine("Part2: "+results2.Min());

void Step(Board b, int energy, Point lastMove, IList<Point> pointList, ref SortedSet<int> solutions,
  ref Dictionary<string, int> hist) {
  if (b.IsFinished()) {
    solutions.Add(energy);
    return;
  }

  var boardHash = b.ToString();
  if (hist.ContainsKey(boardHash)) {
    if (hist[boardHash] < energy) {
      return;
    }
    hist[boardHash] = energy;
  }
  else {
    hist[boardHash] = energy;
  }

  if (solutions.Any() && solutions.Min() < energy) return;

  foreach (var (key, value) in b.Pods.Where(p => !IsInEndPosition(b, p.Key, p.Value) && CanMove(b, p.Key) && p.Key != lastMove)) {
    if (IsRoom(key)) {
      var availableNewPoints = pointList.Except(b.Pods.Keys).Where(p =>
        !IsExit(p) && (IsCorrectRoom(p, value) && b.IsRoomAvailable(value) || IsCorridor(p)));
      foreach (var newPoint in availableNewPoints.Where(np => CanMoveTo(b, key, np))) {
        var (newBoard, newEnergy) = b.Move(key, newPoint);
        Step(newBoard, energy + newEnergy, newPoint, pointList, ref solutions, ref hist);
      }
    }
    else {
      var availableNewPoint = pointList.Except(b.Pods.Keys).Where(p => IsCorrectRoom(p, value)).ToList();
      if (availableNewPoint.Any()) {
        var newPoint = availableNewPoint.Single(x => x.Y == availableNewPoint.Max(p => p.Y));
        if (!CanMoveTo(b, key, newPoint) || !b.IsRoomAvailable(value)) continue;
        var (newBoard, newEnergy) = b.Move(key, newPoint);
        Step(newBoard, energy + newEnergy, newPoint, pointList, ref solutions, ref hist);
      }
    }
  }
}

IList<Point> GetPoints(int roomSize) {
  return Enumerable.Range(0, 11)
    .Select(x => new Point(x, 0))
    .ToList()
    .Concat(Enumerable.Range(1, 4)
      .SelectMany(x => Enumerable.Range(1, roomSize)
        .Select(index => new Point(2 * x, index))))
    .ToList();
}

bool CanMove(Board b, Point p) {
  return IsCorridor(p) || b.Pods.Where(a => a.Key.X == p.X).Min(a => a.Key.Y) == p.Y;
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

  return moves.All(p => !b.Pods.ContainsKey(p));
}

bool IsInEndPosition(Board b, Point p, PodType type) {
  return type switch {
    PodType.A => IsRoomA(p) && b.Pods.Where(x => IsRoomA(x.Key) && x.Key.Y > p.Y).All(x => x.Value == PodType.A),
    PodType.B => IsRoomB(p) && b.Pods.Where(x => IsRoomB(x.Key) && x.Key.Y > p.Y).All(x => x.Value == PodType.B),
    PodType.C => IsRoomC(p) && b.Pods.Where(x => IsRoomC(x.Key) && x.Key.Y > p.Y).All(x => x.Value == PodType.C),
    PodType.D => IsRoomD(p) && b.Pods.Where(x => IsRoomD(x.Key) && x.Key.Y > p.Y).All(x => x.Value == PodType.D),
    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
  };
}

bool IsCorrectRoom(Point p, PodType type) {
  return type switch {
    PodType.A => IsRoomA(p),
    PodType.B => IsRoomB(p),
    PodType.C => IsRoomC(p),
    PodType.D => IsRoomD(p),
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
  public readonly IDictionary<Point, PodType> Pods;
  private readonly int _roomSize;

  public bool IsFinished() {
    if (Pods.Keys.Any(p => p.Y == 0)) return false;
    return Pods.All(p => {
      return p.Value switch {
        PodType.A => p.Key.X == 2,
        PodType.B => p.Key.X == 4,
        PodType.C => p.Key.X == 6,
        PodType.D => p.Key.X == 8,
        _ => throw new ArgumentOutOfRangeException()
      };
    });
  }

  public bool IsRoomAvailable(PodType type) {
    return type switch {
      PodType.A => Pods.Where(x => x.Key.X == 2)
        .All(p => p.Value == PodType.A) && Pods.Count(x => x.Key.X == 2) < _roomSize,
      PodType.B => Pods.Where(x => x.Key.X == 4)
        .All(p => p.Value == PodType.B) && Pods.Count(x => x.Key.X == 4) < _roomSize,
      PodType.C => Pods.Where(x => x.Key.X == 6)
        .All(p => p.Value == PodType.C) && Pods.Count(x => x.Key.X == 6) < _roomSize,
      PodType.D => Pods.Where(x => x.Key.X == 8)
        .All(p => p.Value == PodType.D) && Pods.Count(x => x.Key.X == 8) < _roomSize,
      _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
  }

  public (Board, int) Move(Point pod, Point newPos) {
    var dict = new Dictionary<Point, PodType>();
    foreach (var (key, value) in Pods) {
      dict.Add(key, value);
    }

    var podType = dict[pod];
    var costs = CalcCosts(pod, podType, newPos);
    dict.Remove(pod);
    dict.Add(newPos, podType);
    return (new Board(dict, _roomSize), costs);
  }

  private static int CalcCosts(Point pod, PodType podType, Point newPos) {
    var length = pod.Y + Math.Abs(newPos.X - pod.X) + newPos.Y;
    return length * (int)podType;
  }

  public override string ToString() {
    return string.Join(",", Pods
      .OrderBy(x => x.Key.Y)
      .ThenBy(x => x.Key.X)
      .Select(x => x.Key.Y + "_" + x.Key.X + "_" + x.Value));
  }

  public Board(IDictionary<Point, PodType> pods, int roomSize) {
    _roomSize = roomSize;
    Pods = pods;
  }
}

internal enum PodType {
  A = 1,
  B = 10,
  C = 100,
  D = 1000
}